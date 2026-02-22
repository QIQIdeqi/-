using UnityEngine;
using UnityEditor;
using System.IO;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 皮肤配置编辑器工具
    /// </summary>
    public class SkinConfigEditor : EditorWindow
    {
        private string savePath = "Assets/Resources/Skins/";
        
        [MenuItem("Geometry Warrior/皮肤配置工具")]
        public static void ShowWindow()
        {
            GetWindow<SkinConfigEditor>("皮肤配置工具");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("皮肤配置快速生成器", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            savePath = EditorGUILayout.TextField("保存路径:", savePath);
            
            GUILayout.Space(10);
            GUILayout.Label("点击按钮创建示例皮肤:", EditorStyles.label);
            
            if (GUILayout.Button("创建全部示例皮肤", GUILayout.Height(30)))
            {
                CreateAllSkins();
            }
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("1. 创建默认皮肤"))
            {
                CreateSkin(SkinConfigCreator.CreateDefaultSkin(), "Skin_Default.asset");
            }
            
            if (GUILayout.Button("2. 创建火焰皮肤"))
            {
                CreateSkin(SkinConfigCreator.CreateFireSkin(), "Skin_Fire.asset");
            }
            
            if (GUILayout.Button("3. 创建冰霜皮肤"))
            {
                CreateSkin(SkinConfigCreator.CreateIceSkin(), "Skin_Ice.asset");
            }
            
            if (GUILayout.Button("4. 创建黄金皮肤"))
            {
                CreateSkin(SkinConfigCreator.CreateGoldSkin(), "Skin_Gold.asset");
            }
            
            if (GUILayout.Button("5. 创建霓虹皮肤"))
            {
                CreateSkin(SkinConfigCreator.CreateNeonSkin(), "Skin_Neon.asset");
            }
            
            if (GUILayout.Button("6. 创建暗影皮肤"))
            {
                CreateSkin(SkinConfigCreator.CreateShadowSkin(), "Skin_Shadow.asset");
            }
        }
        
        private void CreateAllSkins()
        {
            CreateSkin(SkinConfigCreator.CreateDefaultSkin(), "Skin_Default.asset");
            CreateSkin(SkinConfigCreator.CreateFireSkin(), "Skin_Fire.asset");
            CreateSkin(SkinConfigCreator.CreateIceSkin(), "Skin_Ice.asset");
            CreateSkin(SkinConfigCreator.CreateGoldSkin(), "Skin_Gold.asset");
            CreateSkin(SkinConfigCreator.CreateNeonSkin(), "Skin_Neon.asset");
            CreateSkin(SkinConfigCreator.CreateShadowSkin(), "Skin_Shadow.asset");
            
            EditorUtility.DisplayDialog("完成", "所有示例皮肤已创建！\n请为每个皮肤配置图标和精灵图片。", "确定");
        }
        
        private void CreateSkin(SkinData skin, string fileName)
        {
            // 确保目录存在
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            
            string fullPath = savePath + fileName;
            
            // 检查是否已存在
            if (File.Exists(fullPath))
            {
                Debug.LogWarning($"皮肤已存在: {fullPath}");
                return;
            }
            
            // 创建资源
            AssetDatabase.CreateAsset(skin, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"创建皮肤成功: {fullPath}");
        }
    }
}
