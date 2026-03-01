using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// OutfitPanelNew 引用自动修复工具 V2
    /// 创建缺失的预制体并配置所有引用
    /// </summary>
    public class OutfitPanelNewRefSetupToolV2 : EditorWindow
    {
        private string savePath = "Assets/Prefabs/UI";
        
        [MenuItem("绒毛几何物语/工具/修复OutfitPanelNew引用V2")]
        public static void ShowWindow()
        {
            GetWindow<OutfitPanelNewRefSetupToolV2>("修复OutfitPanel引用V2");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🔧 修复 OutfitPanelNew 引用", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "此工具会：\n" +
                "1. 创建 CategoryTab 预制体（分类标签）\n" +
                "2. 创建 OutfitPartItem 预制体（部件项）\n" +
                "3. 自动配置到 OutfitPanelNew 预制体", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            savePath = EditorGUILayout.TextField("保存路径:", savePath);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("创建预制体并修复引用", GUILayout.Height(50)))
            {
                SetupAllReferences();
            }
        }
        
        private void SetupAllReferences()
        {
            try
            {
                // 确保目录存在
                if (!AssetDatabase.IsValidFolder(savePath))
                {
                    Directory.CreateDirectory(savePath);
                    AssetDatabase.Refresh();
                }
                
                // 1. 创建 CategoryTab 预制体
                GameObject categoryTabPrefab = CreateCategoryTabPrefab();
                
                // 2. 创建 OutfitPartItem 预制体
                GameObject partItemPrefab = CreatePartItemPrefab();
                
                // 3. 修复场景中的 OutfitPanelNew
                int sceneCount = FixOutfitPanelInScene(categoryTabPrefab, partItemPrefab);
                
                // 4. 修复预制体中的 OutfitPanelNew
                int prefabCount = FixOutfitPanelInPrefab(categoryTabPrefab, partItemPrefab);
                
                AssetDatabase.SaveAssets();
                
                EditorUtility.DisplayDialog("完成", 
                    $"修复完成！\n\n" +
                    $"创建了预制体：\n" +
                    $"- CategoryTab.prefab\n" +
                    $"- OutfitPartItem.prefab\n\n" +
                    $"修复了 {sceneCount} 个场景实例\n" +
                    $"修复了 {prefabCount} 个预制体\n\n" +
                    $"路径：{savePath}", "确定");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("错误", $"修复失败：{e.Message}", "确定");
                Debug.LogError($"[OutfitPanelNewRefSetupToolV2] {e}");
            }
        }
        
        /// <summary>
        /// 创建 CategoryTab 预制体
        /// </summary>
        private GameObject CreateCategoryTabPrefab()
        {
            string prefabPath = $"{savePath}/CategoryTab.prefab";
            
            // 检查是否已存在
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existing != null)
            {
                Debug.Log($"[OutfitPanelNewRefSetupToolV2] CategoryTab 预制体已存在");
                return existing;
            }
            
            // 创建新预制体
            GameObject tab = new GameObject("CategoryTab", typeof(RectTransform), typeof(Image), typeof(Button));
            
            RectTransform rect = tab.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 80);
            
            Image image = tab.GetComponent<Image>();
            image.color = new Color(1f, 0.96f, 0.97f); // #FFF5F7
            
            // 添加普通 Text 组件
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(tab.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = "Tab";
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = new Color(0.365f, 0.251f, 0.216f);
            txt.font = GetSimHeiFont();
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // 保存预制体
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(tab, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, tab);
            #endif
            
            DestroyImmediate(tab);
            
            AssetDatabase.Refresh();
            
            Debug.Log($"[OutfitPanelNewRefSetupToolV2] 创建 CategoryTab 预制体：{prefabPath}");
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }
        
        /// <summary>
        /// 创建 OutfitPartItem 预制体
        /// </summary>
        private GameObject CreatePartItemPrefab()
        {
            string prefabPath = $"{savePath}/OutfitPartItem.prefab";
            
            // 检查是否已存在
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existing != null)
            {
                Debug.Log($"[OutfitPanelNewRefSetupToolV2] OutfitPartItem 预制体已存在");
                return existing;
            }
            
            // 创建新预制体
            GameObject item = new GameObject("OutfitPartItem", typeof(RectTransform), typeof(Image), typeof(Button));
            
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 160);
            
            Image image = item.GetComponent<Image>();
            image.color = Color.white;
            
            // Icon
            GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(item.transform, false);
            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.7f);
            iconRect.anchorMax = new Vector2(0.5f, 0.7f);
            iconRect.sizeDelta = new Vector2(80, 80);
            
            // Name - 普通 Text
            GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(Text));
            nameObj.transform.SetParent(item.transform, false);
            
            Text nameText = nameObj.GetComponent<Text>();
            nameText.text = "部件名称";
            nameText.fontSize = 18;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = new Color(0.365f, 0.251f, 0.216f);
            nameText.font = GetSimHeiFont();
            
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 0.3f);
            nameRect.offsetMin = new Vector2(5, 5);
            nameRect.offsetMax = new Vector2(-5, -5);
            
            // Status - 普通 Text
            GameObject statusObj = new GameObject("Status", typeof(RectTransform), typeof(Text));
            statusObj.transform.SetParent(item.transform, false);
            statusObj.SetActive(false);
            
            Text statusText = statusObj.GetComponent<Text>();
            statusText.text = "已装备";
            statusText.fontSize = 14;
            statusText.alignment = TextAnchor.MiddleCenter;
            statusText.color = new Color(0.4f, 0.7f, 0.5f);
            statusText.font = GetSimHeiFont();
            
            RectTransform statusRect = statusObj.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0, 0.7f);
            statusRect.anchorMax = new Vector2(1, 0.9f);
            statusRect.offsetMin = Vector2.zero;
            statusRect.offsetMax = Vector2.zero;
            
            // 保存预制体
            #if UNITY_2018_3_OR_NEWER
            PrefabUtility.SaveAsPrefabAsset(item, prefabPath);
            #else
            PrefabUtility.CreatePrefab(prefabPath, item);
            #endif
            
            DestroyImmediate(item);
            
            AssetDatabase.Refresh();
            
            Debug.Log($"[OutfitPanelNewRefSetupToolV2] 创建 OutfitPartItem 预制体：{prefabPath}");
            return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }
        
        /// <summary>
        /// 修复场景中的 OutfitPanelNew
        /// </summary>
        private int FixOutfitPanelInScene(GameObject categoryTabPrefab, GameObject partItemPrefab)
        {
            OutfitPanelNew[] panels = FindObjectsOfType<OutfitPanelNew>(true);
            int count = 0;
            
            foreach (var panel in panels)
            {
                if (FixPanelReferences(panel, categoryTabPrefab, partItemPrefab))
                {
                    count++;
                    EditorUtility.SetDirty(panel);
                }
            }
            
            return count;
        }
        
        /// <summary>
        /// 修复预制体中的 OutfitPanelNew
        /// </summary>
        private int FixOutfitPanelInPrefab(GameObject categoryTabPrefab, GameObject partItemPrefab)
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab OutfitPanelNew");
            int count = 0;
            
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    OutfitPanelNew panel = prefab.GetComponent<OutfitPanelNew>();
                    if (panel != null && FixPanelReferences(panel, categoryTabPrefab, partItemPrefab))
                    {
                        EditorUtility.SetDirty(prefab);
                        count++;
                    }
                }
            }
            
            return count;
        }
        
        /// <summary>
        /// 修复单个面板的引用
        /// </summary>
        private bool FixPanelReferences(OutfitPanelNew panel, GameObject categoryTabPrefab, GameObject partItemPrefab)
        {
            bool changed = false;
            SerializedObject so = new SerializedObject(panel);
            
            // 设置 CategoryTab 预制体
            var categoryTabProp = so.FindProperty("categoryTabPrefab");
            if (categoryTabProp.objectReferenceValue == null && categoryTabPrefab != null)
            {
                categoryTabProp.objectReferenceValue = categoryTabPrefab;
                changed = true;
                Debug.Log($"[OutfitPanelNewRefSetupToolV2] 配置 categoryTabPrefab: {panel.name}");
            }
            
            // 设置 PartItem 预制体
            var partItemProp = so.FindProperty("partItemPrefab");
            if (partItemProp.objectReferenceValue == null && partItemPrefab != null)
            {
                partItemProp.objectReferenceValue = partItemPrefab;
                changed = true;
                Debug.Log($"[OutfitPanelNewRefSetupToolV2] 配置 partItemPrefab: {panel.name}");
            }
            
            if (changed)
            {
                so.ApplyModifiedProperties();
            }
            
            return changed;
        }
        
        /// <summary>
        /// 获取黑体字体
        /// </summary>
        private Font GetSimHeiFont()
        {
            Font simhei = Resources.GetBuiltinResource<Font>("SIMHEI.ttf");
            if (simhei == null)
            {
                simhei = Font.CreateDynamicFontFromOSFont("SimHei", 24);
            }
            return simhei ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
    }
}
