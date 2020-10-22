using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ForestGenerator
{
    public static List<Temp> GenerateTreeLocations(float percentage, float[,] heightMap, int count)
    {
        List<Temp> result = new List<Temp>();

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Random.Range(0.0f, 1.0f) > percentage)
                    continue;
                result.Add(new Temp(new Vector3(x, heightMap[x, y], y) , Random.Range(0, count)));
            }
        }
        return result;
    }
}
