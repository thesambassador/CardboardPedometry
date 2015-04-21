using UnityEngine;
using System.Collections;

public class followMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 mousePos = Input.mousePosition;

	    Vector3 newPos = Camera.main.ScreenPointToRay(mousePos).GetPoint(10);

	    //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = newPos;
	}
}
