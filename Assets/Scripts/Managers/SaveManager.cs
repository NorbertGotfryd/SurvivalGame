using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForEndOfFrame();

        if (PlayerPrefs.HasKey("Save"))
            Load();
    }

    private void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
            Save();

        if (Keyboard.current.mKey.wasPressedThisFrame)
            Load();
    }

    private void Save()
    {
        SaveData data = new SaveData();

        //player location
        data.playerPosition = new SVector3(PlayerController.instance.transform.position);
        data.playerRotation = new SVector3(PlayerController.instance.transform.eulerAngles);
        data.playerLook = new SVector3(PlayerController.instance.cameraContainer.localEulerAngles);

        //player needs
        data.health = PlayerNeeds.instance.health.currentValue;
        data.hunger = PlayerNeeds.instance.hunger.currentValue;
        data.thirst = PlayerNeeds.instance.thirst.currentValue;
        data.sleep = PlayerNeeds.instance.sleep.currentValue;

        //inventory
        data.inventory = new SInventorySlot[Inventory.instance.slots.Length];

        for (int x = 0; x < Inventory.instance.slots.Length; x++)
        {
            data.inventory[x] = new SInventorySlot();
            data.inventory[x].occupied = Inventory.instance.slots[x].item != null;

            if (!data.inventory[x].occupied)
                continue;

            data.inventory[x].itemId = Inventory.instance.slots[x].item.id;
            data.inventory[x].quantity = Inventory.instance.slots[x].quantity;
            data.inventory[x].equipeed = Inventory.instance.uiSlots[x].isEquipped;
        }

        //dropped item
        ItemObject[] droppedItems = FindObjectsOfType<ItemObject>();
        data.droppedItems = new SDroppedItem[droppedItems.Length];

        for (int x = 0; x < droppedItems.Length; x++)
        {
            data.droppedItems[x] = new SDroppedItem();
            data.droppedItems[x].itemId = droppedItems[x].item.id;
            data.droppedItems[x].position = new SVector3(droppedItems[x].transform.position);
            data.droppedItems[x].rotation = new SVector3(droppedItems[x].transform.eulerAngles);
        }

        //buildings
        Building[] buildings = FindObjectsOfType<Building>();
        data.buildings = new SBuilding[buildings.Length];

        for (int x = 0; x < buildings.Length; x++)
        {
            data.buildings[x] = new SBuilding();
            data.buildings[x].buildingId = buildings[x].data.id;
            data.buildings[x].position = new SVector3(buildings[x].transform.position);
            data.buildings[x].rotation = new SVector3(buildings[x].transform.eulerAngles);
            data.buildings[x].customProperties = buildings[x].GetCustomProperties();
        }

        //resources
        data.resources = new SResource[ObjectManager.instance.resources.Length];

        for (int x = 0; x < ObjectManager.instance.resources.Length; x++)
        {
            data.resources[x] = new SResource();
            data.resources[x].index = x;
            data.resources[x].destroyed = ObjectManager.instance.resources[x] == null;

            if (!data.resources[x].destroyed)
                data.resources[x].capacity = ObjectManager.instance.resources[x].capacity;
        }

        //NPCs
        NPC[] npcs = FindObjectsOfType<NPC>();
        data.npcs = new SNpc[npcs.Length];

        for (int x = 0; x < npcs.Length; x++)
        {
            data.npcs[x] = new SNpc();
            data.npcs[x].prefabId = npcs[x].data.id;
            data.npcs[x].position = new SVector3(npcs[x].transform.position);
            data.npcs[x].rotation = new SVector3(npcs[x].transform.eulerAngles);
            data.npcs[x].aiState = (int)npcs[x].aiState;
            data.npcs[x].hasAgentDestination = !npcs[x].agent.isStopped;
            data.npcs[x].agentDestination = new SVector3(npcs[x].agent.destination);
        }

        //time of day
        data.timeOfDay = DayNightCycle.instance.time;

        //convert the save data object to a string
        string rawData = JsonUtility.ToJson(data); //magic

        //save it to our PlayerPrefs
        PlayerPrefs.SetString("Save", rawData);    //more magic
    }

    private void Load()
    {
        SaveData data = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("Save"));

        //player location
        PlayerController.instance.transform.position = data.playerPosition.GetVector3();
        PlayerController.instance.transform.eulerAngles = data.playerRotation.GetVector3();
        PlayerController.instance.cameraContainer.localEulerAngles = data.playerLook.GetVector3();

        //player needs
        PlayerNeeds.instance.health.currentValue = data.health;
        PlayerNeeds.instance.hunger.currentValue = data.hunger;
        PlayerNeeds.instance.thirst.currentValue = data.thirst;
        PlayerNeeds.instance.sleep.currentValue = data.sleep;

        //inventory
        int equippedItem = 999;
        
        for (int x = 0; x < data.inventory.Length; x++)
        {
            if (!data.inventory[x].occupied)
                continue;

            Inventory.instance.slots[x].item = ObjectManager.instance.GetItemByID(data.inventory[x].itemId);
            Inventory.instance.slots[x].quantity = data.inventory[x].quantity;

            if (data.inventory[x].equipeed)
            {
                equippedItem = x;
            }
        }

        if(equippedItem != 999)
          Inventory.instance.SelectItem(equippedItem);
          Inventory.instance.OnEquipButton();

        //destroy all pre existing dropped items
        ItemObject[] droppedItems = FindObjectsOfType<ItemObject>();

        for (int x = 0; x < droppedItems.Length; x++)
            Destroy(droppedItems[x].gameObject);

        //spawn in saved dropped items
        for (int x = 0; x < data.droppedItems.Length; x++)
        {
            GameObject prefab = ObjectManager.instance.GetItemByID(data.droppedItems[x].itemId).dropPrefab;
            Instantiate(prefab, data.droppedItems[x].position.GetVector3(), Quaternion.Euler(data.droppedItems[x].rotation.GetVector3()));
        }

        //buildings
        for (int x = 0; x < data.buildings.Length; x++)
        {
            GameObject prefab = ObjectManager.instance.GetBuildingByID(data.buildings[x].buildingId).spawnPrefab;
            GameObject building = Instantiate(prefab, data.buildings[x].position.GetVector3(), Quaternion.Euler(data.buildings[x].rotation.GetVector3()));
            building.GetComponent<Building>().ReciveCustomProperties(data.buildings[x].customProperties);
        }

        //resources
        for (int x = 0; x < ObjectManager.instance.resources.Length; x++)
        {
            if (data.resources[x].destroyed)
            {
                Destroy(ObjectManager.instance.resources[x].gameObject);
                continue;
            }

            ObjectManager.instance.resources[x].capacity = data.resources[x].capacity;
        }

        //destroy all pre existing npcs
        NPC[] npcs = FindObjectsOfType<NPC>();

        for (int x = 0; x < npcs.Length; x++)
            Destroy(npcs[x].gameObject);

        //spawn in saved npcs
        for (int x = 0; x < data.npcs.Length; x++)
        {
            GameObject prefab = ObjectManager.instance.GetNpcByID(data.npcs[x].prefabId).spawnPrefab;
            GameObject npcObject = Instantiate(prefab, data.npcs[x].position.GetVector3(), Quaternion.Euler(data.npcs[x].rotation.GetVector3()));
            NPC npc = npcObject.GetComponent<NPC>();

            npc.aiState = (AiState)data.npcs[x].aiState;
            npc.agent.isStopped = !data.npcs[x].hasAgentDestination;

            if(!npc.agent.isStopped)
            npc.agent.SetDestination(data.npcs[x].agentDestination.GetVector3());
        }

        //time of day
        DayNightCycle.instance.time = data.timeOfDay;
    }
}
