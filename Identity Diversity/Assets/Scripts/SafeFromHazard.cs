using UnityEngine;

public class SafeFromHazard : MonoBehaviour
{
    [SerializeField]
    private ElementalType hazardType;


    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetHazardTotallySafeFrom(hazardType, true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetHazardTotallySafeFrom(hazardType, false);
        }
    }
}
