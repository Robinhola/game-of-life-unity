using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public GameObject cellPrefab;
    public float randomPercentage = 30;
    public float scalePercentage = 100;
    public float updateTimeMs = 1000;
    public int size_x = 10;
    public int size_y = 5;

    List<GameObject> livingCells = new List<GameObject>();

    float time;
    int[,] grid;
    int[,] neighbours;

    // Start is called before the first frame update
    void Start()
    {
        grid = new int[size_y, size_x];
        neighbours = new int[size_y, size_x];
        initialiseGrid();
        time = 0;
    }

    void initialiseGrid()
    {
        for (int y = 0; y < size_y; y++)
        {
            for (int x = 0; x < size_x; x++)
            {
                grid[y, x] = Random.Range(0, 100) < randomPercentage ? 1 : 0;
            }
        }
    }

    int countIfValid(int x, int y)
    {
        return (x < 0 || size_x <= x || y < 0 || size_y <= y) ? 0 : grid[y, x];
    }

    int countNeighbours(int x, int y)
    {
        int count = 0;
        count += countIfValid(x - 1, y - 1);
        count += countIfValid(x - 0, y - 1);
        count += countIfValid(x + 1, y - 1);

        count += countIfValid(x - 1, y - 0);
        count += countIfValid(x + 1, y - 0);

        count += countIfValid(x - 1, y + 1);
        count += countIfValid(x - 0, y + 1);
        count += countIfValid(x + 1, y + 1);
        return count;
    }

    void computeNeighbours()
    {
        for (int y = 0; y < size_y; y++)
        {
            for (int x = 0; x < size_x; x++)
            {
                neighbours[y, x] = countNeighbours(x, y);
            }
        }
    }

    int updateCell(int x, int y)
    {
        int n = neighbours[y, x];
        if (n < 2) return 0;
        if (n == 2) return grid[y, x];
        if (n == 3) return 1;
        return 0;
    }

    void computeGrid()
    {
        for (int y = 0; y < size_y; y++)
        {
            for (int x = 0; x < size_x; x++)
            {
                grid[y, x] = updateCell(x, y);
            }
        }
    }

    void deleteAllCells()
    {
        livingCells.ForEach(Destroy);
        livingCells.Clear();
    }

    void addNewCells()
    {
        float scale = scalePercentage / 100;
        cellPrefab.transform.localScale = new Vector3(scale, scale, scale);
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                if (grid[y, x] == 1)
                {
                    GameObject cell = Instantiate(cellPrefab, new Vector3(
                        x - size_x / 2, y - size_y / 2, 0) * scale, Quaternion.identity);
                    livingCells.Add( cell );
                }
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= updateTimeMs / 1000)
        {
            computeNeighbours();
            computeGrid();
            deleteAllCells();
            addNewCells();
            time = 0;
        }
    }
}
