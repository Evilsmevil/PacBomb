using UnityEngine;

[AddComponentMenu("Status Effects/Speed boost")]
public class SpeedUpEffect : BaseStatusEffect
{
    public float speedIncreaseFactor = 1.5f;

    protected PlayerController lastPlayer;
    /// <summary>
    /// set the players base speed to increase - this is applied per frame and should be
    /// considered an idompent operation
    /// </summary>
    /// <param name="player"></param>
    public override void ApplyStatusEffect(PlayerController player)
    {
        base.ApplyStatusEffect(player);
        lastPlayer = player;
        //increase the players base speed
        player.SetBaseSpeedMultiplier(speedIncreaseFactor);
    }

    public override void RemoveStatusEffect(PlayerController player)
    {
        player.SetBaseSpeedMultiplier(1.0f);
    }
}
