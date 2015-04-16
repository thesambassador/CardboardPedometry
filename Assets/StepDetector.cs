using System;
using UnityEngine;
using System.Collections;

public delegate void StepHandler();

public class StepDetector : MonoBehaviour
{
    private const int filterNumSamplesToAverage = 4;

    private int filterNumSamples = 0; //number of samples to average to pass them into the step detection
    private float filterTotalX = 0;
    private float filterTotalY = 0;
    private float filterTotalZ = 0;

    private const int dynamicThresholdNumSamplesToUpdate = 50; //number of samples to update the dynamic threshold.
    private int dynamicThresholdNumSamples = 0;
    private float curMin = Single.PositiveInfinity; //minimum value of the last dynamicThresholdNumSamples samples
    private float curMax = Single.NegativeInfinity; //maximum value of the last dynamicThresholdNumSamples samples
    private float dynamicThreshold = 0; //the changing threshold to detect a step

    private float curSampleMax = 0; //the current 

    private float lastSample = 0;
    private float stepMinThreshold = .05f; //minimum threshold for a sample to be passed into the step detection stuff
    private float stepSampleOld = 0; //the last valid sample
    private float stepSampleNew = 0; //the potentially new valid sample, might = old sample if the input sample is below the minimum threshold

    private float timeSinceLastDetectedStep = 10;
    private float stepIntervalMax = 1.5f;
    private float stepIntervalMin = .2f;

    public static StepHandler OnStepDetected;

	// Use this for initialization
	void Start () {
        PlotManager.Instance.PlotCreate("AccelVal", -2, 2, Color.cyan, new Vector2(150, 50));
        PlotManager.Instance.PlotCreate("AccelValX", -2, 2, Color.red, "AccelVal");
        PlotManager.Instance.PlotCreate("AccelValY", -2, 2, Color.green, "AccelVal");
        PlotManager.Instance.PlotCreate("AccelValZ", -2, 2, Color.blue, "AccelVal");
        PlotManager.Instance.PlotCreate("Threshold", -2, 2, Color.magenta, "AccelVal");
        PlotManager.Instance.PlotCreate("zero", -2, 2, Color.black, "AccelVal");
        PlotManager.Instance.PlotCreate("p5", -2, 2, Color.black, "AccelVal");
        PlotManager.Instance.PlotCreate("pn5", -2, 2, Color.black, "AccelVal");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        //update step counter time
        timeSinceLastDetectedStep += Time.fixedDeltaTime;

        //sample once per fixed update, sum 4 samples together and average them to smooth out noise, use the average value of every 4 sums in the step detection
        if (filterNumSamples < filterNumSamplesToAverage)
        {
            filterTotalX += Input.acceleration.x;
            filterTotalY += Input.acceleration.y;
            filterTotalZ += Input.acceleration.z;
            filterNumSamples ++;
        }
        else
        {
            //use the largest of the 3 accelerations for step detection, since the phone might be in a lot of different orientations
            float totalX = filterTotalX / filterNumSamplesToAverage;
            float totalY = filterTotalY / filterNumSamplesToAverage;
            float totalZ = filterTotalZ / filterNumSamplesToAverage;

            float largest = ReturnLargest(totalX, ReturnLargest(totalY, totalZ));

            StepDetection(largest);

            PlotManager.Instance.PlotAdd("AccelVal", largest);
            PlotManager.Instance.PlotAdd("AccelValX", totalX);
            PlotManager.Instance.PlotAdd("AccelValY", totalY);
            PlotManager.Instance.PlotAdd("AccelValZ", totalZ);
            PlotManager.Instance.PlotAdd("Threshold", dynamicThreshold);
            PlotManager.Instance.PlotAdd("zero", 0);
            PlotManager.Instance.PlotAdd("p5", .5f);
            PlotManager.Instance.PlotAdd("pn5", -.5f);

            //reset filter counter and values.
            filterNumSamples = 0;
            filterTotalX = 0;
            filterTotalY = 0;
            filterTotalZ = 0;

            

        }

        //update the dynamic threshold every dynamicThresholdNumSamplesToUpdate steps
        if (dynamicThresholdNumSamples < dynamicThresholdNumSamplesToUpdate)
        {
            float largest = ReturnLargest(Input.acceleration.x, ReturnLargest(Input.acceleration.y, Input.acceleration.z));
            float smallest = ReturnSmallest(Input.acceleration.x, ReturnLargest(Input.acceleration.y, Input.acceleration.z));

            if (largest > curMax) curMax = largest;
            if (smallest < curMin) curMin = smallest;

            dynamicThresholdNumSamples ++;
        }
        else
        {
            dynamicThreshold = (curMax + curMin)/2;

            curMin = Single.PositiveInfinity;
            curMax = Single.NegativeInfinity;
            dynamicThresholdNumSamples = 0;
        }

    }

    void StepDetection(float stepSampleResult)
    {
        stepSampleOld = stepSampleNew; //the old threshold value is always updated with whatever was in "new" before

        //Check to see if the difference in acceleration is at least some threshold, if not, stepSampleNew remains unchanged.
        if (Math.Abs(stepSampleResult - stepSampleNew) > stepMinThreshold)
        {
            stepSampleNew = stepSampleResult;

            //we detect a step if we have a negative slope when acceleration crosses below the dynamic threshold
            if (stepSampleNew < dynamicThreshold && stepSampleNew < stepSampleOld)
            {
                //check to see how long ago our last detected step was so that we avoid unrealistic detections
                if (timeSinceLastDetectedStep > stepIntervalMin)
                {
                    //Step detected!
                    timeSinceLastDetectedStep = 0;
                    OnStepDetected();
                }
            }
        }
    }

    float ReturnLargest(float a, float b)
    {
        if (a > b) return a;
        else return b;
    }

    float ReturnSmallest(float a, float b)
    {
        if (a < b) return a;
        else return b;
    }
}
