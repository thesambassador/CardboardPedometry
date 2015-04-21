using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public Transform cardboardHead;
    public LineRenderer laser;

    private Rigidbody rigidbody;

    private float timeSinceLastStep = 0f;

    private Vector3 currentForce = new Vector3();
    public float forwardVelocity = 40;

    private float maxVelocity = 10;

    public Transform gazePointer;

    private float laserLength = 0;
    private float laserSpeed = 100f;

	// Use this for initialization
	void Start ()
	{
	    Input.compass.enabled = true;
        
        StepDetector.OnStepDetected += OnStepDetected;
	    rigidbody = GetComponent<Rigidbody>();
	}

    void Update()
    {
        Debug.Log(Input.compass.rawVector.magnitude);
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = cardboardHead.rotation.eulerAngles.y;
        transform.eulerAngles = currentRotation;

        UpdateLaser();
    }

    void UpdateLaser()
    {
        if (laser != null)
        {
            if (Input.GetMouseButton(0) || Input.compass.rawVector.magnitude > 2400)
            {
                laserLength += laserSpeed*Time.deltaTime;
                laser.enabled = true;
            }
            else
            {
                laser.enabled = false;
                laserLength = 0;
            }
            laser.SetPosition(0, laser.gameObject.transform.position);

            Vector3 targetVector = (gazePointer.position - laser.transform.position).normalized;

            Vector3 newPos = laser.transform.position + targetVector*laserLength;

            laser.SetPosition(1, newPos);
            RaycastHit hit;
            if (Physics.Raycast(laser.transform.position, targetVector, out hit, laserLength))
            {
                EnemyBehavior eb = hit.transform.gameObject.GetComponent<EnemyBehavior>();
                if (eb != null)
                {
                    eb.hp -= 10;
                }
            }
        }
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

        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentForce.z = 10;
        }

        rigidbody.AddRelativeForce(currentForce);

        timeSinceLastStep += Time.fixedDeltaTime;
    }

    void OnStepDetected()
    {
        float forceVal = forwardVelocity * (.5f / timeSinceLastStep);

        currentForce.Set(0 , 0, forceVal);

        timeSinceLastStep = 0;
    }
}
