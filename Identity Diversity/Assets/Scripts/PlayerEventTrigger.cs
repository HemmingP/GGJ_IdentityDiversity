using UnityEngine;
using UnityEngine.Events;

public class PlayerEventTrigger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onPlayerTrigger;
    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            onPlayerTrigger?.Invoke();
        }
    }
}
