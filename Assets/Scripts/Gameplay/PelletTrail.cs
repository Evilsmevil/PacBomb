using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PelletTrail : MonoBehaviour {
	
	//Vars for controlling segment characteristics
	public int minPelletsSeg;
	public int maxPelletsSeg;
	public int minSegments;
	public int maxSegments;
	public float minSpacing;
	public float maxSpacing;
	//--------------------------------------------
	
	public Pellet pelletPrefab;
	public Bomb bombPrefab;
	public LayerMask bombLayer;
	List<Pellet> pelletList;

	public GameObject bombBlastPrefab;
	GameObject bombBlastIndicator;
	//keep total number of pellets in the chain, used for scoring
	int totalPellets;
	//keep track of where the bomb is on this segment
	Vector3 bombPos;
		
	public Bomb Bomb {get; set;}
	
	public TrailSegment testSeg;
	
	public List<TrailSegment> segments;
	// Use this for initialization
	void Awake () {
	
		pelletList = new List<Pellet>();
		segments = new List<TrailSegment>();
		
		//create a random trail
		int numSegments = UnityEngine.Random.Range(minSegments,maxSegments);
		float spacing = UnityEngine.Random.Range(minSpacing, maxSpacing);
		
		for(int i = 0; i < numSegments; ++i)
		{
			Vector3 direction = Utils.GetRandomDirection();
			segments.Add(CreateSegment(direction, 	// direction
			                           spacing,				//spacing
			                           UnityEngine.Random.Range(minPelletsSeg, maxPelletsSeg)));				//num pellets
			                                       
		}
		
				
	}
	
	TrailSegment CreateSegment(Vector3 direction, float spacing, int pelletCount)
	{
		TrailSegment newSeg = new TrailSegment();
		
		//work out the start and end points based on the direction and spacing
		newSeg.startPos = new Vector3(0,0,0);
		
		newSeg.endPos = direction.normalized * (spacing * pelletCount);
		newSeg.numPellets = pelletCount;
		
		return newSeg;
	}
		
	
	/// <summary>
	/// Create a trail from the segments 
	/// </summary>
	public void CreateTrail()
	{
		Vector3 currentPos = Vector3.zero;
		foreach(TrailSegment segment in segments)
		{
			//this function returns the end point of the last pellet created
			//so we keep feeding it in until we run out of segments
			CreatePelletsFromSegment(segment, currentPos);
			currentPos += segment.endPos;
		}
		
		//put a bomb on the end
		CreateBomb(currentPos);
		UpdateBlastRadius();

	}
	
	public void CreatePelletsFromSegment(TrailSegment segment, Vector3 startPos)
	{
		//instantiate all the positions
		//get the position list
		List<Vector3> positions = segment.Positions;
		
		foreach(Vector3 p in positions)
		{
			AddPellet(p + startPos);
		}
				
	}	
	
	/// <summary>
	/// Add a new pellet to the trail
	/// </summary>
	/// <param name="pos">
	/// A <see cref="Vector3"/>
	/// </param>
	public void AddPellet(Vector3 pos)
	{
		//create a new pellet from the prefab
		Pellet newPellet = (Pellet)Instantiate(pelletPrefab, Vector3.zero, Quaternion.identity);
		
		//make this manager the parent (makes moving the trail around easier)
		newPellet.transform.parent = transform;
		newPellet.transform.position = pos;
		
		//add the hit listener
		newPellet.Hit += OnPelletHit;
		pelletList.Add(newPellet);
		
		//keep track of the number of pellets
		totalPellets ++;
	}
	
	
	public void CreatePellets(int numPellets)
	{
		for(int i = 0; i < numPellets; ++i)
		{
			Vector3 pos = transform.forward * ((i - numPellets/2) * 2);

			Pellet newPellet = (Pellet)Instantiate(pelletPrefab, Vector3.zero, Quaternion.identity);
			newPellet.transform.parent = transform;
			newPellet.transform.position = pos;
			
			newPellet.Hit += OnPelletHit;
			pelletList.Add(newPellet);
		}
		
		CreateBomb(transform.forward * (numPellets/2) * 2);
		
		totalPellets = pelletList.Count;
		
		UpdateBlastRadius();
	}
	
	void CreateBomb(Vector3 pos)
	{
		Bomb newBomb = 	(Bomb) Instantiate(bombPrefab, pos, transform.rotation);
		newBomb.transform.parent = transform;
		newBomb.Hit += OnBombHit;	
		//set the type of powerup
		newBomb.SetType(Bomb.GetRandomType());
		
		//instantiate the bomb radius indicator
		bombBlastIndicator = (GameObject) Instantiate(bombBlastPrefab, pos, transform.rotation);
		bombBlastIndicator.transform.parent = newBomb.transform;
		
		Bomb = newBomb;
	}
	
	 void OnPelletHit(Pellet p)
	{
		//remove the pellet from the pellet list as it's about to be destroyed
		pelletList.Remove(p);
		
		UpdateBlastRadius();
	}
	
	void OnBombHit(Bomb bomb, GameObject other)
	{
		//if a player has hit this bomb then give them a powerup
		Bomb.ApplyBombProperty(other);
		
		//detonate and indicate we want to tell
		//the manager that we are complete
		Detonate(Bomb, true);
		
	}
	
	public void Detonate(Bomb bomb, bool sendComplete)
	{
		//destroy all the pellets
		foreach(Pellet p in pelletList)
		{
			Destroy(p.gameObject);
			
		}
		
		ScoreKeeper.Instance.AddBombPoints(CalculateScore());
		
		DestroyEnemies(bomb);
		//destroy the bomb indicator
		Destroy(bombBlastIndicator);
		//clear the list
		pelletList.Clear();
		if(sendComplete)
		{
			Complete(this);
		}
		
		Destroy(gameObject);
	}
	
	
	/// <summary>
	/// Currently you get 1 point for each pellet * the number of pellets you got before hitting the bomb
	/// </summary>
	int CalculateScore()
	{
		int score = GetTotalPelletsHit() * GetTotalPelletsHit(); 
		
		return score;
	}
	
	int GetTotalPelletsHit()
	{
		//Debug.Log("total pellets : " + totalPellets + " pellet count "  + pelletList.Count);
		int total = totalPellets - pelletList.Count;
		return total;
	}
	
	public event Action<PelletTrail> Complete;
	
	
	//destroy any enemies within the radius of the blast
	void DestroyEnemies(Bomb bomb)
	{
		//find all the enemies in range and drestroy them
		Vector3 blastPos = GameObject.FindGameObjectWithTag("Bomb").transform.position; 
		Collider [] objects = Physics.OverlapSphere(blastPos, GetBlastRadius(), bombLayer);
		foreach (Collider c in objects)
		{
			//Give the player the number of enemies they destroyed as points
			//for each enemie - i.e enemies squared
			int enemyScore = ScoreKeeper.Instance.GetEnemyScore(objects.Length, 100);
			ScoreKeeper.Instance.AddEnemyPoints(enemyScore);
			c.gameObject.SendMessage("Kill", enemyScore);
		}	
		
	}
	
	float GetBlastRadius()
	{		
		return Bomb.GetBlastRadius(GetTotalPelletsHit());
	}
	
	void UpdateBlastRadius()
	{
		//Draw a sphere around the bomb so we know how big the explosion is going to be	
		if(bombBlastIndicator!= null)
		{
			//set the scale of the bomb circle
			bombBlastIndicator.transform.localScale = new Vector3(GetBlastRadius() * 2,
			                                              bombBlastIndicator.transform.localScale.y,
			                                              GetBlastRadius() * 2);
		}
	}
	
	
	public void OnDrawGizmos()
	{
		bombPos = GameObject.FindGameObjectWithTag("Bomb").transform.position; 
		Gizmos.DrawWireSphere(bombPos, GetBlastRadius());
	}
}

/// <summary>
/// class to hold information about a trail segment
/// </summary>
public class TrailSegment
{
	public int numPellets;
	public Vector3 startPos;
	public Vector3 endPos;
	
	//This segment type creates linear trails
	Vector3 GetPosition(int pelletNum)
	{
		//work out the distance between the two points
		float distance = Vector3.Distance(startPos, endPos);
		
		//Debug.Log("pn " + pelletNum + " num pellets " + numPellets );
		float percentAlong = (float)pelletNum/ (float)numPellets;
		
		Vector3 direction = endPos - startPos;
		
		Vector3 newPos = startPos + (direction.normalized * (percentAlong * distance));
		return newPos;
	}
	
	/// <summary>
	/// generates a list of positions
	/// </summary>
	public List<Vector3> Positions
	{
		//create a list of positions based on what is stored in the trail segment
		get
		{
			List<Vector3> positions = new List<Vector3>();
			
			for(int i = 0; i < numPellets; ++i)
			{
				positions.Add(GetPosition(i));
			}
			
			return positions;
		}
		
	}
	
	public Vector3 direction
	{
		get { 
				Vector3 dir = endPos - startPos;
			   	return dir.normalized;
			}	
	}
	
	public float spacing
	{
		get 
		{
			return (Vector3.Distance(startPos, endPos) / numPellets);
		}
	}
	
	/// <summary>
	/// get the end position
	/// </summary>
	/*public Vector3 endPos
	{
		get 
		{
			return startPos + (direction * (spacing * numPellets));
		}
	}*/
}
