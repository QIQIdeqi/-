using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 代码索引生成器 - 自动生成工程代码文档
    /// </summary>
    public class CodeIndexer : EditorWindow
    {
        private string outputPath = "Assets/Documentation/CodeIndex.md";
        private bool includeEditorScripts = true;
        private bool includePrivateMembers = false;
        private Vector2 scrollPos;
        private string statusMessage = "";
        
        [MenuItem("绒毛几何物语/工具/生成代码索引")]
        public static void ShowWindow()
        {
            GetWindow<CodeIndexer>("代码索引");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("📚 代码索引生成器", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox(
                "生成工程代码的完整索引文档，包含：\n" +
                "• 所有脚本的分类清单\n" +
                "• 核心功能和职责描述\n" +
                "• 类之间的依赖关系\n" +
                "• 单例模式和关键接口\n" +
                "• 冗余代码提示", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            // 设置
            outputPath = EditorGUILayout.TextField("输出路径:", outputPath);
            includeEditorScripts = EditorGUILayout.Toggle("包含Editor脚本", includeEditorScripts);
            includePrivateMembers = EditorGUILayout.Toggle("包含私有成员", includePrivateMembers);
            
            EditorGUILayout.Space();
            
            // 按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成完整索引", GUILayout.Height(40)))
            {
                GenerateIndex();
            }
            if (GUILayout.Button("快速刷新", GUILayout.Height(40)))
            {
                QuickRefresh();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("打开索引文档"))
            {
                OpenIndexDocument();
            }
            
            EditorGUILayout.Space();
            
            // 状态
            if (!string.IsNullOrEmpty(statusMessage))
            {
                EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
            }
        }
        
        private void GenerateIndex()
        {
            GenerateIndexStatic(includeEditorScripts);
        }
        
        /// <summary>
        /// 静态方法供自动刷新调用
        /// </summary>
        public static void GenerateIndexStatic(bool includeEditor = true)
        {
            const string DEFAULT_OUTPUT_PATH = "Assets/Documentation/CodeIndex.md";
            
            try
            {
                var index = new CodeIndexData();
                
                // 扫描所有脚本
                ScanAllScripts(index, includeEditor);
                
                // 分析依赖关系
                AnalyzeDependencies(index);
                
                // 生成Markdown文档
                string markdown = GenerateMarkdown(index);
                
                // 保存文件
                string fullPath = Path.Combine(Application.dataPath, "..", DEFAULT_OUTPUT_PATH);
                string dir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(fullPath, markdown);
                
                AssetDatabase.Refresh();
                
                Debug.Log($"[CodeIndexer] 索引已生成: {fullPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[CodeIndexer] 生成失败: {e}");
            }
        }
        
        private void QuickRefresh()
        {
            // 只更新修改过的文件
            GenerateIndex();
        }
        
        private void OpenIndexDocument()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", outputPath);
            if (File.Exists(fullPath))
            {
                EditorUtility.OpenWithDefaultApp(fullPath);
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "索引文档不存在，请先生成！", "确定");
            }
        }
        
        private static void ScanAllScripts(CodeIndexData index, bool includeEditor = true)
        {
            string scriptsPath = Path.Combine(Application.dataPath, "Scripts");
            if (!Directory.Exists(scriptsPath)) return;
            
            var csFiles = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories);
            
            foreach (var file in csFiles)
            {
                // 跳过Editor文件夹（如果设置）
                if (!includeEditor && file.Contains("\\Editor\\")) continue;
                
                var scriptInfo = ParseScriptFile(file);
                if (scriptInfo != null)
                {
                    index.scripts.Add(scriptInfo);
                }
            }
        }
        
        private static ScriptInfo ParseScriptFile(string filePath)
        {
            string content = File.ReadAllText(filePath);
            string fileName = Path.GetFileName(filePath);
            
            var info = new ScriptInfo
            {
                fileName = fileName,
                filePath = filePath.Replace(Application.dataPath, "Assets"),
                lastModified = File.GetLastWriteTime(filePath)
            };
            
            // 提取命名空间
            var nsMatch = Regex.Match(content, @"namespace\s+(\w+(?:\.\w+)*)");
            if (nsMatch.Success)
            {
                info.nameSpace = nsMatch.Groups[1].Value;
            }
            
            // 提取类名
            var classMatch = Regex.Match(content, @"class\s+(\w+)\s*:\s*(\w+)");
            if (classMatch.Success)
            {
                info.className = classMatch.Groups[1].Value;
                info.baseClass = classMatch.Groups[2].Value;
            }
            else
            {
                classMatch = Regex.Match(content, @"class\s+(\w+)");
                if (classMatch.Success)
                {
                    info.className = classMatch.Groups[1].Value;
                }
            }
            
            // 提取摘要注释
            var summaryMatch = Regex.Match(content, @"///\s*<summary>\s*///\s*(.+?)\s*///\s*</summary>");
            if (summaryMatch.Success)
            {
                info.description = summaryMatch.Groups[1].Value.Trim();
            }
            
            // 检测单例模式
            if (content.Contains("public static") && content.Contains("Instance"))
            {
                info.isSingleton = true;
            }
            
            // 检测MonoBehaviour
            if (content.Contains(": MonoBehaviour") || info.baseClass == "MonoBehaviour")
            {
                info.isMonoBehaviour = true;
            }
            
            // 提取公共方法
            var methodMatches = Regex.Matches(content, @"public\s+\w+\s+(\w+)\s*\(");
            foreach (Match match in methodMatches)
            {
                info.publicMethods.Add(match.Groups[1].Value);
            }
            
            // 提取SerializedField
            var fieldMatches = Regex.Matches(content, @"\[SerializeField\]\s*\w+\s+(\w+)");
            foreach (Match match in fieldMatches)
            {
                info.serializedFields.Add(match.Groups[1].Value);
            }
            
            // 检测分类
            if (filePath.Contains("\\Editor\\"))
            {
                info.category = "Editor工具";
            }
            else if (filePath.Contains("\\Manager\\"))
            {
                info.category = "管理器";
            }
            else if (filePath.Contains("\\UI\\"))
            {
                info.category = "UI系统";
            }
            else if (filePath.Contains("\\Home\\"))
            {
                info.category = "家园系统";
            }
            else if (filePath.Contains("\\Data\\"))
            {
                info.category = "数据定义";
            }
            else if (filePath.Contains("\\Weapon\\"))
            {
                info.category = "武器系统";
            }
            else if (filePath.Contains("\\Enemy\\"))
            {
                info.category = "敌人系统";
            }
            else if (filePath.Contains("\\Player\\"))
            {
                info.category = "玩家系统";
            }
            else
            {
                info.category = "其他";
            }
            
            return info;
        }
        
        private static void AnalyzeDependencies(CodeIndexData index)
        {
            foreach (var script in index.scripts)
            {
                // 查找被哪些脚本引用
                foreach (var other in index.scripts)
                {
                    if (script == other) continue;
                    
                    string content = File.ReadAllText(other.filePath);
                    if (content.Contains(script.className))
                    {
                        script.referencedBy.Add(other.className);
                    }
                }
            }
        }
        
        private static string GenerateMarkdown(CodeIndexData index)
        {
            var sb = new System.Text.StringBuilder();
            
            // 标题
            sb.AppendLine("# 绒毛几何物语 - 代码索引");
            sb.AppendLine($"> 生成时间: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"> 脚本总数: {index.scripts.Count}");
            sb.AppendLine();
            
            // 目录
            sb.AppendLine("## 📑 目录");
            sb.AppendLine("- [核心系统概览](#核心系统概览)");
            sb.AppendLine("- [单例模式清单](#单例模式清单)");
            sb.AppendLine("- [按分类索引](#按分类索引)");
            sb.AppendLine("- [完整脚本清单](#完整脚本清单)");
            sb.AppendLine("- [冗余代码提示](#冗余代码提示)");
            sb.AppendLine();
            
            // 核心系统概览
            sb.AppendLine("## 核心系统概览");
            sb.AppendLine();
            sb.AppendLine("| 系统 | 核心脚本 | 职责 |");
            sb.AppendLine("|------|----------|------|");
            
            var coreSystems = new Dictionary<string, string[]>
            {
                {"游戏管理", new[] {"GameManager"}},
                {"家园系统", new[] {"HomeManager", "HomeNPC", "HomeDoor"}},
                {"换装系统", new[] {"OutfitManager", "OutfitPanelNew", "PlayerOutfitApplier"}},
                {"背包系统", new[] {"BackpackPanel", "BackpackButton"}},
                {"武器系统", new[] {"WeaponManager", "WeaponBase"}},
                {"升级系统", new[] {"UpgradeManager", "UpgradeUI"}},
                {"敌人系统", new[] {"EnemySpawner", "EnemyBase"}},
            };
            
            foreach (var system in coreSystems)
            {
                var scripts = index.scripts.Where(s => system.Value.Contains(s.className));
                string scriptList = string.Join(", ", scripts.Select(s => $"`{s.className}`"));
                sb.AppendLine($"| {system.Key} | {scriptList} | - |");
            }
            sb.AppendLine();
            
            // 单例模式清单
            sb.AppendLine("## 单例模式清单");
            sb.AppendLine();
            var singletons = index.scripts.Where(s => s.isSingleton).ToList();
            foreach (var s in singletons)
            {
                sb.AppendLine($"- `{s.className}` - {s.description}");
            }
            sb.AppendLine();
            
            // 按分类索引
            sb.AppendLine("## 按分类索引");
            sb.AppendLine();
            var categories = index.scripts.GroupBy(s => s.category).OrderBy(g => g.Key);
            foreach (var category in categories)
            {
                sb.AppendLine($"### {category.Key} ({category.Count()})");
                sb.AppendLine();
                foreach (var script in category.OrderBy(s => s.className))
                {
                    sb.AppendLine($"- `{script.className}` - {script.description}");
                }
                sb.AppendLine();
            }
            
            // 完整脚本清单
            sb.AppendLine("## 完整脚本清单");
            sb.AppendLine();
            foreach (var script in index.scripts.OrderBy(s => s.className))
            {
                sb.AppendLine($"### {script.className}");
                sb.AppendLine($"- **文件**: `{script.filePath}`");
                sb.AppendLine($"- **命名空间**: {script.nameSpace}");
                sb.AppendLine($"- **描述**: {script.description}");
                sb.AppendLine($"- **单例**: {(script.isSingleton ? "是" : "否")}");
                sb.AppendLine($"- **MonoBehaviour**: {(script.isMonoBehaviour ? "是" : "否")}");
                
                if (script.publicMethods.Any())
                {
                    sb.AppendLine($"- **公共方法**: {string.Join(", ", script.publicMethods.Take(5))}{(script.publicMethods.Count > 5 ? "..." : "")}");
                }
                
                if (script.referencedBy.Any())
                {
                    sb.AppendLine($"- **被引用**: {string.Join(", ", script.referencedBy.Take(5))}{(script.referencedBy.Count > 5 ? "..." : "")}");
                }
                
                sb.AppendLine();
            }
            
            // 冗余代码提示
            sb.AppendLine("## 冗余代码提示");
            sb.AppendLine();
            
            // 检测重复命名空间
            var nsGroups = index.scripts.Where(s => !string.IsNullOrEmpty(s.nameSpace)).GroupBy(s => s.nameSpace);
            foreach (var ns in nsGroups.Where(g => g.Count() > 5))
            {
                sb.AppendLine($"> **提示**: 命名空间 `{ns.Key}` 包含 {ns.Count()} 个类，建议拆分");
            }
            
            // 检测空描述
            var noDesc = index.scripts.Where(s => string.IsNullOrEmpty(s.description));
            if (noDesc.Any())
            {
                sb.AppendLine($"> **提示**: 以下 {noDesc.Count()} 个脚本缺少描述注释：");
                foreach (var s in noDesc.Take(10))
                {
                    sb.AppendLine($"> - {s.className}");
                }
                if (noDesc.Count() > 10) sb.AppendLine($"> - ... 还有 {noDesc.Count() - 10} 个");
            }
            
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine("*此文档由 CodeIndexer 自动生成*");
            
            return sb.ToString();
        }
    }
    
    // 数据类
    public class CodeIndexData
    {
        public List<ScriptInfo> scripts = new List<ScriptInfo>();
    }
    
    public class ScriptInfo
    {
        public string fileName;
        public string filePath;
        public string className;
        public string nameSpace;
        public string baseClass;
        public string description;
        public string category;
        public bool isSingleton;
        public bool isMonoBehaviour;
        public List<string> publicMethods = new List<string>();
        public List<string> serializedFields = new List<string>();
        public List<string> referencedBy = new List<string>();
        public System.DateTime lastModified;
    }
}
