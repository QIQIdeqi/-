using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FluffyGeometry.Home
{
    /// <summary>
    /// 家具编辑控制器 - 拖拽、翻转、缩放、确认
    /// </summary>
    public class FurnitureEditController : MonoBehaviour
    {
        [Header("【UI引用 - 编辑工具栏】")]
        [Tooltip("工具栏面板")]
        public GameObject toolbarPanel;
        
        [Tooltip("翻转按钮")]
        public Button flipBtn;
        
        [Tooltip("缩放滑条")]
        public Slider scaleSlider;
        
        [Tooltip("确认按钮")]
        public Button confirmBtn;
        
        [Tooltip("取消按钮")]
        public Button cancelBtn;
        
        [Tooltip("缩放值显示")]
        public Text scaleValueText;
        
        [Header("【配置】")]
        [Tooltip("拖拽时的透明度")]
        [Range(0f, 1f)]
        public float dragAlpha = 0.8f;
        
        [Tooltip("放置时的缩放动画时间")]
        public float placeAnimDuration = 0.2f;
        
        [Tooltip("工具栏与家具的偏移")]
        public Vector2 toolbarOffset = new Vector2(0, 60f);
        
        [Header("【状态】")]
        [Tooltip("当前编辑的家具数据")]
        public FurnitureData currentFurniture;
        
        [Tooltip("当前编辑的家具实例")]
        public GameObject furnitureInstance;
        
        [Tooltip("家具SpriteRenderer")]
        public SpriteRenderer furnitureRenderer;
        
        [Tooltip("当前缩放")]
        public float currentScale = 1f;
        
        [Tooltip("是否已翻转")]
        public bool isFlipped = false;
        
        [Tooltip("是否正在拖拽")]
        public bool isDragging = false;
        
        // 回调
        public System.Action OnConfirm;
        public System.Action OnCancel;
        
        // 背包面板引用（用于回调）
        private FluffyGeometry.UI.BackpackPanel backpackPanel;
        
        /// <summary>
        /// 获取背包面板引用
        /// </summary>
        public FluffyGeometry.UI.BackpackPanel BackpackPanel => backpackPanel;
        
        // 相机
        private Camera mainCamera;
        
        // 拖拽偏移
        private Vector3 dragOffset;
        
        // 初始位置（用于取消时恢复）
        private Vector3 initialPosition;
        
        public void Initialize(FurnitureData furniture, FluffyGeometry.UI.BackpackPanel panel)
        {
            currentFurniture = furniture;
            backpackPanel = panel;
            mainCamera = Camera.main;
            
            // 创建家具实例
            CreateFurnitureInstance();
            
            // 设置工具栏
            SetupToolbar();
            
            // 绑定按钮事件
            BindEvents();
            
            // 初始位置：屏幕中央
            PositionAtScreenCenter();
        }
        
        private void Update()
        {
            HandleDrag();
            UpdateToolbarPosition();
        }
        
        /// <summary>
        /// 创建家具实例
        /// </summary>
        private void CreateFurnitureInstance()
        {
            furnitureInstance = new GameObject($"Editing_{currentFurniture.furnitureName}");
            furnitureInstance.transform.SetParent(transform);
            
            // 添加SpriteRenderer
            furnitureRenderer = furnitureInstance.AddComponent<SpriteRenderer>();
            furnitureRenderer.sprite = currentFurniture.furnitureSprite;
            furnitureRenderer.sortingLayerName = "Furniture";
            furnitureRenderer.sortingOrder = 100; // 编辑时置顶
            
            // 添加碰撞体用于点击检测
            var collider = furnitureInstance.AddComponent<BoxCollider2D>();
            collider.size = furnitureRenderer.bounds.size;
            
            // 设置初始缩放
            currentScale = currentFurniture.defaultScale;
            furnitureInstance.transform.localScale = Vector3.one * currentScale;
            
            // 设置初始透明度
            var color = furnitureRenderer.color;
            color.a = dragAlpha;
            furnitureRenderer.color = color;
        }
        
        /// <summary>
        /// 设置工具栏
        /// </summary>
        private void SetupToolbar()
        {
            if (toolbarPanel == null) return;
            
            toolbarPanel.SetActive(true);
            
            // 设置缩放滑条
            if (scaleSlider != null)
            {
                scaleSlider.minValue = currentFurniture.minScale;
                scaleSlider.maxValue = currentFurniture.maxScale;
                scaleSlider.value = currentFurniture.defaultScale;
                scaleSlider.onValueChanged.AddListener(OnScaleChanged);
            }
            
            // 更新缩放值显示
            UpdateScaleText();
            
            // 设置翻转按钮
            if (flipBtn != null)
            {
                flipBtn.gameObject.SetActive(currentFurniture.canFlip);
            }
            
            // 如果没有取消按钮，创建一个
            if (cancelBtn == null && toolbarPanel != null)
            {
                CreateCancelButton();
            }
        }
        
        /// <summary>
        /// 创建取消按钮
        /// </summary>
        private void CreateCancelButton()
        {
            // 创建按钮
            GameObject btnObj = new GameObject("CancelBtn", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            btnObj.transform.SetParent(toolbarPanel.transform);
            
            var rect = btnObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(60, 40);
            
            var img = btnObj.GetComponent<Image>();
            img.color = new Color(0.8f, 0.3f, 0.3f); // 红色
            
            // 添加文字
            GameObject txtObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
            txtObj.transform.SetParent(btnObj.transform);
            var txtRect = txtObj.GetComponent<RectTransform>();
            txtRect.anchorMin = Vector2.zero;
            txtRect.anchorMax = Vector2.one;
            txtRect.offsetMin = new Vector2(5, 5);
            txtRect.offsetMax = new Vector2(-5, -5);
            
            var txt = txtObj.GetComponent<Text>();
            txt.text = "取消";
            txt.fontSize = 16;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            cancelBtn = btnObj.GetComponent<Button>();
            cancelBtn.onClick.AddListener(OnCancelClick);
            
            Debug.Log("[FurnitureEditController] 创建取消按钮");
        }
        
        /// <summary>
        /// 绑定按钮事件
        /// </summary>
        private void BindEvents()
        {
            if (flipBtn != null)
            {
                flipBtn.onClick.AddListener(OnFlipClick);
            }
            if (confirmBtn != null)
            {
                confirmBtn.onClick.AddListener(OnConfirmClick);
            }
            if (cancelBtn != null)
            {
                cancelBtn.onClick.AddListener(OnCancelClick);
            }
        }
        
        private void OnDestroy()
        {
            if (scaleSlider != null)
            {
                scaleSlider.onValueChanged.RemoveListener(OnScaleChanged);
            }
            if (flipBtn != null)
            {
                flipBtn.onClick.RemoveListener(OnFlipClick);
            }
            if (confirmBtn != null)
            {
                confirmBtn.onClick.RemoveListener(OnConfirmClick);
            }
            if (cancelBtn != null)
            {
                cancelBtn.onClick.RemoveListener(OnCancelClick);
            }
        }
        
        /// <summary>
        /// 将家具定位到屏幕中央
        /// </summary>
        private void PositionAtScreenCenter()
        {
            if (mainCamera == null || furnitureInstance == null) return;
            
            Vector3 screenCenter = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenCenter);
            worldPos.z = 0;
            
            furnitureInstance.transform.position = worldPos;
            initialPosition = worldPos;
        }
        
        /// <summary>
        /// 处理拖拽
        /// </summary>
        private void HandleDrag()
        {
            if (furnitureInstance == null) return;
            
            // 检查是否点击在UI上，如果是则不处理拖拽
            if (IsPointerOverUI()) return;
            
            // 触摸/鼠标按下
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                
                // 检测是否点击到家具
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
                if (hit.collider != null && hit.collider.gameObject == furnitureInstance)
                {
                    isDragging = true;
                    dragOffset = furnitureInstance.transform.position - mousePos;
                }
            }
            
            // 拖拽中
            if (isDragging && Input.GetMouseButton(0))
            {
                Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                furnitureInstance.transform.position = mousePos + dragOffset;
            }
            
            // 释放
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
        
        /// <summary>
        /// 检查鼠标/触摸是否在UI上
        /// </summary>
        private bool IsPointerOverUI()
        {
            // 检查工具栏是否激活且鼠标在其上
            if (toolbarPanel != null && toolbarPanel.activeInHierarchy)
            {
                // 获取工具栏的RectTransform
                RectTransform toolbarRect = toolbarPanel.GetComponent<RectTransform>();
                if (toolbarRect != null)
                {
                    // 将屏幕坐标转换为本地坐标检查
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        toolbarRect, Input.mousePosition, mainCamera, out localPoint);
                    
                    if (toolbarRect.rect.Contains(localPoint))
                    {
                        return true;
                    }
                }
            }
            
            // 使用EventSystem检查是否在任何UI上
            if (EventSystem.current != null)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
            
            return false;
        }
        
        /// <summary>
        /// 更新工具栏位置（跟随家具）
        /// </summary>
        private void UpdateToolbarPosition()
        {
            if (toolbarPanel == null || furnitureInstance == null) return;
            
            // 将家具世界坐标转换为屏幕坐标
            Vector3 furnitureScreenPos = mainCamera.WorldToScreenPoint(furnitureInstance.transform.position);
            
            // 计算工具栏位置（家具上方）
            Vector3 toolbarPos = furnitureScreenPos + new Vector3(toolbarOffset.x, toolbarOffset.y, 0);
            
            // 限制在屏幕内
            toolbarPos.x = Mathf.Clamp(toolbarPos.x, 100, Screen.width - 100);
            toolbarPos.y = Mathf.Clamp(toolbarPos.y, 100, Screen.height - 100);
            
            toolbarPanel.transform.position = toolbarPos;
        }
        
        /// <summary>
        /// 翻转按钮点击
        /// </summary>
        private void OnFlipClick()
        {
            if (furnitureInstance == null || !currentFurniture.canFlip) return;
            
            isFlipped = !isFlipped;
            Vector3 scale = furnitureInstance.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFlipped ? -1 : 1);
            furnitureInstance.transform.localScale = scale;
        }
        
        /// <summary>
        /// 缩放改变
        /// </summary>
        private void OnScaleChanged(float value)
        {
            if (furnitureInstance == null) return;
            
            currentScale = value;
            Vector3 scale = furnitureInstance.transform.localScale;
            scale.x = currentScale * (isFlipped ? -1 : 1);
            scale.y = currentScale;
            furnitureInstance.transform.localScale = scale;
            
            UpdateScaleText();
        }
        
        /// <summary>
        /// 更新缩放值显示
        /// </summary>
        private void UpdateScaleText()
        {
            if (scaleValueText != null)
            {
                scaleValueText.text = $"{currentScale:F1}x";
            }
        }
        
        /// <summary>
        /// 取消按钮点击
        /// </summary>
        private void OnCancelClick()
        {
            Debug.Log("[FurnitureEditController] 取消放置家具");
            
            // 重新打开背包
            if (backpackPanel != null)
            {
                backpackPanel.Reopen();
            }
            
            // 调用取消
            Cancel();
        }
        
        /// <summary>
        /// 确认按钮点击
        /// </summary>
        private void OnConfirmClick()
        {
            // 恢复透明度
            if (furnitureRenderer != null)
            {
                var color = furnitureRenderer.color;
                color.a = 1f;
                furnitureRenderer.color = color;
            }
            
            // 降低排序层级（从编辑态变为正常态）
            if (furnitureRenderer != null)
            {
                furnitureRenderer.sortingOrder = 10;
            }
            
            // 播放放置动画（可选）
            StartCoroutine(PlaceAnimation());
            
            // 通知确认
            OnConfirm?.Invoke();
            
            // 通知背包重新打开
            if (backpackPanel != null)
            {
                backpackPanel.Reopen();
            }
            
            // 销毁编辑控制器
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 放置动画
        /// </summary>
        private System.Collections.IEnumerator PlaceAnimation()
        {
            if (furnitureInstance == null) yield break;
            
            Vector3 targetScale = furnitureInstance.transform.localScale;
            Vector3 startScale = targetScale * 0.8f;
            
            float elapsed = 0;
            while (elapsed < placeAnimDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / placeAnimDuration;
                furnitureInstance.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            furnitureInstance.transform.localScale = targetScale;
        }
        
        /// <summary>
        /// 获取最终放置数据
        /// </summary>
        public PlacedFurnitureData GetPlacedData()
        {
            if (furnitureInstance == null) return null;
            
            return new PlacedFurnitureData
            {
                furnitureId = currentFurniture.furnitureId,
                position = furnitureInstance.transform.position,
                scale = currentScale,
                isFlipped = isFlipped
            };
        }
        
        /// <summary>
        /// 取消编辑
        /// </summary>
        public void Cancel()
        {
            // 销毁家具实例
            if (furnitureInstance != null)
            {
                Destroy(furnitureInstance);
            }
            
            OnCancel?.Invoke();
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 已放置家具数据
    /// </summary>
    [System.Serializable]
    public class PlacedFurnitureData
    {
        public string furnitureId;
        public Vector3 position;
        public float scale;
        public bool isFlipped;
    }
}
