#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
文档规整工具 - 绒毛几何物语
自动整理项目文档目录
"""

import os
import shutil
from pathlib import Path

# 配置
DOC_PATH = Path(r"D:\03_独立游戏\自己\微信小游戏\01_项目\项目01_肉鸽割草\01_文档")

# 文件映射规则
FILE_MAPPINGS = {
    "📋 项目总览": [
        "绒毛几何物语 - 游戏设计文档.md",
        "游戏设计文档.md",
        "文档规整方案.md",
        "整理步骤.md",
    ],
    "🎨 美术资产": [
        "AI提示词.md",
        "AI提示词_家园背包UI.md",
        "第一批素材提示词.md",
        "Logo设计提示词.md",
        "HUD血条经验条设计提示词.md",
        "毛线团怪死亡造型提示词.md",
    ],
    "🎨 美术资产/场景元素": [
        "地板Tile_Prompt.md",
        "两面墙_Prompt.md",
        "家具等距视角_Prompts.md",
    ],
    "🎮 系统设计/家园系统": [
        "家园系统配置指南.md",
        "家园场景配置步骤_色块占位版.md",
        "新背包系统配置指南.md",
        "背包系统预制体配置指南.md",
        "组件制作步骤.md",
        "UI_家园背包设计.md",
    ],
    "🎮 系统设计/UI系统": [
        "虚拟摇杆设计提示词.md",
        "暂停按钮设计提示词.md",
    ],
    "💻 技术文档/项目配置": [
        "微信插件使用说明.md",
    ],
    "💻 技术文档/Shader": [
        "Shader_UI_AdditiveOverlay_使用说明.md",
    ],
    "📝 参考资源": [
        "小红书文案.md",
    ],
    "📁 归档": [
        "开发日志.md",
        "开发日志_2026-02-18.md",
        "开发日志_2026-02-18_完整.md",
        "开发日志_2026-02-19.md",
        "开发日志_2026-02-20.md",
        "开发日志_2026-02-23.md",
        "毛线团敌人设置说明.md",
    ],
}

def create_directories():
    """创建目录结构"""
    print("📁 创建目录结构...")
    for folder in FILE_MAPPINGS.keys():
        folder_path = DOC_PATH / folder
        folder_path.mkdir(parents=True, exist_ok=True)
        print(f"  ✓ {folder}")
    print()

def move_files():
    """移动文件"""
    print("📄 移动文件...")
    moved_count = 0
    not_found = []
    
    for folder, files in FILE_MAPPINGS.items():
        for filename in files:
            src = DOC_PATH / filename
            dst = DOC_PATH / folder / filename
            
            if src.exists():
                # 如果目标已存在，添加序号
                if dst.exists():
                    stem = dst.stem
                    suffix = dst.suffix
                    counter = 1
                    while dst.exists():
                        dst = DOC_PATH / folder / f"{stem}_{counter}{suffix}"
                        counter += 1
                
                shutil.move(str(src), str(dst))
                print(f"  ✓ {filename} → {folder}/")
                moved_count += 1
            else:
                not_found.append(filename)
    
    print(f"\n  共移动 {moved_count} 个文件")
    if not_found:
        print(f"  未找到: {', '.join(not_found)}")
    print()

def create_readme():
    """创建 README.md"""
    print("📝 创建 README.md...")
    
    readme_content = """# 绒毛几何物语 - 文档中心

> 📅 最后更新：2026-03-01  
> 🎮 项目类型：微信小游戏（治愈系收集养成）  
> 🎨 美术风格：羊毛毡手作 + 马卡龙pastel色系

---

## 📋 项目总览

| 文档 | 说明 |
|------|------|
| [游戏设计文档](./项目总览/游戏设计文档.md) | 核心玩法、世界观、目标用户 |
| [文档规整方案](./项目总览/文档规整方案.md) | 文档目录结构说明 |

---

## 🎨 美术资产

- [AI提示词总库](./美术资产/AI提示词.md)
- [Logo设计](./美术资产/Logo设计提示词.md)

### 场景元素
- [地板Tile](./美术资产/场景元素/地板Tile_Prompt.md)
- [两面墙](./美术资产/场景元素/两面墙_Prompt.md)
- [家居等距视角](./美术资产/场景元素/家具等距视角_Prompts.md)

---

## 🎮 系统设计

### 家园系统
- [家园系统配置指南](./家园系统/家园系统配置指南.md)
- [家园场景配置步骤（色块占位版）](./家园系统/家园场景配置步骤_色块占位版.md)
- [背包系统配置](./家园系统/新背包系统配置指南.md)

### UI系统
- [虚拟摇杆设计](./UI系统/虚拟摇杆设计提示词.md)
- [暂停按钮设计](./UI系统/暂停按钮设计提示词.md)

---

## 💻 技术文档

- [微信插件使用说明](./项目配置/微信插件使用说明.md)
- [UI叠加Shader使用说明](./Shader/Shader_UI_AdditiveOverlay_使用说明.md)

---

## 📝 参考资源

- [小红书文案](./参考资源/小红书文案.md)

---

## 📁 归档

历史开发日志和旧文档

---

*文档中心自动生成 - 2026-03-01*
"""
    
    readme_path = DOC_PATH / "README.md"
    with open(readme_path, "w", encoding="utf-8") as f:
        f.write(readme_content)
    print("  ✓ README.md 创建完成")
    print()

def main():
    """主函数"""
    print("=" * 50)
    print("  绒毛几何物语 - 文档规整工具")
    print("=" * 50)
    print()
    
    if not DOC_PATH.exists():
        print(f"❌ 文档路径不存在: {DOC_PATH}")
        return
    
    print(f"文档路径: {DOC_PATH}")
    print()
    
    create_directories()
    move_files()
    create_readme()
    
    print("=" * 50)
    print("  ✅ 文档规整完成！")
    print("=" * 50)
    print()
    print("新目录结构:")
    for folder in FILE_MAPPINGS.keys():
        print(f"  📁 {folder}")

if __name__ == "__main__":
    main()
    input("\n按回车键退出...")
