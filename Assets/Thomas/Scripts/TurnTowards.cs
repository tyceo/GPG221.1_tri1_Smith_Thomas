using UnityEngine;
using System.Collections.Generic;
using System;

public class TurnTowards : MonoBehaviour
{
    public Vector3 TempPosition;
    
    [SerializeField] private float baseRotationSpeed = 2f;
    [SerializeField] private float angleBasedSpeedMultiplier = 1f;
    [SerializeField] private float maxAngularVelocity = 10f;
    [SerializeField] private float waypointReachDistance = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLine = true;
    [SerializeField] private Color debugLineColor = Color.green;
    [SerializeField] private Color pathColor = Color.yellow;
    [SerializeField] private bool showAllNodes = false; 

    
    private Vector3 targetPosition;
    private Vector3 finalDestination;
    private bool hasTarget = false;
    private bool shouldRotate = true;
    private bool hasFinalDestination = false;
    private Rigidbody rb;

   
    private List<Vector3> turningPoints = new List<Vector3>();
    private List<Node> originalPath = new List<Node>(); //for debug visualization
    private int currentWaypointIndex = 0;
    private bool isFollowingPath = false;

    //event for when path is completed
    public event Action OnPathCompleted;

    //pathfinding script can send a new target position to this script or path
    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
        isFollowingPath = false;
    }

    public void SetPath(List<Node> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.Log("Trying to set empty or null path");
            return;
        }

        originalPath = new List<Node>(path);
        turningPoints = ExtractTurningPoints(path);
        
        if (turningPoints.Count == 0)
        {
            Debug.Log("No turning points found in path");
            return;
        }

        currentWaypointIndex = 0;
        isFollowingPath = true;
        hasTarget = true;

        
        targetPosition = turningPoints[currentWaypointIndex];
        
        finalDestination = turningPoints[turningPoints.Count - 1];
        hasFinalDestination = true;
        
    }

    private List<Vector3> ExtractTurningPoints(List<Node> path)
    {
        List<Vector3> points = new List<Vector3>();
        
        if (path.Count == 0) return points;
        
        points.Add(path[0].worldPosition);
        
        if (path.Count == 1) return points;
        
        //check for direction changes
        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector3 directionBefore = (path[i].worldPosition - path[i - 1].worldPosition).normalized;
            Vector3 directionAfter = (path[i + 1].worldPosition - path[i].worldPosition).normalized;
            
            //if not moving in same direction
            if (Vector3.Dot(directionBefore, directionAfter) < 0.99f)
            {
                points.Add(path[i].worldPosition);
            }
        }
        
        //add last point
        points.Add(path[path.Count - 1].worldPosition);
        
        return points;
    }

    public void ClearPath()
    {
        turningPoints.Clear();
        originalPath.Clear();
        currentWaypointIndex = 0;
        isFollowingPath = false;
        hasTarget = false;
        hasFinalDestination = false;
    }

    public void SetFinalDestination(Vector3 position)
    {
        finalDestination = position;
        hasFinalDestination = true;
    }

    public void StopTurning()
    {
        shouldRotate = false;
    }

    public void StartTurning()
    {
        shouldRotate = true;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.Log("TurnTowards requires a Rigidbody component");
        }
        else
        {
            //set max angular velocity
            rb.maxAngularVelocity = maxAngularVelocity;
        }
        
        if (TempPosition != Vector3.zero)
        {
            SetTarget(TempPosition);
        }
    }

    private void FixedUpdate()
    {
        if (!hasTarget || !shouldRotate || rb == null) return;

        //reached the current waypoint
        if (isFollowingPath && turningPoints.Count > 0)
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, targetPosition);
            

            if (distanceToWaypoint <= waypointReachDistance)
            {
                currentWaypointIndex++;
                

                if (currentWaypointIndex >= turningPoints.Count)
                {

                    ClearPath(); //show path
                    
                    //event for path being done
                    OnPathCompleted?.Invoke();
                    return;
                }
                

                targetPosition = turningPoints[currentWaypointIndex];

            }
        }

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        directionToTarget.y = 0;
        
        if (directionToTarget != Vector3.zero)  //turns harder the further away it faces from its target
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            
            //calculate angle difference 0-180 degrees
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            
            //normalize angle
            float angleMultiplier = Mathf.Clamp01(angleDifference / 180f);
            
            
            float angleBasedSpeed = Mathf.Lerp(1f, angleBasedSpeedMultiplier, angleMultiplier);
            
            //final rotation speed
            float currentRotationSpeed = baseRotationSpeed * angleBasedSpeed;
            
            //determine turn direction
            Vector3 cross = Vector3.Cross(transform.forward, directionToTarget);
            float turnDirection = Mathf.Sign(cross.y);
            
            //apply to rigidbody
            Vector3 torque = new Vector3(0, turnDirection * currentRotationSpeed, 0);
            rb.AddTorque(torque);
        }
    }

    private void OnDrawGizmos()
    {
        //path visualization
        if (!showDebugLine) return;

        //draw current target
        if (hasTarget)
        {
            Gizmos.color = debugLineColor;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }

        //draw the original path
        if (showAllNodes && isFollowingPath && originalPath.Count > 0)
        {
            Gizmos.color = Color.gray;
            
            for (int i = 0; i < originalPath.Count - 1; i++)
            {
                Gizmos.DrawLine(originalPath[i].worldPosition, originalPath[i + 1].worldPosition);
            }
            
            for (int i = 0; i < originalPath.Count; i++)
            {
                Gizmos.DrawWireSphere(originalPath[i].worldPosition, 0.2f);
            }
        }

        //draw just the edges
        if (isFollowingPath && turningPoints.Count > 0)
        {
            Gizmos.color = pathColor;
            
            //draw lines
            for (int i = 0; i < turningPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(turningPoints[i], turningPoints[i + 1]);
            }
            
            //draw spheres
            for (int i = 0; i < turningPoints.Count; i++)
            {
                if (i < currentWaypointIndex)
                    Gizmos.color = Color.gray; 
                else if (i == currentWaypointIndex)
                    Gizmos.color = Color.green; 
                else
                    Gizmos.color = pathColor; 
                
                Gizmos.DrawWireSphere(turningPoints[i], 0.4f);
            }
        }

        //final destination
        if (hasFinalDestination)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(finalDestination, 0.75f);
        }
    }

    //might use/ not needed anynmore
    public bool IsTargetReached()
    {
        if (!hasTarget) return false;
        
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToTarget);
        return dot >= 0.99f;
    }

    public bool IsPathComplete()
    {
        return !isFollowingPath;
    }

    public int GetCurrentWaypointIndex()
    {
        return currentWaypointIndex;
    }

    public int GetTotalWaypoints()
    {
        return turningPoints.Count;
    }

    public bool IsRotating => shouldRotate;
}