using UnityEngine;
using System.Collections.Generic;

public class Spawning : MonoBehaviour
{
    public float spacing = 2f; 
    public int gridSize = 5;   
    public Transform spawnOrigin;
    public List<GameObject> prefabs;
    
    [Header("Debug Visualization")]
    [SerializeField] public bool showAllNPCPaths = false;
    [SerializeField] public bool showAlignDebug = false;
    [SerializeField] public bool showAvoidDebug = false;
    [SerializeField] public bool showSeparationDebug = false;
    [SerializeField] public bool showCohesionDebug = false;
    
    private List<TurnTowards> spawnedNPCs = new List<TurnTowards>();

    public void ToggleShowAllPaths()
    {
        showAllNPCPaths = !showAllNPCPaths;
    }
    
    public void ToggleShowAllAlignDebug()
    {
        showAlignDebug = !showAlignDebug;
        UpdateAlignDebugForAllNPCs();
    }
    
    public void ToggleShowAllAvoidDebug()
    {
        showAvoidDebug = !showAvoidDebug;
        UpdateAvoidDebugForAllNPCs();
    }
    
    public void ToggleShowAllSeparationDebug()
    {
        showSeparationDebug = !showSeparationDebug;
        UpdateSeparationDebugForAllNPCs();
    }   
    
    public void ToggleShowAllCohesionDebug()
    {
        showCohesionDebug = !showCohesionDebug;
        UpdateCohesionDebugForAllNPCs();
    }  
    
    void Start()
    {
        SpawnInGrid();
    }

    void Update()
    {

    }
    
    void UpdateAlignDebugForAllNPCs()
    {
        foreach (TurnTowards npc in spawnedNPCs)
        {
            if (npc != null)
            {
                Align align = npc.GetComponent<Align>();
                if (align != null)
                {
                    align.showDebugRay = showAlignDebug;
                }
            }
        }
    }
    
    void UpdateAvoidDebugForAllNPCs()
    {
        foreach (TurnTowards npc in spawnedNPCs)
        {
            if (npc != null)
            {
                Avoid[] avoidScripts = npc.GetComponentsInChildren<Avoid>();
                foreach (Avoid avoid in avoidScripts)
                {
                    avoid.showDebugRay = showAvoidDebug;
                }
            }
        }
    }
    
    void UpdateSeparationDebugForAllNPCs()
    {
        foreach (TurnTowards npc in spawnedNPCs)
        {
            if (npc != null)
            {
                Separation separation = npc.GetComponent<Separation>();
                if (separation != null)
                {
                    separation.showDebugRay = showSeparationDebug;
                }
            }
        }
    }
    
    void UpdateCohesionDebugForAllNPCs()
    {
        foreach (TurnTowards npc in spawnedNPCs)
        {
            if (npc != null)
            {
                Cohesion cohesion = npc.GetComponent<Cohesion>();
                if (cohesion != null)
                {
                    cohesion.showDebugRay = showCohesionDebug;
                }
            }
        }
    }
    
    public void PathfindAllNPCs()
    {
        foreach (TurnTowards npc in spawnedNPCs)
        {
            if (npc != null)
            {
                SteeringManager steeringManager = npc.GetComponent<SteeringManager>();
                if (steeringManager != null)
                {
                    steeringManager.PathfindToRandomSpot();
                }
            }
        }
    }
    
    public void PathfindRandomNPC()
    {
        if (spawnedNPCs.Count == 0)
        {
            Debug.Log("No NPCs to pathfind");
            return;
        }
        
        //1 npc goes to a random spot
        TurnTowards randomNPC = spawnedNPCs[Random.Range(0, spawnedNPCs.Count)];
        
        if (randomNPC != null)
        {
            SteeringManager steeringManager = randomNPC.GetComponent<SteeringManager>();
            if (steeringManager != null)
            {
                steeringManager.PathfindToRandomSpot();
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showAllNPCPaths) return;
        
        //draw paths for all spawned NPCs
        foreach (TurnTowards npc in spawnedNPCs)
        {
            if (npc != null)
            {
                npc.DrawPathGizmos();
            }
        }
    }
    
    void SpawnInGrid()
    {
        Vector3 startPos = spawnOrigin != null ? spawnOrigin.position : transform.position;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = startPos + new Vector3(x * spacing, 0, z * spacing);
                GameObject selectedPrefab = prefabs[Random.Range(0, prefabs.Count)];
                
                // the only time I haven't used rb to rotate the object 
                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                
                GameObject spawnedNPC = Instantiate(selectedPrefab, position, randomRotation);
                
                //track the TurnTowards component if it exists
                TurnTowards turnTowards = spawnedNPC.GetComponent<TurnTowards>();
                if (turnTowards != null)
                {
                    spawnedNPCs.Add(turnTowards);
                }
            }
        }
    }
}
