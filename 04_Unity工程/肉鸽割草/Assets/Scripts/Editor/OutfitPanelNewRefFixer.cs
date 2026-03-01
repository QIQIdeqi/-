using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// OutfitPanelNew 引用配置工具
    /// </summary>
    public class OutfitPanelNewRefFixer : EditorWindow
    {
        [MenuItem("绒毛几何物语/工具/修复OutfitPanelNew引用")]
        public static void ShowWindow()
        {
            GetWindow<OutfitPanelNewRefFixer>("修复OutfitPanel引用");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🔧 OutfitPanelNew 引用修复", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "自动配置 OutfitPanelNew 预制体的缺失引用：\n" +
                "• categoryTabPrefab\n" +
                "• partItemPrefab", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("修复场景中的OutfitPanelNew", GUILayout.Height(40)))
            {
                FixOutfitPanelInScene();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("修复预制体中的OutfitPanelNew", GUILayout.Height(40)))
            {
                FixOutfitPanelInPrefab();
            }
        }
        
        private void FixOutfitPanelInScene()
        {
            OutfitPanelNew[] panels = FindObjectsOfType<OutfitPanelNew>(true);
            int fixedCount = 0;
            
            foreach (var panel in panels)
            {
                if (FixPanelReferences(panel))
                    fixedCount++;
            }
            
            EditorUtility.DisplayDialog("完成", $"已修复 {fixedCount} 个 OutfitPanelNew", "确定");
        }
        
        private void FixOutfitPanelInPrefab()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab OutfitPanelNew");
            int fixedCount = 0;
            
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    OutfitPanelNew panel = prefab.GetComponent<OutfitPanelNew>();
                    if (panel != null && FixPanelReferences(panel))
                    {
                        // 保存预制体修改
                        EditorUtility.SetDirty(prefab);
                        fixedCount++;
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("完成", $"已修复 {fixedCount} 个 OutfitPanelNew 预制体", "确定");
        }
        
        private bool FixPanelReferences(OutfitPanelNew panel)
        {
            bool changed = false;
            SerializedObject so = new SerializedObject(panel);
            
            // 检查并创建 CategoryTab 预制体
            var categoryTabProp = so.FindProperty("categoryTabPrefab");
            if (categoryTabProp.objectReferenceValue == null)
            {
                GameObject tabPrefab = CreateTabPrefab();
                if (tabPrefab != null)
                {
                    categoryTabProp.objectReferenceValue = tabPrefab;
                    changed = true;
                    Debug.Log($"[OutfitPanelNewRefFixer] 配置 categoryTabPrefab: {panel.name}");
                }
            }
            
            // 检查并创建 PartItem 预制体
            var partItemProp = so.FindProperty("partItemPrefab");
            if (partItemProp.objectReferenceValue == null)
            {
                GameObject itemPrefab = CreatePartItemPrefab();
                if (itemPrefab != null)
                {
                    partItemProp.objectReferenceValue = itemPrefab;
                    changed = true;
                    Debug.Log($"[OutfitPanelNewRefFixer] 配置 partItemPrefab: {panel.name}");
                }
            }
            
            if (changed)
            {
                so.ApplyModifiedProperties();
            }
            
            return changed;
        }
        
        private GameObject CreateTabPrefab()
        {
            // 查找现有的 Tab 预制体
            string[] guids = AssetDatabase.FindAssets("t:Prefab CategoryTab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            
            // 创建新的 Tab 预制体
            GameObject tab = new GameObject("CategoryTab", typeof(RectTransform), typeof(Image), typeof(Button));
            
            RectTransform rect = tab.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 80);
            
            Image image = tab.GetComponent<Image>();
            image.color = new Color(1f, 0.96f, 0.97f);
            
            // 添加 Text
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(tab.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = "Tab";
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.365f, 0.251f, 0.216f);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // 保存预制体
            string path2 = "Assets/Prefabs/UI/CategoryTab.prefab";
            string dir = System.IO.Path.GetDirectoryName(path2);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(tab, path2);
            #else
            PrefabUtility.CreatePrefab(path2, tab);
            #endif
            
            DestroyImmediate(tab);
            
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath<GameObject>(path2);
        }
        
        private GameObject CreatePartItemPrefab()
        {
            // 查找现有的 PartItem 预制体
            string[] guids = AssetDatabase.FindAssets("t:Prefab OutfitPartItem");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            
            return null;
        }
    }
}
