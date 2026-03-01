using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 快速添加缺失的UI元素
    /// </summary>
    public class QuickUIElementAdder : EditorWindow
    {
        [MenuItem("绒毛几何物语/工具/添加缺失的UI元素")]
        public static void ShowWindow()
        {
            GetWindow<QuickUIElementAdder>("添加UI元素");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("➕ 快速添加UI元素", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "选择场景中的目标面板，然后点击按钮添加缺失的元素。", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("为 MainMenuPanel 添加 EnergyCoinsText", GUILayout.Height(40)))
            {
                AddEnergyCoinsTextToMainMenu();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("为 MainMenuPanel 添加 HighScoreText", GUILayout.Height(40)))
            {
                AddHighScoreTextToMainMenu();
            }
        }
        
        private void AddEnergyCoinsTextToMainMenu()
        {
            // 查找 MainMenuPanel
            MainMenuPanel mainMenu = FindObjectOfType<MainMenuPanel>();
            if (mainMenu == null)
            {
                EditorUtility.DisplayDialog("错误", "场景中没有找到 MainMenuPanel！", "确定");
                return;
            }
            
            // 创建 EnergyCoinsText
            GameObject textObj = new GameObject("EnergyCoinsText", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(mainMenu.transform, false);
            
            // 配置 RectTransform
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(200, 40);
            
            // 配置 Text
            Text text = textObj.GetComponent<Text>();
            text.text = "能量币: 0";
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleRight;
            text.color = new Color(0.365f, 0.251f, 0.216f);
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            // 绑定到 MainMenuPanel
            SerializedObject so = new SerializedObject(mainMenu);
            so.FindProperty("energyCoinsTextLegacy").objectReferenceValue = text;
            so.ApplyModifiedProperties();
            
            EditorUtility.DisplayDialog("完成", "已添加 EnergyCoinsText 并绑定到 MainMenuPanel！", "确定");
            Selection.activeGameObject = textObj;
        }
        
        private void AddHighScoreTextToMainMenu()
        {
            // 查找 MainMenuPanel
            MainMenuPanel mainMenu = FindObjectOfType<MainMenuPanel>();
            if (mainMenu == null)
            {
                EditorUtility.DisplayDialog("错误", "场景中没有找到 MainMenuPanel！", "确定");
                return;
            }
            
            // 创建 HighScoreText
            GameObject textObj = new GameObject("HighScoreText", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(mainMenu.transform, false);
            
            // 配置 RectTransform
            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -60);
            rect.sizeDelta = new Vector2(200, 40);
            
            // 配置 Text
            Text text = textObj.GetComponent<Text>();
            text.text = "最高分: 0";
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleRight;
            text.color = new Color(0.365f, 0.251f, 0.216f);
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            // 绑定到 MainMenuPanel
            SerializedObject so = new SerializedObject(mainMenu);
            so.FindProperty("highScoreTextLegacy").objectReferenceValue = text;
            so.ApplyModifiedProperties();
            
            EditorUtility.DisplayDialog("完成", "已添加 HighScoreText 并绑定到 MainMenuPanel！", "确定");
            Selection.activeGameObject = textObj;
        }
    }
}
