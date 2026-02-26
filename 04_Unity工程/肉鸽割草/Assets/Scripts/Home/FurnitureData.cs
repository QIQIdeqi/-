using UnityEngine;

namespace FluffyGeometry.Home
{
    /// <summary>
    /// 家具数据 - 可收集的家园装饰物
    /// </summary>
    [CreateAssetMenu(fileName = "NewFurniture", menuName = "绒毛几何物语/家具数据")]
    public class FurnitureData : ScriptableObject
    {
        [Header("【基本信息】")]
        [Tooltip("家具唯一ID")]
        public string furnitureId;
        
        [Tooltip("家具名称")]
        public string furnitureName;
        
        [Tooltip("家具描述")]
        [TextArea(2, 4)]
        public string description;
        
        [Header("【外观】")]
        [Tooltip("家具图标（背包中显示）")]
        public Sprite iconSprite;
        
        [Tooltip("家具原尺寸图（场景中显示）")]
        public Sprite furnitureSprite;
        
        [Tooltip("家具分类")]
        public FurnitureCategory category;
        
        [Header("【尺寸】")]
        [Tooltip("默认缩放比例")]
        public float defaultScale = 1f;
        
        [Tooltip("最小缩放")]
        public float minScale = 0.5f;
        
        [Tooltip("最大缩放")]
        public float maxScale = 2f;
        
        [Header("【状态】")]
        [Tooltip("是否已解锁")]
        public bool isUnlocked = true;
        
        [Tooltip("是否可以翻转")]
        public bool canFlip = true;
        
        [Header("【解锁条件】")]
        [Tooltip("是否需要合成解锁")]
        public bool requireCraft = false;
        
        [Tooltip("合成所需材料")]
        public CraftMaterial[] craftMaterials;
    }
    
    /// <summary>
    /// 家具分类
    /// </summary>
    public enum FurnitureCategory
    {
        座椅,
        桌子,
        装饰,
        灯具,
        墙面,
        地面,
        植物,
        其他
    }
    
    /// <summary>
    /// 合成材料
    /// </summary>
    [System.Serializable]
    public class CraftMaterial
    {
        [Tooltip("材料ID")]
        public string materialId;
        
        [Tooltip("所需数量")]
        public int amount;
    }
}
