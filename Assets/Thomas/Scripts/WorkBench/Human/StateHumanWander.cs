using UnityEngine;
using Anthill.AI;
public class StateHumanWander : AntAIState
{
    private HumanManager humanManager;
    public override void Create(GameObject aGameObject)
    {
        humanManager = aGameObject.GetComponent<HumanManager>();
        //Debug.Log("StateRoam");
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