using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FluffyGeometry.Home;
using GeometryWarrior;

namespace FluffyGeometry.UI
{
    /// <summary>
    /// 背包面板 - 整合主角装扮和家园装扮
    /// </summary>
    public class BackpackPanel : MonoBehaviour
    {
        [Header("【UI引用 - 主面板】")]
        [Tooltip("关闭按钮")]
        public Button closeBtn;
        
        [Tooltip("Tab容器")]
        public Transform tabContainer;
        
        [Tooltip("主角装扮Tab按钮")]
        public Button characterTabBtn;
        
        [Tooltip("家园装扮Tab按钮")]
        public Button furnitureTabBtn;
        
        [Tooltip("主角装扮内容区")]
        public GameObject characterContent;
        
        [Tooltip("家园装扮内容区")]
        public GameObject furnitureContent;
        
        [Header("【UI引用 - 主角装扮】")]
        [Tooltip("装扮部件列表容器")]
        public Transform outfitListContainer;
        
        [Tooltip("装扮部件项预制体")]
        public OutfitItemUI outfitItemPrefab;
        
        [Tooltip("分类标签容器")]
        public Transform categoryContainer;
        
        [Header("【UI引用 - 家园装扮】")]
        [Tooltip("家具列表容器")]
        public Transform furnitureListContainer;
        
        [Tooltip("家具项预制体")]
        public FurnitureItemUI furnitureItemPrefab;
        
        [Header("【配置】")]
        [Tooltip("当前选中的Tab索引")]
        public int currentTab = 0; // 0=主角, 1=家园
        
        [Tooltip("列表项间距")]
        public float itemSpacing = 10f;
        
        [Header("【状态】")]
        [Tooltip("背包按钮引用")]
        public BackpackButton backpackButton;
        
        [Tooltip("家具列表滚动位置")]
        public Vector2 furnitureScrollPos;
        
        // 回调：选择家具进行装饰
        public System.Action<FurnitureData> OnDecorateFurniture;
        
        // 所有装扮部件数据
        private List<OutfitPartData> allOutfitParts = new List<OutfitPartData>();
        
        // 所有家具数据
        private List<FurnitureData> allFurniture = new List<FurnitureData>();
        
        // 装扮项列表
        private List<OutfitItemUI> outfitItems = new List<OutfitItemUI>();
        
        // 家具项列表
        private List<FurnitureItemUI> furnitureItems = new List<FurnitureItemUI>();
        
        private ScrollRect furnitureScrollRect;
        
        public void Initialize(BackpackButton button)
        {
            backpackButton = button;
            
            // 绑定关闭按钮
            if (closeBtn != null)
            {
                closeBtn.onClick.AddListener(OnCloseClick);
            }
            
            // 绑定Tab按钮
            if (characterTabBtn != null)
            {
                characterTabBtn.onClick.AddListener(() => SwitchTab(0));
            }
            if (furnitureTabBtn != null)
            {
                furnitureTabBtn.onClick.AddListener(() => SwitchTab(1));
            }
            
            // 获取滚动组件
            if (furnitureContent != null)
            {
                furnitureScrollRect = furnitureContent.GetComponentInParent<ScrollRect>();
            }
            
            // 加载数据
            LoadData();
        }
        
        private void OnDestroy()
        {
            if (closeBtn != null)
            {
                closeBtn.onClick.RemoveListener(OnCloseClick);
            }
            if (characterTabBtn != null)
            {
                characterTabBtn.onClick.RemoveAllListeners();
            }
            if (furnitureTabBtn != null)
            {
                furnitureTabBtn.onClick.RemoveAllListeners();
            }
        }
        
        /// <summary>
        /// 显示面板
        /// </summary>
        public void Show(int tabIndex = 0)
        {
            gameObject.SetActive(true);
            SwitchTab(tabIndex);
            
            // 恢复家具列表滚动位置
            if (tabIndex == 1 && furnitureScrollRect != null)
            {
                furnitureScrollRect.normalizedPosition = furnitureScrollPos;
            }
        }
        
        /// <summary>
        /// 隐藏面板
        /// </summary>
        public void Hide()
        {
            // 保存滚动位置
            if (furnitureScrollRect != null)
            {
                furnitureScrollPos = furnitureScrollRect.normalizedPosition;
            }
            
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 关闭按钮点击
        /// </summary>
        private void OnCloseClick()
        {
            Hide();
            if (backpackButton != null)
            {
                backpackButton.OnPanelClosed();
            }
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 切换Tab
        /// </summary>
        public void SwitchTab(int tabIndex)
        {
            currentTab = tabIndex;
            
            // 更新Tab按钮状态
            UpdateTabButtons();
            
            // 显示对应内容
            if (characterContent != null)
            {
                characterContent.SetActive(tabIndex == 0);
            }
            if (furnitureContent != null)
            {
                furnitureContent.SetActive(tabIndex == 1);
            }
            
            // 刷新列表
            if (tabIndex == 0)
            {
                RefreshOutfitList();
            }
            else
            {
                RefreshFurnitureList();
            }
        }
        
        /// <summary>
        /// 更新Tab按钮显示状态
        /// </summary>
        private void UpdateTabButtons()
        {
            // 这里可以设置选中态图片或颜色
            if (characterTabBtn != null)
            {
                var colors = characterTabBtn.colors;
                colors.normalColor = currentTab == 0 ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                characterTabBtn.colors = colors;
            }
            if (furnitureTabBtn != null)
            {
                var colors = furnitureTabBtn.colors;
                colors.normalColor = currentTab == 1 ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                furnitureTabBtn.colors = colors;
            }
        }
        
        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData()
        {
            // 从OutfitManager加载装扮数据
            if (OutfitManager.Instance != null)
            {
                allOutfitParts = OutfitManager.Instance.GetAllParts();
            }
            
            // 从HomeManager或数据管理器加载家具数据
            LoadFurnitureData();
        }
        
        /// <summary>
        /// 加载家具数据
        /// </summary>
        private void LoadFurnitureData()
        {
            // TODO: 从存档或数据管理器加载
            // 这里先使用示例数据
            allFurniture = new List<FurnitureData>();
            
            // 可以从Resources加载所有FurnitureData
            var furnitureArray = Resources.LoadAll<FurnitureData>("Furniture");
            foreach (var furniture in furnitureArray)
            {
                if (furniture.isUnlocked)
                {
                    allFurniture.Add(furniture);
                }
            }
        }
        
        /// <summary>
        /// 刷新装扮列表
        /// </summary>
        private void RefreshOutfitList()
        {
            // 清除旧项
            foreach (var item in outfitItems)
            {
                Destroy(item.gameObject);
            }
            outfitItems.Clear();
            
            if (outfitItemPrefab == null || outfitListContainer == null) return;
            
            // 创建新项
            foreach (var partData in allOutfitParts)
            {
                var item = Instantiate(outfitItemPrefab, outfitListContainer);
                bool isUnlocked = OutfitManager.Instance != null && OutfitManager.Instance.IsPartUnlocked(partData);
                bool isEquipped = OutfitManager.Instance != null && OutfitManager.Instance.GetEquippedPart(partData.category) == partData;
                item.Setup(partData, isUnlocked, isEquipped, OnOutfitItemClick);
                outfitItems.Add(item);
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
                Destroy(item.gameObject);
            }
            furnitureItems.Clear();
            
            if (furnitureItemPrefab == null || furnitureListContainer == null) return;
            
            // 创建新项
            foreach (var furniture in allFurniture)
            {
                var item = Instantiate(furnitureItemPrefab, furnitureListContainer);
                item.Setup(furniture, OnFurnitureItemClick, OnDecorateButtonClick);
                furnitureItems.Add(item);
            }
        }
        
        /// <summary>
        /// 装扮项点击
        /// </summary>
        private void OnOutfitItemClick(OutfitPartData partData)
        {
            // 切换装扮
            if (OutfitManager.Instance != null)
            {
                // 检查是否已装备
                var currentlyEquipped = OutfitManager.Instance.GetEquippedPart(partData.category);
                if (currentlyEquipped == partData)
                {
                    // 已装备，卸下
                    OutfitManager.Instance.UnequipPart(partData.category);
                }
                else
                {
                    // 未装备，装备
                    OutfitManager.Instance.EquipPart(partData);
                }
                
                // 刷新列表显示
                RefreshOutfitList();
            }
        }
        
        /// <summary>
        /// 家具项点击
        /// </summary>
        private void OnFurnitureItemClick(FurnitureData furniture)
        {
            // 显示该家具的详情或选中态
            //  furnitureItems 中处理选中态显示
        }
        
        /// <summary>
        /// 装饰按钮点击 - 进入家具编辑模式
        /// </summary>
        private void OnDecorateButtonClick(FurnitureData furniture)
        {
            // 保存当前滚动位置
            if (furnitureScrollRect != null)
            {
                furnitureScrollPos = furnitureScrollRect.normalizedPosition;
            }
            
            // 关闭背包
            Hide();
            
            // 通知外部进入编辑模式
            OnDecorateFurniture?.Invoke(furniture);
            
            // 通知HomeManager进入编辑模式
            var homeManager = FindObjectOfType<HomeManager>();
            if (homeManager != null)
            {
                homeManager.EnterFurnitureEditMode(furniture, this);
            }
        }
        
        /// <summary>
        /// 重新打开背包（编辑完成后调用）
        /// </summary>
        public void Reopen()
        {
            Show(1); // 回到家园装扮Tab
        }
    }
}
