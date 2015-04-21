using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public SpawnStuff spawner;
    public LookAtTarget look;
    public Transform target;

    public int hp = 50;

    private Rigidbody _rigidbody;

    public void Initialize(Transform newTarget, SpawnStuff sp)
    {
        look.target = newTarget;
        target = newTarget;
        spawner = sp;
    }

	// Use this for initialization
	void Start ()
	{
	    _rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
    void Update()
    {
        Vector3 horizontalPosition = target.position;
        horizontalPosition.y = transform.position.y;
        if (Vector3.Distance(transform.position, horizontalPosition) > 8)
        {
            Vector3 playPosNoY = target.position;
            playPosNoY.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, playPosNoY, .1f);
        }
        
        if(hp <= 0) Die();
	}

    void Die()
    {
        spawner.EnemyDied();
        Destroy(this.gameObject);
    }
}
