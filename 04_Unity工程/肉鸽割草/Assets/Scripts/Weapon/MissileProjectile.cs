using UnityEngine;
using System.Collections;

namespace GeometryWarrior
{
    /// <summary>
    /// MissileProjectile - Homing missile that tracks target and explodes
    /// </summary>
    public class MissileProjectile : MonoBehaviour
    {
        [Header("[Movement]")]
        [SerializeField] private float speed = 8f;
        [SerializeField] private float turnSpeed = 180f;
        
        [Header("[Explosion]")]
        [SerializeField] private float explosionRadius = 2f;
        [SerializeField] private int damage = 25;
        [SerializeField] private float lifetime = 5f;
        
        [Header("[Visuals]")]
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private TrailRenderer trailRenderer;
        
        private Transform target;
        private Rigidbody2D rb;
        private bool hasExploded;
        private float lifetimeTimer;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
            }
            
            trailRenderer = GetComponent<TrailRenderer>();
        }
        
        private void Update()
        {
            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
            {
                Explode();
            }
        }
        
        private void FixedUpdate()
        {
            if (hasExploded) return;
            
            // Home in on target
            if (target != null)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float currentAngle = transform.eulerAngles.z;
                
                float angleDiff = Mathf.DeltaAngle(currentAngle, targetAngle);
                float rotation = Mathf.Sign(angleDiff) * Mathf.Min(Mathf.Abs(angleDiff), turnSpeed * Time.fixedDeltaTime);
                
                transform.rotation = Quaternion.Euler(0, 0, currentAngle + rotation);
            }
            
            // Move forward
            rb.velocity = transform.right * speed;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasExploded) return;
            
            // Check if hit enemy or obstacle
            if (other.GetComponent<EnemyBase>() != null || other.CompareTag("Obstacle"))
            {
                Explode();
            }
        }
        
        private void Explode()
        {
            if (hasExploded) return;
            hasExploded = true;
            
            // Deal damage to enemies in radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D hit in hits)
            {
                EnemyBase enemy = hit.GetComponent<EnemyBase>();
                if (enemy != null && !enemy.IsDead)
                {
                    // Damage falls off with distance
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    float damageMultiplier = 1f - (distance / explosionRadius) * 0.5f;
                    enemy.TakeDamage(Mathf.RoundToInt(damage * damageMultiplier));
                }
            }
            
            // Spawn explosion effect
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            }
            
            // Disable trail and destroy
            if (trailRenderer != null)
            {
                trailRenderer.emitting = false;
            }
            
            Destroy(gameObject, 0.1f);
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            
            // Initial rotation towards target
            if (target != null)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        
        public void SetDamage(int newDamage)
        {
            damage = newDamage;
        }
        
        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
        }
        
        public void SetTurnSpeed(float newTurnSpeed)
        {
            turnSpeed = newTurnSpeed;
        }
        
        public void SetExplosionRadius(float newRadius)
        {
            explosionRadius = newRadius;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
