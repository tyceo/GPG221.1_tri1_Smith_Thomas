using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class HumanSense : MonoBehaviour, ISense
{

    public HumanManager _humanManager;

    private void Awake()
    {
        _humanManager = GetComponentInParent<HumanManager>();
		
    }
    // This method will be called automaticaly each time when AntAIAgent decide to update the plan.


    public void CollectConditions(AntAIAgent aAgent, AntAICondition aWorldState)
    {
        aWorldState.BeginUpdate(aAgent.planner);
        {
            aWorldState.Set(HumanScenario.CanSeeAlien, _humanManager.CanSeeAlien);
            aWorldState.Set(HumanScenario.CanSeeMultipleAliens, _humanManager.CanSeeMultipleAliens);
            aWorldState.Set(HumanScenario.CanSeeMotherShip, _humanManager.CanSeeMotherShip);
            aWorldState.Set(HumanScenario.IsStudyingShip, _humanManager.IsStudyingShip);


        }
        aWorldState.EndUpdate();
		
    }
	
	

	
}