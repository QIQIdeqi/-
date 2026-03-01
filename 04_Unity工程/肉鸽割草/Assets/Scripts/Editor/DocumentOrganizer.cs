using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 文档规整工具 - 一键整理项目文档目录
/// </summary>
public class DocumentOrganizer : EditorWindow
{
    private string docPath;
    private bool showDetails = true;
    private Vector2 scrollPos;
    private List<string> logMessages = new List<string>();
    
    [MenuItem("绒毛几何物语/工具/文档规整")]
    public static void ShowWindow()
    {
        GetWindow<DocumentOrganizer>("文档规整");
    }
    
    private void OnEnable()
    {
        docPath = Path.Combine(Application.dataPath, "../01_文档");
        docPath = Path.GetFullPath(docPath);
    }
    
    private void OnGUI()
    {
        GUILayout.Label("📁 文档规整工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("文档路径:", docPath);
        EditorGUILayout.Space();
        
        showDetails = EditorGUILayout.Toggle("显示详细日志", showDetails);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("开始规整文档", GUILayout.Height(40)))
        {
            OrganizeDocuments();
        }
        
        if (GUILayout.Button("仅创建目录结构", GUILayout.Height(30)))
        {
            CreateDirectoryStructure();
        }
        
        EditorGUILayout.Space();
        
        if (showDetails && logMessages.Count > 0)
        {
            GUILayout.Label("操作日志:", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            foreach (var msg in logMessages)
            {
                EditorGUILayout.LabelField(msg, EditorStyles.wordWrappedLabel);
            }
            EditorGUILayout.EndScrollView();
        }
    }
    
    private void Log(string message)
    {
        logMessages.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        Debug.Log($"[DocumentOrganizer] {message}");
    }
    
    private void OrganizeDocuments()
    {
        logMessages.Clear();
        
        if (!Directory.Exists(docPath))
        {
            Log($"❌ 文档目录不存在: {docPath}");
            return;
        }
        
        Log("开始文档规整...");
        
        try
        {
            // 1. 创建目录结构
            CreateDirectoryStructure();
            
            // 2. 移动/整理文件
            MoveFiles();
            
            // 3. 创建README.md
            CreateReadme();
            
            Log("✅ 文档规整完成！");
            EditorUtility.DisplayDialog("完成", "文档规整完成！请检查新的目录结构。", "确定");
        }
        catch (Exception e)
        {
            Log($"❌ 错误: {e.Message}");
            Debug.LogException(e);
        }
    }
    
    private void CreateDirectoryStructure()
    {
        Log("创建目录结构...");
        
        string[] directories = new string[]
        {
            "📋 项目总览",
            "🎨 美术资产/角色设计",
            "🎨 美术资产/场景元素",
            "🎮 系统设计/核心玩法",
            "🎮 系统设计/家园系统",
            "🎮 系统设计/UI系统",
            "💻 技术文档/项目配置",
            "💻 技术文档/Shader",
            "📝 参考资源",
            "📁 归档"
        };
        
        foreach (var dir in directories)
        {
            string fullPath = Path.Combine(docPath, dir);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                Log($"  📁 创建: {dir}");
            }
        }
    }
    
    private void MoveFiles()
    {
        Log("整理文件...");
        
        // 定义文件映射规则
        var fileMappings = new Dictionary<string, string[]>
        {
            // 项目总览
            ["📋 项目总览"] = new string[]
            {
                "绒毛几何物语 - 游戏设计文档.md",
                "游戏设计文档.md",
                "开发日志.md",
                "开发日志_*.md"
            },
            
            // 美术资产
            ["🎨 美术资产"] = new string[]
            {
                "AI提示词.md",
                "AI提示词_家园场景UI.md",
                "第一批素材提示词.md",
                "Logo设计提示词.md"
            },
            
            // 美术资产 - 场景元素
            ["🎨 美术资产/场景元素"] = new string[]
            {
                "地板Tile_Prompt.md",
                "两面墙_Prompt.md",
                "家居等距视角_Prompts.md"
            },
            
            // 家园系统
            ["🎮 系统设计/家园系统"] = new string[]
            {
                "家园系统配置指南.md",
                "家园场景配置步骤_色块占位版.md",
                "换装系统预研与配置指南.md",
                "背包系统配置指南.md"
            },
            
            // UI系统
            ["🎮 系统设计/UI系统"] = new string[]
            {
                "UI_图标设计规范.md",
                "HUD血条设计规范与提示词.md",
                "虚拟摇杆设计提示词.md",
                "暂停按钮设计提示词.md",
                "UI_家园设计规范.md"
            },
            
            // 技术文档
            ["💻 技术文档/项目配置"] = new string[]
            {
                "微信素材规范说明.md"
            },
            
            ["💻 技术文档/Shader"] = new string[]
            {
                "Shader_UI_AdditiveOverlay_使用说明.md"
            },
            
            // 参考资源
            ["📝 参考资源"] = new string[]
            {
                "小红书文案.md",
                "小红书文案v2.md"
            },
            
            // 归档
            ["📁 归档"] = new string[]
            {
                "开发日志_2026-02-18.md",
                "开发日志_2026-02-18_晚间.md",
                "开发日志_2026-02-19.md",
                "开发日志_2026-02-20.md",
                "开发日志_2026-02-23.md"
            }
        };
        
        // 获取所有文件
        var allFiles = Directory.GetFiles(docPath, "*.md", SearchOption.TopDirectoryOnly);
        
        foreach (var file in allFiles)
        {
            string fileName = Path.GetFileName(file);
            bool moved = false;
            
            foreach (var mapping in fileMappings)
            {
                string targetDir = mapping.Key;
                string[] patterns = mapping.Value;
                
                foreach (var pattern in patterns)
                {
                    if (MatchesPattern(fileName, pattern))
                    {
                        string targetPath = Path.Combine(docPath, targetDir, fileName);
                        
                        // 如果目标已存在，添加序号
                        if (File.Exists(targetPath))
                        {
                            string dir = Path.GetDirectoryName(targetPath);
                            string name = Path.GetFileNameWithoutExtension(fileName);
                            string ext = Path.GetExtension(fileName);
                            targetPath = Path.Combine(dir, $"{name}_新{ext}");
                        }
                        
                        File.Move(file, targetPath);
                        Log($"  📄 移动: {fileName} → {targetDir}/");
                        moved = true;
                        break;
                    }
                }
                
                if (moved) break;
            }
            
            if (!moved)
            {
                Log($"  ⚠️ 未分类: {fileName}");
            }
        }
    }
    
    private bool MatchesPattern(string fileName, string pattern)
    {
        // 简单通配符匹配
        if (pattern.Contains("*"))
        {
            string regex = "^" + pattern.Replace("*", ".*").Replace("?", ".") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(fileName, regex);
        }
        return fileName.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }
    
    private void CreateReadme()
    {
        Log("创建 README.md...");
        
        string readmeContent = @"# 绒毛几何物语 - 文档中心

> 📅 最后更新：2026-03-01  
> 🎮 项目类型：微信小游戏（治愈系收集养成）  
> 🎨 美术风格：羊毛毡手作 + 马卡龙pastel色系

---

## 📋 项目总览

| 文档 | 说明 |
|------|------|
| [游戏设计文档](./项目总览/游戏设计文档.md) | 核心玩法、世界观、目标用户、游戏循环 |
| [项目开发日志](./项目总览/项目开发日志.md) | 每日开发记录（合并） |
| [文档规整方案](./项目总览/文档规整方案.md) | 本文档目录结构说明 |

---

## 🎨 美术资产

### 风格指南
- [AI提示词总库](./美术资产/AI提示词.md) - 所有AI生成提示词汇总
- [Logo设计](./美术资产/Logo设计提示词.md)

### 场景元素
- [地板Tile](./美术资产/场景元素/地板Tile_Prompt.md)
- [两面墙](./美术资产/场景元素/两面墙_Prompt.md)
- [家居等距视角](./美术资产/场景元素/家居等距视角_Prompts.md)

### 角色设计
- （待添加）主角设计规范
- （待添加）NPC设计规范

---

## 🎮 系统设计

### 家园系统
| 文档 | 说明 |
|------|------|
| [家园系统配置指南](./家园系统/家园系统配置指南.md) | 完整配置步骤 |
| [家园场景配置步骤（色块占位版）](./家园系统/家园场景配置步骤_色块占位版.md) | 无美术资源时的开发配置 |
| [换装系统详解](./家园系统/换装系统预研与配置指南.md) | 分部件换装机制 |
| [背包系统配置](./家园系统/背包系统配置指南.md) | 背包数据配置 |

### UI系统
| 文档 | 说明 |
|------|------|
| [UI图标设计规范](./UI系统/UI_图标设计规范.md) | 图标风格、尺寸规范 |
| [HUD血条设计](./UI系统/HUD血条设计规范与提示词.md) | 血条UI设计规范 |
| [虚拟摇杆设计](./UI系统/虚拟摇杆设计提示词.md) | 摇杆设计规范 |
| [暂停按钮设计](./UI系统/暂停按钮设计提示词.md) | 暂停按钮规范 |
| [家园界面设计](./UI系统/UI_家园设计规范.md) | 家园场景UI设计（新增） |

### 核心玩法
- （待添加）战斗系统
- （待添加）升级系统
- （待添加）敌人生成系统

---

## 💻 技术文档

### 项目配置
- [微信素材规范](./项目配置/微信素材规范说明.md)

### Shader
- [UI叠加Shader使用说明](./Shader/Shader_UI_AdditiveOverlay_使用说明.md)

---

## 📝 参考资源

- [小红书文案](./参考资源/小红书文案.md)
- [小红书文案v2](./参考资源/小红书文案v2.md)

---

## 📁 归档

历史开发日志（已合并到主日志）：
- 开发日志_2026-02-18.md
- 开发日志_2026-02-19.md
- 开发日志_2026-02-20.md
- 开发日志_2026-02-23.md

---

## 🔗 项目链接

- **Unity工程**: `04_Unity工程/肉鸽割草/`
- **美术资源**: `02_美术资源/`
- **导出目录**: `05_导出/微信小游戏/`
- **GitHub**: https://github.com/QIQIdeqi/项目01_肉鸽割草

---

## 📝 维护指南

1. **新增文档**时，放入对应分类目录
2. **命名规范**: 中文命名，使用 `-` 分隔，如 `家园系统-配置指南.md`
3. **定期归档**: 过时文档移至 `📁 归档/` 目录
4. **更新索引**: 修改本文档时同步更新此 README

---

*文档中心由 UGUI专家 自动生成*  
*生成时间：2026-03-01*
";
        
        string readmePath = Path.Combine(docPath, "README.md");
        File.WriteAllText(readmePath, readmeContent, System.Text.Encoding.UTF8);
        Log($"  ✅ 创建: README.md");
    }
}
