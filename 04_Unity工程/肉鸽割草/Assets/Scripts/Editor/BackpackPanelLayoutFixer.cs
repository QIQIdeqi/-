using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// BackpackPanel 列表布局修复工具
    /// </summary>
    public class BackpackPanelLayoutFixer : EditorWindow
    {
        [MenuItem("绒毛几何物语/修复/BackpackPanel 列表布局")]
        public static void ShowWindow()
        {
            GetWindow<BackpackPanelLayoutFixer>("修复列表布局");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ BackpackPanel 列表布局修复 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具会检查并修复 Content 的布局设置，\n" +
                "确保 GridLayoutGroup 能正确排列列表项。", 
                MessageType.Info);
            
            GUILayout.Space(10);
            
            GUI.backgroundColor = new Color(0.4f, 0.7f, 1f);
            if (GUILayout.Button("查找并修复 BackpackPanel 布局", GUILayout.Height(50)))
            {
                FindAndFixBackpackPanel();
            }
            GUI.backgroundColor = Color.white;
        }
        
        private void FindAndFixBackpackPanel()
        {
            // 首先尝试查找场景中的实例
            var panel = FindObjectOfType<FluffyGeometry.UI.BackpackPanel>();
            
            if (panel != null)
            {
                // 修复场景中的实例
                FixPanelInstance(panel, "场景中的 BackpackPanel");
                return;
            }
            
            // 如果没有实例，尝试查找预制体
            string[] guids = AssetDatabase.FindAssets("t:Prefab BackpackPanel", new[] { "Assets" });
            
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    // 实例化预制体进行修复
                    GameObject instance = Instantiate(prefab);
                    panel = instance.GetComponent<FluffyGeometry.UI.BackpackPanel>();
                    
                    if (panel != null)
                    {
                        FixPanelInstance(panel, "预制体 " + prefab.name);
                        
                        // 保存回预制体
                        PrefabUtility.SaveAsPrefabAsset(instance, path);
                        Debug.Log("[BackpackPanelLayoutFixer] 已保存回预制体: " + path);
                    }
                    
                    DestroyImmediate(instance);
                    return;
                }
            }
            
            EditorUtility.DisplayDialog("错误", "场景中和预制体中都没有找到 BackpackPanel！", "确定");
        }
        
        private void FixPanelInstance(FluffyGeometry.UI.BackpackPanel panel, string name)
        {
            Debug.Log($"[BackpackPanelLayoutFixer] 修复 {name}");
            
            Undo.RecordObject(panel, "Fix BackpackPanel Layout");
            
            // 获取 outfitListContainer
            var outfitContainer = panel.outfitListContainer;
            if (outfitContainer != null)
            {
                FixContentLayout(outfitContainer, "主角装扮列表");
            }
            else
            {
                Debug.LogWarning("[BackpackPanelLayoutFixer] outfitListContainer 为 null");
            }
            
            // 获取 furnitureListContainer
            var furnitureContainer = panel.furnitureListContainer;
            if (furnitureContainer != null)
            {
                FixContentLayout(furnitureContainer, "家具列表");
            }
            else
            {
                Debug.LogWarning("[BackpackPanelLayoutFixer] furnitureListContainer 为 null");
            }
            
            EditorUtility.SetDirty(panel);
            
            Debug.Log("[BackpackPanelLayoutFixer] 布局修复完成");
            EditorUtility.DisplayDialog("完成", $"{name} 的列表布局已修复！\n\n请重新运行游戏查看效果。", "确定");
        }
        
        private void FixContentLayout(Transform content, string name)
        {
            if (content == null) return;
            
            Debug.Log($"[BackpackPanelLayoutFixer] 修复 {name}: {content.name}");
            
            // 1. 确保有 RectTransform
            RectTransform rect = content.GetComponent<RectTransform>();
            if (rect == null)
            {
                rect = content.gameObject.AddComponent<RectTransform>();
                Debug.Log($"  - 添加 RectTransform");
            }
            
            // 2. 设置 RectTransform（关键！）
            // 顶部对齐，宽度填满，高度自适应
            rect.anchorMin = new Vector2(0, 1);  // 左上角
            rect.anchorMax = new Vector2(1, 1);  // 右上角
            rect.pivot = new Vector2(0.5f, 1);   // 顶部中心
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0, 0);  // 宽度填满，高度由 ContentSizeFitter 控制
            
            Debug.Log($"  - 设置 RectTransform: anchorMin={rect.anchorMin}, anchorMax={rect.anchorMax}, pivot={rect.pivot}");
            
            // 3. 确保有 GridLayoutGroup
            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            if (grid == null)
            {
                grid = content.gameObject.AddComponent<GridLayoutGroup>();
                Debug.Log($"  - 添加 GridLayoutGroup");
            }
            
            // 设置 GridLayoutGroup
            grid.cellSize = new Vector2(100, 100);
            grid.spacing = new Vector2(15, 15);
            grid.padding = new RectOffset(15, 15, 15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            
            Debug.Log($"  - 设置 GridLayoutGroup: cellSize={grid.cellSize}, spacing={grid.spacing}");
            
            // 4. 确保有 ContentSizeFitter
            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = content.gameObject.AddComponent<ContentSizeFitter>();
                Debug.Log($"  - 添加 ContentSizeFitter");
            }
            
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            Debug.Log($"  - 设置 ContentSizeFitter: verticalFit={fitter.verticalFit}");
            
            // 5. 检查父级是否有 ScrollRect
            ScrollRect scrollRect = content.GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.content = rect;
                Debug.Log($"  - 绑定到 ScrollRect: {scrollRect.name}");
            }
            
            EditorUtility.SetDirty(content.gameObject);
        }
    }
}
