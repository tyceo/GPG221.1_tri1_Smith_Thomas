using UnityEngine;
using Anthill.AI;
public class StateBecomeAbleToPickUpFood : AntAIState
{
    private SteeringManager steeringManager;
    private float timer;
    
    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        Debug.Log("StateBecomeAbleToPickUpFood");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        timer = 0f;
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        timer += aDeltaTime;
        
        if (timer >= 3f)
        {
            steeringManager.CanPickUpFood = true;
            Finish();
        }
    }
}
