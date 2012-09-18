using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
/// <summary>
/// This is a stupid enemy, it just moves towards the player every frame
/// </summary>
public class Homer : MonoBehaviour {
	
	GameObject player;
	public GameObject scoreLabelPrefab;
	public float acceleration;
	// Use this for initialization
	void Start () 
	{
		//find the player
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	/// <summary>
	/// move the homer towards the player
	/// </summary>
	void FixedUpdate()
	{
		if(player != null)
		{
			Vector3 direction = player.transform.position - transform.position;
			rigidbody.AddForce(direction.normalized * acceleration * Time.fixedDeltaTime);
			//rigidbody.velocity += direction.normalized * acceleration * Time.fixedDeltaTime;
			//rigidbody.velocity = direction.normalized * speed * Time.fixedDeltaTime;
		}
		
	}
	
	//if we want to destroy the player
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			//destroy the player
			PlayerController pc = other.GetComponent<PlayerController>();
			{
				if(pc.invicible == false)
				{
					Destroy(other.gameObject);
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
