using UnityEngine;

[AddComponentMenu("Non-gameplay/Title Screen Manager")]
//manages the title screen
//right now this is simeple, pressing enter will load the game
public class TitleScreenManager : MonoBehaviour
{
    public int levelToLoad = 1;

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Application.LoadLevel(levelToLoad);
        }
    }
}
