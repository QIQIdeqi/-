using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 冰属性效果 - 冰冻定身
    /// </summary>
    public class IceEffect : IProjectileEffect
    {
        private int level;
        
        // 等级配置
        private static readonly float[] FreezeDuration = { 0f, 1.5f, 2.5f, 4f };  // 冰冻时间
        private static readonly float[] DamageBoost = { 0f, 1.2f, 1.5f, 2f };    // 冰冻期间受到的伤害倍率
        
        public IceEffect(int lvl)
        {
            level = Mathf.Clamp(lvl, 1, 3);
        }
        
        public void Apply(GameObject target)
        {
            var enemy = target.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                // 冰冻敌人（定身）
                enemy.ApplyFreeze(
                    FreezeDuration[level],
                    DamageBoost[level]
                );
                
                Debug.Log($"[Ice] 施加冰冻效果: 持续{FreezeDuration[level]}秒, 易伤x{DamageBoost[level]}");
            }
        }
        
        public void Remove(GameObject target)
        {
            var enemy = target.GetComponent<EnemyBase>();
            enemy?.RemoveFreeze();
        }
        
        public string GetDescription()
        {
            return $"冰冻:定身{FreezeDuration[level]}秒,期间受到{DamageBoost[level]}倍伤害";
        }
    }
}
