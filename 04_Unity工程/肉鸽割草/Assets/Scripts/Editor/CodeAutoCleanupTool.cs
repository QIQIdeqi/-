using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 代码自动清理工具 - 安全删除冗余脚本
    /// </summary>
    public class CodeAutoCleanupTool : EditorWindow
    {
        private Vector2 scrollPos;
        private List<CleanupItem> items = new List<CleanupItem>();
        private bool showDetails = true;
        private string backupPath;
        private bool createBackup = true;
        
        private class CleanupItem
        {
            public string fileName;
            public string path;
            public string reason;
            public bool selected = true;
            public bool deleted = false;
            public string status = "";
        }
        
        [MenuItem("绒毛几何物语/工具/自动清理冗余代码")]
        public static void ShowWindow()
        {
            GetWindow<CodeAutoCleanupTool>("自动清理代码");
        }
        
        private void OnEnable()
        {
            ScanRedundantFiles();
            backupPath = Path.Combine(Application.dataPath, "..", "ScriptBackup_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🧹 代码自动清理工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 备份选项
            createBackup = EditorGUILayout.Toggle("删除前创建备份", createBackup);
            if (createBackup)
            {
                EditorGUILayout.LabelField($"备份路径: {backupPath}");
            }
            
            EditorGUILayout.Space();
            
            // 统计信息
            int totalCount = items.Count;
            int selectedCount = items.Count(i => i.selected && !i.deleted);
            int deletedCount = items.Count(i => i.deleted);
            
            EditorGUILayout.LabelField($"发现冗余文件: {totalCount} | 待删除: {selectedCount} | 已删除: {deletedCount}", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            // 按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("全选", GUILayout.Width(80)))
            {
                foreach (var item in items) if (!item.deleted) item.selected = true;
            }
            if (GUILayout.Button("全不选", GUILayout.Width(80)))
            {
                foreach (var item in items) if (!item.deleted) item.selected = false;
            }
            if (GUILayout.Button("刷新列表", GUILayout.Width(100)))
            {
                ScanRedundantFiles();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 文件列表
            showDetails = EditorGUILayout.Toggle("显示详情", showDetails);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(400));
            
            foreach (var item in items)
            {
                if (item.deleted) GUI.enabled = false;
                
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                
                item.selected = EditorGUILayout.Toggle(item.selected, GUILayout.Width(20));
                
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(item.fileName, EditorStyles.boldLabel);
                if (showDetails)
                {
                    EditorGUILayout.LabelField($"原因: {item.reason}", EditorStyles.miniLabel);
                    EditorGUILayout.LabelField($"路径: {item.path}", EditorStyles.miniLabel);
                }
                if (!string.IsNullOrEmpty(item.status))
                {
                    EditorGUILayout.LabelField($"状态: {item.status}", EditorStyles.miniLabel);
                }
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
                
                GUI.enabled = true;
            }
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space();
            
            // 清理按钮
            GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
            if (GUILayout.Button($"开始清理 (删除 {selectedCount} 个文件)", GUILayout.Height(50)))
            {
                if (selectedCount == 0)
                {
                    EditorUtility.DisplayDialog("提示", "没有选择要删除的文件！", "确定");
                }
                else if (EditorUtility.DisplayDialog("确认清理",
                    $"即将删除 {selectedCount} 个脚本文件！\n\n" +
                    (createBackup ? $"已启用备份，备份路径:\n{backupPath}\n\n" : "警告：未启用备份！\n\n") +
                    "此操作不可撤销，是否继续？",
                    "确认删除", "取消"))
                {
                    StartCleanup();
                }
            }
            GUI.backgroundColor = Color.white;
        }
        
        private void ScanRedundantFiles()
        {
            items.Clear();
            string scriptsPath = Path.Combine(Application.dataPath, "Scripts");
            
            // 定义要删除的文件列表（根据分析报告）
            var redundantFiles = new Dictionary<string, string>
            {
                // 旧版 Skin 系统
                {"SkinData.cs", "已弃用，使用 OutfitPartData 替代"},
                {"SkinManager.cs", "已弃用，使用 OutfitManager 替代"},
                {"SkinPanel.cs", "已弃用，使用 OutfitPanelNew 替代"},
                {"SkinItemUI.cs", "已弃用，使用 OutfitItemUI 替代"},
                {"SkinConfigCreator.cs", "已弃用"},
                {"SkinConfigEditor.cs", "已弃用"},
                
                // 重复的 Outfit 工具
                {"OutfitPartQuickCreator.cs", "重复功能，合并到 OutfitPartCreator"},
                {"OutfitPartsEmergencyFix.cs", "临时修复脚本"},
                {"OutfitItemAutoSetup.cs", "临时自动配置脚本"},
                {"OutfitItemUIPrefabFixer.cs", "临时修复脚本"},
                {"OutfitManagerDataFiller.cs", "临时数据填充脚本"},
                {"OutfitManagerSceneChecker.cs", "临时检查脚本"},
                {"OutfitPanelSetupWizard.cs", "旧版工具，使用 UIBuilderTool 替代"},
                
                // 重复的 Backpack 工具
                {"BackpackPanelRebuilder.cs", "临时重建脚本"},
                {"BackpackPanelAutoSetup.cs", "临时自动配置脚本"},
                {"BackpackButtonAutoSetup.cs", "临时自动配置脚本"},
                {"BackpackSystemMenu.cs", "重复功能"},
                
                // 重复的 Furniture 工具
                {"FurnitureItemAutoSetup.cs", "临时自动配置脚本"},
                {"FurnitureEditControllerAutoSetup.cs", "临时自动配置脚本"},
                
                // 其他临时工具
                {"ProjectSetupWizard.cs", "早期项目设置工具"},
                {"ProjectDiagnostics.cs", "临时诊断脚本"},
                {"FontBatchReplacer.cs", "重复功能，使用 UIFontFixer"},
                {"TestUpgradeUI.cs", "测试脚本"},
                {"SetupHelper.cs", "早期辅助工具"},
                {"HomeManagerSetupTool.cs", "早期设置工具"},
                {"QuickUIElementAdder.cs", "临时添加工具"},
                {"UITextFontFixer.cs", "重复功能，使用 UIFontFixer"},
            };
            
            // 扫描文件
            foreach (var kvp in redundantFiles)
            {
                string[] guids = AssetDatabase.FindAssets($"t:Script {kvp.Key.Replace(".cs", "")}");
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (Path.GetFileName(path) == kvp.Key)
                    {
                        items.Add(new CleanupItem
                        {
                            fileName = kvp.Key,
                            path = path,
                            reason = kvp.Value,
                            selected = true
                        });
                    }
                }
            }
            
            // 按文件名排序
            items = items.OrderBy(i => i.fileName).ToList();
        }
        
        private void StartCleanup()
        {
            int successCount = 0;
            int failCount = 0;
            
            // 创建备份
            if (createBackup)
            {
                Directory.CreateDirectory(backupPath);
            }
            
            AssetDatabase.StartAssetEditing();
            
            try
            {
                foreach (var item in items)
                {
                    if (!item.selected || item.deleted) continue;
                    
                    string fullPath = Path.Combine(Application.dataPath, "..", item.path);
                    
                    try
                    {
                        // 备份
                        if (createBackup)
                        {
                            string backupFile = Path.Combine(backupPath, item.fileName);
                            File.Copy(fullPath, backupFile, true);
                        }
                        
                        // 删除
                        AssetDatabase.DeleteAsset(item.path);
                        
                        item.deleted = true;
                        item.status = "✅ 已删除";
                        successCount++;
                        
                        Debug.Log($"[CodeAutoCleanup] 已删除: {item.fileName}");
                    }
                    catch (System.Exception e)
                    {
                        item.status = $"❌ 失败: {e.Message}";
                        failCount++;
                        Debug.LogError($"[CodeAutoCleanup] 删除失败 {item.fileName}: {e.Message}");
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
            
            // 编译检查
            if (failCount == 0)
            {
                EditorUtility.DisplayDialog("清理完成",
                    $"成功删除 {successCount} 个文件！\n\n" +
                    (createBackup ? $"备份已保存到:\n{backupPath}" : ""),
                    "确定");
                
                // 强制编译
                AssetDatabase.Refresh();
                EditorUtility.RequestScriptReload();
            }
            else
            {
                EditorUtility.DisplayDialog("清理完成（有错误）",
                    $"成功: {successCount} | 失败: {failCount}\n\n" +
                    "请检查控制台日志了解详情。",
                    "确定");
            }
        }
    }
}
