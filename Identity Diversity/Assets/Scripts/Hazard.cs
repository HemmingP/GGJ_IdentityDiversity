using UnityEngine;

public class Hazard : MonoBehaviour
{
    public ElementalType hazardType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.InflictHazardType(hazardType);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.InflictHazardType(ElementalType.None);
        }
    }
}
