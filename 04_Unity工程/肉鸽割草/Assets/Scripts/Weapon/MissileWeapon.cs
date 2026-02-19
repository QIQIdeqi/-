using UnityEngine;
using System.Collections;

namespace GeometryWarrior
{
    /// <summary>
    /// MissileWeapon - Fires homing missiles that track enemies
    /// </summary>
    public class MissileWeapon : WeaponBase
    {
        [Header("[Missile Settings]")]
        [SerializeField] private float missileSpeed = 8f;
        [SerializeField] private float turnSpeed = 180f;
        [SerializeField] private float explosionRadius = 2f;
        [SerializeField] private int missileCount = 1;
        [SerializeField] private float missileDelay = 0.1f;
        
        [Header("[Prefabs]")]
        [SerializeField] private GameObject missilePrefab;
        
        protected override void Start()
        {
            base.Start();
            weaponName = "Homing Missile";
            description = "Fires homing missiles that track and explode on enemies";
            attackInterval = 1.2f;
            baseDamage = 25;
        }
        
        protected override void PerformAttack()
        {
            StartCoroutine(FireMissiles());
        }
        
        private IEnumerator FireMissiles()
        {
            for (int i = 0; i < missileCount; i++)
            {
                EnemyBase target = FindNearestEnemy(15f);
                if (target == null) yield break;
                
                FireSingleMissile(target);
                
                if (i < missileCount - 1)
                {
                    yield return new WaitForSeconds(missileDelay);
                }
            }
        }
        
        private void FireSingleMissile(EnemyBase target)
        {
            if (missilePrefab == null) return;
            
            Vector3 spawnPos = playerTransform.position + Random.insideUnitSphere * 0.5f;
            spawnPos.z = 0;
            
            GameObject missileObj = Instantiate(missilePrefab, spawnPos, Quaternion.identity);
            MissileProjectile missile = missileObj.GetComponent<MissileProjectile>();
            
            if (missile != null)
            {
                missile.SetTarget(target.transform);
                missile.SetDamage(Damage);
                missile.SetSpeed(missileSpeed);
                missile.SetTurnSpeed(turnSpeed);
                missile.SetExplosionRadius(explosionRadius);
            }
        }
        
        protected override void OnUpgrade()
        {
            base.OnUpgrade();
            
            switch (currentLevel)
            {
                case 2:
                    missileCount = 2;
                    break;
                case 3:
                    explosionRadius *= 1.3f;
                    break;
                case 4:
                    missileSpeed *= 1.3f;
                    turnSpeed *= 1.3f;
                    break;
                case 5:
                    missileCount = 3;
                    break;
            }
        }
    }
}
