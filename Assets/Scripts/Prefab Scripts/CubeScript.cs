using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    private float width;

    private void Awake()
    {
        width = transform.localScale.x;
    }

    public void setPosition(Vector3 world_position)
    {
        Vector3 new_position = new Vector3(world_position.x + width / 2, world_position.y + width / 2, world_position.z + width / 2);
    }
}
