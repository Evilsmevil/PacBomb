using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Plays a sound from a random array of sounds
/// </summary>
[AddComponentMenu("Sound/Sequential Sound Array")]
public class SequentialSoundArray : BaseSoundClip
{
    public List<AudioClip> soundClips;

    bool loopSoundList = false;
    int lastPlayedIndex = 0;

    //hook up the audio source
    protected override void Start()
    {
        base.Start();
        lastPlayedIndex = 0;
    }

    public override void PlayNextClip()
    {
        //get the sound from the current index
        currentSoundClip = soundClips[lastPlayedIndex];

        //always increment 
        lastPlayedIndex++;

        //make sure we don't go out of bounds
        if (lastPlayedIndex >= soundClips.Count)
        {
            //we're out of bounds so decide whether to reset or not
            if (loopSoundList)
            {
                lastPlayedIndex = 0;
            }
            else
            {
                lastPlayedIndex = soundClips.Count - 1;
            }
        }        

        base.PlayNextClip();


    }

}
