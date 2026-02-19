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
        
        private void LateUpdate()
        {
            if (target == null)
            {
                // Try to find player
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                    target = player.transform;
                else
                    return;
            }
            
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
