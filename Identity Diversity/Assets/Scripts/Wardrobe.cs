using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using System;

public enum ElementalType
{
    None,
    Water,
    Fire,
    Gas,
    Cold,
}

[System.Serializable]
public struct WardrobeItems
{
    public ElementalType ElementalType;
    public GameObject Head;
    public GameObject Body;
    public GameObject LeftArm;
    public GameObject LeftLeg;
    public GameObject RightArm;
    public GameObject RightLeg;
    [Range(0f, 1f)]
    public float mobilityModifier;
    // Remember, the formula to calculate damage taken is:
    // damageTaken = baseDamage / (1 - hazardResistance)
    [Range(0f, 1f)]
    public float hazardResistance;
    public bool canBeUsed;
}


public class Wardrobe : MonoBehaviour
{
    [SerializeField]
    public List<WardrobeItems> WardrobeItemsList = new List<WardrobeItems>();
    private ElementalType currentElementalType = ElementalType.None;
    public ElementalType CurrentElementalType => currentElementalType;
    private bool progressInChanging = false;
    public bool IsVulnerable => currentElementalType == ElementalType.None || progressInChanging;

    [SerializeField]
    private VariantSoundPlayer changeClothesSoundPlayer;

    void Start()
    {
        ChangeClothesTo(ElementalType.None);
        UIElementOptionsManager uIElementOptionsManager = UIElementOptionsManager.Instance();
        if (uIElementOptionsManager != null)
        {
            uIElementOptionsManager.UpdateStatuses(WardrobeItemsList.ToDictionary(item => item.ElementalType, item => item));
        }
    }

    public WardrobeItems GetWardrobeItemsByType(ElementalType type)
    {
        return WardrobeItemsList.FirstOrDefault(item => item.ElementalType == type);
    }

    public void ChangeClothesTo(ElementalType newType)
    {
        if (progressInChanging) return;

        WardrobeItems newItem = WardrobeItemsList.FirstOrDefault(item => item.ElementalType == newType);
        if (newItem.canBeUsed == false) return;

        // StartCoroutine(ChangeClothes(newType));
        StartCoroutine(ChangeClothes(
            WardrobeItemsList.FirstOrDefault(item => item.ElementalType == currentElementalType),
            newItem
        ));
    }

    // IEnumerator ChangeClothes(ElementalType newType)
    IEnumerator ChangeClothes(WardrobeItems previousItem, WardrobeItems newItem)
    {
        print($"Changing clothes from {previousItem.ElementalType} to {newItem.ElementalType}");
        progressInChanging = true;

        if (previousItem.ElementalType == newItem.ElementalType)
        {
            currentElementalType = ElementalType.None;
        }
        else
        {
            currentElementalType = newItem.ElementalType;
        }

        UIElementOptionsManager uIElementOptionsManager = UIElementOptionsManager.Instance();
        if (uIElementOptionsManager != null)
        {
            uIElementOptionsManager.SetActiveElement(newItem.ElementalType);
        }

        if (previousItem.ElementalType != ElementalType.None)
        {
            EquipClothing(previousItem.LeftLeg, false);
            EquipClothing(previousItem.RightLeg, false);
            changeClothesSoundPlayer.PlayRandomSound();
            yield return new WaitForSeconds(0.5f);
            EquipClothing(previousItem.Body, false);
            EquipClothing(previousItem.LeftArm, false);
            EquipClothing(previousItem.RightArm, false);
            changeClothesSoundPlayer.PlayRandomSound();
            yield return new WaitForSeconds(0.5f);
            EquipClothing(previousItem.Head, false);
            changeClothesSoundPlayer.PlayRandomSound();
            if (currentElementalType != ElementalType.None)
            {
                yield return new WaitForSeconds(0.5f);
            }
        }

        if (currentElementalType != ElementalType.None)
        {
            EquipClothing(newItem.Head, true);
            changeClothesSoundPlayer.PlayRandomSound();
            yield return new WaitForSeconds(0.5f);
            EquipClothing(newItem.Body, true);
            EquipClothing(newItem.LeftArm, true);
            EquipClothing(newItem.RightArm, true);
            changeClothesSoundPlayer.PlayRandomSound();
            yield return new WaitForSeconds(0.5f);
            EquipClothing(newItem.LeftLeg, true);
            EquipClothing(newItem.RightLeg, true);
            changeClothesSoundPlayer.PlayRandomSound();
        }

        progressInChanging = false;
    }

    private void EquipClothing(GameObject wardrobeItem, bool equip = true)
    {
        if (wardrobeItem == null) return;
        wardrobeItem.SetActive(equip);
    }

    public void UnlockElementalType(ElementalType type)
    {
        for (int i = 0; i < WardrobeItemsList.Count; i++)
        {
            if (WardrobeItemsList[i].ElementalType == type)
            {
                WardrobeItems item = WardrobeItemsList[i];
                item.canBeUsed = true;
                WardrobeItemsList[i] = item;
                break;
            }
        }

        UIElementOptionsManager uIElementOptionsManager = UIElementOptionsManager.Instance();
        if (uIElementOptionsManager != null)
        {
            uIElementOptionsManager.UpdateStatuses(WardrobeItemsList.ToDictionary(item => item.ElementalType, item => item));
        }
    }
}
