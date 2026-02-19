using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// EnemyBase - Base class for all enemies
    /// </summary>
    public class EnemyBase : MonoBehaviour
    {
        [Header("[Base Stats]")]
        [SerializeField] protected int maxHealth = 30;
        [SerializeField] protected int currentHealth;
        [SerializeField] protected int attackDamage = 10;
        [SerializeField] protected float moveSpeed = 2f;
        [SerializeField] protected int expValue = 10;
        
        [Header("[Drop Settings]")]
        [SerializeField] protected GameObject expOrbPrefab;
        
        [Header("[Visuals]")]
        [SerializeField] protected Color normalColor = Color.red;
        [SerializeField] protected Color damageFlashColor = new Color(1f, 1f, 1f, 0.5f);
        
        protected Rigidbody2D rb;
        protected SpriteRenderer spriteRenderer;
        protected Transform player;
        
        public bool IsDead { get; protected set; }
        public string EnemyTypeName { get; set; }
        
        public System.Action<EnemyBase> OnDeathEvent;
        
        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer != null)
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
            currentHealth = maxHealth;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;
            }
            
            if (player == null)
            {
                PlayerController foundPlayer = FindObjectOfType<PlayerController>();
                if (foundPlayer != null)
                    player = foundPlayer.transform;
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
            
            StartCoroutine(DamageFlash());
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        protected System.Collections.IEnumerator DamageFlash()
        {
            if (spriteRenderer == null) yield break;
            
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = normalColor;
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
