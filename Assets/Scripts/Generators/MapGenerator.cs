using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Texture2D texture;

    public GameObject[] trees;

    public bool autoUpdate;
    public float heightMultiplier = 1.0f;

    float[,] DecodeFloatTexture()
    {
        Color[] colors = texture.GetPixels();
        // HERE YOU CAN GET ALL 4 FLOATS OUT OR JUST THOSE YOU NEED.
        // IN MY CASE ALL 4 VALUES HAVE A MEANING SO I'M GETTING THEM ALL.
        float[,] results = new float[texture.width, texture.height];
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                int i = x + y * texture.width;
                results[x, y] = colors[i].r;
            }
        }
        return results;
    }

    public void GenerateMap()
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();

        float[,] heightMap = DecodeFloatTexture();

        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, heightMultiplier), texture);
    }
}
