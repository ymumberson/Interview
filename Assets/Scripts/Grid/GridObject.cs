using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private GameObject building;

    public void SetBuilding(GameObject new_building)
    {
        if (DestroyBuilding()) Debug.Log("Replaced building");
        building = new_building;
        building.transform.position = this.transform.position;
    }

    public GameObject GetBuilding()
    {
        return building;
    }
    public bool ContainsBuilding()
    {
        return this.building != null;
    }

    public bool DestroyBuilding()
    {
        if (this.ContainsBuilding())
        {
            Destroy(building);
            return true;
        }
        return false;
    }
}
