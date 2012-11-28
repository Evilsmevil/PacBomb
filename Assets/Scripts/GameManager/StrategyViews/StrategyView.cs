using UnityEngine;

/// <summary>
/// Controls view relating to the current strategy we are using
/// </summary>
[AddComponentMenu("Strategies/Basic Strategy View")]
public class StrategyView : MonoBehaviour
{
    public Color BGColor = Color.green;
    public string strategyInfoText = "Basic Text";
    protected bool showLabel = false;
    public void PlayStrategyIntro(LayoutStrategy lastStrategy)
    {
        if (lastStrategy)
        {
            lastStrategy.strategyView.PlayStrategyOutro();
        }

        showLabel = true;
        Camera.mainCamera.backgroundColor = BGColor;
    }

    public void PlayStrategyOutro()
    {
        //base class has no implementation
        showLabel = false;
    }

    /// <summary>
    /// Tell the player the current strategy
    /// </summary>
    protected void OnGUI()
    {
        if (showLabel)
        {
            GUI.Label(new Rect(300, 10, 500, 30), strategyInfoText);
        }
    }
    
}
