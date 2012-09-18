using UnityEngine;
using System.Collections;
using System;

public class Bomb : MonoBehaviour {

	
	public string playerTag; //this is the tag that will cause the pellet to get destroyed
	public float minBlastSize;
	
	public enum BombType { 	speed = 1, 
							blastpower, 
							threatlvl, 
							timeslow};
	public BombType bombType;
	
	public static Color speedCol = Color.red;
	public static Color blastCol = Color.yellow;
	public static Color threatCol = Color.green;
	public static Color timeCol = Color.blue;

	

	//if you want something to destory the pellet give the object 
	//this tag
		
	//this gets registered by the pellet trail so that
	//it knows when it's going to get hit
	public event Action<Bomb, GameObject> Hit;
	
	//set the colour based on the bomb type
	void Awake()
	{
		renderer.material.color = GetBombColour(bombType);
	}
	
	//store the bomb type and set the colour as appropriate
	public void SetType(BombType type)
	{
		bombType = type;
		renderer.material.color = GetBombColour(bombType);
	}
	
	Color GetBombColour(BombType type)
	{
		switch(type)
		{
		case BombType.speed:
			return speedCol;
		case BombType.blastpower:
			return blastCol;
		case BombType.threatlvl:
			return threatCol;
		case BombType.timeslow:
			return timeCol;
		default:
			Debug.LogError("Unknown bomb colour requested " + type.ToString());
			return Color.white;
		}
		
	}
	
	/// <summary>
	/// Do the correct thing based on what the bomb powerup type is
	/// </summary>
	public void ApplyBombProperty(GameObject collider)
	{
		//get the player controller from the go
		PlayerController pc = collider.GetComponent<PlayerController>();
		
		switch(bombType)
		{
		case BombType.speed:
			
			if(pc != null)
			{
				BoostPlayerSpeed(pc);
			}
			break;
		case BombType.blastpower:
			BoostPlayerExplosionModifier();
			break;
		case BombType.threatlvl:
			break;
		case BombType.timeslow:
			InitiateBulletTime(pc, 1.0f);
			break;
		default:
			Debug.LogError("Unknown bomb colour requested " + bombType.ToString());
			break;
		}
		
	}
	
	public void BoostPlayerSpeed(PlayerController pc)
	{
		pc.ModifyMoveSpeedByScalar(1.1f);	
	}
	
	public void BoostPlayerExplosionModifier()
	{
		ScoreKeeper.Instance.ModifyExplosionRadByScalar(1.1f);	
	}
	
	public void InitiateBulletTime(PlayerController pc, float duration)
	{
		//set the timescale
		pc.SetBulletTimeTimer(duration);
	}
	
	
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag(playerTag))
		{
			//make sure our delegate exists
			if(Hit != null)
			{
				Hit(this, other.gameObject);
			}
			
			Hit = null;
			
			//destroy the bomb
			Destroy(gameObject);
			
		}
	}
	
	public float GetBlastRadius(int pelletsHit)
	{		
		float radius = Math.Max(minBlastSize, pelletsHit);
		
		//add the modifier
		radius *= ScoreKeeper.Instance.ExplosionRadModifier;
		return radius;
	}
	
	//utility func to generate a random bomb type
	public static BombType GetRandomType()
	{ 
  		BombType[] values = (BombType[]) Enum.GetValues(typeof(BombType));
		//unity int random range is max exclusive (From docs)
  		return values[UnityEngine.Random.Range(0, values.Length)];
	}

}

