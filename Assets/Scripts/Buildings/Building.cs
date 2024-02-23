using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData data;

    public virtual string GetCustomProperties() //virtual can by overwritten
    {
        return string.Empty;
    }

    public virtual void ReciveCustomProperties(string properties)
    {

    }
}
