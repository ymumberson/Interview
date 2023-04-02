using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  The grid class divides the scene into tiles.
///  It is of a given size (width,height) with a given origin.
///  Note that the "y" used is actually the z axis.
/// </summary>
public class Grid
{
    /// <summary>
    /// Height (Depth) of the grid.
    /// </summary>
    private int height = 10;

    /// <summary>
    /// Width of the grid.
    /// </summary>
    private int width = 10;

    /// <summary>
    /// Size (Width) of each cell.
    /// </summary>
    private float cell_size = 10f;

    /// <summary>
    /// The grid itself.
    /// </summary>
    private GridObject[,] grid;

    /// <summary>
    /// Origin of the grid.
    /// </summary>
    private Vector3 origin;

    /// <summary>
    /// Contructs the grid.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="cell_size"></param>
    public Grid(Vector3 origin, int width, int height, float cell_size)
    {
        this.width = width;
        this.height = height;
        this.cell_size = cell_size;
        this.grid = new GridObject[width,height];
        this.origin = origin;
        for (int j=0; j<height; ++j)
        {
            for (int i=0; i<width; ++i)
            {
                GameObject go = new GameObject();
                go.transform.position = this.GetWorldPosition(i,j);
                grid[i, j] = go.AddComponent<GridObject>();
                Debug.DrawLine(GetWorldPosition(i,j), GetWorldPosition(i+1,j), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j+1), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
    }

    public int GetWidth()
    {
        return this.width;
    }

    public int GetHeight()
    {
        return this.height;
    }

    public float GetCellSize()
    {
        return this.cell_size;
    }

    /// <summary>
    /// Converts a grid (x,y) into world coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cell_size + origin;
    }

    /// <summary>
    /// Converts a grid (x,y) into world coordinates.
    /// </summary>
    /// <param name="grid_position"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(Vector2 grid_position)
    {
        return new Vector3(grid_position.x, 0, grid_position.y) * cell_size + origin;
    }

    /// <summary>
    /// Converts world coordinates into grid (x,y).
    /// </summary>
    /// <param name="world_position"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void GetXY(Vector3 world_position, out int x, out int y)
    {
        x = Mathf.FloorToInt((world_position - origin).x/ cell_size);
        y = Mathf.FloorToInt((world_position - origin).z / cell_size);
    }

    /// <summary>
    /// Converts world coordinates into grid (x,y).
    /// </summary>
    /// <param name="world_position"></param>
    /// <returns></returns>
    public Vector2Int GetXY(Vector3 world_position)
    {
        return new Vector2Int(
            Mathf.FloorToInt((world_position - origin).x / cell_size),
            Mathf.FloorToInt((world_position - origin).z / cell_size));
    }

    /// <summary>
    /// Sets the value of a specified cell.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetValue(int x, int y, GridObject value)
    {
        if (InBounds(x, y))
        {
            grid[x, y] = value;
            return true;
        }
        else
        {
            Debug.LogError("Tried to set grid position [" + x + "," + y + "] which is out of bounds. Width: " + width + ", Height: " + height);
            return false;
        }
    }

    /// <summary>
    /// Sets the value of a specified cell.
    /// </summary>
    /// <param name="world_position"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetValue(Vector3 world_position, GridObject value)
    {
        Vector2Int pos = GetXY(world_position);
        return SetValue(pos.x, pos.y, value);
    }

    /// <summary>
    /// Sets the Building of a specified cell.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="building"></param>
    /// <returns></returns>
    public bool SetBuilding(int x, int y, GameObject building)
    {
        if (InBounds(x, y))
        {
            Building building_script = building.GetComponent<Building>();

            List<Vector2Int> all_positions = building_script.GetAllGridPositions(new Vector2Int(x, y));
            foreach (Vector2Int position in all_positions)
            {
                grid[position.x, position.y].SetBuilding(building);
            }
            grid[x, y].PositionBuildingOnSelf(); /* Setting position changes the transform, so make sure the transform is set to (x,y) */
            return true;
        }
        else
        {
            Debug.LogError("Tried to set grid position [" + x + "," + y + "] which is out of bounds. Width: " + width + ", Height: " + height);
            return false;
        }
    }

    /// <summary>
    /// Sets the Building of a specified cell.
    /// </summary>
    /// <param name="world_position"></param>
    /// <param name="building"></param>
    /// <returns></returns>
    public bool SetBuilding(Vector3 world_position, GameObject building)
    {
        Vector2Int pos = GetXY(world_position);
        return SetBuilding(pos.x, pos.y, building);
    }

    /// <summary>
    /// Removes the Building of a specified cell.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public GameObject RemoveBuilding(int x, int y)
    {
        if (!InBounds(x, y)) return null;
        GameObject building_object = grid[x, y].gameObject;
        grid[x,y].RemoveBuilding();
        return building_object;
    }

    /// <summary>
    /// Removes the Building from all tiles that it occupies.
    /// </summary>
    /// <param name="b"></param>
    public void RemoveBuildingFromAllTiles(Building b)
    {
        foreach (Vector2Int v in b.GetOccupiedTiles(this))
        {
            this.RemoveBuilding(v.x, v.y);
        }
    }

    /// <summary>
    /// Removes the Building of a specified cell and destroys the gameObject.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool RemoveAndDestroyBuilding(int x, int y)
    {
        if (!InBounds(x, y)) return false;
        grid[x, y].RemoveAndDestroyBuilding();
        return true;
    }

    /// <summary>
    /// Gets the building of a specific cell.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Building GetBuilding(int x, int y)
    {
        if (InBounds(x, y) && grid[x, y].ContainsBuilding()) return grid[x, y].GetBuilding();
        return null;
    }

    /// <summary>
    /// Checks if given (x,y) are within grid bounds.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool InBounds(int x, int y)
    {
        return (y >= 0 && y < height) && (x >= 0 && x < width);
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public bool CanBuild(Vector2Int v)
    {
        return CanBuild(v.x, v.y);
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CanBuild(int x, int y)
    {
        return InBounds(x, y) && !grid[x, y].ContainsBuilding();
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="world_position"></param>
    /// <returns></returns>
    public bool CanBuild(Vector3 world_position)
    {
        return CanBuild(GetXY(world_position));
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="all_grid_positions"></param>
    /// <returns></returns>
    public bool CanBuild(List<Vector2Int> all_grid_positions)
    {
        foreach (Vector2Int v in all_grid_positions)
        {
            if (!CanBuild(v))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool CanBuild(Vector2Int v, Building b)
    {
        return CanBuild(v.x, v.y, b);
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool CanBuild(int x, int y, Building b)
    {
        return InBounds(x, y) && (!grid[x, y].ContainsBuilding() || grid[x, y].ContainsBuilding(b));
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="world_position"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool CanBuild(Vector3 world_position, Building b)
    {
        return CanBuild(GetXY(world_position),b);
    }

    /// <summary>
    /// Checks if a tile is already occupied by a Building.
    /// </summary>
    /// <param name="all_grid_positions"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool CanBuild(List<Vector2Int> all_grid_positions, Building b)
    {
        foreach (Vector2Int v in all_grid_positions)
        {
            if (!CanBuild(v,b))
            {
                return false;
            }
        }
        return true;
    }
}
