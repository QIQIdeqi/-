using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using GeometryWarrior;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// UI字体修复工具 - 将TextMeshPro替换为普通UI Text
    /// </summary>
    public class UIFontFixer : EditorWindow
    {
        [MenuItem("绒毛几何物语/工具/修复UI字体")]
        public static void ShowWindow()
        {
            GetWindow<UIFontFixer>("修复UI字体");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🔤 UI 字体修复工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "将场景和预制体中的 TextMeshProUGUI 替换为普通的 UI Text，\n" +
                "以解决中文和Emoji显示为方块的问题。", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("修复当前场景中的UI", GUILayout.Height(40)))
            {
                FixCurrentScene();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("重新生成所有UI预制体", GUILayout.Height(40)))
            {
                RegenerateAllPrefabs();
            }
        }
        
        private void FixCurrentScene()
        {
            int count = 0;
            
            // 查找所有 TextMeshProUGUI
            var tmpTexts = FindObjectsOfType<TMPro.TextMeshProUGUI>(true);
            
            foreach (var tmp in tmpTexts)
            {
                if (tmp == null) continue;
                
                GameObject go = tmp.gameObject;
                string text = tmp.text;
                int fontSize = (int)tmp.fontSize;
                Color color = tmp.color;
                TextAnchor alignment = ConvertAlignment(tmp.alignment);
                
                // 销毁 TextMeshProUGUI
                DestroyImmediate(tmp);
                
                // 添加普通 Text
                Text uiText = go.AddComponent<Text>();
                uiText.text = text;
                uiText.fontSize = fontSize;
                uiText.color = color;
                uiText.alignment = alignment;
                uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                
                count++;
            }
            
            EditorUtility.DisplayDialog("完成", $"已修复 {count} 个 UI 文本", "确定");
        }
        
        private TextAnchor ConvertAlignment(TMPro.TextAlignmentOptions alignment)
        {
            switch (alignment)
            {
                case TMPro.TextAlignmentOptions.TopLeft: return TextAnchor.UpperLeft;
                case TMPro.TextAlignmentOptions.TopRight: return TextAnchor.UpperRight;
                case TMPro.TextAlignmentOptions.Top: return TextAnchor.UpperCenter;
                case TMPro.TextAlignmentOptions.Left: return TextAnchor.MiddleLeft;
                case TMPro.TextAlignmentOptions.Right: return TextAnchor.MiddleRight;
                case TMPro.TextAlignmentOptions.BottomLeft: return TextAnchor.LowerLeft;
                case TMPro.TextAlignmentOptions.BottomRight: return TextAnchor.LowerRight;
                case TMPro.TextAlignmentOptions.Bottom: return TextAnchor.LowerCenter;
                default: return TextAnchor.MiddleCenter;
            }
        }
        
        private void RegenerateAllPrefabs()
        {
            if (EditorUtility.DisplayDialog("确认", 
                "这将重新生成所有UI预制体，现有预制体将被覆盖。\n\n是否继续？", "继续", "取消"))
            {
                // 调用UIBuilderTool重新生成
                UIBuilderTool.ShowWindow();
            }
        }
    }
}
