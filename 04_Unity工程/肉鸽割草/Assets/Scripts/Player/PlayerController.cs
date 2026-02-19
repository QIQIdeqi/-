using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// PlayerController - Player movement, auto-attack, health, and level-up system
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("[Movement]")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("[Health]")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private float invincibleTime = 0.5f;
        
        [Header("[Attack]")]
        [SerializeField] private float attackRange = 8f;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int attackDamage = 20;
        
        [Header("[Level Up]")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentExp = 0;
        [SerializeField] private int expToNextLevel = 100;
        
        [Header("[Visuals]")]
        [SerializeField] private Color normalColor = Color.cyan;
        [SerializeField] private Color damageFlashColor = Color.white;
        
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private float attackTimer;
        private float invincibleTimer;
        private Vector2 moveInput;
        
        public bool IsDead { get; private set; }
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public int CurrentLevel => currentLevel;
        public int CurrentExp => currentExp;
        public int ExpToNextLevel => expToNextLevel;
        
        public System.Action OnDeath;
        public System.Action<int> OnHealthChanged;
        public System.Action<int, int> OnExpChanged;
        public System.Action<int> OnLevelUp;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
            }
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;
            }
        }
        
        private void Start()
        {
            currentHealth = maxHealth;
        }
        
        private void Update()
        {
            if (IsDead) return;
            
            // Handle input
            HandleInput();
            
            // Handle attack
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                AutoAttack();
                attackTimer = attackCooldown;
            }
            
            // Handle invincibility
            if (invincibleTimer > 0)
            {
                invincibleTimer -= Time.deltaTime;
            }
        }
        
        private void FixedUpdate()
        {
            if (IsDead) return;
            
            // Move player
            rb.velocity = moveInput * moveSpeed;
        }
        
        private void HandleInput()
        {
            // Keyboard input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            
            // Mouse click to move (for mobile/touch)
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                Vector2 direction = (mousePos - transform.position).normalized;
                
                if (Vector2.Distance(mousePos, transform.position) > 0.5f)
                {
                    moveInput = direction;
                }
                else
                {
                    moveInput = Vector2.zero;
                }
            }
            else
            {
                moveInput = new Vector2(horizontal, vertical).normalized;
            }
            
            // Flip sprite based on movement
            if (moveInput.x > 0)
                spriteRenderer.flipX = false;
            else if (moveInput.x < 0)
                spriteRenderer.flipX = true;
        }
        
        private void AutoAttack()
        {
            // Find nearest enemy
            EnemyBase nearestEnemy = FindNearestEnemy();
            
            if (nearestEnemy != null && !nearestEnemy.IsDead)
            {
                FireProjectile(nearestEnemy.transform);
            }
        }
        
        private EnemyBase FindNearestEnemy()
        {
            EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
            EnemyBase nearest = null;
            float nearestDistance = attackRange;
            
            foreach (EnemyBase enemy in enemies)
            {
                if (enemy.IsDead) continue;
                
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearest = enemy;
                    nearestDistance = distance;
                }
            }
            
            return nearest;
        }
        
        private void FireProjectile(Transform target)
        {
            if (projectilePrefab == null) return;
            
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Projectile proj = projectile.GetComponent<Projectile>();
            
            if (proj != null)
            {
                proj.SetTarget(target);
                proj.SetDamage(attackDamage);
            }
        }
        
        public void TakeDamage(int damage)
        {
            if (IsDead || invincibleTimer > 0) return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);
            
            OnHealthChanged?.Invoke(currentHealth);
            
            StartCoroutine(DamageFlash());
            
            invincibleTimer = invincibleTime;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private System.Collections.IEnumerator DamageFlash()
        {
            if (spriteRenderer == null) yield break;
            
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = normalColor;
        }
        
        private void Die()
        {
            IsDead = true;
            rb.velocity = Vector2.zero;
            OnDeath?.Invoke();
        }
        
        public void Revive()
        {
            IsDead = false;
            currentHealth = maxHealth;
            invincibleTimer = 1f;
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        public void AddExp(int amount)
        {
            if (IsDead) return;
            
            currentExp += amount;
            OnExpChanged?.Invoke(currentExp, expToNextLevel);
            
            if (currentExp >= expToNextLevel)
            {
                LevelUp();
            }
        }
        
        private void LevelUp()
        {
            currentLevel++;
            currentExp -= expToNextLevel;
            expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);
            
            // Increase stats
            maxHealth += 10;
            currentHealth = maxHealth;
            attackDamage += 5;
            
            Debug.Log($"PlayerController: Level Up to {currentLevel}, invoking OnLevelUp event. Subscribers: {OnLevelUp?.GetInvocationList().Length ?? 0}");
            
            OnLevelUp?.Invoke(currentLevel);
            OnHealthChanged?.Invoke(currentHealth);
            OnExpChanged?.Invoke(currentExp, expToNextLevel);
        }
        
        public void Heal(int amount)
        {
            if (IsDead) return;
            
            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }
        
        /// <summary>
        /// Increase max health and heal for the same amount
        /// </summary>
        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            currentHealth += amount; // Also heal the new amount
            OnHealthChanged?.Invoke(currentHealth);
            Debug.Log($"PlayerController: Max health increased by {amount}, now {maxHealth}/{currentHealth}");
        }
        
        /// <summary>
        /// Increase move speed
        /// </summary>
        public void IncreaseMoveSpeed(float amount)
        {
            moveSpeed += amount;
            Debug.Log($"PlayerController: Move speed increased by {amount}, now {moveSpeed}");
        }
        
        /// <summary>
        /// Increase attack speed (reduce cooldown)
        /// </summary>
        public void IncreaseAttackSpeed(float amount)
        {
            attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
            Debug.Log($"PlayerController: Attack cooldown reduced by {amount}, now {attackCooldown}");
        }
        
        /// <summary>
        /// Increase pickup range (for exp orbs)
        /// </summary>
        public void IncreasePickupRange(float amount)
        {
            // This would need to be implemented in the pickup logic
            // For now, just log it
            Debug.Log($"PlayerController: Pickup range increased by {amount}");
        }
    }
}
