using UnityEngine;
using System.Collections;

namespace GeometryWarrior
{
    /// <summary>
    /// LaserWeapon - Fires penetrating laser beams at enemies
    /// </summary>
    public class LaserWeapon : WeaponBase
    {
        [Header("[Laser Settings]")]
        [SerializeField] private float laserRange = 12f;
        [SerializeField] private float laserWidth = 0.3f;
        [SerializeField] private float laserDuration = 0.2f;
        [SerializeField] private Color laserColor = Color.red;
        [SerializeField] private int laserCount = 1; // Multi-beam at higher levels
        [SerializeField] private float laserAngleSpread = 30f; // Angle between beams
        
        [Header("[Prefabs]")]
        [SerializeField] private GameObject laserPrefab;
        
        private LineRenderer lineRenderer;
        private LineRenderer[] extraLineRenderers;
        private bool isFiring;
        
        protected override void Start()
        {
            base.Start();
            weaponName = "Laser Beam";
            description = "Fires penetrating laser beams at the nearest enemy";
            attackInterval = 0.8f;
            baseDamage = 15;
            
            // Create line renderer for laser visual
            GameObject laserObj = new GameObject("LaserVisual");
            laserObj.transform.SetParent(transform, false);
            lineRenderer = laserObj.AddComponent<LineRenderer>();
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = laserColor;
            lineRenderer.endColor = laserColor;
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
        
        protected override void PerformAttack()
        {
            if (isFiring) return;
            
            EnemyBase target = FindNearestEnemy(laserRange);
            if (target == null) return;
            
            StartCoroutine(FireLasers(target));
        }
        
        private IEnumerator FireLasers(EnemyBase target)
        {
            isFiring = true;
            
            Vector2 startPos = playerTransform.position;
            Vector2 baseDirection = (target.transform.position - playerTransform.position).normalized;
            float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
            
            // Fire multiple lasers at different angles
            for (int i = 0; i < laserCount; i++)
            {
                float angleOffset = 0f;
                if (laserCount > 1)
                {
                    angleOffset = -laserAngleSpread / 2f + (laserAngleSpread / (laserCount - 1)) * i;
                }
                float angle = baseAngle + angleOffset;
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                
                // Show laser visual
                LineRenderer lr = i == 0 ? lineRenderer : GetExtraLineRenderer(i - 1);
                lr.SetPosition(0, startPos);
                lr.SetPosition(1, startPos + direction * laserRange);
                lr.enabled = true;
                
                // Deal damage to all enemies in line
                RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, direction, laserRange);
                foreach (RaycastHit2D hit in hits)
                {
                    EnemyBase enemy = hit.collider.GetComponent<EnemyBase>();
                    if (enemy != null && !enemy.IsDead)
                    {
                        enemy.TakeDamage(Damage);
                    }
                }
            }
            
            // Wait for duration
            yield return new WaitForSeconds(laserDuration);
            
            // Hide all lasers
            lineRenderer.enabled = false;
            if (extraLineRenderers != null)
            {
                foreach (LineRenderer lr in extraLineRenderers)
                {
                    if (lr != null) lr.enabled = false;
                }
            }
            
            isFiring = false;
        }
        
        private LineRenderer GetExtraLineRenderer(int index)
        {
            if (extraLineRenderers == null)
            {
                extraLineRenderers = new LineRenderer[4]; // Max 4 extra lasers
            }
            
            if (extraLineRenderers[index] == null)
            {
                GameObject laserObj = new GameObject($"LaserVisual_{index + 1}");
                laserObj.transform.SetParent(transform, false);
                LineRenderer lr = laserObj.AddComponent<LineRenderer>();
                lr.startWidth = laserWidth;
                lr.endWidth = laserWidth;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = laserColor;
                lr.endColor = laserColor;
                lr.positionCount = 2;
                lr.enabled = false;
                extraLineRenderers[index] = lr;
            }
            
            return extraLineRenderers[index];
        }
        
        protected override void OnUpgrade()
        {
            base.OnUpgrade();
            
            // Upgrade effects
            switch (currentLevel)
            {
                case 2:
                    laserRange *= 1.2f;
                    break;
                case 3:
                    laserWidth *= 1.5f;
                    break;
                case 4:
                    laserDuration *= 1.5f;
                    break;
                case 5:
                    // Multi-beam at max level
                    laserCount = 3;
                    break;
            }
            
            // Update line renderer
            if (lineRenderer != null)
            {
                lineRenderer.startWidth = laserWidth;
                lineRenderer.endWidth = laserWidth;
            }
        }
    }
}
