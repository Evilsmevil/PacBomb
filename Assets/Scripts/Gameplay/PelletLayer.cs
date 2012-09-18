using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Pellet layer. this is a much better way of laying pellets using
/// a simple occupancy grid. the basic strategy is to try and lay a bunch of pellets
/// the algorithm will try and pick a random direction (weighted to the forward direction
/// so that we get straighter chains than pure random) if can lay a pellet it does otherwise it
/// looks for an adjacent tile and tries that one. if it can't go then it just lays the terminating bomb
/// </summary>
[AddComponentMenu("Level Generation/Pellet Layer")]
public class PelletLayer : MonoBehaviour
{
	public Pellet pelletObject;
	public NewBomb bombObject;
	
	public void LayPelletTrail(int chainLength, OccupancyGrid grid, Color color)
	{
		Debug.Log("trying to lay " + chainLength + " pellets ");
		List<Pellet> pelletList = new List<Pellet>(chainLength);	
		
		//Find random space
		GridCoord startLocation = GetRandomLocation(grid);
		
		//is the space a dead end?
		if(grid.IsSurrounded(startLocation))
		{
			//yes? make into a bomb and end
			AddBomb(grid, startLocation, pelletList, color);
			Debug.Log("ran into a dead end at " + startLocation);
			return;
		}
		else //no? make it into a pellet
		{
			AddPellet(grid, startLocation, pelletList, color);
		}
		
		//total chain length is pellets + bomb
		int pelletsToLay = chainLength - 1;
		
		//set the start location as the base location
		GridCoord lastLocation = startLocation;
		while(pelletList.Count < pelletsToLay) //While(not enough pellets made)
		{
			//Find random adjacent space
			GridCoord newLocation = grid.FindRandomEmptyAdjacentSpace(lastLocation);
			
			//Is the space a dead end?
			if(grid.IsSurrounded(newLocation))
			{
				//yes? make into a bomb. end
				AddBomb(grid, newLocation, pelletList, color);
				Debug.Log("ran into a dead end at " + newLocation);
				return;
			}
			else
			{
				//no make a pellet on the space
				AddPellet(grid, newLocation, pelletList, color);
			}
			
			lastLocation = newLocation;
		}
		
		//if we made it this far we've not made a bomb
		//Find random adjacent space
		GridCoord bombLocation = grid.FindRandomEmptyAdjacentSpace(lastLocation);
		//put bomb in it
		AddBomb(grid, bombLocation, pelletList, color);
	}
	
	
	
	/// <summary>
	/// will pick an unfilled random location
	/// </summary>
	/// <returns>
	/// The random location.
	/// </returns>
	/// <param name='grid'>
	/// the occupancy grid you want to get it from
	/// </param>
	protected GridCoord GetRandomLocation(OccupancyGrid grid)
	{
		//right now this is an enxpensive call
		//at some point I'll do dirty grids so we 
		//can cache operations like this
		if(grid.IsFull())
		{
			//we're full so don't even bother 
			//return nonsense
			Debug.Log("Grid is full!");
			return new GridCoord(-1,-1);
		}
		
		//we're not full so try and pick something
		var spotTaken = true;
		int x = 0;
		int y = 0;
		while(spotTaken)
		{
			x = UnityEngine.Random.Range(0,grid.GetWidth());
			y = UnityEngine.Random.Range(0, grid.GetHeight());
			spotTaken = grid.IsOccupied(x,y);
		}
		
		return new GridCoord(x,y);
		
	}
	
	protected void AddPellet(OccupancyGrid grid, GridCoord location, List<Pellet> pellets, Color color)
	{
		Pellet newPellet = Instantiate(pelletObject) as Pellet;
		newPellet.name = "pellet " + pellets.Count;
		newPellet.renderer.material.color = color;
		grid.AddObject(location, newPellet.gameObject);
		pellets.Add(newPellet);
	}
	
	protected void AddBomb(OccupancyGrid grid, GridCoord location, 
						   List<Pellet> pellets, Color color)
	{
		NewBomb newBomb = Instantiate(bombObject) as NewBomb;
		grid.AddObject(location, newBomb.gameObject);
		newBomb.renderer.material.color = color;
		newBomb.AddPellets(pellets);
	}
}


