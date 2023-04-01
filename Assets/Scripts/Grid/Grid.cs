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
    public Grid(int width, int height, float cell_size)
    {
        this.width = width;
        this.height = height;
        this.cell_size = cell_size;
        this.grid = new GridObject[height,width];
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

    public Vector3 GetWorldPosition(int x, int y)
    {
        //return new Vector3(y, x) * cell_size;
        return new Vector3(x, 0, y) * cell_size;
    }

    public Vector3 GetWorldPosition(Vector2 grid_position)
    {
        return new Vector3(grid_position.x, 0, grid_position.y) * cell_size;
    }
    public void GetXY(Vector3 world_position, out int x, out int y)
    {
        x = Mathf.FloorToInt(world_position.x/cell_size);
        y = Mathf.FloorToInt(world_position.z / cell_size);
    }

    public Vector2Int GetXY(Vector3 world_position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(world_position.x / cell_size),
            Mathf.FloorToInt(world_position.z / cell_size));
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
            grid[x, y].SetBuilding(building);
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
}
