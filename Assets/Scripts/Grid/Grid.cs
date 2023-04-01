using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Uses the grid system provided by: https://www.youtube.com/watch?v=waEsGu--9P8&list=PLzDRvYVwl53uhO8yhqxcyjDImRjO9W722&index=1
///  with some minor changes :)
///  
///  Access the grid in style [y,x], but relative to the world it's [z,x] (I like computer graphics so (y,x) is more natural to me than (x,y))
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
                go.transform.position = this.GetWorldPosition(j,i);
                grid[j, i] = go.AddComponent<GridObject>();
                Debug.DrawLine(GetWorldPosition(j,i), GetWorldPosition(j+1,i), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(j, i), GetWorldPosition(j, i+1), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(height, 0), GetWorldPosition(height, width), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(0, width), GetWorldPosition(height, width), Color.white, 100f);
    }

    public Vector3 GetWorldPosition(int y, int x)
    {
        //return new Vector3(y, x) * cell_size;
        return new Vector3(x, 0, y) * cell_size;
    }

    public Vector3 GetWorldPosition(Vector2 yx)
    {
        return new Vector3(yx.x, 0, yx.y) * cell_size;
    }
    public void GetXY(Vector3 world_position, out int y, out int x)
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
    public bool SetValue(int y, int x, GridObject value)
    {
        if (InBounds(y, x))
        {
            grid[y, x] = value;
            return true;
        }
        else
        {
            Debug.LogError("Tried to set grid position [" + y + "," + x + "] which is out of bounds. Height: " + height + ", Width: " + width);
            return false;
        }
    }
    public bool SetValue(Vector3 world_position, GridObject value)
    {
        Vector2Int pos = GetXY(world_position);
        return SetValue(pos.y, pos.x, value);
    }

    public bool SetBuilding(int y, int x, GameObject building)
    {
        if (InBounds(y, x))
        {
            grid[y, x].SetBuilding(building);
            return true;
        }
        else
        {
            Debug.LogError("Tried to set grid position [" + y + "," + x + "] which is out of bounds. Height: " + height + ", Width: " + width);
            return false;
        }
    }

    public bool SetBuilding(Vector3 world_position, GameObject building)
    {
        Vector2Int pos = GetXY(world_position);
        return SetBuilding(pos.y, pos.x, building);
    }
    public bool InBounds(int y, int x)
    {
        return (y >= 0 && y < height) && (x >= 0 && x < width);
    }
}
