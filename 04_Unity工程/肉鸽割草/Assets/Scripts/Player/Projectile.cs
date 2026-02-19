using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// Projectile - Auto-tracking projectile fired by player
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 15f;
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private int damage = 20;
        
        private Transform target;
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null && !enemy.IsDead)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
