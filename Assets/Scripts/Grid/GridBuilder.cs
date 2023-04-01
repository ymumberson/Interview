using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    //[SerializeField] GameObject cube_prefab;
    //[SerializeField] GameObject sphere_prefab;
    //[SerializeField] GameObject big_cube_prefab;
    [SerializeField] List<GameObject> prefab_list;
    [SerializeField] LayerMask detect_layer;
    public Building.Direction rotation;
    private GameObject selected_prefab;

    private int height;
    private int width;
    private float cell_size;
    public Grid WorldGrid { get; private set; }
    private void Awake()
    {
        height = 10;
        width = 10;
        cell_size = 10f;
        WorldGrid = new Grid(height, width, cell_size);
        selected_prefab = prefab_list[0];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, detect_layer);
            if (hit.rigidbody != null)
            {
                //Debug.Log("Clicked at coordinate: " + hit.point + " == " + WorldGrid.GetXY(hit.point));
                Vector2Int click_position = WorldGrid.GetXY(hit.point);
                Vector3 world_position = WorldGrid.GetWorldPosition(click_position);

                GameObject building = Instantiate(selected_prefab, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(rotation), 0));
                Building building_script = building.GetComponent<Building>();
                building_script.SetDirection(rotation);
                building_script.SetCellSize(cell_size);

                List<Vector2Int> all_grid_positions = building_script.GetAllGridPositions(click_position);
                if (WorldGrid.CanBuild(all_grid_positions))
                {
                    WorldGrid.SetBuilding(world_position, building);
                }
                else
                {
                    Debug.Log("There is alread a building here!");
                    Destroy(building.gameObject);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) selected_prefab = prefab_list[0];
        if (Input.GetKeyDown(KeyCode.Alpha2)) selected_prefab = prefab_list[1];
        if (Input.GetKeyDown(KeyCode.Alpha3)) selected_prefab = prefab_list[2];
        if (Input.GetKeyDown(KeyCode.Alpha4)) selected_prefab = prefab_list[3];
    }
}
