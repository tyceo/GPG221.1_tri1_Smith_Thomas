using System.Collections.Generic;
using UnityEngine;

public class Align : MonoBehaviour
{
    public Detector detector;
    public Rigidbody rb;
    public float force = 100f;
    public bool showDebugRay = true;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        if (detector == null)
        {
            detector = GetComponentInChildren<Detector>();
        }
    }

    void FixedUpdate()
    {
        List<GameObject> neighbours = detector.GetObjectsInTrigger();
        
	
        Vector3 targetDirection = CalculateMove(neighbours);
		
        //into a rotation force vector
        Vector3 cross = Vector3.Cross(transform.forward, targetDirection);

        if (showDebugRay)
        {
            //where it wants to face
            Debug.DrawRay(transform.position, targetDirection * 10f, Color.blue);
            
            //where it is facing
            Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);
        }

        rb.AddTorque(cross * force);
    }

    public Vector3 CalculateMove(List<GameObject> neighbours)
    {
        if (neighbours.Count == 0)
            return Vector3.zero;

        Vector3 alignmentDirection = Vector3.zero;

        //average of all neighbours directions
        foreach (GameObject item in neighbours)
        {
            alignmentDirection += item.transform.forward;
        }

        alignmentDirection /= neighbours.Count;

        return alignmentDirection;
    }
}