using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //player location
    public SVector3 playerPosition;
    public SVector3 playerRotation;
    public SVector3 playerLook;

    //player needs
    public float health;
    public float hunger;
    public float thirst;
    public float sleep;

    //inventory
    public SInventorySlot[] inventory;

    //dropped items
    public SDroppedItem[] droppedItems;

    //buildings
    public SBuilding[] buildings;

    //resources
    public SResource[] resources;

    //npcs
    public SNpc[] npcs;

    //time
    public float timeOfDay;
}

[System.Serializable]
public struct SVector3
{
    public float x;
    public float y;
    public float z;

    public SVector3 (Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 GetVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public struct SInventorySlot
{
    public bool occupied;
    public string itemId;
    public int quantity;
    public bool equipeed;
}

[System.Serializable]
public struct SDroppedItem
{
    public string itemId;
    public SVector3 position;
    public SVector3 rotation;
}

[System.Serializable]
public struct SBuilding
{
    public string buildingId;
    public SVector3 position;
    public SVector3 rotation;
    public string customProperties;
}

[System.Serializable]
public struct SResource
{
    public int index;
    public bool destroyed;
    public int capacity;
}

[System.Serializable]
public struct SNpc
{
    public string prefabId;
    public SVector3 position;
    public SVector3 rotation;
    public int aiState;
    public bool hasAgentDestination;
    public SVector3 agentDestination;
}
