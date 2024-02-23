using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingRecipeUi : MonoBehaviour
{
    public BuildingRecipe recipe;
    public Image backgroundImage;
    public Image icon;
    public TextMeshProUGUI buildName;
    public Image[] resourceCost;
    public Color canBuildColor;
    public Color cannotBuildColor;

    private bool canBuild;

    private void Start()
    {
        icon.sprite = recipe.icon;
        buildName.text = recipe.displayName;

        for (int x = 0; x < resourceCost.Length; x++)
        {
            if (x < recipe.cost.Length)
            {
                resourceCost[x].gameObject.SetActive(true);

                resourceCost[x].sprite = recipe.cost[x].item.icon;
                resourceCost[x].transform.GetComponentInChildren<TextMeshProUGUI>().text = recipe.cost[x].quantity.ToString();
            }
            else
                resourceCost[x].gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        UpdateCanCraft();
    }

    private void UpdateCanCraft()
    {
        canBuild = true;

        for (int i = 0; i < recipe.cost.Length; i++)
        {
            if(!Inventory.instance.HasItems(recipe.cost[i].item, recipe.cost[i].quantity))
            {
                canBuild = false;
                break;
            }
        }

        backgroundImage.color = canBuild ? canBuildColor : cannotBuildColor;
    }

    public void OnClickButton()
    {
        if (canBuild)
        {
            EquipBuildingKit.instance.SetNewBuildingRecipe(recipe);
        }
        else
        {
            PlayerController.instance.ToggleCursor(true);
            gameObject.SetActive(false);
        }
    }
}
