using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using GeometryWarrior;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// GameHUD 引用修复工具
    /// </summary>
    public class GameHUDFixer : EditorWindow
    {
        [MenuItem("绒毛几何物语/工具/修复GameHUD引用")]
        public static void ShowWindow()
        {
            GetWindow<GameHUDFixer>("修复GameHUD");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🔧 GameHUD 引用修复", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "自动查找场景中的 GameHUD 并重新绑定引用。\n" +
                "如果 GameHUD 参数丢失，运行此工具。", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("自动修复GameHUD引用", GUILayout.Height(50)))
            {
                FixGameHUD();
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("查找所有Slider和Text", GUILayout.Height(30)))
            {
                FindAllUIElements();
            }
        }
        
        private void FixGameHUD()
        {
            int fixedCount = 0;
            
            // 查找所有 GameHUD
            GameHUD[] huds = FindObjectsOfType<GameHUD>(true);
            
            foreach (var hud in huds)
            {
                if (hud == null) continue;
                
                SerializedObject so = new SerializedObject(hud);
                
                // 查找子物体
                Transform hudTrans = hud.transform;
                
                // Health Slider
                Slider healthSlider = FindComponentInChildren<Slider>(hudTrans, "Health");
                if (healthSlider != null)
                {
                    so.FindProperty("healthSlider").objectReferenceValue = healthSlider;
                }
                
                // Health Text
                TextMeshProUGUI healthText = FindComponentInChildren<TextMeshProUGUI>(hudTrans, "HealthText");
                if (healthText == null)
                {
                    // 尝试普通 Text
                    Text healthTextLegacy = FindComponentInChildren<Text>(hudTrans, "HealthText");
                    if (healthTextLegacy != null)
                    {
                        // 转换为 TMP
                        healthText = ConvertToTMP(healthTextLegacy);
                    }
                }
                if (healthText != null)
                {
                    so.FindProperty("healthText").objectReferenceValue = healthText;
                }
                
                // Exp Slider
                Slider expSlider = FindComponentInChildren<Slider>(hudTrans, "Exp");
                if (expSlider != null)
                {
                    so.FindProperty("expSlider").objectReferenceValue = expSlider;
                }
                
                // Level Text
                TextMeshProUGUI levelText = FindComponentInChildren<TextMeshProUGUI>(hudTrans, "Level");
                if (levelText != null)
                {
                    so.FindProperty("levelText").objectReferenceValue = levelText;
                }
                
                // Score Text
                TextMeshProUGUI scoreText = FindComponentInChildren<TextMeshProUGUI>(hudTrans, "Score");
                if (scoreText != null)
                {
                    so.FindProperty("scoreText").objectReferenceValue = scoreText;
                }
                
                // Time Text
                TextMeshProUGUI timeText = FindComponentInChildren<TextMeshProUGUI>(hudTrans, "Time");
                if (timeText != null)
                {
                    so.FindProperty("timeText").objectReferenceValue = timeText;
                }
                
                so.ApplyModifiedProperties();
                
                Debug.Log($"[GameHUDFixer] 已修复: {hud.name}");
                fixedCount++;
            }
            
            // 同时修复 MainMenuPanel
            FixMainMenuPanels();
            
            // 同时修复 GameOverPanel
            FixGameOverPanels();
            
            EditorUtility.DisplayDialog("完成", $"修复了 {fixedCount} 个 GameHUD、MainMenuPanel 和 GameOverPanel", "确定");
        }
        
        private T FindComponentInChildren<T>(Transform parent, string nameContains) where T : Component
        {
            T[] components = parent.GetComponentsInChildren<T>(true);
            foreach (var comp in components)
            {
                if (comp.name.Contains(nameContains))
                {
                    return comp;
                }
            }
            return null;
        }
        
        private TextMeshProUGUI ConvertToTMP(Text legacyText)
        {
            if (legacyText == null) return null;
            
            GameObject go = legacyText.gameObject;
            string text = legacyText.text;
            int fontSize = legacyText.fontSize;
            Color color = legacyText.color;
            
            DestroyImmediate(legacyText);
            
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            
            return tmp;
        }
        
        private void FixMainMenuPanels()
        {
            MainMenuPanel[] panels = FindObjectsOfType<MainMenuPanel>(true);
            
            foreach (var panel in panels)
            {
                if (panel == null) continue;
                
                SerializedObject so = new SerializedObject(panel);
                Transform panelTrans = panel.transform;
                
                // HighScore Text
                Text highScoreLegacy = FindComponentInChildren<Text>(panelTrans, "HighScore");
                if (highScoreLegacy != null)
                {
                    so.FindProperty("highScoreTextLegacy").objectReferenceValue = highScoreLegacy;
                }
                
                // EnergyCoins Text
                Text energyLegacy = FindComponentInChildren<Text>(panelTrans, "Energy");
                if (energyLegacy != null)
                {
                    so.FindProperty("energyCoinsTextLegacy").objectReferenceValue = energyLegacy;
                }
                
                so.ApplyModifiedProperties();
                
                Debug.Log($"[GameHUDFixer] 已修复 MainMenuPanel: {panel.name}");
            }
        }
        
        private void FixGameOverPanels()
        {
            GameOverPanel[] panels = FindObjectsOfType<GameOverPanel>(true);
            
            foreach (var panel in panels)
            {
                if (panel == null) continue;
                
                SerializedObject so = new SerializedObject(panel);
                Transform panelTrans = panel.transform;
                
                // Title Text
                Text titleLegacy = FindComponentInChildren<Text>(panelTrans, "Title");
                if (titleLegacy != null)
                    so.FindProperty("titleTextLegacy").objectReferenceValue = titleLegacy;
                
                // Score Text
                Text scoreLegacy = FindComponentInChildren<Text>(panelTrans, "Score");
                if (scoreLegacy != null)
                    so.FindProperty("scoreTextLegacy").objectReferenceValue = scoreLegacy;
                
                // Time Text
                Text timeLegacy = FindComponentInChildren<Text>(panelTrans, "Time");
                if (timeLegacy != null)
                    so.FindProperty("timeTextLegacy").objectReferenceValue = timeLegacy;
                
                // Energy Text
                Text energyLegacy = FindComponentInChildren<Text>(panelTrans, "Energy");
                if (energyLegacy != null)
                    so.FindProperty("energyCoinsTextLegacy").objectReferenceValue = energyLegacy;
                
                // HighScore Text
                Text highScoreLegacy = FindComponentInChildren<Text>(panelTrans, "High");
                if (highScoreLegacy != null)
                    so.FindProperty("highScoreTextLegacy").objectReferenceValue = highScoreLegacy;
                
                // Revive Button Text
                Text reviveBtnLegacy = FindComponentInChildren<Text>(panelTrans, "Revive");
                if (reviveBtnLegacy != null)
                    so.FindProperty("reviveButtonTextLegacy").objectReferenceValue = reviveBtnLegacy;
                
                // Revive Hint Text
                Text reviveHintLegacy = FindComponentInChildren<Text>(panelTrans, "Hint");
                if (reviveHintLegacy != null)
                    so.FindProperty("reviveHintTextLegacy").objectReferenceValue = reviveHintLegacy;
                
                so.ApplyModifiedProperties();
                
                Debug.Log($"[GameHUDFixer] 已修复 GameOverPanel: {panel.name}");
            }
        }
        
        private void FindAllUIElements()
        {
            string result = "场景中的 UI 元素:\n\n";
            
            Slider[] sliders = FindObjectsOfType<Slider>(true);
            result += $"Sliders: {sliders.Length}\n";
            foreach (var s in sliders)
            {
                result += $"  - {s.name}\n";
            }
            
            result += "\n";
            
            TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>(true);
            result += $"TextMeshPro Texts: {texts.Length}\n";
            foreach (var t in texts)
            {
                result += $"  - {t.name}\n";
            }
            
            EditorUtility.DisplayDialog("UI元素列表", result, "确定");
        }
    }
}
