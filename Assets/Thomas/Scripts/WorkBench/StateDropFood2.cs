using UnityEngine;
using Anthill.AI;
public class StateDropFood2 : AntAIState
{
    private SteeringManager steeringManager;
    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        Debug.Log("StateDropFood2");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        steeringManager.HasFood = false;
        steeringManager.CanPickUpFood = false;
        Finish();
    }
}