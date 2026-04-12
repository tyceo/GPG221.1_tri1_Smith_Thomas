using UnityEngine;
using System.Collections.Generic;

public class HumanEyes : MonoBehaviour
{
    private List<GameObject> npcsInRange = new List<GameObject>();
    
    public HumanManager humanManager;
    private void Start()
    {
        humanManager = GetComponentInParent<HumanManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //check if the object's name contains "NPC"
        if (other.gameObject.name.Contains("NPC"))
        {
            if (!npcsInRange.Contains(other.gameObject))
            {
                npcsInRange.Add(other.gameObject);
            }
        }

        if (other.gameObject.name.Contains("hill")) //this is the rocket ship
        {
            humanManager.CanSeeMotherShip = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object's name contains "NPC"
        if (other.gameObject.name.Contains("NPC"))
        {
            // Remove from list
            if (npcsInRange.Remove(other.gameObject))
            {
                //Debug.Log($"NPC left: {other.gameObject.name}. Total NPCs: {npcsInRange.Count}");
            }
        }

        if (other.gameObject.name.Contains("Rocket"))
        {
            humanManager.CanSeeMotherShip = false;
        }
    }
    
    public List<GameObject> GetNPCsInRange()
    {
        return npcsInRange;
    }
}