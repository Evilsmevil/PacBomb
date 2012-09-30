using UnityEngine;
using System;
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
	
	//sounds
	//noises that the bomb can make
	public AudioClip bleepSound;
	public AudioClip explosionSound;
	
	public float blastScaleFactor = 4.0f;
	//the number of pellets that have been picked up
	protected List<Pellet> pellets;
	protected int pelletsPickedUp = 0;
    public bool readyToBlow = false;

    public Action<NewBomb> bombExploded;
    public Action<NewBomb> pelletPickedUp;
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
		}
	}
	
	protected void OnHit(Pellet p)
	{
		pelletsPickedUp++;
		
		//we can be blown up by other bombs if pick up a pellet
		readyToBlow = true;

        pelletPickedUp(this);

		//find the pellet and remove it from the list
		pellets.Remove(p);
		
		//update the explosion prefab
		UpdateExplosionIndicator();
		
		//play a sound
		Camera.mainCamera.audio.PlayOneShot(bleepSound);
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
		
		/*List<NewBomb> bombsToExplode = new List<NewBomb>();
		FindBombs(bombsToExplode);
		
		foreach(NewBomb bomb in bombsToExplode)
		{
			if(bomb != this && bomb.readyToBlow)
			{
                MarkExplodable();
			}
			bomb.collider.enabled = true;
		}*/
	}

    public void MarkExplodable()
    {
        renderer.material.color = Color.red;
    }
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag(playerTag))
		{
            //call the delegate - bomb manager now deals with 
            //destruction
            if (bombExploded != null) bombExploded(this);
		}
	}
	
	public IEnumerator ExplodeBombs(List<NewBomb> bombList)
	{
		Debug.Log("Going to destroy " + bombList.Count);
		//now we have our list we can explode our bombs
		for(int i = 0; i < bombList.Count; ++i)
		{
			Debug.Log("Exploding " + bombList[i].name + " in " + (i * 0.2f) + " seconds");
			bombList[i].ExplodeBomb(); // this will hide the go's not destroy them
			yield return new WaitForSeconds(0.2f);
		}
		
		//destroy the bomb game objects
		foreach(NewBomb b in bombList)
		{
			Destroy(b.gameObject);	
		}
	}
	
	public void FindBombs(List<NewBomb> explodeList)
	{
		//disable collider
		collider.enabled = false;
		//add to the list
		explodeList.Add(this);
		List<NewBomb> bombsInRange = FindBombsInRange();
		
		//find more bombs
		foreach(NewBomb bomb in bombsInRange)
		{
			//call find bombs on the bombs we've found
			if(explodeList.Contains(bomb) == false)
			{
				bomb.FindBombs(explodeList);
			}
		}
		
	}
	
	public List<NewBomb> FindBombsInRange()
	{
		//find any enemies
		float blastRadius = GetBlastRadius() / 2;
		Collider [] objects = Physics.OverlapSphere(transform.position, 
													GetBlastRadius()/2, 
													bombLayer);	
		List<NewBomb> bombsInRange = new List<NewBomb>();
		foreach(Collider collider in objects)
		{
			NewBomb possibleBomb = collider.GetComponent<NewBomb>();
			if(possibleBomb && possibleBomb.readyToBlow)
			{
				//there is a bomb
				bombsInRange.Add(possibleBomb);
			}
		}
		
		//how many more bombs are there?
		Debug.Log("there are " + bombsInRange.Count + " bombs in range");
		return bombsInRange;
	}
	
    public void ExplodeBomb(float delay = 0.0f)
    {
		//call the delegate
        //if (bombExploded != null) bombExploded(this);
		
		Camera.mainCamera.audio.PlayOneShot(explosionSound);
		
        //destroy the remaining pellets
        foreach (Pellet pellet in pellets)
        {
            pellet.ExplodeAsUncollected();
        }

        //DestroyEnemies();

        //destroy the bomb itself
        //TODO work out a way to dispose the delegate without leaking
        //Destroy(gameObject);
		this.renderer.enabled = false;
		this.blastSizeIndicator.renderer.enabled = false;

    }

    public List<GameObject> FindEnemies()
    {
        //find any enemies
        Collider[] objects = Physics.OverlapSphere(transform.position,
                                                    GetBlastRadius() / 2,
                                                    enemyLayer);

        List<GameObject> enemies = new List<GameObject>();
        foreach (Collider c in objects)
        {
            enemies.Add(c.gameObject);
        }

        return enemies;
    }

	void DestroyEnemies()
	{
        //find any enemies
        Collider[] objects = Physics.OverlapSphere(transform.position,
                                                    GetBlastRadius() / 2,
                                                    enemyLayer);

		//DESTROY
		foreach (Collider c in objects)
		{
			//Give the player the number of enemies they destroyed as points
			//for each enemie - i.e enemies squared
			int enemyScore = ScoreKeeper.Instance.GetEnemyScore(objects.Length, 100);
			ScoreKeeper.Instance.AddEnemyPoints(enemyScore);
			c.gameObject.SendMessage("Kill", enemyScore);
		}	
		
		
	}
	
	public float GetBlastRadius()
	{
		return pelletsPickedUp * blastScaleFactor;	
	}
	
}
