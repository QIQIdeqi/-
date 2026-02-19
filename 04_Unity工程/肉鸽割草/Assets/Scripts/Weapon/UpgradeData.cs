using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// UpgradeType - Types of upgrades available
    /// </summary>
    public enum UpgradeType
    {
        NewWeapon,      // Add a new weapon
        UpgradeWeapon,  // Upgrade existing weapon
        StatBoost       // Boost player stats
    }
    
    /// <summary>
    /// UpgradeData - ScriptableObject for upgrade configuration
    /// </summary>
    [CreateAssetMenu(fileName = "NewUpgrade", menuName = "Geometry Warrior/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("[Basic Info]")]
        public string upgradeName = "Upgrade";
        public string description = "Upgrade description";
        public Sprite icon;
        public UpgradeType type = UpgradeType.NewWeapon;
        
        [Header("[Weapon Specific]")]
        public string weaponTypeName; // "LaserWeapon", "MissileWeapon", etc.
        
        [Header("[Stat Boost Specific]")]
        public StatBoostType statType;
        public float statValue;
        
        [Header("[Requirements]")]
        public int minLevel = 1;
        public int maxLevel = 5;
        
        public string GetDisplayName()
        {
            switch (type)
            {
                case UpgradeType.NewWeapon:
                    return $"New: {upgradeName}";
                case UpgradeType.UpgradeWeapon:
                    return $"Upgrade: {upgradeName}";
                case UpgradeType.StatBoost:
                    return upgradeName;
                default:
                    return upgradeName;
            }
        }
        
        public string GetDescription()
        {
            switch (type)
            {
                case UpgradeType.NewWeapon:
                    return description;
                case UpgradeType.UpgradeWeapon:
                    return $"{description} (Level +1)";
                case UpgradeType.StatBoost:
                    return $"{description} (+{statValue})";
                default:
                    return description;
            }
        }
    }
    
    public enum StatBoostType
    {
        MaxHealth,
        MoveSpeed,
        AttackSpeed,
        PickupRange
    }
}
