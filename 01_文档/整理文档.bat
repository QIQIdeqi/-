@echo off
chcp 65001
setlocal enabledelayedexpansion

set "DOC_PATH=D:\03_独立游戏\自己\微信小游戏\01_项目\项目01_肉鸽割草\01_文档"

cd /d "%DOC_PATH%"

echo ========================================
echo  绒毛几何物语 - 文档规整工具
echo ========================================
echo.
echo 文档路径: %DOC_PATH%
echo.

:: 创建目录结构
echo [1/3] 创建目录结构...

if not exist "📋 项目总览" mkdir "📋 项目总览"
if not exist "🎨 美术资产\角色设计" mkdir "🎨 美术资产\角色设计"
if not exist "🎨 美术资产\场景元素" mkdir "🎨 美术资产\场景元素"
if not exist "🎮 系统设计\核心玩法" mkdir "🎮 系统设计\核心玩法"
if not exist "🎮 系统设计\家园系统" mkdir "🎮 系统设计\家园系统"
if not exist "🎮 系统设计\UI系统" mkdir "🎮 系统设计\UI系统"
if not exist "💻 技术文档\项目配置" mkdir "💻 技术文档\项目配置"
if not exist "💻 技术文档\Shader" mkdir "💻 技术文档\Shader"
if not exist "📝 参考资源" mkdir "📝 参考资源"
if not exist "📁 归档" mkdir "📁 归档"

echo 目录创建完成！
echo.

:: 移动文件
echo [2/3] 整理文件...

:: 项目总览
call :moveFile "绒毛几何物语 - 游戏设计文档.md" "📋 项目总览"
call :moveFile "游戏设计文档.md" "📋 项目总览"
call :moveFile "文档规整方案.md" "📋 项目总览"

:: 美术资产
call :moveFile "AI提示词.md" "🎨 美术资产"
call :moveFile "AI提示词_家园场景UI.md" "🎨 美术资产"
call :moveFile "第一批素材提示词.md" "🎨 美术资产"
call :moveFile "Logo设计提示词.md" "🎨 美术资产"

:: 美术资产 - 场景元素
call :moveFile "地板Tile_Prompt.md" "🎨 美术资产\场景元素"
call :moveFile "两面墙_Prompt.md" "🎨 美术资产\场景元素"
call :moveFile "家居等距视角_Prompts.md" "🎨 美术资产\场景元素"

:: 家园系统
call :moveFile "家园系统配置指南.md" "🎮 系统设计\家园系统"
call :moveFile "家园场景配置步骤_色块占位版.md" "🎮 系统设计\家园系统"
call :moveFile "换装系统预研与配置指南.md" "🎮 系统设计\家园系统"
call :moveFile "背包系统配置指南.md" "🎮 系统设计\家园系统"

:: UI系统
call :moveFile "UI_图标设计规范.md" "🎮 系统设计\UI系统"
call :moveFile "HUD血条设计规范与提示词.md" "🎮 系统设计\UI系统"
call :moveFile "虚拟摇杆设计提示词.md" "🎮 系统设计\UI系统"
call :moveFile "暂停按钮设计提示词.md" "🎮 系统设计\UI系统"

:: 技术文档
call :moveFile "微信素材规范说明.md" "💻 技术文档\项目配置"
call :moveFile "Shader_UI_AdditiveOverlay_使用说明.md" "💻 技术文档\Shader"

:: 参考资源
call :moveFile "小红书文案.md" "📝 参考资源"
call :moveFile "小红书文案v2.md" "📝 参考资源"

:: 归档
call :moveFile "开发日志_2026-02-18.md" "📁 归档"
call :moveFile "开发日志_2026-02-18_晚间.md" "📁 归档"
call :moveFile "开发日志_2026-02-19.md" "📁 归档"
call :moveFile "开发日志_2026-02-20.md" "📁 归档"
call :moveFile "开发日志_2026-02-23.md" "📁 归档"

echo.
echo [3/3] 创建 README.md...

:: 创建 README.md
(
echo # 绒毛几何物语 - 文档中心
echo.
echo ^> 📅 最后更新：2026-03-01  
echo ^> 🎮 项目类型：微信小游戏（治愈系收集养成）  
echo ^> 🎨 美术风格：羊毛毡手作 + 马卡龙pastel色系
echo.
echo ---
echo.
echo ## 📋 项目总览
echo.
echo ^| 文档 ^| 说明 ^|
echo ^|------^|------^|
echo ^| [游戏设计文档](./项目总览/游戏设计文档.md) ^| 核心玩法、世界观、目标用户、游戏循环 ^|
echo ^| [文档规整方案](./项目总览/文档规整方案.md) ^| 本文档目录结构说明 ^|
echo.
echo ---
echo.
echo ## 🎨 美术资产
echo.
echo ### 风格指南
echo - [AI提示词总库](./美术资产/AI提示词.md) - 所有AI生成提示词汇总
echo - [Logo设计](./美术资产/Logo设计提示词.md)
echo.
echo ### 场景元素
echo - [地板Tile](./美术资产/场景元素/地板Tile_Prompt.md)
echo - [两面墙](./美术资产/场景元素/两面墙_Prompt.md)
echo - [家居等距视角](./美术资产/场景元素/家居等距视角_Prompts.md)
echo.
echo ---
echo.
echo ## 🎮 系统设计
echo.
echo ### 家园系统
echo ^| 文档 ^| 说明 ^|
echo ^|------^|------^|
echo ^| [家园系统配置指南](./家园系统/家园系统配置指南.md) ^| 完整配置步骤 ^|
echo ^| [家园场景配置步骤（色块占位版）](./家园系统/家园场景配置步骤_色块占位版.md) ^| 无美术资源时的开发配置 ^|
echo ^| [换装系统详解](./家园系统/换装系统预研与配置指南.md) ^| 分部件换装机制 ^|
echo ^| [背包系统配置](./家园系统/背包系统配置指南.md) ^| 背包数据配置 ^|
echo.
echo ### UI系统
echo ^| 文档 ^| 说明 ^|
echo ^|------^|------^|
echo ^| [UI图标设计规范](./UI系统/UI_图标设计规范.md) ^| 图标风格、尺寸规范 ^|
echo ^| [HUD血条设计](./UI系统/HUD血条设计规范与提示词.md) ^| 血条UI设计规范 ^|
echo ^| [虚拟摇杆设计](./UI系统/虚拟摇杆设计提示词.md) ^| 摇杆设计规范 ^|
echo ^| [暂停按钮设计](./UI系统/暂停按钮设计提示词.md) ^| 暂停按钮规范 ^|
echo.
echo ### 核心玩法
echo - （待添加）
echo.
echo ---
echo.
echo ## 💻 技术文档
echo.
echo - [微信素材规范](./项目配置/微信素材规范说明.md)
echo - [UI叠加Shader使用说明](./Shader/Shader_UI_AdditiveOverlay_使用说明.md)
echo.
echo ---
echo.
echo ## 📝 参考资源
echo.
echo - [小红书文案](./参考资源/小红书文案.md)
echo.
echo ---
echo.
echo ## 📁 归档
echo.
echo 历史开发日志（已合并）：
echo - 开发日志_2026-02-18.md
)echo - 开发日志_2026-02-19.md
echo - 开发日志_2026-02-20.md
echo - 开发日志_2026-02-23.md
echo.
echo ---
echo.
echo ## 🔗 项目链接
echo.
echo - **Unity工程**: `04_Unity工程/肉鸽割草/`
echo - **美术资源**: `02_美术资源/`
echo - **导出目录**: `05_导出/微信小游戏/`
echo.
echo ---
echo.
echo *文档中心自动生成 - 2026-03-01*
) > README.md

echo README.md 创建完成！
echo.
echo ========================================
echo  文档规整完成！
echo ========================================
echo.
echo 新目录结构：
dir /b /ad
echo.
pause
exit /b

:moveFile
if exist "%~1" (
    move "%~1" "%~2\" >nul 2>&1
    if !errorlevel! equ 0 (
        echo   [✓] %~1 → %~2\
    ) else (
        echo   [!] 移动失败: %~1
    )
) else (
    echo   [ ] 跳过(不存在): %~1
)
exit /b
