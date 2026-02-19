using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// Simple Camera Follow - Alternative camera follow script
    /// </summary>
    public class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float followSpeed = 3f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        
        private void Start()
        {
            if (target == null)
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                    target = player.transform;
            }
        }
        
        private void FixedUpdate()
        {
            if (target == null) return;
            
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);
        }
    }
}
