using UnityEngine;

[AddComponentMenu("Strategies/Fill When Empty Strat")]
//this will wait until all the bombs are blown up
//before creating a bunch more
public class RefillWhenEmptyStrategy : LayoutStrategy
{
	public int numberOfBombsToCreate = 4;
	public int pelletsPerTrail = 5;
	public override void OnBombDestroyed (NewBomb destroyedBomb)
	{
		base.OnBombDestroyed(destroyedBomb);
		//do we have no bombs left?
		if(bombsInPlay.Count == 0)
		{
			//create all the bombs at once
			for(int i = 0; i < numberOfBombsToCreate; ++i)
			{
					occupancyGrid.LayNewTrail(pelletsPerTrail);
			}
		}
	}
}
