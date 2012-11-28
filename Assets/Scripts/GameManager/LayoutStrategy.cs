using UnityEngine;
using System.Collections.Generic;

///Layout strategies are ways that dictate how
//to lay bombs 
public abstract class LayoutStrategy : MonoBehaviour
{
	public HashSet<NewBomb> bombsInPlay;
	public OccupancyGridView occupancyGrid;
    public StrategyView strategyView;

	public virtual void OnBombDestroyed(NewBomb destroyedBomb)
	{
		bombsInPlay.Remove(destroyedBomb);
	}
	
	protected virtual void Awake()
	{
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
    public virtual void CleanupStrategy()
    {
        //base does nothing
    }

    public virtual void SetCurrentBombSet(HashSet<NewBomb> currentBombs)
    {
        bombsInPlay = currentBombs;

        if (bombsInPlay.Count == 0)
        {
            CreateInitialLayout();
        }
    }

    public void Init(LayoutStrategy lastStrat, HashSet<NewBomb> newBombs)
    {
        if (strategyView)
        {
            strategyView.PlayStrategyIntro(lastStrat);
        }
        SetCurrentBombSet(newBombs);

    }
}
