using UnityEngine;

public class AntsEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("NPC"))
        {
            Destroy(other.gameObject);
        }
    }
}
