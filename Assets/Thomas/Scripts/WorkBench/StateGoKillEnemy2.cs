using UnityEngine;
using Anthill.AI;
using System.Collections.Generic;
public class StateGoKillEnemy2 : AntAIState
{
    private SteeringManager steeringManager;
    private AstarPathfinding pathfinding;
    private TurnTowards turnTowards;
    private float timeInState = 0f;
    private const float maxTimeInState = 10f;
    
    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        pathfinding = Object.FindObjectOfType<AstarPathfinding>();
        turnTowards = aGameObject.GetComponent<TurnTowards>();
        //Debug.Log("StateGoKillEnemy2");
    }
    
    public override void Enter()
    {
        timeInState = 0f;
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
        
        //if found a human, pathfind to it
        if (nearestHuman != null)
        {
            steeringManager.PathfindToEnemy(nearestHuman);
        }
        else
        {
            //Debug.Log("No Human found in scene");
            steeringManager.WantToKillEnemy = false;
            Finish();
        }
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        timeInState += aDeltaTime;
        
        if (timeInState >= maxTimeInState)
        {
            //Debug.Log("StateGoKillEnemy2 timed out after 10 seconds");
            steeringManager.WantToKillEnemy = false;
            Finish();
            return;
        }
        
        if (steeringManager.WantToKillEnemy == false && steeringManager.CanSeeEnemy == false)
        {
            Finish();
        }
    }
}