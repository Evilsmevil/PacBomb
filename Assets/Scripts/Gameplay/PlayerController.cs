using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    bool returnedToDead;

    //tracking var so we know how much boost to apply at any time
    protected float currentBoostMult = 1.0f;
    protected float baseSpeedMult = 1.0f;

    public PlayerBoost speedBoost;

    List<BaseStatusEffect> effects;

	// Use this for initialization
	void Start () {
        alive = true;
		lastDirection = Vector3.zero;
		this.rigidbody.isKinematic = true;

        speedBoost.Init();

        effects = new List<BaseStatusEffect>();
	}
	
	//set the timescale based on the current time
	void Update()
	{
		//SetTimeScale();

        //reset effects - they must be applied every frame
        ResetEffects();

        ApplyStatusEffects();

        speedBoost.UpdateBoost();

		//move the player based on input
		MovePlayer();




	}

    //this is kind of gross, right now the player controller
    //sort of needs to know about all the effects that could change
    //one way of getting around this could be to provide a list of 
    //public methods through an effects API - this way we know about the things that
    //the statuses can actually change
    protected void ResetEffects()
    {
        baseSpeedMult = 1.0f;
    }
	
	void MovePlayer()
	{
		//easy way to work out direction could be to look at if horizontal is bigger than vertical
		//then look at if the movement is positive or negative
		float horizontalMovement = Input.GetAxisRaw("Horizontal");
		float verticalMovement = Input.GetAxisRaw("Vertical");
		
		float absHorz = Mathf.Abs(horizontalMovement);
		float absVert = Mathf.Abs(verticalMovement);
		Vector3 newDirection = Vector3.zero;
		//if horizontal movement then work out the direction
		if(absHorz > absVert && 
			absHorz > deadZone)
		{
			if(horizontalMovement > 0)
			{
				//we are going right		
				newDirection = PelletDefs.GameRight;
			}
			else
			{
				//we are going left
				newDirection = PelletDefs.GameLeft;
			}
			
		}
		else if(absVert > deadZone)//process vertical movement
		{
			if(verticalMovement > 0)
			{
				//we are going up	
				newDirection = PelletDefs.GameUp;
			}
			else
			{
				//we are going down
				newDirection = PelletDefs.GameDown;
			}
		}

        //If we've pressed the same direction button as the direction we're already
        //going then do a speedboost
        if (speedBoost.SpeedBoostReady() && lastDirection == newDirection && returnedToDead)
        {
            //set speed boost
            speedBoost.SetBoosting();
        }

		//if we've not moved the stick keep moving the way we want
        if (absHorz <= deadZone &&
           absVert <= deadZone &&
            useLastDirection)
        {
            newDirection = lastDirection;
            returnedToDead = true;
        }
        else
        {
            lastDirection = newDirection;
            returnedToDead = false;
        }

        
		if(newDirection != Vector3.zero)
		{
			transform.forward = newDirection;	
		}

		//we're kinematic so update the position
		currentVelocity = (newDirection * (moveSpeed * baseSpeedMult) * speedBoost.GetCurrentBoostMult() * Time.deltaTime);
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
            Debug.Log("player died");
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

        //remove any effects that might have been active
        effects.Clear();

    }
	public void ModifyMoveSpeedByScalar(float multiplier)
	{
		moveSpeed *= multiplier	;
	}

    public void SetBaseSpeedMultiplier(float newMult)
	{
        baseSpeedMult = newMult;
	}
	
	//set the bullet time expiration timer based on what time is is and how
	//long bullet time should last for
	public void SetBulletTimeTimer(float duration)
	{
		bulletTimeExpiration = Time.time + duration;	
	}

    /// <summary>
    /// Apply all status effects here
    /// </summary>
    protected void ApplyStatusEffects()
    {
        //find all our status effects

        Collider [] colliders = Physics.OverlapSphere(transform.position, 1.0f);

        //clear our effects list
        effects.Clear();

        foreach (Collider col in colliders)
        {
            BaseStatusEffect effect = col.GetComponent<BaseStatusEffect>();

            if(effect)
            {
                effects.Add(effect);
            }

        }

        foreach (BaseStatusEffect effect in effects)
        {
            effect.ApplyStatusEffect(this);
        }
    }
	
}
