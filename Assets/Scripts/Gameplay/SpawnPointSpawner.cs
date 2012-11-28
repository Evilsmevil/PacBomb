using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This GO will look for an object with a
/// spawner tag and then pick one at random
/// </summary>
public class SpawnPointSpawner : MonoBehaviour {
	
	//chacked spawn points
	List<GameObject> spawnPoints;
	
	public Bomb bombPrefab;
	
	// Use this for initialization
	void Start () 
	{		
		spawnPoints = new List<GameObject>();
		
		//find any spawn objects and ad them to the list
		GameObject [] objects = GameObject.FindGameObjectsWithTag("SpawnPoint");
		
		spawnPoints.AddRange(objects);
		
		Spawn();		
	
	}

	
	//get a spawn point then creat an object
	void Spawn()
	{
		CreateBomb(GetSpawnPoint().transform.position);
	}
	
	void CreateBomb(Vector3 pos)
	{
		Bomb newBomb = 	(Bomb) Instantiate(bombPrefab, pos, transform.rotation);
		newBomb.transform.parent = transform;
		newBomb.Hit += OnBombHit;
	}
	
	void OnBombHit(Bomb bomb, GameObject other)
	{		
		ScoreKeeper.Instance.AddPoints(10);
		
		
		//DestroyEnemies(bomb);
		//destroy the bomb indicator
		Destroy(bomb.gameObject);
		Spawn();

	}
	
	
	GameObject GetSpawnPoint()
	{
		//get a spawn point at random
		if(spawnPoints != null)
		{
			int rand = Random.Range(0, spawnPoints.Count - 1);
			return spawnPoints[rand];	
			
		}
		else 
		{
			return null;
		}
	}
	
	
	
}
