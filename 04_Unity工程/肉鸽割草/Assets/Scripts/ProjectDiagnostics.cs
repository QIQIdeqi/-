using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// 诊断工具 - 检查常见配置问题
    /// </summary>
    public class ProjectDiagnostics : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void CheckOnStart()
        {
            Debug.Log("=== 项目诊断开始 ===");
            
            // 检查Layer
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            if (obstacleLayer == -1)
            {
                Debug.Log("⚠️ Layer 'Obstacle' 未定义（可选，没有障碍物时可忽略）");
            }
            else
            {
                Debug.Log($"✅ Obstacle Layer: {obstacleLayer}");
            }
            
            Debug.Log("=== 项目诊断结束 ===");
        }
    }
}
