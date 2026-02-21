using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// EnemyBase - Base class for all enemies with scaling stats and health bar
    /// </summary>
    public class EnemyBase : MonoBehaviour
    {
        [Header("[Base Stats]")]
        [SerializeField] protected int baseMaxHealth = 30;
        [SerializeField] protected int baseAttackDamage = 10;
        [SerializeField] protected float baseMoveSpeed = 2f;
        [SerializeField] protected int baseExpValue = 10;
        
        // 当前实际值（考虑难度加成）
        protected int maxHealth;
        protected int currentHealth;
        protected int attackDamage;
        protected float moveSpeed;
        protected int expValue;
        
        [Header("[Drop Settings]")]
        [SerializeField] protected GameObject expOrbPrefab;
        
        [Header("[Visuals]")]
        [SerializeField] protected bool useCustomColor = false;
        [SerializeField] protected Color normalColor = Color.white;
        [SerializeField] protected Color damageFlashColor = Color.white;
        
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Transform player;
        protected EnemyHealthBar healthBar;
        
        public bool IsDead { get; protected set; }
        public string EnemyTypeName { get; set; }
        public int AttackDamage => attackDamage;
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        
        public System.Action<EnemyBase> OnDeathEvent;
        public System.Action<float> OnHealthChanged; // 血量百分比变化事件
        
        // 难度等级（由EnemySpawner设置）
        protected int difficultyLevel = 0;
        protected float difficultyMultiplier = 1f;
        
        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            healthBar = GetComponentInChildren<EnemyHealthBar>();
            
            if (spriteRenderer != null && useCustomColor)
            {
                spriteRenderer.color = normalColor;
            }
            
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
            }
        }
        
        protected virtual void OnEnable()
        {
            IsDead = false;
            
            // 应用难度加成后的属性
            ApplyDifficultyStats();
            
            currentHealth = maxHealth;
            
            if (spriteRenderer != null && useCustomColor)
            {
                spriteRenderer.color = normalColor;
            }
            
            if (player == null)
            {
                PlayerController foundPlayer = FindObjectOfType<PlayerController>();
                if (foundPlayer != null)
                    player = foundPlayer.transform;
            }
            
            // 通知血条更新
            NotifyHealthChanged();
        }
        
        /// <summary>
        /// 设置难度等级（由EnemySpawner在游戏开始时调用）
        /// </summary>
        public virtual void SetDifficultyLevel(int level, float multiplier)
        {
            difficultyLevel = level;
            difficultyMultiplier = multiplier;
            ApplyDifficultyStats();
        }
        
        /// <summary>
        /// 应用难度加成到属性
        /// </summary>
        protected virtual void ApplyDifficultyStats()
        {
            // 血量 = 基础 × 难度倍率
            maxHealth = Mathf.RoundToInt(baseMaxHealth * difficultyMultiplier);
            // 攻击 = 基础 × 难度倍率
            attackDamage = Mathf.RoundToInt(baseAttackDamage * difficultyMultiplier);
            // 移速轻微增加
            moveSpeed = baseMoveSpeed * (1f + (difficultyMultiplier - 1f) * 0.3f);
            // 经验值也增加
            expValue = Mathf.RoundToInt(baseExpValue * (1f + (difficultyMultiplier - 1f) * 0.5f));
            
            // 同步当前血量
            if (currentHealth > maxHealth || currentHealth == 0)
            {
                currentHealth = maxHealth;
            }
        }
        
        protected virtual void Update()
        {
            if (IsDead) return;
            
            MoveToPlayer();
        }
        
        protected virtual void MoveToPlayer()
        {
            if (player == null) return;
            
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            
            if (spriteRenderer != null)
            {
                if (direction.x > 0)
                    spriteRenderer.flipX = false;
                else if (direction.x < 0)
                    spriteRenderer.flipX = true;
            }
        }
        
        public virtual void TakeDamage(int damage)
        {
            if (IsDead) return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            // 通知血条更新
            NotifyHealthChanged();
            
            StartCoroutine(DamageFlash());
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// 通知血量变化事件
        /// </summary>
        protected void NotifyHealthChanged()
        {
            float healthPercent = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
            OnHealthChanged?.Invoke(healthPercent);
        }
        
        protected System.Collections.IEnumerator DamageFlash()
        {
            if (spriteRenderer == null) yield break;
            
            Color originalColor = useCustomColor ? normalColor : spriteRenderer.color;
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = originalColor;
        }
        
        protected virtual void Die()
        {
            IsDead = true;
            if (rb != null)
                rb.velocity = Vector2.zero;
            
            // Drop exp orb
            if (expOrbPrefab != null)
            {
                Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            }
            
            OnDeathEvent?.Invoke(this);
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null && !player.IsDead)
            {
                player.TakeDamage(attackDamage);
            }
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.IsDead)
            {
                player.TakeDamage(attackDamage);
            }
        }
        
        public void OnOutOfBounds()
        {
            if (!IsDead)
            {
                Die();
            }
        }
    }
}
