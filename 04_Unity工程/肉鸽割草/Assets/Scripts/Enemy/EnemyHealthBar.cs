using UnityEngine;
using UnityEngine.UI;

namespace GeometryWarrior
{
    /// <summary>
    /// EnemyHealthBar - 敌人血条显示
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [Header("[UI References]")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Image fillImage;
        [SerializeField] private Canvas canvas;
        
        [Header("[Visual Settings]")]
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private float lowHealthThreshold = 0.3f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.6f, 0);
        [SerializeField] private float hideDelay = 2f; // 受伤后多久隐藏血条
        
        private EnemyBase enemy;
        private Camera mainCamera;
        private float hideTimer;
        private bool isVisible = false;
        
        private void Awake()
        {
            enemy = GetComponentInParent<EnemyBase>();
            if (enemy == null)
            {
                Debug.LogWarning("EnemyHealthBar: 未找到父物体的EnemyBase组件");
                enabled = false;
                return;
            }
            
            // 如果没有设置引用，尝试查找
            if (canvas == null)
                canvas = GetComponent<Canvas>();
            if (healthSlider == null)
                healthSlider = GetComponentInChildren<Slider>();
            if (fillImage == null && healthSlider != null)
                fillImage = healthSlider.fillRect.GetComponent<Image>();
            
            // 自动配置Canvas
            SetupCanvas();
            
            mainCamera = Camera.main;
            
            // 初始隐藏
            SetVisible(false);
        }
        
        /// <summary>
        /// 配置Canvas为World Space模式
        /// </summary>
        private void SetupCanvas()
        {
            if (canvas == null)
            {
                Debug.LogError("EnemyHealthBar: 未找到Canvas组件！");
                return;
            }
            
            // 设置为World Space模式
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                canvas.renderMode = RenderMode.WorldSpace;
            }
            
            // 设置参考相机
            if (canvas.worldCamera == null)
            {
                canvas.worldCamera = Camera.main;
            }
            
            // 设置Canvas大小
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 0.2f);
            
            // 设置缩放，使UI大小合适
            transform.localScale = Vector3.one * 0.01f;
        }
        
        private void Start()
        {
            // 订阅受伤事件
            if (enemy != null)
            {
                enemy.OnHealthChanged += OnEnemyHealthChanged;
            }
        }
        
        private void OnDestroy()
        {
            if (enemy != null)
            {
                enemy.OnHealthChanged -= OnEnemyHealthChanged;
            }
        }
        
        private void LateUpdate()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null) return;
            }
            
            // 跟随敌人位置
            if (enemy != null)
            {
                transform.position = enemy.transform.position + offset;
            }
            
            // 面向相机
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
            
            // 隐藏计时
            if (isVisible && hideTimer > 0)
            {
                hideTimer -= Time.deltaTime;
                if (hideTimer <= 0)
                {
                    SetVisible(false);
                }
            }
        }
        
        private void OnEnemyHealthChanged(float healthPercent)
        {
            if (healthSlider != null)
            {
                healthSlider.value = healthPercent;
            }
            
            // 更新颜色（使用 lowHealthThreshold 作为阈值）
            if (fillImage != null)
            {
                // 当血量低于阈值时显示红色，高于阈值时从红色渐变到绿色
                float colorT = Mathf.InverseLerp(0f, lowHealthThreshold, healthPercent);
                fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, colorT);
            }
            
            // 显示血条并开始隐藏计时
            SetVisible(true);
            hideTimer = hideDelay;
        }
        
        private void SetVisible(bool visible)
        {
            isVisible = visible;
            if (canvas != null)
            {
                canvas.enabled = visible;
            }
        }
        
        /// <summary>
        /// 强制显示血条（不自动隐藏）
        /// </summary>
        public void ShowPermanent()
        {
            SetVisible(true);
            hideTimer = -1f; // 不自动隐藏
        }
        
        /// <summary>
        /// 隐藏血条
        /// </summary>
        public void Hide()
        {
            SetVisible(false);
        }
    }
}
