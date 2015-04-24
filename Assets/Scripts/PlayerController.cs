using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class PlayerController : MonoBehaviour
{

    public Transform cardboardHead;
    public LineRenderer laser;
    public PIDController controller;

    private Rigidbody rigidbody;

    private float timeSinceLastStep = 5f;

    public float maxForwardVelocity = 8;
    public float minForwardVelocity = 1;

    private float curVel = 0;

    public Transform gazePointer;

    private float laserLength = 0;
    private float laserSpeed = 120f;

    private bool magnetCalibrated = false;

    private float magnetThreshold = 2400;

	// Use this for initialization
	void Start ()
	{
	    Input.compass.enabled = true;
        
        StepDetector.OnStepDetected += OnStepDetected;
	    rigidbody = GetComponent<Rigidbody>();
	}

    void Update()
    {
        //Debug.Log(Input.compass.rawVector.magnitude);
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = cardboardHead.rotation.eulerAngles.y;
        transform.eulerAngles = currentRotation;

        UpdateLaser();

        if(!magnetCalibrated && Vector3.Dot(cardboardHead.transform.forward, Vector3.down) > .95)
        {
            magnetThreshold = Input.compass.rawVector.magnitude - 100;
        }
    }

    void UpdateLaser()
    {
        if (laser != null)
        {
            //Debug.Log(Input.compass.rawVector.magnitude);
            if (Input.GetMouseButton(0) || Input.compass.rawVector.magnitude > magnetThreshold)
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnStepDetected();
        }
        timeSinceLastStep += Time.fixedDeltaTime;

        if (timeSinceLastStep > 1)
        {
            controller.targetVelocity.Set(0, 0, 0);
        }
        else
        {
            if(curVel > 0)
                curVel -= .1f;
            if (curVel < 0)
                curVel = 0;
            controller.targetVelocity.Set(0, 0, curVel);
        }

        

        
    }

    void OnStepDetected()
    {
        //Debug.Log("Time since last step: " + timeSinceLastStep);
        if (timeSinceLastStep > 1)
            curVel = minForwardVelocity;
        else if (timeSinceLastStep < .2)
            curVel = maxForwardVelocity;
        else
        {
            float slope = (minForwardVelocity - maxForwardVelocity)/.8f;
            float yintercept = -slope + minForwardVelocity;
            //Debug.Log("Slope: " + slope + " Yint: " + yintercept);
            curVel = timeSinceLastStep * slope + yintercept;
        }


        controller.targetVelocity.Set(0, 0, curVel);

        timeSinceLastStep = 0;
    }
}
