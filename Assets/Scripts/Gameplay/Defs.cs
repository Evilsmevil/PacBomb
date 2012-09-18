using UnityEngine;
public class PelletDefs
{
	public static Vector3 GameUp = new Vector3(0f, 0f, 1.0f);
	public static Vector3 GameDown = new Vector3(0f, 0, -1.0f);
	public static Vector3 GameLeft = new Vector3(-1.0f,0.0f,0f);
	public static Vector3 GameRight = new Vector3(1.0f,0f,0f);
	
	
}

public struct GridCoord
{
	public int x;
	public int y;
	
	public GridCoord(int x, int y)
	{
		this.x = x; this.y = y;	
	}
	
	public bool IsInvalid()
	{
		return x <= 0 && y <= 0;
	}
	
	public override string ToString()
	{
		return "(" + x + ", " + y + ")";	
	}
	
	public static bool operator ==(GridCoord a, GridCoord b)
	{
		return a.x == b.x && a.y == b.y;	
	}
	
	public static bool operator != (GridCoord a, GridCoord b)
	{
		return !(a == b);	
	}
	
	public static GridCoord InvalidPosition = new GridCoord(-1,-1);
}
