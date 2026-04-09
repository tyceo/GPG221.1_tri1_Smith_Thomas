using UnityEngine;
using Anthill.AI;
public class StateLookForFood : AntAIState
{
    private SteeringManager steeringManager;
    private float timer;
    private float nextPathfindTime;
    
    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        Debug.Log("StateLookForFood");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        timer = 0f;
        nextPathfindTime = Random.Range(5f, 10f);
        //Debug.Log("StateLookForFood TEST HERE");
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        timer += aDeltaTime;
        
        if (timer >= nextPathfindTime)
        {
            steeringManager.PathfindToRandomSpot();
            timer = 0f;
            nextPathfindTime = Random.Range(5f, 10f);
        }
        
        if (steeringManager.CanSeeFood == true)
        {
            Finish();
        }
        
    }
}