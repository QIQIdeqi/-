using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园装饰物 - 可在家园中放置和配置的装饰物品
    /// </summary>
    public class HomeDecoration : MonoBehaviour
    {
        [Header("【装饰信息】")]
        [Tooltip("装饰物唯一ID，用于保存位置")]
        public string decorationId;
        
        [Tooltip("装饰物显示名称")]
        public string decorationName;
        
        [Tooltip("装饰物类别（家具/植物/灯具等）")]
        [SerializeField] private DecorationCategory category;
        
        [Header("【交互设置】")]
        [Tooltip("是否可以用鼠标拖动")]
        [SerializeField] private bool isDraggable = true;
        
        [Tooltip("是否可以旋转")]
        [SerializeField] private bool canRotate = true;
        
        [Tooltip("是否可以缩放")]
        [SerializeField] private bool canScale = false;
        
        [Header("【边界限制】")]
        [Tooltip("是否限制在家园边界内")]
        [SerializeField] private bool useBounds = true;
        
        private bool isDragging = false;
        private Vector3 dragOffset;
        private Camera mainCamera;
        
        // 家园边界（由HomeManager设置）
        public Bounds HomeBounds { get; set; }
        
        private void Start()
        {
            mainCamera = Camera.main;
            
            // 确保有碰撞体（用于接收鼠标事件）
            EnsureCollider();
        }
        
        /// <summary>
        /// 确保装饰物有碰撞体（用于拖拽）
        /// </summary>
        private void EnsureCollider()
        {
            // 检查是否已有 2D 碰撞体
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                // 自动添加 BoxCollider2D
                BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                
                // 如果有 SpriteRenderer，根据 Sprite 大小调整碰撞体
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    boxCollider.size = sr.sprite.bounds.size;
                }
                
                Debug.Log($"[HomeDecoration] 自动添加 BoxCollider2D: {decorationName}");
            }
        }
        
        private void OnMouseDown()
        {
            if (!isDraggable) return;
            
            isDragging = true;
            Vector3 mousePos = GetMouseWorldPosition();
            dragOffset = transform.position - mousePos;
        }
        
        private void OnMouseDrag()
        {
            if (!isDragging) return;
            
            Vector3 mousePos = GetMouseWorldPosition();
            Vector3 newPosition = mousePos + dragOffset;
            
            // 限制在边界内
            if (useBounds && HomeBounds.size != Vector3.zero)
            {
                newPosition = ClampPositionToBounds(newPosition);
            }
            
            transform.position = newPosition;
        }
        
        private void OnMouseUp()
        {
            isDragging = false;
            
            // 保存位置到HomeManager
            SavePosition();
        }
        
        /// <summary>
        /// 获取鼠标在世界空间的位置
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
            return mainCamera.ScreenToWorldPoint(mouseScreenPos);
        }
        
        /// <summary>
        /// 将位置限制在边界内
        /// </summary>
        private Vector3 ClampPositionToBounds(Vector3 position)
        {
            // 获取物体的半尺寸
            Renderer rend = GetComponent<Renderer>();
            float halfWidth = 0.5f;
            float halfHeight = 0.5f;
            
            if (rend != null)
            {
                halfWidth = rend.bounds.extents.x;
                halfHeight = rend.bounds.extents.y;
            }
            
            position.x = Mathf.Clamp(position.x, HomeBounds.min.x + halfWidth, HomeBounds.max.x - halfWidth);
            position.y = Mathf.Clamp(position.y, HomeBounds.min.y + halfHeight, HomeBounds.max.y - halfHeight);
            
            return position;
        }
        
        /// <summary>
        /// 保存位置
        /// </summary>
        private void SavePosition()
        {
            if (HomeManager.Instance != null)
            {
                HomeManager.Instance.SaveDecorationPosition(decorationId, transform.position);
            }
        }
        
        /// <summary>
        /// 加载位置
        /// </summary>
        public void LoadPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        /// <summary>
        /// 旋转装饰物
        /// </summary>
        public void Rotate(float angle)
        {
            if (canRotate)
            {
                transform.Rotate(0, 0, angle);
            }
        }
        
        /// <summary>
        /// 缩放装饰物
        /// </summary>
        public void SetScale(float scale)
        {
            if (canScale)
            {
                transform.localScale = Vector3.one * scale;
            }
        }
    }
    
    /// <summary>
    /// 装饰物类别
    /// </summary>
    public enum DecorationCategory
    {
        Furniture,  // 家具
        Plant,      // 植物
        WallDecor,  // 墙面装饰
        FloorDecor, // 地面装饰
        Lighting,   // 灯具
        Special     // 特殊装饰
    }
}
