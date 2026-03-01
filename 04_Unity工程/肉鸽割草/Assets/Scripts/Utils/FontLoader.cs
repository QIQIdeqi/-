using UnityEngine;
using UnityEngine.UI;

namespace GeometryWarrior
{
    /// <summary>
    /// 字体加载工具 - 统一加载项目中文字体
    /// </summary>
    public static class FontLoader
    {
        private static Font simheiFont;
        private static Font legacyFont;
        
        /// <summary>
        /// 获取黑体字体（优先使用项目中的 SIMHEI.TTF）
        /// </summary>
        public static Font GetSimHeiFont()
        {
            // 如果已缓存，直接返回
            if (simheiFont != null)
                return simheiFont;
            
            // 尝试从 Resources 加载
            simheiFont = Resources.Load<Font>("Fonts/SIMHEI");
            
            // 如果找不到，尝试其他路径
            if (simheiFont == null)
            {
                simheiFont = Resources.Load<Font>("SIMHEI");
            }
            
            // 如果还找不到，尝试 TextMesh Pro 路径
            if (simheiFont == null)
            {
                #if UNITY_EDITOR
                // 编辑器模式下可以直接加载
                simheiFont = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>(
                    "Assets/TextMesh Pro/Fonts/SIMHEI.TTF");
                #endif
            }
            
            // 最后回退到系统字体或 LegacyRuntime
            if (simheiFont == null)
            {
                simheiFont = GetLegacyFont();
            }
            
            return simheiFont;
        }
        
        /// <summary>
        /// 获取 LegacyRuntime 字体（内置回退字体）
        /// </summary>
        public static Font GetLegacyFont()
        {
            if (legacyFont != null)
                return legacyFont;
            
            legacyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return legacyFont;
        }
        
        /// <summary>
        /// 设置 Text 组件的字体为黑体
        /// </summary>
        public static void SetSimHeiFont(Text textComponent)
        {
            if (textComponent != null)
            {
                textComponent.font = GetSimHeiFont();
            }
        }
        
        /// <summary>
        /// 清除缓存（用于热重载）
        /// </summary>
        public static void ClearCache()
        {
            simheiFont = null;
            legacyFont = null;
        }
    }
}
