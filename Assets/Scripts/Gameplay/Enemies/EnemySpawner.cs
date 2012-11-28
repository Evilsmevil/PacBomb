using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
	
	public List<GameObject> spawnObjects;
    public BombManager bombManager;
	public float timeDelay;
	public float increaseSpeedEvery;
	public float timeStep;
	public float minDelay;
	public float minSpawnDistance;
	
	GameObject player;
	
    protected float initialTimeDelay;
    
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(SpawnEnemy());
		StartCoroutine(IncreaseTime());
	}

    public void Reset()
    {
        StopAllCoroutines();

        timeDelay = initialTimeDelay;
        StartCoroutine(SpawnEnemy());
        StartCoroutine(IncreaseTime());
    }
    
	// Update is called once per frame
	void Awake () 
	{
		//find the player
		player = GameObject.FindGameObjectWithTag("Player");
        initialTimeDelay = timeDelay + timeStep;

	}
	
	IEnumerator SpawnEnemy()
	{
		while(true)
		{
			//spawn a new gameObject somwhere random
			Vector3 newPos = new Vector3(Random.Range(-17.0f,17.0f), 0, Random.Range(-12.0f,12.0f));
			
			if(player!= null)
			{
				while(Vector3.Distance(newPos, player.transform.position) < minSpawnDistance)
				{
					newPos = new Vector3(Random.Range(-17.0f,17.0f), 0, Random.Range(-12.0f,12.0f));
				}
			}
			
            //get a random object
            GameObject nextEnemy = spawnObjects[UnityEngine.Random.Range(0, spawnObjects.Count)];

            GameObject newObj = (GameObject)Instantiate(nextEnemy, newPos, transform.rotation);
			newObj.transform.parent = transform;
		    
            //tell the game manager about the new enemy we spawned
            bombManager.AddEnemy(newObj);

			yield return new WaitForSeconds(timeDelay);
		}
	}
	
	IEnumerator IncreaseTime()
	{
		while(true)
		{
			timeDelay -= timeStep;
			if(timeDelay < minDelay)
			{
				timeDelay = minDelay;	
			}
			yield return new WaitForSeconds(increaseSpeedEvery);
		}
	}

}
