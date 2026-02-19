using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace GeometryWarrior
{
    /// <summary>
    /// UpgradeUI - UI for upgrade selection (3-choice system)
    /// </summary>
    public class UpgradeUI : MonoBehaviour
    {
        [Header("[UI Elements]")]
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private Transform optionsContainer;
        [SerializeField] private GameObject optionPrefab;
        
        [Header("[Animation]")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float optionDelay = 0.1f;
        
        private List<UpgradeOptionUI> activeOptions = new List<UpgradeOptionUI>();
        private Action<UpgradeData> onSelectCallback;
        private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            if (upgradePanel != null)
            {
                upgradePanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Show upgrade options
        /// </summary>
        public void ShowUpgradeOptions(List<UpgradeData> upgrades, Action<UpgradeData> onSelect)
        {
            Debug.Log($"UpgradeUI: Showing {upgrades.Count} upgrade options");
            
            onSelectCallback = onSelect;
            
            // Check required fields
            if (upgradePanel == null)
            {
                Debug.LogError("UpgradeUI: Upgrade Panel is not assigned!");
                return;
            }
            if (optionsContainer == null)
            {
                Debug.LogError("UpgradeUI: Options Container is not assigned!");
                return;
            }
            if (optionPrefab == null)
            {
                Debug.LogError("UpgradeUI: Option Prefab is not assigned!");
                return;
            }
            
            // Clear previous options
            ClearOptions();
            
            // Show panel
            upgradePanel.SetActive(true);
            
            // Fade in
            StartCoroutine(FadeIn());
            
            // Create option buttons
            for (int i = 0; i < upgrades.Count; i++)
            {
                CreateOption(upgrades[i], i);
            }
        }
        
        /// <summary>
        /// Hide upgrade UI
        /// </summary>
        public void Hide()
        {
            StartCoroutine(FadeOut());
        }
        
        /// <summary>
        /// Create a single option button
        /// </summary>
        private void CreateOption(UpgradeData upgrade, int index)
        {
            if (optionPrefab == null || optionsContainer == null) return;
            
            GameObject optionObj = Instantiate(optionPrefab, optionsContainer);
            UpgradeOptionUI optionUI = optionObj.GetComponent<UpgradeOptionUI>();
            
            if (optionUI != null)
            {
                optionUI.Setup(upgrade, () => OnOptionSelected(upgrade));
                optionUI.SetDelay(index * optionDelay);
                activeOptions.Add(optionUI);
            }
        }
        
        /// <summary>
        /// Clear all option buttons
        /// </summary>
        private void ClearOptions()
        {
            foreach (UpgradeOptionUI option in activeOptions)
            {
                if (option != null && option.gameObject != null)
                {
                    Destroy(option.gameObject);
                }
            }
            activeOptions.Clear();
        }
        
        /// <summary>
        /// Handle option selection
        /// </summary>
        private void OnOptionSelected(UpgradeData upgrade)
        {
            onSelectCallback?.Invoke(upgrade);
        }
        
        private System.Collections.IEnumerator FadeIn()
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;
            
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = elapsed / fadeInDuration;
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        private System.Collections.IEnumerator FadeOut()
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;
            
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = startAlpha * (1f - elapsed / fadeInDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
            
            if (upgradePanel != null)
            {
                upgradePanel.SetActive(false);
            }
            
            ClearOptions();
        }
    }
}
