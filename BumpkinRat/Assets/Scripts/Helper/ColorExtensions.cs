using UnityEngine;
using System.Collections.Generic;

public static class ColorX 
{
    public static Color FullAlpha(Color col)
    {
        if(col.a >= 1)
        {
            return col;
        }

        return new Color(col.r, col.g, col.b, 1);
    }

    public static bool Approximately(Color a, Color b, float threshold = 0.1f)
    {
        float diff = Vector4.Distance(a, b);

        if(diff == 0)
        {
            return true;
        }

        bool neg = diff < 0;

        return neg ? diff > threshold : diff < threshold;
    }

    public static List<string> GetColorsHexesFromStrip(Texture2D s, int colorCount)
    {
        List<string> colors = new List<string>();
        Color col = Color.clear;
        int width = s.width;
        Color[] pixelRow = s.GetPixels(0, 0, width, 1, 0);

        int counter = width / colorCount;

        for(int i = 0; i < width; i+= counter)
        {
            string colToString = ColorUtility.ToHtmlStringRGB(pixelRow[i]);
            if (!colors.Contains(colToString))
            {
                colors.Add(colToString);

            }
        }

        return colors;
    }

    public static Color ToColor(this string hex)
    {
        Color col = Color.white;
        bool valid = ColorUtility.TryParseHtmlString($"#{hex}", out col);
        return col;
    }

    public static Color Orange => new Color(1, 0.655f, 0, 1);

    public static Color Auburn => new Color(0.60f, 0.14f, 0.14f, 1);
    public static Color DeepPink => new Color(1.00f, 0.08f, 0.58f);
}
