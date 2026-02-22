using UnityEngine;
using UnityEngine.EventSystems;

namespace GeometryWarrior
{
    /// <summary>
    /// 虚拟摇杆 - 支持触摸和鼠标输入
    /// </summary>
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("[摇杆组件]")]
        [SerializeField] private RectTransform background;      // 背景圆盘
        [SerializeField] private RectTransform handle;          // 摇杆把手
        
        [Header("[设置]")]
        [SerializeField] private float handleRange = 1f;        // 把手移动范围 (0-1)
        [SerializeField] private float deadZone = 0.1f;         // 死区，小于此值不响应
        [SerializeField] private bool fixedPosition = true;     // true=固定位置, false=跟随触摸
        
        // 输出属性
        public Vector2 Direction { get; private set; }          // 输出方向 (-1 到 1)
        public bool IsDragging { get; private set; }            // 是否正在拖拽
        
        private Vector2 origin;         // 原点位置
        private Vector2 input;          // 原始输入
        private float radius;           // 背景半径
        private Camera uiCamera;
        
        private void Start()
        {
            // 获取UI相机
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            }
            
            // 确保 handle 是 background 的子物体
            if (handle.parent != background)
            {
                handle.SetParent(background, false);
            }
            
            // 初始化位置
            origin = background.anchoredPosition;
            radius = background.sizeDelta.x * 0.5f * handleRange;
            
            // 初始把手位置归零
            handle.localPosition = Vector2.zero;
        }
        
        /// <summary>
        /// 获取标准化方向（考虑死区）
        /// </summary>
        public Vector2 GetDirection()
        {
            if (Direction.magnitude < deadZone)
                return Vector2.zero;
            return Direction;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            // 检查触摸点是否在背景圆盘内（只在固定位置模式下检查）
            if (fixedPosition)
            {
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    background,
                    eventData.position,
                    uiCamera,
                    out localPoint);
                
                // 如果触摸点在圆盘外，不响应
                if (localPoint.magnitude > radius)
                {
                    return;
                }
            }
            
            IsDragging = true;
            
            // 只有非固定位置模式下才移动摇杆到触摸位置
            if (!fixedPosition)
            {
                Vector2 touchPos = ScreenToLocal(eventData.position);
                background.anchoredPosition = touchPos;
            }
            
            OnDrag(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging) return;
            
            // 获取触摸点在背景圆盘内的本地坐标
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, 
                eventData.position, 
                uiCamera, 
                out localPoint);
            
            input = localPoint;
            
            // 计算方向和距离
            float distance = input.magnitude;
            Vector2 normalizedInput = input.normalized;
            
            // 限制把手在圆盘内
            if (distance > radius)
            {
                input = normalizedInput * radius;
                distance = radius;
            }
            
            // 更新把手位置（相对于背景）
            handle.localPosition = input;
            
            // 输出标准化方向 (-1 到 1)
            Direction = normalizedInput * (distance / radius);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            IsDragging = false;
            
            // 重置把手位置（相对于背景）
            handle.localPosition = Vector2.zero;
            Direction = Vector2.zero;
            input = Vector2.zero;
            
            // 只有非固定位置模式下才重置背景位置
            if (!fixedPosition)
            {
                background.anchoredPosition = origin;
            }
        }
        
        /// <summary>
        /// 将屏幕坐标转换为本地坐标
        /// </summary>
        private Vector2 ScreenToLocal(Vector2 screenPos)
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform as RectTransform, 
                screenPos, 
                uiCamera, 
                out localPos);
            return localPos;
        }
    }
}
