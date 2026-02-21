using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 玩家武器系统 - 发射飞弹
    /// </summary>
    public class PlayerWeapon : MonoBehaviour
    {
        [Header("飞弹配置")]
        public GameObject projectilePrefab;
        public Transform firePoint;
        
        [Header("发射参数")]
        public float fireRate = 0.2f; // 每秒发射次数
        public float projectileSpeed = 15f;
        public float projectileDamage = 10f;
        
        [Header("当前属性")]
        public ElementType currentElement = ElementType.None;
        public int elementLevel = 1;
        
        private float fireCooldown = 0f;
        private Camera mainCamera;
        
        void Start()
        {
            mainCamera = Camera.main;
        }
        
        void Update()
        {
            fireCooldown -= Time.deltaTime;
            
            // 自动瞄准最近的敌人（简化版）
            // 或者：鼠标/触摸方向
            HandleFiring();
        }
        
        void HandleFiring()
        {
            if (fireCooldown > 0f) return;
            
            // 获取射击方向（朝向鼠标位置）
            Vector2 fireDirection = GetFireDirection();
            
            if (fireDirection != Vector2.zero)
            {
                Fire(fireDirection);
                fireCooldown = 1f / fireRate;
            }
        }
        
        Vector2 GetFireDirection()
        {
            // 方式1：鼠标位置
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                return (mousePos - transform.position).normalized;
            }
            #endif
            
            // 方式2：自动瞄准最近的敌人
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Enemy"));
            if (enemies.Length > 0)
            {
                Transform nearest = null;
                float nearestDist = float.MaxValue;
                
                foreach (var enemy in enemies)
                {
                    float dist = Vector2.Distance(transform.position, enemy.transform.position);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = enemy.transform;
                    }
                }
                
                if (nearest != null)
                {
                    return (nearest.position - transform.position).normalized;
                }
            }
            
            // 默认向右
            return Vector2.right;
        }
        
        void Fire(Vector2 direction)
        {
            if (projectilePrefab == null) return;
            
            // 创建飞弹
            GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projObj.GetComponent<Projectile>();
            
            if (projectile != null)
            {
                // 初始化飞弹
                projectile.InitializeFull(
                    direction,
                    currentElement,
                    elementLevel,
                    projectileDamage
                );
            }
            
            // 播放发射音效/特效
            PlayFireEffect();
        }
        
        void PlayFireEffect()
        {
            // TODO: 播放发射音效
            // TODO: 播放枪口闪光粒子
        }
        
        /// <summary>
        /// 切换属性（用于三选一后应用）
        /// </summary>
        public void SetElement(ElementType element, int level)
        {
            currentElement = element;
            elementLevel = level;
            
            Debug.Log($"[PlayerWeapon] 属性切换为: {element} Lv.{level}");
        }
        
        void OnDrawGizmos()
        {
            // 绘制射程范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 10f);
            
            // 绘制发射点
            if (firePoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(firePoint.position, 0.1f);
            }
        }
    }
}
