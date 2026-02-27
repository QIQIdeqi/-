using UnityEngine;
using UnityEditor;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 紧急修复 OutfitParts 数据
    /// </summary>
    public class OutfitPartsEmergencyFix : EditorWindow
    {
        [MenuItem("绒毛几何物语/紧急修复/创建示例部件")]
        public static void ShowWindow()
        {
            GetWindow<OutfitPartsEmergencyFix>("创建示例部件");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("✦ 紧急创建示例装扮部件 ✦", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            EditorGUILayout.HelpBox(
                "检测到 Resources/OutfitParts 文件夹为空！\n\n" +
                "点击下面的按钮创建 9 个示例部件。", 
                MessageType.Warning);
            
            GUILayout.Space(10);
            
            GUI.backgroundColor = new Color(1f, 0.6f, 0.2f);
            if (GUILayout.Button("创建 9 个示例部件", GUILayout.Height(50)))
            {
                CreateAllSampleParts();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(20);
            
            EditorGUILayout.LabelField("当前状态", EditorStyles.boldLabel);
            
            // 显示当前文件夹状态
            string outfitPartsPath = Application.dataPath + "/Resources/OutfitParts";
            if (Directory.Exists(outfitPartsPath))
            {
                var files = Directory.GetFiles(outfitPartsPath, "*.asset");
                EditorGUILayout.LabelField($"Resources/OutfitParts 中的 .asset 文件数: {files.Length}");
                
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        EditorGUILayout.LabelField($"  • {Path.GetFileName(file)}");
                    }
                }
            }
            else
            {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("Resources/OutfitParts 文件夹不存在！");
                GUI.color = Color.white;
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("刷新 AssetDatabase"))
            {
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("完成", "AssetDatabase 已刷新", "确定");
            }
        }
        
        private void CreateAllSampleParts()
        {
            // 确保目录存在
            EnsureDirectoryExists("Assets/Resources/OutfitParts");
            
            // 9个示例部件
            var samples = new (string id, string name, string desc, OutfitCategory cat, Vector2 offset)[]
            {
                ("bow_red_001", "红色蝴蝶结", "热情活泼的红色蝴蝶结", OutfitCategory.Bow, new Vector2(0, 0.3f)),
                ("bow_pink_001", "粉色蝴蝶结", "甜美可爱的粉色蝴蝶结", OutfitCategory.Bow, new Vector2(0, 0.3f)),
                ("hat_beanie_001", "毛线帽", "温暖的针织毛线帽", OutfitCategory.Hat, new Vector2(0, 0.35f)),
                ("hat_straw_001", "草帽", "夏日清凉的草帽", OutfitCategory.Hat, new Vector2(0, 0.35f)),
                ("glasses_round_001", "圆框眼镜", "文艺气息的圆框眼镜", OutfitCategory.Glasses, new Vector2(0, 0.1f)),
                ("glasses_sun_001", "墨镜", "酷炫的墨镜", OutfitCategory.Glasses, new Vector2(0, 0.1f)),
                ("scarf_red_001", "红色围巾", "保暖的红色围巾", OutfitCategory.Scarf, new Vector2(0, -0.1f)),
                ("backpack_bear_001", "小熊背包", "可爱的小熊背包", OutfitCategory.Backpack, new Vector2(0, 0)),
                ("special_wings_001", "天使翅膀", "梦幻的天使翅膀", OutfitCategory.Special, new Vector2(0, 0.2f)),
            };
            
            int created = 0;
            
            foreach (var sample in samples)
            {
                string assetPath = $"Assets/Resources/OutfitParts/{sample.id}.asset";
                
                // 删除已存在的（重新创建）
                if (AssetDatabase.LoadAssetAtPath<OutfitPartData>(assetPath) != null)
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
                
                // 创建数据
                OutfitPartData data = ScriptableObject.CreateInstance<OutfitPartData>();
                data.partId = sample.id;
                data.partName = sample.name;
                data.description = sample.desc;
                data.category = sample.cat;
                data.isUnlockedByDefault = true;
                data.scale = Vector2.one;
                data.offset = sample.offset;
                data.rotation = 0;
                data.price = 0;
                data.unlockLevel = 0;
                data.hasParticleEffect = false;
                data.glowColor = Color.white;
                
                // 保存
                AssetDatabase.CreateAsset(data, assetPath);
                created++;
                
                Debug.Log($"[OutfitPartsEmergencyFix] 创建: {assetPath}");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"[OutfitPartsEmergencyFix] 共创建 {created} 个部件");
            EditorUtility.DisplayDialog("完成", 
                $"成功创建 {created} 个示例部件！\n\n" +
                "请重新运行游戏查看效果。", 
                "确定");
        }
        
        private void EnsureDirectoryExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace('\\', '/');
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
