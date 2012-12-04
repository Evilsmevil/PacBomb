using UnityEngine;

[System.Serializable]
public class PlayerBoost
{
    //speed boost vars
    public float boostCooldownTime = 1.5f;

    //boost duration in seconds
    public float boostDuration = 1.0f;
    //how much you speed up by
    public float boostSpeedIncreaseMult = 1.5f;
    //lkast time we boosted
    protected float lastBoostTime;

    //tracking var so we know how much boost to apply at any time
    public float currentBoostMult = 1.0f;

    public void Init()
    {
        lastBoostTime = Time.time + 0.02f;
    }

    public void SetBoosting()
    {
        lastBoostTime = Time.time;
    }

    public void UpdateBoost()
    {
        //normalised boost time left is
        float timeIntoBoost = Time.time - lastBoostTime;
        float timeIntoBoostNormalised = timeIntoBoost / boostDuration;

        currentBoostMult = Mathf.Lerp(boostSpeedIncreaseMult, 1.0f, timeIntoBoostNormalised);

    }

    public float GetCurrentBoostMult()
    {
        return currentBoostMult;
    }

    public bool SpeedBoostReady()
    {
        float currentTime = Time.time;
        float nextBoostTime = lastBoostTime + boostCooldownTime;
        if (currentTime >= nextBoostTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

