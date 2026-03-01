# 绒毛几何物语 - 代码索引
> 生成时间: 2026-03-01 21:35:34
> 脚本总数: 59

## 📑 目录
- [核心系统概览](#核心系统概览)
- [单例模式清单](#单例模式清单)
- [按分类索引](#按分类索引)
- [完整脚本清单](#完整脚本清单)
- [冗余代码提示](#冗余代码提示)

## 核心系统概览

| 系统 | 核心脚本 | 职责 |
|------|----------|------|
| 游戏管理 | `GameManager` | - |
| 家园系统 | `HomeDoor`, `HomeNPC`, `HomeManager` | - |
| 换装系统 | `OutfitManager`, `PlayerOutfitApplier`, `OutfitPanelNew` | - |
| 背包系统 | `BackpackButton`, `BackpackPanel` | - |
| 武器系统 | `WeaponBase`, `WeaponManager` | - |
| 升级系统 | `UpgradeUI`, `UpgradeManager` | - |
| 敌人系统 | `EnemyBase`, `EnemySpawner` | - |

## 单例模式清单

- `CodeIndexer` - 代码索引生成器 - 自动生成工程代码文档
- `HomeSceneSetupTool` - 家园场景快速搭建工具 - 一键创建所有基础结构
- `OutfitPartCreator` - 装扮部件创建工具
- `FurnitureInventory` - 家具库存管理器 - 管理背包中家具的数量
- `HomeDecoration` - 家园装饰物 - 支持点击编辑模式
- `GameManager` - GameManager - Singleton pattern, manages global game state and UI
- `HomeManager` - 家园管理器 - 管理家园场景的所有功能
- `OutfitManager` - 从 Resources 文件夹自动加载所有部件
- `BackpackPanel` - 背包主界面 - 整合主角装备和家园装扮两个页签
- `HomeHUD` - 家园场景HUD - 显示摇杆、背包按钮、提示信息等
- `UpgradeManager` - UpgradeManager - Manages level-up upgrade selection
- `WeaponManager` - WeaponManager - Singleton, manages all weapon instances

## 按分类索引

### Editor工具 (15)

- `BackpackPanelLayoutFixer` - BackpackPanel 列表布局修复工具（已弃用 - 使用 UIBuilderTool 替代）
- `BackpackSystemSetupWizard` - 背包系统一键配置工具（已弃用 - 使用 UIBuilderTool 替代）
- `BackpackUIReferenceSetupTool` - 背包UI引用配置工具（已弃用 - 使用 UIBuilderTool 替代）
- `CodeAutoCleanupTool` - 代码自动清理工具 - 安全删除冗余脚本
- `CodeIndexAutoRefresher` - 检查是否需要自动刷新
- `CodeIndexer` - 代码索引生成器 - 自动生成工程代码文档
- `DocumentOrganizer` - 文档规整工具 - 一键整理项目文档目录
- `GameHUDFixer` - GameHUD 引用修复工具
- `HomeManagerSceneFix` - 修复 HomeManager 场景名引用
- `HomeSceneSetupTool` - 家园场景快速搭建工具 - 一键创建所有基础结构
- `HomeSystemCleanupTool` - 
- `OutfitPartCreator` - 装扮部件创建工具
- `SceneDebugger` - 场景调试工具 - 查找损坏的对象和空引用
- `UIBuilderTool` - UI生成工具 - 一键创建家园UI预制体
- `UIFontFixer` - UI字体修复工具 - 将TextMeshPro替换为普通UI Text

### UI系统 (16)

- `BackpackButton` - 背包按钮 - 家园场景左上角的背包入口
- `BackpackPanel` - 背包主界面 - 整合主角装备和家园装扮两个页签
- `ButtonAnimation` - 羊毛毡按钮点击效果 - 按压回弹
- `FurnitureItemUI` - 家具项UI - 背包中的家具列表项
- `GameHUD` - 
- `GameOverPanel` - Game Over / Pause Panel - 支持死亡和暂停两种模式
- `HomeHUD` - 家园场景HUD - 显示摇杆、背包按钮、提示信息等
- `HUDPauseButton` - HUD 暂停按钮
- `Joystick` - 虚拟摇杆 - 支持触摸和鼠标输入
- `MainMenuPanel` - 主菜单面板
- `OutfitItemUI` - 装扮部件列表项UI
- `OutfitPanel` - 装扮界面 - 分部件换装系统
- `OutfitPanelNew` - 
- `QuickOutfitTool` - 应用整套换装
- `UpgradeOptionUI` - Individual upgrade option UI element
- `UpgradeUI` - UpgradeUI - UI for upgrade selection (3-choice system)

### 家园系统 (7)

- `FurnitureData` - 家具数据 - 可收集的家园装饰物
- `FurnitureEditController` - 家具编辑控制器 - 拖拽、翻转、缩放、确认
- `FurnitureInventory` - 家具库存管理器 - 管理背包中家具的数量
- `HomeDecoration` - 家园装饰物 - 支持点击编辑模式
- `HomeDoor` - 家园之门 - 玩家接触后显示离开提示，点击返回主界面
- `HomeFloorManager` - 检查是否已存在容器
- `HomeNPC` - 家园NPC - 与玩家交互，打开装扮界面

### 敌人系统 (3)

- `EnemyBase` - EnemyBase - 敌人基类（支持属性Debuff系统）
- `EnemyHealthBar` - EnemyHealthBar - 敌人血条显示
- `EnemySpawner` - 单条难度记录数据

### 数据定义 (1)

- `OutfitPartData` - 装扮部件数据 - 可装备的装饰部件（蝴蝶结、帽子等）

### 武器系统 (7)

- `LaserWeapon` - LaserWeapon - Fires penetrating laser beams at enemies
- `MissileProjectile` - MissileProjectile - Homing missile that tracks target and explodes
- `MissileWeapon` - MissileWeapon - Fires homing missiles that track enemies
- `UpgradeData` - UpgradeType - Types of upgrades available
- `UpgradeManager` - UpgradeManager - Manages level-up upgrade selection
- `WeaponBase` - WeaponBase - Abstract base class for all weapons
- `WeaponManager` - WeaponManager - Singleton, manages all weapon instances

### 玩家系统 (3)

- `PlayerController` - PlayerController - Player movement, auto-attack, health, and level-up system
- `PlayerOutfitApplier` - 装扮变化回调
- `Projectile` - Projectile - 玩家飞弹（简化版，移除元素系统）

### 管理器 (7)

- `CameraFollow` - CameraFollow - Smooth camera follow for player
- `ExpOrb` - ExpOrb - Experience orb that players collect to gain EXP
- `GameManager` - GameManager - Singleton pattern, manages global game state and UI
- `HomeManager` - 家园管理器 - 管理家园场景的所有功能
- `OutfitManager` - 从 Resources 文件夹自动加载所有部件
- `SimpleCameraFollow` - Simple Camera Follow - Alternative camera follow script
- `TileManager` - TileManager - Manages the background tile grid (fixed position, centered on player at start)

## 完整脚本清单

### BackpackButton
- **文件**: `Assets\Scripts\UI\BackpackButton.cs`
- **命名空间**: FluffyGeometry.UI
- **描述**: 背包按钮 - 家园场景左上角的背包入口
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: OpenBackpack, CloseBackpack, OnPanelClosed, SetNewItemBadge
- **被引用**: CodeAutoCleanupTool, CodeIndexer, HomeSystemCleanupTool, UIBuilderTool, HomeManager...

### BackpackPanel
- **文件**: `Assets\Scripts\UI\BackpackPanel.cs`
- **命名空间**: GeometryWarrior
- **描述**: 背包主界面 - 整合主角装备和家园装扮两个页签
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: Initialize, Show, Show, Hide, Reopen
- **被引用**: BackpackPanelLayoutFixer, BackpackSystemSetupWizard, CodeAutoCleanupTool, CodeIndexer, HomeSystemCleanupTool...

### BackpackPanelLayoutFixer
- **文件**: `Assets\Scripts\Editor\BackpackPanelLayoutFixer.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: BackpackPanel 列表布局修复工具（已弃用 - 使用 UIBuilderTool 替代）
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: HomeSystemCleanupTool

### BackpackSystemSetupWizard
- **文件**: `Assets\Scripts\Editor\BackpackSystemSetupWizard.cs`
- **命名空间**: FluffyGeometry.Editor
- **描述**: 背包系统一键配置工具（已弃用 - 使用 UIBuilderTool 替代）
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: HomeSystemCleanupTool

### BackpackUIReferenceSetupTool
- **文件**: `Assets\Scripts\Editor\BackpackUIReferenceSetupTool.cs`
- **命名空间**: FluffyGeometry.Editor
- **描述**: 背包UI引用配置工具（已弃用 - 使用 UIBuilderTool 替代）
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: HomeSystemCleanupTool

### ButtonAnimation
- **文件**: `Assets\Scripts\UI\UIAnimationHelper.cs`
- **命名空间**: 
- **描述**: 羊毛毡按钮点击效果 - 按压回弹
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: OnPointerDown, OnPointerUp, OnPointerEnter, OnPointerExit

### CameraFollow
- **文件**: `Assets\Scripts\Manager\CameraFollow.cs`
- **命名空间**: GeometryWarrior
- **描述**: CameraFollow - Smooth camera follow for player
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: SetTarget
- **被引用**: SimpleCameraFollow

### CodeAutoCleanupTool
- **文件**: `Assets\Scripts\Editor\CodeAutoCleanupTool.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 代码自动清理工具 - 安全删除冗余脚本
- **单例**: 否
- **MonoBehaviour**: 否

### CodeIndexAutoRefresher
- **文件**: `Assets\Scripts\Editor\CodeIndexAutoRefresher.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 检查是否需要自动刷新
- **单例**: 否
- **MonoBehaviour**: 否

### CodeIndexer
- **文件**: `Assets\Scripts\Editor\CodeIndexer.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 代码索引生成器 - 自动生成工程代码文档
- **单例**: 是
- **MonoBehaviour**: 是
- **被引用**: CodeIndexAutoRefresher

### DocumentOrganizer
- **文件**: `Assets\Scripts\Editor\DocumentOrganizer.cs`
- **命名空间**: 
- **描述**: 文档规整工具 - 一键整理项目文档目录
- **单例**: 否
- **MonoBehaviour**: 否

### EnemyBase
- **文件**: `Assets\Scripts\Enemy\EnemyBase.cs`
- **命名空间**: GeometryWarrior
- **描述**: EnemyBase - 敌人基类（支持属性Debuff系统）
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: ApplyBurning, RemoveBurning, ApplyFreeze, RemoveFreeze, ApplySlow...
- **被引用**: CodeIndexer, EnemyHealthBar, EnemySpawner, PlayerController, Projectile...

### EnemyHealthBar
- **文件**: `Assets\Scripts\Enemy\EnemyHealthBar.cs`
- **命名空间**: GeometryWarrior
- **描述**: EnemyHealthBar - 敌人血条显示
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: ShowPermanent, Hide
- **被引用**: EnemyBase

### EnemySpawner
- **文件**: `Assets\Scripts\Enemy\EnemySpawner.cs`
- **命名空间**: GeometryWarrior
- **描述**: 单条难度记录数据
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: ToCsvLine, ExportDifficultyHistory, SaveToLocal, OpenSaveFolder, ClearAllEnemies...
- **被引用**: CodeIndexer, GameManager

### ExpOrb
- **文件**: `Assets\Scripts\Manager\ExpOrb.cs`
- **命名空间**: GeometryWarrior
- **描述**: ExpOrb - Experience orb that players collect to gain EXP
- **单例**: 否
- **MonoBehaviour**: 是

### FurnitureData
- **文件**: `Assets\Scripts\Home\FurnitureData.cs`
- **命名空间**: FluffyGeometry.Home
- **描述**: 家具数据 - 可收集的家园装饰物
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: FurnitureEditController, FurnitureInventory, HomeDecoration, HomeManager, FurnitureItemUI

### FurnitureEditController
- **文件**: `Assets\Scripts\Home\FurnitureEditController.cs`
- **命名空间**: FluffyGeometry.Home
- **描述**: 家具编辑控制器 - 拖拽、翻转、缩放、确认
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: Initialize, GetPlacedData, Cancel
- **被引用**: CodeAutoCleanupTool, HomeManager

### FurnitureInventory
- **文件**: `Assets\Scripts\Home\FurnitureInventory.cs`
- **命名空间**: FluffyGeometry.Home
- **描述**: 家具库存管理器 - 管理背包中家具的数量
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: GetAvailableCount, GetTotalCount, GetPlacedCount, AddFurniture, CanPlace...
- **被引用**: HomeManager, FurnitureItemUI

### FurnitureItemUI
- **文件**: `Assets\Scripts\UI\FurnitureItemUI.cs`
- **命名空间**: FluffyGeometry.UI
- **描述**: 家具项UI - 背包中的家具列表项
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: Setup, UpdateCountDisplay, OnPointerClick, SetSelected, HideDecorateButton...

### GameHUD
- **文件**: `Assets\Scripts\UI\GameHUD.cs`
- **命名空间**: GeometryWarrior
- **描述**: 
- **单例**: 否
- **MonoBehaviour**: 是
- **被引用**: GameHUDFixer, SceneDebugger, GameManager

### GameHUDFixer
- **文件**: `Assets\Scripts\Editor\GameHUDFixer.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: GameHUD 引用修复工具
- **单例**: 否
- **MonoBehaviour**: 否

### GameManager
- **文件**: `Assets\Scripts\Manager\GameManager.cs`
- **命名空间**: GeometryWarrior
- **描述**: GameManager - Singleton pattern, manages global game state and UI
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: StartGame, PauseGame, ResumeGame, RestartGame, ReturnToMenu...
- **被引用**: CodeIndexer, SceneDebugger, EnemySpawner, CameraFollow, PlayerController...

### GameOverPanel
- **文件**: `Assets\Scripts\UI\GameOverPanel.cs`
- **命名空间**: GeometryWarrior
- **描述**: Game Over / Pause Panel - 支持死亡和暂停两种模式
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: Show, UpdateDisplay, Hide, GetCurrentMode
- **被引用**: GameHUDFixer, SceneDebugger, GameManager

### HomeDecoration
- **文件**: `Assets\Scripts\Home\HomeDecoration.cs`
- **命名空间**: GeometryWarrior
- **描述**: 家园装饰物 - 支持点击编辑模式
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: Select, Deselect, EnterEditMode, ExitEditMode, Flip...
- **被引用**: HomeSceneSetupTool, HomeManager

### HomeDoor
- **文件**: `Assets\Scripts\Home\HomeDoor.cs`
- **命名空间**: GeometryWarrior
- **描述**: 家园之门 - 玩家接触后显示离开提示，点击返回主界面
- **单例**: 否
- **MonoBehaviour**: 是
- **被引用**: CodeIndexer, HomeSceneSetupTool, HomeManager

### HomeFloorManager
- **文件**: `Assets\Scripts\Home\HomeFloorManager.cs`
- **命名空间**: GeometryWarrior
- **描述**: 检查是否已存在容器
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: GenerateRoom, ClearRoom, GetFloorBounds, GetTileWorldPosition

### HomeHUD
- **文件**: `Assets\Scripts\UI\HomeHUD.cs`
- **命名空间**: GeometryWarrior
- **描述**: 家园场景HUD - 显示摇杆、背包按钮、提示信息等
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: ShowNPCHint, HideNPCHint, ShowExitHint, HideExitHint, ShowToast
- **被引用**: BackpackSystemSetupWizard, HomeSystemCleanupTool, UIBuilderTool

### HomeManager
- **文件**: `Assets\Scripts\Manager\HomeManager.cs`
- **命名空间**: GeometryWarrior
- **描述**: 家园管理器 - 管理家园场景的所有功能
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: ReturnToMainMenu, SaveDecorationPosition, SaveAllFurnitureData, AddDecoration, RemoveDecoration...
- **被引用**: CodeAutoCleanupTool, CodeIndexer, HomeManagerSceneFix, HomeSceneSetupTool, FurnitureInventory...

### HomeManagerSceneFix
- **文件**: `Assets\Scripts\Editor\HomeManagerSceneFix.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 修复 HomeManager 场景名引用
- **单例**: 否
- **MonoBehaviour**: 否

### HomeNPC
- **文件**: `Assets\Scripts\Home\HomeNPC.cs`
- **命名空间**: GeometryWarrior
- **描述**: 家园NPC - 与玩家交互，打开装扮界面
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: OnOutfitPanelClosed
- **被引用**: CodeIndexer, HomeSceneSetupTool, HomeManager, OutfitPanel

### HomeSceneSetupTool
- **文件**: `Assets\Scripts\Editor\HomeSceneSetupTool.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 家园场景快速搭建工具 - 一键创建所有基础结构
- **单例**: 是
- **MonoBehaviour**: 否

### HomeSystemCleanupTool
- **文件**: `Assets\Scripts\Editor\HomeSystemCleanupTool.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 
- **单例**: 否
- **MonoBehaviour**: 否

### HUDPauseButton
- **文件**: `Assets\Scripts\UI\HUDPauseButton.cs`
- **命名空间**: GeometryWarrior
- **描述**: HUD 暂停按钮
- **单例**: 否
- **MonoBehaviour**: 是

### Joystick
- **文件**: `Assets\Scripts\UI\Joystick.cs`
- **命名空间**: GeometryWarrior
- **描述**: 虚拟摇杆 - 支持触摸和鼠标输入
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: GetDirection, OnPointerDown, OnDrag, OnPointerUp
- **被引用**: HomeSystemCleanupTool, UIBuilderTool, HomeManager, PlayerController, HomeHUD

### LaserWeapon
- **文件**: `Assets\Scripts\Weapon\LaserWeapon.cs`
- **命名空间**: GeometryWarrior
- **描述**: LaserWeapon - Fires penetrating laser beams at enemies
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: UpgradeData

### MainMenuPanel
- **文件**: `Assets\Scripts\UI\MainMenuPanel.cs`
- **命名空间**: GeometryWarrior
- **描述**: 主菜单面板
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: Show, Hide
- **被引用**: GameHUDFixer, SceneDebugger, GameManager

### MissileProjectile
- **文件**: `Assets\Scripts\Weapon\MissileProjectile.cs`
- **命名空间**: GeometryWarrior
- **描述**: MissileProjectile - Homing missile that tracks target and explodes
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: SetTarget, SetDamage, SetSpeed, SetTurnSpeed, SetExplosionRadius
- **被引用**: MissileWeapon

### MissileWeapon
- **文件**: `Assets\Scripts\Weapon\MissileWeapon.cs`
- **命名空间**: GeometryWarrior
- **描述**: MissileWeapon - Fires homing missiles that track enemies
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: UpgradeData

### OutfitItemUI
- **文件**: `Assets\Scripts\UI\OutfitItemUI.cs`
- **命名空间**: GeometryWarrior
- **描述**: 装扮部件列表项UI
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: Setup, SetSelected
- **被引用**: CodeAutoCleanupTool, OutfitPanel

### OutfitManager
- **文件**: `Assets\Scripts\Manager\OutfitManager.cs`
- **命名空间**: GeometryWarrior
- **描述**: 从 Resources 文件夹自动加载所有部件
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: IsPartUnlocked, UnlockPart, EquipPart, UnequipPart, GetEquippedPart...
- **被引用**: CodeAutoCleanupTool, CodeIndexer, HomeManager, PlayerController, PlayerOutfitApplier...

### OutfitPanel
- **文件**: `Assets\Scripts\UI\OutfitPanel.cs`
- **命名空间**: GeometryWarrior
- **描述**: 装扮界面 - 分部件换装系统
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: Show, Hide
- **被引用**: BackpackPanelLayoutFixer, BackpackSystemSetupWizard, BackpackUIReferenceSetupTool, CodeAutoCleanupTool, CodeIndexer...

### OutfitPanelNew
- **文件**: `Assets\Scripts\UI\OutfitPanelNew.cs`
- **命名空间**: 
- **描述**: 
- **单例**: 否
- **MonoBehaviour**: 是
- **被引用**: BackpackSystemSetupWizard, CodeAutoCleanupTool, CodeIndexer, HomeSystemCleanupTool, UIBuilderTool...

### OutfitPartCreator
- **文件**: `Assets\Scripts\Editor\OutfitPartCreator.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 装扮部件创建工具
- **单例**: 是
- **MonoBehaviour**: 否
- **被引用**: CodeAutoCleanupTool

### OutfitPartData
- **文件**: `Assets\Scripts\Data\OutfitPartData.cs`
- **命名空间**: GeometryWarrior
- **描述**: 装扮部件数据 - 可装备的装饰部件（蝴蝶结、帽子等）
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: CodeAutoCleanupTool, HomeSceneSetupTool, OutfitPartCreator, OutfitManager, PlayerOutfitApplier...

### PlayerController
- **文件**: `Assets\Scripts\Player\PlayerController.cs`
- **命名空间**: GeometryWarrior
- **描述**: PlayerController - Player movement, auto-attack, health, and level-up system
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: ApplySkin, SetMovementBounds, TakeDamage, Revive, AddExp...
- **被引用**: EnemyBase, EnemySpawner, HomeDoor, HomeNPC, CameraFollow...

### PlayerOutfitApplier
- **文件**: `Assets\Scripts\Player\PlayerOutfitApplier.cs`
- **命名空间**: GeometryWarrior
- **描述**: 装扮变化回调
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: ApplyOutfit
- **被引用**: CodeIndexer, HomeManager, PlayerController, OutfitPanelNew

### Projectile
- **文件**: `Assets\Scripts\Player\Projectile.cs`
- **命名空间**: GeometryWarrior
- **描述**: Projectile - 玩家飞弹（简化版，移除元素系统）
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: SetTarget, SetDamage, InitializeFull
- **被引用**: PlayerController, MissileProjectile, MissileWeapon

### QuickOutfitTool
- **文件**: `Assets\Scripts\UI\QuickOutfitTool.cs`
- **命名空间**: 
- **描述**: 应用整套换装
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: ApplyOutfitSet, SaveCurrentAsSet

### SceneDebugger
- **文件**: `Assets\Scripts\Editor\SceneDebugger.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: 场景调试工具 - 查找损坏的对象和空引用
- **单例**: 否
- **MonoBehaviour**: 否

### SimpleCameraFollow
- **文件**: `Assets\Scripts\Manager\SimpleCameraFollow.cs`
- **命名空间**: GeometryWarrior
- **描述**: Simple Camera Follow - Alternative camera follow script
- **单例**: 否
- **MonoBehaviour**: 是

### TileManager
- **文件**: `Assets\Scripts\Manager\TileManager.cs`
- **命名空间**: GeometryWarrior
- **描述**: TileManager - Manages the background tile grid (fixed position, centered on player at start)
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: CenterOnPosition, SetTileSprite, SetSpawnPoint
- **被引用**: EnemySpawner, GameManager

### UIBuilderTool
- **文件**: `Assets\Scripts\Editor\UIBuilderTool.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: UI生成工具 - 一键创建家园UI预制体
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: BackpackPanelLayoutFixer, BackpackSystemSetupWizard, BackpackUIReferenceSetupTool, CodeAutoCleanupTool, UIFontFixer

### UIFontFixer
- **文件**: `Assets\Scripts\Editor\UIFontFixer.cs`
- **命名空间**: GeometryWarrior.Editor
- **描述**: UI字体修复工具 - 将TextMeshPro替换为普通UI Text
- **单例**: 否
- **MonoBehaviour**: 否
- **被引用**: CodeAutoCleanupTool

### UpgradeData
- **文件**: `Assets\Scripts\Weapon\UpgradeData.cs`
- **命名空间**: GeometryWarrior
- **描述**: UpgradeType - Types of upgrades available
- **单例**: 否
- **MonoBehaviour**: 否
- **公共方法**: GetDisplayName, GetDescription
- **被引用**: UpgradeOptionUI, UpgradeUI, UpgradeManager

### UpgradeManager
- **文件**: `Assets\Scripts\Weapon\UpgradeManager.cs`
- **命名空间**: GeometryWarrior
- **描述**: UpgradeManager - Manages level-up upgrade selection
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: ShowUpgradeOptions
- **被引用**: CodeIndexer

### UpgradeOptionUI
- **文件**: `Assets\Scripts\UI\UpgradeOptionUI.cs`
- **命名空间**: GeometryWarrior
- **描述**: Individual upgrade option UI element
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: Setup, SetDelay
- **被引用**: UpgradeUI

### UpgradeUI
- **文件**: `Assets\Scripts\UI\UpgradeUI.cs`
- **命名空间**: GeometryWarrior
- **描述**: UpgradeUI - UI for upgrade selection (3-choice system)
- **单例**: 否
- **MonoBehaviour**: 是
- **公共方法**: ShowUpgradeOptions, Hide
- **被引用**: CodeAutoCleanupTool, CodeIndexer, UpgradeManager

### WeaponBase
- **文件**: `Assets\Scripts\Weapon\WeaponBase.cs`
- **命名空间**: GeometryWarrior
- **描述**: WeaponBase - Abstract base class for all weapons
- **单例**: 否
- **MonoBehaviour**: 是
- **被引用**: CodeIndexer, LaserWeapon, MissileWeapon, UpgradeManager, WeaponManager

### WeaponManager
- **文件**: `Assets\Scripts\Weapon\WeaponManager.cs`
- **命名空间**: GeometryWarrior
- **描述**: WeaponManager - Singleton, manages all weapon instances
- **单例**: 是
- **MonoBehaviour**: 是
- **公共方法**: AddWeaponFromPrefab, AddWeaponByTypeName, HasWeaponByTypeName, GetWeaponByTypeName, UpgradeWeapon...
- **被引用**: CodeIndexer, UpgradeManager, WeaponBase

## 冗余代码提示

> **提示**: 命名空间 `GeometryWarrior` 包含 36 个类，建议拆分
> **提示**: 命名空间 `GeometryWarrior.Editor` 包含 12 个类，建议拆分
> **提示**: 以下 3 个脚本缺少描述注释：
> - HomeSystemCleanupTool
> - GameHUD
> - OutfitPanelNew

---
*此文档由 CodeIndexer 自动生成*
