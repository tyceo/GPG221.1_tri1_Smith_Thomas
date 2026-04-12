using UnityEngine;
using Anthill.AI;
public class StateInspectmotherShip : AntAIState
{
    private HumanManager humanManager;
    private MoveForward moveForward;
    public override void Create(GameObject aGameObject)
    {
        humanManager = aGameObject.GetComponent<HumanManager>();
        //Debug.Log("StateRoam");
        
        moveForward = aGameObject.GetComponent<MoveForward>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        moveForward.enabled = false;
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        if (humanManager.CanSeeMotherShip == false || humanManager.CanSeeAlien == true)
        {
            moveForward.enabled = true;
            humanManager.CanSeeMotherShip = false;
            Finish();
        }
        
    }
}