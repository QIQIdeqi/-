using UnityEngine;

namespace GeometryWarrior
{
    /// <summary>
    /// ExpOrb - Experience orb that players collect to gain EXP
    /// </summary>
    public class ExpOrb : MonoBehaviour
    {
        [SerializeField] private int expValue = 10;
        [SerializeField] private float attractRange = 3f;
        [SerializeField] private float attractSpeed = 8f;
        [SerializeField] private float lifetime = 10f;
        
        private Transform player;
        private bool isAttracting = false;
        
        private void Start()
        {
            PlayerController foundPlayer = FindObjectOfType<PlayerController>();
            if (foundPlayer != null)
                player = foundPlayer.transform;
            
            Destroy(gameObject, lifetime);
        }
        
        private void Update()
        {
            if (player == null) return;
            
            float distance = Vector2.Distance(transform.position, player.position);
            
            // Start attracting when player is close
            if (distance < attractRange)
            {
                isAttracting = true;
            }
            
            if (isAttracting)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, player.position, attractSpeed * Time.deltaTime);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && !player.IsDead)
            {
                player.AddExp(expValue);
                Destroy(gameObject);
            }
        }
    }
}
