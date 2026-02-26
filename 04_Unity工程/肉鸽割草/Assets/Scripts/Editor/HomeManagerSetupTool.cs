using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using GeometryWarrior;
using FluffyGeometry.Home;

namespace FluffyGeometry.Editor
{
    /// <summary>
    /// HomeManager一键配置工具
    /// 自动查找并配置HomeManager上的所有引用
    /// </summary>
    public class HomeManagerSetupTool : EditorWindow
    {
        private HomeManager targetHomeManager;
        private Vector2 scrollPosition;
        
        private string prefabPath = "Assets/Prefabs/UI/";
        
        // 配置状态
        private bool hasBackpackButtonPrefab = false;
        private bool hasFurnitureEditControllerPrefab = false;
        private bool hasPlayerPrefab = false;
        private bool hasPlayerSpawnPoint = false;
        private bool hasJoystick = false;
        
        [MenuItem("绒毛几何物语/HomeManager一键配置")]
        public static void ShowWindow()
        {
            var window = GetWindow<HomeManagerSetupTool>("HomeManager配置");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }
        
        private void OnGUI()
        {
            GUILayout.Space(10);
            
            // 标题
            GUIStyle titleStyle = new GUIStyle(EditorStyles.largeLabel);
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("🏠 HomeManager一键配置", titleStyle);
            
            GUILayout.Space(5);
            
            GUIStyle descStyle = new GUIStyle(EditorStyles.label);
            descStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("自动查找并配置所有引用", descStyle);
            
            GUILayout.Space(20);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            // 目标HomeManager
            GUILayout.Label("🎯 目标设置", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            targetHomeManager = EditorGUILayout.ObjectField(
                "HomeManager", 
                targetHomeManager, 
                typeof(HomeManager), 
                true
            ) as HomeManager;
            
            if (targetHomeManager == null)
            {
                EditorGUILayout.HelpBox(
                    "未找到HomeManager！\n" +
                    "1. 先在场景中选择一个带有HomeManager的物体\n" +
                    "2. 或手动拖入HomeManager引用", 
                    MessageType.Warning
                );
                
                if (GUILayout.Button("自动查找场景中的HomeManager"))
                {
                    AutoFindHomeManager();
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"已选中: {targetHomeManager.name}", MessageType.Info);
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            
            // 预制体路径设置
            GUILayout.Label("📁 预制体路径", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            prefabPath = EditorGUILayout.TextField("预制体查找路径", prefabPath);
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            
            // 当前状态
            if (targetHomeManager != null)
            {
                GUILayout.Label("📊 当前配置状态", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                CheckConfigurationStatus();
                
                EditorGUILayout.LabelField("背包按钮预制体", hasBackpackButtonPrefab ? "✅ 已配置" : "❌ 未配置");
                EditorGUILayout.LabelField("家具编辑控制器预制体", hasFurnitureEditControllerPrefab ? "✅ 已配置" : "❌ 未配置");
                EditorGUILayout.LabelField("玩家预制体", hasPlayerPrefab ? "✅ 已配置" : "❌ 未配置");
                EditorGUILayout.LabelField("玩家出生点", hasPlayerSpawnPoint ? "✅ 已配置" : "❌ 未配置");
                EditorGUILayout.LabelField("虚拟摇杆", hasJoystick ? "✅ 已配置" : "❌ 未配置");
                
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.Space(15);
            
            // 配置选项
            if (targetHomeManager != null)
            {
                GUILayout.Label("🔧 配置选项", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("将自动配置：", EditorStyles.boldLabel);
                GUILayout.Label("• 背包按钮预制体 (BackpackButton.prefab)");
                GUILayout.Label("• 家具编辑控制器预制体 (FurnitureEditController.prefab)");
                GUILayout.Label("• 玩家预制体 (自动查找)");
                GUILayout.Label("• 玩家出生点 (自动创建)");
                GUILayout.Label("• 虚拟摇杆 (自动查找)");
                GUILayout.Label("• 主菜单场景名称 (MainScene)");
                
                EditorGUILayout.EndVertical();
            }
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // 一键配置按钮
            GUI.enabled = targetHomeManager != null;
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fixedHeight = 50;
            
            if (GUILayout.Button("🚀 一键配置HomeManager", buttonStyle))
            {
                SetupHomeManager();
            }
            
            GUI.enabled = true;
            
            GUILayout.Space(10);
            
            // 说明
            EditorGUILayout.HelpBox(
                "使用步骤：\n" +
                "1. 确保已运行'背包系统一键配置'创建预制体\n" +
                "2. 点击'自动查找'或手动拖入HomeManager\n" +
                "3. 点击'一键配置'\n" +
                "4. 检查自动配置的参数是否正确", 
                MessageType.Info
            );
        }
        
        /// <summary>
        /// 自动查找场景中的HomeManager
        /// </summary>
        private void AutoFindHomeManager()
        {
            var homeManager = FindObjectOfType<HomeManager>();
            if (homeManager != null)
            {
                targetHomeManager = homeManager;
                EditorUtility.DisplayDialog("查找成功", 
                    $"找到HomeManager: {homeManager.name}", "确定");
                Selection.activeGameObject = homeManager.gameObject;
            }
            else
            {
                EditorUtility.DisplayDialog("未找到", 
                    "场景中没有找到HomeManager！\n" +
                    "请先在场景中创建一个空物体并添加HomeManager脚本。", "确定");
            }
        }
        
        /// <summary>
        /// 检查当前配置状态
        /// </summary>
        private void CheckConfigurationStatus()
        {
            if (targetHomeManager == null) return;
            
            // 使用反射获取私有字段
            var type = typeof(HomeManager);
            
            var backpackPrefabField = type.GetField("backpackButtonPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hasBackpackButtonPrefab = backpackPrefabField?.GetValue(targetHomeManager) != null;
            
            var editControllerField = type.GetField("furnitureEditControllerPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hasFurnitureEditControllerPrefab = editControllerField?.GetValue(targetHomeManager) != null;
            
            var playerPrefabField = type.GetField("playerPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hasPlayerPrefab = playerPrefabField?.GetValue(targetHomeManager) != null;
            
            var spawnPointField = type.GetField("playerSpawnPoint", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hasPlayerSpawnPoint = spawnPointField?.GetValue(targetHomeManager) != null;
            
            var joystickField = type.GetField("joystick", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            hasJoystick = joystickField?.GetValue(targetHomeManager) != null;
        }
        
        /// <summary>
        /// 配置HomeManager
        /// </summary>
        private void SetupHomeManager()
        {
            if (targetHomeManager == null) return;
            
            Undo.RecordObject(targetHomeManager, "Setup HomeManager");
            
            int configuredCount = 0;
            string log = "=== HomeManager配置日志 ===\n";
            
            // 1. 配置背包按钮预制体
            var backpackPrefab = LoadPrefab("BackpackButton.prefab");
            if (backpackPrefab != null)
            {
                SetPrivateField("backpackButtonPrefab", backpackPrefab);
                log += "✅ 背包按钮预制体\n";
                configuredCount++;
            }
            else
            {
                log += "❌ 背包按钮预制体 (未找到)\n";
            }
            
            // 2. 配置家具编辑控制器预制体
            var editControllerPrefab = LoadPrefab("FurnitureEditController.prefab");
            if (editControllerPrefab != null)
            {
                var editControllerComponent = editControllerPrefab.GetComponent<FurnitureEditController>();
                if (editControllerComponent != null)
                {
                    SetPrivateField("furnitureEditControllerPrefab", editControllerComponent);
                    log += "✅ 家具编辑控制器预制体\n";
                    configuredCount++;
                }
                else
                {
                    log += "❌ 家具编辑控制器预制体 (未找到组件)\n";
                }
            }
            else
            {
                log += "❌ 家具编辑控制器预制体 (未找到)\n";
            }
            
            // 3. 配置玩家预制体
            var playerPrefab = FindPlayerPrefab();
            if (playerPrefab != null)
            {
                SetPrivateField("playerPrefab", playerPrefab);
                log += $"✅ 玩家预制体 ({playerPrefab.name})\n";
                configuredCount++;
            }
            else
            {
                log += "❌ 玩家预制体 (未找到)\n";
            }
            
            // 4. 配置玩家出生点
            var spawnPoint = FindOrCreatePlayerSpawnPoint();
            if (spawnPoint != null)
            {
                SetPrivateField("playerSpawnPoint", spawnPoint);
                log += $"✅ 玩家出生点 ({spawnPoint.name})\n";
                configuredCount++;
            }
            
            // 5. 配置虚拟摇杆
            var joystick = FindJoystick();
            if (joystick != null)
            {
                SetPrivateField("joystick", joystick);
                log += $"✅ 虚拟摇杆 ({joystick.name})\n";
                configuredCount++;
            }
            else
            {
                log += "❌ 虚拟摇杆 (未找到，请在场景中添加)\n";
            }
            
            // 6. 配置主菜单场景名称
            SetPrivateField("mainMenuSceneName", "MainScene");
            log += "✅ 主菜单场景名称 (MainScene)\n";
            configuredCount++;
            
            EditorUtility.SetDirty(targetHomeManager);
            
            Debug.Log(log);
            
            EditorUtility.DisplayDialog("配置完成", 
                $"成功配置 {configuredCount} 项！\n\n" +
                $"{log}\n" +
                "请检查Console中的详细日志。", "确定");
        }
        
        /// <summary>
        /// 加载预制体
        /// </summary>
        private GameObject LoadPrefab(string prefabName)
        {
            string path = prefabPath + prefabName;
            if (File.Exists(path))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            
            // 尝试在其他路径查找
            string[] guids = AssetDatabase.FindAssets($"t:Prefab {Path.GetFileNameWithoutExtension(prefabName)}");
            foreach (string guid in guids)
            {
                string foundPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(foundPath);
                if (prefab != null && prefab.GetComponent<FluffyGeometry.UI.BackpackButton>() != null && 
                    prefabName.Contains("BackpackButton"))
                {
                    return prefab;
                }
                if (prefab != null && prefab.GetComponent<FluffyGeometry.Home.FurnitureEditController>() != null && 
                    prefabName.Contains("FurnitureEditController"))
                {
                    return prefab;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 查找玩家预制体
        /// </summary>
        private GameObject FindPlayerPrefab()
        {
            // 在Resources/Prefabs中查找
            string[] guids = AssetDatabase.FindAssets("t:Prefab Player");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.GetComponent<PlayerController>() != null)
                {
                    return prefab;
                }
            }
            
            // 查找任何带有PlayerController的预制体
            guids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && prefab.GetComponent<PlayerController>() != null)
                {
                    return prefab;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 查找或创建玩家出生点
        /// </summary>
        private Transform FindOrCreatePlayerSpawnPoint()
        {
            // 先查找现有的
            Transform existing = targetHomeManager.transform.Find("PlayerSpawnPoint");
            if (existing != null) return existing;
            
            // 查找场景中任何名为PlayerSpawnPoint的物体
            var spawnPointObj = GameObject.Find("PlayerSpawnPoint");
            if (spawnPointObj != null) return spawnPointObj.transform;
            
            // 创建新的
            GameObject newSpawnPoint = new GameObject("PlayerSpawnPoint");
            newSpawnPoint.transform.SetParent(targetHomeManager.transform);
            newSpawnPoint.transform.position = Vector3.zero;
            
            // 添加图标（可选）
            var icon = newSpawnPoint.AddComponent<DrawSpawnPointGizmo>();
            
            return newSpawnPoint.transform;
        }
        
        /// <summary>
        /// 查找虚拟摇杆
        /// </summary>
        private Joystick FindJoystick()
        {
            // 在场景根查找
            var joystick = FindObjectOfType<Joystick>();
            if (joystick != null) return joystick;
            
            // 查找Canvas下的摇杆
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                joystick = canvas.GetComponentInChildren<Joystick>(true);
                if (joystick != null) return joystick;
            }
            
            return null;
        }
        
        /// <summary>
        /// 设置私有字段
        /// </summary>
        private void SetPrivateField(string fieldName, object value)
        {
            var field = typeof(HomeManager).GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                field.SetValue(targetHomeManager, value);
            }
        }
    }
    
    /// <summary>
    /// 出生点Gizmo绘制
    /// </summary>
    public class DrawSpawnPointGizmo : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1f);
        }
    }
}
