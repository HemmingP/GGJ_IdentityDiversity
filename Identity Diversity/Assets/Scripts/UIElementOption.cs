using UnityEngine;
using UnityEngine.UI;

public class UIElementOption : MonoBehaviour
{
    [SerializeField]
    Image backgroundImage;
    [SerializeField]
    Image iconImage;

    [SerializeField]
    private ElementalType elementalType;
    [SerializeField]
    Color backgroundImageInactive;
    [SerializeField]
    Color backgroundImageActive;
    [SerializeField]
    Color imageInactive;
    [SerializeField]
    Color imageActive;

    public ElementalType ElementalType => elementalType;

    public void SetActiveState(bool isActive)
    {
        if (isActive)
        {
            backgroundImage.color = backgroundImageActive;
            iconImage.color = imageActive;
        }
        else
        {
            backgroundImage.color = backgroundImageInactive;
            iconImage.color = imageInactive;
        }
    }

    public void SetLockedState(bool isLocked)
    {
        if (isLocked)
        {
            backgroundImage.gameObject.SetActive(false);
        }
        else
        {
            backgroundImage.gameObject.SetActive(true);
        }
    }
}
