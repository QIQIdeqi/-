using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 飞弹视觉管理器 - 单例，管理所有飞弹的视觉表现
    /// </summary>
    public class ProjectileVisualManager : MonoBehaviour
    {
        public static ProjectileVisualManager Instance { get; private set; }
        
        [Header("材质配置")]
        public Material fireMaterial;
        public Material iceMaterial;
        public Material electricMaterial;
        public Material defaultMaterial;
        
        [Header("形状配置 - Sprite")]
        public Sprite fireShape;      // 三角形
        public Sprite iceShape;       // 六边形
        public Sprite electricShape;  // 折线/球形
        public Sprite defaultShape;   // 默认圆形
        
        [Header("颜色配置")]
        public Color fireColor = new Color(1f, 0.27f, 0.27f);      // 火红
        public Color iceColor = new Color(0f, 0.53f, 1f);          // 冰蓝
        public Color electricColor = new Color(0.53f, 0.27f, 1f);  // 电紫
        
        [Header("Trail配置")]
        public float trailTime = 0.15f;
        public AnimationCurve trailWidthCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        /// <summary>
        /// 应用视觉配置到飞弹
        /// </summary>
        public void ApplyVisual(Projectile proj, ElementType type, int level)
        {
            if (proj.spriteRenderer == null) return;
            
            // 设置形状
            proj.spriteRenderer.sprite = GetShape(type);
            
            // 设置材质
            proj.spriteRenderer.material = GetMaterial(type);
            
            // 设置颜色（根据等级调整亮度）
            Color baseColor = GetBaseColor(type);
            proj.spriteRenderer.color = GetLevelColor(baseColor, level);
            
            // 设置拖尾
            if (proj.trailRenderer != null)
            {
                ApplyTrail(proj.trailRenderer, baseColor, level);
            }
            
            // 设置自发光强度
            float emission = 1f + (level - 1) * 0.5f;
            proj.spriteRenderer.material.SetFloat("_Emission", emission);
        }
        
        Sprite GetShape(ElementType type)
        {
            switch (type)
            {
                case ElementType.Fire: return fireShape;
                case ElementType.Ice: return iceShape;
                case ElementType.Electric: return electricShape;
                default: return defaultShape;
            }
        }
        
        Material GetMaterial(ElementType type)
        {
            switch (type)
            {
                case ElementType.Fire: return fireMaterial != null ? fireMaterial : defaultMaterial;
                case ElementType.Ice: return iceMaterial != null ? iceMaterial : defaultMaterial;
                case ElementType.Electric: return electricMaterial != null ? electricMaterial : defaultMaterial;
                default: return defaultMaterial;
            }
        }
        
        Color GetBaseColor(ElementType type)
        {
            switch (type)
            {
                case ElementType.Fire: return fireColor;
                case ElementType.Ice: return iceColor;
                case ElementType.Electric: return electricColor;
                default: return Color.white;
            }
        }
        
        Color GetLevelColor(Color baseColor, int level)
        {
            // 等级越高越亮
            float brightness = 1f + (level - 1) * 0.3f;
            return baseColor * brightness;
        }
        
        void ApplyTrail(TrailRenderer trail, Color color, int level)
        {
            trail.time = trailTime;
            trail.widthCurve = trailWidthCurve;
            trail.colorGradient = CreateTrailGradient(color, level);
        }
        
        Gradient CreateTrailGradient(Color color, int level)
        {
            var grad = new Gradient();
            
            // 根据等级调整透明度
            float alpha = 0.5f + (level - 1) * 0.2f;
            
            grad.SetKeys(
                new GradientColorKey[] 
                { 
                    new GradientColorKey(color, 0f),
                    new GradientColorKey(Color.white, 0.3f),
                    new GradientColorKey(new Color(color.r, color.g, color.b, 0f), 1f)
                },
                new GradientAlphaKey[] 
                { 
                    new GradientAlphaKey(alpha, 0f),
                    new GradientAlphaKey(alpha * 0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            
            return grad;
        }
        
        /// <summary>
        /// 获取属性颜色（静态方法，供其他脚本使用）
        /// </summary>
        public static Color GetElementColor(ElementType type)
        {
            if (Instance == null) return Color.white;
            return Instance.GetBaseColor(type);
        }
    }
}
