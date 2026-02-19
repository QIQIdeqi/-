using UnityEngine;
using System.Collections;

namespace GeometryWarrior
{
    /// <summary>
    /// WeaponBase - Abstract base class for all weapons
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("[Weapon Info]")]
        [SerializeField] protected string weaponName = "Weapon";
        [SerializeField] protected string description = "Weapon description";
        
        [Header("[Base Stats]")]
        [SerializeField] protected float attackInterval = 1f;
        [SerializeField] protected int baseDamage = 10;
        [SerializeField] protected int maxLevel = 5;
        
        [Header("[Visuals]")]
        [SerializeField] protected Sprite weaponIcon;
        
        protected int currentLevel = 1;
        protected float attackTimer;
        protected Transform playerTransform;
        protected WeaponManager weaponManager;
        
        public string WeaponName => weaponName;
        public string Description => description;
        public int Level => currentLevel;
        public int MaxLevel => maxLevel;
        public Sprite Icon => weaponIcon;
        public int Damage => baseDamage + (currentLevel - 1) * 5;
        public float AttackInterval => attackInterval * Mathf.Pow(0.95f, currentLevel - 1);
        
        protected virtual void Start()
        {
            // Find player transform
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
                transform.SetParent(playerTransform, false);
            }
            
            weaponManager = WeaponManager.Instance;
        }
        
        protected virtual void Update()
        {
            if (playerTransform == null) return;
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;
            
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                PerformAttack();
                attackTimer = AttackInterval;
            }
        }
        
        protected abstract void PerformAttack();
        
        public virtual void Upgrade()
        {
            if (currentLevel < maxLevel)
            {
                currentLevel++;
                OnUpgrade();
            }
        }
        
        protected virtual void OnUpgrade()
        {
            // Override in derived classes for specific upgrade effects
        }
        
        /// <summary>
        /// Find nearest enemy within range
        /// </summary>
        protected EnemyBase FindNearestEnemy(float range)
        {
            EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
            EnemyBase nearest = null;
            float nearestDistance = range;
            
            foreach (EnemyBase enemy in enemies)
            {
                if (enemy.IsDead) continue;
                
                float distance = Vector2.Distance(playerTransform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearest = enemy;
                    nearestDistance = distance;
                }
            }
            
            return nearest;
        }
        
        /// <summary>
        /// Find all enemies within range
        /// </summary>
        protected EnemyBase[] FindEnemiesInRange(float range)
        {
            EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
            System.Collections.Generic.List<EnemyBase> inRange = new System.Collections.Generic.List<EnemyBase>();
            
            foreach (EnemyBase enemy in enemies)
            {
                if (enemy.IsDead) continue;
                
                float distance = Vector2.Distance(playerTransform.position, enemy.transform.position);
                if (distance < range)
                {
                    inRange.Add(enemy);
                }
            }
            
            return inRange.ToArray();
        }
    }
}
