using UnityEngine;
using System.Collections;

public class collect : MonoBehaviour
{

    private SpawnStuff spawner;

    public void Initialize(SpawnStuff spawn)
    {
        spawner = spawn;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody.tag == "Player")
        {
            Destroy(this.gameObject);
            spawner.Collected();
        }
    }
}
