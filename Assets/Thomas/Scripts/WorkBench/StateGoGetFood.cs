using UnityEngine;
using Anthill.AI;
public class StateGoGetFood : AntAIState
{
    private SteeringManager steeringManager;
    
    
    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        //Debug.Log("StateGoGetFood");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        steeringManager.PathfindToNearestTargetLocation();
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        if (steeringManager.HasFood == true)
        {
            Finish();
        }
        
    }
}