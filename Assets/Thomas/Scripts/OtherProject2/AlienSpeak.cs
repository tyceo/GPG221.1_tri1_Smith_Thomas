using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SoundEmitterEvent : UnityEvent<SoundEmitter> { }

public class AlienSpeak : MonoBehaviour
{
    public SoundEmitterEvent onNPCDetected;
    private int npcLayer;

    void Awake()
    {
        if (onNPCDetected == null)
        {
            onNPCDetected = new SoundEmitterEvent();
        }
    }

    void Start()
    {
        npcLayer = LayerMask.NameToLayer("NPC");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is on the NPC layer
        if (other.gameObject.layer == npcLayer)
        {
            // Get the SoundEmitter component from the detected NPC
            SoundEmitter soundEmitter = other.GetComponent<SoundEmitter>();
            
            if (soundEmitter != null)
            {
                // Invoke the event, passing the SoundEmitter
                onNPCDetected?.Invoke(soundEmitter);
                Debug.Log($"AlienSpeak detected NPC: {other.gameObject.name}");
            }
        }
    }
}