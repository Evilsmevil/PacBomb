using UnityEngine;

/// <summary>
/// Status effects applied when a player enters a certain coloured zone
/// the base effect provides base on which to build these
/// </summary>
public abstract class BaseStatusEffect : MonoBehaviour
{
    public Color effectColour = Color.white;

    public virtual void ApplyStatusEffect(PlayerController player)
    {

    }

    public virtual void RemoveStatusEffect(PlayerController player)
    {

    }

    
}