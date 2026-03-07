using UnityEngine;
using System.Collections.Generic;

public class AstarPathfinding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NodeGrid nodeGrid;
    
    [Header("Debug")]
    [SerializeField] private bool showPathGizmos = true;
    [SerializeField] private Color pathColor = Color.cyan;
    
    private List<Node> finalPath = new List<Node>();

    void Start()
    {
        if (nodeGrid == null)
        {
            nodeGrid = FindObjectOfType<NodeGrid>();
        }
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = nodeGrid.NodeFromWorldPoint(startPos);
        Node targetNode = nodeGrid.NodeFromWorldPoint(targetPos);

        //check if start or target are unwalkable
        if (!startNode.walkable || !targetNode.walkable)
        {
            Debug.Log("Start or target position is not walkable");
            return new List<Node>();
        }

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            //pick the one with the lowest fCost
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || 
                    (openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //stop
            if (currentNode == targetNode)
            {
                finalPath = RetracePath(startNode, targetNode);
                return finalPath;
            }

            
            foreach (Node neighbour in GetNeighbours(currentNode))
            {
                //skip blocked or ones in the closed list
                if (!neighbour.walkable || closedList.Contains(neighbour))
                {
                    continue;
                }

                //calculate costs using temporary variables
                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                
                //if new path to neighbour is shorter OR neighbour is not in open list
                if (newGCost < neighbour.gCost || !openList.Contains(neighbour))
                {
                    //calculate hCost (distance to target)
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    
                    //update gCost
                    neighbour.gCost = newGCost;
                    
                    //set parent
                    neighbour.parent = currentNode;

                    //add to open list
                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        //no path found
        Debug.Log("no path found");
        return new List<Node>();
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        //walk back to the starting point using parent references
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        
        //reverse the path so it goes from start to end
        path.Reverse();
        
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        //diagonal distance: 14 for diagonal, 10 for straight
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        //check all 8 directions (including diagonals)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //skip the center node (0,0)
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                //check if within grid bounds
                if (checkX >= 0 && checkX < nodeGrid.MaxSize && 
                    checkY >= 0 && checkY < nodeGrid.MaxSize)
                {
                    Node neighbourNode = nodeGrid.NodeFromWorldPoint(
                        node.worldPosition + new Vector3(x, 0, y)
                    );
                    
                    if (neighbourNode != null)
                    {
                        neighbours.Add(neighbourNode);
                    }
                }
            }
        }

        return neighbours;
    }

    //get the latest path
    public List<Node> GetPath()
    {
        return finalPath;
    }

    void OnDrawGizmos()
    {
        if (!showPathGizmos || finalPath == null || finalPath.Count == 0)
            return;

        Gizmos.color = pathColor;
        for (int i = 0; i < finalPath.Count - 1; i++)
        {
            Gizmos.DrawLine(finalPath[i].worldPosition, finalPath[i + 1].worldPosition);
            Gizmos.DrawSphere(finalPath[i].worldPosition, 0.3f);
        }
        
        if (finalPath.Count > 0)
        {
            Gizmos.DrawSphere(finalPath[finalPath.Count - 1].worldPosition, 0.3f);
        }
    }
}
