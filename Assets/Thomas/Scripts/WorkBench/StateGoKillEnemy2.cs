using UnityEngine;
using Anthill.AI;
public class StateGoKillEnemy2 : AntAIState
{
    private SteeringManager steeringManager;
    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        Debug.Log("StateGoKillEnemy2");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        
        Finish();
    }
}