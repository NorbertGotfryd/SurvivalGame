using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : Building, IInteractable
{
    private CraftingWindow craftingWindow;
    private PlayerController player;


    private void Start()
    {
        craftingWindow = FindObjectOfType<CraftingWindow>(true); //FindObjectOfType search only for active objects. true mean we can search inactive object
        player = FindObjectOfType<PlayerController>();
    }

    public string GetInteractPrompt()
    {
        return "Craft";
    }

    public void OnIntereact()
    {
        craftingWindow.gameObject.SetActive(true);
        player.ToggleCursor(true);
    }
}
