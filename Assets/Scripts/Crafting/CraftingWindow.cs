using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingWindow : MonoBehaviour
{
    public CraftingRecipeUI[] recipeUis;

    public static CraftingWindow instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        Inventory.instance.onOpenInventory.AddListener(OnOpenInventory);
    }

    private void OnDisable()
    {
        Inventory.instance.onOpenInventory.RemoveListener(OnOpenInventory);
    }

    private void OnOpenInventory()
    {
        gameObject.SetActive(false);
    }

    public void Craft (CraftingRecipe recipe)
    {
        //remove require items after craft
        for (int i = 0; i < recipe.cost.Length; i++)
        {
            for (int x = 0; x < recipe.cost[i].quantity; x++)
            {
                Inventory.instance.RemoveItem(recipe.cost[i].item);
            }
        }

        Inventory.instance.AddItem(recipe.itemToCraft);

        for (int i = 0; i < recipeUis.Length; i++)
        {
            recipeUis[i].UpdateCanCraft();
        }
    }
}
