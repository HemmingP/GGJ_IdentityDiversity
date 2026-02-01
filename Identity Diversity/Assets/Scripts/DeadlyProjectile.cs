using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeadlyProjectile : MonoBehaviour
{
    [Range(0f, 1f)]
    public float healthLoss = .25f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.ChangeHealth(-healthLoss);
        }
        Destroy(gameObject);
    }
}
