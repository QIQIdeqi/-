using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// BackpackPanel 层级修复工具 - 修复页签点击问题
    /// </summary>
    public class BackpackPanelFixer : EditorWindow
    {
        [MenuItem("绒毛几何物语/工具/修复BackpackPanel层级")]
        public static void ShowWindow()
        {
            GetWindow<BackpackPanelFixer>("修复BackpackPanel");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🔧 BackpackPanel 层级修复", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "修复页签按钮被遮挡无法点击的问题：\n" +
                "• 确保页签在最上层\n" +
                "• 修复按钮的Raycast设置\n" +
                "• 调整页面层级", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("修复场景中的BackpackPanel", GUILayout.Height(40)))
            {
                FixBackpackPanelInScene();
            }
        }
        
        private void FixBackpackPanelInScene()
        {
            BackpackPanel[] panels = FindObjectsOfType<BackpackPanel>(true);
            
            if (panels.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "场景中没有找到BackpackPanel！", "确定");
                return;
            }
            
            int fixedCount = 0;
            
            foreach (var panel in panels)
            {
                if (panel == null) continue;
                
                Transform content = panel.transform.Find("Content");
                if (content == null) continue;
                
                // 1. 找到Tabs并移到最上层
                Transform tabs = content.Find("Tabs");
                if (tabs != null)
                {
                    tabs.SetAsLastSibling();
                    
                    // 确保每个页签按钮可点击
                    foreach (Transform tab in tabs)
                    {
                        Button btn = tab.GetComponent<Button>();
                        if (btn != null)
                        {
                            btn.enabled = true;
                            
                            // 确保Image的raycastTarget启用
                            Image img = tab.GetComponent<Image>();
                            if (img != null)
                            {
                                img.raycastTarget = true;
                            }
                        }
                        
                        // 确保Text不阻挡点击
                        Text txt = tab.GetComponentInChildren<Text>();
                        if (txt != null)
                        {
                            txt.raycastTarget = false;
                        }
                    }
                    
                    Debug.Log($"[BackpackPanelFixer] 已修复 Tabs 层级: {panel.name}");
                    fixedCount++;
                }
                
                // 2. 确保页面在Tabs下面
                Transform playerPage = content.Find("PlayerEquipPage");
                Transform outfitPage = content.Find("HomeOutfitPage");
                
                if (playerPage != null)
                {
                    // 确保页面的Image不接收点击事件（避免阻挡页签）
                    Image pageImg = playerPage.GetComponent<Image>();
                    if (pageImg != null)
                    {
                        pageImg.raycastTarget = false;
                    }
                }
                
                if (outfitPage != null)
                {
                    Image pageImg = outfitPage.GetComponent<Image>();
                    if (pageImg != null)
                    {
                        pageImg.raycastTarget = false;
                    }
                }
            }
            
            EditorUtility.DisplayDialog("完成", $"已修复 {fixedCount} 个 BackpackPanel！", "确定");
        }
    }
}
