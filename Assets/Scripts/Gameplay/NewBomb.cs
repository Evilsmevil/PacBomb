using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// New bomb is much simpler than old bomb
/// basically it has some pellets, each pellet will increase
/// the bomb blast by a given amount
/// </summary>
public class NewBomb : MonoBehaviour {
	
	public string playerTag; //this is the tag that will cause the pellet to get destroyed
	public float minBlastSize;
	public LayerMask enemyLayer;
	public LayerMask bombLayer;
	//you want something
	public GameObject blastSizeIndicatorPrefab;
	protected GameObject blastSizeIndicator;
	//the pellets that are attached to this bomb
	
	public float blastScaleFactor = 4.0f;
	//the number of pellets that have been picked up
	protected List<Pellet> pellets;
	protected int pelletsPickedUp = 0;
	// Use this for initialization
	void Start () 
	{
		pelletsPickedUp = 0;
		
		//make the blast size indicator
		if(blastSizeIndicatorPrefab)
		{
			blastSizeIndicator = Instantiate(blastSizeIndicatorPrefab) as GameObject;
			blastSizeIndicator.transform.parent = this.transform;
			blastSizeIndicator.transform.localPosition = blastSizeIndicatorPrefab.transform.localPosition;
		}
		else
		{
			Debug.LogError("No blast size indicator attached. please attach one");	
		}
	}
	
	public void AddPellets(List<Pellet> newPellets)
	{
		//add the pellets to the current list
		//and then set the callback so we know when 
		//a pellet as been hit
		if(pellets == null)
		{
			pellets = new List<Pellet>(newPellets.Count);	
		}
		foreach(Pellet p in newPellets)
		{
			pellets.Add(p);
			p.Hit = OnHit;
			Debug.Log("adding pellets");
		}
	}
	
	protected void OnHit(Pellet p)
	{
		pelletsPickedUp++;
		
		//find the pellet and remove it from the list
		pellets.Remove(p);
		
		//update the explosion prefab
		UpdateExplosionIndicator();
	}
	
	/// <summary>
	/// Updates the explosion inidicator which shows how big the blast radius will be
	/// </summary>
	protected void UpdateExplosionIndicator()
	{
		//assuming a 1x1x1 unit sphere
		float blastScale = Mathf.Max(GetBlastRadius(), 1);
		blastSizeIndicator.transform.localScale = new Vector3(blastScale,
																  1.0f,
																  blastScale);	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag(playerTag))
		{
			//destroy the remaining pellets
			foreach(Pellet pellet in pellets)
			{
				pellet.ExplodeAsUncollected();	
			}
			
			//destroy the bomb itself
			//TODO work out a way to dispose the delegate without leaking
			Destroy(gameObject);
			Destroy(blastSizeIndicator);
			
			DestroyEnemies();
		}
	}
	
	void DestroyEnemies()
	{
		//find any enemies
		Collider [] objects = Physics.OverlapSphere(transform.position, 
													GetBlastRadius()/2, 
													enemyLayer);	
		Debug.Log("Found " + objects.Length + " enemies");
		//DESTROY
		foreach (Collider c in objects)
		{
			//Give the player the number of enemies they destroyed as points
			//for each enemie - i.e enemies squared
			int enemyScore = ScoreKeeper.Instance.GetEnemyScore(objects.Length, 100);
			ScoreKeeper.Instance.AddEnemyPoints(enemyScore);
			c.gameObject.SendMessage("Kill", enemyScore);
		}	
		
		//get all the bombs that are close enough and blow them up too
		//find any enemies
		Collider [] bombs = Physics.OverlapSphere(transform.position, 
													GetBlastRadius()/2, 
													bombLayer);	
		
		//destroy all the bombs
		
	}
	
	protected IEnumerator DestroyBombs(Collider [] bombs)
	{
			
	}
	
	float GetBlastRadius()
	{
		return pelletsPickedUp * blastScaleFactor;	
	}
	
}
