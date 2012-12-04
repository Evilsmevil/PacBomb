using UnityEngine;

public static class ColorExtensions 
{
    /// <summary>
    /// This will return the colour with every channel added by a specific amount
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
   public static Color BumpedColour(this Color color, float bumpAmount)
   {
       Color bumpedCol = new Color(color.r + bumpAmount,
                                    color.g + bumpAmount,
                                    color.b + bumpAmount);

       return bumpedCol;
   }
}
