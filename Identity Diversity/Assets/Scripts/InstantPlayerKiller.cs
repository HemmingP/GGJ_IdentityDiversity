using UnityEngine;

public class InstantPlayerKiller : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.ChangeHealth(-1f);
        }
    }
}
