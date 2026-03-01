using UnityEngine;
using UnityEditor;
using GeometryWarrior;
using FluffyGeometry.Home;

namespace FluffyGeometry.Editor
{
    /// <summary>
    /// 背包系统一键配置工具（已弃用 - 使用 UIBuilderTool 替代）
    /// </summary>
    public class BackpackSystemSetupWizard : EditorWindow
    {
        [MenuItem("绒毛几何物语/旧版工具/背包系统一键配置（已弃用）")]
        public static void ShowWindow()
        {
            EditorUtility.DisplayDialog("提示", 
                "此工具已弃用！\n\n请使用新的 UIBuilderTool：\n" +
                "菜单 → 绒毛几何物语 → UI工具 → 生成OutfitPanel\n\n" +
                "新的工具可以生成：\n" +
                "- OutfitPanelNew\n" +
                "- HomeHUD\n" +
                "- BackpackPanel\n" +
                "- PartItem 预制体", "确定");
        }
    }
}
