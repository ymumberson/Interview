using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeSolver : MonoBehaviour
{
    public GridBuilder grid_builder;
    public GameObject cube_template;
    public Material start_material;
    public Material end_material;

    private GameObject start_cube;
    private GameObject end_cube;

    private List<Vector2Int> path;
    private bool is_moving_along_path;
    private float timer;
    private float move_delay;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        move_delay = 0.25f;
        float cell_size = grid_builder.WorldGrid.GetCellSize();

        start_cube = Instantiate(cube_template, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(Building.Direction.UP), 0));
        Building building_script = start_cube.GetComponent<Building>();
        building_script.SetCellSize(cell_size);
        start_cube.GetComponentInChildren<Renderer>().material = start_material;

        end_cube = Instantiate(cube_template, Vector3.zero, Quaternion.Euler(0, Building.GetDirectionAngle(Building.Direction.UP), 0));
        building_script = start_cube.GetComponent<Building>();
        building_script.SetCellSize(cell_size);
        end_cube.GetComponentInChildren<Renderer>().material = end_material;

        int height = grid_builder.WorldGrid.GetHeight();
        int width = grid_builder.WorldGrid.GetWidth();

        grid_builder.WorldGrid.SetBuilding(0, 0, start_cube);
        grid_builder.WorldGrid.SetBuilding(width-1, height-1, end_cube);
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (is_moving_along_path && timer > move_delay)
        {
            Vector2Int current = path[path.Count - 1];
            path.RemoveAt(path.Count-1);
            if (path.Count == 0)
            {
                is_moving_along_path = false; // Reached destination
            }

            Vector3 world_pos = grid_builder.WorldGrid.GetWorldPosition(current);
            start_cube.GetComponent<Building>().SetPosition(world_pos);
            timer = 0;
        }
    }

    public void PathFind()
    {
        if (is_moving_along_path) return;
        int height = grid_builder.WorldGrid.GetHeight();
        int width = grid_builder.WorldGrid.GetWidth();
        path = AStarPathfinding(new Vector2Int(0, 0), new Vector2Int(width - 1, height - 1));

        if (path.Count > 0)
        {
            print("Found path");
            is_moving_along_path = true;
        }
        else
        {
            print("No path!");
        }
    }

    private List<Vector2Int> AStarPathfinding(Vector2Int start, Vector2Int finish)
    {
        Grid WorldGrid = grid_builder.WorldGrid;
        Building end_building = end_cube.GetComponent<Building>();
        int height = grid_builder.WorldGrid.GetHeight();
        int width = grid_builder.WorldGrid.GetWidth();

        Tile[,] tiles = new Tile[width, height];
        for (int i=0; i<width; ++i)
        {
            for (int j=0; j<height; ++j)
            {
                //tiles[i, j].x = i;
                //tiles[i, j].y = j;
                tiles[i, j] = new Tile(i, j, 0, 0, null);
            }
        }

        List<Tile> open_list = new List<Tile>();
        List<Tile> closed_list = new List<Tile>();

        /* Start of pathfinding */
        tiles[0, 0] = new Tile(0,0,0,Mathf.Pow(finish.x - start.x, 2) + Mathf.Pow(finish.y - start.y, 2), null);
        open_list.Add(tiles[0, 0]);

        bool found = false;
        while (open_list.Count > 0)
        {
            Tile current_tile = open_list[0];
            float lowest_f = float.MaxValue;
            foreach (Tile t in open_list)
            {
                if (t.f() < lowest_f)
                {
                    lowest_f = t.f();
                    current_tile = t;
                }
            }

            open_list.Remove(current_tile);
            closed_list.Add(current_tile);

            if (current_tile.x == finish.x && current_tile.y == finish.y)
            {
                found = true;
                print("Found!");
                break;
            }

            for (int i=-1; i<=1; ++i)
            {
                for (int j=-1; j<=1; ++j)
                {
                    if (Mathf.Abs(i) == Mathf.Abs(j)) continue; //Skips corners
                    int x1 = current_tile.x + i;
                    int y1 = current_tile.y + j;

                    if (!(i == 0 && j == 0) && (WorldGrid.InBounds(x1, y1)) && WorldGrid.CanBuild(x1,y1,end_building))
                    {
                        //print("Didn't skip: " + x1 + "," + y1);
                        Tile adjacent = tiles[x1, y1];

                        /* These values didn't align for some reason */
                        adjacent.x = x1;
                        adjacent.y = y1;
                        /**/

                        adjacent.g = current_tile.g + 1;
                        adjacent.h = Mathf.Pow(finish.x - current_tile.x, 2) + Mathf.Pow(finish.y - current_tile.y, 2);

                        bool tile_is_in_closed_list = false;
                        foreach (Tile tile in closed_list)
                        {
                            //print("Closed list contains: " + tile.x + "," + tile.y);
                            //if (tile == adjacent)
                            if (tile.x == adjacent.x && tile.y == adjacent.y)
                            {
                                //print(tile.x + "," + tile.y + " == " + adjacent.x + "," + adjacent.y + " (" + x1 + "," + y1 + ")");
                                tile_is_in_closed_list = true;
                                break;
                            }
                        }
                        if (tile_is_in_closed_list)
                        {
                            //print("Tile is in closed list: " + x1 + "," + y1);
                            continue;
                        }

                        bool tile_is_in_open_list = false;
                        foreach (Tile tile in open_list)
                        {
                            //if (tile == adjacent)
                            if (tile.x == adjacent.x && tile.y == adjacent.y)
                            {
                                tile_is_in_open_list = true;
                                if (current_tile.g < tile.parent.g)
                                {
                                    tile.g = current_tile.g + 1;
                                    tile.parent = current_tile;
                                }
                            }
                        }

                        if (!tile_is_in_open_list)
                        {
                            open_list.Add(adjacent);
                            adjacent.parent = current_tile;
                            //print("Added " + adjacent.x + "," + adjacent.y);
                        }
                    }
                }
            }
        }

        if (!found)
        {
            return new List<Vector2Int>();
        }

        /* Found tile so check convert closed list into a Vector2Int list (But reverse order) */
        List<Vector2Int> list = new List<Vector2Int>();
        Tile current = tiles[finish.x, finish.y];

        while (current != null)
        {
            list.Add(new Vector2Int(current.x, current.y));
            current = current.parent;
        }
        return list;
    }
    private class Tile
    {
        public int x;
        public int y;
        public float g;
        public float h;
        public Tile parent;

        public Tile()
        {
            g = 0;
            h = 0;
            parent = null;
        }

        public Tile(int x, int y, float g, float h, Tile parent)
        {
            this.g = g;
            this.h = h;
            this.parent = parent;
        }

        public float f()
        {
            return g + h;
        }
    }
}


