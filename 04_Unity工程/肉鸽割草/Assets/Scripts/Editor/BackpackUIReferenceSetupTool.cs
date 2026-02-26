using UnityEngine;
using UnityEditor;

namespace FluffyGeometry.Editor
{
    /// <summary>
    /// 背包UI引用配置工具
    /// 自动配置BackpackPanel、FurnitureItemUI、FurnitureEditController之间的引用
    /// </summary>
    public class BackpackUIReferenceSetupTool : EditorWindow
    {
        private GameObject backpackPanelPrefab;
        private GameObject furnitureItemPrefab;
        private GameObject furnitureEditControllerPrefab;
        
        private Vector2 scrollPosition;
        
        [MenuItem("绒毛几何物语/背包UI引用配置")]
        public static void ShowWindow()
        {
            var window = GetWindow<BackpackUIReferenceSetupTool>("背包UI引用配置");
            window.minSize = new Vector2(400, 400);
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
            GUILayout.Label("🎒 背包UI引用配置", titleStyle);
            
            GUILayout.Space(5);
            
            GUIStyle descStyle = new GUIStyle(EditorStyles.label);
            descStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("配置背包面板、家具项、编辑控制器之间的引用", descStyle);
            
            GUILayout.Space(20);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            // 拖入预制体
            GUILayout.Label("📦 拖入预制体", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            EditorGUILayout.HelpBox(
                "请先运行'背包系统一键配置'创建预制体，\n然后在这里拖入对应的预制体文件。", 
                MessageType.Info
            );
            
            GUILayout.Space(10);
            
            backpackPanelPrefab = EditorGUILayout.ObjectField(
                "BackpackPanel 预制体", 
                backpackPanelPrefab, 
                typeof(GameObject), 
                false
            ) as GameObject;
            
            furnitureItemPrefab = EditorGUILayout.ObjectField(
                "FurnitureItem 预制体", 
                furnitureItemPrefab, 
                typeof(GameObject), 
                false
            ) as GameObject;
            
            furnitureEditControllerPrefab = EditorGUILayout.ObjectField(
                "FurnitureEditController 预制体", 
                furnitureEditControllerPrefab, 
                typeof(GameObject), 
                false
            ) as GameObject;
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            
            // 当前状态
            GUILayout.Label("📊 当前配置状态", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (backpackPanelPrefab != null)
            {
                var panel = backpackPanelPrefab.GetComponent<FluffyGeometry.UI.BackpackPanel>();
                if (panel != null)
                {
                    EditorGUILayout.LabelField("BackpackPanel:", "✅ 已找到脚本");
                    EditorGUILayout.LabelField("  - Furniture Item Prefab:", panel.furnitureItemPrefab != null ? "✅ 已配置" : "❌ 未配置");
                    EditorGUILayout.LabelField("  - Outfit Item Prefab:", panel.outfitItemPrefab != null ? "✅ 已配置" : "❌ 未配置");
                }
                else
                {
                    EditorGUILayout.HelpBox("BackpackPanel 预制体缺少 BackpackPanel 脚本！", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.LabelField("BackpackPanel:", "等待拖入...");
            }
            
            GUILayout.Space(5);
            
            if (furnitureItemPrefab != null)
            {
                var item = furnitureItemPrefab.GetComponent<FluffyGeometry.UI.FurnitureItemUI>();
                if (item != null)
                {
                    EditorGUILayout.LabelField("FurnitureItem:", "✅ 已找到脚本");
                }
                else
                {
                    EditorGUILayout.HelpBox("FurnitureItem 预制体缺少 FurnitureItemUI 脚本！", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.LabelField("FurnitureItem:", "等待拖入...");
            }
            
            GUILayout.Space(5);
            
            if (furnitureEditControllerPrefab != null)
            {
                var controller = furnitureEditControllerPrefab.GetComponent<FluffyGeometry.Home.FurnitureEditController>();
                if (controller != null)
                {
                    EditorGUILayout.LabelField("FurnitureEditController:", "✅ 已找到脚本");
                    EditorGUILayout.LabelField("  - Toolbar Panel:", controller.toolbarPanel != null ? "✅ 已配置" : "❌ 未配置");
                }
                else
                {
                    EditorGUILayout.HelpBox("FurnitureEditController 预制体缺少 FurnitureEditController 脚本！", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.LabelField("FurnitureEditController:", "等待拖入...");
            }
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15);
            
            // 配置说明
            GUILayout.Label("🔧 将自动配置", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("1. BackpackPanel.furnitureItemPrefab = FurnitureItem 预制体");
            GUILayout.Label("2. FurnitureEditController 子物体引用（FlipBtn, ScaleSlider, ConfirmBtn）");
            GUILayout.Label("3. FurnitureItemUI 子物体引用（Icon, Name, SelectedBorder, DecorateBtn）");
            EditorGUILayout.EndVertical();
            
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            
            // 一键配置按钮
            bool canConfigure = backpackPanelPrefab != null && furnitureItemPrefab != null && furnitureEditControllerPrefab != null;
            GUI.enabled = canConfigure;
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 16;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fixedHeight = 50;
            
            if (GUILayout.Button("🚀 一键配置引用", buttonStyle))
            {
                ConfigureReferences();
            }
            
            GUI.enabled = true;
            
            GUILayout.Space(10);
            
            if (!canConfigure)
            {
                EditorGUILayout.HelpBox("请拖入所有三个预制体后再点击配置", MessageType.Warning);
            }
        }
        
        /// <summary>
        /// 配置所有引用
        /// </summary>
        private void ConfigureReferences()
        {
            if (backpackPanelPrefab == null || furnitureItemPrefab == null || furnitureEditControllerPrefab == null)
            {
                EditorUtility.DisplayDialog("配置失败", "请先拖入所有预制体！", "确定");
                return;
            }
            
            Undo.RecordObject(backpackPanelPrefab, "Configure Backpack References");
            Undo.RecordObject(furnitureEditControllerPrefab, "Configure Backpack References");
            
            string log = "=== 背包UI引用配置日志 ===\n";
            int configuredCount = 0;
            
            // 1. 配置 BackpackPanel
            var panel = backpackPanelPrefab.GetComponent<FluffyGeometry.UI.BackpackPanel>();
            if (panel != null)
            {
                // 配置 furnitureItemPrefab
                var furnitureItem = furnitureItemPrefab.GetComponent<FluffyGeometry.UI.FurnitureItemUI>();
                if (furnitureItem != null)
                {
                    panel.furnitureItemPrefab = furnitureItem;
                    log += "✅ BackpackPanel.furnitureItemPrefab\n";
                    configuredCount++;
                }
                else
                {
                    log += "❌ FurnitureItem 预制体缺少 FurnitureItemUI 脚本\n";
                }
                
                EditorUtility.SetDirty(backpackPanelPrefab);
            }
            else
            {
                log += "❌ BackpackPanel 预制体缺少 BackpackPanel 脚本\n";
            }
            
            // 2. 配置 FurnitureEditController
            var editController = furnitureEditControllerPrefab.GetComponent<FluffyGeometry.Home.FurnitureEditController>();
            if (editController != null)
            {
                configuredCount += ConfigureFurnitureEditController(editController, ref log);
                EditorUtility.SetDirty(furnitureEditControllerPrefab);
            }
            else
            {
                log += "❌ FurnitureEditController 预制体缺少脚本\n";
            }
            
            // 3. 配置 FurnitureItemUI
            var furnitureItemUI = furnitureItemPrefab.GetComponent<FluffyGeometry.UI.FurnitureItemUI>();
            if (furnitureItemUI != null)
            {
                configuredCount += ConfigureFurnitureItemUI(furnitureItemUI, ref log);
                EditorUtility.SetDirty(furnitureItemPrefab);
            }
            
            AssetDatabase.SaveAssets();
            
            Debug.Log(log);
            
            EditorUtility.DisplayDialog("配置完成", 
                $"成功配置 {configuredCount} 项引用！\n\n{log}", 
                "确定");
        }
        
        /// <summary>
        /// 配置 FurnitureEditController
        /// </summary>
        private int ConfigureFurnitureEditController(FluffyGeometry.Home.FurnitureEditController controller, ref string log)
        {
            int count = 0;
            
            // 查找 ToolbarPanel
            Transform toolbarPanel = furnitureEditControllerPrefab.transform.Find("ToolbarCanvas/ToolbarPanel");
            if (toolbarPanel == null)
            {
                toolbarPanel = furnitureEditControllerPrefab.transform.Find("ToolbarPanel");
            }
            
            if (toolbarPanel != null)
            {
                controller.toolbarPanel = toolbarPanel.gameObject;
                log += "✅ FurnitureEditController.toolbarPanel\n";
                count++;
                
                // 查找子按钮
                Transform flipBtn = toolbarPanel.Find("FlipBtn");
                if (flipBtn != null)
                {
                    controller.flipBtn = flipBtn.GetComponent<UnityEngine.UI.Button>();
                    log += "✅ FurnitureEditController.flipBtn\n";
                    count++;
                }
                
                Transform confirmBtn = toolbarPanel.Find("ConfirmBtn");
                if (confirmBtn != null)
                {
                    controller.confirmBtn = confirmBtn.GetComponent<UnityEngine.UI.Button>();
                    log += "✅ FurnitureEditController.confirmBtn\n";
                    count++;
                }
                
                Transform sliderContainer = toolbarPanel.Find("ScaleSliderContainer");
                if (sliderContainer != null)
                {
                    Transform slider = sliderContainer.Find("ScaleSlider");
                    if (slider != null)
                    {
                        controller.scaleSlider = slider.GetComponent<UnityEngine.UI.Slider>();
                        log += "✅ FurnitureEditController.scaleSlider\n";
                        count++;
                    }
                    
                    Transform valueText = sliderContainer.Find("ScaleValue");
                    if (valueText != null)
                    {
                        controller.scaleValueText = valueText.GetComponent<UnityEngine.UI.Text>();
                        log += "✅ FurnitureEditController.scaleValueText\n";
                        count++;
                    }
                }
            }
            else
            {
                log += "❌ 未找到 ToolbarPanel\n";
            }
            
            return count;
        }
        
        /// <summary>
        /// 配置 FurnitureItemUI
        /// </summary>
        private int ConfigureFurnitureItemUI(FluffyGeometry.UI.FurnitureItemUI itemUI, ref string log)
        {
            int count = 0;
            
            Transform icon = furnitureItemPrefab.transform.Find("Icon");
            if (icon != null)
            {
                itemUI.furnitureIcon = icon.GetComponent<UnityEngine.UI.Image>();
                log += "✅ FurnitureItemUI.furnitureIcon\n";
                count++;
            }
            
            Transform nameText = furnitureItemPrefab.transform.Find("Name");
            if (nameText != null)
            {
                itemUI.furnitureNameText = nameText.GetComponent<UnityEngine.UI.Text>();
                log += "✅ FurnitureItemUI.furnitureNameText\n";
                count++;
            }
            
            Transform selectedBorder = furnitureItemPrefab.transform.Find("SelectedBorder");
            if (selectedBorder != null)
            {
                itemUI.selectedBorder = selectedBorder.gameObject;
                log += "✅ FurnitureItemUI.selectedBorder\n";
                count++;
            }
            
            Transform decorateBtn = furnitureItemPrefab.transform.Find("DecorateBtn");
            if (decorateBtn != null)
            {
                itemUI.decorateBtn = decorateBtn.GetComponent<UnityEngine.UI.Button>();
                
                Transform decorateText = decorateBtn.Find("Text");
                if (decorateText != null)
                {
                    itemUI.decorateBtnText = decorateText.GetComponent<UnityEngine.UI.Text>();
                    log += "✅ FurnitureItemUI.decorateBtnText\n";
                    count++;
                }
                
                log += "✅ FurnitureItemUI.decorateBtn\n";
                count++;
            }
            
            return count;
        }
    }
}
