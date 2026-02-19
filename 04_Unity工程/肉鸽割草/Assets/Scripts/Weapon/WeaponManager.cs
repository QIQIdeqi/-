using UnityEngine;
using System.Collections.Generic;

namespace GeometryWarrior
{
    /// <summary>
    /// WeaponManager - Singleton, manages all weapon instances
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        public static WeaponManager Instance { get; private set; }
        
        [Header("[Weapon Prefabs]")]
        [SerializeField] private List<GameObject> weaponPrefabObjects = new List<GameObject>();
        
        [Header("[Active Weapons]")]
        [SerializeField] private List<WeaponBase> activeWeapons = new List<WeaponBase>();
        
        private Transform playerTransform;
        
        public List<WeaponBase> ActiveWeapons => activeWeapons;
        
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
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
        
        /// <summary>
        /// Add a weapon by prefab GameObject
        /// </summary>
        public bool AddWeaponFromPrefab(GameObject prefab)
        {
            if (prefab == null) return false;
            
            // Check if already has this weapon type
            WeaponBase prefabWeapon = prefab.GetComponent<WeaponBase>();
            if (prefabWeapon != null && HasWeaponByTypeName(prefabWeapon.GetType().Name))
            {
                return false;
            }
            
            // Instantiate weapon
            GameObject weaponObj = Instantiate(prefab);
            weaponObj.name = prefab.name;
            
            // Set parent after instantiation (fixes persistent error)
            if (playerTransform != null)
            {
                weaponObj.transform.SetParent(playerTransform, false);
            }
            
            WeaponBase weapon = weaponObj.GetComponent<WeaponBase>();
            if (weapon != null)
            {
                activeWeapons.Add(weapon);
                return true;
            }
            
            Destroy(weaponObj);
            return false;
        }
        
        /// <summary>
        /// Add weapon by type name (for upgrade system)
        /// </summary>
        public bool AddWeaponByTypeName(string typeName)
        {
            foreach (GameObject prefab in weaponPrefabObjects)
            {
                if (prefab == null) continue;
                
                WeaponBase weaponBase = prefab.GetComponent<WeaponBase>();
                if (weaponBase != null && weaponBase.GetType().Name == typeName)
                {
                    return AddWeaponFromPrefab(prefab);
                }
            }
            return false;
        }
        
        /// <summary>
        /// Check if player has a weapon of specific type
        /// </summary>
        public bool HasWeaponByTypeName(string typeName)
        {
            foreach (WeaponBase weapon in activeWeapons)
            {
                if (weapon != null && weapon.GetType().Name == typeName)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Get weapon by type name
        /// </summary>
        public WeaponBase GetWeaponByTypeName(string typeName)
        {
            foreach (WeaponBase weapon in activeWeapons)
            {
                if (weapon != null && weapon.GetType().Name == typeName)
                {
                    return weapon;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Get all available weapon type names from prefabs
        /// </summary>
        public List<string> GetAvailableWeaponTypes()
        {
            List<string> types = new List<string>();
            foreach (GameObject prefab in weaponPrefabObjects)
            {
                if (prefab == null) continue;
                
                WeaponBase weapon = prefab.GetComponent<WeaponBase>();
                if (weapon != null)
                {
                    types.Add(weapon.GetType().Name);
                }
            }
            return types;
        }
        
        /// <summary>
        /// Upgrade a weapon by type name
        /// </summary>
        public bool UpgradeWeapon(string typeName)
        {
            WeaponBase weapon = GetWeaponByTypeName(typeName);
            if (weapon != null && weapon.Level < weapon.MaxLevel)
            {
                weapon.Upgrade();
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Clear all weapons (for game restart)
        /// </summary>
        public void ClearAllWeapons()
        {
            foreach (WeaponBase weapon in activeWeapons)
            {
                if (weapon != null)
                {
                    Destroy(weapon.gameObject);
                }
            }
            activeWeapons.Clear();
        }
        
        /// <summary>
        /// Get weapon prefab GameObject by type name
        /// </summary>
        public GameObject GetWeaponPrefab(string typeName)
        {
            foreach (GameObject prefab in weaponPrefabObjects)
            {
                if (prefab == null) continue;
                
                WeaponBase weapon = prefab.GetComponent<WeaponBase>();
                if (weapon != null && weapon.GetType().Name == typeName)
                {
                    return prefab;
                }
            }
            return null;
        }
    }
}
