using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting Recipe", menuName = "New Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public ItemData itemToCraft;
    public RecourceCost[] cost;
}

[System.Serializable]
public class RecourceCost
{
    public ItemData item;
    public int quantity;
}
