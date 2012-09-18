using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public GameObject spawnObject;
	public float timeDelay;
	public float increaseSpeedEvery;
	public float timeStep;
	public float minDelay;
	public float minSpawnDistance;
	
	GameObject player;
	
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(SpawnEnemy());
		StartCoroutine(IncreaseTime());
	}
	
	// Update is called once per frame
	void Awake () 
	{
		//find the player
		player = GameObject.FindGameObjectWithTag("Player");
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
			
			GameObject newObj = (GameObject) Instantiate(spawnObject, newPos, transform.rotation);
			newObj.transform.parent = transform;
		
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
