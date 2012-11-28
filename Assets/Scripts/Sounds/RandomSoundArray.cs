using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Plays a sound from a random array of sounds
/// </summary>
[AddComponentMenu("Sound/Random Sound Array")]
public class RandomSoundArray : BaseSoundClip
{
    public List<AudioClip> soundClips; 


    public AudioClip GetRandomClip()
    {
        return soundClips[UnityEngine.Random.Range(0, soundClips.Count)];
    }

    public override void PlayNextClip()
    {
        //set the clip
        currentSoundClip = GetRandomClip();

        base.PlayNextClip();
    }

}
