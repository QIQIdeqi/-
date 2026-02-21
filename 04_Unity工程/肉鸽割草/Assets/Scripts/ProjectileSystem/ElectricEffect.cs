using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 电属性效果 - 减速麻痹
    /// </summary>
    public class ElectricEffect : IProjectileEffect
    {
        private int level;
        
        // 等级配置
        private static readonly float[] SlowPercent = { 0f, 0.3f, 0.5f, 0.7f };  // 减速比例
        private static readonly float[] Duration = { 0f, 2f, 3.5f, 5f };         // 持续时间
        private static readonly float[] ChainChance = { 0f, 0f, 0.3f, 0.5f };    // 连锁几率（2级以上）
        private static readonly float[] ChainRange = { 0f, 0f, 3f, 5f };         // 连锁范围
        
        public ElectricEffect(int lvl)
        {
            level = Mathf.Clamp(lvl, 1, 3);
        }
        
        public void Apply(GameObject target)
        {
            var enemy = target.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                // 减速敌人
                enemy.ApplySlow(
                    SlowPercent[level], 
                    Duration[level]
                );
                
                // 2级以上：尝试连锁
                if (level >= 2 && Random.value < ChainChance[level])
                {
                    ApplyChainLightning(target.transform.position);
                }
                
                Debug.Log($"[Electric] 施加麻痹效果: 减速{SlowPercent[level]*100}%, 持续{Duration[level]}秒");
            }
        }
        
        void ApplyChainLightning(Vector3 origin)
        {
            // 查找范围内的其他敌人
            Collider2D[] nearby = Physics2D.OverlapCircleAll(origin, ChainRange[level], LayerMask.GetMask("Enemy"));
            
            foreach (var col in nearby)
            {
                if (col.gameObject != null && Random.value < 0.5f)
                {
                    var enemy = col.GetComponent<EnemyBase>();
                    enemy?.TakeDamage(5); // 连锁伤害
                    // TODO: 创建电弧视觉效果
                }
            }
        }
        
        public void Remove(GameObject target)
        {
            var enemy = target.GetComponent<EnemyBase>();
            enemy?.RemoveSlow();
        }
        
        public string GetDescription()
        {
            string desc = $"麻痹:减速{SlowPercent[level]*100:f0}%,持续{Duration[level]}秒";
            if (level >= 2)
            {
                desc += $",{ChainChance[level]*100:f0}%几率连锁闪电";
            }
            return desc;
        }
    }
}
