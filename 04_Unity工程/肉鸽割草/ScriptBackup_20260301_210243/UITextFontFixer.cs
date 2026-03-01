using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// UI文本字体修复工具 - 修复所有Text的字体设置
    /// </summary>
    public class UITextFontFixer : EditorWindow
    {
        private Font targetFont;
        
        [MenuItem("绒毛几何物语/工具/修复所有文本字体")]
        public static void ShowWindow()
        {
            GetWindow<UITextFontFixer>("修复文本字体");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🔤 UI 文本字体修复", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "修复场景中所有 UI Text 的字体设置。\n" +
                "如果不指定字体，将自动使用 Arial 字体。", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            targetFont = (Font)EditorGUILayout.ObjectField("目标字体 (可选):", targetFont, typeof(Font), false);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("修复场景中所有Text", GUILayout.Height(40)))
            {
                FixAllTextsInScene();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("加载Arial字体", GUILayout.Height(30)))
            {
                targetFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
        }
        
        private void FixAllTextsInScene()
        {
            int count = 0;
            
            // 获取字体
            Font fontToUse = targetFont;
            if (fontToUse == null)
            {
                fontToUse = Resources.GetBuiltinResource<Font>("Arial.ttf");
                if (fontToUse == null)
                {
                    EditorUtility.DisplayDialog("错误", "无法加载Arial字体，请手动指定字体！", "确定");
                    return;
                }
            }
            
            // 查找所有UI Text
            Text[] allTexts = FindObjectsOfType<Text>(true);
            
            foreach (var text in allTexts)
            {
                if (text == null) continue;
                
                Undo.RecordObject(text, "Fix Font");
                
                // 设置字体
                text.font = fontToUse;
                
                // 确保颜色不是透明的
                if (text.color.a < 0.1f)
                {
                    Color c = text.color;
                    c.a = 1f;
                    text.color = c;
                }
                
                // 确保字体大小合理
                if (text.fontSize < 10)
                {
                    text.fontSize = 24;
                }
                
                count++;
            }
            
            Debug.Log($"[UITextFontFixer] 修复了 {count} 个 Text 的字体");
            EditorUtility.DisplayDialog("完成", $"已修复 {count} 个 UI Text 的字体设置！", "确定");
        }
    }
}
