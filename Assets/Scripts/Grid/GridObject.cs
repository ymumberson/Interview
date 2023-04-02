using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for storing information within each cell of the Grid object.
/// </summary>
public class GridObject : MonoBehaviour
{
    /// <summary>
    /// The building that occupies this GridObject.
    /// </summary>
    private Building building;

    /// <summary>
    /// Sets the Building.
    /// </summary>
    /// <param name="new_building"></param>
    public void SetBuilding(GameObject new_building)
    {
        if (new_building == building) return;
        if (DestroyBuilding()) Debug.Log("Replaced building");
        building = new_building.GetComponent<Building>();
        PositionBuildingOnSelf();
    }

    /// <summary>
    /// Sets the position of the Building that occupies this GridObject.
    /// </summary>
    /// <param name="world_position"></param>
    public void SetBuildingPosition(Vector3 world_position)
    {
        if (building != null) building.SetPosition(this.transform.position);
    }

    /// <summary>
    /// Repositions the Building that occupies this GridObject onto the transform of this GridObject.
    /// </summary>
    public void PositionBuildingOnSelf()
    {
        if (building != null) building.SetPosition(this.transform.position);
    }

    /// <summary>
    /// Returns the Building that occupies this GridObject.
    /// </summary>
    /// <returns></returns>
    public Building GetBuilding()
    {
        return building;
    }

    /// <summary>
    /// Checks if this GridObject is occupied by a Building.
    /// </summary>
    /// <returns></returns>
    public bool ContainsBuilding()
    {
        return this.building != null;
    }

    /// <summary>
    /// Checks if this GridObject is occupied the given Building.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool ContainsBuilding(Building b)
    {
        return this.building != null && this.building == b;
    }

    /// <summary>
    /// Removes the Building occupying this GridObject.
    /// </summary>
    public void RemoveBuilding()
    {
        this.building = null;
    }

    /// <summary>
    /// Removes the Building occupying this GridObject and destroys the Building gameObject.
    /// </summary>
    public void RemoveAndDestroyBuilding()
    {
        this.DestroyBuilding();
        this.building = null;
    }

    /// <summary>
    /// Destroys the Building gameObject of the Building that occupies this tile.
    /// </summary>
    /// <returns></returns>
    public bool DestroyBuilding()
    {
        if (this.ContainsBuilding())
        {
            Destroy(building.gameObject);
            return true;
        }
        return false;
    }
}
