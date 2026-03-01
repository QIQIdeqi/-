using UnityEngine;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// OutfitManager 场景检查工具
    /// </summary>
    public class OutfitManagerSceneChecker : EditorWindow
    {
        [MenuItem("绒毛几何物语/检查/OutfitManager")]
        public static void ShowWindow()
        {
            GetWindow<OutfitManagerSceneChecker>("检查 OutfitManager");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ OutfitManager 场景检查 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            // 检查场景中的 OutfitManager
            OutfitManager[] managers = FindObjectsOfType<OutfitManager>();
            
            if (managers.Length == 0)
            {
                EditorGUILayout.HelpBox(
                    "❌ 场景中没有 OutfitManager！\n\n" +
                    "这会导致装扮系统无法工作。", 
                    MessageType.Error);
                
                GUILayout.Space(10);
                
                GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                if (GUILayout.Button("创建 OutfitManager", GUILayout.Height(40)))
                {
                    CreateOutfitManager();
                }
                GUI.backgroundColor = Color.white;
            }
            else if (managers.Length == 1)
            {
                EditorGUILayout.HelpBox(
                    $"✅ 场景中有 1 个 OutfitManager\n" +
                    $"实例名称: {managers[0].name}\n" +
                    $"单例状态: {(OutfitManager.Instance != null ? "正常" : "未初始化")}", 
                    MessageType.Info);
                
                GUILayout.Space(10);
                
                // 显示部件数量
                var allParts = managers[0].GetAllParts();
                EditorGUILayout.LabelField($"已加载部件数: {allParts?.Count ?? 0}");
                
                GUILayout.Space(10);
                
                if (GUILayout.Button("选中 OutfitManager"))
                {
                    Selection.activeGameObject = managers[0].gameObject;
                    EditorGUIUtility.PingObject(managers[0].gameObject);
                }
                
                if (GUILayout.Button("重新加载部件数据"))
                {
                    // 通过反射调用私有方法
                    var method = typeof(OutfitManager).GetMethod("LoadAllPartsFromResources", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (method != null)
                    {
                        method.Invoke(managers[0], null);
                        EditorUtility.DisplayDialog("完成", "部件数据已重新加载！", "确定");
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"⚠️ 场景中有 {managers.Length} 个 OutfitManager！\n" +
                    "这会导致冲突，应该只保留一个。", 
                    MessageType.Warning);
                
                if (GUILayout.Button("删除多余的 OutfitManager"))
                {
                    for (int i = 1; i < managers.Length; i++)
                    {
                        DestroyImmediate(managers[i].gameObject);
                    }
                    EditorUtility.DisplayDialog("完成", "已删除多余的 OutfitManager", "确定");
                }
            }
            
            GUILayout.Space(20);
            
            // 检查 Resources/OutfitParts
            EditorGUILayout.LabelField("资源检查", EditorStyles.boldLabel);
            
            string outfitPartsPath = Application.dataPath + "/Resources/OutfitParts";
            if (System.IO.Directory.Exists(outfitPartsPath))
            {
                var files = System.IO.Directory.GetFiles(outfitPartsPath, "*.asset");
                EditorGUILayout.HelpBox(
                    $"✅ Resources/OutfitParts 文件夹存在\n" +
                    $"找到 {files.Length} 个 .asset 文件", 
                    MessageType.Info);
                
                GUILayout.Space(5);
                
                // 列出文件
                foreach (var file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    EditorGUILayout.LabelField($"  • {fileName}");
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "❌ Resources/OutfitParts 文件夹不存在！", 
                    MessageType.Error);
            }
            
            GUILayout.Space(20);
            
            // 调试按钮
            EditorGUILayout.LabelField("调试", EditorStyles.boldLabel);
            
            if (GUILayout.Button("手动测试 Resources.LoadAll"))
            {
                TestResourcesLoad();
            }
        }
        
        private void CreateOutfitManager()
        {
            GameObject go = new GameObject("OutfitManager");
            go.AddComponent<OutfitManager>();
            
            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
            
            Debug.Log("[OutfitManagerSceneChecker] 创建 OutfitManager 完成");
            EditorUtility.DisplayDialog("完成", "OutfitManager 已创建！\n\n请运行游戏查看效果。", "确定");
        }
        
        private void TestResourcesLoad()
        {
            var parts = Resources.LoadAll<OutfitPartData>("OutfitParts");
            
            if (parts == null || parts.Length == 0)
            {
                Debug.LogWarning("[OutfitManagerSceneChecker] Resources.LoadAll<OutfitPartData>(\"OutfitParts\") 返回空数组！");
                EditorUtility.DisplayDialog("测试结果", 
                    "Resources.LoadAll 返回空数组！\n\n" +
                    "可能原因：\n" +
                    "1. 文件夹路径不对（应该是 Assets/Resources/OutfitParts）\n" +
                    "2. 文件不是 OutfitPartData 类型\n" +
                    "3. Unity 需要刷新（尝试点击菜单 Assets → Refresh）", 
                    "确定");
            }
            else
            {
                string msg = $"找到 {parts.Length} 个部件：\n";
                foreach (var part in parts)
                {
                    msg += $"  • {part?.partId ?? "null"} - {part?.partName ?? "null"}\n";
                }
                
                Debug.Log("[OutfitManagerSceneChecker] " + msg);
                EditorUtility.DisplayDialog("测试结果", msg, "确定");
            }
        }
    }
}
