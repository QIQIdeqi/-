using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园布置页面内容管理器
    /// 管理家具/装饰品的显示和选择
    /// </summary>
    public class HomeOutfitPageContent : MonoBehaviour
    {
        [Header("【家具布置页面】")]
        [Tooltip("HomeDecorationPage 预制体或物体")] public GameObject decorationPagePrefab;
        [Tooltip("面板容器")] public Transform panelContainer;
        
        private GameObject currentDecorationPage;
        private HomeDecorationPage decorationPageScript;
        
        void Start()
        {
            InitializePanel();
        }
        
        void OnEnable()
        {
            // 每次显示时刷新
            RefreshPanel();
        }
        
        /// <summary>
        /// 初始化布置面板
        /// </summary>
        private void InitializePanel()
        {
            if (panelContainer == null)
            {
                panelContainer = transform;
            }
            
            // 清除旧的
            if (currentDecorationPage != null)
            {
                Destroy(currentDecorationPage);
            }
            
            // 检查是否已有 HomeDecorationPage 组件
            decorationPageScript = GetComponentInChildren<HomeDecorationPage>(true);
            if (decorationPageScript != null)
            {
                currentDecorationPage = decorationPageScript.gameObject;
                return;
            }
            
            // 如果没有，创建一个新的
            GameObject pageObj = new GameObject("HomeDecorationPage", typeof(RectTransform));
            pageObj.transform.SetParent(panelContainer, false);
            
            RectTransform rect = pageObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            
            // 添加 HomeDecorationPage 脚本
            decorationPageScript = pageObj.AddComponent<HomeDecorationPage>();
            
            // 创建基础UI结构
            CreateDecorationPageUI(pageObj.transform, decorationPageScript);
            
            currentDecorationPage = pageObj;
        }
        
        /// <summary>
        /// 创建家园布置页面的UI结构
        /// </summary>
        private void CreateDecorationPageUI(Transform parent, HomeDecorationPage script)
        {
            // 创建分类标签容器
            GameObject tabsObj = new GameObject("CategoryTabs", typeof(RectTransform));
            tabsObj.transform.SetParent(parent, false);
            
            RectTransform tabsRect = tabsObj.GetComponent<RectTransform>();
            tabsRect.anchorMin = new Vector2(0, 1);
            tabsRect.anchorMax = new Vector2(1, 1);
            tabsRect.pivot = new Vector2(0.5f, 1);
            tabsRect.sizeDelta = new Vector2(0, 100);
            tabsRect.anchoredPosition = new Vector2(0, -80);
            
            HorizontalLayoutGroup tabsLayout = tabsObj.AddComponent<HorizontalLayoutGroup>();
            tabsLayout.spacing = 10;
            tabsLayout.childAlignment = TextAnchor.MiddleCenter;
            tabsLayout.childControlWidth = false;
            tabsLayout.childControlHeight = false;
            tabsLayout.childForceExpandWidth = false;
            tabsLayout.childForceExpandHeight = false;
            
            script.categoryContainer = tabsObj.transform;
            
            // 创建家具列表区域
            GameObject listObj = new GameObject("FurnitureList", typeof(RectTransform), typeof(ScrollRect), typeof(Image));
            listObj.transform.SetParent(parent, false);
            
            RectTransform listRect = listObj.GetComponent<RectTransform>();
            listRect.anchorMin = Vector2.zero;
            listRect.anchorMax = Vector2.one;
            listRect.offsetMin = new Vector2(20, 20);
            listRect.offsetMax = new Vector2(-20, -120);
            
            Image listBg = listObj.GetComponent<Image>();
            listBg.color = new Color(1f, 1f, 1f, 0.3f);
            
            // Viewport
            GameObject viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image));
            viewport.transform.SetParent(listObj.transform, false);
            
            RectTransform vpRect = viewport.GetComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero;
            vpRect.anchorMax = Vector2.one;
            vpRect.offsetMin = new Vector2(10, 10);
            vpRect.offsetMax = new Vector2(-10, -10);
            
            // Content
            GameObject content = new GameObject("Content", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(viewport.transform, false);
            
            GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(120, 160);
            grid.spacing = new Vector2(15, 15);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            
            ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = Vector2.zero;
            
            // 配置 ScrollRect
            ScrollRect scroll = listObj.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = vpRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            
            script.furnitureScrollRect = scroll;
            script.furnitureContainer = content.transform;
            
            // 提示文本
            GameObject hintObj = new GameObject("HintText", typeof(RectTransform), typeof(Text));
            hintObj.transform.SetParent(parent, false);
            
            RectTransform hintRect = hintObj.GetComponent<RectTransform>();
            hintRect.anchorMin = new Vector2(0.5f, 0);
            hintRect.anchorMax = new Vector2(0.5f, 0);
            hintRect.pivot = new Vector2(0.5f, 0);
            hintRect.sizeDelta = new Vector2(400, 40);
            hintRect.anchoredPosition = new Vector2(0, 20);
            
            Text hintText = hintObj.GetComponent<Text>();
            hintText.text = "";
            hintText.fontSize = 20;
            hintText.alignment = TextAnchor.MiddleCenter;
            hintText.color = new Color(0.365f, 0.251f, 0.216f);
            hintText.font = FontLoader.GetSimHeiFont();
            
            Debug.Log("[HomeOutfitPageContent] 家园布置页面UI创建完成");
        }
        
        /// <summary>
        /// 刷新面板
        /// </summary>
        private void RefreshPanel()
        {
            if (decorationPageScript != null)
            {
                decorationPageScript.RefreshDisplay();
            }
        }
    }
}
