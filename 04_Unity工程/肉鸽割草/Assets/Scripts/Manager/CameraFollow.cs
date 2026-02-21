using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// CameraFollow - Smooth camera follow for player
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        
        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerSpawned += OnPlayerSpawned;
            }
            FindPlayerTarget();
        }
        
        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPlayerSpawned -= OnPlayerSpawned;
            }
        }
        
        private void LateUpdate()
        {
            if (target == null)
            {
                FindPlayerTarget();
                if (target == null) return;
            }
            
            // 计算目标位置
            Vector3 desiredPosition = target.position + offset;
            
            // 平滑移动
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        
        private void OnPlayerSpawned(PlayerController player)
        {
            if (player != null)
            {
                target = player.transform;
                // 立即更新相机位置，避免从远处飞过来
                transform.position = target.position + offset;
                Debug.Log($"CameraFollow: 接收到Player - {target.name}", this);
            }
        }
        
        private void FindPlayerTarget()
        {
            if (GameManager.Instance != null && GameManager.Instance.Player != null)
            {
                target = GameManager.Instance.Player.transform;
                return;
            }
            
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
