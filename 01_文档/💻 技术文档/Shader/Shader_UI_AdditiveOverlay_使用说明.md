# UI Additive Overlay Shader 使用说明

## 简介

为《绒毛几何物语》家园背包系统设计的 UI Shader，支持九宫格主图 + 平铺叠加图效果。

---

## 两个版本

### 1. UI_AdditiveOverlay (基础版)
简单高效的 Add 叠加，适合大多数场景。

### 2. UI_AdditiveOverlay_Enhanced (增强版)
支持多种混合模式、滚动动画、旋转等高级功能。

---

## 快速开始

### 1. 创建材质球

1. 在 Project 窗口右键 → Create → Material
2. 选择 Shader: `Custom/UI_AdditiveOverlay` 或 `Custom/UI_AdditiveOverlay_Enhanced`
3. 拖拽到 UI Image 组件的 Material 槽位

### 2. 设置九宫格

1. 选中主图 (MainTex)
2. Inspector → Sprite Editor
3. 设置 Border（九宫格切割线）
4. Image 组件的 Image Type 选择 `Sliced`

### 3. 配置叠加图

| 参数 | 说明 |
|------|------|
| AddTex | 叠加纹理（会平铺在整个主图上）|
| Add Intensity | 叠加强度 |
| Add Tex Scale | 平铺缩放（越大平铺次数越多）|

---

## 参数详解

### 基础版参数

```
主图 (MainTex)          - 九宫格主纹理
叠加图 (AddTex)         - 平铺叠加纹理
叠加强度 (Add Intensity) - 叠加图亮度倍数
平铺滚动X (Add Speed X)  - X方向滚动速度
平铺滚动Y (Add Speed Y)  - Y方向滚动速度
```

### 增强版额外参数

```
[叠加设置]
混合模式 (Blend Mode)    - 0=Add 1=Multiply 2=Screen 3=Overlay 4=SoftLight
叠加染色 (Add Tint)      - 给叠加图染色

[平铺设置]
平铺缩放 (Add Tex Scale) - 控制平铺密度
滚动速度X/Y              - 动画滚动
旋转速度 (Rotate Speed)  - 叠加图旋转动画

[高级]
使用主图Alpha            - 是否只用主图的Alpha通道
反转叠加图               - 颜色反转
透明度裁剪               - 剔除过低透明度像素
```

---

## 使用场景示例

### 场景 1：毛毡质感背景 + 纹理叠加
```
主图: 毛毡面板背景（九宫格）
叠加图: 细微的布料纹理（平铺）
混合模式: Add
强度: 0.3
效果: 背景有细微的织物纹理感
```

### 场景 2：动态流光边框
```
主图: 按钮背景（九宫格）
叠加图: 渐变光条（平铺）
混合模式: Add
滚动速度X: 0.5
效果: 流光扫过按钮
```

### 场景 3：选中态高亮
```
主图: 列表项背景（九宫格）
叠加图: 渐变圆形（平铺）
混合模式: SoftLight
强度: 0.5
效果: 柔和的选中高亮
```

---

## 材质球预设

### 毛毡纹理叠加 (FeltTextureOverlay)
```
Shader: UI_AdditiveOverlay
AddTex: 细微毛毡纹理
Add Intensity: 0.2
效果: 给纯色背景增加毛毡质感
```

### 流光按钮 (GlowingButton)
```
Shader: UI_AdditiveOverlay_Enhanced
Blend Mode: Add
AddTex: 水平渐变条
Add Speed X: 0.3
Add Tint: 浅粉色
效果: 粉色流光扫过按钮
```

### 选中高亮 (SelectedHighlight)
```
Shader: UI_AdditiveOverlay_Enhanced
Blend Mode: Overlay
AddTex: 径向渐变
Add Intensity: 0.4
效果: 柔和的选中状态
```

---

## 注意事项

1. **九宫格设置**：主图必须在 Sprite Editor 中设置好 Border，Image Type 选择 Sliced
2. **叠加图导入**：AddTex 的 Wrap Mode 建议设为 Repeat（虽然 Shader 里用了 frac，但这样更安全）
3. **性能**：增强版功能多但稍重，移动端大量使用时建议用基础版
4. **微信小游戏**：Shader 使用 HLSL，团结引擎/Tuanjie 兼容

---

## 故障排除

### 叠加图没有平铺
- 检查 Add Tex Scale 是否设置正确
- 检查 AddTex 是否有内容（不是纯黑）

### 九宫格没有效果
- 检查主图是否设置了 Border
- 检查 Image Type 是否为 Sliced

### 颜色太亮/太暗
- 调整 Add Intensity
- 尝试不同的 Blend Mode

### 移动端显示异常
- 确保使用基础版 Shader
- 检查是否开启了 GPU Instancing

---

## 文件位置

```
Assets/
├── Shaders/
│   ├── UI_AdditiveOverlay.shader          # 基础版
│   ├── UI_AdditiveOverlay_Enhanced.shader # 增强版
│   └── Includes/                          # 如果有公共库
├── Materials/
│   └── UI/
│       ├── FeltTextureOverlay.mat         # 毛毡纹理叠加
│       ├── GlowingButton.mat              # 流光按钮
│       └── SelectedHighlight.mat          # 选中高亮
└── Resources/
    └── Textures/
        └── UI/
            ├── felt_pattern.png           # 毛毡纹理示例
            └── gradient_flow.png          # 流光纹理示例
```

---

*创建时间: 2026-02-28*
*适用项目: 绒毛几何物语*
