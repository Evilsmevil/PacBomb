using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// A pellet is controlled an object that is destroyed when a player
/// runs into it. It is owned by a pellet trail and tells the pellet
/// trail when it gets hit
/// </summary>
public class Pellet : MonoBehaviour {
	
	public string playerTag; //this is the tag that will cause the pellet to get destroyed
					  //if you want something to destory the pellet give the object 
					  //this tag
		
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//this gets registered by the pellet trail so that
	//it knows when it's going to get hit
	public Action<Pellet> Hit;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag(playerTag))
		{
			CollectedByPlayer();
		}
	}
	
	void CollectedByPlayer()
	{
		
		if(Hit != null)
		{
			//our callback
			Hit(this);
		}
		
		Hit = null;
		//destroy the pellet
		//give the player a point for being so good
		ScoreKeeper.Instance.AddPelletPoints(2);
		Destroy(gameObject);
			
	}
	
	public void ExplodeAsUncollected()
	{
		ScoreKeeper.Instance.AddPelletPoints(1);
		Hit = null;

        Vector2 moveDirection = UnityEngine.Random.insideUnitCircle;
        iTween.MoveTo(this.gameObject, new Vector3(moveDirection.x * 7f, 0.0f, moveDirection.y * 7f), 1.0f);  
        iTween.FadeTo(this.gameObject, iTween.Hash("alpha", 0.0f, "time", 0.3f, "onComplete", "fadeComplete", "onCompleteTarget", this.gameObject));
	}

    protected void fadeComplete()
    {
        Destroy(gameObject);

    }
}
