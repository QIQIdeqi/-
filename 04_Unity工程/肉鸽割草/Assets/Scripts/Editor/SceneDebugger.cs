using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 场景调试工具 - 查找损坏的对象和空引用
    /// </summary>
    public class SceneDebugger : EditorWindow
    {
        private Vector2 scrollPos;
        private List<string> debugLog = new List<string>();
        
        [MenuItem("绒毛几何物语/工具/场景调试")]
        public static void ShowWindow()
        {
            GetWindow<SceneDebugger>("场景调试");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🐛 场景调试工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "此工具帮助查找场景中的问题：\n" +
                "• 空引用对象\n" +
                "• 损坏的组件\n" +
                "• Missing脚本\n" +
                "• 重复的GameManager", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("扫描当前场景", GUILayout.Height(40)))
            {
                ScanScene();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("清理空对象", GUILayout.Height(30)))
            {
                CleanEmptyObjects();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("修复GameManager", GUILayout.Height(30)))
            {
                FixGameManager();
            }
            
            EditorGUILayout.Space();
            
            // 显示日志
            GUILayout.Label("调试日志:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
            foreach (var log in debugLog)
            {
                EditorGUILayout.LabelField(log, EditorStyles.wordWrappedLabel);
            }
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("清除日志"))
            {
                debugLog.Clear();
            }
        }
        
        private void Log(string message)
        {
            debugLog.Add($"[{System.DateTime.Now:HH:mm:ss}] {message}");
            Debug.Log($"[SceneDebugger] {message}");
        }
        
        private void ScanScene()
        {
            debugLog.Clear();
            Log("=== 开始扫描场景 ===");
            
            // 1. 检查GameManager
            var gameManagers = FindObjectsOfType<GameManager>();
            Log($"发现 {gameManagers.Length} 个 GameManager");
            foreach (var gm in gameManagers)
            {
                Log($"  - {gm.name}: {(gm.gameObject != null ? "OK" : "NULL OBJECT")}");
                
                // 检查UI引用
                var so = new SerializedObject(gm);
                var mainMenuProp = so.FindProperty("mainMenuPanel");
                var hudProp = so.FindProperty("hudCanvas");
                var gameOverProp = so.FindProperty("gameOverPanel");
                
                Log($"    mainMenuPanel: {(mainMenuProp.objectReferenceValue != null ? "OK" : "NULL")}");
                Log($"    hudCanvas: {(hudProp.objectReferenceValue != null ? "OK" : "NULL")}");
                Log($"    gameOverPanel: {(gameOverProp.objectReferenceValue != null ? "OK" : "NULL")}");
            }
            
            // 2. 检查UI面板
            var mainMenus = FindObjectsOfType<MainMenuPanel>(true);
            Log($"发现 {mainMenus.Length} 个 MainMenuPanel");
            
            var gameOverPanels = FindObjectsOfType<GameOverPanel>(true);
            Log($"发现 {gameOverPanels.Length} 个 GameOverPanel");
            
            var huds = FindObjectsOfType<GameHUD>(true);
            Log($"发现 {huds.Length} 个 GameHUD");
            
            // 3. 检查损坏的组件
            var allObjects = FindObjectsOfType<GameObject>(true);
            int missingCount = 0;
            foreach (var go in allObjects)
            {
                if (go == null) continue;
                
                var components = go.GetComponents<Component>();
                foreach (var comp in components)
                {
                    if (comp == null)
                    {
                        Log($"⚠️ {go.name} 有 Missing 组件!");
                        missingCount++;
                    }
                }
            }
            
            if (missingCount == 0)
            {
                Log("✅ 没有发现 Missing 组件");
            }
            else
            {
                Log($"⚠️ 共发现 {missingCount} 个 Missing 组件");
            }
            
            // 4. 检查空对象
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            int emptyCount = 0;
            foreach (var root in rootObjects)
            {
                CheckEmptyRecursive(root, ref emptyCount);
            }
            
            Log($"发现 {emptyCount} 个空对象（无组件或只有Transform）");
            
            Log("=== 扫描完成 ===");
        }
        
        private void CheckEmptyRecursive(GameObject go, ref int count)
        {
            if (go == null) return;
            
            var components = go.GetComponents<Component>();
            if (components.Length <= 1) // 只有Transform
            {
                if (go.transform.childCount == 0) // 没有子物体
                {
                    Log($"  空对象: {go.name}");
                    count++;
                }
            }
            
            foreach (Transform child in go.transform)
            {
                CheckEmptyRecursive(child.gameObject, ref count);
            }
        }
        
        private void CleanEmptyObjects()
        {
            int deletedCount = 0;
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            
            foreach (var root in rootObjects)
            {
                deletedCount += CleanEmptyRecursive(root);
            }
            
            Log($"已删除 {deletedCount} 个空对象");
        }
        
        private int CleanEmptyRecursive(GameObject go)
        {
            int count = 0;
            
            // 先清理子物体
            var children = new List<GameObject>();
            foreach (Transform child in go.transform)
            {
                children.Add(child.gameObject);
            }
            
            foreach (var child in children)
            {
                count += CleanEmptyRecursive(child);
            }
            
            // 检查当前物体是否为空
            if (go.transform.childCount == 0) // 子物体已清理
            {
                var components = go.GetComponents<Component>();
                if (components.Length <= 1) // 只有Transform
                {
                    Undo.DestroyObjectImmediate(go);
                    count++;
                }
            }
            
            return count;
        }
        
        private void FixGameManager()
        {
            var gameManagers = FindObjectsOfType<GameManager>();
            
            if (gameManagers.Length == 0)
            {
                Log("❌ 场景中没有 GameManager!");
                return;
            }
            
            // 保留第一个，删除其他的
            for (int i = 1; i < gameManagers.Length; i++)
            {
                Undo.DestroyObjectImmediate(gameManagers[i].gameObject);
                Log($"已删除多余的 GameManager: {gameManagers[i].name}");
            }
            
            // 重新绑定第一个的引用
            var gm = gameManagers[0];
            var so = new SerializedObject(gm);
            
            // 查找场景中的UI
            var mainMenu = FindObjectOfType<MainMenuPanel>(true);
            var gameOver = FindObjectOfType<GameOverPanel>(true);
            var hud = FindObjectOfType<GameHUD>(true);
            
            so.FindProperty("mainMenuPanel").objectReferenceValue = mainMenu;
            so.FindProperty("gameOverPanel").objectReferenceValue = gameOver;
            so.FindProperty("hudCanvas").objectReferenceValue = hud != null ? hud.gameObject : null;
            
            so.ApplyModifiedProperties();
            
            Log("✅ 已修复 GameManager 引用");
        }
    }
}
