# 快速解决方案：手动配置 OutfitManager

## 问题原因
运行时 `Resources.LoadAll` 返回 null，可能是因为 Unity 版本或资源编译问题。

## 最简单可靠的解决方案：Inspector 手动配置

### 步骤 1：找到 OutfitManager
1. 在场景中选中 **OutfitManager** 游戏对象
2. 如果没有，创建一个空物体，命名为 OutfitManager，添加 OutfitManager 脚本

### 步骤 2：配置 All Parts 列表
1. 在 Inspector 中找到 **All Parts** 字段
2. 点击右边的圆点 🔘，选择需要的部件
3. 或者直接把 Resources/OutfitParts 文件夹下的部件拖到列表中

### 具体操作
```
OutfitManager (Script)
  All Parts
    Size: 9
    Element 0: [拖入 bow_red_001]
    Element 1: [拖入 bow_pink_001]
    Element 2: [拖入 hat_beanie_001]
    ...以此类推
```

### 步骤 3：保存场景
1. Ctrl+S 保存场景
2. 运行游戏测试

---

## 自动化方案（推荐）

如果不想手动拖，运行下面这个菜单命令：

```
绒毛几何物语 → 自动配置 → OutfitManager 数据
```

这个命令会在编辑器模式下自动读取所有部件并填充到列表中。

---

## 备选方案：直接修改脚本

如果不想手动配置，可以修改 `OutfitManager.cs`，删除运行时和编辑器的区分，统一使用手动扫描的方式：

```csharp
private void LoadAllPartsFromResources()
{
    allParts.Clear();
    
    // 使用 Directory 直接读取文件
    string path = Application.dataPath + "/Resources/OutfitParts";
    if (Directory.Exists(path))
    {
        var files = Directory.GetFiles(path, "*.asset");
        foreach (var file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            // 使用 Resources.Load 单独加载每个文件
            var part = Resources.Load<OutfitPartData>($"OutfitParts/{fileName}");
            if (part != null)
                allParts.Add(part);
        }
    }
}
```

但需要注意在文件顶部添加：
```csharp
using System.IO;
```
