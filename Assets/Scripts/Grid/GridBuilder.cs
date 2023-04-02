using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GridBuilder : MonoBehaviour
{
    [SerializeField] GameObject floor;
    [SerializeField] List<GameObject> prefab_list;
    [SerializeField] LayerMask detect_layer;
    [SerializeField] Material ghost_material;
    [SerializeField] Material selected_material;
    [SerializeField] Material pickedup_material;
    public Building.Direction rotation;
    private GameObject selected_prefab;
    private Building ghost;

    /// <summary>
    /// Triggered when a new object is selected.
    /// </summary>
    public UnityEvent changed_selected_object_in_scene;

    /// <summary>
    /// Triggered when the building layer is changed.
    /// </summary>
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
        num_grids = PlayerPrefs.GetInt("layers");
        height = PlayerPrefs.GetInt("height");
        width = PlayerPrefs.GetInt("width");

        cell_size = 10f;
        
        /* Creates a seperate grid for each layer */
        grid_list = new List<Grid>();
        for (int i=0; i<num_grids; ++i)
        {
            grid_list.Add(new Grid(new Vector3(0, i*cell_size, 0), width, height, cell_size));
        }
        WorldGrid = grid_list[0];
        grid_index = 0;

        /* Rescales the floor to be the same size as the grid */
        floor.transform.localScale = new Vector3(width*cell_size, 1, height * cell_size);
        floor.transform.position = new Vector3((width * cell_size)/2f, 0, (height * cell_size)/2f);

        /* Default value for new placed objects */
        new_object_color = Color.white;

        /* Initialise other values */
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
        if (PauseMenuScript.Is_Paused) return; /* Skip update if game is paused */
        
        /* Raycase into the scene to find what grid the mouse is looking at */
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000f, detect_layer);
        Vector2Int click_position = Vector2Int.zero;
        Vector3 world_position = Vector3.zero;
        if (hit.rigidbody != null) /* Get the grid position if one was hit */
        {
            click_position = WorldGrid.GetXY(hit.point);
            world_position = WorldGrid.GetWorldPosition(click_position);
        }

        /* If left click the try place an object */
        if (Input.GetMouseButton(0))
        {
            if (hit.rigidbody != null && !EventSystem.current.IsPointerOverGameObject())
            {
                if (picked_up_selected_object_in_scene)
                {
                    RepositionSelectedObjectInScene(click_position);
                }
                else
                {
                    /* Instantiate new object */
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

        /* If right click then select the object occupying the tile clicked */
        if (Input.GetMouseButtonDown(1))
        {
            if (hit.rigidbody != null && !EventSystem.current.IsPointerOverGameObject())
            {
                Building b = WorldGrid.GetBuilding(click_position.x, click_position.y);
                if (b != null)
                {
                    SetSelectedObjectInScene(b.gameObject);
                }
            }
        }

        /* Change current rotation */
        if (Input.GetKeyDown("r"))
        {
            this.rotation = Building.NextDirection(this.rotation);
            UpdateGhost();
        }

        /* Change selected prefab via number keys */
        if (Input.GetKeyDown(KeyCode.Alpha1)) selected_prefab = prefab_list[0]; UpdateGhost();
        if (Input.GetKeyDown(KeyCode.Alpha2)) selected_prefab = prefab_list[1]; UpdateGhost();
        if (Input.GetKeyDown(KeyCode.Alpha3)) selected_prefab = prefab_list[2]; UpdateGhost();
        if (Input.GetKeyDown(KeyCode.Alpha4)) selected_prefab = prefab_list[3]; UpdateGhost();

        /* Move up a layer */
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            if (grid_index < num_grids - 1)
            {
                ++grid_index;
                WorldGrid = grid_list[grid_index];
                changed_build_level.Invoke();
            }
        }

        /* Move down a layer */
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            if (grid_index > 0)
            {
                --grid_index;
                WorldGrid = grid_list[grid_index];
                changed_build_level.Invoke();
            }
        }

        /* Delete object at grid position clicked on */
        if (Input.GetKey(KeyCode.Backspace) || Input.GetKey(KeyCode.Mouse2))
        {
            if (hit.rigidbody != null && !EventSystem.current.IsPointerOverGameObject())
            {
                Building b = WorldGrid.GetBuilding(click_position.x, click_position.y);
                if (b != null) DestroyBuilding(b);
            }
        }

        /* Update ghost position */
        ghost.SetPosition(world_position);
    }

    /// <summary>
    /// Keeps the ghost snapping to the grid position aimed at.
    /// </summary>
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

    /// <summary>
    /// Sets the ghost to the currently selected object.
    /// </summary>
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

    /// <summary>
    /// Sets the ghost to the selected prefab.
    /// </summary>
    /// <param name="b"></param>
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

    /// <summary>
    /// Sets the ghost building to a given index.
    /// </summary>
    /// <param name="index"></param>
    public void SetGhostBuilding(int index)
    {
        if (index >= 0 && index < prefab_list.Count) selected_prefab = prefab_list[index];
    }

    /// <summary>
    /// Sets the selected object in the scene.
    /// </summary>
    /// <param name="building_gameObject"></param>
    public void SetSelectedObjectInScene(GameObject building_gameObject)
    {
        if (selected_object_in_scene == building_gameObject)
        {
            UnselectObjectInScene();
            return;
        }

        UnselectObjectInScene();
        selected_object_in_scene = building_gameObject;
        Renderer renderer = selected_object_in_scene.GetComponentInChildren<Renderer>();
        selected_object_in_scene_material = renderer.material;
        renderer.material = selected_material;
        selected_object_in_scene_grid_index = grid_index;

        changed_selected_object_in_scene.Invoke();
    }

    /// <summary>
    /// De-Selects the selected object.
    /// </summary>
    public void UnselectObjectInScene()
    {
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

    /// <summary>
    /// Destroys a given building.
    /// </summary>
    /// <param name="b"></param>
    public void DestroyBuilding(Building b)
    {
        WorldGrid.RemoveBuildingFromAllTiles(b);
        if (selected_object_in_scene == b.gameObject) selected_object_in_scene = null;
        Destroy(b.gameObject);
        AudioManager.Instance.PlaySound("Break");
    }

    /// <summary>
    /// Destroys a given building.
    /// </summary>
    /// <param name="building_gameObject"></param>
    public void DestroyBuilding(GameObject building_gameObject)
    {
        Building b = building_gameObject.GetComponent<Building>();
        if (b == null) return;
        DestroyBuilding(b);
        AudioManager.Instance.PlaySound("Break");
    }

    /// <summary>
    /// Destroys the selected building.
    /// </summary>
    public void DestroySelectedBuilding()
    {
        if (selected_object_in_scene != null)
        {
            DestroyBuilding(selected_object_in_scene);
            selected_object_in_scene = null;
            AudioManager.Instance.PlaySound("Break");
        }
    }

    /// <summary>
    /// Sets the selected object to the "pickedup" material.
    /// </summary>
    public void HideSelectedObjectInScene()
    {
        if (selected_object_in_scene != null)
        {
            selected_object_in_scene.GetComponentInChildren<Renderer>().material = pickedup_material;
        }
    }

    /// <summary>
    /// Sets the selected object to the "selected" material.
    /// </summary>
    public void ShowSelectedObjectInScene()
    {
        if (selected_object_in_scene != null)
        {
            selected_object_in_scene.GetComponentInChildren<Renderer>().material = selected_object_in_scene_material;
        }
    }

    /// <summary>
    /// Sets the ghost to the selected object, and sets the selected object to the "pickedup" material.
    /// </summary>
    public void PickupSelectedObjectInScene()
    {
        if (selected_object_in_scene == null) return;
        SetGhostToSelectedObjectInScene();
        HideSelectedObjectInScene();
        picked_up_selected_object_in_scene = true;
    }

    /// <summary>
    /// Places the picked up object to where it originally was.
    /// </summary>
    public void DropSelectedObjectInScene()
    {
        ShowSelectedObjectInScene();
        picked_up_selected_object_in_scene = false;
        AudioManager.Instance.PlaySound("Place");
    }

    /// <summary>
    /// Repositions the selected object to a new position in the scene.
    /// (This position can be to a new grid)
    /// </summary>
    /// <param name="grid_position"></param>
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
            AudioManager.Instance.PlaySound("Place");
        }
    }

    /// <summary>
    /// Changes the colour of the selected object.
    /// </summary>
    /// <param name="c"></param>
    public void ChangeColourSeletedObject(Color c)
    {
        if (selected_object_in_scene != null)
        {
            selected_object_in_scene_material.color = c;
            UnselectObjectInScene();
        }
        else
        {
            new_object_color = c;
        }
    }

    /// <summary>
    /// Gets the colour of the selected object.
    /// </summary>
    /// <returns></returns>
    public Color GetColorSelectedObject()
    {
        if (selected_object_in_scene != null) return selected_object_in_scene_material.color;
        return new_object_color;
    }

    /// <summary>
    /// Gets the current layer that is being built on.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentBuildLevel()
    {
        return grid_index + 1;
    }
}
