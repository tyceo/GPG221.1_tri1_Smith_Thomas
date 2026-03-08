using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private List<GameObject> objectsInTrigger = new List<GameObject>();
    private int NPCLayer;

    private void Start()
    {
        NPCLayer = LayerMask.NameToLayer("NPC");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == NPCLayer)
        {
            if (!objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == NPCLayer)
        {
            if (objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Remove(other.gameObject);
            }
        }
    }

    void Update()
    {
        CheckLineOfSight();
    }

    private void CheckLineOfSight()
    {
        int layerMask = ~(1 << NPCLayer); 
        
        for (int i = objectsInTrigger.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsInTrigger[i];
            
            if (obj == null)
            {
                objectsInTrigger.RemoveAt(i);
                continue;
            }

            Vector3 direction = obj.transform.position - transform.position;
            float distance = direction.magnitude;

            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, layerMask))
            {
                objectsInTrigger.RemoveAt(i);
            }
        }
    }

    //method to access the list
    public List<GameObject> GetObjectsInTrigger()
    {
        return objectsInTrigger;
    }
}
