using UnityEngine;
using System.Collections;

/// <summary>
/// This controls a single session
/// is responsible for starting the game
/// and cleaing up when it's done
/// </summary>
public class GameManager : MonoBehaviour {

    public BombManager bombManager;
    public PlayerController player;
    public EnemySpawner enemySpawner;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (player.alive == false)
            {
                RestartGame();
            }
        }
	}

    void RestartGame()
    {
        //restart everything
        bombManager.Reset();
        enemySpawner.Reset();

        //reset the player
        player.Reset();

        ScoreKeeper.Instance.Reset();
    }
}
