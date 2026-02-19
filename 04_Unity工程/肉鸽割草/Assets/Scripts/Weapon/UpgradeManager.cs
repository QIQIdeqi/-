using UnityEngine;
using System.Collections.Generic;

namespace GeometryWarrior
{
    /// <summary>
    /// UpgradeManager - Manages level-up upgrade selection
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }
        
        [Header("[Upgrade Data]")]
        [SerializeField] private List<UpgradeData> allUpgrades = new List<UpgradeData>();
        [SerializeField] private int optionCount = 3;
        
        [Header("[UI]")]
        [SerializeField] private UpgradeUI upgradeUI;
        
        [Header("[Player]")]
        [SerializeField] private PlayerController player;
        
        private WeaponManager weaponManager;
        private bool isUpgradePending;
        
        public bool IsUpgradePending => isUpgradePending;
        
        [Header("[Debug]")]
        [SerializeField] private KeyCode debugKey = KeyCode.U;
        
        private void Update()
        {
            if (Input.GetKeyDown(debugKey))
            {
                Debug.Log("UpgradeManager: Debug key pressed, forcing ShowUpgradeOptions");
                ShowUpgradeOptions();
            }
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Try to get WeaponManager
            weaponManager = WeaponManager.Instance;
            if (weaponManager == null)
            {
                weaponManager = FindObjectOfType<WeaponManager>();
            }
            
            if (weaponManager != null)
            {
                Debug.Log($"UpgradeManager: WeaponManager found - {weaponManager.name}");
            }
            else
            {
                Debug.LogError("UpgradeManager: WeaponManager not found in scene! Please add a GameObject with WeaponManager script.");
            }
            
            // Subscribe to GameManager's player spawn event
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerSpawned += OnPlayerSpawned;
                Debug.Log("UpgradeManager: Subscribed to GameManager.OnPlayerSpawned");
                
                // If player already exists (e.g., in editor), subscribe immediately
                if (GameManager.Instance.Player != null)
                {
                    OnPlayerSpawned(GameManager.Instance.Player);
                }
            }
            else
            {
                Debug.LogWarning("UpgradeManager: GameManager instance not found!");
            }
        }
        
        private void OnPlayerSpawned(PlayerController spawnedPlayer)
        {
            if (player != null)
            {
                // Unsubscribe from old player
                player.OnLevelUp -= OnPlayerLevelUp;
            }
            
            player = spawnedPlayer;
            
            // Subscribe to level up event
            if (player != null)
            {
                player.OnLevelUp += OnPlayerLevelUp;
                Debug.Log($"UpgradeManager: Player spawned and subscribed to level up events");
            }
        }
        
        private void OnDestroy()
        {
            if (player != null)
            {
                player.OnLevelUp -= OnPlayerLevelUp;
            }
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerSpawned -= OnPlayerSpawned;
            }
        }
        
        private void OnPlayerLevelUp(int newLevel)
        {
            Debug.Log($"UpgradeManager: Player leveled up to {newLevel}, showing upgrade options");
            ShowUpgradeOptions();
        }
        
        /// <summary>
        /// Show upgrade selection UI
        /// </summary>
        public void ShowUpgradeOptions()
        {
            if (isUpgradePending) return;
            
            Debug.Log($"UpgradeManager: Getting {optionCount} random upgrades from {allUpgrades.Count} total");
            
            List<UpgradeData> availableUpgrades = GetRandomAvailableUpgrades(optionCount);
            
            Debug.Log($"UpgradeManager: Found {availableUpgrades.Count} available upgrades");
            
            if (availableUpgrades.Count == 0)
            {
                // No upgrades available, give random stat boost
                Debug.Log("No upgrades available");
                return;
            }
            
            isUpgradePending = true;
            
            // Pause game
            Time.timeScale = 0f;
            
            // Show UI
            if (upgradeUI != null)
            {
                upgradeUI.ShowUpgradeOptions(availableUpgrades, OnUpgradeSelected);
            }
            else
            {
                Debug.LogError("UpgradeManager: Upgrade UI is not assigned!");
                // Resume game if UI is missing
                Time.timeScale = 1f;
                isUpgradePending = false;
            }
        }
        
        /// <summary>
        /// Get random available upgrades
        /// </summary>
        private List<UpgradeData> GetRandomAvailableUpgrades(int count)
        {
            List<UpgradeData> available = new List<UpgradeData>();
            
            foreach (UpgradeData upgrade in allUpgrades)
            {
                if (IsUpgradeAvailable(upgrade))
                {
                    available.Add(upgrade);
                }
            }
            
            // Shuffle and take count
            ShuffleList(available);
            
            List<UpgradeData> result = new List<UpgradeData>();
            for (int i = 0; i < Mathf.Min(count, available.Count); i++)
            {
                result.Add(available[i]);
            }
            
            return result;
        }
        
        /// <summary>
        /// Check if an upgrade is currently available
        /// </summary>
        private bool IsUpgradeAvailable(UpgradeData upgrade)
        {
            if (upgrade == null) return false;
            
            // Get WeaponManager instance directly
            WeaponManager wm = weaponManager ?? WeaponManager.Instance;
            
            switch (upgrade.type)
            {
                case UpgradeType.NewWeapon:
                    // Only show if player doesn't have this weapon
                    if (wm != null && !string.IsNullOrEmpty(upgrade.weaponTypeName))
                    {
                        bool hasWeapon = wm.HasWeaponByTypeName(upgrade.weaponTypeName);
                        return !hasWeapon;
                    }
                    return true;
                    
                case UpgradeType.UpgradeWeapon:
                    // Only show if player has this weapon and it's not max level
                    if (wm != null && !string.IsNullOrEmpty(upgrade.weaponTypeName))
                    {
                        WeaponBase weapon = wm.GetWeaponByTypeName(upgrade.weaponTypeName);
                        return weapon != null && weapon.Level < weapon.MaxLevel;
                    }
                    return false;
                    
                case UpgradeType.StatBoost:
                    // Always available
                    return true;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Apply selected upgrade
        /// </summary>
        private void OnUpgradeSelected(UpgradeData selectedUpgrade)
        {
            if (selectedUpgrade == null) return;
            
            ApplyUpgrade(selectedUpgrade);
            
            // Resume game
            Time.timeScale = 1f;
            isUpgradePending = false;
            
            // Hide UI
            if (upgradeUI != null)
            {
                upgradeUI.Hide();
            }
        }
        
        /// <summary>
        /// Apply upgrade effect
        /// </summary>
        private void ApplyUpgrade(UpgradeData upgrade)
        {
            Debug.Log($"ApplyUpgrade: Type={upgrade.type}, Name={upgrade.upgradeName}, WeaponTypeName={upgrade.weaponTypeName}");
            
            // Get WeaponManager instance directly (may not be cached yet)
            WeaponManager wm = weaponManager ?? WeaponManager.Instance;
            
            // If still null, try to find in scene
            if (wm == null)
            {
                wm = FindObjectOfType<WeaponManager>();
                Debug.Log($"ApplyUpgrade: FindObjectOfType<WeaponManager> returned {wm}");
            }
            
            switch (upgrade.type)
            {
                case UpgradeType.NewWeapon:
                    if (wm != null)
                    {
                        Debug.Log($"ApplyUpgrade: Calling AddWeaponByTypeName with '{upgrade.weaponTypeName}'");
                        bool success = wm.AddWeaponByTypeName(upgrade.weaponTypeName);
                        Debug.Log($"ApplyUpgrade: AddWeaponByTypeName returned {success}");
                    }
                    else
                    {
                        Debug.LogError("ApplyUpgrade: WeaponManager is null! Make sure WeaponManager exists in the scene.");
                    }
                    break;
                    
                case UpgradeType.UpgradeWeapon:
                    WeaponManager wm2 = weaponManager ?? WeaponManager.Instance;
                    if (wm2 != null)
                    {
                        wm2.UpgradeWeapon(upgrade.weaponTypeName);
                        Debug.Log($"Upgraded weapon: {upgrade.weaponTypeName}");
                    }
                    break;
                    
                case UpgradeType.StatBoost:
                    ApplyStatBoost(upgrade);
                    break;
            }
        }
        
        /// <summary>
        /// Apply stat boost
        /// </summary>
        private void ApplyStatBoost(UpgradeData upgrade)
        {
            PlayerController p = player ?? FindObjectOfType<PlayerController>();
            if (p == null)
            {
                Debug.LogError("ApplyStatBoost: Player is null!");
                return;
            }
            
            switch (upgrade.statType)
            {
                case StatBoostType.MaxHealth:
                    p.IncreaseMaxHealth(Mathf.RoundToInt(upgrade.statValue));
                    Debug.Log($"Max Health boosted by {upgrade.statValue}");
                    break;
                    
                case StatBoostType.MoveSpeed:
                    p.IncreaseMoveSpeed(upgrade.statValue);
                    Debug.Log($"Move Speed boosted by {upgrade.statValue}");
                    break;
                    
                case StatBoostType.AttackSpeed:
                    p.IncreaseAttackSpeed(upgrade.statValue);
                    Debug.Log($"Attack Speed boosted by {upgrade.statValue}");
                    break;
                    
                case StatBoostType.PickupRange:
                    p.IncreasePickupRange(upgrade.statValue);
                    Debug.Log($"Pickup Range boosted by {upgrade.statValue}");
                    break;
            }
        }
        
        /// <summary>
        /// Shuffle a list using Fisher-Yates algorithm
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
