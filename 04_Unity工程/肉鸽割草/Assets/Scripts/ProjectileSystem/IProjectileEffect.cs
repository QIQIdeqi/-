using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 属性效果接口 - 所有属性效果需实现此接口
    /// </summary>
    public interface IProjectileEffect
    {
        /// <summary>
        /// 应用效果到目标
        /// </summary>
        void Apply(GameObject target);
        
        /// <summary>
        /// 从目标移除效果
        /// </summary>
        void Remove(GameObject target);
        
        /// <summary>
        /// 获取效果描述
        /// </summary>
        string GetDescription();
    }
}
