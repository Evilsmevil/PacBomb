using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used to create pellet trails
/// </summary>
public class PelletTrailManager : MonoBehaviour {
	
	public PelletTrail startTrail;
	public int minTrailLength;
	public int maxTrailLength;
	public int numberOfTrails = 4;
	public bool destroyAllOtherTrails;
	//the time (in seconds) between destroying the bomb chains
	public float bombDestroyInterval;
	
	List<PelletTrail> trails;
	// Use this for initialization
	void Start () 
	{
		trails = new List<PelletTrail>();
		CreatePelletTrails(numberOfTrails);
	}
	
	/// <summary>
	/// Do something when a trail is destroyed
	/// </summary>
	void OnComplete(PelletTrail trail)
	{
		trails.Remove(trail);
		if(destroyAllOtherTrails)
		{
			StartCoroutine(RefreshTrails());		
		}
	}
	
	IEnumerator RefreshTrails()
	{
		//destroy all the existing trails and then create a new set
		yield return StartCoroutine(DestroyOldTrails());	
		CreatePelletTrails(numberOfTrails);

	}
	
	void CreatePelletTrails(int numTrails)
	{
		//create as many trails as neccesary
		for(int i = 0; i < numTrails; ++i)
		{
				CreateNewTrail(GetTrailLength());
		}
	}
	
	//iterate through the trail list and will detonate all the bombs manually
	IEnumerator DestroyOldTrails()
	{
		foreach(PelletTrail pt in trails)
		{
			//Destroy bomb but indicate we don't want to
			//send a complete message (this causes infinite recursion)
			pt.Detonate(pt.Bomb, false);
			yield return new WaitForSeconds(bombDestroyInterval);
		}
		
		//arraylist now containes invalid refs so clear
		trails.Clear();
		
	}
	
	void CreateNewTrail(int numPellets)
	{
		//Create a new pellet trail
		PelletTrail newTrail = (PelletTrail) Instantiate(startTrail, Vector3.zero, transform.rotation);
		newTrail.CreateTrail();
		//move and rotate
		Vector3 newPos = new Vector3(Random.Range(-1f,1.0f), 0, Random.Range(-1.0f,1.0f));
		newTrail.transform.position = newPos;
		newTrail.transform.Rotate(0, 0, 0);
		newTrail.Complete += OnComplete;
		
		//add to the list for tracking
		trails.Add(newTrail);
	}
	
	/// <summary>
	/// Get the total length of a trail
	/// </summary>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	int GetTrailLength()
	{
		return Random.Range(minTrailLength, maxTrailLength);
	}
}
