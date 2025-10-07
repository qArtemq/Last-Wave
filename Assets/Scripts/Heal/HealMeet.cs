using UnityEngine;

public class HealMeet : MonoBehaviour
{
    [SerializeField] int healAmount = 25;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player_Movement player = other.GetComponent<Player_Movement>();
            if (player != null)
            {
                player.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
