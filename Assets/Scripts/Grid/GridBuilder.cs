using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GridBuilder : MonoBehaviour
{
    //[SerializeField] GameObject cube_prefab;
    //[SerializeField] GameObject sphere_prefab;
    //[SerializeField] GameObject big_cube_prefab;
    [SerializeField] List<GameObject> prefab_list;
    [SerializeField] LayerMask detect_layer;
    [SerializeField] Material ghost_material;
    [SerializeField] Material selected_material;
    [SerializeField] Material pickedup_material;
    public Building.Direction rotation;
    private GameObject selected_prefab;
    private Building ghost;

    public UnityEvent changed_selected_object_in_scene;
    public UnityEvent changed_build_level;

    private GameObject selected_object_in_scene;
    private Material selected_object_in_scene_material;
    private bool picked_up_selected_object_in_scene;
    private int selected_object_in_scene_grid_index;

    private Color new_object_color;

    private int height;
    private int width;
    private float cell_size;
    public Grid WorldGrid { get; private set; }
    private List<Grid> grid_list;
    private int grid_index;
    private int num_grids;
    private void Awake()
    {
        height = 10;
        width = 10;
        cell_size = 10f;
        //WorldGrid = new Grid(new Vector3(0, 0, 0), height, width, cell_size);
        num_grids = 4;
        grid_list = new List<Grid>();
        for (int i=0; i<num_grids; ++i)
        {
            grid_list.Add(new Grid(new Vector3(0, i*cell_size, 0), height, width, cell_size));
        }
        WorldGrid = grid_list[0];
        grid_index = 0;

        new_object_color = Color.white;

        selected_prefab = prefab_list[0];
        selected_object_in_scene = null;
        selected_object_in_scene_material = null;
        picked_up_selected_object_in_scene = false;

        ghost = Instantiate(selected_prefab, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(rotation), 0)).GetComponent<Building>();
        ghost.SetDirection(rotation);
        ghost.SetCellSize(cell_size);
    }

    private void Update()
    {
        if (PauseMenuScript.Is_Paused) return;
        
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
            if (hit.rigidbody != null && !EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Clicked at coordinate: " + hit.point + " == " + WorldGrid.GetXY(hit.point));
                //Vector2Int click_position = WorldGrid.GetXY(hit.point);
                //Vector3 world_position = WorldGrid.GetWorldPosition(click_position);

                if (picked_up_selected_object_in_scene)
                {
                    RepositionSelectedObjectInScene(click_position);
                }
                else
                {
                    GameObject building = Instantiate(selected_prefab, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(rotation), 0));
                    Building building_script = building.GetComponent<Building>();
                    building_script.SetDirection(rotation);
                    building_script.SetCellSize(cell_size);

                    building.GetComponentInChildren<Renderer>().material.color = new_object_color;

                    List<Vector2Int> all_grid_positions = building_script.GetAllGridPositions(click_position);
                    if (WorldGrid.CanBuild(all_grid_positions))
                    {
                        WorldGrid.SetBuilding(world_position, building);
                        AudioManager.Instance.PlaySound("Place");
                    }
                    else
                    {
                        Debug.Log("There is alread a building here!");
                        Destroy(building.gameObject);
                    }
                }

                
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            //RaycastHit hit;
            //Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, detect_layer);
            if (hit.rigidbody != null && !EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Clicked at coordinate: " + hit.point + " == " + WorldGrid.GetXY(hit.point));
                //Vector2Int click_position = WorldGrid.GetXY(hit.point);
                //Vector3 world_position = WorldGrid.GetWorldPosition(click_position);

                Building b = WorldGrid.GetBuilding(click_position.x, click_position.y);
                if (b != null)
                {
                    //SetGhostToSelectedBuilding(b);
                    SetSelectedObjectInScene(b.gameObject);
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

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if (grid_index < num_grids - 1)
            {
                ++grid_index;
                WorldGrid = grid_list[grid_index];
                changed_build_level.Invoke();
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (grid_index > 0)
            {
                --grid_index;
                WorldGrid = grid_list[grid_index];
                changed_build_level.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (hit.rigidbody != null && !EventSystem.current.IsPointerOverGameObject())
            {
                Building b = WorldGrid.GetBuilding(click_position.x, click_position.y);
                if (b != null) DestroyBuilding(b);
            }
        }

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

        ghost.gameObject.layer = 10;
        foreach (Transform child in ghost.transform)
        {
            child.gameObject.layer = 10;
        }
        
        Collider col = ghost.gameObject.GetComponentInChildren <Collider>();
        if (col != null) col.isTrigger = true;
    }

    public void SetGhostToSelectedObjectInScene()
    {
        if (selected_object_in_scene != null)
        {
            Building b = selected_object_in_scene.GetComponent<Building>();
            if (b != null)
            {
                SetGhostToSelectedBuilding(b);
            }
        }
    }
    private void SetGhostToSelectedBuilding(Building b)
    {
        switch (b.gameObject.name)
        {
            default:
            case "Cube(Clone)":
                selected_prefab = prefab_list[0];
                break;
            case "Sphere(Clone)":
                selected_prefab = prefab_list[1];
                break;
            case "Big Cube(Clone)":
                selected_prefab = prefab_list[2];
                break;
            case "Rectangle(Clone)":
                selected_prefab = prefab_list[3];
                break;
        }
        this.rotation = b.GetDirection();
        this.UpdateGhost();
    }

    public void SetGhostBuilding(int index)
    {
        if (index >= 0 && index < prefab_list.Count) selected_prefab = prefab_list[index];
    }

    public void SetSelectedObjectInScene(GameObject building_gameObject)
    {
        if (selected_object_in_scene == building_gameObject)
        {
            UnselectObjectInScene();
            return;
        }


        //if (selected_object_in_scene != null)
        //{
        //    selected_object_in_scene.GetComponentInChildren<Renderer>().material = selected_object_in_scene_material;
        //    if (picked_up_selected_object_in_scene)
        //    {
        //        DropSelectedObjectInScene();
        //    }
        //}
        UnselectObjectInScene();
        selected_object_in_scene = building_gameObject;
        Renderer renderer = selected_object_in_scene.GetComponentInChildren<Renderer>();
        selected_object_in_scene_material = renderer.material;
        renderer.material = selected_material;
        selected_object_in_scene_grid_index = grid_index;

        changed_selected_object_in_scene.Invoke();
    }

    public void UnselectObjectInScene()
    {
        //if (selected_object_in_scene != null)
        //{
        //    Renderer r = selected_object_in_scene.GetComponentInChildren<Renderer>();
        //    if (r != null)
        //    {
        //        r.material = selected_object_in_scene_material;
        //        print("Changed material");
        //    }
        //    else
        //    {
        //        print("Null thuing>");
        //    }
        //    if (picked_up_selected_object_in_scene) DropSelectedObjectInScene();
        //    selected_object_in_scene = null;
        //    print("selected object is null?" + (selected_object_in_scene == null));
        //}
        if (selected_object_in_scene != null)
        {
            selected_object_in_scene.GetComponentInChildren<Renderer>().material = selected_object_in_scene_material;
            if (picked_up_selected_object_in_scene)
            {
                DropSelectedObjectInScene();
            }
            selected_object_in_scene = null;
            changed_selected_object_in_scene.Invoke();
        }
    }
    public void DestroyBuilding(Building b)
    {
        WorldGrid.RemoveBuildingFromAllTiles(b);
        if (selected_object_in_scene == b.gameObject) selected_object_in_scene = null;
        Destroy(b.gameObject);
        AudioManager.Instance.PlaySound("Break");
    }

    public void DestroyBuilding(GameObject building_gameObject)
    {
        Building b = building_gameObject.GetComponent<Building>();
        if (b == null) return;
        DestroyBuilding(b);
        AudioManager.Instance.PlaySound("Break");
    }

    public void DestroySelectedBuilding()
    {
        if (selected_object_in_scene != null)
        {
            DestroyBuilding(selected_object_in_scene);
            selected_object_in_scene = null;
            AudioManager.Instance.PlaySound("Break");
        }
    }

    public void HideSelectedObjectInScene()
    {
        if (selected_object_in_scene != null)
        {
            //selected_object_in_scene.SetActive(false);
            selected_object_in_scene.GetComponentInChildren<Renderer>().material = pickedup_material;
        }
    }

    public void ShowSelectedObjectInScene()
    {
        if (selected_object_in_scene != null)
        {
            //selected_object_in_scene.SetActive(true);
            selected_object_in_scene.GetComponentInChildren<Renderer>().material = selected_object_in_scene_material;
        }
    }
    public void PickupSelectedObjectInScene()
    {
        if (selected_object_in_scene == null) return;
        SetGhostToSelectedObjectInScene();
        HideSelectedObjectInScene();
        picked_up_selected_object_in_scene = true;
    }

    public void DropSelectedObjectInScene()
    {
        ShowSelectedObjectInScene();
        picked_up_selected_object_in_scene = false;
        AudioManager.Instance.PlaySound("Place");
    }

    //public void RepositionSelectedObjectInScene(Vector2Int grid_position)
    //{
    //    Building building_script = selected_object_in_scene.GetComponent<Building>();
    //    List<Vector2Int> all_grid_positions = building_script.GetAllGridPositions(grid_position, rotation);
    //    if (WorldGrid.CanBuild(all_grid_positions, building_script))
    //    {
    //        WorldGrid.RemoveBuildingFromAllTiles(building_script);
    //        building_script.SetDirection(rotation);
    //        WorldGrid.SetBuilding(grid_position.x, grid_position.y, selected_object_in_scene);
    //        ShowSelectedObjectInScene();
    //        picked_up_selected_object_in_scene = false;
    //        Debug.Log("Repositioned building");
    //    }
    //}

    public void RepositionSelectedObjectInScene(Vector2Int grid_position)
    {
        Building building_script = selected_object_in_scene.GetComponent<Building>();
        List<Vector2Int> all_grid_positions = building_script.GetAllGridPositions(grid_position, rotation);
        if (WorldGrid.CanBuild(all_grid_positions, building_script))
        {
            grid_list[selected_object_in_scene_grid_index].RemoveBuildingFromAllTiles(building_script);
            building_script.SetDirection(rotation);
            WorldGrid.SetBuilding(grid_position.x, grid_position.y, selected_object_in_scene);
            ShowSelectedObjectInScene();
            picked_up_selected_object_in_scene = false;
            Debug.Log("Repositioned building");
            AudioManager.Instance.PlaySound("Place");
        }
    }

    public void TestChangeColour()
    {
        ChangeColourSeletedObject(Color.red);
    }

    public void ChangeColourSeletedObject(Color c)
    {
        /* Design question is if I want the color to overwrite the "selected" colour, so instead I deselect it -> Same effect-ish */
        if (selected_object_in_scene != null)
        {
            //selected_object_in_scene.GetComponentInChildren<Renderer>().material.color = c; // Would overwrite the selected color
            selected_object_in_scene_material.color = c;
            UnselectObjectInScene();
        }
        else
        {
            new_object_color = c;
        }
    }

    public Color GetColorSelectedObject()
    {
        if (selected_object_in_scene != null) return selected_object_in_scene_material.color;
        //return Color.black;
        return new_object_color;
    }

    public int GetCurrentBuildLevel()
    {
        return grid_index + 1;
    }
}
