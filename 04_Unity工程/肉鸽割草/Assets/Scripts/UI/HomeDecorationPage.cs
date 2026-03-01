using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FluffyGeometry.Home;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园布置页面 - 显示家具/装饰品列表
    /// </summary>
    public class HomeDecorationPage : MonoBehaviour
    {
        [Header("【分类标签】")]
        [Tooltip("分类标签容器")] public Transform categoryContainer;
        [Tooltip("分类标签预制体")] public GameObject categoryTabPrefab;
        
        [Header("【家具列表】")]
        [Tooltip("家具列表ScrollRect")] public ScrollRect furnitureScrollRect;
        [Tooltip("家具项预制体")] public GameObject furnitureItemPrefab;
        [Tooltip("家具项容器")] public Transform furnitureContainer;
        
        [Header("【预览区域】")]
        [Tooltip("家园预览图片")] public Image homePreview;
        
        [Header("【样式配置】")]
        [Tooltip("选中的标签颜色")] public Color selectedTabColor = new Color(1f, 0.72f, 0.77f);
        [Tooltip("未选中标签颜色")] public Color unselectedTabColor = new Color(1f, 0.96f, 0.97f);
        
        // 家具类别 - 使用现有的 FurnitureCategory
        private List<FurnitureCategory> categories = new List<FurnitureCategory>
        {
            FurnitureCategory.座椅,
            FurnitureCategory.桌子,
            FurnitureCategory.装饰,
            FurnitureCategory.灯具,
            FurnitureCategory.植物
        };
        
        private Dictionary<FurnitureCategory, string> categoryNames = new Dictionary<FurnitureCategory, string>
        {
            { FurnitureCategory.座椅, "座椅" },
            { FurnitureCategory.桌子, "桌子" },
            { FurnitureCategory.装饰, "装饰" },
            { FurnitureCategory.灯具, "灯具" },
            { FurnitureCategory.墙面, "墙面" },
            { FurnitureCategory.地面, "地面" },
            { FurnitureCategory.植物, "植物" },
            { FurnitureCategory.其他, "其他" }
        };
        
        private FurnitureCategory currentCategory;
        private List<GameObject> categoryTabs = new List<GameObject>();
        private List<GameObject> furnitureItems = new List<GameObject>();
        
        void Start()
        {
            InitializeCategoryTabs();
        }
        
        void OnEnable()
        {
            RefreshDisplay();
        }
        
        /// <summary>
        /// 初始化分类标签
        /// </summary>
        private void InitializeCategoryTabs()
        {
            // 清除旧标签
            foreach (var tab in categoryTabs)
            {
                if (tab != null) Destroy(tab);
            }
            categoryTabs.Clear();
            
            // 创建新标签
            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];
                CreateCategoryTab(category, i);
            }
            
            // 默认选中第一个
            if (categories.Count > 0)
                SelectCategory(categories[0]);
        }
        
        /// <summary>
        /// 创建单个分类标签
        /// </summary>
        private void CreateCategoryTab(FurnitureCategory category, int index)
        {
            if (categoryTabPrefab == null || categoryContainer == null) return;
            
            GameObject tabObj = Instantiate(categoryTabPrefab, categoryContainer);
            categoryTabs.Add(tabObj);
            
            // 设置文本
            var text = tabObj.GetComponentInChildren<Text>();
            if (text != null)
                text.text = categoryNames[category];
            
            // 绑定点击事件
            var button = tabObj.GetComponent<Button>();
            if (button != null)
            {
                var captureCategory = category;
                button.onClick.AddListener(() => SelectCategory(captureCategory));
            }
            
            // 初始状态
            UpdateTabVisual(tabObj, index == 0);
        }
        
        /// <summary>
        /// 选择分类
        /// </summary>
        private void SelectCategory(FurnitureCategory category)
        {
            currentCategory = category;
            
            // 更新标签视觉
            for (int i = 0; i < categoryTabs.Count; i++)
            {
                UpdateTabVisual(categoryTabs[i], categories[i] == category);
            }
            
            // 刷新列表
            RefreshFurnitureList();
        }
        
        /// <summary>
        /// 更新标签视觉
        /// </summary>
        private void UpdateTabVisual(GameObject tabObj, bool isSelected)
        {
            var image = tabObj.GetComponent<Image>();
            if (image != null)
            {
                image.color = isSelected ? selectedTabColor : unselectedTabColor;
                tabObj.transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
            }
        }
        
        /// <summary>
        /// 刷新家具列表
        /// </summary>
        private void RefreshFurnitureList()
        {
            // 清除旧项
            foreach (var item in furnitureItems)
            {
                if (item != null) Destroy(item);
            }
            furnitureItems.Clear();
            
            // 从 HomeManager 获取家具列表
            // 这里需要 HomeManager 提供按类别获取家具的方法
            var allFurniture = HomeManager.Instance?.GetUnlockedFurnitureList();
            if (allFurniture == null) return;
            
            // 过滤当前类别的家具
            List<FurnitureData> categoryFurniture = new List<FurnitureData>();
            foreach (var furniture in allFurniture)
            {
                if (furniture != null && furniture.category == currentCategory)
                {
                    categoryFurniture.Add(furniture);
                }
            }
            
            for (int i = 0; i < categoryFurniture.Count; i++)
            {
                CreateFurnitureItem(categoryFurniture[i], i);
            }
        }
        
        /// <summary>
        /// 创建单个家具项
        /// </summary>
        private void CreateFurnitureItem(FurnitureData furniture, int index)
        {
            if (furnitureItemPrefab == null || furnitureContainer == null) return;
            
            GameObject itemObj = Instantiate(furnitureItemPrefab, furnitureContainer);
            furnitureItems.Add(itemObj);
            
            // 设置图标
            var iconImage = itemObj.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null && furniture.iconSprite != null)
                iconImage.sprite = furniture.iconSprite;
            
            // 设置名称
            var nameText = itemObj.transform.Find("Name")?.GetComponent<Text>();
            if (nameText != null)
                nameText.text = furniture.furnitureName;
            
            // 检查解锁状态
            bool isUnlocked = furniture.isUnlocked;
            
            // 设置状态标签
            var statusObj = itemObj.transform.Find("Status")?.gameObject;
            var statusText = statusObj?.GetComponent<Text>();
            if (statusText != null)
            {
                if (!isUnlocked)
                {
                    statusText.text = "未解锁";
                    statusObj.SetActive(true);
                }
                else
                {
                    statusObj.SetActive(false);
                }
            }
            
            // 设置背景色
            var bgImage = itemObj.GetComponent<Image>();
            if (bgImage != null)
            {
                bgImage.color = isUnlocked ? Color.white : new Color(0.8f, 0.8f, 0.8f);
            }
            
            // 点击事件
            var button = itemObj.GetComponent<Button>();
            if (button != null)
            {
                var captureFurniture = furniture;
                button.onClick.AddListener(() => OnFurnitureClick(captureFurniture));
            }
            
            // 入场动画
            StartCoroutine(ItemEntryAnimation(itemObj, index));
        }
        
        /// <summary>
        /// 家具项入场动画
        /// </summary>
        private IEnumerator ItemEntryAnimation(GameObject itemObj, int index)
        {
            if (itemObj == null) yield break;
            
            itemObj.transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(index * 0.03f);
            
            if (itemObj == null) yield break;
            
            // 简单缩放动画
            float timer = 0;
            while (timer < 0.3f)
            {
                if (itemObj == null) yield break;
                
                timer += Time.deltaTime;
                float t = timer / 0.3f;
                float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
                itemObj.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            
            if (itemObj != null)
                itemObj.transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// 点击家具项
        /// </summary>
        private void OnFurnitureClick(FurnitureData furniture)
        {
            if (furniture == null) return;
            
            if (!furniture.isUnlocked)
            {
                Debug.Log($"[HomeDecorationPage] 家具未解锁: {furniture.furnitureName}");
                return;
            }
            
            // 已解锁，进入布置模式
            EnterPlacementMode(furniture);
        }
        
        /// <summary>
        /// 进入布置模式
        /// </summary>
        private void EnterPlacementMode(FurnitureData furniture)
        {
            Debug.Log($"[HomeDecorationPage] 进入布置模式: {furniture.furnitureName}");
            
            // 关闭背包
            BackpackPanel.Instance?.Hide();
            
            // 调用 HomeManager 开始编辑家具
            HomeManager.Instance?.StartFurniturePlacement(furniture);
            
            // 触发布置事件
            OnEnterPlacementMode?.Invoke(furniture);
        }
        
        /// <summary>
        /// 刷新整个页面
        /// </summary>
        public void RefreshDisplay()
        {
            RefreshFurnitureList();
        }
        
        // 事件：进入布置模式
        public System.Action<FurnitureData> OnEnterPlacementMode;
    }
}
