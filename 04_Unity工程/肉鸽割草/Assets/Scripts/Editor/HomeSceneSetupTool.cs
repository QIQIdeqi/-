using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace GeometryWarrior.Editor
{
    /// <summary>
    /// 家园场景快速搭建工具 - 一键创建所有基础结构
    /// </summary>
    public class HomeSceneSetupTool : EditorWindow
    {
        private bool createHomeManager = true;
        private bool createEnvironment = true;
        private bool createNPCAndDoor = true;
        private bool createUI = true;
        private bool createDecorations = true;

        private Color floorColor = new Color(1f, 0.71f, 0.76f); // 浅粉色 #FFB6C1
        private Color wallColor = new Color(1f, 0.94f, 0.96f); // 淡紫粉 #FFF0F5
        private Color npcColor = new Color(1f, 0.41f, 0.71f); // 热粉色 #FF69B4
        private Color doorColor = new Color(0.87f, 0.63f, 0.87f); // 梅红色 #DDA0DD

        [MenuItem("Geometry Warrior/家园场景/一键搭建场景")]
        public static void ShowWindow()
        {
            GetWindow<HomeSceneSetupTool>("家园场景搭建工具");
        }

        private void OnGUI()
        {
            GUILayout.Label("🏠 家园场景快速搭建工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("此工具将一键创建家园场景的所有基础结构，使用色块占位，后续可替换为正式美术资源。", MessageType.Info);
            EditorGUILayout.Space();

            // 选项区域
            GUILayout.Label("创建选项", EditorStyles.boldLabel);
            createHomeManager = EditorGUILayout.ToggleLeft("创建 HomeManager 管理器", createHomeManager);
            createEnvironment = EditorGUILayout.ToggleLeft("创建环境 (地板/墙面)", createEnvironment);
            createNPCAndDoor = EditorGUILayout.ToggleLeft("创建 NPC 和 门", createNPCAndDoor);
            createUI = EditorGUILayout.ToggleLeft("创建 UI 画布和面板", createUI);
            createDecorations = EditorGUILayout.ToggleLeft("创建装饰物容器", createDecorations);

            EditorGUILayout.Space();
            
            // 颜色设置
            if (createEnvironment || createNPCAndDoor)
            {
                GUILayout.Label("占位色块颜色设置", EditorStyles.boldLabel);
                floorColor = EditorGUILayout.ColorField("地板颜色", floorColor);
                wallColor = EditorGUILayout.ColorField("墙面颜色", wallColor);
                npcColor = EditorGUILayout.ColorField("NPC颜色", npcColor);
                doorColor = EditorGUILayout.ColorField("门颜色", doorColor);
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            
            // 创建按钮
            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
            if (GUILayout.Button("🚀 一键搭建家园场景", GUILayout.Height(40)))
            {
                SetupHomeScene();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space();
            
            // 快捷操作
            GUILayout.Label("快捷操作", EditorStyles.boldLabel);
            if (GUILayout.Button("仅创建示例装扮部件"))
            {
                CreateSampleOutfitParts();
            }
            if (GUILayout.Button("选中 HomeManager"))
            {
                SelectHomeManager();
            }
        }

        private void SetupHomeScene()
        {
            // 检查场景是否已保存
            if (string.IsNullOrEmpty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path))
            {
                EditorUtility.DisplayDialog("提示", "请先保存当前场景（File > Save Scene），然后再运行此工具。", "确定");
                return;
            }

            int createdCount = 0;

            // 1. 创建HomeManager
            if (createHomeManager)
            {
                CreateHomeManager();
                createdCount++;
            }

            // 2. 创建环境
            if (createEnvironment)
            {
                CreateEnvironment();
                createdCount++;
            }

            // 3. 创建NPC和Door
            if (createNPCAndDoor)
            {
                CreateNPCAndDoor();
                createdCount++;
            }

            // 4. 创建UI
            if (createUI)
            {
                CreateUI();
                createdCount++;
            }

            // 5. 创建装饰物容器
            if (createDecorations)
            {
                CreateDecorations();
                createdCount++;
            }

            // 配置引用关系
            ConfigureReferences();

            EditorUtility.DisplayDialog("完成", $"家园场景搭建完成！\n\n创建了 {createdCount} 个主要模块。\n\n请检查各物体的Inspector面板进行微调。", "确定");
            
            Debug.Log("[HomeSceneSetupTool] 家园场景搭建完成！");
        }

        private void CreateHomeManager()
        {
            // 检查是否已存在
            var existing = FindObjectOfType<HomeManager>();
            if (existing != null)
            {
                Debug.LogWarning("[HomeSceneSetupTool] HomeManager 已存在，跳过创建");
                return;
            }

            GameObject homeManager = new GameObject("【管理器】HomeManager");
            homeManager.transform.position = Vector3.zero;
            
            var manager = homeManager.AddComponent<HomeManager>();
            
            // 设置边界（通过反射或序列化，这里只是占位）
            EditorUtility.SetDirty(manager);
            
            Debug.Log("[HomeSceneSetupTool] 创建 HomeManager");
        }

        private void CreateEnvironment()
        {
            // 创建环境父物体
            GameObject envParent = new GameObject("【环境】");
            envParent.transform.position = Vector3.zero;

            // 创建地板网格
            GameObject floorGrid = new GameObject("FloorGrid");
            floorGrid.transform.SetParent(envParent.transform);
            floorGrid.transform.position = Vector3.zero;
            
            // 创建示例地板Tile（4x4网格）
            for (int x = -2; x < 2; x++)
            {
                for (int y = -2; y < 2; y++)
                {
                    GameObject tile = CreatePlaceholderSprite($"Tile_{x}_{y}", floorColor, new Vector3(x, y, 0), new Vector3(0.95f, 0.95f, 1));
                    tile.transform.SetParent(floorGrid.transform);
                }
            }

            // 创建左墙
            GameObject leftWall = CreatePlaceholderSprite("LeftWall", wallColor, new Vector3(-3, 1, 0), new Vector3(1, 4, 1));
            leftWall.transform.SetParent(envParent.transform);
            leftWall.AddComponent<BoxCollider2D>();

            // 创建右墙
            GameObject rightWall = CreatePlaceholderSprite("RightWall", wallColor, new Vector3(3, 1, 0), new Vector3(1, 4, 1));
            rightWall.transform.SetParent(envParent.transform);
            rightWall.AddComponent<BoxCollider2D>();

            Debug.Log("[HomeSceneSetupTool] 创建环境 (地板/墙面)");
        }

        private void CreateNPCAndDoor()
        {
            GameObject interactables = new GameObject("【交互物体】");
            interactables.transform.position = Vector3.zero;

            // 创建NPC
            GameObject npc = CreatePlaceholderSprite("HomeNPC", npcColor, new Vector3(2, 0, 0), Vector3.one * 0.8f);
            npc.transform.SetParent(interactables.transform);
            npc.AddComponent<CircleCollider2D>().isTrigger = true;
            var npcComponent = npc.AddComponent<HomeNPC>();
            EditorUtility.SetDirty(npcComponent);

            // 创建Door
            GameObject door = CreatePlaceholderSprite("HomeDoor", doorColor, new Vector3(-2, 0, 0), new Vector3(1, 1.5f, 1));
            door.transform.SetParent(interactables.transform);
            door.AddComponent<BoxCollider2D>().isTrigger = true;
            var doorComponent = door.AddComponent<HomeDoor>();
            EditorUtility.SetDirty(doorComponent);

            // 创建PlayerSpawnPoint
            GameObject spawnPoint = new GameObject("PlayerSpawnPoint");
            spawnPoint.transform.SetParent(interactables.transform);
            spawnPoint.transform.position = Vector3.zero;

            Debug.Log("[HomeSceneSetupTool] 创建 NPC、Door 和 SpawnPoint");
        }

        private void CreateUI()
        {
            // 检查是否已有Canvas
            Canvas existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas != null)
            {
                Debug.LogWarning("[HomeSceneSetupTool] Canvas 已存在，跳过创建");
                return;
            }

            // 创建Canvas
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();

            // 创建EventSystem
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // 创建OutfitPanel
            GameObject outfitPanel = new GameObject("OutfitPanel");
            outfitPanel.transform.SetParent(canvasGO.transform, false);
            outfitPanel.AddComponent<OutfitPanel>();
            
            RectTransform panelRect = outfitPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            outfitPanel.SetActive(false); // 默认隐藏

            // 添加面板背景
            Image panelImage = outfitPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);

            Debug.Log("[HomeSceneSetupTool] 创建 UI (Canvas + OutfitPanel)");
        }

        private void CreateDecorations()
        {
            GameObject decorations = new GameObject("【装饰物】");
            decorations.transform.position = Vector3.zero;

            // 创建几个示例装饰物（用不同颜色区分）
            Color rugColor = new Color(1f, 0.75f, 0.8f); // 粉红
            Color sofaColor = new Color(1f, 1f, 1f); // 白色
            Color plantColor = new Color(0.56f, 0.93f, 0.56f); // 浅绿

            // 地毯
            GameObject rug = CreatePlaceholderSprite("Deco_地毯", rugColor, new Vector3(0, -1, 0), new Vector3(1.5f, 1f, 1));
            rug.transform.SetParent(decorations.transform);
            var rugDeco = rug.AddComponent<HomeDecoration>();
            rugDeco.decorationId = "rug_01";
            rugDeco.decorationName = "圆形地毯";
            EditorUtility.SetDirty(rugDeco);

            // 沙发
            GameObject sofa = CreatePlaceholderSprite("Deco_沙发", sofaColor, new Vector3(-1, 0.5f, 0), new Vector3(2f, 1.2f, 1));
            sofa.transform.SetParent(decorations.transform);
            var sofaDeco = sofa.AddComponent<HomeDecoration>();
            sofaDeco.decorationId = "sofa_01";
            sofaDeco.decorationName = "云朵沙发";
            EditorUtility.SetDirty(sofaDeco);

            // 盆栽
            GameObject plant = CreatePlaceholderSprite("Deco_盆栽", plantColor, new Vector3(1.5f, 1.5f, 0), Vector3.one * 0.4f);
            plant.transform.SetParent(decorations.transform);
            var plantDeco = plant.AddComponent<HomeDecoration>();
            plantDeco.decorationId = "plant_01";
            plantDeco.decorationName = "多肉盆栽";
            EditorUtility.SetDirty(plantDeco);

            Debug.Log("[HomeSceneSetupTool] 创建装饰物 (地毯/沙发/盆栽)");
        }

        private GameObject CreatePlaceholderSprite(string name, Color color, Vector3 position, Vector3 scale)
        {
            GameObject go = new GameObject(name);
            go.transform.position = position;
            go.transform.localScale = scale;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd"); // 使用内置的方形纹理
            sr.color = color;

            return go;
        }

        private void ConfigureReferences()
        {
            // 获取所有组件
            var homeManager = FindObjectOfType<HomeManager>();
            var homeNPC = FindObjectOfType<HomeNPC>();
            var homeDoor = FindObjectOfType<HomeDoor>();
            var outfitPanel = FindObjectOfType<OutfitPanel>();

            // 这里可以通过反射设置私有字段，但为了避免复杂性，建议手动配置
            Debug.Log("[HomeSceneSetupTool] 基础结构已创建。请在 Inspector 中配置引用关系：");
            Debug.Log("  - HomeManager: 配置 PlayerPrefab, SpawnPoint, NPC, Door, Decorations");
            Debug.Log("  - HomeNPC: 配置 InteractionHint 和 OutfitPanel");
            Debug.Log("  - HomeDoor: 配置 ArrowIndicator");
        }

        private void CreateSampleOutfitParts()
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

            // 创建示例部件1: 粉色蝴蝶结
            CreateOutfitPartAsset("bow_pink", "粉色蝴蝶结", OutfitCategory.Bow, true, 0, 0);
            
            // 创建示例部件2: 红色蝴蝶结
            CreateOutfitPartAsset("bow_red", "红色蝴蝶结", OutfitCategory.Bow, true, 0, 0);
            
            // 创建示例部件3: 针织帽
            CreateOutfitPartAsset("hat_beanie", "针织帽", OutfitCategory.Hat, true, 0, 0);
            
            // 创建示例部件4: 圆框眼镜（需解锁）
            CreateOutfitPartAsset("glasses_round", "圆框眼镜", OutfitCategory.Glasses, false, 5, 100);

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("完成", "已创建 4 个示例装扮部件！\n\n- 粉色蝴蝶结 (默认解锁)\n- 红色蝴蝶结 (默认解锁)\n- 针织帽 (默认解锁)\n- 圆框眼镜 (等级5解锁)", "确定");
        }

        private void CreateOutfitPartAsset(string id, string name, OutfitCategory category, bool unlockedByDefault, int unlockLevel, int price)
        {
            string assetPath = $"Assets/Resources/OutfitParts/{id}.asset";
            
            // 检查是否已存在
            if (AssetDatabase.LoadAssetAtPath<OutfitPartData>(assetPath) != null)
            {
                Debug.LogWarning($"[HomeSceneSetupTool] 部件 {id} 已存在，跳过");
                return;
            }

            OutfitPartData partData = ScriptableObject.CreateInstance<OutfitPartData>();
            partData.partId = id;
            partData.partName = name;
            partData.description = $"{name} - 让编织精灵更可爱！";
            partData.category = category;
            partData.isUnlockedByDefault = unlockedByDefault;
            partData.unlockLevel = unlockLevel;
            partData.price = price;
            
            AssetDatabase.CreateAsset(partData, assetPath);
            Debug.Log($"[HomeSceneSetupTool] 创建装扮部件: {assetPath}");
        }

        private void SelectHomeManager()
        {
            var manager = FindObjectOfType<HomeManager>();
            if (manager != null)
            {
                Selection.activeGameObject = manager.gameObject;
                EditorGUIUtility.PingObject(manager.gameObject);
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "场景中找不到 HomeManager，请先运行'一键搭建场景'。", "确定");
            }
        }
    }
}
