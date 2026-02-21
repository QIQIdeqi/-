#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GeometryWarrior
{
    /// <summary>
    /// 编辑器工具 - 自动修复项目配置
    /// </summary>
    public class ProjectSetupWizard : EditorWindow
    {
        [MenuItem("Tools/项目设置向导")]
        public static void ShowWindow()
        {
            GetWindow<ProjectSetupWizard>("项目设置向导");
        }

        void OnGUI()
        {
            GUILayout.Label("=== 项目配置修复 ===", EditorStyles.boldLabel);
            
            if (GUILayout.Button("1. 创建必要的Tags", GUILayout.Height(30)))
            {
                CreateRequiredTags();
            }
            
            if (GUILayout.Button("2. 创建必要的Layers", GUILayout.Height(30)))
            {
                CreateRequiredLayers();
            }
            
            GUILayout.Space(20);
            GUILayout.Label("=== 诊断信息 ===", EditorStyles.boldLabel);
            
            // 检查状态
            bool hasEnemyTag = TagExists("Enemy");
            bool hasObstacleTag = TagExists("Obstacle");
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            
            GUILayout.Label($"Enemy Tag: {(hasEnemyTag ? "✅ 已定义" : "❌ 未定义")}");
            GUILayout.Label($"Obstacle Tag: {(hasObstacleTag ? "✅ 已定义" : "❌ 未定义")}");
            GUILayout.Label($"Obstacle Layer: {(obstacleLayer != -1 ? $"✅ Layer {obstacleLayer}" : "❌ 未定义")}");
        }
        
        void CreateRequiredTags()
        {
            // 使用SerializedObject修改Tags
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            
            string[] requiredTags = { "Enemy", "Obstacle" };
            
            foreach (string tag in requiredTags)
            {
                if (!TagExists(tag))
                {
                    // 找到第一个空槽位
                    for (int i = 0; i < tagsProp.arraySize; i++)
                    {
                        if (string.IsNullOrEmpty(tagsProp.GetArrayElementAtIndex(i).stringValue))
                        {
                            tagsProp.GetArrayElementAtIndex(i).stringValue = tag;
                            Debug.Log($"✅ 创建Tag: {tag}");
                            break;
                        }
                    }
                }
            }
            
            tagManager.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
        
        void CreateRequiredLayers()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            
            // 在User Layer 8添加Obstacle
            if (layersProp.GetArrayElementAtIndex(8).stringValue != "Obstacle")
            {
                layersProp.GetArrayElementAtIndex(8).stringValue = "Obstacle";
                Debug.Log("✅ 创建Layer: Obstacle (Layer 8)");
            }
            
            tagManager.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }
        
        bool TagExists(string tagName)
        {
            // 检查标签是否存在
            try
            {
                GameObject test = new GameObject("test");
                bool exists = test.CompareTag(tagName);
                DestroyImmediate(test);
                return false; // CompareTag会抛出异常如果tag不存在
            }
            catch
            {
                return true; // 如果抛出异常说明tag存在（Unity的行为）
            }
        }
    }
    
    /// <summary>
    /// 运行时代码 - 兼容层
    /// </summary>
    public static class TagHelper
    {
        /// <summary>
        /// 安全的CompareTag，不会报错
        /// </summary>
        public static bool SafeCompareTag(this GameObject obj, string tag)
        {
            try
            {
                return obj.CompareTag(tag);
            }
            catch
            {
                return false;
            }
        }
    }
}
#endif
