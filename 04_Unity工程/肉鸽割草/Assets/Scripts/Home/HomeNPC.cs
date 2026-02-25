using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园NPC - 与玩家交互，打开装扮界面
    /// </summary>
    public class HomeNPC : MonoBehaviour
    {
        [Header("【交互设置】")]
        [Tooltip("玩家进入此范围后可以交互")]
        [SerializeField] private float interactionRadius = 1.5f;
        
        [Tooltip("PC端交互按键")]
        [SerializeField] private KeyCode interactionKey = KeyCode.E;
        
        [Header("【UI元素】")]
        [Tooltip("靠近时显示的提示（如'按E对话'）")]
        [SerializeField] private GameObject interactionHint;
        
        [Tooltip("点击NPC后打开的装扮界面面板")]
        [SerializeField] private OutfitPanel outfitPanel;
        
        [Header("【视觉效果】")]
        [Tooltip("NPC的精灵渲染器（用于高亮效果）")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Tooltip("正常状态颜色")]
        [SerializeField] private Color normalColor = Color.white;
        
        [Tooltip("玩家靠近时的高亮颜色")]
        [SerializeField] private Color highlightColor = new Color(1f, 0.9f, 0.7f);
        
        private Transform playerTransform;
        private bool isPlayerNearby = false;
        
        private void Start()
        {
            // 获取SpriteRenderer
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            
            // 初始隐藏交互提示
            if (interactionHint != null)
                interactionHint.SetActive(false);
        }
        
        private void Update()
        {
            // 查找玩家
            if (playerTransform == null)
            {
                var player = FindObjectOfType<PlayerController>();
                if (player != null)
                    playerTransform = player.transform;
                return;
            }
            
            // 检查玩家距离
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            bool wasNearby = isPlayerNearby;
            isPlayerNearby = distance <= interactionRadius;
            
            // 状态变化时更新UI
            if (isPlayerNearby != wasNearby)
            {
                OnProximityChanged(isPlayerNearby);
            }
            
            // 在附近时检测交互输入
            if (isPlayerNearby && !outfitPanel.IsVisible)
            {
                // PC端按键交互
                if (Input.GetKeyDown(interactionKey))
                {
                    OpenOutfitPanel();
                }
                
                // 移动端触摸交互（点击NPC）
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    float clickDistance = Vector2.Distance(mousePos, transform.position);
                    if (clickDistance <= 0.5f) // NPC本身的点击范围
                    {
                        OpenOutfitPanel();
                    }
                }
            }
        }
        
        /// <summary>
        /// 玩家进入/离开交互范围
        /// </summary>
        private void OnProximityChanged(bool isNearby)
        {
            // 显示/隐藏交互提示
            if (interactionHint != null)
            {
                interactionHint.SetActive(isNearby);
            }
            
            // 高亮效果
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isNearby ? highlightColor : normalColor;
            }
        }
        
        /// <summary>
        /// 打开装扮界面
        /// </summary>
        private void OpenOutfitPanel()
        {
            if (outfitPanel != null)
            {
                outfitPanel.Show();
                
                // 隐藏交互提示
                if (interactionHint != null)
                    interactionHint.SetActive(false);
            }
            else
            {
                Debug.LogError("[HomeNPC] outfitPanel is not assigned!");
            }
        }
        
        /// <summary>
        /// 关闭装扮界面的回调（供OutfitPanel调用）
        /// </summary>
        public void OnOutfitPanelClosed()
        {
            // 如果玩家还在附近，重新显示交互提示
            if (isPlayerNearby && interactionHint != null)
            {
                interactionHint.SetActive(true);
            }
        }
        
        // 在编辑器中显示交互范围
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
