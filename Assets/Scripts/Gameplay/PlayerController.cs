using UnityEngine;
using System.Collections;

/// <summary>
/// This controls the player character
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
		
	//player speed
	public float moveSpeed;
	
	float bulletTimeExpiration;
	
	//Previous input amounts to allow for better keyboard control
	float lastHorizontal;
	float lastVertical;
	
	public bool useLastDirection = true;
	public float deadZone = 0.1f; //the min amount of a direction to consider it pressed
	Vector3 lastDirection;
	
	public bool invicible = false;
	
	//for debug
	public Vector3 currentVelocity;
	// Use this for initialization
	void Start () {
	
		lastDirection = Vector3.zero;
		this.rigidbody.isKinematic = true;
	}
	
	//set the timescale based on the current time
	void Update()
	{
		//SetTimeScale();
		
		//move the player based on input
		MovePlayer();
	}
	
	void MovePlayer()
	{
		//easy way to work out direction could be to look at if horizontal is bigger than vertical
		//then look at if the movement is positive or negative
		float horizontalMovement = Input.GetAxisRaw("Horizontal");
		float verticalMovement = Input.GetAxisRaw("Vertical");
		
		float absHorz = Mathf.Abs(horizontalMovement);
		float absVert = Mathf.Abs(verticalMovement);
		Vector3 direction = Vector3.zero;
		//if horizontal movement then work out the direction
		if(absHorz > absVert && 
			absHorz > deadZone)
		{
			if(horizontalMovement > 0)
			{
				//we are going right		
				direction = PelletDefs.GameRight;
			}
			else
			{
				//we are going left
				direction = PelletDefs.GameLeft;
			}
			
		}
		else if(absVert > deadZone)//process vertical movement
		{
			if(verticalMovement > 0)
			{
				//we are going up	
				direction = PelletDefs.GameUp;
			}
			else
			{
				//we are going down
				direction = PelletDefs.GameDown;
			}
		}

		//if we've not moved the stick keep moving the way we want
		if(absHorz <= deadZone &&
		   absVert <= deadZone &&
			useLastDirection)
		{
			direction = lastDirection;
		}

		lastDirection = direction;
		if(direction != Vector3.zero)
		{
			transform.forward = direction;	
		}
		//we're kinematic so update the position
		currentVelocity = (direction * moveSpeed * Time.deltaTime);
		rigidbody.position += currentVelocity;
	}
	void SetTimeScale()
	{
		if(Time.time <= bulletTimeExpiration)
		{
			//still time for bullet time
			Time.timeScale = 0.5f;	
		}
		else
		{
			Time.timeScale = 1.0f;	
		}
	}
	
	
	public void ModifyMoveSpeedByScalar(float multiplier)
	{
		moveSpeed *= multiplier	;
	}
	
	public void SetMoveSpeedAbsolute(float newSpeed)
	{
		moveSpeed = newSpeed;	
	}
	
	//set the bullet time expiration timer based on what time is is and how
	//long bullet time should last for
	public void SetBulletTimeTimer(float duration)
	{
		bulletTimeExpiration = Time.time + duration;	
	}
	
}
