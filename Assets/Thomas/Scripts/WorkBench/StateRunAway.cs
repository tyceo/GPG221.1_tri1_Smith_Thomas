using UnityEngine;
using Anthill.AI;
using System.Collections;
public class StateRunAway : AntAIState
{
    private SteeringManager steeringManager;
    private GameObject earsAndVoice;
    private GameObject npcAnt;

    public override void Create(GameObject aGameObject)
    {
        steeringManager = aGameObject.GetComponent<SteeringManager>();
        npcAnt = aGameObject;
        
        //find EarsAndVoice as a child of the NPCAnt GameObject
        Transform earsAndVoiceTransform = aGameObject.transform.Find("EarsAndVoice");
        if (earsAndVoiceTransform != null)
        {
            earsAndVoice = earsAndVoiceTransform.gameObject;
            //Debug.Log("Found EarsAndVoice");
        }
        else
        {
            //Debug.LogWarning("EarsAndVoice GameObject not found! Searching in children...");
            //recursively
            earsAndVoiceTransform = aGameObject.transform.Find("EarsAndVoice");
            if (earsAndVoiceTransform == null)
            {
                foreach (Transform child in aGameObject.GetComponentsInChildren<Transform>())
                {
                    if (child.name == "EarsAndVoice")
                    {
                        earsAndVoice = child.gameObject;
                        //Debug.Log("Found EarsAndVoice in children");
                        break;
                    }
                }
            }
        }
        
        //Debug.Log("StateRunAway");
    }

    public override void Enter()
    {
        if (earsAndVoice != null && npcAnt != null)
        {
            //use the SteeringManager to run the coroutine
            if (steeringManager != null)
            {
                steeringManager.StartCoroutine(ShowEarsAndVoiceTemporarily());
            }
            else
            {
                Debug.LogWarning("SteeringManager not found for coroutine");
            }
        }
        else
        {
            Debug.LogWarning("EarsAndVoice or NPCAnt reference is null");
        }
    }

    public override void Execute(float aDeltaTime, float aTimeScale)
    {
        if (steeringManager.IsScared == false)
        {
            steeringManager.WantToKillEnemy = true;
            Finish();
        }
        
    }

    private IEnumerator ShowEarsAndVoiceTemporarily()
    {
        earsAndVoice.SetActive(true);
        steeringManager.PathfindToRandomSpot();
        yield return new WaitForSeconds(2f);
        earsAndVoice.SetActive(false);
        steeringManager.IsScared = false;
    }
}