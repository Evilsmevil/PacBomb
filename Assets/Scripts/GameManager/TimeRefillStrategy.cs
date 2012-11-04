using UnityEngine;
using System.Collections;
[AddComponentMenu("Level Generation/Time Refill Strategy")]
//this will wait until all the bombs are blown up
//before creating a bunch more
public class TimeRefillStrategy : LayoutStrategy
{
    //the time between bomb updates in seconds
	public float bombTimeInterval = 2;
	public int pelletsPerTrail = 5;
    public int pelletVariance = 0;
	public override void OnBombDestroyed (NewBomb destroyedBomb)
	{
		base.OnBombDestroyed(destroyedBomb);
		//this strategy is time based so we don't actually want to do anything when a bomb gets destroyed
	}

    public IEnumerator CreateNewTrail()
    {
        while (true)
        {
            //create a new trail based on the parameters
            //work out how many pellets there should be based on base number and variance
            int numPellets = pelletsPerTrail + UnityEngine.Random.Range(pelletVariance * -1, pelletVariance);
            occupancyGrid.LayNewTrail(numPellets);

            yield return new WaitForSeconds(bombTimeInterval);
        }
    }

    /// <summary>
    /// Hook into this to start our creation co-routine
    /// </summary>
    public override void CreateInitialLayout()
    {
        StartCoroutine(CreateNewTrail());
    }

    public override void CleanupStrategy(LayoutStrategy newStrategy)
    {
        base.CleanupStrategy(newStrategy);

        //stop our coroutine
        StopAllCoroutines();
    }
}
