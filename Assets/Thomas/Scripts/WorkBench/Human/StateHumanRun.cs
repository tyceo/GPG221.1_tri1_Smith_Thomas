using UnityEngine;
using Anthill.AI;
public class StateHumanRun : AntAIState
{
    private HumanManager humanManager;
    private GameObject gameObject;
    
    public override void Create(GameObject aGameObject)
    {
        humanManager = aGameObject.GetComponent<HumanManager>();
        gameObject = aGameObject;
        //Debug.Log("StateRoam");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Enter()
    {
        GameObject closestNPC = FindClosestNPC();
        
        if (closestNPC != null)
        {
            Vector3 directionToNPC = closestNPC.transform.position - gameObject.transform.position;
            directionToNPC.y = 0; //keep rotation on horizontal plane only
            
            Vector3 awayDirection = -directionToNPC;
            
            if (awayDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(awayDirection);
                gameObject.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            }
        }
    }

    private GameObject FindClosestNPC()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        GameObject closestNPC = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("NPC") && obj != gameObject)
            {
                float distance = Vector3.Distance(gameObject.transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNPC = obj;
                }
            }
        }
        
        return closestNPC;
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        
        Finish();
    }
}