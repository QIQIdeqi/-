using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园地板管理器 - 等距视角（isometric）自动拼接地板 tile
    /// 类似饥荒/星露谷的斜角房间视角
    /// </summary>
    [ExecuteInEditMode]
    public class HomeFloorManager : MonoBehaviour
    {
        [Header("【生成模式】")]
        [Tooltip("使用Tile拼接模式还是完整背景图模式")]
        [SerializeField] private bool useTileMode = false;
        
        [Header("【完整房间背景】")]
        [Tooltip("完整的房间背景图（包含地板+两面墙）")]
        [SerializeField] private Sprite roomBackgroundSprite;
        
        [Tooltip("房间背景缩放")]
        [SerializeField] private float roomBackgroundScale = 1f;
        
        [Header("【Tile模式 - 地板设置】")]
        [Tooltip("地板 tile 预制体（Tile模式时使用）")]
        [SerializeField] private GameObject floorTilePrefab;
        
        [Tooltip("单块地板宽度（世界单位）")]
        [SerializeField] private float tileWidth = 1f;
        
        [Tooltip("单块地板高度（世界单位）")]
        [SerializeField] private float tileHeight = 0.5f;
        
        [Tooltip("地板网格宽度（格子数）")]
        [SerializeField] private int gridWidth = 6;
        
        [Tooltip("地板网格高度（格子数）")]
        [SerializeField] private int gridHeight = 6;
        
        [Header("【两面墙背景】")]
        [Tooltip("是否生成两面墙背景")]
        [SerializeField] private bool generateWalls = true;
        
        [Tooltip("左墙预制体（完整的一面墙）")]
        [SerializeField] private GameObject leftWallPrefab;
        
        [Tooltip("右墙预制体（完整的一面墙）")]
        [SerializeField] private GameObject rightWallPrefab;
        
        [Tooltip("墙的Y轴偏移（用于微调墙与地板的对齐）")]
        [SerializeField] private float wallYOffset = 0f;
        
        [Tooltip("左墙的X轴偏移")]
        [SerializeField] private float leftWallXOffset = 0f;
        
        [Tooltip("右墙的X轴偏移")]
        [SerializeField] private float rightWallXOffset = 0f;
        
        [Tooltip("左墙缩放")]
        [SerializeField] private float leftWallScale = 1f;
        
        [Tooltip("右墙缩放")]
        [SerializeField] private float rightWallScale = 1f;
        
        [Header("【排序层级】")]
        [Tooltip("地板的基础排序层级")]
        [SerializeField] private int floorBaseSortingOrder = 0;
        
        [Tooltip("墙的基础排序层级（应该比地板高）")]
        [SerializeField] private int wallBaseSortingOrder = 100;
        
        [Header("【图案设置】")]
        [Tooltip("是否使用棋盘格图案")]
        [SerializeField] private bool useCheckerboardPattern = false;
        
        [Tooltip("棋盘格第二种颜色")]
        [SerializeField] private Color secondaryColor = new Color(0.95f, 0.95f, 0.95f);
        
        // 生成的地板容器
        private Transform floorContainer;
        private Transform wallContainer;
        
        // 房间原点（左下角）
        private Vector3 roomOrigin;
        
        private void OnEnable()
        {
            #if UNITY_EDITOR
            // 只在首次加载场景且没有容器时生成
            // 避免运行/停止运行时重复生成
            if (Application.isPlaying)
            {
                // 运行时：清理编辑模式的残留，生成新的
                CleanupEditorContainers();
                GenerateRoom();
            }
            // 编辑模式下不在这里生成，由 OnValidate 处理
            #else
            // 真机运行：直接生成
            GenerateRoom();
            #endif
        }
        
        /// <summary>
        /// 检查是否已存在容器
        /// </summary>
        private bool HasExistingContainers()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "FloorContainer" || child.name == "WallContainer")
                {
                    return true;
                }
            }
            return false;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// 清理编辑模式生成的容器
        /// </summary>
        private void CleanupEditorContainers()
        {
            // 查找并删除已存在的容器
            foreach (Transform child in transform)
            {
                if (child.name == "FloorContainer" || child.name == "WallContainer")
                {
                    // 编辑模式用 DestroyImmediate，运行时用 Destroy
                    if (Application.isPlaying)
                    {
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }
            floorContainer = null;
            wallContainer = null;
        }
        #endif
        
        private void Update()
        {
            // 在编辑模式下实时更新
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GenerateRoom();
            }
            #endif
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// 当Inspector参数改变时自动刷新
        /// </summary>
        private void OnValidate()
        {
            // 延迟执行，避免序列化问题
            UnityEditor.EditorApplication.delayCall += () =>
            {
                // 只在编辑模式、对象存在、且完全稳定时刷新
                // 排除：运行时、正在切换播放模式、有容器存在时（避免停止运行后重复生成）
                if (this == null || gameObject == null) return;
                if (Application.isPlaying) return;
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
                if (UnityEditor.EditorApplication.isPlaying) return;
                if (HasExistingContainers()) return; // 已有容器，不重复生成
                
                // 生成新的
                GenerateRoom();
            };
        }
        #endif
        
        /// <summary>
        /// 生成完整房间（地板+墙）
        /// </summary>
        public void GenerateRoom()
        {
            // 创建容器
            CreateContainers();
            
            if (useTileMode)
            {
                // Tile 拼接模式
                CalculateRoomOrigin();
                GenerateFloor();
                if (generateWalls)
                {
                    GenerateWalls();
                }
                if (Application.isPlaying)
                {
                    Debug.Log($"[HomeFloorManager] 生成了 {gridWidth}x{gridHeight} 的等距房间");
                }
            }
            else
            {
                // 完整背景图模式
                GenerateRoomBackground();
            }
        }
        
        /// <summary>
        /// 生成完整房间背景图
        /// </summary>
        private void GenerateRoomBackground()
        {
            if (roomBackgroundSprite == null)
            {
                Debug.LogError("[HomeFloorManager] Room Background Sprite 未设置！");
                return;
            }
            
            // 创建背景物体
            GameObject bgObj = new GameObject("RoomBackground");
            bgObj.transform.SetParent(floorContainer);
            bgObj.transform.position = transform.position;
            bgObj.transform.localScale = Vector3.one * roomBackgroundScale;
            
            // 添加 SpriteRenderer
            SpriteRenderer sr = bgObj.AddComponent<SpriteRenderer>();
            sr.sprite = roomBackgroundSprite;
            sr.sortingOrder = floorBaseSortingOrder;
            
            // 计算房间边界（基于背景图尺寸）
            float width = roomBackgroundSprite.bounds.size.x * roomBackgroundScale;
            float height = roomBackgroundSprite.bounds.size.y * roomBackgroundScale;
            roomOrigin = transform.position - new Vector3(width / 2f, height / 2f, 0);
            
            if (Application.isPlaying)
            {
                Debug.Log($"[HomeFloorManager] 使用完整背景图模式");
            }
        }
        
        /// <summary>
        /// <summary>
        /// 创建容器
        /// </summary>
        private void CreateContainers()
        {
            if (floorContainer == null)
            {
                floorContainer = new GameObject("FloorContainer").transform;
                floorContainer.SetParent(transform);
                floorContainer.localPosition = Vector3.zero;
            }
            else
            {
                ClearContainer(floorContainer);
            }
            
            if (wallContainer == null)
            {
                wallContainer = new GameObject("WallContainer").transform;
                wallContainer.SetParent(transform);
                wallContainer.localPosition = Vector3.zero;
            }
            else
            {
                ClearContainer(wallContainer);
            }
        }
        
        /// <summary>
        /// 清除容器内容
        /// </summary>
        private void ClearContainer(Transform container)
        {
            // 编辑模式下使用 DestroyImmediate，运行时 Destroy
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                while (container.childCount > 0)
                {
                    DestroyImmediate(container.GetChild(0).gameObject);
                }
                return;
            }
            #endif
            
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
        
        /// <summary>
        /// 计算房间原点（地板左下角）
        /// 让网格中心对齐到 transform.position
        /// </summary>
        private void CalculateRoomOrigin()
        {
            // 计算网格的中心 tile 位置
            float centerX = (gridWidth - 1) / 2f;
            float centerY = (gridHeight - 1) / 2f;
            
            // 中心 tile 的世界坐标
            Vector3 centerWorldPos = GetIsometricPositionFromOrigin(Vector3.zero, centerX, centerY);
            
            // 原点偏移 = 目标中心位置 - 当前中心位置
            roomOrigin = transform.position - centerWorldPos;
        }
        
        /// <summary>
        /// 从指定原点计算等距坐标位置
        /// </summary>
        private Vector3 GetIsometricPositionFromOrigin(Vector3 origin, float x, float y)
        {
            float posX = (x - y) * tileWidth / 2f;
            float posY = (x + y) * tileHeight / 2f;
            return origin + new Vector3(posX, posY, 0);
        }
        
        /// <summary>
        /// 生成等距地板
        /// </summary>
        private void GenerateFloor()
        {
            if (floorTilePrefab == null)
            {
                Debug.LogError("[HomeFloorManager] floorTilePrefab is not assigned!");
                return;
            }
            
            // 等距网格生成（菱形排列）
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 position = GetIsometricPosition(x, y);
                    CreateFloorTile(x, y, position);
                }
            }
        }
        
        /// <summary>
        /// 获取等距坐标位置
        /// </summary>
        private Vector3 GetIsometricPosition(int x, int y)
        {
            // 等距投影公式
            float posX = (x - y) * tileWidth / 2f;
            float posY = (x + y) * tileHeight / 2f;
            
            return roomOrigin + new Vector3(posX, posY, 0);
        }
        
        /// <summary>
        /// 创建地板 tile
        /// </summary>
        private void CreateFloorTile(int x, int y, Vector3 position)
        {
            GameObject tile = Instantiate(floorTilePrefab, position, Quaternion.identity, floorContainer);
            tile.name = $"FloorTile_{x}_{y}";
            
            // 排序层级（根据Y坐标，实现遮挡关系，从0开始）
            SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = floorBaseSortingOrder + y * gridWidth + x;
                
                // 棋盘格颜色
                if (useCheckerboardPattern)
                {
                    bool isEven = (x + y) % 2 == 0;
                    sr.color = isEven ? Color.white : secondaryColor;
                }
            }
        }
        
        /// <summary>
        /// 生成两面墙 - 完整墙壁模式
        /// </summary>
        private void GenerateWalls()
        {
            // 左墙（完整的一面墙）
            if (leftWallPrefab != null)
            {
                Vector3 position = GetLeftWallPosition();
                GameObject leftWall = Instantiate(leftWallPrefab, position, Quaternion.identity, wallContainer);
                leftWall.name = "LeftWall";
                
                // 设置缩放和排序
                leftWall.transform.localScale = Vector3.one * leftWallScale;
                SpriteRenderer sr = leftWall.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = wallBaseSortingOrder;
            }
            
            // 右墙（完整的一面墙）
            if (rightWallPrefab != null)
            {
                Vector3 position = GetRightWallPosition();
                GameObject rightWall = Instantiate(rightWallPrefab, position, Quaternion.identity, wallContainer);
                rightWall.name = "RightWall";
                
                // 设置缩放和排序
                rightWall.transform.localScale = Vector3.one * rightWallScale;
                SpriteRenderer sr = rightWall.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = wallBaseSortingOrder + 10;
            }
        }
        
        /// <summary>
        /// 获取左墙位置（完整墙壁）
        /// </summary>
        private Vector3 GetLeftWallPosition()
        {
            // 获取地板后边缘中间位置
            float centerX = (gridWidth - 1) / 2f;
            Vector3 basePos = GetIsometricPosition((int)centerX, gridHeight - 1);
            
            // 应用偏移
            return basePos + new Vector3(leftWallXOffset, wallYOffset, 0);
        }
        
        /// <summary>
        /// 获取右墙位置（完整墙壁）
        /// </summary>
        private Vector3 GetRightWallPosition()
        {
            // 获取地板右边缘中间位置
            float centerY = (gridHeight - 1) / 2f;
            Vector3 basePos = GetIsometricPosition(gridWidth - 1, (int)centerY);
            
            // 应用偏移
            return basePos + new Vector3(rightWallXOffset, wallYOffset, 0);
        }
        
        /// <summary>
        /// 清除所有地板和墙
        /// </summary>
        public void ClearRoom()
        {
            if (floorContainer != null) ClearContainer(floorContainer);
            if (wallContainer != null) ClearContainer(wallContainer);
        }
        
        /// <summary>
        /// 获取地板边界（用于限制玩家和装饰物）
        /// </summary>
        public Bounds GetFloorBounds()
        {
            Vector3 center = GetIsometricPosition(gridWidth / 2, gridHeight / 2);
            Vector3 size = new Vector3(
                (gridWidth + gridHeight) * tileWidth / 2f,
                (gridWidth + gridHeight) * tileHeight / 2f,
                0
            );
            return new Bounds(center, size);
        }
        
        /// <summary>
        /// 获取指定格子的世界坐标（用于放置家具）
        /// </summary>
        public Vector3 GetTileWorldPosition(int x, int y)
        {
            return GetIsometricPosition(x, y);
        }
        
        // Gizmos 已移除 - 用户自行绘制碰撞边界
    }
}
