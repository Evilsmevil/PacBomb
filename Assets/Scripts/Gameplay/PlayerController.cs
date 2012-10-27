using UnityEngine;
using System.Collections;
using System;

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

    public bool alive = false;
	public bool invicible = false;
    public Action<PlayerController> OnPlayerDied;
	//for debug
	public Vector3 currentVelocity;
	// Use this for initialization
	void Start () {
        alive = true;
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

    public void PlayerHit(GameObject hitter)
    {
        if (OnPlayerDied != null)
        {
            OnPlayerDied(this);
        }
        this.gameObject.SetActiveRecursively(false);
        renderer.enabled = false;
        collider.enabled = false;
        alive = false;
    }

    public void Reset()
    {
        alive = true;
        collider.enabled = true;
        renderer.enabled = true;
        this.gameObject.SetActiveRecursively(true);
        rigidbody.position = new Vector3(0, rigidbody.position.y, 0);
        lastDirection = Vector3.zero;

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
