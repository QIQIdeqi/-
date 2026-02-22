using System.Collections.Generic;
using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 皮肤管理器 - 管理所有皮肤的解锁和切换
    /// </summary>
    public class SkinManager : MonoBehaviour
    {
        public static SkinManager Instance { get; private set; }
        
        [Header("[皮肤列表]")]
        [SerializeField] private List<SkinData> allSkins = new List<SkinData>();
        
        [Header("[默认皮肤]")]
        [SerializeField] private SkinData defaultSkin;
        
        // 当前装备的皮肤
        public SkinData CurrentSkin { get; private set; }
        
        // 已解锁的皮肤ID列表
        private HashSet<string> unlockedSkins = new HashSet<string>();
        
        // 保存键
        private const string SAVE_KEY = "SkinManagerData";
        private const string CURRENT_SKIN_KEY = "CurrentSkinId";
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 如果没有手动配置皮肤，自动查找所有
            if (allSkins == null || allSkins.Count == 0)
            {
                LoadAllSkinsFromResources();
            }
            
            LoadData();
        }
        
        /// <summary>
        /// 从 Resources 文件夹自动加载所有皮肤
        /// </summary>
        private void LoadAllSkinsFromResources()
        {
            allSkins = new List<SkinData>();
            SkinData[] skins = Resources.LoadAll<SkinData>("Skins");
            
            if (skins != null && skins.Length > 0)
            {
                allSkins.AddRange(skins);
                Debug.Log($"[SkinManager] 自动加载了 {skins.Length} 个皮肤");
                
                // 设置默认皮肤
                if (defaultSkin == null && allSkins.Count > 0)
                {
                    defaultSkin = allSkins[0];
                }
            }
            else
            {
                Debug.LogWarning("[SkinManager] 在 Resources/Skins 中找不到任何皮肤文件！");
            }
        }
        
        /// <summary>
        /// 获取所有皮肤
        /// </summary>
        public List<SkinData> GetAllSkins()
        {
            return allSkins;
        }
        
        /// <summary>
        /// 检查皮肤是否解锁
        /// </summary>
        public bool IsSkinUnlocked(SkinData skin)
        {
            if (skin == null) return false;
            if (skin.isUnlockedByDefault) return true;
            return unlockedSkins.Contains(skin.skinId);
        }
        
        /// <summary>
        /// 解锁皮肤
        /// </summary>
        public void UnlockSkin(SkinData skin)
        {
            if (skin == null || IsSkinUnlocked(skin)) return;
            
            unlockedSkins.Add(skin.skinId);
            SaveData();
            
            Debug.Log($"[SkinManager] 解锁皮肤: {skin.skinName}");
        }
        
        /// <summary>
        /// 装备皮肤
        /// </summary>
        public void EquipSkin(SkinData skin)
        {
            if (skin == null || !IsSkinUnlocked(skin))
            {
                Debug.LogWarning($"[SkinManager] 无法装备皮肤: 未解锁或为空");
                return;
            }
            
            CurrentSkin = skin;
            PlayerPrefs.SetString(CURRENT_SKIN_KEY, skin.skinId);
            PlayerPrefs.Save();
            
            Debug.Log($"[SkinManager] 装备皮肤: {skin.skinName}");
            
            // 通知玩家更新外观
            UpdatePlayerSkin();
        }
        
        /// <summary>
        /// 获取当前皮肤
        /// </summary>
        public SkinData GetCurrentSkin()
        {
            if (CurrentSkin == null)
            {
                CurrentSkin = defaultSkin;
            }
            return CurrentSkin;
        }
        
        /// <summary>
        /// 检查并自动解锁（基于等级、分数等）
        /// </summary>
        public void CheckUnlockConditions(int playerLevel, int playerScore)
        {
            foreach (var skin in allSkins)
            {
                if (IsSkinUnlocked(skin)) continue;
                
                // 检查等级解锁
                if (skin.unlockLevel > 0 && playerLevel >= skin.unlockLevel)
                {
                    UnlockSkin(skin);
                    continue;
                }
                
                // 检查分数解锁
                if (skin.unlockScore > 0 && playerScore >= skin.unlockScore)
                {
                    UnlockSkin(skin);
                    continue;
                }
            }
        }
        
        /// <summary>
        /// 更新玩家外观
        /// </summary>
        private void UpdatePlayerSkin()
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.ApplySkin(CurrentSkin);
            }
        }
        
        /// <summary>
        /// 加载保存的数据
        /// </summary>
        private void LoadData()
        {
            // 加载已解锁的皮肤
            string savedData = PlayerPrefs.GetString(SAVE_KEY, "");
            if (!string.IsNullOrEmpty(savedData))
            {
                string[] skinIds = savedData.Split(',');
                foreach (var id in skinIds)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        unlockedSkins.Add(id);
                    }
                }
            }
            
            // 加载当前装备的皮肤
            string currentSkinId = PlayerPrefs.GetString(CURRENT_SKIN_KEY, "");
            if (!string.IsNullOrEmpty(currentSkinId))
            {
                CurrentSkin = allSkins.Find(s => s.skinId == currentSkinId);
            }
            
            // 如果没有当前皮肤，使用默认
            if (CurrentSkin == null)
            {
                CurrentSkin = defaultSkin;
            }
            
            Debug.Log($"[SkinManager] 加载完成，已解锁 {unlockedSkins.Count} 个皮肤");
        }
        
        /// <summary>
        /// 保存数据
        /// </summary>
        private void SaveData()
        {
            string saveData = string.Join(",", unlockedSkins);
            PlayerPrefs.SetString(SAVE_KEY, saveData);
            PlayerPrefs.Save();
        }
    }
}
