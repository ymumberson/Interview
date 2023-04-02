using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Uses the grid system provided by: https://www.youtube.com/watch?v=waEsGu--9P8&list=PLzDRvYVwl53uhO8yhqxcyjDImRjO9W722&index=1
///  with some minor changes :)
/// </summary>
public class Grid
{
    private int height = 10;
    private int width = 10;
    private float cell_size = 10f;
    private GridObject[,] grid;
    private Vector3 origin;
    public Grid(Vector3 origin, int width, int height, float cell_size)
    {
        this.width = width;
        this.height = height;
        this.cell_size = cell_size;
        this.grid = new GridObject[height,width];
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

    public Vector3 GetWorldPosition(int x, int y)
    {
        //return new Vector3(y, x) * cell_size;
        return new Vector3(x, 0, y) * cell_size + origin;
    }

    public Vector3 GetWorldPosition(Vector2 grid_position)
    {
        return new Vector3(grid_position.x, 0, grid_position.y) * cell_size + origin;
    }
    public void GetXY(Vector3 world_position, out int x, out int y)
    {
        x = Mathf.FloorToInt((world_position - origin).x/ cell_size);
        y = Mathf.FloorToInt((world_position - origin).z / cell_size);
    }

    public Vector2Int GetXY(Vector3 world_position)
    {
        return new Vector2Int(
            Mathf.FloorToInt((world_position - origin).x / cell_size),
            Mathf.FloorToInt((world_position - origin).z / cell_size));
    }
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
    public bool SetValue(Vector3 world_position, GridObject value)
    {
        Vector2Int pos = GetXY(world_position);
        return SetValue(pos.x, pos.y, value);
    }

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


            //if (building_script.GetHeight() == 1 && building_script.GetWidth() == 1)
            //{
            //    grid[x, y].SetBuilding(building);
            //}
            //else
            //{
            //    for (int i = building_script.GetWidth() - 1; i >= 0; --i)
            //    {
            //        for (int j = building_script.GetHeight() - 1; j >= 0; --j)
            //        {
            //            grid[x + i, y + j].SetBuilding(building);
            //        }
            //    }
            //}
            return true;
        }
        else
        {
            Debug.LogError("Tried to set grid position [" + x + "," + y + "] which is out of bounds. Width: " + width + ", Height: " + height);
            return false;
        }
    }
    public bool SetBuilding(Vector3 world_position, GameObject building)
    {
        Vector2Int pos = GetXY(world_position);
        return SetBuilding(pos.x, pos.y, building);
    }

    public GameObject RemoveBuilding(int x, int y)
    {
        if (!InBounds(x, y)) return null;
        GameObject building_object = grid[x, y].gameObject;
        grid[x,y].RemoveBuilding();
        return building_object;
    }

    public void RemoveBuildingFromAllTiles(Building b)
    {
        foreach (Vector2Int v in b.GetOccupiedTiles(this))
        {
            this.RemoveBuilding(v.x, v.y);
        }
    }
    public bool RemoveAndDestroyBuilding(int x, int y)
    {
        if (!InBounds(x, y)) return false;
        grid[x, y].RemoveAndDestroyBuilding();
        return true;
    }
    public Building GetBuilding(int x, int y)
    {
        if (InBounds(x, y) && grid[x, y].ContainsBuilding()) return grid[x, y].GetBuilding();
        return null;
    }
    public bool InBounds(int x, int y)
    {
        return (y >= 0 && y < height) && (x >= 0 && x < width);
    }

    public bool CanBuild(Vector2Int v)
    {
        return CanBuild(v.x, v.y);
    }
    public bool CanBuild(int x, int y)
    {
        return InBounds(x, y) && !grid[x, y].ContainsBuilding();
    }

    public bool CanBuild(Vector3 world_position)
    {
        return CanBuild(GetXY(world_position));
    }

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

    public bool CanBuild(Vector2Int v, Building b)
    {
        return CanBuild(v.x, v.y, b);
    }
    public bool CanBuild(int x, int y, Building b)
    {
        return InBounds(x, y) && (!grid[x, y].ContainsBuilding() || grid[x, y].ContainsBuilding(b));
    }

    public bool CanBuild(Vector3 world_position, Building b)
    {
        return CanBuild(GetXY(world_position),b);
    }

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
