using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public List<Vector2Int> GetAllGridPositions(Vector2Int starting_point)
    {
        /* Later, this will have to account for direction so that we can rotate objects */
        List<Vector2Int> all_positions = new List<Vector2Int>();
        for (int i=0; i<width; ++i)
        {
            for (int j=0; j<height; ++j)
            {
                all_positions.Add(new Vector2Int(starting_point.x + i, starting_point.y + j));
            }
        }
        return all_positions;
    }
}
