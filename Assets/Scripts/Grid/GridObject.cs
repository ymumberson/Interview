using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    private Building building;

    public void SetBuilding(GameObject new_building)
    {
        if (new_building == building) return;
        if (DestroyBuilding()) Debug.Log("Replaced building");
        building = new_building.GetComponent<Building>();
        PositionBuildingOnSelf();
    }

    public void SetBuildingPosition(Vector3 world_position)
    {
        if (building != null) building.SetPosition(this.transform.position);
    }

    public void PositionBuildingOnSelf()
    {
        if (building != null) building.SetPosition(this.transform.position);
    }
    public Building GetBuilding()
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
