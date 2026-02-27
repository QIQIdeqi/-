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
            
            // 延迟加载数据，确保 OutfitManager 已初始化
            StartCoroutine(DelayedLoadData());
        }
        
        private System.Collections.IEnumerator DelayedLoadData()
        {
            // 等待一帧，确保 OutfitManager 已初始化
            yield return null;
            
            // 再等待一帧，确保所有单例都已就绪
            if (OutfitManager.Instance == null)
            {
                Debug.Log("[BackpackPanel] 等待 OutfitManager 初始化...");
                yield return new WaitForSeconds(0.1f);
            }
            
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
            
            // 只切换Tab UI，不立即刷新列表（数据可能还没加载）
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
            
            // 恢复家具列表滚动位置
            if (tabIndex == 1 && furnitureScrollRect != null)
            {
                furnitureScrollRect.normalizedPosition = furnitureScrollPos;
            }
            
            // 延迟刷新列表，确保数据已加载（避免显示"空消息"）
            if (tabIndex == 0)
            {
                // 主角装扮 - 延迟刷新
                StartCoroutine(DelayedRefreshOutfitList());
            }
            else
            {
                // 家园装扮 - 延迟刷新
                StartCoroutine(DelayedRefreshFurnitureList());
            }
        }
        
        /// <summary>
        /// 延迟刷新家具列表
        /// </summary>
        private System.Collections.IEnumerator DelayedRefreshFurnitureList()
        {
            // 等待数据加载完成
            int attempts = 0;
            while ((allFurniture == null || allFurniture.Count == 0) && attempts < 10)
            {
                yield return new WaitForSeconds(0.05f);
                attempts++;
                
                // 尝试重新加载数据
                LoadFurnitureData();
            }
            
            Debug.Log($"[BackpackPanel] 延迟刷新家具列表，数据数量: {allFurniture?.Count ?? 0}");
            RefreshFurnitureList();
            
            // 然后刷新数量显示
            yield return null;
            foreach (var item in furnitureItems)
            {
                item.UpdateCountDisplay();
            }
        }
        
        /// <summary>
        /// 延迟刷新装扮列表
        /// </summary>
        private System.Collections.IEnumerator DelayedRefreshOutfitList()
        {
            // 等待数据加载完成
            int attempts = 0;
            while ((allOutfitParts == null || allOutfitParts.Count == 0) && attempts < 10)
            {
                yield return new WaitForSeconds(0.05f);
                attempts++;
                
                // 尝试重新加载数据
                if (OutfitManager.Instance != null)
                {
                    allOutfitParts = OutfitManager.Instance.GetAllParts();
                }
            }
            
            Debug.Log($"[BackpackPanel] 延迟刷新装扮列表，数据数量: {allOutfitParts?.Count ?? 0}");
            RefreshOutfitList();
        }
        
        /// <summary>
        /// 延迟刷新家具数量
        /// </summary>
        private System.Collections.IEnumerator DelayedRefreshFurnitureCounts()
        {
            // 等待一帧，确保 FurnitureInventory 已初始化
            yield return null;
            
            // 刷新所有家具项的数量显示
            foreach (var item in furnitureItems)
            {
                item.UpdateCountDisplay();
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
            Debug.Log("[BackpackPanel] LoadData 开始");
            
            // 从OutfitManager加载装扮数据
            if (OutfitManager.Instance != null)
            {
                allOutfitParts = OutfitManager.Instance.GetAllParts();
                Debug.Log($"[BackpackPanel] 从 OutfitManager 获取到 {allOutfitParts?.Count ?? 0} 个装扮部件");
            }
            else
            {
                Debug.LogError("[BackpackPanel] OutfitManager.Instance 为 null！");
            }
            
            // 从HomeManager或数据管理器加载家具数据
            LoadFurnitureData();
        }
        
        /// <summary>
        /// 加载家具数据
        /// </summary>
        private void LoadFurnitureData()
        {
            allFurniture = new List<FurnitureData>();
            
            // 从Resources加载所有FurnitureData
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
        /// 刷新特定家具的显示（摆放后调用）
        /// </summary>
        public void RefreshFurnitureItem(FurnitureData furniture)
        {
            foreach (var item in furnitureItems)
            {
                if (item.furnitureData == furniture)
                {
                    item.UpdateCountDisplay();
                    break;
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
            
            // 清除空消息提示（如果存在）
            if (outfitListContainer != null)
            {
                Transform emptyMsg = outfitListContainer.Find("EmptyMessage");
                if (emptyMsg != null)
                {
                    Destroy(emptyMsg.gameObject);
                }
            }
            
            Debug.Log($"[BackpackPanel] RefreshOutfitList 开始 - outfitItemPrefab={outfitItemPrefab != null}, outfitListContainer={outfitListContainer != null}");
            
            if (outfitItemPrefab == null || outfitListContainer == null)
            {
                Debug.LogError("[BackpackPanel] outfitItemPrefab 或 outfitListContainer 未设置！请在BackpackPanel预制体上配置这些字段。");
                return;
            }
            
            // 检查是否有数据
            if (allOutfitParts == null || allOutfitParts.Count == 0)
            {
                Debug.LogWarning("[BackpackPanel] 没有装扮部件数据！请检查：\n1. Resources/OutfitParts 文件夹是否存在部件文件\n2. OutfitManager 是否正确加载");
                ShowEmptyOutfitMessage();
                return;
            }
            
            Debug.Log($"[BackpackPanel] 找到 {allOutfitParts.Count} 个部件数据");
            
            // 创建新项
            int nullCount = 0;
            int validCount = 0;
            
            foreach (var partData in allOutfitParts)
            {
                if (partData == null)
                {
                    nullCount++;
                    continue;
                }
                
                validCount++;
                Debug.Log($"[BackpackPanel] 创建部件UI: {partData.partName}");
                
                try
                {
                    var item = Instantiate(outfitItemPrefab, outfitListContainer);
                    if (item == null)
                    {
                        Debug.LogError("[BackpackPanel] Instantiate 返回 null");
                        continue;
                    }
                    
                    // 确保 GameObject 是激活的
                    item.gameObject.SetActive(true);
                    
                    // 设置明显的背景颜色用于调试
                    var image = item.GetComponent<UnityEngine.UI.Image>();
                    if (image != null)
                    {
                        image.color = new Color(0.3f, 0.5f, 0.8f, 1f); // 蓝色背景，调试用
                    }
                    
                    bool isUnlocked = OutfitManager.Instance != null && OutfitManager.Instance.IsPartUnlocked(partData);
                    bool isEquipped = OutfitManager.Instance != null && OutfitManager.Instance.GetEquippedPart(partData.category) == partData;
                    item.Setup(partData, isUnlocked, isEquipped, OnOutfitItemClick);
                    outfitItems.Add(item);
                    
                    Debug.Log($"[BackpackPanel] 部件UI创建成功: {item.name}, 位置: {item.transform.localPosition}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[BackpackPanel] 创建部件UI时出错: {e.Message}\n{e.StackTrace}");
                }
            }
            
            Debug.Log($"[BackpackPanel] 完成: 成功创建 {outfitItems.Count} 个UI项, 跳过 {nullCount} 个null");
            
            Debug.Log($"[BackpackPanel] 刷新了 {outfitItems.Count} 个装扮部件");
        }
        
        /// <summary>
        /// 显示空状态提示
        /// </summary>
        private void ShowEmptyOutfitMessage()
        {
            // 创建提示文本
            GameObject msgObj = new GameObject("EmptyMessage", typeof(RectTransform), typeof(UnityEngine.UI.Text));
            msgObj.transform.SetParent(outfitListContainer, false);
            
            var rect = msgObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 100);
            
            var text = msgObj.GetComponent<UnityEngine.UI.Text>();
            text.text = "还没有装扮部件~\n\n请使用菜单：\n绒毛几何物语 → 快速创建 → 装扮部件";
            text.fontSize = 18;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.7f, 0.7f, 0.7f);
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        /// 家具项点击 - 单选逻辑
        /// </summary>
        private void OnFurnitureItemClick(FurnitureData furniture)
        {
            // 单选：先取消所有其他项的选中状态
            foreach (var item in furnitureItems)
            {
                if (item.furnitureData != furniture)
                {
                    item.SetSelected(false);
                }
            }
            
            // 找到当前点击的项并设置为选中
            foreach (var item in furnitureItems)
            {
                if (item.furnitureData == furniture)
                {
                    item.SetSelected(true);
                    break;
                }
            }
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
