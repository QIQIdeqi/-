using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 皮肤配置创建器 - 提供快速创建示例皮肤的方法
    /// </summary>
    public static class SkinConfigCreator
    {
        /// <summary>
        /// 创建默认皮肤配置
        /// </summary>
        public static SkinData CreateDefaultSkin()
        {
            var skin = ScriptableObject.CreateInstance<SkinData>();
            skin.skinId = "skin_default";
            skin.skinName = "经典战士";
            skin.description = "最经典的战士外观，陪伴你踏上冒险之旅";
            skin.isUnlockedByDefault = true;
            skin.isFree = true;
            skin.tintColor = Color.white;
            skin.hasTrailEffect = false;
            skin.hasGlowEffect = false;
            return skin;
        }
        
        /// <summary>
        /// 创建火焰皮肤
        /// </summary>
        public static SkinData CreateFireSkin()
        {
            var skin = ScriptableObject.CreateInstance<SkinData>();
            skin.skinId = "skin_fire";
            skin.skinName = "烈焰勇士";
            skin.description = "燃烧的意志，如同烈火般炽热";
            skin.isUnlockedByDefault = false;
            skin.unlockLevel = 3;
            skin.isFree = true;
            skin.tintColor = new Color(1f, 0.8f, 0.6f);
            skin.hasTrailEffect = true;
            skin.trailColor = new Color(1f, 0.3f, 0f, 0.8f);
            skin.hasGlowEffect = true;
            skin.glowColor = new Color(1f, 0.5f, 0f, 0.5f);
            return skin;
        }
        
        /// <summary>
        /// 创建冰霜皮肤
        /// </summary>
        public static SkinData CreateIceSkin()
        {
            var skin = ScriptableObject.CreateInstance<SkinData>();
            skin.skinId = "skin_ice";
            skin.skinName = "冰霜行者";
            skin.description = "来自极寒之地的战士，周身环绕着寒气";
            skin.isUnlockedByDefault = false;
            skin.unlockScore = 5000;
            skin.isFree = true;
            skin.tintColor = new Color(0.7f, 0.9f, 1f);
            skin.hasTrailEffect = true;
            skin.trailColor = new Color(0f, 0.8f, 1f, 0.7f);
            skin.hasGlowEffect = true;
            skin.glowColor = new Color(0.5f, 0.8f, 1f, 0.4f);
            return skin;
        }
        
        /// <summary>
        /// 创建黄金皮肤
        /// </summary>
        public static SkinData CreateGoldSkin()
        {
            var skin = ScriptableObject.CreateInstance<SkinData>();
            skin.skinId = "skin_gold";
            skin.skinName = "黄金战神";
            skin.description = "传说中的黄金铠甲，只有最强大的战士才能驾驭";
            skin.isUnlockedByDefault = false;
            skin.unlockLevel = 10;
            skin.isFree = false;
            skin.price = 1000;
            skin.tintColor = new Color(1f, 0.85f, 0.3f);
            skin.hasTrailEffect = true;
            skin.trailColor = new Color(1f, 0.8f, 0f, 0.9f);
            skin.hasGlowEffect = true;
            skin.glowColor = new Color(1f, 0.9f, 0.5f, 0.6f);
            return skin;
        }
        
        /// <summary>
        /// 创建霓虹皮肤（Shader特效）
        /// </summary>
        public static SkinData CreateNeonSkin()
        {
            var skin = ScriptableObject.CreateInstance<SkinData>();
            skin.skinId = "skin_neon";
            skin.skinName = "霓虹幻影";
            skin.description = "来自未来的赛博战士，身上流动着霓虹光芒";
            skin.isUnlockedByDefault = false;
            skin.isFree = false;
            skin.price = 2000;
            skin.tintColor = Color.white;
            skin.hasTrailEffect = true;
            skin.trailColor = new Color(0.5f, 0f, 1f, 0.8f);
            skin.hasGlowEffect = true;
            skin.glowColor = new Color(0.8f, 0.2f, 1f, 0.7f);
            // skin.customMaterial = ... // 需要赋值一个发光Shader材质
            return skin;
        }
        
        /// <summary>
        /// 创建暗影皮肤
        /// </summary>
        public static SkinData CreateShadowSkin()
        {
            var skin = ScriptableObject.CreateInstance<SkinData>();
            skin.skinId = "skin_shadow";
            skin.skinName = "暗影刺客";
            skin.description = "潜伏在黑暗中的神秘战士";
            skin.isUnlockedByDefault = false;
            skin.unlockLevel = 5;
            skin.isFree = true;
            skin.tintColor = new Color(0.4f, 0.4f, 0.5f);
            skin.hasTrailEffect = true;
            skin.trailColor = new Color(0.2f, 0.2f, 0.3f, 0.6f);
            skin.hasGlowEffect = false;
            return skin;
        }
    }
}
