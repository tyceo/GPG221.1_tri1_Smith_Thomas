using System.Collections;
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

    private Align align;
    private Cohesion cohesion;
    private Separation separation;

    [Header("Pathfinding")]
    [SerializeField] private NodeGrid nodeGrid;
    [SerializeField] private AstarPathfinding pathfinding;
    [SerializeField] private TurnTowards turnTowards;
    
    [SerializeField] private bool autoPathfind = false; //when the npc reaches the end of the path it will find a new one
    
    [SerializeField] private float pathRecalculateInterval = 5f; //recalculate path every 5 seconds 

    [SerializeField] private List<GameObject> targetLocations = new List<GameObject>(); //list of locations to pathfind to
    [SerializeField] private GameObject anthill; //anthill to return to after reaching target
    
    private Vector3 currentGoal;
    private bool hasGoal = false;
    private float timeSinceLastRecalculation = 0f;
    private bool returningToAnthill = false;


    [Header("AI Workbench")] 
    public bool CanSeeFood;
    public bool CanSeeEnemy;
    public bool WantToKillEnemy;
    public bool HasFood;
    public bool CanPickUpFood;
    public bool IsFoodHome;
    public bool IsScared;
    
    

    public Detector detector;
    
    private float maxNeighbourDistance = 10f; 
    void Start()
    {
        detector = GetComponentInChildren<Detector>();
        
        SphereCollider sphereCollider = detector.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            maxNeighbourDistance = sphereCollider.radius;
        }
        
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

        // Get references to steering behavior components
        align = GetComponent<Align>();
        cohesion = GetComponent<Cohesion>();
        separation = GetComponent<Separation>();

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
        // Update CanSeeEnemy based on detector's humansInTrigger list
        if (detector != null)
        {
            CanSeeEnemy = detector.GetHumansInTrigger().Count > 0;
            
            // Update CanSeeFood based on if any detected object is in targetLocations list
            CanSeeFood = CheckIfCanSeeFood();
        }
        
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

        // Disable steering behaviors when left or right objects hit a wall
        if (leftAvoid != null && rightAvoid != null)
        {
            bool leftOrRightHittingWall = IsHittingWall(leftAvoid) || IsHittingWall(rightAvoid);
            
            if (align != null)
                align.enabled = !leftOrRightHittingWall;
            
            if (cohesion != null)
                cohesion.enabled = !leftOrRightHittingWall;
            
            //if (separation != null)
                //separation.enabled = !leftOrRightHittingWall;
        }

        //manual set path
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PathfindToRandomTargetLocation();
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

    private bool IsHittingWall(Avoid avoid)
    {
        if (avoid == null || !avoid.isHitting)
            return false;
        
        // Check if the hit object has the "Wall" layer
        RaycastHit hit;
        if (Physics.Raycast(avoid.transform.position, avoid.transform.forward, out hit, 10f))
        {
            return hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall");
        }
        
        return false;
    }

    private bool CheckIfCanSeeFood()
    {
        if (detector == null || targetLocations.Count == 0)
            return false;
        
        List<GameObject> detectedObjects = detector.GetAllObjectsInTrigger();
        
        foreach (GameObject detectedObj in detectedObjects)
        {
            if (detectedObj != null && targetLocations.Contains(detectedObj))
            {
                return true;
            }
        }
        
        return false;
    }

    //called when path is complete
    void OnPathReached()
    {
        hasGoal = false;
        timeSinceLastRecalculation = 0f;
        
        if (autoPathfind)
        {
            if (HasFood)
            {
                //reached anthill, now go to a random target location
                //returningToAnthill = false;
                
                PathfindToAnthill();
            }
            else
            {
                //reached target location, now return to anthill
                returningToAnthill = true;
                PathfindToRandomTargetLocation();
                
            }
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
            //Debug.Log("failed to recalculate path");
            
            //try again if the pathfinding fails
            if (autoPathfind)
            {
                
                StartCoroutine(TryAgain());
            }
        }
    }

    IEnumerator TryAgain()
    {
        yield return new WaitForSeconds(1f);
        RecalculatePathToCurrentGoal();
        
    }

    public void PathfindToRandomSpot()
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
                //Debug.Log("random path failed");
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
            //Debug.Log("no valid spot to path too");
        }
    }

    Node GetRandomWalkableNode()
    {
        //get all walkable nodes from the grid
        if (nodeGrid.openList.Count == 0)
        {
            //Debug.Log("no valid spot to path too");
            return null;
        }

        //picks a random walkable node
        int randomIndex = Random.Range(0, nodeGrid.openList.Count);
        return nodeGrid.openList[randomIndex];
    }
    
    public void PathfindToRandomTargetLocation()
    {
        if (nodeGrid == null || pathfinding == null || turnTowards == null)
        {
            return;
        }
        
        if (targetLocations.Count == 0)
        {
            Debug.Log("No target locations in list");
            return;
        }
        
        //pick a random target location
        GameObject randomTarget = targetLocations[Random.Range(0, targetLocations.Count)];
        
        if (randomTarget != null)
        {
            currentGoal = randomTarget.transform.position;
            hasGoal = true;
            timeSinceLastRecalculation = 0f;
            returningToAnthill = false;
            
            //calculate path
            List<Node> path = pathfinding.FindPath(transform.position, currentGoal);
            
            if (path.Count > 0)
            {
                turnTowards.SetPath(path);
            }
            else
            {
                //Debug.Log("path to target location failed");
                hasGoal = false;
                
                //try again with another location
                if (autoPathfind)
                {
                    Invoke(nameof(PathfindToRandomTargetLocation), 0.5f);
                }
            }
        }
        else
        {
            Debug.Log("target location is null");
        }
    }
    
    public void PathfindToAnthill()
    {
        if (nodeGrid == null || pathfinding == null || turnTowards == null)
        {
            return;
        }
        
        if (anthill == null)
        {
            Debug.Log("Anthill is not assigned");
            return;
        }
        
        currentGoal = anthill.transform.position;
        hasGoal = true;
        timeSinceLastRecalculation = 0f;
        returningToAnthill = true;
        
        //calculate path
        List<Node> path = pathfinding.FindPath(transform.position, currentGoal);
        
        if (path.Count > 0)
        {
            turnTowards.SetPath(path);
        }
        else
        {
            Debug.Log("path to anthill failed");
            hasGoal = false;
            
            //try again
            if (autoPathfind)
            {
                 Invoke(nameof(PathfindToAnthill), 0.5f);
            }
        }
    }
    
    public void PathfindToNearestTargetLocation()
    {
        if (nodeGrid == null || pathfinding == null || turnTowards == null)
        {
            return;
        }
        
        if (targetLocations.Count == 0)
        {
            Debug.Log("No target locations in list");
            return;
        }
        
        // Find the nearest target location
        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (GameObject target in targetLocations)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = target;
                }
            }
        }
        
        if (nearestTarget != null)
        {
            currentGoal = nearestTarget.transform.position;
            hasGoal = true;
            timeSinceLastRecalculation = 0f;
            returningToAnthill = false;
            
            //calculate path
            List<Node> path = pathfinding.FindPath(transform.position, currentGoal);
            
            if (path.Count > 0)
            {
                turnTowards.SetPath(path);
            }
            else
            {
                //Debug.Log("path to nearest target location failed");
                hasGoal = false;
                
                //try again with random location as fallback
                if (autoPathfind)
                {
                    Invoke(nameof(PathfindToRandomTargetLocation), 0.5f);
                    Debug.Log("BIG TEST");
                }
            }
        }
        else
        {
            Debug.Log("No valid target locations found");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
        // Check if the collided object is in the targetLocations list
        if (targetLocations.Contains(collision.gameObject))
        {
            HasFood = true;
        }
*/
        if (collision.gameObject.name.Contains("Human"))
        {
            CanSeeEnemy = false;
            WantToKillEnemy = false;
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is in the targetLocations list
        if (targetLocations.Contains(other.gameObject))
        {
            float distance = Vector3.Distance(transform.position, other.gameObject.transform.position);
            
            if (distance < 6f)
            {
                HasFood = true;
            }
        }
/*
        if (other.gameObject.name.Contains("Human"))
        {
            CanSeeEnemy = false;
            WantToKillEnemy = false;
            Destroy(other.gameObject);
        }
        */
    }
}
