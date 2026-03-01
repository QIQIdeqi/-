using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 背包页面内容生成工具
    /// 自动生成主角装备页和家园装扮页的内容
    /// </summary>
    public class BackpackContentSetupTool : EditorWindow
    {
        private GameObject backpackPanelPrefab;
        private bool createPlayerEquipSlots = true;
        private bool createHomeOutfitContent = true;
        
        [MenuItem("绒毛几何物语/工具/设置背包页面内容")]
        public static void ShowWindow()
        {
            GetWindow<BackpackContentSetupTool>("设置背包内容");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("📦 背包页面内容设置工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "此工具会自动：\n" +
                "1. 给主角装备页添加装备槽位\n" +
                "2. 给家园装扮页添加 OutfitPanelNew\n" +
                "3. 配置所有必要的引用", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            backpackPanelPrefab = EditorGUILayout.ObjectField(
                "BackpackPanel 预制体:", 
                backpackPanelPrefab, 
                typeof(GameObject), 
                false
            ) as GameObject;
            
            EditorGUILayout.Space();
            
            createPlayerEquipSlots = EditorGUILayout.Toggle("创建主角装备槽位", createPlayerEquipSlots);
            createHomeOutfitContent = EditorGUILayout.Toggle("创建家园装扮内容", createHomeOutfitContent);
            
            EditorGUILayout.Space();
            
            GUI.backgroundColor = new Color(0.659f, 0.902f, 0.812f);
            if (GUILayout.Button("开始设置", GUILayout.Height(50)))
            {
                SetupBackpackContent();
            }
            GUI.backgroundColor = Color.white;
        }
        
        private void SetupBackpackContent()
        {
            if (backpackPanelPrefab == null)
            {
                EditorUtility.DisplayDialog("错误", "请先拖入 BackpackPanel 预制体！", "确定");
                return;
            }
            
            try
            {
                // 实例化到场景中
                GameObject instance = PrefabUtility.InstantiatePrefab(backpackPanelPrefab) as GameObject;
                
                BackpackPanel backpackPanel = instance.GetComponent<BackpackPanel>();
                if (backpackPanel == null)
                {
                    EditorUtility.DisplayDialog("错误", "BackpackPanel 预制体缺少 BackpackPanel 脚本！", "确定");
                    DestroyImmediate(instance);
                    return;
                }
                
                // 设置主角装备页
                if (createPlayerEquipSlots)
                {
                    SetupPlayerEquipPage(backpackPanel);
                }
                
                // 设置家园装扮页
                if (createHomeOutfitContent)
                {
                    SetupHomeOutfitPage(backpackPanel);
                }
                
                // 保存回预制体
                PrefabUtility.ApplyPrefabInstance(instance, InteractionMode.UserAction);
                
                DestroyImmediate(instance);
                
                EditorUtility.DisplayDialog("完成", "背包页面内容设置完成！\n\n请确保场景中的 BackpackPanel 也更新了。", "确定");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("错误", $"设置失败：{e.Message}", "确定");
                Debug.LogError($"[BackpackContentSetupTool] {e}");
            }
        }
        
        /// <summary>
        /// 设置主角装备页
        /// </summary>
        private void SetupPlayerEquipPage(BackpackPanel backpackPanel)
        {
            if (backpackPanel.playerEquipPage == null)
            {
                Debug.LogWarning("[BackpackContentSetupTool] 主角装备页为空！");
                return;
            }
            
            // 添加或获取 PlayerEquipPage 脚本
            PlayerEquipPage playerPage = backpackPanel.playerEquipPage.GetComponent<PlayerEquipPage>();
            if (playerPage == null)
            {
                playerPage = backpackPanel.playerEquipPage.AddComponent<PlayerEquipPage>();
            }
            
            // 获取或创建内容容器
            Transform content = backpackPanel.playerEquipPage.transform.Find("Content");
            if (content == null)
            {
                content = new GameObject("Content", typeof(RectTransform)).transform;
                content.SetParent(backpackPanel.playerEquipPage.transform, false);
                
                RectTransform contentRect = content.GetComponent<RectTransform>();
                contentRect.anchorMin = Vector2.zero;
                contentRect.anchorMax = Vector2.one;
                contentRect.offsetMin = Vector2.zero;
                contentRect.offsetMax = Vector2.zero;
            }
            
            // 创建角色预览区域
            GameObject previewArea = CreatePlayerPreviewArea(content);
            
            // 创建装备槽位区域
            GameObject slotsArea = CreateEquipSlotsArea(content);
            
            // 创建属性显示区域
            GameObject statsArea = CreateStatsArea(content);
            
            // 配置 PlayerEquipPage 引用
            SerializedObject so = new SerializedObject(playerPage);
            
            // 角色预览
            Transform charImage = previewArea.transform.Find("CharacterImage");
            if (charImage != null)
                so.FindProperty("characterPreview").objectReferenceValue = charImage.GetComponent<Image>();
            
            Transform rotateLeft = previewArea.transform.Find("RotateLeftBtn");
            if (rotateLeft != null)
                so.FindProperty("rotateLeftBtn").objectReferenceValue = rotateLeft.GetComponent<Button>();
            
            Transform rotateRight = previewArea.transform.Find("RotateRightBtn");
            if (rotateRight != null)
                so.FindProperty("rotateRightBtn").objectReferenceValue = rotateRight.GetComponent<Button>();
            
            // 槽位容器
            Transform slotsContainer = slotsArea.transform.Find("SlotsContainer");
            if (slotsContainer != null)
                so.FindProperty("slotContainer").objectReferenceValue = slotsContainer;
            
            // 属性文本
            so.FindProperty("attackText").objectReferenceValue = statsArea.transform.Find("AttackText")?.GetComponent<Text>();
            so.FindProperty("defenseText").objectReferenceValue = statsArea.transform.Find("DefenseText")?.GetComponent<Text>();
            so.FindProperty("healthText").objectReferenceValue = statsArea.transform.Find("HealthText")?.GetComponent<Text>();
            
            so.ApplyModifiedProperties();
            
            Debug.Log("[BackpackContentSetupTool] 主角装备页设置完成");
        }
        
        /// <summary>
        /// 创建角色预览区域
        /// </summary>
        private GameObject CreatePlayerPreviewArea(Transform parent)
        {
            GameObject area = parent.Find("PreviewArea")?.gameObject;
            if (area != null) return area;
            
            area = new GameObject("PreviewArea", typeof(RectTransform), typeof(Image));
            area.transform.SetParent(parent, false);
            
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(300, 300);
            rect.anchoredPosition = new Vector2(0, -100);
            
            Image image = area.GetComponent<Image>();
            image.color = new Color(1f, 0.894f, 0.925f); // #FFE4EC
            
            // 角色图片
            GameObject charImage = new GameObject("CharacterImage", typeof(RectTransform), typeof(Image));
            charImage.transform.SetParent(area.transform, false);
            
            RectTransform charRect = charImage.GetComponent<RectTransform>();
            charRect.anchorMin = new Vector2(0.5f, 0.5f);
            charRect.anchorMax = new Vector2(0.5f, 0.5f);
            charRect.sizeDelta = new Vector2(200, 200);
            charRect.anchoredPosition = Vector2.zero;
            
            // 旋转按钮
            CreateCircleButton(area.transform, "RotateLeftBtn", "<", new Vector2(20, 150));
            CreateCircleButton(area.transform, "RotateRightBtn", ">", new Vector2(280, 150));
            
            return area;
        }
        
        /// <summary>
        /// 创建装备槽位区域
        /// </summary>
        private GameObject CreateEquipSlotsArea(Transform parent)
        {
            GameObject area = parent.Find("SlotsArea")?.gameObject;
            if (area != null) return area;
            
            area = new GameObject("SlotsArea", typeof(RectTransform), typeof(GridLayoutGroup));
            area.transform.SetParent(parent, false);
            
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600, 200);
            rect.anchoredPosition = new Vector2(0, -50);
            
            GridLayoutGroup grid = area.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(100, 120);
            grid.spacing = new Vector2(20, 10);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.childAlignment = TextAnchor.MiddleCenter;
            
            // 创建槽位容器
            GameObject slotsContainer = new GameObject("SlotsContainer", typeof(RectTransform));
            slotsContainer.transform.SetParent(area.transform, false);
            slotsContainer.transform.SetAsFirstSibling();
            
            // 创建示例槽位
            for (int i = 0; i < 5; i++)
            {
                CreateEquipSlotPrefab(area.transform, i);
            }
            
            return area;
        }
        
        /// <summary>
        /// 创建装备槽位预制体实例
        /// </summary>
        private void CreateEquipSlotPrefab(Transform parent, int index)
        {
            GameObject slot = new GameObject($"Slot_{index}", typeof(RectTransform), typeof(Image), typeof(Button));
            slot.transform.SetParent(parent, false);
            
            RectTransform rect = slot.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 120);
            
            Image image = slot.GetComponent<Image>();
            image.color = new Color(0.9f, 0.9f, 0.9f);
            
            // Icon
            GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(slot.transform, false);
            
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.7f);
            iconRect.anchorMax = new Vector2(0.5f, 0.7f);
            iconRect.sizeDelta = new Vector2(60, 60);
            
            // Name - 使用普通 Text
            GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(Text));
            nameObj.transform.SetParent(slot.transform, false);
            
            Text nameText = nameObj.GetComponent<Text>();
            nameText.text = "空槽位";
            nameText.fontSize = 16;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = new Color(0.5f, 0.5f, 0.5f);
            nameText.font = GetSimHeiFont();
            
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 0.3f);
            nameRect.offsetMin = new Vector2(5, 5);
            nameRect.offsetMax = new Vector2(-5, -5);
        }
        
        /// <summary>
        /// 创建属性显示区域
        /// </summary>
        private GameObject CreateStatsArea(Transform parent)
        {
            GameObject area = parent.Find("StatsArea")?.gameObject;
            if (area != null) return area;
            
            area = new GameObject("StatsArea", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            area.transform.SetParent(parent, false);
            
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.sizeDelta = new Vector2(500, 80);
            rect.anchoredPosition = new Vector2(0, 120);
            
            HorizontalLayoutGroup layout = area.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.MiddleCenter;
            
            // 创建属性文本
            CreateStatText(area.transform, "AttackText", "攻击: +0");
            CreateStatText(area.transform, "DefenseText", "防御: +0");
            CreateStatText(area.transform, "HealthText", "生命: +0");
            
            return area;
        }
        
        /// <summary>
        /// 创建属性文本
        /// </summary>
        private void CreateStatText(Transform parent, string name, string defaultText)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(parent, false);
            
            Text text = textObj.GetComponent<Text>();
            text.text = defaultText;
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.365f, 0.251f, 0.216f);
            text.font = GetSimHeiFont();
            text.fontStyle = FontStyle.Bold;
            
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 40);
        }
        
        /// <summary>
        /// 设置家园装扮页
        /// </summary>
        private void SetupHomeOutfitPage(BackpackPanel backpackPanel)
        {
            if (backpackPanel.homeOutfitPage == null)
            {
                Debug.LogWarning("[BackpackContentSetupTool] 家园装扮页为空！");
                return;
            }
            
            // 添加 HomeOutfitPageContent 脚本
            HomeOutfitPageContent homeContent = backpackPanel.homeOutfitPage.GetComponent<HomeOutfitPageContent>();
            if (homeContent == null)
            {
                homeContent = backpackPanel.homeOutfitPage.AddComponent<HomeOutfitPageContent>();
            }
            
            // 设置面板容器
            SerializedObject so = new SerializedObject(homeContent);
            so.FindProperty("panelContainer").objectReferenceValue = backpackPanel.homeOutfitPage.transform;
            
            // 查找 OutfitPanelNew 预制体
            string[] guids = AssetDatabase.FindAssets("t:Prefab OutfitPanelNew");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                so.FindProperty("outfitPanelPrefab").objectReferenceValue = prefab;
                
                // 同时设置 BackpackPanel 的引用
                SerializedObject backpackSO = new SerializedObject(backpackPanel);
                backpackSO.FindProperty("outfitPanelPrefab").objectReferenceValue = prefab;
                backpackSO.ApplyModifiedProperties();
            }
            
            so.ApplyModifiedProperties();
            
            Debug.Log("[BackpackContentSetupTool] 家园装扮页设置完成");
        }
        
        /// <summary>
        /// 创建圆形按钮
        /// </summary>
        private void CreateCircleButton(Transform parent, string name, string text, Vector2 position)
        {
            GameObject btn = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(50, 50);
            rect.anchoredPosition = position;
            
            Image image = btn.GetComponent<Image>();
            image.color = new Color(0.659f, 0.902f, 0.812f);
            
            // 文字
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btn.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 20;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = GetSimHeiFont();
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
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
