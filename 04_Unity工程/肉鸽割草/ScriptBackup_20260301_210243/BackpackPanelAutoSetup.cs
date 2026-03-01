using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using FluffyGeometry.UI;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// BackpackPanel 自动配置工具 - 快速设置缺失的引用
    /// </summary>
    public class BackpackPanelAutoSetup : EditorWindow
    {
        [MenuItem("绒毛几何物语/自动配置/BackpackPanel")]
        public static void ShowWindow()
        {
            GetWindow<BackpackPanelAutoSetup>("配置 BackpackPanel");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ BackpackPanel 自动配置 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具会自动为 BackpackPanel 配置缺失的字段：\n" +
                "• 主角装扮列表容器\n" +
                "• 家园装扮列表容器\n" +
                "• 部件项预制体\n" +
                "• 家具项预制体", 
                MessageType.Info);
            
            GUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);
            if (GUILayout.Button("自动配置选中的 BackpackPanel", GUILayout.Height(40)))
            {
                SetupSelectedBackpackPanel();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("查找场景中的 BackpackPanel"))
            {
                FindBackpackPanelInScene();
            }
            
            GUILayout.Space(20);
            
            // 显示当前选中对象信息
            EditorGUILayout.LabelField("当前选中:", Selection.activeGameObject?.name ?? "无");
        }
        
        private void SetupSelectedBackpackPanel()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("错误", "请先选中 BackpackPanel 游戏对象！", "确定");
                return;
            }
            
            BackpackPanel panel = selected.GetComponent<BackpackPanel>();
            if (panel == null)
            {
                EditorUtility.DisplayDialog("错误", "选中的对象没有 BackpackPanel 组件！", "确定");
                return;
            }
            
            Undo.RecordObject(panel, "Setup BackpackPanel");
            
            // 通过 SerializedObject 设置字段
            SerializedObject serializedObj = new SerializedObject(panel);
            
            // 1. 设置 outfitListContainer (CharacterContent 或其子对象)
            Transform characterContent = FindOrCreateChild(selected.transform, "CharacterContent");
            if (characterContent != null)
            {
                // 查找或创建 ScrollView -> Viewport -> Content
                Transform scrollView = FindOrCreateChild(characterContent, "ScrollView");
                Transform viewport = FindOrCreateChild(scrollView, "Viewport");
                Transform content = FindOrCreateChild(viewport, "Content");
                
                serializedObj.FindProperty("outfitListContainer").objectReferenceValue = content;
                
                // 添加必要的布局组件
                SetupContentLayout(content);
            }
            
            // 2. 设置 furnitureListContainer (FurnitureContent 或其子对象)
            Transform furnitureContent = FindOrCreateChild(selected.transform, "FurnitureContent");
            if (furnitureContent != null)
            {
                Transform scrollView = FindOrCreateChild(furnitureContent, "ScrollView");
                Transform viewport = FindOrCreateChild(scrollView, "Viewport");
                Transform content = FindOrCreateChild(viewport, "Content");
                
                serializedObj.FindProperty("furnitureListContainer").objectReferenceValue = content;
                
                // 添加必要的布局组件
                SetupFurnitureContentLayout(content);
            }
            
            // 3. 创建或加载 outfitItemPrefab（必须在保存预制体之前完成）
            GameObject outfitItemPrefab = CreateOutfitItemPrefab();
            
            // 4. 创建或加载 furnitureItemPrefab（必须在保存预制体之前完成）
            GameObject furnitureItemPrefab = CreateFurnitureItemPrefab();
            
            // 应用 SerializedObject 的更改
            serializedObj.FindProperty("outfitItemPrefab").objectReferenceValue = outfitItemPrefab;
            serializedObj.FindProperty("furnitureItemPrefab").objectReferenceValue = furnitureItemPrefab;
            
            // 5. 设置 Tab 按钮
            Transform tabContainer = FindOrCreateChild(selected.transform, "TabContainer");
            if (tabContainer != null)
            {
                Transform characterTab = tabContainer.Find("CharacterTab");
                Transform furnitureTab = tabContainer.Find("FurnitureTab");
                
                if (characterTab != null)
                    serializedObj.FindProperty("characterTabBtn").objectReferenceValue = characterTab.GetComponent<Button>();
                if (furnitureTab != null)
                    serializedObj.FindProperty("furnitureTabBtn").objectReferenceValue = furnitureTab.GetComponent<Button>();
            }
            
            // 6. 设置关闭按钮
            Transform closeBtn = selected.transform.Find("CloseBtn");
            if (closeBtn != null)
                serializedObj.FindProperty("closeBtn").objectReferenceValue = closeBtn.GetComponent<Button>();
            
            serializedObj.ApplyModifiedProperties();
            
            EditorUtility.SetDirty(panel);
            
            Debug.Log("[BackpackPanelAutoSetup] BackpackPanel 配置完成！");
            EditorUtility.DisplayDialog("完成", "BackpackPanel 自动配置完成！\n\n所有必要字段已设置。", "确定");
        }
        
        private void FindBackpackPanelInScene()
        {
            BackpackPanel[] panels = FindObjectsOfType<BackpackPanel>();
            if (panels.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "场景中没有找到 BackpackPanel！", "确定");
                return;
            }
            
            if (panels.Length == 1)
            {
                Selection.activeGameObject = panels[0].gameObject;
                EditorGUIUtility.PingObject(panels[0].gameObject);
            }
            else
            {
                EditorUtility.DisplayDialog("提示", $"场景中找到 {panels.Length} 个 BackpackPanel，请手动选择。", "确定");
                Selection.activeObject = panels[0];
            }
        }
        
        private Transform FindOrCreateChild(Transform parent, string name)
        {
            if (parent == null) return null;
            
            Transform child = parent.Find(name);
            if (child != null) return child;
            
            // 创建子对象
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }
        
        private void SetupContentLayout(Transform content)
        {
            if (content == null)
            {
                Debug.LogError("[BackpackPanelAutoSetup] SetupContentLayout: content 为 null");
                return;
            }
            
            // 确保有 RectTransform
            RectTransform rect = content.GetComponent<RectTransform>();
            if (rect == null) rect = content.gameObject.AddComponent<RectTransform>();
            
            // 添加 GridLayoutGroup
            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = content.gameObject.AddComponent<GridLayoutGroup>();
            
            grid.cellSize = new Vector2(100, 100);
            grid.spacing = new Vector2(15, 15);
            grid.padding = new RectOffset(15, 15, 15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            
            // 添加 ContentSizeFitter
            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        private void SetupFurnitureContentLayout(Transform content)
        {
            SetupContentLayout(content);
        }
        
        private GameObject CreateOutfitItemPrefab()
        {
            string path = "Assets/Prefabs/UI/BackpackOutfitItem.prefab";
            
            // 检查是否已存在
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;
            
            // 创建新的预制体
            GameObject go = new GameObject("BackpackOutfitItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
            
            Image bg = go.GetComponent<Image>();
            bg.color = new Color(0.25f, 0.25f, 0.3f);
            
            // 添加 OutfitItemUI 脚本
            var itemUI = go.AddComponent<OutfitItemUI>();
            
            // 创建图标
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(go.transform, false);
            
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
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
            
            // 保存
            EnsureDirectoryExists("Assets/Prefabs/UI");
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            DestroyImmediate(go);
            
            Debug.Log("[BackpackPanelAutoSetup] 创建 OutfitItem 预制体: " + path);
            return prefab;
        }
        
        private GameObject CreateFurnitureItemPrefab()
        {
            string path = "Assets/Prefabs/UI/FurnitureItem.prefab";
            
            // 检查是否已存在
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null) return existing;
            
            // 创建新的预制体
            GameObject go = new GameObject("FurnitureItem", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(120, 140);
            
            Image bg = go.GetComponent<Image>();
            bg.color = new Color(0.3f, 0.3f, 0.35f);
            
            // 添加 FurnitureItemUI 脚本
            var itemUI = go.AddComponent<FurnitureItemUI>();
            
            // 创建家具图标
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(go.transform, false);
            
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.25f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            // 创建名称文本
            GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(Text));
            nameObj.transform.SetParent(go.transform, false);
            
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.05f, 0.05f);
            nameRect.anchorMax = new Vector2(0.95f, 0.2f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            
            Text nameText = nameObj.GetComponent<Text>();
            nameText.text = "家具名称";
            nameText.fontSize = 14;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = Color.white;
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // 保存
            EnsureDirectoryExists("Assets/Prefabs/UI");
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            DestroyImmediate(go);
            
            Debug.Log("[BackpackPanelAutoSetup] 创建 FurnitureItem 预制体: " + path);
            return prefab;
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
