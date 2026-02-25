using UnityEngine;
using UnityEditor;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 装扮部件创建工具
    /// </summary>
    public class OutfitPartCreator : EditorWindow
    {
        private string partId = "part_001";
        private string partName = "新部件";
        private OutfitCategory category = OutfitCategory.Bow;
        private Sprite partSprite;
        private Sprite icon;
        private bool isUnlockedByDefault = true;
        private int unlockLevel = 0;
        private int price = 0;
        
        [MenuItem("Geometry Warrior/创建装扮部件")]
        public static void ShowWindow()
        {
            GetWindow<OutfitPartCreator>("创建装扮部件");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("创建新的装扮部件", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            partId = EditorGUILayout.TextField("部件ID", partId);
            partName = EditorGUILayout.TextField("部件名称", partName);
            category = (OutfitCategory)EditorGUILayout.EnumPopup("类别", category);
            
            EditorGUILayout.Space();
            partSprite = (Sprite)EditorGUILayout.ObjectField("部件精灵", partSprite, typeof(Sprite), false);
            icon = (Sprite)EditorGUILayout.ObjectField("图标", icon, typeof(Sprite), false);
            
            EditorGUILayout.Space();
            isUnlockedByDefault = EditorGUILayout.Toggle("默认解锁", isUnlockedByDefault);
            unlockLevel = EditorGUILayout.IntField("解锁等级", unlockLevel);
            price = EditorGUILayout.IntField("价格", price);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("创建部件", GUILayout.Height(30)))
            {
                CreateOutfitPart();
            }
        }
        
        private void CreateOutfitPart()
        {
            // 确保文件夹存在
            string folderPath = "Assets/Resources/OutfitParts";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string resourcesPath = "Assets/Resources";
                if (!AssetDatabase.IsValidFolder(resourcesPath))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                AssetDatabase.CreateFolder(resourcesPath, "OutfitParts");
            }
            
            // 创建数据资产
            OutfitPartData partData = ScriptableObject.CreateInstance<OutfitPartData>();
            partData.partId = partId;
            partData.partName = partName;
            partData.description = $"{partName} - 让编织精灵更可爱！";
            partData.category = category;
            partData.partSprite = partSprite;
            partData.icon = icon != null ? icon : partSprite;
            partData.isUnlockedByDefault = isUnlockedByDefault;
            partData.unlockLevel = unlockLevel;
            partData.price = price;
            
            // 保存资产
            string assetPath = $"{folderPath}/{partId}.asset";
            AssetDatabase.CreateAsset(partData, assetPath);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"[OutfitPartCreator] 创建装扮部件: {assetPath}");
            
            // 选中创建的资产
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = partData;
        }
    }
}
