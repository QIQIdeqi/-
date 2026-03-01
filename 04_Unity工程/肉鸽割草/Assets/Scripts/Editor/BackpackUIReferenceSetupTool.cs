using UnityEngine;
using UnityEditor;
using GeometryWarrior;

namespace FluffyGeometry.Editor
{
    /// <summary>
    /// 背包UI引用配置工具（已弃用 - 使用 UIBuilderTool 替代）
    /// </summary>
    public class BackpackUIReferenceSetupTool : EditorWindow
    {
        [MenuItem("绒毛几何物语/旧版工具/背包UI引用配置（已弃用）")]
        public static void ShowWindow()
        {
            EditorUtility.DisplayDialog("提示", 
                "此工具已弃用！\n\n请使用新的 UIBuilderTool：\n" +
                "菜单 → 绒毛几何物语 → UI工具 → 生成OutfitPanel", "确定");
        }
    }
}
