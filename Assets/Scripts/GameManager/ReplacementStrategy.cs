using System;
using UnityEngine;

[AddComponentMenu("Level Generation/Replacement strategy")]
//this will immediately replace a bomb that gets destroyed
public class ReplacementStrategy : LayoutStrategy
{
	public int numPelletsPerTrail = 7;
	
	public override void OnBombDestroyed (NewBomb destroyedBomb)
	{
		base.OnBombDestroyed(destroyedBomb);
		occupancyGrid.LayNewTrail();
	}
}


