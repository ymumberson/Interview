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
    [SerializeField] Material ghost_material;
    public Building.Direction rotation;
    private GameObject selected_prefab;
    private Building ghost;

    private int height;
    private int width;
    private float cell_size;
    public Grid WorldGrid { get; private set; }
    private void Awake()
    {
        height = 10;
        width = 10;
        cell_size = 10f;
        WorldGrid = new Grid(new Vector3(0, 0, 0), height, width, cell_size);
        selected_prefab = prefab_list[0];

        ghost = Instantiate(selected_prefab, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(rotation), 0)).GetComponent<Building>();
        ghost.SetDirection(rotation);
        ghost.SetCellSize(cell_size);
    }

    private void Update()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, detect_layer);
        Vector2Int click_position = Vector2Int.zero;
        Vector3 world_position = Vector3.zero;
        if (hit.rigidbody != null)
        {
            //Debug.Log("Clicked at coordinate: " + hit.point + " == " + WorldGrid.GetXY(hit.point));
            click_position = WorldGrid.GetXY(hit.point);
            world_position = WorldGrid.GetWorldPosition(click_position);
        }

            if (Input.GetMouseButtonDown(0))
        {
            //RaycastHit hit;
            //Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, detect_layer);
            if (hit.rigidbody != null)
            {
                //Debug.Log("Clicked at coordinate: " + hit.point + " == " + WorldGrid.GetXY(hit.point));
                //Vector2Int click_position = WorldGrid.GetXY(hit.point);
                //Vector3 world_position = WorldGrid.GetWorldPosition(click_position);

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

        if (Input.GetMouseButtonDown(1))
        {
            //RaycastHit hit;
            //Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, detect_layer);
            if (hit.rigidbody != null)
            {
                //Debug.Log("Clicked at coordinate: " + hit.point + " == " + WorldGrid.GetXY(hit.point));
                //Vector2Int click_position = WorldGrid.GetXY(hit.point);
                //Vector3 world_position = WorldGrid.GetWorldPosition(click_position);

                Building b = WorldGrid.GetBuilding(click_position.x, click_position.y);

                if (b != null)
                {
                    foreach (Vector2Int v in b.GetOccupiedTiles(WorldGrid))
                    {
                        WorldGrid.RemoveBuilding(v.x, v.y);
                    }
                }
            }
        }

        if (Input.GetKeyDown("r"))
        {
            this.rotation = Building.NextDirection(this.rotation);
            UpdateGhost();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) selected_prefab = prefab_list[0]; UpdateGhost();
        if (Input.GetKeyDown(KeyCode.Alpha2)) selected_prefab = prefab_list[1]; UpdateGhost();
        if (Input.GetKeyDown(KeyCode.Alpha3)) selected_prefab = prefab_list[2]; UpdateGhost();
        if (Input.GetKeyDown(KeyCode.Alpha4)) selected_prefab = prefab_list[3]; UpdateGhost();

        ghost.SetPosition(world_position);
    }

    private void UpdateGhost()
    {
        if (ghost != null) Destroy(ghost.gameObject);
        ghost = Instantiate(selected_prefab, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(rotation), 0)).GetComponent<Building>();
        ghost.SetDirection(rotation);
        ghost.SetCellSize(cell_size);

        Renderer r = ghost.gameObject.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            r.material = ghost_material;
        }
    }
}
