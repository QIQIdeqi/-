# HUD血条和经验条 - 九宫格样式设计提示词

## 设计说明

### 九宫格原理
- **四个角**：不拉伸，保持原样
- **四条边**：单向拉伸（上下边水平拉伸，左右边垂直拉伸）
- **中心区域**：双向拉伸（主要内容区域）

### 适用尺寸建议
- **血条/经验条高度**：20-40像素
- **边角宽度**：10-20像素（不拉伸区域）
- **总长度**：可以任意拉伸

---

## 方案1：毛线编织风格（推荐）

### 设计理念
将进度条设计成一条正在被编织的毛线，Fill区域是毛线逐渐变长的效果。

### Fill Area（背景框）- 英文提示词
```
Pure top-down orthographic view, UI element for game HUD.

A horizontal progress bar frame designed as a piece of embroidered fabric label.

Structure (9-slice compatible):
- Overall size: 200x40 pixels
- Corner areas (20x20 pixels each): Decorative elements
  - Top-left: Small felt flower or button detail
  - Top-right: Matching decorative element
  - Bottom-left: Loose thread end or stitch detail
  - Bottom-right: Matching finish
- Edge borders (20px width): Visible embroidery stitching
  - Top edge: Running stitch pattern in cream thread
  - Bottom edge: Matching stitch pattern
  - Left edge: Vertical blanket stitch
  - Right edge: Vertical blanket stitch

Center area (160x20 pixels, stretchable):
- Soft cream or beige felt texture background
- Subtle fabric weave pattern
- Very gentle gradient from light to slightly darker

Colors:
- Frame: Warm brown or cream stitching on beige felt
- Background: Soft cream #FFF8DC or light beige #F5F5DC
- Stitching: Darker brown or cream thread

Style: Handmade craft, cozy, soft edges, warm lighting, 9-patch ready

Parameters: --ar 5:1 --style raw --v 6.0
Negative: sharp edges, neon, metallic, geometric hard lines, gradient mesh
```

### Fill（填充条）- 英文提示词

#### 血条版本（红色/粉色系）
```
Pure top-down orthographic view, UI element for game HUD health bar fill.

A horizontal fill bar designed as a knitted yarn strip.

Structure (9-slice compatible):
- Overall size: 200x40 pixels
- Corner areas (20x20 pixels each): 
  - Rounded yarn corners with visible knit texture
  - Small pompom or decorative knot at left end
  - Clean rounded finish at right end
- Edge borders (20px height): 
  - Top: Thick knitted rib texture
  - Bottom: Matching rib texture

Center area (160x20 pixels, stretchable):
- Dense knitted wool texture
- V-stitch pattern visible (like stockinette stitch)
- Soft fuzzy yarn appearance
- Gentle horizontal ribbing

Colors:
- Base: Warm pastel pink #FFB6C1 or soft coral #FFA07A
- Highlights: Lighter pink areas suggesting yarn fuzz
- Shadows: Slightly darker pink in recesses between stitches
- Overall: Soft, warm, inviting

Style: Hand-knitted texture, soft and fuzzy, 9-patch ready, cozy wool appearance

Parameters: --ar 5:1 --style raw --v 6.0
Negative: smooth gradient, flat color, hard edges, geometric, shiny, plastic
```

#### 经验条版本（蓝色/绿色系）
```
Pure top-down orthographic view, UI element for game HUD experience bar fill.

A horizontal fill bar designed as a knitted yarn strip.

Structure (9-slice compatible):
- Overall size: 200x40 pixels
- Corner areas (20x20 pixels each): 
  - Rounded yarn corners with visible knit texture
  - Small decorative felt star or sparkle at left end
  - Clean rounded finish at right end
- Edge borders (20px height): 
  - Top: Thick knitted rib texture
  - Bottom: Matching rib texture

Center area (160x20 pixels, stretchable):
- Dense knitted wool texture
- V-stitch pattern visible (like stockinette stitch)
- Soft fuzzy yarn appearance
- Gentle horizontal ribbing

Colors:
- Base: Soft mint green #98FF98 or baby blue #87CEEB
- Highlights: Lighter areas suggesting yarn fuzz
- Shadows: Slightly darker tones in stitch recesses
- Optional: Subtle gold or yellow sparkle threads woven in

Style: Hand-knitted texture, soft and fuzzy, magical feel, 9-patch ready

Parameters: --ar 5:1 --style raw --v 6.0
Negative: smooth gradient, flat color, hard edges, geometric, shiny, plastic
```

---

## 方案2：刺绣卷轴风格

### Fill Area - 英文提示词
```
Pure top-down orthographic view, UI element for game HUD.

A horizontal progress bar frame designed as an embroidered ribbon banner.

Structure (9-slice compatible):
- Overall size: 200x36 pixels
- Left end (24x36 pixels): Decorative scroll end with scalloped edge
- Right end (24x36 pixels): Matching scroll end
- Top/bottom edges (12px height): Decorative embroidered border

Center area (152x36 pixels, stretchable):
- Linen or canvas texture background
- Subtle crosshatch fabric pattern
- Light beige or cream color

Decorative elements:
- Small embroidered flowers at corners
- Dotted stitch border around entire frame
- Tiny buttons or beads as accents

Colors:
- Background: Natural linen #FAF0E6 or cream
- Border: Pastel pink or mint embroidery thread
- Accents: Small colorful details

Style: Vintage sewing notion, delicate embroidery, 9-patch ready

Parameters: --ar 5:1 --style raw --v 6.0
```

### Fill - 血条 - 英文提示词
```
Pure top-down orthographic view, UI element for game HUD health bar fill.

A horizontal fill bar designed as an embroidered satin ribbon.

Structure (9-slice compatible):
- Overall size: 200x36 pixels
- Left end (24x36 pixels): Folded ribbon end with X-stitch
- Right end (24x36 pixels): Cut ribbon end with small fray

Center area (152x36 pixels, stretchable):
- Satin ribbon texture with subtle sheen
- Horizontal ribbed texture suggesting fabric weave
- Soft folds and creases for realism

Colors:
- Base: Rose pink or coral satin #FFB6C1
- Highlights: Lighter areas where light hits the satin
- Shadows: Soft folds creating depth
- Sheen: Very subtle glossy areas

Style: Embroidered ribbon, soft fabric texture, 9-patch ready

Parameters: --ar 5:1 --style raw --v 6.0
```

---

## 方案3：毛毡剪贴风格（最简洁）

### Fill Area - 英文提示词
```
Pure top-down orthographic view, UI element for game HUD.

A simple horizontal progress bar frame made of felt.

Structure (9-slice compatible):
- Overall size: 200x32 pixels
- Corner radius: 8px rounded corners
- Border: 4px thick felt edge

Center area (stretchable):
- Flat felt texture
- Very subtle fiber texture
- Light cream or white color

Colors:
- Border: Light brown or beige felt
- Background: Off-white or cream

Style: Minimalist felt craft, clean and simple, 9-patch ready

Parameters: --ar 6:1 --style raw --v 6.0
```

### Fill - 血条 - 英文提示词
```
Pure top-down orthographic view, UI element for game HUD health bar fill.

A simple horizontal fill bar made of colored felt.

Structure (9-slice compatible):
- Overall size: 200x32 pixels
- Corner radius: 8px rounded corners (left side only, or both for full bar)
- Clean cut edges

Center area (stretchable):
- Flat felt texture
- Visible fiber fuzz
- Solid color appearance

Colors:
- Health: Soft red or pink felt #FF9999 or #FFB6C1
- Exp: Mint green or sky blue felt #98FF98 or #87CEEB

Style: Minimalist felt cutout, soft texture, 9-patch ready

Parameters: --ar 6:1 --style raw --v 6.0
```

---

## Unity 九宫格设置说明

### 1. 导入设置
1. 选中图片 → Inspector
2. **Texture Type**: Sprite (2D and UI)
3. **Sprite Mode**: Single
4. 点击 **Sprite Editor**

### 2. 九宫格切割
在Sprite Editor中：
```
Border 设置：
- Left: 20 (根据你的边角宽度调整)
- Right: 20
- Top: 20
- Bottom: 20

这表示上下左右各20像素为不拉伸区域
```

### 3. UI设置
1. 创建 Image 对象
2. 拖拽 Sprite 到 Source Image
3. 选择 **Image Type**: Sliced
4. 勾选 **Fill Center**（如果需要填充中心）

### 4. 配合Slider使用
```csharp
// 血条Slider设置
Slider healthSlider;
Image fillImage;

// 代码控制
healthSlider.value = currentHealth / maxHealth;
```

---

## 设计对比

| 方案 | 复杂度 | 风格 | 推荐用途 |
|-----|-------|------|---------|
| 毛线编织 | 高 | 精致手工 | 主HUD，重点展示 |
| 刺绣卷轴 | 中 | 优雅复古 | 血条，强调治疗/生命 |
| 毛毡剪贴 | 低 | 简洁清新 | 经验条，次要UI |

---

## 导出建议

### 图片尺寸
- **推荐**: 200x40 像素（可被4整除，方便九宫格切割）
- **最小**: 100x20 像素
- **格式**: PNG（保留透明背景）

### 九宫格边距设置
```
统一推荐: 20px
- 可以确保边角装饰完整保留
- 中心区域有足够空间拉伸
```

---

*设计日期：2026-02-23*
*项目：绒毛几何物语*
