using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using GeometryWarrior;
using FluffyGeometry.UI;
using FluffyGeometry.Home;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 家园系统清理整合工具
    /// 自动清理冗余UI、整合重复资源
    /// </summary>
    public class HomeSystemCleanupTool : EditorWindow
    {
        private Vector2 scrollPos;
        private List<string> cleanupLog = new List<string>();
        private bool showDetails = true;
        
        [MenuItem("绒毛几何物语/工具/家园系统清理整合")]
        public static void ShowWindow()
        {
            GetWindow<HomeSystemCleanupTool>("家园系统清理");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🧹 家园系统清理整合工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "此工具会自动清理家园系统中的冗余内容：\n" +
                "• 删除 JoystickPanel 中的 BackpackButton\n" +
                "• 清理场景中重复的 UI 元素\n" +
                "• 更新旧的引用和配置\n" +
                "• 生成整合报告", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            showDetails = EditorGUILayout.Toggle("显示详细日志", showDetails);
            
            EditorGUILayout.Space();
            
            GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
            if (GUILayout.Button("开始清理整合", GUILayout.Height(50)))
            {
                if (EditorUtility.DisplayDialog("确认", 
                    "清理操作会修改场景和预制体，建议先保存项目！\n\n是否继续？", 
                    "继续", "取消"))
                {
                    StartCleanup();
                }
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("仅检查（不清理）", GUILayout.Height(30)))
            {
                CheckOnly();
            }
            
            EditorGUILayout.Space();
            
            if (showDetails && cleanupLog.Count > 0)
            {
                GUILayout.Label("操作日志:", EditorStyles.boldLabel);
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));
                foreach (var log in cleanupLog)
                {
                    EditorGUILayout.LabelField(log, EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.EndScrollView();
            }
        }
        
        private void Log(string message)
        {
            cleanupLog.Add($"[{System.DateTime.Now:HH:mm:ss}] {message}");
            Debug.Log($"[HomeSystemCleanup] {message}");
        }
        
        private void StartCleanup()
        {
            cleanupLog.Clear();
            Log("=== 开始清理整合 ===");
            
            int totalChanges = 0;
            
            try
            {
                // 1. 清理 JoystickPanel
                totalChanges += CleanupJoystickPanel();
                
                // 2. 清理场景中的重复UI
                totalChanges += CleanupDuplicateUIInScenes();
                
                // 3. 更新预制体引用
                totalChanges += UpdatePrefabReferences();
                
                // 4. 删除旧的冗余脚本
                totalChanges += RemoveObsoleteScripts();
                
                Log($"=== 清理完成，共 {totalChanges} 处修改 ===");
                
                EditorUtility.DisplayDialog("完成", 
                    $"清理整合完成！\n\n共处理 {totalChanges} 处冗余内容。\n\n建议保存场景并测试游戏。", "确定");
            }
            catch (System.Exception e)
            {
                Log($"❌ 错误: {e.Message}");
                EditorUtility.DisplayDialog("错误", $"清理过程中出错:\n{e.Message}", "确定");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private void CheckOnly()
        {
            cleanupLog.Clear();
            Log("=== 检查模式（不会修改任何内容） ===");
            
            int issues = 0;
            
            // 检查 JoystickPanel
            string[] joystickPanels = AssetDatabase.FindAssets("t:Prefab JoystickPanel", new[] { "Assets" });
            foreach (var guid in joystickPanels)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    Transform backpackBtn = prefab.transform.Find("BackpackButton");
                    if (backpackBtn != null)
                    {
                        Log($"⚠️ 发现冗余: {path} 中包含 BackpackButton");
                        issues++;
                    }
                }
            }
            
            // 检查场景中的重复UI
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                
                var rootObjects = scene.GetRootGameObjects();
                int homeHUDCount = 0;
                int backpackPanelCount = 0;
                
                foreach (var root in rootObjects)
                {
                    homeHUDCount += root.GetComponentsInChildren<HomeHUD>(true).Length;
                    backpackPanelCount += root.GetComponentsInChildren<BackpackPanel>(true).Length;
                }
                
                if (homeHUDCount > 1)
                {
                    Log($"⚠️ 场景 {scene.name} 中有 {homeHUDCount} 个 HomeHUD（建议保留1个）");
                    issues++;
                }
                
                if (backpackPanelCount > 1)
                {
                    Log($"⚠️ 场景 {scene.name} 中有 {backpackPanelCount} 个 BackpackPanel（建议保留1个）");
                    issues++;
                }
            }
            
            Log($"=== 检查完成，发现 {issues} 处需要处理 ===");
        }
        
        #region 清理方法
        
        private int CleanupJoystickPanel()
        {
            int count = 0;
            Log("清理 JoystickPanel...");
            
            string[] joystickPanels = AssetDatabase.FindAssets("t:Prefab JoystickPanel", new[] { "Assets" });
            
            foreach (var guid in joystickPanels)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab == null) continue;
                
                // 检查并删除 BackpackButton
                Transform backpackBtn = prefab.transform.Find("BackpackButton");
                if (backpackBtn != null)
                {
                    DestroyImmediate(backpackBtn.gameObject, true);
                    Log($"  ✅ 已删除: {path}/BackpackButton");
                    count++;
                }
                
                // 检查其他可能的冗余按钮
                string[] redundantNames = new[] { "BagButton", "PackButton", "InventoryButton" };
                foreach (var name in redundantNames)
                {
                    Transform redundant = prefab.transform.Find(name);
                    if (redundant != null)
                    {
                        DestroyImmediate(redundant.gameObject, true);
                        Log($"  ✅ 已删除: {path}/{name}");
                        count++;
                    }
                }
            }
            
            if (count == 0) Log("  ℹ️ JoystickPanel 无需清理");
            return count;
        }
        
        private int CleanupDuplicateUIInScenes()
        {
            int count = 0;
            Log("清理场景中的重复UI...");
            
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                
                var rootObjects = scene.GetRootGameObjects();
                
                // 收集所有 UI
                List<HomeHUD> homeHUDs = new List<HomeHUD>();
                List<BackpackPanel> backpackPanels = new List<BackpackPanel>();
                List<OutfitPanelNew> outfitPanels = new List<OutfitPanelNew>();
                
                foreach (var root in rootObjects)
                {
                    homeHUDs.AddRange(root.GetComponentsInChildren<HomeHUD>(true));
                    backpackPanels.AddRange(root.GetComponentsInChildren<BackpackPanel>(true));
                    outfitPanels.AddRange(root.GetComponentsInChildren<OutfitPanelNew>(true));
                }
                
                // 保留第一个 HomeHUD，删除其他的
                for (int j = 1; j < homeHUDs.Count; j++)
                {
                    if (homeHUDs[j] != null && homeHUDs[j].gameObject != null)
                    {
                        DestroyImmediate(homeHUDs[j].gameObject);
                        Log($"  ✅ 已删除场景 {scene.name} 中多余的 HomeHUD ({j})");
                        count++;
                    }
                }
                
                // 保留第一个 BackpackPanel，删除其他的
                for (int j = 1; j < backpackPanels.Count; j++)
                {
                    if (backpackPanels[j] != null && backpackPanels[j].gameObject != null)
                    {
                        DestroyImmediate(backpackPanels[j].gameObject);
                        Log($"  ✅ 已删除场景 {scene.name} 中多余的 BackpackPanel ({j})");
                        count++;
                    }
                }
                
                // 场景中的 OutfitPanelNew 应该是 BackpackPanel 的子物体，如果不是则删除
                foreach (var panel in outfitPanels)
                {
                    if (panel != null && panel.transform.parent != null)
                    {
                        var parentBackpack = panel.GetComponentInParent<BackpackPanel>();
                        if (parentBackpack == null)
                        {
                            // 这是一个独立的 OutfitPanel，应该删除
                            DestroyImmediate(panel.gameObject);
                            Log($"  ✅ 已删除场景 {scene.name} 中独立的 OutfitPanelNew");
                            count++;
                        }
                    }
                }
            }
            
            if (count == 0) Log("  ℹ️ 场景中无需清理重复UI");
            return count;
        }
        
        private int UpdatePrefabReferences()
        {
            int count = 0;
            Log("更新预制体引用...");
            
            // 查找所有 HomeHUD 预制体
            string[] homeHUDs = AssetDatabase.FindAssets("t:Prefab HomeHUD", new[] { "Assets" });
            foreach (var guid in homeHUDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                
                HomeHUD hud = prefab.GetComponent<HomeHUD>();
                if (hud == null) continue;
                
                // 清理序列化对象中的空引用
                SerializedObject so = new SerializedObject(hud);
                SerializedProperty prop = so.GetIterator();
                bool modified = false;
                
                while (prop.NextVisible(true))
                {
                    if (prop.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        // 检查是否是已删除的返回按钮引用
                        if (prop.name.Contains("return") || prop.name.Contains("Return"))
                        {
                            if (prop.objectReferenceValue != null)
                            {
                                prop.objectReferenceValue = null;
                                modified = true;
                                Log($"  ✅ 已清理: {path} 中的 {prop.name}");
                                count++;
                            }
                        }
                    }
                }
                
                if (modified)
                {
                    so.ApplyModifiedProperties();
                }
            }
            
            if (count == 0) Log("  ℹ️ 预制体引用无需更新");
            return count;
        }
        
        private int RemoveObsoleteScripts()
        {
            int count = 0;
            Log("检查废弃脚本...");
            
            // 列出已弃用的工具脚本
            string[] obsoleteScripts = new string[]
            {
                "BackpackSystemSetupWizard.cs",
                "BackpackUIReferenceSetupTool.cs",
                "BackpackPanelLayoutFixer.cs"
            };
            
            foreach (var scriptName in obsoleteScripts)
            {
                string[] guids = AssetDatabase.FindAssets($"t:Script {scriptName.Replace(".cs", "")}");
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path.Contains(scriptName))
                    {
                        Log($"  ℹ️ 发现已弃用脚本: {path}");
                        Log($"     这些脚本已被标记为弃用，可以安全删除");
                    }
                }
            }
            
            return count;
        }
        
        #endregion
    }
}
