using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GeometryWarrior
{
    /// <summary>
    /// Individual upgrade option UI element
    /// </summary>
    public class UpgradeOptionUI : MonoBehaviour
    {
        [Header("[UI Elements]")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Text nameTextLegacy;  // Unity default Text
        [SerializeField] private Text descriptionTextLegacy;  // Unity default Text
        [SerializeField] private TextMeshProUGUI nameTextTMP;
        [SerializeField] private TextMeshProUGUI descriptionTextTMP;
        [SerializeField] private Button selectButton;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("[Animation]")]
        [SerializeField] private float scaleInDuration = 0.3f;
        
        private Action onSelectAction;
        
        public void Setup(UpgradeData upgrade, Action onSelect)
        {
            onSelectAction = onSelect;
            
            if (iconImage != null && upgrade.icon != null)
            {
                iconImage.sprite = upgrade.icon;
            }
            
            // Set name text (support both Text and TMP)
            string displayName = upgrade.GetDisplayName();
            if (nameTextLegacy != null)
            {
                nameTextLegacy.text = displayName;
            }
            if (nameTextTMP != null)
            {
                nameTextTMP.text = displayName;
            }
            
            // Set description text (support both Text and TMP)
            string description = upgrade.GetDescription();
            if (descriptionTextLegacy != null)
            {
                descriptionTextLegacy.text = description;
            }
            if (descriptionTextTMP != null)
            {
                descriptionTextTMP.text = description;
            }
            
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => onSelectAction?.Invoke());
            }
        }
        
        public void SetDelay(float delay)
        {
            // Only start coroutine if game object is active
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateIn(delay));
            }
            else
            {
                // If inactive, just set final state
                transform.localScale = Vector3.one;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                }
            }
        }
        
        private System.Collections.IEnumerator AnimateIn(float delay)
        {
            // Wait for delay
            float elapsed = 0f;
            while (elapsed < delay)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            
            // Scale in animation
            elapsed = 0f;
            transform.localScale = Vector3.zero;
            
            while (elapsed < scaleInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / scaleInDuration;
                // Elastic ease out
                t = Mathf.Sin(-13f * (t + 1f) * Mathf.PI / 2f) * Mathf.Pow(2f, -10f * t) + 1f;
                transform.localScale = Vector3.one * t;
                yield return null;
            }
            
            transform.localScale = Vector3.one;
        }
    }
}
