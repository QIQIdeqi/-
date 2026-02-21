using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// TileManager - Manages the background tile grid (fixed position, centered on player at start)
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        [Header("[Tile Settings]")]
        [Tooltip("地块图片 - 必须在Inspector中设置")]
        [SerializeField] private Sprite tileSprite;
        [SerializeField] private int gridSizeX = 10;
        [SerializeField] private int gridSizeY = 10;
        [SerializeField] private float tileSize = 1f;
        [SerializeField] private Color tileColor = Color.white;
        
        [Header("[Spawn Point]")]
        [Tooltip("Player出生点 - 如果不设置，使用当前位置")]
        [SerializeField] private Transform spawnPoint;
        
        private SpriteRenderer[,] tileRenderers;
        private bool initialized = false;
        
        // 公开属性供其他脚本读取
        public float GridWidth => gridSizeX * tileSize;
        public float GridHeight => gridSizeY * tileSize;
        public Bounds GridBounds => new Bounds(
            transform.position + new Vector3(GridWidth / 2f, GridHeight / 2f, 0),
            new Vector3(GridWidth, GridHeight, 1)
        );
        
        private void Start()
        {
            // 如果有设置spawnPoint，将网格中心对准spawnPoint
            if (spawnPoint != null)
            {
                CenterOnPosition(spawnPoint.position);
            }
            
            // 生成Tile
            InitializeTiles();
        }
        
        /// <summary>
        /// 将TileManager定位，使网格中心对准指定位置
        /// </summary>
        public void CenterOnPosition(Vector3 centerPos)
        {
            float gridWidth = gridSizeX * tileSize;
            float gridHeight = gridSizeY * tileSize;
            
            // 网格中心对准指定位置
            // TileManager的位置 = 中心位置 - 网格半尺寸
            float posX = centerPos.x - gridWidth / 2f;
            float posY = centerPos.y - gridHeight / 2f;
            
            transform.position = new Vector3(posX, posY, 0);
        }
        
        private void InitializeTiles()
        {
            if (tileSprite == null)
            {
                Debug.LogWarning("TileManager: TileSprite未设置！请在Inspector中拖入地块图片。", this);
                return;
            }
            
            if (initialized) return;
            
            // 清除现有的tiles
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            tileRenderers = new SpriteRenderer[gridSizeX, gridSizeY];
            
            // 创建网格，以(0,0)为左下角开始
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    GameObject tileObj = new GameObject($"Tile_{x}_{y}");
                    tileObj.transform.SetParent(transform);
                    
                    // 设置本地位置（相对于父物体）
                    float posX = x * tileSize;
                    float posY = y * tileSize;
                    tileObj.transform.localPosition = new Vector3(posX, posY, 0);
                    
                    // 添加SpriteRenderer
                    SpriteRenderer sr = tileObj.AddComponent<SpriteRenderer>();
                    sr.sprite = tileSprite;
                    sr.color = tileColor;
                    sr.sortingLayerName = "Default";
                    sr.sortingOrder = -100; // 确保在最底层
                    
                    // 设置缩放以匹配tileSize
                    float spriteWidth = tileSprite.bounds.size.x;
                    float spriteHeight = tileSprite.bounds.size.y;
                    if (spriteWidth > 0 && spriteHeight > 0)
                    {
                        float scaleX = tileSize / spriteWidth;
                        float scaleY = tileSize / spriteHeight;
                        tileObj.transform.localScale = new Vector3(scaleX, scaleY, 1);
                    }
                    
                    tileRenderers[x, y] = sr;
                }
            }
            
            initialized = true;
            Debug.Log($"TileManager: 初始化完成 {gridSizeX}x{gridSizeY} 网格，位置固定", this);
        }
        
        /// <summary>
        /// 手动设置Tile Sprite
        /// </summary>
        public void SetTileSprite(Sprite sprite)
        {
            tileSprite = sprite;
            if (!initialized)
            {
                InitializeTiles();
            }
            else
            {
                if (tileRenderers != null)
                {
                    foreach (var sr in tileRenderers)
                    {
                        if (sr != null) sr.sprite = tileSprite;
                    }
                }
            }
        }
        
        /// <summary>
        /// 设置出生点位置（供外部调用）
        /// </summary>
        public void SetSpawnPoint(Vector3 position)
        {
            CenterOnPosition(position);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!initialized) return;
            
            // 绘制网格范围
            Gizmos.color = Color.cyan;
            float gridWidth = gridSizeX * tileSize;
            float gridHeight = gridSizeY * tileSize;
            Vector3 center = transform.position + new Vector3(gridWidth / 2f, gridHeight / 2f, 0);
            Gizmos.DrawWireCube(center, new Vector3(gridWidth, gridHeight, 0.1f));
        }
    }
}
