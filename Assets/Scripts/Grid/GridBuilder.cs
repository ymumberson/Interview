using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] GameObject cube_prefab;
    [SerializeField] LayerMask detect_layer;
    public Grid WorldGrid { get; private set; }
    private void Awake()
    {
        int height = 10;
        int width = 10;
        float cell_size = 10f;
        WorldGrid = new Grid(height, width, cell_size);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, detect_layer);
            if (hit.rigidbody != null)
            {
                Debug.Log("Clicked at coordinate: " + hit.point + " == " + WorldGrid.GetXY(hit.point));
                Vector2 click_position = WorldGrid.GetXY(hit.point);
                Vector3 world_position = WorldGrid.GetWorldPosition(click_position);
                GameObject building = Instantiate(cube_prefab, Vector3.zero, Quaternion.identity);
                if (!WorldGrid.SetBuilding(world_position, building)) {
                    Destroy(building);
                }
            }
        }
    }
}