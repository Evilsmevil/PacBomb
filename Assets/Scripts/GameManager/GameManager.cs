using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This controls a single session
/// is responsible for starting the game
/// and cleaing up when it's done
/// </summary>
public class GameManager : MonoBehaviour {

    public BombManager bombManager;
    public PlayerController player;
    public EnemySpawner enemySpawner;
    public GameObject gameoverDisplayPrefab;

    protected List<GameObject> enemies;
    protected GameObject gameoverDisplay;
	// Use this for initialization
	void Start () 
    {
        gameoverDisplay = GameObject.Instantiate(gameoverDisplayPrefab) as GameObject;
        gameoverDisplay.renderer.enabled = false;
        player.OnPlayerDied += PlayerDied;
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

        //hide the death card
        HideGameoverDisplay();
    }

    void PlayerDied(PlayerController player)
    {
        ShowGameoverDisplay();
        bombManager.Clear();
        enemySpawner.StopAllCoroutines();
        
    }

    void ShowGameoverDisplay()
    {
        gameoverDisplay.renderer.enabled = true;
    }

    void HideGameoverDisplay()
    {
        gameoverDisplay.renderer.enabled = false;

    }

}
