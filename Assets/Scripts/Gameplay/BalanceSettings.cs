using UnityEngine;
using System.Collections;

/// <summary>
/// This contains all of the balance settings in the game
/// any game vaiables that affect balance should be placed in this class
/// and then linked into an other game objects as required.
/// </summary>
public class BalanceSettings : MonoBehaviour 
{
	public float basePlayerSpeed;
	
	public float moveSpeedIncreasePCT;
	public float bulletTimeDuration;
	public float threatDecreasePCT;
	public float explosionIncreasePCT;
	
	public int minTrailsPerGroup;
	public int maxTrailsPerGroup;
}

