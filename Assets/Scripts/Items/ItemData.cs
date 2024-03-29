using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable
}

public enum ConsumableType
{
    Hunger,
    Thirst,
    Health,
    Sleep
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    public string id;

    [Header("Info")]
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;
    public string displayName;
    public string description;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;

    [Header("Equip")]
    public GameObject equipPrefab;
}

[System.Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}
