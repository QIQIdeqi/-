using UnityEngine;
using UnityEditor;

namespace GeometryWarrior
{
    /// <summary>
    /// Editor tool to generate simple weapon icons
    /// </summary>
    public class WeaponIconGenerator : EditorWindow
    {
        [MenuItem("Tools/Geometry Warrior/Generate Weapon Icons")]
        public static void ShowWindow()
        {
            GetWindow<WeaponIconGenerator>("Weapon Icon Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("生成武器图标", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("生成激光图标 (红色细长矩形)", GUILayout.Height(30)))
            {
                GenerateLaserIcon();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("生成导弹图标 (橙色三角形)", GUILayout.Height(30)))
            {
                GenerateMissileIcon();
            }

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("图标将保存在 Assets/Sprites/Icons/ 目录", MessageType.Info);
        }

        private void GenerateLaserIcon()
        {
            // Create directory
            string path = "Assets/Sprites/Icons/";
            System.IO.Directory.CreateDirectory(path);

            // Create texture (64x16 - thin rectangle)
            Texture2D texture = new Texture2D(64, 16, TextureFormat.RGBA32, false);
            
            // Fill with red color
            Color red = new Color(1f, 0.2f, 0.2f, 1f);
            Color glow = new Color(1f, 0.5f, 0.5f, 0.8f);
            
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    // Center is brighter
                    float distFromCenter = Mathf.Abs(y - 7.5f) / 7.5f;
                    Color pixelColor = Color.Lerp(red, glow, distFromCenter);
                    texture.SetPixel(x, y, pixelColor);
                }
            }
            
            texture.Apply();

            // Save as PNG
            byte[] bytes = texture.EncodeToPNG();
            string filePath = path + "Icon_Laser.png";
            System.IO.File.WriteAllBytes(filePath, bytes);
            
            AssetDatabase.ImportAsset(filePath);
            
            // Configure import settings
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            Debug.Log($"激光图标已生成: {filePath}");
            DestroyImmediate(texture);
        }

        private void GenerateMissileIcon()
        {
            // Create directory
            string path = "Assets/Sprites/Icons/";
            System.IO.Directory.CreateDirectory(path);

            // Create texture (32x32 - triangle)
            Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            
            // Fill with transparent first
            Color transparent = new Color(0, 0, 0, 0);
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            // Draw triangle (pointing right)
            Color orange = new Color(1f, 0.5f, 0f, 1f);
            Color orangeDark = new Color(0.8f, 0.4f, 0f, 1f);
            
            for (int x = 0; x < 32; x++)
            {
                // Triangle shape: width increases from left to right
                int widthAtX = Mathf.RoundToInt((x / 31f) * 30f);
                int startY = 16 - widthAtX / 2;
                int endY = 16 + widthAtX / 2;
                
                for (int y = startY; y <= endY; y++)
                {
                    if (y >= 0 && y < 32)
                    {
                        // Gradient from dark to light
                        float gradient = x / 31f;
                        Color pixelColor = Color.Lerp(orangeDark, orange, gradient);
                        texture.SetPixel(x, y, pixelColor);
                    }
                }
            }
            
            texture.Apply();

            // Save as PNG
            byte[] bytes = texture.EncodeToPNG();
            string filePath = path + "Icon_Missile.png";
            System.IO.File.WriteAllBytes(filePath, bytes);
            
            AssetDatabase.ImportAsset(filePath);
            
            // Configure import settings
            TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Point;
                importer.SaveAndReimport();
            }

            Debug.Log($"导弹图标已生成: {filePath}");
            DestroyImmediate(texture);
        }
    }
}
