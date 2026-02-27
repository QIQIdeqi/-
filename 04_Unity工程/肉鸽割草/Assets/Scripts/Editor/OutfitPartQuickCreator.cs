using UnityEngine;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 快速创建装扮部件数据工具
    /// </summary>
    public class OutfitPartQuickCreator : EditorWindow
    {
        private string partId = "bow_red_001";
        private string partName = "红色蝴蝶结";
        private string description = "可爱的红色蝴蝶结，让编织精灵更加活泼~";
        private OutfitCategory category = OutfitCategory.Bow;
        private bool isUnlockedByDefault = true;
        
        [MenuItem("绒毛几何物语/快速创建/装扮部件")]
        public static void ShowWindow()
        {
            GetWindow<OutfitPartQuickCreator>("创建装扮部件");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ 快速创建装扮部件 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            // 基本信息
            GUILayout.Label("基本信息", EditorStyles.boldLabel);
            partId = EditorGUILayout.TextField("部件ID", partId);
            partName = EditorGUILayout.TextField("部件名称", partName);
            description = EditorGUILayout.TextField("描述", description, GUILayout.Height(40));
            category = (OutfitCategory)EditorGUILayout.EnumPopup("类别", category);
            isUnlockedByDefault = EditorGUILayout.Toggle("默认解锁", isUnlockedByDefault);
            
            GUILayout.Space(10);
            
            // ID命名建议
            EditorGUILayout.HelpBox(
                "ID命名建议：类别_颜色_序号\n" +
                "例如：bow_red_001, hat_blue_002, glasses_black_001", 
                MessageType.Info);
            
            GUILayout.Space(10);
            
            // 创建按钮
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("创建部件数据", GUILayout.Height(40)))
            {
                CreateOutfitPart();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(20);
            
            // 批量创建示例按钮
            GUILayout.Label("快捷操作", EditorStyles.boldLabel);
            if (GUILayout.Button("创建示例部件（9个）"))
            {
                CreateSampleParts();
            }
            
            if (GUILayout.Button("打开 OutfitParts 文件夹"))
            {
                string path = System.IO.Path.Combine(Application.dataPath, "Resources/OutfitParts");
                if (System.IO.Directory.Exists(path))
                {
                    System.Diagnostics.Process.Start("explorer.exe", path);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "文件夹不存在，请先创建至少一个部件。", "确定");
                }
            }
        }
        
        private void CreateOutfitPart()
        {
            // 确保目录存在
            EnsureDirectoryExists("Assets/Resources/OutfitParts");
            
            // 检查ID是否为空
            if (string.IsNullOrEmpty(partId))
            {
                EditorUtility.DisplayDialog("错误", "部件ID不能为空！", "确定");
                return;
            }
            
            // 检查是否已存在
            string assetPath = $"Assets/Resources/OutfitParts/{partId}.asset";
            if (AssetDatabase.LoadAssetAtPath<OutfitPartData>(assetPath) != null)
            {
                if (!EditorUtility.DisplayDialog("确认", $"部件 '{partId}' 已存在，是否覆盖？", "覆盖", "取消"))
                {
                    return;
                }
            }
            
            // 创建数据
            OutfitPartData data = ScriptableObject.CreateInstance<OutfitPartData>();
            data.partId = partId;
            data.partName = partName;
            data.description = description;
            data.category = category;
            data.isUnlockedByDefault = isUnlockedByDefault;
            data.scale = Vector2.one;
            
            // 根据类别设置默认偏移
            data.offset = GetDefaultOffsetForCategory(category);
            
            // 保存
            AssetDatabase.CreateAsset(data, assetPath);
            AssetDatabase.SaveAssets();
            
            // 选中
            Selection.activeObject = data;
            EditorGUIUtility.PingObject(data);
            
            Debug.Log($"[OutfitPartQuickCreator] 创建部件: {assetPath}");
            EditorUtility.DisplayDialog("成功", $"部件 '{partName}' 创建完成！\n\n记得设置图标和精灵图片。", "确定");
        }
        
        private Vector2 GetDefaultOffsetForCategory(OutfitCategory category)
        {
            switch (category)
            {
                case OutfitCategory.Bow:
                    return new Vector2(0, 0.3f); // 头顶上方
                case OutfitCategory.Hat:
                    return new Vector2(0, 0.35f);
                case OutfitCategory.Glasses:
                    return new Vector2(0, 0.1f);
                case OutfitCategory.Scarf:
                    return new Vector2(0, -0.1f);
                case OutfitCategory.Backpack:
                    return new Vector2(0, 0);
                case OutfitCategory.Shoes:
                    return new Vector2(0, -0.4f);
                case OutfitCategory.Special:
                    return new Vector2(0, 0.2f);
                default:
                    return Vector2.zero;
            }
        }
        
        private void CreateSampleParts()
        {
            // 确保目录存在
            EnsureDirectoryExists("Assets/Resources/OutfitParts");
            
            // 示例数据
            var samples = new (string id, string name, string desc, OutfitCategory cat)[]
            {
                ("bow_red_001", "红色蝴蝶结", "热情活泼的红色蝴蝶结", OutfitCategory.Bow),
                ("bow_pink_001", "粉色蝴蝶结", "甜美可爱的粉色蝴蝶结", OutfitCategory.Bow),
                ("hat_beanie_001", "毛线帽", "温暖的针织毛线帽", OutfitCategory.Hat),
                ("hat_straw_001", "草帽", "夏日清凉的草帽", OutfitCategory.Hat),
                ("glasses_round_001", "圆框眼镜", "文艺气息的圆框眼镜", OutfitCategory.Glasses),
                ("glasses_sun_001", "墨镜", "酷炫的墨镜", OutfitCategory.Glasses),
                ("scarf_red_001", "红色围巾", "保暖的红色围巾", OutfitCategory.Scarf),
                ("backpack_bear_001", "小熊背包", "可爱的小熊背包", OutfitCategory.Backpack),
                ("special_wings_001", "天使翅膀", "梦幻的天使翅膀", OutfitCategory.Special),
            };
            
            int created = 0;
            foreach (var sample in samples)
            {
                string assetPath = $"Assets/Resources/OutfitParts/{sample.id}.asset";
                
                // 跳过已存在的
                if (AssetDatabase.LoadAssetAtPath<OutfitPartData>(assetPath) != null)
                {
                    continue;
                }
                
                OutfitPartData data = ScriptableObject.CreateInstance<OutfitPartData>();
                data.partId = sample.id;
                data.partName = sample.name;
                data.description = sample.desc;
                data.category = sample.cat;
                data.isUnlockedByDefault = true;
                data.scale = Vector2.one;
                data.offset = GetDefaultOffsetForCategory(sample.cat);
                
                AssetDatabase.CreateAsset(data, assetPath);
                created++;
            }
            
            AssetDatabase.SaveAssets();
            
            Debug.Log($"[OutfitPartQuickCreator] 创建了 {created} 个示例部件");
            EditorUtility.DisplayDialog("完成", $"创建了 {created} 个示例部件！\n\n请为它们设置图标和精灵图片。", "确定");
        }
        
        private void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                string folderName = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
