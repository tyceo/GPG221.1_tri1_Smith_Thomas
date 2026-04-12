using UnityEngine;
using Anthill.AI;
public class StateHumanFreezeAndPray : AntAIState
{
    private HumanManager humanManager;
    private MoveForward moveForward;
    
    public override void Create(GameObject aGameObject)
    {
        humanManager = aGameObject.GetComponent<HumanManager>();
        moveForward = aGameObject.GetComponent<MoveForward>();
        //Debug.Log("StateRoam");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        if (moveForward != null)
        {
            moveForward.enabled = false;
        }
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        if (humanManager.CanSeeMultipleAliens == false)
        {
            moveForward.enabled = true;
            Finish();
        }
        
    }
}