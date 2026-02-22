using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 皮肤数据
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkin", menuName = "Geometry Warrior/Skin Data")]
    public class SkinData : ScriptableObject
    {
        [Header("[基本信息]")]
        public string skinId;           // 唯一ID
        public string skinName;         // 显示名称
        public string description;      // 描述
        public Sprite icon;             // 图标
        
        [Header("[外观]")]
        public Sprite playerSprite;     // 主角精灵
        public Color tintColor = Color.white;  // 染色
        
        [Header("[特效]")]
        public bool hasTrailEffect;     // 是否有拖尾特效
        public Color trailColor = Color.white; // 拖尾颜色
        public bool hasGlowEffect;      // 是否有发光特效
        public Color glowColor = Color.white;  // 发光颜色
        public Material customMaterial; // 自定义材质（Shader）
        
        [Header("[解锁条件]")]
        public bool isUnlockedByDefault; // 默认解锁
        public int unlockLevel;          // 达到等级解锁
        public int unlockScore;          // 达到分数解锁
        public string unlockAchievement; // 成就ID解锁
        
        [Header("[价格]")]
        public bool isFree = true;
        public int price;                // 金币价格
    }
}
