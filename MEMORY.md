# MEMORY.md - OpenClaw Long-term Memory

## About This File
This file contains curated long-term memory for the OpenClaw session. It persists across sessions and should be updated with important information, decisions, and context.

## User Profile

### Identity
- **Name**: (Not yet set)
- **Pronouns**: (Not yet set)
- **Timezone**: Asia/Shanghai (GMT+8)

### Projects

#### 绒毛几何物语 (Fluffy Geometry Tales) - 转型完成
- **曾用名**: 几何战士 / 肉鸽割草
- **Type**: Indie Roguelike Bullet-hell Game (转型为治愈收集)
- **Platform**: WeChat Mini Game (微信小游戏)
- **Engine**: Unity 2022.3.61f1 (团结引擎/Tuanjie for WeChat)
- **Status**: 全面转型完成，文档齐全，等待美术资产
- **转型日期**: 2026-02-23 (重要里程碑)

**Project Paths**:
- Unity Project: `D:\03_独立游戏\自己\微信小游戏\01_项目\项目01_肉鸽割草\04_Unity工程\肉鸽割草`
- Export Path: `D:\03_独立游戏\自己\微信小游戏\01_项目\项目01_肉鸽割草\05_导出\微信小游戏`
- Documentation: `D:\03_独立游戏\自己\微信小游戏\01_项目\项目01_肉鸽割草\01_文档`
- GitHub: https://github.com/QIQIdeqi/项目01_肉鸽割草

**Core Transformation**:
| Aspect | Before | After |
|--------|--------|-------|
| 美术风格 | 霓虹赛博 | 羊毛毡手作 |
| 目标用户 | 泛用户 | 18-35岁女性 |
| 核心体验 | 战斗爽快感 | 收集治愈感 |
| 世界观 | 几何战士战斗 | 编织精灵安抚 |
| 色彩 | 霓虹高饱和 | 马卡龙pastel |

**Key Features Documented**:
- 编织精灵角色系统（换装/皮肤）
- 安抚型互动对象（毛线团怪、脱线小兵、纽扣眼魔偶）
- 家园系统（手作小屋）
- 收集系统（图鉴/材料）
- 完整AI美术资产生成规范

**Next Priority**: 使用AI生成第一批核心美术资产

### Preferences
- **Communication**: 飞书 (Feishu)
- **Documentation**: Markdown files in project directory
- **Version Control**: Git (GitHub)
- **美术资源管理**: AI生成 + 本地PSD不提交Git

## Session History

### 2026-02-23 - 重大转型日 ⭐
**关键词**: 项目转型、美术重构、文档完善、Git维护

**Major Accomplishments**:
1. 项目全面转型：几何战士→绒毛几何物语
2. 完成所有设计文档重写（世界观、机制、美术规范）
3. 创建完整的AI美术资产生成提示词库
4. 修复Shader翻转问题（暂时禁用flipX）
5. 解决Git大文件问题（添加.gitignore）
6. 创建UI设计文档（Logo、血条、暂停按钮、虚拟摇杆）

**关键决策**:
- 保留代码框架，仅做表现层适配
- 所有美术资产统一使用羊毛毡手作风
- PSD源文件不提交Git，使用云盘备份

**完整记录**: `memory/2026-02-23.md` 和 `开发日志_2026-02-23.md`

### 2026-02-22 - Unity开发马拉松
**关键词**: 团结引擎迁移、皮肤系统、暂停功能、WeChat导出
- 修复迁移问题
- 实现完整皮肤系统
- 添加暂停功能
- 解决微信小游戏导出问题

## Technical Notes

### Unity项目配置
- **Render Pipeline**: Built-in (暂不更换)
- **Shader**: 自定义Shader Graph（需修复flipX问题）
- **Input**: 虚拟摇杆 (Joystick)
- **UI**: uGUI + TextMeshPro

### Git配置
- **忽略文件**: PSD, AI, 导出文件, Library/Temp
- **大文件处理**: 使用.gitignore + 本地备份
- **提交规范**: 中文描述，功能分组

### 美术资产工作流
1. 使用AI生成（Midjourney/DALL-E）
2. 本地PSD编辑（不提交Git）
3. 导出PNG到Unity
4. 九宫格切割（如需）
5. 导入测试

## Ongoing Tasks

### 当前进行中的工作
- 绒毛几何物语美术资产生成（AI）
- 第一批核心素材：精灵、敌人、地面贴图

### 待开始
- Shader翻转问题彻底解决
- 家园系统开发
- 图鉴系统开发

## Notes
- (Add important decisions, lessons learned, etc.)
