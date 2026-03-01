using UnityEngine;
using UnityEditor;
using GeometryWarrior;
using FluffyGeometry.UI;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// BackpackPanel 列表布局修复工具（已弃用 - 使用 UIBuilderTool 替代）
    /// </summary>
    public class BackpackPanelLayoutFixer : EditorWindow
    {
        [MenuItem("绒毛几何物语/旧版工具/BackpackPanel列表布局（已弃用）")]
        public static void ShowWindow()
        {
            EditorUtility.DisplayDialog("提示", 
                "此工具已弃用！\n\n请使用新的 UIBuilderTool：\n" +
                "菜单 → 绒毛几何物语 → UI工具 → 生成OutfitPanel", "确定");
        }
    }
}
