using UnityEngine;
using System.Collections.Generic;

public class BaseSoundClip : MonoBehaviour
{
    public AudioClip currentSoundClip;
    public float defaultVolume = 1.0f;
    protected List<AudioSource> sources;
    //play to camera will mean that the sound will work even if the script is destroyed
    public bool playToCamera = false;

    protected virtual void Start()
    {
        sources = new List<AudioSource>();
    }

    public virtual void PlayNextClip()
    {
        if(playToCamera)
        {
            Camera.main.audio.PlayOneShot(currentSoundClip);
        }
        foreach (AudioSource src in sources)
        {
            if (src.isPlaying == false)
            {
                src.clip = currentSoundClip;
                src.Play();
                return;
            }
        }

        //if we get here then no sources are available
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.volume = defaultVolume;
        sources.Add(newSource);

        newSource.clip = currentSoundClip;
        newSource.Play();
    }
}
