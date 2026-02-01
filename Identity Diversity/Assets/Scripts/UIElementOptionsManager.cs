using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElementOptionsManager : MonoBehaviour
{
    private static UIElementOptionsManager instance;
    public static UIElementOptionsManager Instance()
    {
        return instance;
    }

    [SerializeField]
    List<UIElementOption> elementOptions = new List<UIElementOption>();

    public RectMask2D healthMask;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveElement(ElementalType activeType)
    {
        foreach (UIElementOption option in elementOptions)
        {
            option.SetActiveState(false);
        }
        foreach (UIElementOption option in elementOptions)
        {
            if (option.ElementalType == activeType)
                option.SetActiveState(true);
        }
    }

    // float amount is between 0 and 1
    public void SetHealthAmount(float amount)
    {
        Vector4 padding = healthMask.padding;
        padding.x = amount * healthMask.rectTransform.rect.width;
        healthMask.padding = padding;
    }

    public void UpdateStatuses(Dictionary<ElementalType, WardrobeItems> statuses)
    {
        foreach (UIElementOption option in elementOptions)
        {
            if (statuses.ContainsKey(option.ElementalType))
            {
                option.SetLockedState(!statuses[option.ElementalType].canBeUsed);
            }
        }
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
