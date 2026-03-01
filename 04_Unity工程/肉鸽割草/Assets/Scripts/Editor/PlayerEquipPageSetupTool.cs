using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 主角装扮页面设置工具 - 修复重叠问题
    /// </summary>
    public class PlayerEquipPageSetupTool : EditorWindow
    {
        [MenuItem("绒毛几何物语/工具/设置主角装扮页面")]
        public static void ShowWindow()
        {
            GetWindow<PlayerEquipPageSetupTool>("设置主角装扮页面");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("👤 设置主角装扮页面", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "创建无重叠的垂直布局：\n" +
                "• 预览区（固定300px）\n" +
                "• 槽位区（固定140px）\n" +
                "• 描述区（固定100px）", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("创建/重置页面结构", GUILayout.Height(50)))
            {
                SetupPlayerEquipPage();
            }
        }
        
        private void SetupPlayerEquipPage()
        {
            var backpackPanels = FindObjectsOfType<BackpackPanel>(true);
            if (backpackPanels.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "场景中未找到 BackpackPanel！", "确定");
                return;
            }
            
            int count = 0;
            foreach (var panel in backpackPanels)
            {
                if (SetupPage(panel))
                    count++;
            }
            
            EditorUtility.DisplayDialog("完成", $"已设置 {count} 个主角装扮页面", "确定");
        }
        
        private bool SetupPage(BackpackPanel panel)
        {
            if (panel.playerEquipPage == null)
            {
                Debug.LogWarning($"[PlayerEquipPageSetupTool] {panel.name} 的 playerEquipPage 为空");
                return false;
            }
            
            // 清除旧内容
            ClearContent(panel.playerEquipPage.transform);
            
            // 添加 PlayerEquipPage 脚本
            var pageScript = panel.playerEquipPage.GetComponent<PlayerEquipPage>();
            if (pageScript == null)
            {
                pageScript = panel.playerEquipPage.AddComponent<PlayerEquipPage>();
            }
            
            Transform pageTransform = panel.playerEquipPage.transform;
            
            // 1. 创建角色预览区（顶部）
            GameObject previewArea = CreatePreviewArea(pageTransform);
            
            // 2. 创建槽位区域（中部）
            GameObject slotsArea = CreateSlotsArea(pageTransform);
            
            // 3. 创建描述区域（底部）
            GameObject descArea = CreateDescriptionArea(pageTransform);
            
            // 配置脚本引用
            ConfigureScript(pageScript, previewArea, slotsArea, descArea);
            
            EditorUtility.SetDirty(panel.playerEquipPage);
            
            Debug.Log($"[PlayerEquipPageSetupTool] 已设置 {panel.name} 的主角装扮页面");
            return true;
        }
        
        private void ClearContent(Transform parent)
        {
            // 删除所有子物体
            while (parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }
        
        private GameObject CreatePreviewArea(Transform parent)
        {
            GameObject area = new GameObject("PreviewArea", typeof(RectTransform), typeof(Image));
            area.transform.SetParent(parent, false);
            
            // 设置锚点：顶部居中，固定高度
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1); // 左下角锚点
            rect.anchorMax = new Vector2(0.5f, 1); // 右上角锚点
            rect.pivot = new Vector2(0.5f, 1);     // 中心点在顶部
            rect.sizeDelta = new Vector2(560, 300); // 宽度560，高度300
            rect.anchoredPosition = new Vector2(0, -20); // 距离顶部20px
            
            Image image = area.GetComponent<Image>();
            image.color = new Color(1f, 0.894f, 0.925f); // #FFE4EC 浅粉色
            
            // 角色图片（居中）
            GameObject charImage = new GameObject("CharacterImage", typeof(RectTransform), typeof(Image));
            charImage.transform.SetParent(area.transform, false);
            
            RectTransform charRect = charImage.GetComponent<RectTransform>();
            charRect.anchorMin = new Vector2(0.5f, 0.5f);
            charRect.anchorMax = new Vector2(0.5f, 0.5f);
            charRect.pivot = new Vector2(0.5f, 0.5f);
            charRect.sizeDelta = new Vector2(200, 200);
            charRect.anchoredPosition = Vector2.zero;
            
            // 旋转按钮 - 左（在预览区左侧内部）
            CreateCircleButton(area.transform, "RotateLeftBtn", "◀", 
                new Vector2(30, 0), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            
            // 旋转按钮 - 右（在预览区右侧内部）
            CreateCircleButton(area.transform, "RotateRightBtn", "▶", 
                new Vector2(-30, 0), new Vector2(1, 0.5f), new Vector2(1, 0.5f));
            
            return area;
        }
        
        private GameObject CreateSlotsArea(Transform parent)
        {
            GameObject area = new GameObject("SlotsArea", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            area.transform.SetParent(parent, false);
            
            // 设置锚点：在预览区下方，固定高度
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(560, 140);
            rect.anchoredPosition = new Vector2(0, -340); // 300(预览区) + 20(间距) = 320，再加20内边距
            
            HorizontalLayoutGroup layout = area.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            
            // 创建5个槽位
            string[] slotNames = { "头饰", "帽子", "眼镜", "围巾", "背饰" };
            for (int i = 0; i < 5; i++)
            {
                CreateSlot(area.transform, i, slotNames[i]);
            }
            
            return area;
        }
        
        private void CreateSlot(Transform parent, int index, string name)
        {
            GameObject slot = new GameObject($"Slot_{index}", typeof(RectTransform), typeof(Image), typeof(Button));
            slot.transform.SetParent(parent, false);
            
            RectTransform rect = slot.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 120);
            
            Image image = slot.GetComponent<Image>();
            image.color = new Color(0.95f, 0.95f, 0.95f);
            
            // 图标区域
            GameObject iconArea = new GameObject("IconArea", typeof(RectTransform), typeof(Image));
            iconArea.transform.SetParent(slot.transform, false);
            
            RectTransform iconRect = iconArea.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 0.65f);
            iconRect.anchorMax = new Vector2(0.5f, 0.65f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.sizeDelta = new Vector2(70, 70);
            
            Image iconBg = iconArea.GetComponent<Image>();
            iconBg.color = new Color(1f, 1f, 1f, 0.5f);
            
            // 图标
            GameObject icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            icon.transform.SetParent(iconArea.transform, false);
            
            RectTransform iconImgRect = icon.GetComponent<RectTransform>();
            iconImgRect.anchorMin = Vector2.zero;
            iconImgRect.anchorMax = Vector2.one;
            iconImgRect.offsetMin = new Vector2(5, 5);
            iconImgRect.offsetMax = new Vector2(-5, -5);
            
            // 名称文本（底部）
            GameObject nameObj = new GameObject("Name", typeof(RectTransform), typeof(Text));
            nameObj.transform.SetParent(slot.transform, false);
            
            Text nameText = nameObj.GetComponent<Text>();
            nameText.text = name;
            nameText.fontSize = 16;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = new Color(0.4f, 0.4f, 0.4f);
            
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 0);
            nameRect.pivot = new Vector2(0.5f, 0);
            nameRect.sizeDelta = new Vector2(0, 30);
            nameRect.anchoredPosition = new Vector2(0, 5);
        }
        
        private GameObject CreateDescriptionArea(Transform parent)
        {
            GameObject area = new GameObject("DescriptionArea", typeof(RectTransform), typeof(Image));
            area.transform.SetParent(parent, false);
            
            // 设置锚点：在槽位区下方，固定高度
            RectTransform rect = area.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.sizeDelta = new Vector2(560, 100);
            rect.anchoredPosition = new Vector2(0, -500); // 340 + 140 + 20 = 500
            
            Image image = area.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.5f);
            
            // 描述文本
            GameObject textObj = new GameObject("DescriptionText", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(area.transform, false);
            
            Text text = textObj.GetComponent<Text>();
            text.text = "点击槽位查看装扮详情";
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.365f, 0.251f, 0.216f);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(15, 15);
            textRect.offsetMax = new Vector2(-15, -15);
            
            return area;
        }
        
        private void CreateCircleButton(Transform parent, string name, string text, 
            Vector2 anchoredPosition, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject btn = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            btn.transform.SetParent(parent, false);
            
            RectTransform rect = btn.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(40, 40);
            rect.anchoredPosition = anchoredPosition;
            
            Image image = btn.GetComponent<Image>();
            image.color = new Color(0.659f, 0.902f, 0.812f);
            
            // 文字
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btn.transform, false);
            
            Text txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.fontSize = 16;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        private void ConfigureScript(PlayerEquipPage script, GameObject previewArea, GameObject slotsArea, GameObject descArea)
        {
            SerializedObject so = new SerializedObject(script);
            
            // 角色预览
            Transform charImage = previewArea.transform.Find("CharacterImage");
            if (charImage != null)
                so.FindProperty("characterPreview").objectReferenceValue = charImage.GetComponent<Image>();
            
            // 旋转按钮
            Transform rotateLeft = previewArea.transform.Find("RotateLeftBtn");
            if (rotateLeft != null)
                so.FindProperty("rotateLeftBtn").objectReferenceValue = rotateLeft.GetComponent<Button>();
            
            Transform rotateRight = previewArea.transform.Find("RotateRightBtn");
            if (rotateRight != null)
                so.FindProperty("rotateRightBtn").objectReferenceValue = rotateRight.GetComponent<Button>();
            
            // 槽位容器
            so.FindProperty("slotContainer").objectReferenceValue = slotsArea.transform;
            
            // 描述文本
            Transform descText = descArea.transform.Find("DescriptionText");
            if (descText != null)
                so.FindProperty("descriptionText").objectReferenceValue = descText.GetComponent<Text>();
            
            so.ApplyModifiedProperties();
        }
    }
}
