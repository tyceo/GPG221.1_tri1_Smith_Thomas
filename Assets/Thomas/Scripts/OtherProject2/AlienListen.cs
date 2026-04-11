using UnityEngine;

public class AlienListen : MonoBehaviour
{
    private AlienSpeak alienSpeak;

    void Start()
    {
        // Find the AlienSpeak component on the EarsAndVoice child (including inactive objects)
        alienSpeak = GetComponentInChildren<AlienSpeak>(true);
        
        if (alienSpeak != null)
        {
            alienSpeak.onNPCDetected.AddListener(OnNPCHeard);
        }
        else
        {
            Debug.LogWarning("alienSpeak not found on EarsAndVoice child");
        }
    }

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