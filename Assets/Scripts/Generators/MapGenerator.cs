using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Texture2D texture;

    public GameObject[] trees;
    [Range(0f, 1f)]
    public float percentage;
    public bool autoUpdate;
    public float heightMultiplier = 1.0f;
    public GameObject TreesParent;

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
                results[x, y] = colors[i].r * heightMultiplier;
            }
        }
        return results;
    }

    public void GenerateMap()
    {
        foreach (Transform child in TreesParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        float[,] heightMap = DecodeFloatTexture();
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap), texture);

        List<Temp> result = ForestGenerator.GenerateTreeLocations(percentage, heightMap, trees.Length);

        for (int i = 0; i < result.Count; i++)
        {
            Temp temp = result[i];
            Instantiate(trees[temp.type].gameObject, temp.pos, Quaternion.identity, TreesParent.transform);
        }
    }
}
