using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSelectUI : MonoBehaviour
{
    [SerializeField] GridBuilder gb;

    public void SetSelectedBuildingCube()
    {
        gb.SetGhostBuilding(0);
    }

    public void SetSelectedBuildingSphere()
    {
        gb.SetGhostBuilding(1);
    }

    public void SetSelectedBuildingBigCube()
    {
        gb.SetGhostBuilding(2);
    }

    public void SetSelectedBuildingRectangle()
    {
        gb.SetGhostBuilding(3);
    }
}
