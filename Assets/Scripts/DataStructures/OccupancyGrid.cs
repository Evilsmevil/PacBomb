using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Occupancy grid keeps track of if cells are occupied or not
/// </summary>
public class OccupancyGrid
{
	//using a grid of fixed size to begin
	protected GridSpace[,] grid;
	
	public Action<GridCoord, GameObject> ObjectAdded;
	
	
	//internal shit to speed up some operations like determining if the grid is full
	protected bool isDirty;
	protected bool isFull;
	
	public OccupancyGrid(int height, int width)
	{
		grid = new GridSpace[width,height];	
		
		isDirty = true;
		InitialiseGrid();	
	}
	
	protected void InitialiseGrid()
	{
		for(int i = 0; i < GetHeight(); ++i)
		{
			for(int j = 0; j < GetWidth(); ++j)
			{
				grid[i,j] = new GridSpace();		
			}
		}	
	}
	public int GetHeight()
	{
		return grid.GetLength(0);	
	}
	
	/// <summary>
	/// Gets the width. - it's a multi dim array so all are the same height
	/// </summary>
	/// <returns>
	/// The width.
	/// </returns>
	public int GetWidth()
	{
		return grid.GetLength(1);	
	}
	
	public bool IsOccupied(GridCoord location)
	{
		//sanity
		if(OutOfBounds(location))
			return true;
		
		return grid[location.x,location.y].IsOccupied();	
	}
	
	public bool IsOccupied(int x, int y)
	{
		//sanity
		GridCoord location = new GridCoord(x,y);
		return IsOccupied(location);
	}
	
	public bool IsSurrounded(GridCoord location)
	{
		//sanity - out of bounds is 'surrounded'
		if(OutOfBounds(location))
			return true;
		
		return IsOccupied(location.x + 1, location.y ) &&
			   IsOccupied(location.x - 1, location.y) &&
			   IsOccupied(location.x, location.y + 1) &&
			   IsOccupied(location.x, location.y - 1);
			
	}
	
	public bool OutOfBounds(GridCoord location)
	{
		return 	location.x >= GetWidth() || location.y >= GetHeight()
			|| location.x < 0 || location.y < 0;
	}
	
	/// <summary>
	/// Determines whether this instance is full.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is full; otherwise, <c>false</c>.
	/// </returns>
	public bool IsFull()
	{
		//if we've calculated this before and not changed anything then don't recalc
		if(isDirty == false) return isFull;
		
		bool full = true;
		for(int i = 0; i < GetHeight(); ++i)
		{
			for(int j = 0; j < GetWidth(); ++j)
			{
				full = full && grid[i,j].IsOccupied();		
			}
		}
		
		//we've recalculated so we're clean
		isFull = full;
		isDirty = false;
		
		return isFull;
	}
	
	/// <summary>
	/// Adds the object. returns true if the object was added
	/// </summary>
	/// <returns>
	/// The object.
	/// </returns>
	/// <param name='x'>
	/// If set to <c>true</c> x.
	/// </param>
	/// <param name='y'>
	/// If set to <c>true</c> y.
	/// </param>
	/// <param name='newObject'>
	/// If set to <c>true</c> new object.
	/// </param>
	public bool AddObject(GridCoord location, GameObject newObject)
	{
		//sanity check
		if(OutOfBounds(location) || newObject == null)
			return false;
		
		if(grid[location.x,location.y].IsOccupied())
		{
			return false;
		}
		
		grid[location.x,location.y].occupier = newObject;
		
		
		//grid is dirty
		isDirty = true;
		
		//tell any observers that we've updated the view
		ObjectAdded(location, newObject);
		
		return true;
	}
	
	public GridSpace GetGridSpace(GridCoord position)
	{
		return grid[position.x, position.y];	
	}
	
	public GridCoord FindRandomEmptyAdjacentSpace(GridCoord startSpace)
	{
		//we will never have more than 4 spaces so might as well not
		//use too much memory?
		List<GridCoord> eligibleSpaces = new List<GridCoord>(4);	
		
		//get all the spaces we could possible have
		
		//north space
		GridCoord space = new GridCoord(startSpace.x, startSpace.y + 1);
		if(!OutOfBounds(space) && !IsOccupied(space))
		{
			eligibleSpaces.Add(space);	
		}
		
		//east space
		space = new GridCoord(startSpace.x + 1, startSpace.y);
		if(!OutOfBounds(space) && !IsOccupied(space))
		{
			eligibleSpaces.Add(space);	
		}
		
		//south space
		space = new GridCoord(startSpace.x, startSpace.y - 1);
		if(!OutOfBounds(space) && !IsOccupied(space))
		{
			eligibleSpaces.Add(space);	
		}
		
		//west space
		space = new GridCoord(startSpace.x - 1, startSpace.y);
		if(!OutOfBounds(space) && !IsOccupied(space))
		{
			eligibleSpaces.Add(space);	
		}
		
		//pick a random space
		int pickedSpace = UnityEngine.Random.Range(0, eligibleSpaces.Count - 1);
		
		return eligibleSpaces[pickedSpace];
		
	}
	
}

public class GridSpace
{
	public GameObject occupier;
	
	public GridSpace()
	{
		occupier = null;	
	}
	
	public GridSpace(GameObject newObject)
	{
		this.occupier = newObject;	
	}
	public bool IsOccupied()
	{
		return occupier != null;	
	}
	
}


