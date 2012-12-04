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

    public SphereCollider effectTriggerCollider;
	//you want something
	public GameObject blastSizeIndicatorPrefab;
	protected GameObject blastSizeIndicator;
	//the pellets that are attached to this bomb
	
	public float blastScaleFactor = 4.0f;
	//the number of pellets that have been picked up
	protected List<Pellet> pellets;
	protected int pelletsPickedUp = 0;
    public bool readyToBlow = false;
    public List<GameObject> LinkIndicators { get; set; }
    protected HashSet<NewBomb> linkedBombs;


    #region soundclips
    public BaseSoundClip bombSoundsPrefab;
    public BaseSoundClip pelletSoundsPrefab;
    public BaseSoundClip allPelletBombSoundPrefab;

    protected BaseSoundClip bombSounds;
    protected BaseSoundClip pelletSounds;
    protected BaseSoundClip allPelletBombSound;
    
    #endregion
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
			blastSizeIndicator.transform.parent = this.transform.parent;
            blastSizeIndicator.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z);
			//blastSizeIndicator.transform.localPosition = blastSizeIndicatorPrefab.transform.localPosition;

            //give it a little tint so it's possible to tell which blast radius is which
            Color startColor = this.renderer.material.color;
            Color tintColor = new Color(startColor.r,
                                        startColor.g,
                                        startColor.b,
                                        blastSizeIndicator.renderer.material.GetColor("_TintColor").a);
            blastSizeIndicator.renderer.material.SetColor("_TintColor", tintColor);
            this.renderer.material.color = this.renderer.material.color.BumpedColour(0.7f);
            

            //blastSizeIndicator.renderer.material.color = tintColor;
        }
		else
		{
			Debug.LogError("No blast size indicator attached. please attach one");	
		}

        LinkIndicators = new List<GameObject>();
        linkedBombs = new HashSet<NewBomb>();

        SetupSounds();
	}

    protected void SetupSounds()
    {
        LoadSoundClip(bombSoundsPrefab, ref bombSounds);
        LoadSoundClip(pelletSoundsPrefab, ref pelletSounds);
        LoadSoundClip(allPelletBombSoundPrefab, ref allPelletBombSound);
        
        /*if (bombSoundsPrefab)
        {
            bombSounds = GameObject.Instantiate(bombSoundsPrefab) as BaseSoundClip;
            bombSounds.transform.parent = this.transform;
        }

        if (pelletSoundsPrefab)
        {
            pelletSounds = GameObject.Instantiate(pelletSoundsPrefab) as BaseSoundClip;
            pelletSounds.transform.parent = this.transform;
        }

        if (allPelletBombSound)
        {
            allPelletBombSound = GameObject.Instantiate(allPelletBombSoundPrefab) as BaseSoundClip;
            allPelletBombSound.transform.parent = this.transform;
        }*/
    }

    protected void LoadSoundClip(BaseSoundClip soundPrefab, ref BaseSoundClip soundVar)
    {
        if (soundPrefab)
        {
            soundVar = GameObject.Instantiate(soundPrefab) as BaseSoundClip;
            soundVar.transform.parent = this.transform;
        }
    }

    public void Reset()
    {
        bombExploded = null;
        pelletPickedUp = null;

        DestroyBombLinks();
        foreach (Pellet p in pellets)
        {
            Destroy(p.gameObject);
        }
        pellets.Clear();
        Destroy(blastSizeIndicator.gameObject);
        Destroy(this.gameObject);
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
			//parent the pellet to the bomb
			p.transform.parent = transform;
		}
	}
	
	protected void OnHit(Pellet p)
	{
		pelletsPickedUp++;
		
		//we can be blown up by other bombs if pick up a pellet
		readyToBlow = true;

        //tell anything that cares that we picked up a pellet
        pelletPickedUp(this);

		//find the pellet and remove it from the list
		pellets.Remove(p);
		
		//update the explosion prefab
		UpdateExplosionIndicator();

        //update the status effect trigger
        UpdateStatusEffectTrigger();

		//play a sound
        if (pelletSounds)
        {
            pelletSounds.PlayNextClip();
        }
	}
    
    protected void UpdateStatusEffectTrigger()
    {
        //get the collider and set the size to the the size of the explosion radius
        if (effectTriggerCollider)
        {
            effectTriggerCollider.radius = (GetBlastRadius() / blastScaleFactor) + 1;
        }

    }
	
	/// <summary>
	/// Updates the explosion inidicator which shows how big the blast radius will be
	/// </summary>
	protected void UpdateExplosionIndicator()
	{
		//assuming a 1x1x1 unit sphere
		float blastScale = Mathf.Max(GetBlastRadius(), 1);
        if (blastSizeIndicator)
        {
            blastSizeIndicator.transform.localScale = new Vector3(blastScale,
                                                                  1.0f,
                                                                  blastScale);
        }
	}

    public void MarkExplodable()
    {
        //Don't do anything for now - maybe we will add a highlight effect later
        //renderer.material.color = Color.red;
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
		//now we have our list we can explode our bombs
		for(int i = 0; i < bombList.Count; ++i)
		{
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
		return bombsInRange;
	}
	
    public void ExplodeBomb(float delay = 0.0f)
    {
        bool pelletsComplete = pellets.Count == 0;
        //destroy the remaining pellets
        foreach (Pellet pellet in pellets)
        {
            pellet.ExplodeAsUncollected();
        }


        //hide the bomb
		this.renderer.enabled = false;
		this.blastSizeIndicator.renderer.enabled = false;
		
		//add to the score - always make bomb worth at least 1 pellets worth of points
		if(pelletsPickedUp == 0) pelletsPickedUp = 1;
		ScoreKeeper.Instance.AddBombPoints(pelletsPickedUp * ScoreKeeper.baseBombPoints);

        //play the right sound depending on if we picked up all the pellets or not
        if (pelletsComplete)
        {
            if (allPelletBombSound)
            {
                allPelletBombSound.PlayNextClip();
            }
        }
        else
        {
            if (bombSounds)
            {
                bombSounds.PlayNextClip();
            }
        }


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

    public void DestroyBombLinks()
    {
        foreach(GameObject go in LinkIndicators)
        {
            if (go) //it might be null because it was destroyed by another bomb
            {
                Destroy(go);
            }
        }

        linkedBombs.Clear();
    }

    public void AddBombLink(NewBomb otherBomb, LineRenderer linkLine)
    {
        //only add the bomb if we've not already added it
        if (linkedBombs.Contains(otherBomb) == false)
        {
            //create a transparent colour for the end of the bomb that is not active
            Color endColor = otherBomb.renderer.material.color;
            Color transparentEnd = new Color(endColor.r, endColor.g, endColor.b, 0.0f);

            //create the link
            LineRenderer newRenderer = Instantiate(linkLine) as LineRenderer;
            newRenderer.SetVertexCount(2);
            Vector3 lineStart = new Vector3(this.transform.position.x,
                                this.transform.position.y - 1,
                                this.transform.position.z);

            Vector3 lineEnd = new Vector3(otherBomb.transform.position.x,
                              otherBomb.transform.position.y - 1,
                              otherBomb.transform.position.z);

            newRenderer.SetPosition(0, lineStart);
            newRenderer.SetPosition(1, lineEnd);
            newRenderer.SetWidth(1.0f, 0.0f);
            newRenderer.SetColors(this.renderer.material.color, otherBomb.renderer.material.color);

            newRenderer.transform.parent = this.transform;
            //we add the line to both origin and destination because we want to to blow up if either of the bombs
            //are destroyed
            this.LinkIndicators.Add(newRenderer.gameObject);
            otherBomb.LinkIndicators.Add(newRenderer.gameObject);

            //add to the set of bombs we are linked to (so we can't do it multiple times)
            linkedBombs.Add(otherBomb);
        }

    }
}
