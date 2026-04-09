using UnityEngine;

public class AntsEnd : MonoBehaviour
{

    private void Awake()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("NPC"))
        {
            SteeringManager steeringManager = other.gameObject.GetComponent<SteeringManager>();
            
            if (steeringManager != null && steeringManager.HasFood == true)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
