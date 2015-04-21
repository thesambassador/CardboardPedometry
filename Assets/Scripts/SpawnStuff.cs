﻿using UnityEngine;
using System.Collections.Generic;

public class SpawnStuff : MonoBehaviour
{

    public GameObject enemyPrefab;
    public Transform playerObject;

    private List<Transform> spawnPoints;

    public float spawnTimer = 0;
    public float spawnTime = 5f;
    public int curEnemies = 0;
    public int maxEnemies = 10;

    private int minHeight = 4;
    private int maxHeight = 15;


	// Use this for initialization
	void Start () {
	    spawnPoints = new List<Transform>();

        foreach(Transform child in transform.GetComponentInChildren<Transform>())
	    {
	        spawnPoints.Add(child);
	    }
	}
	
	// Update is called once per frame
	void Update ()
	{
	    spawnTimer -= Time.deltaTime;

	    if (spawnTimer <= 0)
	    {
	        CreateEnemy();
	        spawnTimer = spawnTime;
	    }

	}

    void CreateEnemy()
    {
        GameObject newEnemy = GameObject.Instantiate(enemyPrefab);
        int rand = Random.Range(0, spawnPoints.Count);

        Vector3 newPos = spawnPoints[rand].position;
        newPos.y = Random.Range(minHeight, maxHeight);
        newEnemy.transform.position = newPos;

        EnemyBehavior eb = newEnemy.GetComponent<EnemyBehavior>();
        eb.Initialize(playerObject, this);

        curEnemies += 1;
    }

    public void EnemyDied()
    {
        curEnemies -= 1;
    }
}
