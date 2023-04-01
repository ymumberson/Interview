using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeEditScript : MonoBehaviour
{
    [SerializeField] GridBuilder gb;

    public void DestroySelectedObject()
    {
        gb.DestroySelectedBuilding();
    }

    public void PickupSelectedObject()
    {
        gb.PickupSelectedObjectInScene();
    }
}
