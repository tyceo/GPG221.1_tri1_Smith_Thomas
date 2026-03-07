using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public Rigidbody Rb;
    public float accelerationForce = 10f; 
    public float maxSpeed = 5f; 
    public float slowedSpeed = 2f; //speed when slowed down
    
    public bool isSlowedDown = false;

    void Start()
    {
        Rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        float currentSpeed = Rb.linearVelocity.magnitude;
        
        //speed can be slowed down
        float targetSpeed = isSlowedDown ? slowedSpeed : maxSpeed;
        
        //only apply force if below target speed
        if (currentSpeed < targetSpeed)
        {
            Rb.AddRelativeForce(Vector3.forward * accelerationForce);
        }
        //slow down if going too fast
        else if (currentSpeed > targetSpeed)
        {
            Rb.linearVelocity = Vector3.Lerp(Rb.linearVelocity, Rb.linearVelocity.normalized * targetSpeed, Time.fixedDeltaTime * 5f);
        }
    }

    public void SetSlowedDown(bool slowed)
    {
        isSlowedDown = slowed;
    }
}