using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public Transform cardboardHead;

    private Rigidbody rigidbody;

    private float timeSinceLastStep = 0f;

    private Vector3 currentForce = new Vector3();
    public float forwardVelocity = 20;

    private float maxVelocity = 5;

	// Use this for initialization
	void Start () {
        
        StepDetector.OnStepDetected += OnStepDetected;
	    rigidbody = GetComponent<Rigidbody>();
	}

    void Update()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = cardboardHead.rotation.eulerAngles.y;
        transform.eulerAngles = currentRotation;
    }

    void FixedUpdate()
    {
        

        if (timeSinceLastStep > 1)
        {
            currentForce.Set(0, 0, 0);
        }

        Vector3 localVel = transform.InverseTransformDirection(rigidbody.velocity);

        if (localVel.z > maxVelocity)
            currentForce.Set(0, 0, 0);

        if (Mathf.Abs(localVel.x) > .1)
        {

    
        }

        rigidbody.AddRelativeForce(currentForce);

        timeSinceLastStep += Time.fixedDeltaTime;
    }

    void OnStepDetected()
    {
        float forceVal = 10 * (.5f / timeSinceLastStep);

        currentForce.Set(0 , 0, forceVal);

        timeSinceLastStep = 0;
    }
}
