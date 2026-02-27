using System.Collections.Generic;
using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 装扮管理器 - 管理所有装扮部件的解锁和装备
    /// 替换原有的 SkinManager，支持分部件换装
    /// </summary>
    public class OutfitManager : MonoBehaviour
    {
        public static OutfitManager Instance { get; private set; }
        
        [Header("【所有部件】")]
        [Tooltip("所有可用的装扮部件列表，可手动配置或自动加载")]
        [SerializeField] private List<OutfitPartData> allParts = new List<OutfitPartData>();
        
        [Header("【默认部件】")]
        [Tooltip("新玩家默认装备的部件")]
        [SerializeField] private List<OutfitPartData> defaultParts = new List<OutfitPartData>();
        
        // 当前装备的部件（按类别存储）
        private Dictionary<OutfitCategory, OutfitPartData> equippedParts = new Dictionary<OutfitCategory, OutfitPartData>();
        
        // 已解锁的部件ID列表
        private HashSet<string> unlockedParts = new HashSet<string>();
        
        // 保存键
        private const string SAVE_KEY = "OutfitManagerData";
        private const string EQUIPPED_PARTS_KEY = "EquippedParts";
        
        public System.Action OnOutfitChanged;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 如果没有手动配置，自动查找所有
            if (allParts == null || allParts.Count == 0)
            {
                LoadAllPartsFromResources();
            }
            
            LoadData();
        }
        
        /// <summary>
        /// 从 Resources 文件夹自动加载所有部件
        /// </summary>
        private void LoadAllPartsFromResources()
        {
            allParts = new List<OutfitPartData>();
            
            Debug.Log("[OutfitManager] 开始从 Resources/OutfitParts 加载部件...");
            
            // 方法1: 使用 Resources.LoadAll
            OutfitPartData[] parts = Resources.LoadAll<OutfitPartData>("OutfitParts");
            Debug.Log($"[OutfitManager] Resources.LoadAll 返回数组长度: {parts?.Length ?? 0}");
            
            // 方法2: 手动加载每个文件（如果方法1失败）
            if (parts == null || parts.Length == 0 || AllElementsNull(parts))
            {
                Debug.Log("[OutfitManager] 尝试手动加载...");
                LoadPartsManually();
            }
            else
            {
                foreach (var part in parts)
                {
                    if (part != null)
                    {
                        allParts.Add(part);
                        Debug.Log($"[OutfitManager] 加载部件: {part.partId}");
                    }
                }
            }
            
            Debug.Log($"[OutfitManager] 共加载 {allParts.Count} 个有效部件");
        }
        
        private bool AllElementsNull(OutfitPartData[] array)
        {
            if (array == null) return true;
            foreach (var item in array)
            {
                if (item != null) return false;
            }
            return true;
        }
        
        private void LoadPartsManually()
        {
            #if UNITY_EDITOR
            // 编辑器模式下直接读取文件
            string path = Application.dataPath + "/Resources/OutfitParts";
            if (System.IO.Directory.Exists(path))
            {
                var files = System.IO.Directory.GetFiles(path, "*.asset");
                Debug.Log($"[OutfitManager] 手动扫描找到 {files.Length} 个 .asset 文件");
                
                foreach (var file in files)
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                    string assetPath = $"Assets/Resources/OutfitParts/{fileName}.asset";
                    
                    var part = UnityEditor.AssetDatabase.LoadAssetAtPath<OutfitPartData>(assetPath);
                    if (part != null)
                    {
                        allParts.Add(part);
                        Debug.Log($"[OutfitManager] 手动加载: {part.partId}");
                    }
                    else
                    {
                        Debug.LogWarning($"[OutfitManager] 无法加载: {assetPath}");
                    }
                }
            }
            #else
            // 运行时使用备用方案：硬编码加载已知部件
            Debug.LogWarning("[OutfitManager] 运行时使用备用加载方案");
            #endif
        }
        
        /// <summary>
        /// 获取所有部件（返回副本，防止外部修改）
        /// </summary>
        public List<OutfitPartData> GetAllParts()
        {
            // 过滤掉 null 元素
            return allParts.FindAll(p => p != null);
        }
        
        /// <summary>
        /// 按类别获取部件
        /// </summary>
        public List<OutfitPartData> GetPartsByCategory(OutfitCategory category)
        {
            return allParts.FindAll(p => p.category == category);
        }
        
        /// <summary>
        /// 检查部件是否解锁
        /// </summary>
        public bool IsPartUnlocked(OutfitPartData part)
        {
            if (part == null) return false;
            if (part.isUnlockedByDefault) return true;
            return unlockedParts.Contains(part.partId);
        }
        
        /// <summary>
        /// 解锁部件
        /// </summary>
        public void UnlockPart(OutfitPartData part)
        {
            if (part == null || IsPartUnlocked(part)) return;
            
            unlockedParts.Add(part.partId);
            SaveData();
            
            Debug.Log($"[OutfitManager] 解锁部件: {part.partName}");
        }
        
        /// <summary>
        /// 装备部件
        /// </summary>
        public void EquipPart(OutfitPartData part)
        {
            if (part == null || !IsPartUnlocked(part))
            {
                Debug.LogWarning($"[OutfitManager] 无法装备部件: 未解锁或为空");
                return;
            }
            
            // 同一类别只能装备一个
            equippedParts[part.category] = part;
            SaveData();
            
            Debug.Log($"[OutfitManager] 装备部件: {part.partName}");
            OnOutfitChanged?.Invoke();
        }
        
        /// <summary>
        /// 卸下某类别的部件
        /// </summary>
        public void UnequipPart(OutfitCategory category)
        {
            if (equippedParts.ContainsKey(category))
            {
                equippedParts.Remove(category);
                SaveData();
                Debug.Log($"[OutfitManager] 卸下类别: {category}");
                OnOutfitChanged?.Invoke();
            }
        }
        
        /// <summary>
        /// 获取当前装备的部件（按类别）
        /// </summary>
        public OutfitPartData GetEquippedPart(OutfitCategory category)
        {
            if (equippedParts.TryGetValue(category, out OutfitPartData part))
            {
                return part;
            }
            return null;
        }
        
        /// <summary>
        /// 获取所有已装备的部件
        /// </summary>
        public Dictionary<OutfitCategory, OutfitPartData> GetAllEquippedParts()
        {
            return new Dictionary<OutfitCategory, OutfitPartData>(equippedParts);
        }
        
        /// <summary>
        /// 检查并自动解锁（基于等级）
        /// </summary>
        public void CheckUnlockConditions(int playerLevel)
        {
            foreach (var part in allParts)
            {
                if (IsPartUnlocked(part)) continue;
                
                // 检查等级解锁
                if (part.unlockLevel > 0 && playerLevel >= part.unlockLevel)
                {
                    UnlockPart(part);
                }
            }
        }
        
        /// <summary>
        /// 加载保存的数据
        /// </summary>
        private void LoadData()
        {
            // 加载已解锁的部件
            string savedData = PlayerPrefs.GetString(SAVE_KEY, "");
            if (!string.IsNullOrEmpty(savedData))
            {
                string[] partIds = savedData.Split(',');
                foreach (var id in partIds)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        unlockedParts.Add(id);
                    }
                }
            }
            
            // 加载已装备的部件
            string equippedData = PlayerPrefs.GetString(EQUIPPED_PARTS_KEY, "");
            if (!string.IsNullOrEmpty(equippedData))
            {
                string[] entries = equippedData.Split(';');
                foreach (var entry in entries)
                {
                    string[] parts = entry.Split(':');
                    if (parts.Length == 2)
                    {
                        if (int.TryParse(parts[0], out int categoryInt))
                        {
                            OutfitCategory category = (OutfitCategory)categoryInt;
                            OutfitPartData part = allParts.Find(p => p.partId == parts[1]);
                            if (part != null && IsPartUnlocked(part))
                            {
                                equippedParts[category] = part;
                            }
                        }
                    }
                }
            }
            
            // 如果没有装备任何部件，装备默认部件
            if (equippedParts.Count == 0 && defaultParts != null)
            {
                foreach (var part in defaultParts)
                {
                    if (part != null && IsPartUnlocked(part))
                    {
                        equippedParts[part.category] = part;
                    }
                }
            }
            
            Debug.Log($"[OutfitManager] 加载完成，已解锁 {unlockedParts.Count} 个部件，已装备 {equippedParts.Count} 个");
        }
        
        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveData()
        {
            // 保存已解锁的部件
            string saveData = string.Join(",", unlockedParts);
            PlayerPrefs.SetString(SAVE_KEY, saveData);
            
            // 保存已装备的部件
            List<string> equippedEntries = new List<string>();
            foreach (var kvp in equippedParts)
            {
                equippedEntries.Add($"{(int)kvp.Key}:{kvp.Value.partId}");
            }
            string equippedData = string.Join(";", equippedEntries);
            PlayerPrefs.SetString(EQUIPPED_PARTS_KEY, equippedData);
            
            PlayerPrefs.Save();
        }
    }
}
