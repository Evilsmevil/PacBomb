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

    public void GenerateColor()
    {
        generatedColor = ColorUtilities.HSVToRGBTom(UnityEngine.Random.Range(0f,1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GenerateColor();
        }
    }
}
