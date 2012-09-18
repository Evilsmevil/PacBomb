using System;
using UnityEngine;
/// <summary>
/// This visualises an occupancy grid
/// </summary>
public class OccupancyGridView : MonoBehaviour
{
	public int gridHeight = 10;
	public int gridWidth  = 10;
	
	//defines the grid spacing
	public float gridX = 1.0f;
	public float gridY = 1.0f;
	
	public int numberOfPelletsPerTrail = 7;
	
	public PelletLayer pelletLayer;
	
	//the grid we are viewing
	OccupancyGrid grid;
	
	void Start()
	{
		//create the grid	
		grid = new OccupancyGrid(gridHeight, gridWidth);
		grid.ObjectAdded += OnObjectAdded;
		
		
		if(pelletLayer != null)
		{
			for(int i = 0; i < 3; ++i)
			{
				Vector3 colorVec = UnityEngine.Random.insideUnitSphere;
				Color newColor = new Color(colorVec.x, colorVec.y, colorVec.z);
				pelletLayer.LayPelletTrail(numberOfPelletsPerTrail, grid, newColor);	
			}
		}
		
	}
	
	void OnObjectAdded(GridCoord location, GameObject go)
	{
		//move the object to somewhere useful in world space and do any parenting etc
		go.transform.parent = this.transform;
		go.transform.localPosition = new Vector3(location.x * gridX, 0, location.y * gridY);
	}
	
	//Draw the occupancy grid so we can see the extents
	void OnDrawGizmos()
	{
		int viewHeight = gridHeight;
		int viewWidth = gridWidth;
		Color cellColor = Color.white;
		if(grid != null)
		{
			viewHeight = grid.GetHeight();
			viewWidth = grid.GetWidth();
		}
		for(int i = 0; i < viewHeight; ++i)
		{
			for(int j = 0; j < viewWidth; ++j)
			{
				Vector3 position = this.transform.position + new Vector3(i * gridX, 0f, j * gridY);
				
				//colour the grid if it's active
				if(grid != null) cellColor = grid.IsOccupied(i,j) ? Color.red : Color.white;
				
				Gizmos.color = cellColor;
				Gizmos.DrawWireCube(position, new Vector3(gridX, 1f, gridY)); 
			}
		}
	}
}

