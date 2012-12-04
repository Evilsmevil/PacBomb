using System;
using UnityEngine;

public class ColorUtilities
{
    public static float brightnessBoost = 0.3f;
    
    //convert hsv colorspace coordinates to RGB
    //this function is converted from a function given at
    //http://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
    /*public static Color HSVToRGB(float h, float s, float v)
    {
        int h_i = (int) h * 6;
        float f = h*6 - h_i;
        float p = v * (1-s);
        float q = v * (1-f*s);
        float t = v * (1-(1-f) *s);
        
        float r = 0,g = 0,b = 0;
        switch(h_i)
        {
            case 0:
                r = v; g = t; b = p;
                break;
            case 1:
                r = q; g = v; b = p;
                break;
            case 2:
                r = p; g= v; b = t;
                break;
            case 3:
                r = p; g= q; b = v;
                break;
            case 4:
                r = t; g = p; b = v;
                break;
            case 5:
                r = v; g = p; b = q;
                break;
        }

        Debug.Log("Hsv input is H:" + h +" S:" + s + " V:" + v );
        Debug.Log("RGB output is R:" + r + " G:" + g + " B:" + b);

        return new Color(r, g, b);

    }*/

    public static Color HSVToRGBTom(float h, float s, float v)
    {
        //convert H for 0..1 to 0..360
        float hDeg = h * 360;

        //find the chroma
        float chroma = v * s;

        //find h dash which is h /60 degrees?
        float hDash = h / 60;

        //find a point in the rgb cube with the same 
        //values as the hsv, using intermediate point x
        float x = chroma * 1 - Math.Abs(hDash % 1);

        float r = 0, g = 0, b = 0;

        if (h >= 0 && h < 1)
        {
            r = chroma; g = x; b = 0;
        }
        if (h >= 1 && h < 2)
        {
            r = x; g = chroma; b = 0;
        }
        if (h >= 2 && h < 3)
        {
            r = 0; g = chroma; b = x;
        }
        if (h >= 3 && h < 4)
        {
            r = 0; g = x; b = chroma;
        }
        if (h >= 4 && h < 5)
        {
            r = x; g = 0; b= chroma;
        }
        if (h >= 5 && h < 6)
        {
            r = chroma; g = 0; b = x;
        }

        float m = v - chroma;

        Color finalColor = new Color(r + m, g + m, b + m);

        Debug.Log("Hsv input is H:" + h + " S:" + s + " V:" + v);
        Debug.Log("RGB output is " + finalColor);

        return finalColor;
    }

    public static Color HSVtoRGB(float h, float s, float v)
    {
        Debug.Log("Hsv input is H:" + h + " S:" + s + " V:" + v);
        float r,g, b;
        int i;
        float f, p, q, t;
        if (s == 0)
        {
            // achromatic (grey)
            r = g = b = v;
            return new Color(r,g,b);
        }
        h /= 60;			// sector 0 to 5
        i = (int) Mathf.Floor(h);
        f = h - i;			// factorial part of h
        p = v * (1 - s);
        q = v * (1 - s * f);
        t = v * (1 - s * (1 - f));
        switch (i)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            default:		// case 5:
                r = v;
                g = p;
                b = q;
                break;
        }

        
        Debug.Log("RGB output is R:" + r + " G:" + g + " B:" + b);
        return new Color(r, g, b);
    }

    //return one of the base colors; red green or blue
    static int lastColor = 0;
    public static Color GetRandomRGB()
    {
        int randomColour = UnityEngine.Random.Range(0, 3);

        //we want some variation so don't generate the same colour twice in a row
        if (randomColour == lastColor)
        {
            randomColour = (randomColour + 1) % 3;
        }

        lastColor = randomColour;
        Color newColor;
        switch (randomColour)
        {
            case 0:
                newColor = Color.red;
                break;
            case 1:
                newColor = Color.green;
                break;
            case 2:
                newColor = Color.blue;
                break;
            default:
                newColor = Color.white;
                break;
        }
        Debug.Log("Generated "+ randomColour + " which is "  + newColor);
        return newColor;

    }
}
