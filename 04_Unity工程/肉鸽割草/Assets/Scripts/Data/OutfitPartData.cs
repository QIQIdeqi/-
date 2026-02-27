using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 装扮部件数据 - 可装备的装饰部件（蝴蝶结、帽子等）
    /// </summary>
    [CreateAssetMenu(fileName = "NewOutfitPart", menuName = "绒毛几何物语/装扮部件数据")]
    public class OutfitPartData : ScriptableObject
    {
        [Header("【基本信息】")]
        [Tooltip("部件唯一ID，用于保存和识别")]
        public string partId;
        
        [Tooltip("显示名称，如'红色蝴蝶结'")]
        public string partName;
        
        [Tooltip("部件描述，显示在UI中")]
        public string description;
        
        [Tooltip("部件类别（蝴蝶结/帽子/眼镜等）")]
        public OutfitCategory category;
        
        [Tooltip("在列表中显示的图标")]
        public Sprite icon;
        
        [Header("【外观设置】")]
        [Tooltip("部件的精灵图片，会显示在角色身上")]
        public Sprite partSprite;
        
        [Tooltip("相对于角色的偏移位置（X,Y）")]
        public Vector2 offset;
        
        [Tooltip("旋转角度（度）")]
        public float rotation;
        
        [Tooltip("缩放比例（X,Y）")]
        public Vector2 scale = Vector2.one;
        
        [Header("【解锁条件】")]
        [Tooltip("是否默认解锁（无需条件即可获得）")]
        public bool isUnlockedByDefault;
        
        [Tooltip("达到此等级自动解锁（0表示不通过等级解锁）")]
        public int unlockLevel;
        
        [Tooltip("金币价格（0表示免费）")]
        public int price;
        
        [Header("【特效设置】")]
        [Tooltip("是否有粒子特效")]
        public bool hasParticleEffect;
        
        [Tooltip("发光颜色（如有发光特效）")]
        public Color glowColor = Color.white;
    }
    
    /// <summary>
    /// 装扮部件类别
    /// </summary>
    public enum OutfitCategory
    {
        Bow,        // 蝴蝶结
        Hat,        // 帽子
        Glasses,    // 眼镜
        Scarf,      // 围巾
        Backpack,   // 背包
        Shoes,      // 鞋子
        Special     // 特殊装饰
    }
}
