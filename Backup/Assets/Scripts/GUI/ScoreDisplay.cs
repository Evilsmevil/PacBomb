using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {
    
    string helpText = "M to mute";

	void OnGUI()
	{
        GUI.color = Color.black;
        GUI.Label(new Rect(10, 10, 100, 20), "Score: " + ScoreKeeper.Instance.Score);
        GUI.Label(new Rect(10, 30, 200, 20), "HighScore: " + ScoreKeeper.Instance.GetHighScore());
        GUI.Label(new Rect(10, 50, 500, 20), helpText);
		
	}
}
