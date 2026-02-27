using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园装饰物 - 支持点击编辑模式
    /// </summary>
    public class HomeDecoration : MonoBehaviour
    {
        [Header("【装饰信息】")]
        [Tooltip("装饰物唯一ID，用于保存位置")]
        public string decorationId;
        
        [Tooltip("装饰物显示名称")]
        public string decorationName;
        
        [Header("【交互设置】")]
        [Tooltip("是否可以被点击选中")]
        public bool canBeSelected = true;
        
        [Tooltip("是否可以拖动（仅在编辑模式下）")]
        public bool canDrag = true;
        
        [Header("【选中效果】")]
        [Tooltip("选中时的描边颜色")]
        public Color selectedOutlineColor = new Color(1f, 0.8f, 0.2f, 0.8f);
        
        [Tooltip("选中时的描边宽度")]
        public float selectedOutlineWidth = 0.05f;
        
        // 状态
        private bool isSelected = false;
        private bool isDragging = false;
        private bool isInEditMode = false;
        
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected => isSelected;
        
        // 引用
        private Camera mainCamera;
        private SpriteRenderer spriteRenderer;
        private Material originalMaterial;
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthProperty = Shader.PropertyToID("_OutlineWidth");
        private static readonly int EnableOutlineProperty = Shader.PropertyToID("_EnableOutline");
        
        // 当前正在编辑的装饰物（静态，确保只有一个在编辑）
        public static HomeDecoration CurrentlyEditing { get; private set; }
        
        // 点击回调
        public System.Action<HomeDecoration> OnDecorationClicked;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalMaterial = spriteRenderer.material;
            }
            
            // 确保有碰撞体
            EnsureCollider();
        }
        
        /// <summary>
        /// 确保有碰撞体用于点击检测
        /// </summary>
        private void EnsureCollider()
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    boxCollider.size = sr.sprite.bounds.size;
                }
            }
        }
        
        private void OnMouseDown()
        {
            if (!canBeSelected) return;
            
            // 检查是否点击在UI上
            if (IsPointerOverUI()) return;
            
            if (isInEditMode)
            {
                // 编辑模式下开始拖拽
                if (canDrag)
                {
                    isDragging = true;
                }
            }
            else
            {
                // 非编辑模式下，点击选中
                Select();
            }
        }
        
        private void OnMouseDrag()
        {
            if (!isInEditMode || !isDragging || !canDrag) return;
            
            Vector3 mousePos = GetMouseWorldPosition();
            transform.position = mousePos;
        }
        
        private void OnMouseUp()
        {
            if (isDragging)
            {
                isDragging = false;
                SavePosition();
            }
        }
        
        /// <summary>
        /// 选中此装饰物
        /// </summary>
        public void Select()
        {
            // 如果已经有其他装饰物在编辑，先取消编辑
            if (CurrentlyEditing != null && CurrentlyEditing != this)
            {
                CurrentlyEditing.Deselect();
            }
            
            isSelected = true;
            CurrentlyEditing = this;
            
            // 显示描边效果
            ShowSelectionEffect();
            
            // 通知回调
            OnDecorationClicked?.Invoke(this);
            
            // 通知HomeManager进入编辑模式
            if (HomeManager.Instance != null)
            {
                HomeManager.Instance.EnterDecorationEditMode(this);
            }
        }
        
        /// <summary>
        /// 取消选中
        /// </summary>
        public void Deselect()
        {
            isSelected = false;
            isInEditMode = false;
            isDragging = false;
            
            if (CurrentlyEditing == this)
            {
                CurrentlyEditing = null;
            }
            
            // 隐藏描边效果
            HideSelectionEffect();
        }
        
        /// <summary>
        /// 进入编辑模式
        /// </summary>
        public void EnterEditMode()
        {
            isInEditMode = true;
        }
        
        /// <summary>
        /// 退出编辑模式
        /// </summary>
        public void ExitEditMode()
        {
            isInEditMode = false;
            isDragging = false;
            Deselect();
        }
        
        /// <summary>
        /// 显示选中效果（描边）
        /// </summary>
        private void ShowSelectionEffect()
        {
            if (spriteRenderer == null) return;
            
            // 使用Shader实现描边效果
            // 如果材质没有Outline属性，创建一个简单的描边材质
            if (originalMaterial != null && !originalMaterial.HasProperty(EnableOutlineProperty))
            {
                // 创建选中材质
                Material selectedMaterial = new Material(Shader.Find("Sprites/Default"));
                if (selectedMaterial != null)
                {
                    // 简单的描边：复制Sprite并放大作为描边
                    // 这里使用一个简单的材质替换
                    spriteRenderer.color = new Color(1f, 0.9f, 0.6f, 1f); // 高亮颜色
                }
            }
            else if (originalMaterial != null)
            {
                originalMaterial.SetFloat(EnableOutlineProperty, 1f);
                originalMaterial.SetColor(OutlineColorProperty, selectedOutlineColor);
                originalMaterial.SetFloat(OutlineWidthProperty, selectedOutlineWidth);
            }
        }
        
        /// <summary>
        /// 隐藏选中效果
        /// </summary>
        private void HideSelectionEffect()
        {
            if (spriteRenderer == null) return;
            
            spriteRenderer.color = Color.white; // 恢复默认颜色
            
            if (originalMaterial != null && originalMaterial.HasProperty(EnableOutlineProperty))
            {
                originalMaterial.SetFloat(EnableOutlineProperty, 0f);
            }
        }
        
        /// <summary>
        /// 翻转
        /// </summary>
        public void Flip()
        {
            Vector3 scale = transform.localScale;
            scale.x = -scale.x;
            transform.localScale = scale;
        }
        
        /// <summary>
        /// 设置缩放
        /// </summary>
        public void SetScale(float scale)
        {
            Vector3 currentScale = transform.localScale;
            // 保留翻转符号，直接设置新的绝对缩放值
            bool isFlipped = currentScale.x < 0;
            currentScale.x = scale * (isFlipped ? -1 : 1);
            currentScale.y = scale;
            transform.localScale = currentScale;
        }
        
        /// <summary>
        /// 获取当前缩放
        /// </summary>
        public float GetCurrentScale()
        {
            return Mathf.Abs(transform.localScale.y);
        }
        
        /// <summary>
        /// 检查是否已翻转
        /// </summary>
        public bool IsFlipped()
        {
            return transform.localScale.x < 0;
        }
        
        /// <summary>
        /// 获取鼠标世界位置
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
            return mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }
        
        /// <summary>
        /// 检查鼠标是否在UI上
        /// </summary>
        private bool IsPointerOverUI()
        {
            // 使用EventSystem检查是否点击在UI上
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            }
            
            return false;
        }
        
        /// <summary>
        /// 保存位置
        /// </summary>
        private void SavePosition()
        {
            if (HomeManager.Instance != null)
            {
                HomeManager.Instance.SaveDecorationPosition(decorationId, transform.position);
                // 同时保存所有家具数据（因为可能修改了缩放或位置）
                HomeManager.Instance.SaveAllFurnitureData();
            }
        }
        
        /// <summary>
        /// 确认编辑完成
        /// </summary>
        public void ConfirmEdit()
        {
            SavePosition();
            ExitEditMode();
        }
    }
}
