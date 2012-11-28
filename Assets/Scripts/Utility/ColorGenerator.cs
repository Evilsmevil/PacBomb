using System;
using UnityEngine;

/// <summary>
/// Generates colours for testing
/// </summary>
public class ColorGenerator : MonoBehaviour
{
    public Color generatedColor;

    public float h;
    public float s;
    public float v;

    float goldenRatio = 0.618033988749895f;

    public void GenerateColor()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        rand += goldenRatio;
        rand = rand %1;
        generatedColor = ColorUtilities.HSVtoRGB(rand,
           0.5f,
           0.95f);

        ColorUtilities.HSVtoRGB(0.5f, 0.5f, 0.5f);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GenerateColor();
        }
    }
}
