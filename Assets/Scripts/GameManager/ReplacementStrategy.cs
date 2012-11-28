using System;
using UnityEngine;

[AddComponentMenu("Strategies/Replacement strategy")]
//this will immediately replace a bomb that gets destroyed
public class ReplacementStrategy : LayoutStrategy
{
	public int numPelletsPerTrail = 7;
    public int variance = 3;

	public override void OnBombDestroyed (NewBomb destroyedBomb)
	{
		base.OnBombDestroyed(destroyedBomb);

        int numPellets = numPelletsPerTrail + UnityEngine.Random.Range(-variance, variance);
        occupancyGrid.LayNewTrail(numPellets);
	}
}


