using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园之门 - 玩家接触后显示离开提示，点击返回主界面
    /// </summary>
    public class HomeDoor : MonoBehaviour
    {
        [Header("【交互设置】")]
        [Tooltip("玩家进入此范围触发交互")]
        [SerializeField] private float triggerRadius = 1f;
        
        [Header("【UI元素】")]
        [Tooltip("玩家靠近时门上显示的箭头")]
        [SerializeField] private GameObject arrowIndicator;
        
        [Tooltip("离开提示文字（如'点击离开'）")]
        [SerializeField] private GameObject leaveHint;
        
        [Tooltip("点击后返回主界面的按钮")]
        [SerializeField] private Button leaveButton;
        
        [Header("【动画效果】")]
        [Tooltip("箭头弹跳动画速度")]
        [SerializeField] private float bounceSpeed = 2f;
        
        [Tooltip("箭头弹跳高度")]
        [SerializeField] private float bounceHeight = 0.2f;
        
        private Transform playerTransform;
        private Vector3 arrowOriginalPos;
        private bool isPlayerNearby = false;
        
        private void Start()
        {
            // 初始隐藏UI
            if (arrowIndicator != null)
            {
                arrowIndicator.SetActive(false);
                arrowOriginalPos = arrowIndicator.transform.position;
            }
            
            if (leaveHint != null)
                leaveHint.SetActive(false);
            
            // 绑定按钮事件
            if (leaveButton != null)
            {
                leaveButton.onClick.AddListener(OnLeaveClicked);
                leaveButton.gameObject.SetActive(false);
            }
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
            isPlayerNearby = distance <= triggerRadius;
            
            // 状态变化时更新UI
            if (isPlayerNearby != wasNearby)
            {
                OnProximityChanged(isPlayerNearby);
            }
            
            // 玩家在近处时，箭头动画
            if (isPlayerNearby && arrowIndicator != null && arrowIndicator.activeSelf)
            {
                AnimateArrow();
            }
        }
        
        /// <summary>
        /// 玩家进入/离开触发范围
        /// </summary>
        private void OnProximityChanged(bool isNearby)
        {
            // 显示/隐藏箭头
            if (arrowIndicator != null)
            {
                arrowIndicator.SetActive(isNearby);
                if (isNearby)
                {
                    arrowOriginalPos = arrowIndicator.transform.position;
                }
            }
            
            // 显示/隐藏离开提示
            if (leaveHint != null)
                leaveHint.SetActive(isNearby);
            
            // 显示/隐藏离开按钮
            if (leaveButton != null)
                leaveButton.gameObject.SetActive(isNearby);
        }
        
        /// <summary>
        /// 箭头弹跳动画
        /// </summary>
        private void AnimateArrow()
        {
            float yOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            arrowIndicator.transform.position = arrowOriginalPos + Vector3.up * yOffset;
        }
        
        /// <summary>
        /// 点击离开按钮
        /// </summary>
        private void OnLeaveClicked()
        {
            Debug.Log("[HomeDoor] 玩家点击离开，返回主界面");
            
            // 返回到主界面（战斗场景）
            if (HomeManager.Instance != null)
            {
                HomeManager.Instance.ReturnToMainMenu();
            }
            else
            {
                Debug.LogError("[HomeDoor] HomeManager.Instance is null!");
                // 备用方案：直接加载主场景
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
            }
        }
        
        // 在编辑器中显示触发范围
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }
}
