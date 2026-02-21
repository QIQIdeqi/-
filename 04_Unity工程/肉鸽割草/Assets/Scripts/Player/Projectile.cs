using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// Projectile - 玩家飞弹（支持属性系统）
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("基础属性")]
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private int damage = 20;
        
        [Header("属性系统")]
        public ElementType elementType = ElementType.None;
        [Range(1, 3)]
        public int elementLevel = 1;
        
        [Header("视觉组件")]
        public SpriteRenderer spriteRenderer;
        public TrailRenderer trailRenderer;
        public ParticleSystem hitEffectPrefab;
        
        private Transform target;
        private Rigidbody2D rb;
        private IProjectileEffect elementEffect;
        private bool isVisualApplied = false;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (trailRenderer == null)
                trailRenderer = GetComponent<TrailRenderer>();
            
            Destroy(gameObject, lifetime);
        }
        
        private void Start()
        {
            // 初始化属性效果
            InitializeElementEffect();
            // 应用视觉
            ApplyVisuals();
        }
        
        private void FixedUpdate()
        {
            if (target != null)
            {
                // Track target
                Vector2 direction = (target.position - transform.position).normalized;
                rb.velocity = direction * speed;
                
                // Rotate towards target
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                // Continue in current direction if target lost
                rb.velocity = transform.right * speed;
            }
        }
        
        #region 原有接口
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void SetDamage(int newDamage)
        {
            damage = newDamage;
        }
        
        #endregion
        
        #region 属性系统
        
        /// <summary>
        /// 设置完整属性（发射时调用）
        /// </summary>
        public void SetElement(ElementType type, int level)
        {
            elementType = type;
            elementLevel = Mathf.Clamp(level, 1, 3);
            InitializeElementEffect();
            ApplyVisuals();
        }
        
        /// <summary>
        /// 完整初始化（方向+属性+伤害）
        /// </summary>
        public void InitializeFull(Vector2 direction, ElementType element, int level, float dmg)
        {
            // 设置目标（自动瞄准）
            FindAndSetTarget(direction);
            
            // 设置属性
            elementType = element;
            elementLevel = Mathf.Clamp(level, 1, 3);
            damage = Mathf.RoundToInt(dmg);
            
            // 初始化
            InitializeElementEffect();
            ApplyVisuals();
        }
        
        void FindAndSetTarget(Vector2 direction)
        {
            // 射线检测寻找敌人（不限制Layer，检测所有碰撞体）
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 20f);
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    var enemy = hit.collider.GetComponent<EnemyBase>();
                    if (enemy != null && !enemy.IsDead)
                    {
                        SetTarget(hit.collider.transform);
                        return;
                    }
                }
            }
        }
        
        void InitializeElementEffect()
        {
            switch (elementType)
            {
                case ElementType.Fire:
                    elementEffect = new FireEffect(damage, elementLevel);
                    break;
                case ElementType.Ice:
                    elementEffect = new IceEffect(elementLevel);
                    break;
                case ElementType.Electric:
                    elementEffect = new ElectricEffect(elementLevel);
                    break;
                default:
                    elementEffect = null;
                    break;
            }
        }
        
        void ApplyVisuals()
        {
            if (isVisualApplied) return;
            
            if (ProjectileVisualManager.Instance != null)
            {
                ProjectileVisualManager.Instance.ApplyVisual(this, elementType, elementLevel);
                isVisualApplied = true;
            }
            else
            {
                ApplyDefaultVisual();
            }
        }
        
        void ApplyDefaultVisual()
        {
            if (spriteRenderer == null) return;
            
            switch (elementType)
            {
                case ElementType.Fire:
                    spriteRenderer.color = new Color(1f, 0.27f, 0.27f);
                    break;
                case ElementType.Ice:
                    spriteRenderer.color = new Color(0f, 0.53f, 1f);
                    break;
                case ElementType.Electric:
                    spriteRenderer.color = new Color(0.53f, 0.27f, 1f);
                    break;
            }
            
            isVisualApplied = true;
        }
        
        #endregion
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(damage);
                elementEffect?.Apply(other.gameObject);
                SpawnHitEffect();
                Destroy(gameObject);
                return;
            }
            
            // TODO: 添加障碍物检测（当有了墙/箱子后再启用）
            // if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            // {
            //     SpawnHitEffect();
            //     Destroy(gameObject);
            // }
        }
        
        void SpawnHitEffect()
        {
            if (hitEffectPrefab != null)
            {
                var effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                var main = effect.main;
                main.startColor = GetElementColor();
                effect.Play();
                Destroy(effect.gameObject, 1f);
            }
        }
        
        Color GetElementColor()
        {
            switch (elementType)
            {
                case ElementType.Fire: return new Color(1f, 0.27f, 0.27f);
                case ElementType.Ice: return new Color(0f, 0.53f, 1f);
                case ElementType.Electric: return new Color(0.53f, 0.27f, 1f);
                default: return Color.white;
            }
        }
    }
}
