using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private List<GameObject> objectsInTrigger = new List<GameObject>();
    private int NPCLayer;
    
    
    //detecting humans
    private int humanLayer;
    private List<GameObject> humansInTrigger = new List<GameObject>();
    public float fieldOfViewAngle = 160f;
    
    //detecting all objects for food check
    private List<GameObject> allObjectsInTrigger = new List<GameObject>();
    
    private void Start()
    {
        NPCLayer = LayerMask.NameToLayer("NPC");
        
        humanLayer = LayerMask.NameToLayer("Human");
    }

    private void OnTriggerEnter(Collider other)
    {
        //track all objects in trigger
        if (!allObjectsInTrigger.Contains(other.gameObject))
        {
            allObjectsInTrigger.Add(other.gameObject);
        }
        
        if (other.gameObject.layer == NPCLayer)
        {
            if (!objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Add(other.gameObject);
            }
        }

        //only add objects with "Human" in the name, on the Human layer, and within FOV
        if (other.gameObject.layer == humanLayer && other.gameObject.name.Contains("Human"))
        {
            if (IsInFieldOfView(other.gameObject) && !humansInTrigger.Contains(other.gameObject))
            {
                humansInTrigger.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //remove from all objects list
        if (allObjectsInTrigger.Contains(other.gameObject))
        {
            allObjectsInTrigger.Remove(other.gameObject);
        }
        
        if (other.gameObject.layer == NPCLayer)
        {
            if (objectsInTrigger.Contains(other.gameObject))
            {
                objectsInTrigger.Remove(other.gameObject);
            }
        }
        if (other.gameObject.layer == humanLayer)
        {
            humansInTrigger.Remove(other.gameObject);
        }
    }

    void Update()
    {
        CheckLineOfSight();
        CheckHumansInFieldOfView();
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

    private void CheckHumansInFieldOfView()
    {
        for (int i = humansInTrigger.Count - 1; i >= 0; i--)
        {
            GameObject human = humansInTrigger[i];
            
            if (human == null)
            {
                humansInTrigger.RemoveAt(i);
                continue;
            }

            if (!IsInFieldOfView(human))
            {
                humansInTrigger.RemoveAt(i);
            }
        }
    }

    private bool IsInFieldOfView(GameObject target)
    {
        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        
        return angle <= fieldOfViewAngle / 2f;
    }

    //method to access the list
    public List<GameObject> GetObjectsInTrigger()
    {
        return objectsInTrigger;
    }
    
    public List<GameObject> GetHumansInTrigger()
    {
        return humansInTrigger;
    }
    
    public List<GameObject> GetAllObjectsInTrigger()
    {
        return allObjectsInTrigger;
    }
}
