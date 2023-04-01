using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] GameObject cube_prefab;
    [SerializeField] GameObject sphere_prefab;
    [SerializeField] GameObject big_cube_prefab;
    [SerializeField] LayerMask detect_layer;
    public Building.Direction rotation;

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
                Vector2Int click_position = WorldGrid.GetXY(hit.point);
                Vector3 world_position = WorldGrid.GetWorldPosition(click_position);

                ///* Check if we can build at a certain place */
                //if (WorldGrid.CanBuild(world_position))
                //{
                //    //GameObject building = Instantiate(cube_prefab, Vector3.zero, Quaternion.identity);
                //    GameObject building;
                //    if (Random.value < 0.5)
                //    {
                //        building = Instantiate(cube_prefab, Vector3.zero, Quaternion.identity);
                //    }
                //    else
                //    {
                //        building = Instantiate(sphere_prefab, Vector3.zero, Quaternion.identity);
                //    }
                //    //GameObject building = Instantiate(big_cube_prefab, Vector3.zero, Quaternion.identity);
                //    WorldGrid.SetBuilding(world_position, building);
                //}
                //else
                //{
                //    Debug.Log("There is alread a building here!");
                //}

                //GameObject building = Instantiate(big_cube_prefab, Vector3.zero, Quaternion.identity);

                GameObject building = Instantiate(big_cube_prefab, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(rotation), 0));
                Building building_script = building.GetComponent<Building>();
                building_script.SetDirection(rotation);
                building_script.SetCellSize(cell_size);
                Vector3 offset = building_script.GetDirectionOffset();
                Debug.Log("Offset: " + offset);
                building.transform.position = building.transform.position + building_script.GetDirectionOffset();

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

                //if (WorldGrid.CanBuild(world_position, building.GetComponent<Building>()))
                //{
                //    WorldGrid.SetBuilding(world_position, building);
                //}
                //else
                //{
                //    Debug.Log("There is alread a building here!");
                //    Destroy(building.gameObject);
                //}

                /* Replace building where we click */
                //GameObject building = Instantiate(cube_prefab, Vector3.zero, Quaternion.identity);
                //if (!WorldGrid.SetBuilding(world_position, building)) {
                //    Destroy(building);
                //}


            }
        }
    }
}
