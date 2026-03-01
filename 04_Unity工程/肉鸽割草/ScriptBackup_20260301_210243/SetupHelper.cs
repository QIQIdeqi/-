using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// Simple editor helper to setup the scene
    /// Run this in editor to verify setup
    /// </summary>
    public class SetupHelper : MonoBehaviour
    {
        [ContextMenu("验证场景设置")]
        private void ValidateSetup()
        {
            // 检查GameManager
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm == null)
            {
                Debug.LogError("❌ 未找到GameManager！");
            }
            else
            {
                Debug.Log("✅ GameManager已找到");
            }
            
            // 检查CameraFollow
            CameraFollow cf = FindObjectOfType<CameraFollow>();
            if (cf == null)
            {
                Debug.LogError("❌ Camera上没有CameraFollow脚本！请添加Assets/Scripts/Manager/CameraFollow.cs");
            }
            else
            {
                Debug.Log("✅ CameraFollow已找到");
            }
            
            // 检查TileManager
            TileManager tm = FindObjectOfType<TileManager>();
            if (tm == null)
            {
                Debug.LogWarning("⚠️ 未找到TileManager！如需地面Tile，请创建一个空物体并添加TileManager.cs");
            }
            else
            {
                Debug.Log("✅ TileManager已找到");
            }
            
            // 检查Player（游戏中动态创建，所以开始时不应该有）
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc == null)
            {
                Debug.Log("ℹ️ 场景中没有Player（正常，Player由GameManager动态创建）");
            }
            else
            {
                Debug.Log("✅ 场景中有Player实例");
            }
        }
    }
}
