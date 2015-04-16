using UnityEngine;
using System.Collections;

public class AcelPlot : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //  Create a new graph named "MouseX", with a range of 0 to 2000, colour green at position 100,100
        PlotManager.Instance.PlotCreate("AccelX", -2, 2, Color.red, new Vector2(0, 0));

        // Create a new child "MouseY" graph.  Colour is red and parent is "MouseX"
        PlotManager.Instance.PlotCreate("AccelY", Color.green, "AccelX");

        PlotManager.Instance.PlotCreate("AccelZ", Color.blue, "AccelX");
	}
	
	// Update is called once per frame
	void Update () {
        // Add data to graphs
        PlotManager.Instance.PlotAdd("AccelX", Input.acceleration.x);
        PlotManager.Instance.PlotAdd("AccelY", Input.acceleration.y);
        PlotManager.Instance.PlotAdd("AccelZ", Input.acceleration.z);
	}
}
