# 项目01_几何战士 (Geometry Warrior)

## 📁 目录说明

```
项目01_肉鸽割草/
├── 01_文档/          # 策划文档、GDD、提示词、插件说明
├── 02_美术/          # 美术资源（Sprites、图集）
├── 03_音效/          # 音效、BGM
├── 04_Unity工程/     # Unity项目目录
│   └── 肉鸽割草/     # 实际工程文件夹
│       ├── Assets/
│       │   ├── Scripts/  # 代码
│       │   │   ├── Player/    # 玩家相关
│       │   │   ├── Enemy/     # 敌人相关
│       │   │   ├── Manager/   # 管理器
│       │   │   ├── UI/        # UI逻辑
│       │   │   └── Weapon/    # 武器系统
│       │   ├── Prefabs/       # 预制体
│       │   ├── Sprites/       # 图片资源
│       │   ├── Scenes/        # 场景文件
│       │   └── Audio/         # 音频资源
│       └── ...
├── 05_导出/          # 微信小游戏导出目录
└── 06_截图录像/      # 宣传素材
```

## 🚀 开始开发

### 1. 打开Unity工程
- 打开团结引擎 Hub
- Unity版本：2022.3.61f1
- 添加项目 → 选择 `04_Unity工程\肉鸽割草` 文件夹
- 打开项目

### 2. 安装微信小游戏插件
**官方插件仓库：** https://gitee.com/wechat-minigame/minigame-unity-webgl-transform

**安装步骤：**
1. 访问上述仓库，下载最新 Release
2. 解压后将 `minigame.unitypackage` 导入Unity
3. 详见：`01_文档/微信插件使用说明.md`

### 3. 创建场景
- 在 `Assets/Scenes/` 创建 `GameScene`
- 基础设置：2D模式、正交相机

### 4. 使用AI生成代码
- 打开 `01_文档/AI提示词.md`
- 复制提示词到 Kimi/文心一言/通义千问
- 生成代码后放到对应文件夹

### 5. 微信小游戏导出
- Unity菜单：`微信小游戏` → `转换小游戏`
- 配置 AppID：`wxa6101eec77c5541d`
- 导出到 `05_导出/微信小游戏/`
- 用微信开发者工具打开测试

## 📝 开发顺序建议

1. PlayerController（玩家移动+攻击）
2. EnemyBase + Spawner（敌人生成）
3. WeaponBase（武器系统）
4. GameManager（游戏逻辑）
5. UI系统
6. 微信导出测试

## 📱 AppID
wxa6101eec77c5541d

## 🔗 重要链接
- **微信插件仓库**：https://gitee.com/wechat-minigame/minigame-unity-webgl-transform
- **微信开发者文档**：https://developers.weixin.qq.com/minigame/dev/guide/
