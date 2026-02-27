using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// OutfitItemUI 预制体修复工具
    /// </summary>
    public class OutfitItemUIPrefabFixer : EditorWindow
    {
        [MenuItem("绒毛几何物语/修复/OutfitItemUI 预制体")]
        public static void ShowWindow()
        {
            GetWindow<OutfitItemUIPrefabFixer>("修复 OutfitItemUI");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ OutfitItemUI 预制体修复 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具会重新创建 OutfitItem 预制体，\n" +
                "并正确绑定所有 Image 字段。", 
                MessageType.Info);
            
            GUILayout.Space(10);
            
            GUI.backgroundColor = new Color(1f, 0.6f, 0.2f);
            if (GUILayout.Button("重新创建 OutfitItem 预制体", GUILayout.Height(50)))
            {
                RecreateOutfitItemPrefab();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("检查现有预制体"))
            {
                CheckExistingPrefab();
            }
        }
        
        private void RecreateOutfitItemPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/BackpackOutfitItem.prefab";
            
            // 删除旧的
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                AssetDatabase.DeleteAsset(prefabPath);
                Debug.Log("[OutfitItemUIPrefabFixer] 删除旧预制体");
            }
            
            // 确保目录存在
            EnsureDirectoryExists("Assets/Prefabs/UI");
            
            // 创建新的
            GameObject go = new GameObject("BackpackOutfitItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            
            // 设置背景
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
            
            Image bg = go.GetComponent<Image>();
            bg.color = new Color(0.25f, 0.25f, 0.3f);
            
            // 创建图标 Image（关键！）
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(go.transform, false);
            
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            Image iconImage = iconObj.GetComponent<Image>();
            iconImage.color = Color.white;
            
            // 创建锁定图标（默认隐藏）
            GameObject lockObj = new GameObject("LockIcon", typeof(RectTransform), typeof(Image));
            lockObj.transform.SetParent(go.transform, false);
            lockObj.SetActive(false);
            
            RectTransform lockRect = lockObj.GetComponent<RectTransform>();
            lockRect.anchorMin = new Vector2(0.3f, 0.3f);
            lockRect.anchorMax = new Vector2(0.7f, 0.7f);
            lockRect.offsetMin = Vector2.zero;
            lockRect.offsetMax = Vector2.zero;
            
            // 创建已装备标识（默认隐藏）
            GameObject equippedObj = new GameObject("EquippedIndicator", typeof(RectTransform), typeof(Image));
            equippedObj.transform.SetParent(go.transform, false);
            equippedObj.SetActive(false);
            
            RectTransform equippedRect = equippedObj.GetComponent<RectTransform>();
            equippedRect.anchorMin = new Vector2(0.7f, 0.7f);
            equippedRect.anchorMax = new Vector2(1, 1);
            equippedRect.offsetMin = Vector2.zero;
            equippedRect.offsetMax = Vector2.zero;
            
            // 添加 OutfitItemUI 脚本
            OutfitItemUI itemUI = go.AddComponent<OutfitItemUI>();
            
            // 通过 SerializedObject 设置字段（关键！）
            SerializedObject serializedObj = new SerializedObject(itemUI);
            serializedObj.FindProperty("iconImage").objectReferenceValue = iconImage;
            serializedObj.FindProperty("lockImage").objectReferenceValue = lockObj.GetComponent<Image>();
            serializedObj.FindProperty("equippedIndicator").objectReferenceValue = equippedObj;
            serializedObj.FindProperty("button").objectReferenceValue = go.GetComponent<Button>();
            serializedObj.ApplyModifiedProperties();
            
            // 保存预制体
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            DestroyImmediate(go);
            
            // 选中
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            
            Debug.Log("[OutfitItemUIPrefabFixer] 预制体创建完成: " + prefabPath);
            EditorUtility.DisplayDialog("完成", 
                "OutfitItem 预制体已重新创建！\n\n所有字段已正确绑定。", 
                "确定");
        }
        
        private void CheckExistingPrefab()
        {
            string prefabPath = "Assets/Prefabs/UI/BackpackOutfitItem.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("检查结果", "预制体不存在，请重新创建。", "确定");
                return;
            }
            
            // 实例化检查
            GameObject instance = Instantiate(prefab);
            OutfitItemUI itemUI = instance.GetComponent<OutfitItemUI>();
            
            if (itemUI == null)
            {
                DestroyImmediate(instance);
                EditorUtility.DisplayDialog("检查结果", "预制体没有 OutfitItemUI 组件！", "确定");
                return;
            }
            
            // 检查字段
            string report = "预制体检查结果:\n\n";
            
            // 通过 SerializedObject 读取
            SerializedObject serializedObj = new SerializedObject(itemUI);
            
            var iconImage = serializedObj.FindProperty("iconImage").objectReferenceValue;
            var lockImage = serializedObj.FindProperty("lockImage").objectReferenceValue;
            var equippedIndicator = serializedObj.FindProperty("equippedIndicator").objectReferenceValue;
            var button = serializedObj.FindProperty("button").objectReferenceValue;
            
            report += $"iconImage: {(iconImage != null ? "✓ 已绑定" : "✗ 未绑定")}\n";
            report += $"lockImage: {(lockImage != null ? "✓ 已绑定" : "✗ 未绑定")}\n";
            report += $"equippedIndicator: {(equippedIndicator != null ? "✓ 已绑定" : "✗ 未绑定")}\n";
            report += $"button: {(button != null ? "✓ 已绑定" : "✗ 未绑定")}\n";
            
            DestroyImmediate(instance);
            
            EditorUtility.DisplayDialog("检查结果", report, "确定");
        }
        
        private void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                string folderName = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
