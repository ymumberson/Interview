using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    private float cell_size;
    private Direction direction;

    public static Direction NextDirection(Direction d)
    {
        if (d == Direction.LEFT) return Direction.UP;
        return d + 1;
    }
    public static int GetDirectionAngle(Direction d)
    {
        switch (d)
        {
            default:
            case Direction.UP: return 0;
            case Direction.DOWN: return 180;
            case Direction.LEFT: return 270;
            case Direction.RIGHT: return 90;
        }
    }

    //public Vector2Int GetDirectionOffset(Direction d)
    //{
    //    switch (d)
    //    {
    //        default:
    //        case Direction.UP: return new Vector2Int(0,0);
    //        case Direction.DOWN: return new Vector2Int(width, height);
    //        case Direction.LEFT: return new Vector2Int(height, 0);
    //        case Direction.RIGHT: return new Vector2Int(0, width);
    //    }
    //}

    /// <summary>
    /// Direction relative to the UP vector
    /// </summary>
    public enum Direction
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3
    }

    private void Awake()
    {
        this.direction = Direction.UP;
    }
    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void SetCellSize(float cell_size)
    {
        this.cell_size = cell_size;
    }

    public List<Vector2Int> GetAllGridPositions(Vector2Int starting_point)
    {
        return GetAllGridPositions(starting_point, this.direction);
    }
    public List<Vector2Int> GetAllGridPositions(Vector2Int starting_point, Direction dir)
    {
        List<Vector2Int> all_positions = new List<Vector2Int>();
        switch (dir)
        {
            case Direction.DOWN:
            case Direction.UP:
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        all_positions.Add(new Vector2Int(i, j) + starting_point);
                    }
                }
                break;
            case Direction.LEFT:
            case Direction.RIGHT:
                for (int i = 0; i < height; ++i)
                {
                    for (int j = 0; j < width; ++j)
                    {
                        all_positions.Add(new Vector2Int(i, j) + starting_point);
                    }
                }
                break;
            default: // == UP
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        all_positions.Add(new Vector2Int(starting_point.x + i, starting_point.y + j));
                    }
                }
                break;
        }
        return all_positions;
    }

    public Direction GetDirection()
    {
        return this.direction;
    }

    public void SetDirection(Direction new_direction)
    {
        this.direction = new_direction;
        this.transform.rotation = Quaternion.Euler(0, Building.GetDirectionAngle(this.direction),0);
    }

    public Vector3 GetDirectionOffset()
    {
        switch (this.direction)
        {
            default:
            case Direction.UP: return new Vector3(0, 0, 0) * this.cell_size;
            case Direction.DOWN: return new Vector3(width, 0, height) * this.cell_size;
            case Direction.LEFT: return new Vector3(height, 0, 0) * this.cell_size;
            case Direction.RIGHT: return new Vector3(0, 0, width) * this.cell_size;
        }
    }

    public void SetPosition(Vector3 world_position)
    {
        this.transform.position = world_position + this.GetDirectionOffset();
    }
    public List<Vector2Int> GetOccupiedTiles(Grid world_grid)
    {
        Vector2Int grid_position = world_grid.GetXY(this.transform.position - this.GetDirectionOffset());
        return this.GetAllGridPositions(grid_position);
    }
}
