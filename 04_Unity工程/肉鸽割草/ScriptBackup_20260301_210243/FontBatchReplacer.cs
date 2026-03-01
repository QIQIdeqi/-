using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 字体批量替换工具 - 一键替换项目中所有Text/TMP的字体
    /// </summary>
    public class FontBatchReplacer : EditorWindow
    {
        // 原字体筛选（可选）
        private Font oldFont;
        private TMP_FontAsset oldTMPFont;
        
        // 新字体
        private Font newFont;
        private TMP_FontAsset newTMPFont;
        private Material newTMPMaterial;
        
        // 替换范围
        private bool replaceInScene = true;
        private bool replaceInPrefabs = true;
        
        // 统计
        private int replacedTextCount = 0;
        private int replacedTMPCount = 0;
        private int skippedCount = 0;
        private List<string> modifiedPrefabPaths = new List<string>();
        
        [MenuItem("绒毛几何物语/工具/字体批量替换")]
        public static void ShowWindow()
        {
            GetWindow<FontBatchReplacer>("字体批量替换");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ 字体批量替换工具 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "此工具会批量替换项目中所有 Text 和 TMP 的字体。\n" +
                "可以选择只替换特定字体，或替换全部。", 
                MessageType.Info);
            
            GUILayout.Space(10);
            
            // 筛选条件（可选）
            EditorGUILayout.LabelField("筛选条件（可选）", EditorStyles.boldLabel);
            oldFont = (Font)EditorGUILayout.ObjectField("原 uGUI 字体", oldFont, typeof(Font), false);
            oldTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("原 TMP 字体", oldTMPFont, typeof(TMP_FontAsset), false);
            
            EditorGUILayout.HelpBox(
                "留空则替换所有字体，\n" +
                "指定则只替换匹配的字体。", 
                MessageType.Info);
            
            GUILayout.Space(15);
            
            // 新字体
            EditorGUILayout.LabelField("新字体", EditorStyles.boldLabel);
            newFont = (Font)EditorGUILayout.ObjectField("新 uGUI 字体", newFont, typeof(Font), false);
            newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("新 TMP 字体", newTMPFont, typeof(TMP_FontAsset), false);
            newTMPMaterial = (Material)EditorGUILayout.ObjectField("新 TMP 材质（可选）", newTMPMaterial, typeof(Material), false);
            
            GUILayout.Space(15);
            
            // 替换范围
            EditorGUILayout.LabelField("替换范围", EditorStyles.boldLabel);
            replaceInScene = EditorGUILayout.Toggle("替换场景中的字体", replaceInScene);
            replaceInPrefabs = EditorGUILayout.Toggle("替换预制体中的字体", replaceInPrefabs);
            
            GUILayout.Space(20);
            
            // 执行按钮
            GUI.backgroundColor = new Color(1f, 0.6f, 0.2f);
            if (GUILayout.Button("开始批量替换", GUILayout.Height(50)))
            {
                if (ValidateInputs())
                {
                    StartBatchReplace();
                }
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            
            // 显示结果
            if (replacedTextCount > 0 || replacedTMPCount > 0)
            {
                EditorGUILayout.LabelField("替换结果", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"uGUI Text 替换数量: {replacedTextCount}");
                EditorGUILayout.LabelField($"TMP Text 替换数量: {replacedTMPCount}");
                EditorGUILayout.LabelField($"跳过数量: {skippedCount}");
                
                if (modifiedPrefabPaths.Count > 0)
                {
                    EditorGUILayout.LabelField($"修改的预制体: {modifiedPrefabPaths.Count} 个");
                    if (GUILayout.Button("查看修改的预制体列表"))
                    {
                        ShowModifiedPrefabs();
                    }
                }
            }
        }
        
        private bool ValidateInputs()
        {
            if (newFont == null && newTMPFont == null)
            {
                EditorUtility.DisplayDialog("错误", "请至少指定一种新字体（uGUI 或 TMP）！", "确定");
                return false;
            }
            
            if (!replaceInScene && !replaceInPrefabs)
            {
                EditorUtility.DisplayDialog("错误", "请至少选择一个替换范围（场景或预制体）！", "确定");
                return false;
            }
            
            return true;
        }
        
        private void StartBatchReplace()
        {
            replacedTextCount = 0;
            replacedTMPCount = 0;
            skippedCount = 0;
            modifiedPrefabPaths.Clear();
            
            if (replaceInScene)
            {
                ReplaceInScene();
            }
            
            if (replaceInPrefabs)
            {
                ReplaceInPrefabs();
            }
            
            Debug.Log($"[FontBatchReplacer] 批量替换完成！Text: {replacedTextCount}, TMP: {replacedTMPCount}, 跳过: {skippedCount}");
            EditorUtility.DisplayDialog("完成", 
                $"批量替换完成！\n\n" +
                $"uGUI Text: {replacedTextCount} 个\n" +
                $"TMP Text: {replacedTMPCount} 个\n" +
                $"跳过: {skippedCount} 个\n" +
                $"修改预制体: {modifiedPrefabPaths.Count} 个", 
                "确定");
        }
        
        private void ReplaceInScene()
        {
            Debug.Log("[FontBatchReplacer] 开始替换场景中的字体...");
            
            // 查找所有 Text
            Text[] allTexts = FindObjectsOfType<Text>(true);
            foreach (var text in allTexts)
            {
                if (ShouldReplaceFont(text.font, oldFont))
                {
                    Undo.RecordObject(text, "Replace Font");
                    text.font = newFont;
                    replacedTextCount++;
                    EditorUtility.SetDirty(text);
                }
                else
                {
                    skippedCount++;
                }
            }
            
            // 查找所有 TMP_Text
            TMP_Text[] allTMPs = FindObjectsOfType<TMP_Text>(true);
            foreach (var tmp in allTMPs)
            {
                if (ShouldReplaceTMPFont(tmp.font, oldTMPFont))
                {
                    Undo.RecordObject(tmp, "Replace TMP Font");
                    tmp.font = newTMPFont;
                    if (newTMPMaterial != null)
                    {
                        tmp.material = newTMPMaterial;
                    }
                    replacedTMPCount++;
                    EditorUtility.SetDirty(tmp);
                }
                else
                {
                    skippedCount++;
                }
            }
            
            Debug.Log($"[FontBatchReplacer] 场景替换完成");
        }
        
        private void ReplaceInPrefabs()
        {
            Debug.Log("[FontBatchReplacer] 开始替换预制体中的字体...");
            
            // 查找所有预制体
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
            
            int processedCount = 0;
            int totalCount = prefabGuids.Length;
            
            foreach (string guid in prefabGuids)
            {
                processedCount++;
                if (processedCount % 100 == 0)
                {
                    EditorUtility.DisplayProgressBar("替换预制体字体", 
                        $"处理中... ({processedCount}/{totalCount})", 
                        (float)processedCount / totalCount);
                }
                
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                // 跳过编辑器文件夹
                if (path.Contains("/Editor/")) continue;
                
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;
                
                bool modified = false;
                
                // 处理 Text 组件
                Text[] texts = prefab.GetComponentsInChildren<Text>(true);
                foreach (var text in texts)
                {
                    if (ShouldReplaceFont(text.font, oldFont))
                    {
                        text.font = newFont;
                        replacedTextCount++;
                        modified = true;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
                
                // 处理 TMP_Text 组件
                TMP_Text[] tmps = prefab.GetComponentsInChildren<TMP_Text>(true);
                foreach (var tmp in tmps)
                {
                    if (ShouldReplaceTMPFont(tmp.font, oldTMPFont))
                    {
                        tmp.font = newTMPFont;
                        if (newTMPMaterial != null)
                        {
                            tmp.material = newTMPMaterial;
                        }
                        replacedTMPCount++;
                        modified = true;
                    }
                    else
                    {
                        skippedCount++;
                    }
                }
                
                if (modified)
                {
                    EditorUtility.SetDirty(prefab);
                    modifiedPrefabPaths.Add(path);
                }
            }
            
            EditorUtility.ClearProgressBar();
            
            // 保存所有修改
            if (modifiedPrefabPaths.Count > 0)
            {
                AssetDatabase.SaveAssets();
            }
            
            Debug.Log($"[FontBatchReplacer] 预制体替换完成，修改了 {modifiedPrefabPaths.Count} 个预制体");
        }
        
        private bool ShouldReplaceFont(Font currentFont, Font targetFont)
        {
            // 如果没有指定目标字体，则替换所有
            if (targetFont == null) return true;
            
            // 否则只替换匹配的字体
            return currentFont == targetFont;
        }
        
        private bool ShouldReplaceTMPFont(TMP_FontAsset currentFont, TMP_FontAsset targetFont)
        {
            // 如果没有指定目标字体，则替换所有
            if (targetFont == null) return true;
            
            // 否则只替换匹配的字体
            return currentFont == targetFont;
        }
        
        private void ShowModifiedPrefabs()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("修改的预制体列表：", EditorStyles.boldLabel);
            
            foreach (string path in modifiedPrefabPaths)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(path, EditorStyles.miniLabel);
                if (GUILayout.Button("打开", GUILayout.Width(50)))
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                    {
                        Selection.activeObject = prefab;
                        EditorGUIUtility.PingObject(prefab);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}
