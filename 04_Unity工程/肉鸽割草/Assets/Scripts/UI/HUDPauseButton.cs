using UnityEngine;
using UnityEngine.UI;

namespace GeometryWarrior
{
    /// <summary>
    /// HUD 暂停按钮
    /// </summary>
    public class HUDPauseButton : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        
        private void Awake()
        {
            if (pauseButton == null)
            {
                pauseButton = GetComponent<Button>();
            }
            
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
            }
        }
        
        private void OnPauseClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
        }
        
        private void OnDestroy()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveListener(OnPauseClicked);
            }
        }
    }
}
