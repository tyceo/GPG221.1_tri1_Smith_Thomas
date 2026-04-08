using UnityEngine;
using Anthill.AI;
using Anthill.Utils;

public class NPCSense : MonoBehaviour, ISense
{

    public SteeringManager _NPCManager;

    private void Awake()
    {
        _NPCManager = GetComponentInParent<SteeringManager>();
		
    }
    // This method will be called automaticaly each time when AntAIAgent decide to update the plan.


    public void CollectConditions(AntAIAgent aAgent, AntAICondition aWorldState)
    {
        aWorldState.BeginUpdate(aAgent.planner);
        {
            aWorldState.Set(WorkerAntScenario.CanSeeFood, _NPCManager.CanSeeFood);
            aWorldState.Set(WorkerAntScenario.CanSeeEnemy, _NPCManager.CanSeeEnemy);
            aWorldState.Set(WorkerAntScenario.WantToKillEnemy, _NPCManager.WantToKillEnemy);
            aWorldState.Set(WorkerAntScenario.HasFood, _NPCManager.HasFood);
            aWorldState.Set(WorkerAntScenario.CanPickUpFood, _NPCManager.CanPickUpFood);
            aWorldState.Set(WorkerAntScenario.IsFoodHome, _NPCManager.IsFoodHome);
            aWorldState.Set(WorkerAntScenario.IsScared, _NPCManager.IsScared);

        }
        aWorldState.EndUpdate();
		
    }
	
	

	
}