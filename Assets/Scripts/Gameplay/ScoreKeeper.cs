using UnityEngine;
using System.Collections;

public class ScoreKeeper
{
    private static ScoreKeeper instance;
	public static int pointsPerPellet = 100;
	public static int baseBombPoints = 50;
    public static int maxMultiplier = 10;
    static string HighScorePrefString = "HighScore";
    public ScoreKeeper () 
    {
        if (instance != null)
        {
            Debug.LogError ("Cannot have two instances of singleton. Self destruction in 3...");
            return;
        }
        //set the highScore
        highScore = PlayerPrefs.GetInt(HighScorePrefString, 0);
        instance = this;
    }
    
    public static ScoreKeeper Instance
    {
        get
        {
            if (instance == null)
            {
                new ScoreKeeper ();
            }
            
            return instance;
        }
    }
	
	public int GetEnemyScore(int numEnemies, int baseEnemyValue)
	{
        //cap the maximum mult value
        int multiplier = Mathf.Min(numEnemies, maxMultiplier);

        return multiplier * baseEnemyValue;
	}
	
	int score;		//total score
	int enemyScore; //num points awarded by killing enemies
	int pelletScore;//num points awarded by pellets
	int bombScore;  //num points awarded by hitting bombs
	int multiplier = 1;

    int highScore = 0;
	//the bonus explosion radius that the player currently has (percentage)
	float explosionRadModifier = 1.0f;
	
	public int Score
	{
		get { return score;}
	}
	
	public float ExplosionRadModifier
	{	
		get { return explosionRadModifier; }
	}
	
	public void ModifyExplosionRadByScalar(float multiplier)
	{
		explosionRadModifier *= multiplier;
	}
	
	public int Multiplier 
	{
		get { return multiplier;}
		set { multiplier = Mathf.Max(1, value);}
	}
	
	public void AddEnemyPoints(int points)
	{
		AddPoints(points);
		enemyScore += points + multiplier;
	}
	
	public void AddPelletPoints(int points)
	{
		AddPoints(points);
		pelletScore += points + multiplier;
	}
	
	public void AddBombPoints(int points)
	{
		AddPoints(points);
		bombScore += points * multiplier;;
	}
	
	public void AddPoints(int points)
	{
		score += points * multiplier;	

        //have we broken the highscore?
        if (score >= highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScorePrefString, highScore);
        }
	}
	
	public string GetPointsBreakdown()
	{
		return "Pellets : " + pelletScore
			+  " Bombs : " + bombScore
			+  " enemies : " + enemyScore;
						
	}

    public void Reset()
    {
        score = 0;	//total score
        multiplier = 1;
        //set the highScore
        highScore = PlayerPrefs.GetInt(HighScorePrefString, 0);

    }

    public int GetHighScore()
    {
        return highScore;
    }
}

	
	