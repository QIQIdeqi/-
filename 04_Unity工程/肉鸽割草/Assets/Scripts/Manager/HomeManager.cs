using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeometryWarrior
{
    /// <summary>
    /// 家园管理器 - 管理家园场景的所有功能
    /// </summary>
    public class HomeManager : MonoBehaviour
    {
        public static HomeManager Instance { get; private set; }
        
        [Header("【场景设置】")]
        [Tooltip("主菜单场景的名称，点击离开时会加载此场景")]
        [SerializeField] private string mainMenuSceneName = "MainScene";
        
        [Header("【玩家设置】")]
        [Tooltip("玩家角色预制体")]
        [SerializeField] private GameObject playerPrefab;
        
        [Tooltip("玩家进入家园时的出生位置")]
        [SerializeField] private Transform playerSpawnPoint;
        
        [Tooltip("虚拟摇杆（用于玩家移动控制）")]
        [SerializeField] private Joystick joystick;
        
        [Header("【装饰物】")]
        [Tooltip("家园中的所有装饰物列表")]
        [SerializeField] private List<HomeDecoration> decorations = new List<HomeDecoration>();
        
        [Header("【NPC和门】")]
        [Tooltip("家园中的NPC（用于打开装扮界面）")]
        [SerializeField] private HomeNPC homeNPC;
        
        [Tooltip("家园之门（用于返回主菜单）")]
        [SerializeField] private HomeDoor homeDoor;
        
        private PlayerController player;
        private const string DECORATION_SAVE_KEY = "HomeDecorations";
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            // 生成玩家
            SpawnPlayer();
            
            // 加载装饰物位置
            LoadDecorationPositions();
        }
        
        /// <summary>
        /// 生成玩家
        /// </summary>
        private void SpawnPlayer()
        {
            if (playerPrefab == null) return;
            
            Vector3 spawnPos = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
            GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            player = playerObj.GetComponent<PlayerController>();
            
            if (player != null)
            {
                // 设置虚拟摇杆
                if (joystick != null)
                {
                    player.joystick = joystick;
                }
                else
                {
                    // 自动查找场景中的摇杆
                    player.joystick = FindObjectOfType<Joystick>();
                }
                
                // 应用当前装扮
                ApplyPlayerOutfit();
            }
        }
        
        /// <summary>
        /// 应用玩家装扮
        /// </summary>
        private void ApplyPlayerOutfit()
        {
            if (OutfitManager.Instance != null && player != null)
            {
                var outfitApplier = player.GetComponent<PlayerOutfitApplier>();
                if (outfitApplier != null)
                {
                    outfitApplier.ApplyOutfit(OutfitManager.Instance.GetAllEquippedParts());
                }
            }
        }
        
        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void ReturnToMainMenu()
        {
            Debug.Log("[HomeManager] 返回主菜单");
            SceneManager.LoadScene(mainMenuSceneName);
        }
        
        /// <summary>
        /// 保存装饰物位置
        /// </summary>
        public void SaveDecorationPosition(string decorationId, Vector3 position)
        {
            string key = $"{DECORATION_SAVE_KEY}_{decorationId}";
            string posString = $"{position.x},{position.y},{position.z}";
            PlayerPrefs.SetString(key, posString);
            PlayerPrefs.Save();
            
            Debug.Log($"[HomeManager] 保存装饰物位置: {decorationId} = {position}");
        }
        
        /// <summary>
        /// 加载装饰物位置
        /// </summary>
        private void LoadDecorationPositions()
        {
            foreach (var deco in decorations)
            {
                string key = $"{DECORATION_SAVE_KEY}_{deco.GetComponent<HomeDecoration>().decorationId}";
                string savedPos = PlayerPrefs.GetString(key, "");
                
                if (!string.IsNullOrEmpty(savedPos))
                {
                    string[] parts = savedPos.Split(',');
                    if (parts.Length == 3)
                    {
                        if (float.TryParse(parts[0], out float x) &&
                            float.TryParse(parts[1], out float y) &&
                            float.TryParse(parts[2], out float z))
                        {
                            deco.transform.position = new Vector3(x, y, z);
                            Debug.Log($"[HomeManager] 加载装饰物位置: {deco.decorationId} = {deco.transform.position}");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 添加新的装饰物
        /// </summary>
        public void AddDecoration(HomeDecoration decoration)
        {
            if (!decorations.Contains(decoration))
            {
                decorations.Add(decoration);
            }
        }
        
        /// <summary>
        /// 移除装饰物
        /// </summary>
        public void RemoveDecoration(HomeDecoration decoration)
        {
            decorations.Remove(decoration);
        }
    }
}
