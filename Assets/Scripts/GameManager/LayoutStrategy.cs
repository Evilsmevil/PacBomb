using UnityEngine;
using System.Collections.Generic;

///Layout strategies are ways that dictate how
//to lay bombs 
public abstract class LayoutStrategy : MonoBehaviour
{
	public HashSet<NewBomb> bombsInPlay;
	public OccupancyGridView occupancyGrid;
	
	public virtual void OnBombDestroyed(NewBomb destroyedBomb)
	{
		bombsInPlay.Remove(destroyedBomb);
	}
	
	protected virtual void Awake()
	{
		bombsInPlay = new HashSet<NewBomb>();
	}
	
	public virtual void AddBomb(NewBomb bomb)
	{
		bombsInPlay.Add(bomb);
	}
	
	public virtual void Reset()
	{
		foreach (NewBomb bomb in bombsInPlay)
        {
            bomb.Reset();
        }
		
        bombsInPlay.Clear();
		
		CreateInitialLayout();
	}
	
	//create the initial layout for the specific strategy
	public virtual void CreateInitialLayout()
	{
		for(int i = 0; i < 3; ++i)
		{
			//this actually eventually calls the AddBomb function so we're ok
			occupancyGrid.LayNewTrail(8);
		}
	}

    /// <summary>
    /// This is used to cleanup any objects we might be doing stuff with when
    /// we create a new strategy
    /// </summary>
    public virtual void CleanupStrategy(LayoutStrategy newStrategy)
    {
        //transfer all the bombs to the bombs in play dataset over to the new strategy
        foreach (NewBomb bomb in bombsInPlay)
        {
            newStrategy.bombsInPlay.Add(bomb);
        }

        //clear the bomb set and reset any other variables
        bombsInPlay.Clear();
    }
}
