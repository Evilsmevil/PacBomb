using UnityEngine;
using System.Collections;

/// <summary>
/// Score labels are used to indicate how much particular things are worth
/// </summary>
[RequireComponent(typeof(TextMesh))]
[RequireComponent(typeof(MeshRenderer))]
public class ScoreLabel : MonoBehaviour {
	
	//the points the label is banging to
	int targetScore;
	
	//the current score being shown on the label
	int currentScore;
	
	float startTime;
	
	//the time to bangup
	public float bangTime;
	
	//time to hold on the final value
	public float holdTime;
	
	TextMesh textMesh;
	
	// Use this for initialization
	void Start () 
	{		
		//connect up the textmesh for easy access
		textMesh = (TextMesh) GetComponent(typeof(TextMesh));
		
		SetLabel(0);
		
		//record the start time
		startTime = Time.time;
	}
	
	public void SetTargetValue(int scoreValue)
	{
		targetScore = scoreValue;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//are we banging?
		float elapsedTime = Time.time - startTime;
		float pctOfBang = elapsedTime/bangTime;
		
		pctOfBang = Mathf.Clamp01(pctOfBang);
		
		currentScore = (int) (targetScore * pctOfBang);
		
		SetLabel(currentScore);
		
		//destroy the label if it's been there long enough
		float totalDisplayTime = bangTime + holdTime;
		if(elapsedTime >= totalDisplayTime)
		{
			Destroy(gameObject);	
		}
		
		
	
	}
	
	//billboard the font, we always want to be looking at the camera
	void LateUpdate()
	{
		//transform.LookAt(Camera.main.transform.position);
	}
	
	void SetLabel(int score)
	{
		textMesh.text = "" + score;
	}
}
