using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	void OnGUI()
	{
		GUI.Label(new Rect(10,10, 100, 20), "Score: " + ScoreKeeper.Instance.Score);	
		
	}
}