using UnityEngine;
using System.Collections.Generic;
using GeometryWarrior;

namespace FluffyGeometry.Home
{
    /// <summary>
    /// 家具库存管理器 - 管理背包中家具的数量
    /// </summary>
    public class FurnitureInventory : MonoBehaviour
    {
        public static FurnitureInventory Instance { get; private set; }
        
        [Header("【调试】")]
        [Tooltip("初始家具数量（测试用）")]
        public List<FurnitureInitialData> initialFurniture = new List<FurnitureInitialData>();
        
        [Space(10)]
        [Tooltip("勾选后自动清除所有库存数据（运行时生效）")]
        public bool clearAllData = false;
        
        // 家具数量存储: key=furnitureId, value=数量
        private Dictionary<string, int> furnitureCounts = new Dictionary<string, int>();
        
        // 已放置的家具计数（每个ID已放置多少个）
        private Dictionary<string, int> placedCounts = new Dictionary<string, int>();
        
        private const string INVENTORY_SAVE_KEY = "FurnitureInventory";
        private const string PLACED_SAVE_KEY = "FurniturePlaced";
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            Debug.Log("[FurnitureInventory] Awake - 开始初始化");
            LoadInventory();
            
            // 打印当前库存状态
            foreach (var kvp in furnitureCounts)
            {
                int placed = GetPlacedCount(kvp.Key);
                Debug.Log($"[FurnitureInventory] 库存: {kvp.Key} = 总计{kvp.Value}, 已放置{placed}, 可用{kvp.Value - placed}");
            }
        }
        
        private void Update()
        {
            // 检查是否请求清除数据
            if (clearAllData)
            {
                clearAllData = false; // 重置按钮状态
                
                // 先清除场景中的家具
                if (HomeManager.Instance != null)
                {
                    HomeManager.Instance.ClearAllFurniture();
                }
                
                // 再清除库存数据
                ClearInventory();
                
                // 重新初始化
                if (initialFurniture.Count > 0)
                {
                    ApplyInitialFurnitureData();
                }
                
                Debug.Log("[FurnitureInventory] 所有数据已清除并重新初始化");
            }
        }
        
        /// <summary>
        /// 获取家具当前可用数量
        /// </summary>
        public int GetAvailableCount(string furnitureId)
        {
            int total = GetTotalCount(furnitureId);
            int placed = GetPlacedCount(furnitureId);
            return Mathf.Max(0, total - placed);
        }
        
        /// <summary>
        /// 获取家具总数量（拥有数量）
        /// </summary>
        public int GetTotalCount(string furnitureId)
        {
            if (furnitureCounts.TryGetValue(furnitureId, out int count))
            {
                return count;
            }
            return 0;
        }
        
        /// <summary>
        /// 获取已放置数量
        /// </summary>
        public int GetPlacedCount(string furnitureId)
        {
            if (placedCounts.TryGetValue(furnitureId, out int count))
            {
                return count;
            }
            return 0;
        }
        
        /// <summary>
        /// 添加家具到背包
        /// </summary>
        public void AddFurniture(string furnitureId, int amount = 1)
        {
            if (furnitureCounts.ContainsKey(furnitureId))
            {
                furnitureCounts[furnitureId] += amount;
            }
            else
            {
                furnitureCounts[furnitureId] = amount;
            }
            SaveInventory();
            Debug.Log($"[FurnitureInventory] 添加家具: {furnitureId} +{amount}, 总计: {furnitureCounts[furnitureId]}");
        }
        
        /// <summary>
        /// 是否可以摆放（还有剩余未摆放的）
        /// </summary>
        public bool CanPlace(string furnitureId)
        {
            return GetAvailableCount(furnitureId) > 0;
        }
        
        /// <summary>
        /// 摆放家具时调用 - 增加已放置计数
        /// </summary>
        public void OnFurniturePlaced(string furnitureId)
        {
            if (placedCounts.ContainsKey(furnitureId))
            {
                placedCounts[furnitureId]++;
            }
            else
            {
                placedCounts[furnitureId] = 1;
            }
            SaveInventory();
            Debug.Log($"[FurnitureInventory] 摆放家具: {furnitureId}, 已放置: {placedCounts[furnitureId]}, 剩余可用: {GetAvailableCount(furnitureId)}");
        }
        
        /// <summary>
        /// 移除已摆放的家具
        /// </summary>
        public void OnFurnitureRemoved(string furnitureId)
        {
            if (placedCounts.ContainsKey(furnitureId) && placedCounts[furnitureId] > 0)
            {
                placedCounts[furnitureId]--;
                SaveInventory();
                Debug.Log($"[FurnitureInventory] 移除家具: {furnitureId}, 已放置: {placedCounts[furnitureId]}, 剩余可用: {GetAvailableCount(furnitureId)}");
            }
        }
        
        /// <summary>
        /// 设置家具总数量（用于初始化或合成后）
        /// </summary>
        public void SetFurnitureCount(string furnitureId, int count)
        {
            furnitureCounts[furnitureId] = Mathf.Max(0, count);
            SaveInventory();
        }
        
        /// <summary>
        /// 直接设置已放置数量（用于同步修正）
        /// </summary>
        public void SetPlacedCount(string furnitureId, int count)
        {
            placedCounts[furnitureId] = Mathf.Max(0, count);
            SaveInventory();
        }
        
        /// <summary>
        /// 获取所有已知的家具ID（包括有库存的或有放置记录的）
        /// </summary>
        public List<string> GetAllFurnitureIds()
        {
            HashSet<string> ids = new HashSet<string>();
            foreach (var id in furnitureCounts.Keys) ids.Add(id);
            foreach (var id in placedCounts.Keys) ids.Add(id);
            foreach (var data in initialFurniture) 
            {
                if (!string.IsNullOrEmpty(data.furnitureId))
                    ids.Add(data.furnitureId);
            }
            return new List<string>(ids);
        }
        
        /// <summary>
        /// 保存库存数据
        /// </summary>
        private void SaveInventory()
        {
            // 保存拥有数量
            List<string> saveData = new List<string>();
            foreach (var kvp in furnitureCounts)
            {
                saveData.Add($"{kvp.Key}:{kvp.Value}");
            }
            PlayerPrefs.SetString(INVENTORY_SAVE_KEY, string.Join("|", saveData));
            
            // 保存已放置数量
            List<string> placedData = new List<string>();
            foreach (var kvp in placedCounts)
            {
                placedData.Add($"{kvp.Key}:{kvp.Value}");
            }
            PlayerPrefs.SetString(PLACED_SAVE_KEY, string.Join("|", placedData));
            
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// 加载库存数据
        /// </summary>
        private void LoadInventory()
        {
            furnitureCounts.Clear();
            placedCounts.Clear();
            
            // 加载拥有数量
            string savedData = PlayerPrefs.GetString(INVENTORY_SAVE_KEY, "");
            if (!string.IsNullOrEmpty(savedData))
            {
                string[] entries = savedData.Split('|');
                foreach (var entry in entries)
                {
                    string[] parts = entry.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int count))
                    {
                        furnitureCounts[parts[0]] = count;
                    }
                }
            }
            
            // 加载已放置数量
            string placedData = PlayerPrefs.GetString(PLACED_SAVE_KEY, "");
            if (!string.IsNullOrEmpty(placedData))
            {
                string[] entries = placedData.Split('|');
                foreach (var entry in entries)
                {
                    string[] parts = entry.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int count))
                    {
                        placedCounts[parts[0]] = count;
                    }
                }
            }
            
            // 应用 Inspector 中设置的初始数据
            // 如果 initialFurniture 中有设置，且该家具当前数量为0，则使用初始值
            ApplyInitialFurnitureData();
        }
        
        /// <summary>
        /// 应用 Inspector 中设置的初始家具数据
        /// </summary>
        private void ApplyInitialFurnitureData()
        {
            if (initialFurniture.Count == 0) return;
            
            bool hasChanges = false;
            
            foreach (var data in initialFurniture)
            {
                if (string.IsNullOrEmpty(data.furnitureId)) continue;
                
                // 如果该家具当前总数量为 0，则使用初始值
                int currentTotal = GetTotalCount(data.furnitureId);
                if (currentTotal == 0 && data.initialCount > 0)
                {
                    furnitureCounts[data.furnitureId] = data.initialCount;
                    hasChanges = true;
                    Debug.Log($"[FurnitureInventory] 初始化家具数量: {data.furnitureId} = {data.initialCount}");
                }
            }
            
            if (hasChanges)
            {
                SaveInventory();
            }
        }
        
        /// <summary>
        /// 清除所有数据（调试用）
        /// </summary>
        [ContextMenu("清除库存数据")]
        public void ClearInventory()
        {
            PlayerPrefs.DeleteKey(INVENTORY_SAVE_KEY);
            PlayerPrefs.DeleteKey(PLACED_SAVE_KEY);
            furnitureCounts.Clear();
            placedCounts.Clear();
            Debug.Log("[FurnitureInventory] 库存数据已清除");
        }
    }
    
    /// <summary>
    /// 初始家具数据（用于测试）
    /// </summary>
    [System.Serializable]
    public class FurnitureInitialData
    {
        public string furnitureId;
        public int initialCount = 1;
    }
}
