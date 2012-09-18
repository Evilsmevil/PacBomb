using UnityEngine;

/// <summary>
/// Place for some helpful functions
/// </summary>
public class Utils
{
	public static Vector3 GetRandomDirection()
	{
		//generate a random direction
		int direction = Random.Range(0,3);
		
		switch(direction)
		{
		case 0:
			//up
			return PelletDefs.GameUp;
		case 1:
			//right
			return PelletDefs.GameRight;
		case 2:
			//down
			return PelletDefs.GameDown;
		case 3:
			//left
			return PelletDefs.GameLeft;
		default:
			//shouldn't get here
			return Vector3.zero;
		}
	}
}

