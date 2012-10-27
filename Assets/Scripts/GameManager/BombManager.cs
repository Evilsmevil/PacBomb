using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is the main orchestrator of bomb
/// choreography. It is responsible for co-ordinating
/// behaviour and delegating it to various sub-systems that 
/// will generate various actions such as destroy objects and
/// calculating scores.
/// </summary>
public class BombManager : MonoBehaviour {

    public OccupancyGridView occupancyGridView;
    public LineRenderer linkLine;
    protected HashSet<NewBomb> bombsInPlay;
    protected HashSet<GameObject> enemiesInPlay;

    public void AddBomb(NewBomb bomb)
    {
        bombsInPlay.Add(bomb);
        
        //should we check here to see if the bomb could be in range and be primed?
    }

    public void Reset()
    {
        foreach (NewBomb bomb in bombsInPlay)
        {
            bomb.Reset();
        }
        bombsInPlay.Clear();
        foreach (GameObject enemy in enemiesInPlay)
        {
            Destroy(enemy);
        }
        enemiesInPlay.Clear();

        occupancyGridView.Reset();
    }


    /// <summary>
    /// Having enemies in a cetralised location makes it 
    /// faster to 
    /// </summary>
    /// <param name="enemy"></param>
    public void AddEnemy(GameObject enemy)
    {
        enemiesInPlay.Add(enemy);
    }

	// Use this for initialization
	void Awake () 
    {
        bombsInPlay = new HashSet<NewBomb>();
        enemiesInPlay = new HashSet<GameObject>();
	}
	

    /// <summary>
    /// This code should deal with destroying a set of bombs and then 
    /// calculating the score and dealing with all the enemies
    /// </summary>
    /// <param name="bomb"></param>
    public void OnBombTriggered(NewBomb bomb)
    {
        //get a list of bombs to explode
        List<NewBomb> bombsToExplode = FindConnectedBombs(bomb);
        
        //find all the unique enemies in range of the bombs
        List<GameObject> enemies = new List<GameObject>(FindEnemies(bombsToExplode));

        DestroyBombLinks(bombsToExplode);

        //destroy bombs and enemies in a way that looks cool
        DestroyEnemies(enemies);

        //remove the bombs from the list of bombs in play
        DestroyBombs(bombsToExplode, bomb);
    }

    List<NewBomb> FindConnectedBombs(NewBomb bomb)
    {
        List<NewBomb> bombsFound = new List<NewBomb>();
        bomb.FindBombs(bombsFound);

        return bombsFound;
    }

    void DestroyBombLinks(List<NewBomb> bombList)
    {
        foreach (NewBomb bomb in bombList)
        {
            bomb.DestroyBombLinks();
        }
    }

    void DestroyBombs(List<NewBomb> bombs, NewBomb startBomb)
    {
        //remove bombs from the manager
        foreach (NewBomb bomb in bombs)
        {
            bombsInPlay.Remove(bomb);
            occupancyGridView.LayNewTrail();
        }

        //send all this information somwhere so that the score can be calculated
        StartCoroutine(startBomb.ExplodeBombs(bombs));
    }

    IEnumerable<GameObject> FindEnemies(List<NewBomb> bombs)
    {
        HashSet<GameObject> enemies = new HashSet<GameObject>();
        foreach (NewBomb bomb in bombs)
        {
            //find all the enemies near a bomb
            List<GameObject> enemiesInRange = bomb.FindEnemies();

            //add them to the set. any clashes should be ignored?
            foreach (GameObject enemy in enemiesInRange)
            {
                enemies.Add(enemy);
            }
        }

        return enemies;
    }

    void DestroyEnemies(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            //Give the player the number of enemies they destroyed as points
            //for each enemie - i.e enemies squared
            int enemyScore = ScoreKeeper.Instance.GetEnemyScore(enemies.Count, 100);
            ScoreKeeper.Instance.AddEnemyPoints(enemyScore);
            enemy.SendMessage("Kill", enemyScore);

            enemiesInPlay.Remove(enemy);
        }
    }

    public void OnPelletCollected(NewBomb owner)
    {
        //need to know which bomb it was connected to?
        float blastRadius = owner.GetBlastRadius();
        //check to see if any bombs are in range of the bomb that just got a pellet collected
        foreach (NewBomb bomb in bombsInPlay)
        {
            //check to see if bombs are in range
            float distanceBetweenBombs = Vector3.Distance(bomb.transform.position, owner.transform.position);
            if (distanceBetweenBombs < blastRadius/2 && bomb.readyToBlow && bomb != owner)
            {
                //it's in range so lets mark it as such
                bomb.MarkExplodable();
                //create line link
                CreateLink(owner, bomb);
            }
        }


        //if a bomb has just been activated then we need to check to see
        //if it falls under radius of any other bombs
        //check to see if any bombs are in range of the bomb that just got a pellet collected
        foreach (NewBomb bomb in bombsInPlay)
        {
            float blastRad = bomb.GetBlastRadius();
            //check to see if bombs are in range
            float distanceBetweenBombs = Vector3.Distance(bomb.transform.position, owner.transform.position);
            if (distanceBetweenBombs < blastRad / 2 && bomb != owner)
            {
                //it's in range so lets mark it as such
                owner.MarkExplodable();
                //create line link
                CreateLink(bomb, owner);
            }
        }
    }

    void CreateLink(NewBomb origin, NewBomb destination)
    {
        origin.AddBombLink(destination, linkLine);

        /*LineRenderer newRenderer = Instantiate(linkLine) as LineRenderer;
        newRenderer.SetVertexCount(2);
        newRenderer.SetPosition(0, origin.transform.position);
        newRenderer.SetPosition(1, destination.transform.position);
        newRenderer.SetColors(origin.renderer.material.color, destination.renderer.material.color);

        newRenderer.transform.parent = origin.transform;
        //we add the line to both origin and destination because we want to to blow up if either of the bombs
        //are destroyed
        origin.LinkIndicators.Add(newRenderer.gameObject);
        destination.LinkIndicators.Add(newRenderer.gameObject);*/
    }
}
