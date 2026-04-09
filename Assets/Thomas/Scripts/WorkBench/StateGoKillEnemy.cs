using UnityEngine;
using Anthill.AI;
using System.Collections.Generic;

public class StateGoKillEnemy : AntAIState
{
    private SteeringManager steeringManager;
    private AstarPathfinding pathfinding;
    private TurnTowards turnTowards;
    
    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        pathfinding = Object.FindObjectOfType<AstarPathfinding>();
        turnTowards = aGameObject.GetComponent<TurnTowards>();
        Debug.Log("StateGoKillEnemy");
    }
    
    public override void Enter()
    {
        steeringManager.WantToKillEnemy = true;
        //find all GameObjects with "Human" in their name
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        GameObject nearestHuman = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Human"))
            {
                float distance = Vector3.Distance(steeringManager.transform.position, obj.transform.position);
                
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestHuman = obj;
                }
            }
        }
        
        //if we found a human, pathfind to it
        if (nearestHuman != null && pathfinding != null && turnTowards != null)
        {
            List<Node> path = pathfinding.FindPath(steeringManager.transform.position, nearestHuman.transform.position);
            
            if (path.Count > 0)
            {
                turnTowards.SetPath(path);
                Debug.Log("Path set to nearest Human: " + nearestHuman.name);
            }
            else
            {
                Debug.Log("Failed to find path to Human");
            }
        }
        else
        {
            Debug.Log("No Human found in scene");
            steeringManager.WantToKillEnemy = false;
            Finish();
        }
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        if (steeringManager.WantToKillEnemy == false && steeringManager.CanSeeEnemy == false)
        {
            Finish();
        }
    }
}