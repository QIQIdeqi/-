using UnityEngine;
using UnityEditor;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// OutfitManager 数据自动填充工具
    /// </summary>
    public class OutfitManagerDataFiller : EditorWindow
    {
        [MenuItem("绒毛几何物语/自动配置/OutfitManager 数据")]
        public static void ShowWindow()
        {
            GetWindow<OutfitManagerDataFiller>("填充 OutfitManager 数据");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ OutfitManager 数据自动填充 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具会扫描 Resources/OutfitParts 文件夹，\n" +
                "把所有部件数据自动填充到场景中的 OutfitManager。", 
                MessageType.Info);
            
            GUILayout.Space(10);
            
            // 查找场景中的 OutfitManager
            OutfitManager manager = FindObjectOfType<OutfitManager>();
            
            if (manager == null)
            {
                EditorGUILayout.HelpBox("❌ 场景中没有 OutfitManager！", MessageType.Error);
                
                if (GUILayout.Button("创建 OutfitManager"))
                {
                    GameObject go = new GameObject("OutfitManager");
                    go.AddComponent<OutfitManager>();
                    Selection.activeGameObject = go;
                }
                return;
            }
            
            // 显示当前状态
            EditorGUILayout.LabelField("当前状态", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"场景中的 OutfitManager: {manager.name}");
            EditorGUILayout.LabelField($"当前部件数量: {manager.GetAllParts()?.Count ?? 0}");
            
            GUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("自动填充部件数据", GUILayout.Height(40)))
            {
                FillOutfitManagerData(manager);
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("清空所有部件数据"))
            {
                if (EditorUtility.DisplayDialog("确认", "确定要清空所有部件数据吗？", "确定", "取消"))
                {
                    ClearOutfitManagerData(manager);
                }
            }
            
            GUILayout.Space(20);
            
            // 扫描 Resources/OutfitParts
            EditorGUILayout.LabelField("Resources/OutfitParts 扫描结果", EditorStyles.boldLabel);
            
            string path = Application.dataPath + "/Resources/OutfitParts";
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.asset");
                EditorGUILayout.LabelField($"找到 {files.Length} 个 .asset 文件:");
                
                foreach (var file in files)
                {
                    string fileName = Path.GetFileName(file);
                    EditorGUILayout.LabelField($"  • {fileName}");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Resources/OutfitParts 文件夹不存在！", MessageType.Error);
            }
        }
        
        private void FillOutfitManagerData(OutfitManager manager)
        {
            string path = Application.dataPath + "/Resources/OutfitParts";
            if (!Directory.Exists(path))
            {
                EditorUtility.DisplayDialog("错误", "Resources/OutfitParts 文件夹不存在！", "确定");
                return;
            }
            
            Undo.RecordObject(manager, "Fill OutfitManager Data");
            
            var files = Directory.GetFiles(path, "*.asset");
            int successCount = 0;
            int failCount = 0;
            
            // 通过 SerializedObject 修改私有字段
            SerializedObject serializedObj = new SerializedObject(manager);
            SerializedProperty allPartsProp = serializedObj.FindProperty("allParts");
            
            // 清空现有列表
            allPartsProp.ClearArray();
            
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string assetPath = $"Assets/Resources/OutfitParts/{fileName}.asset";
                
                var part = AssetDatabase.LoadAssetAtPath<OutfitPartData>(assetPath);
                if (part != null)
                {
                    int index = allPartsProp.arraySize;
                    allPartsProp.InsertArrayElementAtIndex(index);
                    allPartsProp.GetArrayElementAtIndex(index).objectReferenceValue = part;
                    successCount++;
                }
                else
                {
                    Debug.LogWarning($"[OutfitManagerDataFiller] 无法加载: {assetPath}");
                    failCount++;
                }
            }
            
            serializedObj.ApplyModifiedProperties();
            EditorUtility.SetDirty(manager);
            
            Debug.Log($"[OutfitManagerDataFiller] 成功填充 {successCount} 个部件，失败 {failCount} 个");
            EditorUtility.DisplayDialog("完成", 
                $"成功填充 {successCount} 个部件！\n" +
                (failCount > 0 ? $"失败 {failCount} 个\n" : "") +
                "\n记得 Ctrl+S 保存场景！", 
                "确定");
        }
        
        private void ClearOutfitManagerData(OutfitManager manager)
        {
            Undo.RecordObject(manager, "Clear OutfitManager Data");
            
            SerializedObject serializedObj = new SerializedObject(manager);
            SerializedProperty allPartsProp = serializedObj.FindProperty("allParts");
            allPartsProp.ClearArray();
            serializedObj.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(manager);
            
            Debug.Log("[OutfitManagerDataFiller] 已清空所有部件数据");
        }
    }
}
