using UnityEngine;
using System.Collections;

namespace GeometryWarrior
{
    /// <summary>
    /// EnemyBase - 敌人基类（支持属性Debuff系统）
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
        protected float baseMoveSpeedValue; // 存储基础移速用于Debuff计算
        protected int expValue;
        
        [Header("[Drop Settings]")]
        [SerializeField] protected GameObject expOrbPrefab;
        
        [Header("[Visuals]")]
        [SerializeField] protected bool useCustomColor = false;
        [SerializeField] protected Color normalColor = Color.white;
        [SerializeField] protected Color damageFlashColor = Color.white;
        
        // Debuff粒子效果
        [Header("[Debuff Effects]")]
        [SerializeField] protected ParticleSystem burningEffect;
        [SerializeField] protected ParticleSystem frozenEffect;
        [SerializeField] protected ParticleSystem slowEffect;
        
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
        public System.Action<float> OnHealthChanged;
        
        // 难度等级
        protected int difficultyLevel = 0;
        protected float difficultyMultiplier = 1f;
        
        #region Debuff状态
        
        private bool isFrozen = false;
        private bool isSlowed = false;
        private float damageMultiplier = 1f;
        
        private Coroutine burningCoroutine;
        private Coroutine freezeCoroutine;
        private Coroutine slowCoroutine;
        
        #endregion
        
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
            isFrozen = false;
            isSlowed = false;
            damageMultiplier = 1f;
            
            ApplyDifficultyStats();
            
            currentHealth = maxHealth;
            baseMoveSpeedValue = moveSpeed;
            
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
            
            NotifyHealthChanged();
        }
        
        protected virtual void OnDisable()
        {
            // 清理所有Debuff协程
            RemoveAllDebuffs();
        }
        
        public virtual void SetDifficultyLevel(int level, float multiplier)
        {
            difficultyLevel = level;
            difficultyMultiplier = multiplier;
            ApplyDifficultyStats();
        }
        
        protected virtual void ApplyDifficultyStats()
        {
            maxHealth = Mathf.RoundToInt(baseMaxHealth * difficultyMultiplier);
            attackDamage = Mathf.RoundToInt(baseAttackDamage * difficultyMultiplier);
            moveSpeed = baseMoveSpeed * (1f + (difficultyMultiplier - 1f) * 0.3f);
            expValue = Mathf.RoundToInt(baseExpValue * (1f + (difficultyMultiplier - 1f) * 0.5f));
            
            if (currentHealth > maxHealth || currentHealth == 0)
            {
                currentHealth = maxHealth;
            }
        }
        
        protected virtual void Update()
        {
            if (IsDead) return;
            
            if (!isFrozen)
            {
                MoveToPlayer();
            }
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
        
        #region 伤害处理
        
        public virtual void TakeDamage(int damage)
        {
            if (IsDead) return;
            
            float finalDamage = damage * damageMultiplier;
            currentHealth -= Mathf.RoundToInt(finalDamage);
            currentHealth = Mathf.Max(0, currentHealth);
            
            NotifyHealthChanged();
            StartCoroutine(DamageFlash());
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        protected void NotifyHealthChanged()
        {
            float healthPercent = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
            OnHealthChanged?.Invoke(healthPercent);
        }
        
        protected IEnumerator DamageFlash()
        {
            if (spriteRenderer == null) yield break;
            
            Color originalColor = useCustomColor ? normalColor : spriteRenderer.color;
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(0.05f);
            
            // 恢复时考虑Debuff颜色
            if (isFrozen)
                spriteRenderer.color = new Color(0.5f, 0.8f, 1f);
            else if (isSlowed)
                spriteRenderer.color = new Color(0.8f, 0.5f, 1f);
            else
                spriteRenderer.color = originalColor;
        }
        
        #endregion
        
        #region Debuff系统
        
        // ===== 燃烧 =====
        public void ApplyBurning(float damagePerSecond, float duration, float tickInterval)
        {
            if (burningCoroutine != null)
                StopCoroutine(burningCoroutine);
            
            burningCoroutine = StartCoroutine(BurningCoroutine(damagePerSecond, duration, tickInterval));
        }
        
        public void RemoveBurning()
        {
            if (burningCoroutine != null)
            {
                StopCoroutine(burningCoroutine);
                burningCoroutine = null;
            }
            
            if (burningEffect != null)
                burningEffect.Stop();
        }
        
        IEnumerator BurningCoroutine(float dps, float duration, float interval)
        {
            float timer = 0f;
            float tickTimer = 0f;
            
            if (burningEffect != null)
                burningEffect.Play();
            
            while (timer < duration && !IsDead && currentHealth > 0)
            {
                tickTimer += Time.deltaTime;
                timer += Time.deltaTime;
                
                if (tickTimer >= interval)
                {
                    TakeDamage(Mathf.RoundToInt(dps * interval));
                    tickTimer = 0f;
                }
                
                yield return null;
            }
            
            RemoveBurning();
        }
        
        // ===== 冰冻 =====
        public void ApplyFreeze(float duration, float damageBoost)
        {
            if (freezeCoroutine != null)
                StopCoroutine(freezeCoroutine);
            
            freezeCoroutine = StartCoroutine(FreezeCoroutine(duration, damageBoost));
        }
        
        public void RemoveFreeze()
        {
            if (freezeCoroutine != null)
            {
                StopCoroutine(freezeCoroutine);
                freezeCoroutine = null;
            }
            
            isFrozen = false;
            damageMultiplier = 1f;
            
            if (rb != null)
                rb.velocity = Vector2.zero;
            
            if (frozenEffect != null)
                frozenEffect.Stop();
            
            if (spriteRenderer != null)
                spriteRenderer.color = useCustomColor ? normalColor : Color.white;
        }
        
        IEnumerator FreezeCoroutine(float duration, float damageBoost)
        {
            isFrozen = true;
            damageMultiplier = damageBoost;
            
            if (rb != null)
                rb.velocity = Vector2.zero;
            
            if (frozenEffect != null)
                frozenEffect.Play();
            
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(0.5f, 0.8f, 1f);
            
            yield return new WaitForSeconds(duration);
            
            RemoveFreeze();
        }
        
        // ===== 减速 =====
        public void ApplySlow(float slowPercent, float duration)
        {
            if (slowCoroutine != null)
                StopCoroutine(slowCoroutine);
            
            slowCoroutine = StartCoroutine(SlowCoroutine(slowPercent, duration));
        }
        
        public void RemoveSlow()
        {
            if (slowCoroutine != null)
            {
                StopCoroutine(slowCoroutine);
                slowCoroutine = null;
            }
            
            isSlowed = false;
            moveSpeed = baseMoveSpeedValue;
            
            if (slowEffect != null)
                slowEffect.Stop();
            
            if (spriteRenderer != null && !isFrozen)
                spriteRenderer.color = useCustomColor ? normalColor : Color.white;
        }
        
        IEnumerator SlowCoroutine(float slowPercent, float duration)
        {
            isSlowed = true;
            moveSpeed = baseMoveSpeedValue * (1f - slowPercent);
            
            if (slowEffect != null)
                slowEffect.Play();
            
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(0.8f, 0.5f, 1f);
            
            yield return new WaitForSeconds(duration);
            
            RemoveSlow();
        }
        
        void RemoveAllDebuffs()
        {
            RemoveBurning();
            RemoveFreeze();
            RemoveSlow();
        }
        
        #endregion
        
        protected virtual void Die()
        {
            IsDead = true;
            if (rb != null)
                rb.velocity = Vector2.zero;
            
            RemoveAllDebuffs();
            
            if (expOrbPrefab != null)
            {
                Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            }
            
            OnDeathEvent?.Invoke(this);
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            PlayerController playerCtrl = collision.collider.GetComponent<PlayerController>();
            if (playerCtrl != null && !playerCtrl.IsDead)
            {
                playerCtrl.TakeDamage(attackDamage);
            }
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController playerCtrl = other.GetComponent<PlayerController>();
            if (playerCtrl != null && !playerCtrl.IsDead)
            {
                playerCtrl.TakeDamage(attackDamage);
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
