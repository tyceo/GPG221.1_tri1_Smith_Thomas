using UnityEngine;

public class AlienListen : MonoBehaviour
{
    private AlienSpeak alienSpeak;

    void Start()
    {

        alienSpeak = GetComponentInChildren<AlienSpeak>(true);
        
        //add listener for when another NPC/ Alien is detected
        if (alienSpeak != null)
        {
            alienSpeak.onNPCDetected.AddListener(OnNPCHeard);
        }
        else
        {
            Debug.LogWarning("alienSpeak not found on EarsAndVoice child");
        }
    }

    //if another alien is scared they scream and make all nearby aliens scared as well and then work together to kill the human
    private void OnNPCHeard(SoundEmitter soundEmitter)
    {
        SteeringManager detectedNPCSteeringManager = soundEmitter.GetComponent<SteeringManager>();
        
        if (detectedNPCSteeringManager != null)
        {
            detectedNPCSteeringManager.IsScared = true;
        }
    }

    private void OnDestroy()
    {
        if (alienSpeak != null)
        {
            alienSpeak.onNPCDetected.RemoveListener(OnNPCHeard);
        }
    }
}