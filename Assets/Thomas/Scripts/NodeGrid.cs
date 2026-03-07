using UnityEngine;
using System.Collections.Generic;

public class NodeGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2 gridWorldSize = new Vector2(10, 10);
    [SerializeField] private float nodeRadius = 0.5f;
    [SerializeField] private LayerMask wallMask;
    
    [Header("Visualization")]
    [SerializeField] private bool displayGridGizmos = true;
    
    private Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    //updated lists of valid and invalid pathfinding spots
    public List<Node> openList = new List<Node>();
    public List<Node> closedList = new List<Node>();

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        //clear lists before updating
        openList.Clear();
        closedList.Clear();

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                
                //trying checksphere to see if the point is blocked
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, wallMask);
                
                grid[x, y] = new Node(walkable, worldPoint, x, y);

                //sort into open and closed lists
                if (walkable)
                {
                    openList.Add(grid[x, y]);
                }
                else
                {
                    closedList.Add(grid[x, y]);
                }
            }
        }

        Debug.Log("2d world grid created");
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) //set the size and position of the grid
    {
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public Node GetNode(int x, int y)
    {
        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            return grid[x, y];
        }
        return null;
    }

    public int GridSizeX => gridSizeX;
    public int GridSizeY => gridSizeY;

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}

[System.Serializable]
public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gCost = int.MaxValue; //start with high value
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}
