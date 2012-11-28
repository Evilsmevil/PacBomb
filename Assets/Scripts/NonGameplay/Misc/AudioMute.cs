using UnityEngine;

public class AudioMute : MonoBehaviour
{
    public KeyCode muteKey;
    static string MUTE_SETTING = "MuteSetting";

    /// <summary>
    /// Check player prefs to see if we should mute audio on start
    /// </summary>
    void Start()
    {
        int muteSetting = (PlayerPrefs.GetInt(MUTE_SETTING, 0));

        if(muteSetting == 1)
        {
            AudioListener.volume = 0.0f;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(muteKey))
        {
            if (AudioListener.volume <= 0.01f)
            {
                PlayerPrefs.SetInt(MUTE_SETTING, 0);
                AudioListener.volume = 1.0f;
            }
            else
            {
                PlayerPrefs.SetInt(MUTE_SETTING, 1);
                AudioListener.volume = 0.0f;
            }
        }
    }
}
