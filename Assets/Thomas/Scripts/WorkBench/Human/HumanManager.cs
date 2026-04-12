using UnityEngine;
using System.Collections.Generic;
public class HumanManager : MonoBehaviour
{
    public bool CanSeeAlien;
    public bool CanSeeMultipleAliens;
    public bool CanSeeMotherShip;
    public bool IsStudyingShip;
    
    public HumanEyes humanEyes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        humanEyes = GetComponentInChildren<HumanEyes>();
    }

    // Update is called once per frame
    void Update()
    {
        if (humanEyes != null)
        {
            CanSeeAlien = humanEyes.GetNPCsInRange().Count == 1;
            CanSeeMultipleAliens = humanEyes.GetNPCsInRange().Count > 1;
        }
    }
}
