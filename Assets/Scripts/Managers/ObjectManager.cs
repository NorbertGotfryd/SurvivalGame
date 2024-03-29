using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [HideInInspector]
    public ItemData[] items;
    [HideInInspector]
    public BuildingData[] buildings;
    [HideInInspector]
    public NPCData[] npcs;
    [HideInInspector]
    public Resource[] resources;

    public static ObjectManager instance;

    private void Awake()
    {
        instance = this;

        //load in all the assets we need
        items = Resources.LoadAll<ItemData>("Items");
        buildings = Resources.LoadAll<BuildingData>("Buildings");
        npcs = Resources.LoadAll<NPCData>("NPCs");
    }

    private void Start()
    {
        //get all of the resources
        resources = FindObjectsOfType<Resource>();
    }

    public ItemData GetItemByID(string id)
    {
        for (int x = 0; x < items.Length; x++)
        {
            if (items[x].id == id)
                return items[x];
        }

        Debug.LogError("No item has been found.");
        return null;
    }

    public BuildingData GetBuildingByID(string id)
    {
        for (int x = 0; x < buildings.Length; x++)
        {
            if(buildings[x].id == id)
                return buildings[x];
        }

        Debug.LogError("No building has been found.");
        return null;
    }

    public NPCData GetNpcByID(string id)
    {
        for (int x = 0; x < npcs.Length; x++)
        {
            if (npcs[x].id == id)
                return npcs[x];
        }

        Debug.LogError("No NPC has been found.");
        return null;
    }
}
