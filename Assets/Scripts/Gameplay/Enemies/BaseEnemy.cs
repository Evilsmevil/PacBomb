using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
/// <summary>
/// This is a stupid enemy, it just moves towards the player every frame
/// </summary>
public class BaseEnemy : MonoBehaviour {
	
	protected GameObject player;
	public GameObject scoreLabelPrefab;
	
	// Use this for initialization
	protected virtual void Start () 
	{
		//find the player
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	/// <summary>
	/// move the homer towards the player
	/// </summary>
	protected virtual void FixedUpdate()
	{	
	    //do nothing in base	
	}
	
	//if we want to destroy the player
	protected virtual void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			//destroy the player
			PlayerController pc = other.GetComponent<PlayerController>();
			{
				if(pc.invicible == false)
				{
                    //tell the player they have been hit
                    pc.PlayerHit(this.gameObject);
				}
			}
		}
	}
	
	void Kill(int score)
	{
		CreateScoreLabel(score);
		Destroy(gameObject);
	}
	/// <summary>
	/// create the score label for the enemy that was destroyed
	/// </summary>
	void CreateScoreLabel(int pointValue)
	{
		Vector3 lookDirection = transform.position - Camera.main.transform.position;
		if(scoreLabelPrefab != null)
		{
			GameObject label = (GameObject) Instantiate(scoreLabelPrefab, transform.position, Quaternion.LookRotation(lookDirection));
			ScoreLabel sl = label.GetComponent<ScoreLabel>();
			sl.SetTargetValue(pointValue);
		}	
	}


}
