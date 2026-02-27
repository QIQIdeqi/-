using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// Projectile - 玩家飞弹（简化版，移除元素系统）
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("基础属性")]
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private int damage = 20;
        
        [Header("视觉组件")]
        public SpriteRenderer spriteRenderer;
        public TrailRenderer trailRenderer;
        public ParticleSystem hitEffectPrefab;
        
        private Transform target;
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (trailRenderer == null)
                trailRenderer = GetComponent<TrailRenderer>();
            
            Destroy(gameObject, lifetime);
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
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void SetDamage(int newDamage)
        {
            damage = newDamage;
        }
        
        /// <summary>
        /// 完整初始化（简化版）
        /// </summary>
        public void InitializeFull(Vector2 direction, float dmg)
        {
            // 设置目标（自动瞄准）
            FindAndSetTarget(direction);
            
            // 设置伤害
            damage = Mathf.RoundToInt(dmg);
        }
        
        void FindAndSetTarget(Vector2 direction)
        {
            // 射线检测寻找敌人
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(damage);
                SpawnHitEffect();
                Destroy(gameObject);
                return;
            }
        }
        
        void SpawnHitEffect()
        {
            if (hitEffectPrefab != null)
            {
                var effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, 1f);
            }
        }
    }
}
