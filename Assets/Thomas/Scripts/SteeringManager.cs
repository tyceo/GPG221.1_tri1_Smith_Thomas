using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SteeringManager : MonoBehaviour
{
    private Avoid forwardAvoid;
    private Avoid leftAvoid;
    private Avoid rightAvoid;

    private GameObject leftObject;
    private GameObject rightObject;

    [Header("Pathfinding")]
    [SerializeField] private NodeGrid nodeGrid;
    [SerializeField] private AstarPathfinding pathfinding;
    [SerializeField] private TurnTowards turnTowards;
    
    [SerializeField] private bool autoPathfind = false; //when the npc reaches the end of the path it will find a new one
    
    [SerializeField] private float pathRecalculateInterval = 5f; //recalculate path every 5 seconds 

    private Vector3 currentGoal;
    private bool hasGoal = false;
    private float timeSinceLastRecalculation = 0f;

    void Start()
    {
        
        Transform forwardTransform = transform.Find("Forward");
        Transform leftTransform = transform.Find("Left");
        Transform rightTransform = transform.Find("Right");
        if (forwardTransform != null)
            forwardAvoid = forwardTransform.GetComponent<Avoid>();
        if (leftTransform != null)
        {
            leftAvoid = leftTransform.GetComponent<Avoid>();
            leftObject = leftTransform.gameObject;
        }
        if (rightTransform != null)
        {
            rightAvoid = rightTransform.GetComponent<Avoid>();
            rightObject = rightTransform.gameObject;
        }
        if (nodeGrid == null)
            nodeGrid = FindObjectOfType<NodeGrid>();
        if (pathfinding == null)
            pathfinding = FindObjectOfType<AstarPathfinding>();
        if (turnTowards == null)
            turnTowards = GetComponent<TurnTowards>();

        //path completion event
        if (turnTowards != null)
        {
            turnTowards.OnPathCompleted += OnPathReached;
        }
    }

    void OnDestroy()
    {
        if (turnTowards != null)
        {
            turnTowards.OnPathCompleted -= OnPathReached;
        }
    }

    void Update()
    {
        //if all 3 antennas are triggered disable left and right ones to stop being stuck
        if (forwardAvoid != null && leftAvoid != null && rightAvoid != null)
        {
            bool allHitting = forwardAvoid.isHitting && leftAvoid.isHitting && rightAvoid.isHitting;
            
            if (allHitting)
            {
                if (leftObject != null && leftObject.activeSelf)
                    leftObject.SetActive(false);
                
                if (rightObject != null && rightObject.activeSelf)
                    rightObject.SetActive(false);
            }
            else
            {
                if (leftObject != null && !leftObject.activeSelf)
                    leftObject.SetActive(true);
                
                if (rightObject != null && !rightObject.activeSelf)
                    rightObject.SetActive(true);
            }
        }

        //manual set path
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PathfindToRandomSpot();
        }

        //recalculate path every X seconds
        if (hasGoal)
        {
            timeSinceLastRecalculation += Time.deltaTime;
            
            if (timeSinceLastRecalculation >= pathRecalculateInterval)
            {
                RecalculatePathToCurrentGoal();
                timeSinceLastRecalculation = 0f;
            }
        }
    }

    //called when path is complete
    void OnPathReached()
    {
        hasGoal = false;
        timeSinceLastRecalculation = 0f;
        
        if (autoPathfind)
        {
            PathfindToRandomSpot();
        }
    }

    void RecalculatePathToCurrentGoal()
    {
        if (!hasGoal || nodeGrid == null || pathfinding == null || turnTowards == null)
        {
            return;
        }
        
        //recalculate the path
        List<Node> path = pathfinding.FindPath(transform.position, currentGoal);
        
        if (path.Count > 0)
        {
            turnTowards.SetPath(path);
        }
        else
        {
            Debug.Log("failed to recalculate path");
            
            //just go somewhere else if the pathfinding fails
            if (autoPathfind)
            {
                PathfindToRandomSpot();
            }
        }
    }

    void PathfindToRandomSpot()
    {
        if (nodeGrid == null || pathfinding == null)
        {
            return;
        }
        if (turnTowards == null)
        {
            return;
        }

        //get a random walkable node
        Node randomNode = GetRandomWalkableNode();
        
        if (randomNode != null)
        {
            currentGoal = randomNode.worldPosition;
            hasGoal = true;
            timeSinceLastRecalculation = 0f;
            
            //calculate path
            List<Node> path = pathfinding.FindPath(transform.position, currentGoal);
            
            if (path.Count > 0)
            {
                turnTowards.SetPath(path);
            }
            else
            {
                Debug.Log("random path failed");
                hasGoal = false;
                
                //try again
                if (autoPathfind)
                {
                    Invoke(nameof(PathfindToRandomSpot), 0.5f);
                }
            }
        }
        else
        {
            Debug.Log("no valid spot to path too");
        }
    }

    Node GetRandomWalkableNode()
    {
        //get all walkable nodes from the grid
        if (nodeGrid.openList.Count == 0)
        {
            Debug.Log("no valid spot to path too");
            return null;
        }

        //picks a random walkable node
        int randomIndex = Random.Range(0, nodeGrid.openList.Count);
        return nodeGrid.openList[randomIndex];
    }
}
