using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using FluffyGeometry.Home;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园管理器 - 管理家园场景的所有功能
    /// </summary>
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager Instance { get; private set; }
        
        [Header("【场景设置】")]
        [Tooltip("主菜单场景的名称，点击离开时会加载此场景")]
        [SerializeField] private string mainMenuSceneName = "GameScene"; // 主菜单场景名
        
        [Header("【玩家设置】")]
        [Tooltip("玩家角色预制体")]
        [SerializeField] private GameObject playerPrefab;
        
        [Tooltip("玩家进入家园时的出生位置")]
        [SerializeField] private Transform playerSpawnPoint;
        
        [Tooltip("虚拟摇杆（用于玩家移动控制）")]
        [SerializeField] private Joystick joystick;
        
        [Header("【背包系统】")]
        [Tooltip("背包按钮预制体")]
        [SerializeField] private GameObject backpackButtonPrefab;
        
        [Tooltip("背包按钮父节点（留空则使用Canvas）")]
        [SerializeField] private Transform backpackButtonParent;
        
        [Tooltip("家具编辑控制器预制体")]
        [SerializeField] private FurnitureEditController furnitureEditControllerPrefab;
        
        [Header("【装饰物】")]
        [Tooltip("家园中的所有装饰物列表")]
        [SerializeField] private List<HomeDecoration> decorations = new List<HomeDecoration>();
        
        [Header("【NPC和门】")]
        [Tooltip("家园中的NPC（用于打开装扮界面）")]
        [SerializeField] private HomeNPC homeNPC;
        
        [Tooltip("家园之门（用于返回主菜单）")]
        [SerializeField] private HomeDoor homeDoor;
        
        private PlayerController player;
        private FluffyGeometry.UI.BackpackButton backpackButton;
        private FurnitureEditController currentEditController;
        private const string DECORATION_SAVE_KEY = "HomeDecorations";
        private const string PLACED_FURNITURE_SAVE_KEY = "PlacedFurnitureData";
        
        // 装饰物编辑
        private HomeDecoration currentEditingDecoration;
        private GameObject decorationEditToolbar;
        private Button decoFlipBtn;
        private Button decoConfirmBtn;
        private Button decoRecallBtn; // 收回按钮
        private Slider decoScaleSlider;
        private Text decoScaleValueText;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            // 生成玩家
            SpawnPlayer();
            
            // 加载装饰物位置（场景预置的）
            LoadDecorationPositions();
            
            // 加载并重新生成已放置的家具（从背包摆放的）
            LoadAndSpawnPlacedFurniture();
            
            // 同步场景中的初始装饰物到库存系统（标记为已放置）
            SyncInitialDecorationsToInventory();
            
            // 创建背包按钮
            CreateBackpackButton();
        }
        
        /// <summary>
        /// 同步场景中的初始装饰物到库存系统
        /// 根据场景中实际存在的家具数量，修正 placedCounts
        /// </summary>
        private void SyncInitialDecorationsToInventory()
        {
            if (FurnitureInventory.Instance == null) return;
            
            // 统计场景中实际存在的每种家具数量
            Dictionary<string, int> actualCounts = new Dictionary<string, int>();
            foreach (var decoration in decorations)
            {
                if (decoration == null) continue;
                
                if (actualCounts.ContainsKey(decoration.decorationId))
                {
                    actualCounts[decoration.decorationId]++;
                }
                else
                {
                    actualCounts[decoration.decorationId] = 1;
                }
            }
            
            // 获取所有可能有 placed count 的家具ID
            HashSet<string> allIds = new HashSet<string>(FurnitureInventory.Instance.GetAllFurnitureIds());
            foreach (var id in actualCounts.Keys)
            {
                allIds.Add(id);
            }
            
            // 修正 placedCounts
            foreach (var furnitureId in allIds)
            {
                int actualCount = actualCounts.ContainsKey(furnitureId) ? actualCounts[furnitureId] : 0;
                int savedPlacedCount = FurnitureInventory.Instance.GetPlacedCount(furnitureId);
                int totalCount = FurnitureInventory.Instance.GetTotalCount(furnitureId);
                
                // 如果场景中实际数量与 placed count 不一致，进行修正
                if (actualCount != savedPlacedCount)
                {
                    Debug.Log($"[HomeManager] 修正 placed count: {furnitureId} {savedPlacedCount} -> {actualCount} (场景中实际数量)");
                    FurnitureInventory.Instance.SetPlacedCount(furnitureId, actualCount);
                }
                
                // 如果总数量小于实际数量，增加总数量（这种情况不应该发生，但做保护）
                if (totalCount < actualCount)
                {
                    Debug.Log($"[HomeManager] 修正 total count: {furnitureId} {totalCount} -> {actualCount}");
                    FurnitureInventory.Instance.SetFurnitureCount(furnitureId, actualCount);
                }
            }
        }
        
        /// <summary>
        /// 创建背包按钮
        /// </summary>
        private void CreateBackpackButton()
        {
            if (backpackButtonPrefab == null) return;
            
            Transform parent = backpackButtonParent;
            if (parent == null)
            {
                // 查找Canvas作为父节点
                var canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    parent = canvas.transform;
                }
            }
            
            if (parent != null)
            {
                var btnObj = Instantiate(backpackButtonPrefab, parent);
                backpackButton = btnObj.GetComponent<FluffyGeometry.UI.BackpackButton>();
            }
        }
        
        /// <summary>
        /// 生成玩家
        /// </summary>
        private void SpawnPlayer()
        {
            if (playerPrefab == null) return;
            
            Vector3 spawnPos = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
            GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player = playerObj.GetComponent<PlayerController>();
            
            if (player != null)
            {
                // 设置虚拟摇杆
                if (joystick != null)
                {
                    player.joystick = joystick;
                }
                else
                {
                    // 自动查找场景中的摇杆
                    player.joystick = FindObjectOfType<Joystick>();
                }
                
                // 应用当前装扮
                ApplyPlayerOutfit();
            }
        }
        
        /// <summary>
        /// 应用玩家装扮
        /// </summary>
        private void ApplyPlayerOutfit()
        {
            if (OutfitManager.Instance != null && player != null)
            {
                var outfitApplier = player.GetComponent<PlayerOutfitApplier>();
                if (outfitApplier != null)
                {
                    outfitApplier.ApplyOutfit(OutfitManager.Instance.GetAllEquippedParts());
                }
            }
        }
        
        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void ReturnToMainMenu()
        {
            Debug.Log("[HomeManager] 返回主菜单");
            SceneManager.LoadScene(mainMenuSceneName);
        }
        
        /// <summary>
        /// 保存装饰物位置（用于场景中预置的装饰物）
        /// </summary>
        public void SaveDecorationPosition(string decorationId, Vector3 position)
        {
            string key = $"{DECORATION_SAVE_KEY}_{decorationId}";
            string posString = $"{position.x},{position.y},{position.z}";
            PlayerPrefs.SetString(key, posString);
            PlayerPrefs.Save();
            
            Debug.Log($"[HomeManager] 保存装饰物位置: {decorationId} = {position}");
        }
        
        /// <summary>
        /// 加载装饰物位置（用于场景中预置的装饰物）
        /// </summary>
        private void LoadDecorationPositions()
        {
            if (decorations == null || decorations.Count == 0) return;
            
            foreach (var deco in decorations)
            {
                if (deco == null) continue;
                
                string key = $"{DECORATION_SAVE_KEY}_{deco.decorationId}";
                string savedPos = PlayerPrefs.GetString(key, "");
                
                if (!string.IsNullOrEmpty(savedPos))
                {
                    string[] parts = savedPos.Split(',');
                    if (parts.Length == 3)
                    {
                        if (float.TryParse(parts[0], out float x) &&
                            float.TryParse(parts[1], out float y) &&
                            float.TryParse(parts[2], out float z))
                        {
                            deco.transform.position = new Vector3(x, y, z);
                            Debug.Log($"[HomeManager] 加载装饰物位置: {deco.decorationId} = {deco.transform.position}");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 保存已放置的家具数据（从背包动态摆放的）
        /// </summary>
        private void SavePlacedFurniture()
        {
            List<string> saveData = new List<string>();
            
            foreach (var deco in decorations)
            {
                if (deco == null) continue;
                
                // 跳过场景中预置的装饰物（通过检查是否是运行时创建的）
                // 这里简单处理：保存所有装饰物的完整数据
                string data = $"{deco.decorationId},{deco.transform.position.x},{deco.transform.position.y},{deco.transform.position.z},{deco.transform.localScale.x},{deco.transform.localScale.y},{deco.GetComponent<SpriteRenderer>()?.sortingOrder ?? 10}";
                saveData.Add(data);
            }
            
            PlayerPrefs.SetString(PLACED_FURNITURE_SAVE_KEY, string.Join("|", saveData));
            PlayerPrefs.Save();
            
            Debug.Log($"[HomeManager] 保存已放置家具: {saveData.Count} 个");
        }
        
        /// <summary>
        /// 加载并重新生成已放置的家具
        /// </summary>
        private void LoadAndSpawnPlacedFurniture()
        {
            string savedData = PlayerPrefs.GetString(PLACED_FURNITURE_SAVE_KEY, "");
            if (string.IsNullOrEmpty(savedData)) return;
            
            string[] entries = savedData.Split('|');
            
            foreach (var entry in entries)
            {
                string[] parts = entry.Split(',');
                if (parts.Length >= 7)
                {
                    string furnitureId = parts[0];
                    
                    // 检查是否已经在场景中存在（避免重复创建）
                    bool alreadyExists = false;
                    foreach (var existing in decorations)
                    {
                        if (existing != null && existing.decorationId == furnitureId)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }
                    
                    if (alreadyExists) continue;
                    
                    // 解析位置和缩放
                    if (float.TryParse(parts[1], out float px) &&
                        float.TryParse(parts[2], out float py) &&
                        float.TryParse(parts[3], out float pz) &&
                        float.TryParse(parts[4], out float sx) &&
                        float.TryParse(parts[5], out float sy) &&
                        int.TryParse(parts[6], out int sortOrder))
                    {
                        // 从Resources加载家具数据
                        var furnitureData = Resources.Load<FurnitureData>($"Furniture/{furnitureId}");
                        if (furnitureData == null)
                        {
                            // 尝试直接加载
                            var allFurniture = Resources.LoadAll<FurnitureData>("Furniture");
                            foreach (var f in allFurniture)
                            {
                                if (f.furnitureId == furnitureId)
                                {
                                    furnitureData = f;
                                    break;
                                }
                            }
                        }
                        
                        if (furnitureData != null)
                        {
                            // 重新创建家具
                            SpawnSavedFurniture(furnitureData, new Vector3(px, py, pz), new Vector3(sx, sy, 1), sortOrder);
                        }
                    }
                }
            }
            
            Debug.Log($"[HomeManager] 加载已放置家具: {entries.Length} 个");
        }
        
        /// <summary>
        /// 保存所有家具数据（公共接口供 HomeDecoration 调用）
        /// </summary>
        public void SaveAllFurnitureData()
        {
            SavePlacedFurniture();
        }
        
        /// <summary>
        /// 重新生成保存的家具
        /// </summary>
        private void SpawnSavedFurniture(FurnitureData furnitureData, Vector3 position, Vector3 scale, int sortOrder)
        {
            var furnitureObj = new GameObject($"Furniture_{furnitureData.furnitureName}");
            furnitureObj.transform.position = position;
            furnitureObj.transform.localScale = scale;
            
            // 添加SpriteRenderer
            var spriteRenderer = furnitureObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = furnitureData.furnitureSprite;
            spriteRenderer.sortingLayerName = "Furniture";
            spriteRenderer.sortingOrder = sortOrder;
            
            // 添加HomeDecoration组件
            var decoration = furnitureObj.AddComponent<HomeDecoration>();
            decoration.decorationId = furnitureData.furnitureId;
            decoration.canDrag = true;
            
            // 添加到列表
            AddDecoration(decoration);
            
            Debug.Log($"[HomeManager] 重新生成家具: {furnitureData.furnitureName} at {position}");
        }
        
        /// <summary>
        /// 添加新的装饰物
        /// </summary>
        public void AddDecoration(HomeDecoration decoration)
        {
            if (!decorations.Contains(decoration))
            {
                decorations.Add(decoration);
            }
        }
        
        /// <summary>
        /// 移除装饰物
        /// </summary>
        public void RemoveDecoration(HomeDecoration decoration)
        {
            decorations.Remove(decoration);
        }
        
        /// <summary>
        /// 清除所有家具（从场景中删除）
        /// </summary>
        public void ClearAllFurniture()
        {
            Debug.Log($"[HomeManager] 清除所有家具，共 {decorations.Count} 个");
            
            // 复制列表避免遍历时修改
            List<HomeDecoration> toRemove = new List<HomeDecoration>(decorations);
            
            foreach (var decoration in toRemove)
            {
                if (decoration != null && decoration.gameObject != null)
                {
                    Destroy(decoration.gameObject);
                }
            }
            
            decorations.Clear();
            
            // 同时清除保存的家具数据
            PlayerPrefs.DeleteKey(PLACED_FURNITURE_SAVE_KEY);
            PlayerPrefs.Save();
            
            Debug.Log("[HomeManager] 所有家具已清除");
        }
        
        /// <summary>
        /// 进入家具编辑模式
        /// </summary>
        public void EnterFurnitureEditMode(FurnitureData furniture, BackpackPanel backpackPanel)
        {
            if (furnitureEditControllerPrefab == null) return;
            
            // 暂停玩家移动
            if (player != null)
            {
                player.enabled = false;
            }
            
            // 创建编辑控制器
            var editController = Instantiate(furnitureEditControllerPrefab);
            currentEditController = editController.GetComponent<FurnitureEditController>();
            
            if (currentEditController != null)
            {
                currentEditController.Initialize(furniture, backpackPanel);
                currentEditController.OnConfirm = () => OnFurnitureEditConfirm(furniture);
            }
        }
        
        /// <summary>
        /// 家具编辑确认
        /// </summary>
        private void OnFurnitureEditConfirm(FurnitureData furnitureData)
        {
            if (currentEditController == null) return;
            
            // 获取放置数据
            var placedData = currentEditController.GetPlacedData();
            if (placedData == null) return;
            
            // 创建正式的家具装饰物（内部会扣减库存）
            PlaceFurniture(furnitureData, placedData);
            
            // 恢复玩家移动
            if (player != null)
            {
                player.enabled = true;
            }
            
            // 注意：背包面板会在 FurnitureEditController 中自动重新打开
            // 重新打开时会刷新列表，数量显示会自动更新
            
            currentEditController = null;
        }
        
        /// <summary>
        /// 放置家具到场景中
        /// </summary>
        private void PlaceFurniture(FurnitureData furnitureData, PlacedFurnitureData placedData)
        {
            // 检查是否可以放置（有可用数量）
            if (FurnitureInventory.Instance != null)
            {
                if (!FurnitureInventory.Instance.CanPlace(furnitureData.furnitureId))
                {
                    Debug.LogWarning($"[HomeManager] 家具数量不足，无法放置: {furnitureData.furnitureName}");
                    return;
                }
            }
            
            // 创建家具GameObject
            var furnitureObj = new GameObject($"Furniture_{furnitureData.furnitureName}");
            furnitureObj.transform.position = placedData.position;
            furnitureObj.transform.localScale = Vector3.one * placedData.scale;
            
            // 如果有翻转
            if (placedData.isFlipped)
            {
                Vector3 scale = furnitureObj.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                furnitureObj.transform.localScale = scale;
            }
            
            // 添加SpriteRenderer
            var spriteRenderer = furnitureObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = furnitureData.furnitureSprite;
            spriteRenderer.sortingLayerName = "Furniture";
            spriteRenderer.sortingOrder = 10;
            
            // 添加HomeDecoration组件用于保存位置
            var decoration = furnitureObj.AddComponent<HomeDecoration>();
            decoration.decorationId = furnitureData.furnitureId;
            decoration.canDrag = true;
            
            // 添加到列表
            AddDecoration(decoration);
            
            // 保存位置
            SaveDecorationPosition(decoration.decorationId, placedData.position);
            
            // 扣减库存数量
            if (FurnitureInventory.Instance != null)
            {
                FurnitureInventory.Instance.OnFurniturePlaced(furnitureData.furnitureId);
            }
            
            // 保存所有已放置家具（用于下次场景加载时重新生成）
            SavePlacedFurniture();
            
            Debug.Log($"[HomeManager] 放置家具: {furnitureData.furnitureName} at {placedData.position}");
        }
        
        /// <summary>
        /// 取消家具编辑
        /// </summary>
        public void CancelFurnitureEdit()
        {
            if (currentEditController != null)
            {
                currentEditController.Cancel();
                currentEditController = null;
            }
            
            // 恢复玩家移动
            if (player != null)
            {
                player.enabled = true;
            }
        }
        
        #region 装饰物编辑模式
        
        /// <summary>
        /// 进入装饰物编辑模式
        /// </summary>
        public void EnterDecorationEditMode(HomeDecoration decoration)
        {
            if (currentEditingDecoration != null && currentEditingDecoration != decoration)
            {
                ExitDecorationEditMode();
            }
            
            currentEditingDecoration = decoration;
            decoration.EnterEditMode();
            
            // 显示编辑工具栏
            ShowDecorationEditToolbar(decoration);
            
            // 暂停玩家移动
            if (player != null)
            {
                player.enabled = false;
            }
        }
        
        /// <summary>
        /// 退出装饰物编辑模式
        /// </summary>
        public void ExitDecorationEditMode()
        {
            if (currentEditingDecoration != null)
            {
                currentEditingDecoration.ExitEditMode();
                currentEditingDecoration = null;
            }
            
            // 隐藏编辑工具栏
            HideDecorationEditToolbar();
            
            // 恢复玩家移动
            if (player != null)
            {
                player.enabled = true;
            }
        }
        
        /// <summary>
        /// 显示装饰物编辑工具栏
        /// </summary>
        private void ShowDecorationEditToolbar(HomeDecoration decoration)
        {
            // 创建工具栏（如果不存在或需要重建）
            if (decorationEditToolbar == null || decoRecallBtn == null)
            {
                // 如果存在旧的，先销毁
                if (decorationEditToolbar != null)
                {
                    Destroy(decorationEditToolbar.transform.parent.gameObject);
                    decorationEditToolbar = null;
                }
                CreateDecorationEditToolbar();
            }
            
            if (decorationEditToolbar != null)
            {
                decorationEditToolbar.SetActive(true);
                UpdateDecorationToolbarPosition(decoration);
            }
        }
        
        /// <summary>
        /// 隐藏装饰物编辑工具栏
        /// </summary>
        private void HideDecorationEditToolbar()
        {
            if (decorationEditToolbar != null)
            {
                decorationEditToolbar.SetActive(false);
            }
        }
        
        /// <summary>
        /// 创建装饰物编辑工具栏
        /// </summary>
        private void CreateDecorationEditToolbar()
        {
            // 创建画布
            GameObject canvasObj = new GameObject("DecorationEditToolbarCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.transform.SetParent(transform);
            
            // 工具栏面板
            decorationEditToolbar = new GameObject("ToolbarPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            decorationEditToolbar.transform.SetParent(canvasObj.transform);
            
            var rect = decorationEditToolbar.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(380, 60); // 增加宽度容纳收回按钮
            
            decorationEditToolbar.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            // 水平布局
            var layout = decorationEditToolbar.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.spacing = 10;
            layout.childAlignment = TextAnchor.MiddleCenter;
            
            // 翻转按钮
            decoFlipBtn = CreateToolbarButton(decorationEditToolbar.transform, "翻转", new Color(0.4f, 0.6f, 1f));
            decoFlipBtn.onClick.AddListener(OnDecorationFlip);
            
            // 缩放滑条
            CreateDecorationScaleSlider(decorationEditToolbar.transform);
            
            // 确认按钮
            decoConfirmBtn = CreateToolbarButton(decorationEditToolbar.transform, "确认", new Color(0.4f, 0.8f, 0.4f));
            decoConfirmBtn.onClick.AddListener(OnDecorationConfirm);
            
            // 收回按钮（添加到背包）
            decoRecallBtn = CreateToolbarButton(decorationEditToolbar.transform, "收回", new Color(0.8f, 0.4f, 0.4f));
            decoRecallBtn.onClick.AddListener(OnDecorationRecall);
        }
        
        /// <summary>
        /// 创建装饰物缩放滑条
        /// </summary>
        private void CreateDecorationScaleSlider(Transform parent)
        {
            GameObject container = new GameObject("ScaleSliderContainer", typeof(RectTransform));
            container.transform.SetParent(parent);
            var containerRect = container.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(100, 40);
            
            // 滑条
            GameObject sliderObj = new GameObject("ScaleSlider", typeof(RectTransform), typeof(Slider));
            sliderObj.transform.SetParent(container.transform);
            var sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0, 0.5f);
            sliderRect.anchorMax = new Vector2(1, 0.5f);
            sliderRect.pivot = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = new Vector2(0, -5);
            sliderRect.sizeDelta = new Vector2(-10, 15);
            
            decoScaleSlider = sliderObj.GetComponent<Slider>();
            decoScaleSlider.minValue = 0.5f;
            decoScaleSlider.maxValue = 2f;
            decoScaleSlider.value = 1f;
            decoScaleSlider.onValueChanged.AddListener(OnDecorationScaleChanged);
            
            // Background
            GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            bgObj.transform.SetParent(sliderObj.transform);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgObj.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f);
            
            // Fill Area
            GameObject fillAreaObj = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaObj.transform.SetParent(sliderObj.transform);
            var fillAreaRect = fillAreaObj.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5, 0);
            fillAreaRect.offsetMax = new Vector2(-5, 0);
            
            GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            fillObj.transform.SetParent(fillAreaObj.transform);
            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            fillObj.GetComponent<Image>().color = new Color(0.4f, 0.8f, 0.4f);
            
            // Handle Area
            GameObject handleAreaObj = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleAreaObj.transform.SetParent(sliderObj.transform);
            var handleAreaRect = handleAreaObj.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);
            
            GameObject handleObj = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            handleObj.transform.SetParent(handleAreaObj.transform);
            handleObj.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 25);
            handleObj.GetComponent<Image>().color = Color.white;
            
            decoScaleSlider.fillRect = fillRect;
            decoScaleSlider.handleRect = handleObj.GetComponent<RectTransform>();
            decoScaleSlider.targetGraphic = handleObj.GetComponent<Image>();
            
            // 数值文本
            GameObject valueObj = new GameObject("ScaleValue", typeof(RectTransform), typeof(Text));
            valueObj.transform.SetParent(container.transform);
            var valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0, 1);
            valueRect.anchorMax = new Vector2(1, 1);
            valueRect.pivot = new Vector2(0.5f, 1);
            valueRect.anchoredPosition = new Vector2(0, 0);
            valueRect.sizeDelta = new Vector2(0, 15);
            
            decoScaleValueText = valueObj.GetComponent<Text>();
            decoScaleValueText.text = "1.0x";
            decoScaleValueText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            decoScaleValueText.fontSize = 12;
            decoScaleValueText.alignment = TextAnchor.MiddleCenter;
            decoScaleValueText.color = Color.white;
        }
        
        /// <summary>
        /// 装饰物缩放改变
        /// </summary>
        private void OnDecorationScaleChanged(float value)
        {
            if (currentEditingDecoration != null)
            {
                currentEditingDecoration.SetScale(value);
            }
            if (decoScaleValueText != null)
            {
                decoScaleValueText.text = $"{value:F1}x";
            }
        }
        
        /// <summary>
        /// 更新工具栏位置（跟随装饰物）
        /// </summary>
        private void UpdateDecorationToolbarPosition(HomeDecoration decoration)
        {
            if (decorationEditToolbar == null || decoration == null) return;
            
            // 将装饰物世界坐标转换为屏幕坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(decoration.transform.position);
            screenPos.y += 80f; // 在装饰物上方
            
            // 限制在屏幕内
            screenPos.x = Mathf.Clamp(screenPos.x, 150, Screen.width - 150);
            screenPos.y = Mathf.Clamp(screenPos.y, 50, Screen.height - 50);
            
            decorationEditToolbar.transform.position = screenPos;
        }
        
        private void Update()
        {
            // 更新工具栏位置
            if (currentEditingDecoration != null && decorationEditToolbar != null && decorationEditToolbar.activeInHierarchy)
            {
                UpdateDecorationToolbarPosition(currentEditingDecoration);
            }
        }
        
        /// <summary>
        /// 翻转按钮点击
        /// </summary>
        private void OnDecorationFlip()
        {
            if (currentEditingDecoration != null)
            {
                currentEditingDecoration.Flip();
            }
        }
        
        /// <summary>
        /// 确认按钮点击
        /// </summary>
        private void OnDecorationConfirm()
        {
            if (currentEditingDecoration != null)
            {
                currentEditingDecoration.ConfirmEdit();
            }
            ExitDecorationEditMode();
        }
        
        /// <summary>
        /// 收回按钮点击 - 将家具收回背包
        /// </summary>
        private void OnDecorationRecall()
        {
            if (currentEditingDecoration == null) return;
            
            string furnitureId = currentEditingDecoration.decorationId;
            
            Debug.Log($"[HomeManager] 收回家具到背包: {furnitureId}");
            
            // 从库存中减少已放置计数（相当于放回背包）
            if (FurnitureInventory.Instance != null)
            {
                FurnitureInventory.Instance.OnFurnitureRemoved(furnitureId);
            }
            
            // 从列表中移除
            RemoveDecoration(currentEditingDecoration);
            
            // 删除游戏对象
            Destroy(currentEditingDecoration.gameObject);
            
            // 清除保存的位置数据
            PlayerPrefs.DeleteKey($"{DECORATION_SAVE_KEY}_{furnitureId}");
            
            // 重新保存所有家具数据
            SavePlacedFurniture();
            
            // 退出编辑模式
            ExitDecorationEditMode();
            
            Debug.Log($"[HomeManager] 家具 {furnitureId} 已收回背包");
        }
        
        /// <summary>
        /// 检查装饰物编辑工具栏是否激活
        /// </summary>
        public bool IsDecorationEditToolbarActive()
        {
            return decorationEditToolbar != null && decorationEditToolbar.activeInHierarchy;
        }
        
        /// <summary>
        /// 创建工具栏按钮
        /// </summary>
        private UnityEngine.UI.Button CreateToolbarButton(Transform parent, string text, Color color)
        {
            GameObject btnObj = new GameObject($"{text}Btn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(UnityEngine.UI.Button));
            btnObj.transform.SetParent(parent);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(80, 40);
            
            btnObj.GetComponent<Image>().color = color;
            
            // 文本
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(btnObj.transform);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);
            
            var txt = textObj.GetComponent<Text>();
            txt.text = text;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 18;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            
            return btnObj.GetComponent<UnityEngine.UI.Button>();
        }
        
        #endregion
    }
}
