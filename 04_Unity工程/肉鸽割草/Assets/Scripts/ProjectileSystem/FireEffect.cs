using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 火属性效果 - 持续燃烧伤害
    /// </summary>
    public class FireEffect : IProjectileEffect
    {
        private float baseDamage;
        private int level;
        
        // 等级配置
        private static readonly float[] DamagePerSecond = { 0f, 2f, 4f, 8f };    // 每秒伤害
        private static readonly float[] Duration = { 0f, 3f, 5f, 8f };           // 持续时间
        private static readonly float[] TickInterval = { 0f, 0.5f, 0.5f, 0.3f }; // 伤害间隔（秒）
        
        public FireEffect(float baseDmg, int lvl)
        {
            baseDamage = baseDmg;
            level = Mathf.Clamp(lvl, 1, 3);
        }
        
        public void Apply(GameObject target)
        {
            var enemy = target.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                // 添加燃烧Debuff
                enemy.ApplyBurning(
                    DamagePerSecond[level], 
                    Duration[level], 
                    TickInterval[level]
                );
                
                Debug.Log($"[Fire] 施加燃烧效果: 每秒{DamagePerSecond[level]}伤害, 持续{Duration[level]}秒");
            }
        }
        
        public void Remove(GameObject target)
        {
            var enemy = target.GetComponent<EnemyBase>();
            enemy?.RemoveBurning();
        }
        
        public string GetDescription()
        {
            return $"燃烧:每秒造成{DamagePerSecond[level]}伤害,持续{Duration[level]}秒";
        }
    }
}
