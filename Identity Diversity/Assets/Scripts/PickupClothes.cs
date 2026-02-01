using UnityEngine;

public class PickupClothes : MonoBehaviour
{
    public ElementalType clothingType;

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Wardrobe wardrobe = player.wardrobe;
            if (wardrobe != null)
            {
                wardrobe.UnlockElementalType(clothingType);
                gameObject.SetActive(false);
            }
        }
    }
}
