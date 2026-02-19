using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// TestUpgradeUI - Temporary script for testing upgrade UI
    /// </summary>
    public class TestUpgradeUI : MonoBehaviour
    {
        [SerializeField] private KeyCode testKey = KeyCode.U;
        
        private void Update()
        {
            if (Input.GetKeyDown(testKey))
            {
                Debug.Log("TestUpgradeUI: Manual trigger pressed");
                if (UpgradeManager.Instance != null)
                {
                    // Use reflection to call private method
                    var method = typeof(UpgradeManager).GetMethod("ShowUpgradeOptions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (method != null)
                    {
                        method.Invoke(UpgradeManager.Instance, null);
                    }
                }
                else
                {
                    Debug.LogWarning("TestUpgradeUI: UpgradeManager instance not found!");
                }
            }
        }
    }
}
