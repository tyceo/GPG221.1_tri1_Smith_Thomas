using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private float soundRadius = 5f;
    [SerializeField] private string soundType = "NPC";
    
    public float SoundRadius => soundRadius;
    public string SoundType => soundType;
    
    //maybe
    public Vector3 Position => transform.position;
}