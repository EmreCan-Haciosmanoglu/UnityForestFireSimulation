using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Texture2D texture;

    public GameObject[] trees;
    [Range(0f, 1f)]
    public float percentage = 0.01f;
    public bool autoUpdate = true;
    public float heightMultiplier = 1.0f;
    public GameObject TreesParent;
    public GameObject emptyGameObjectPrefab;
    public GameObject burntIconPrefab;
    public GameObject firePrefab;
    public float MaxDistanceToSpreadFire = 10f;

    public List<ForestTree> treesOnTheScene = new List<ForestTree>();
    public static List<ForestTree> treesOnFire = new List<ForestTree>();
    public void SetRandomTreeOnFire()
    {
        int randomIndex = Random.Range(0, treesOnTheScene.Count);
        Debug.Log("Count : " + treesOnTheScene.Count + "\nIndex: " + randomIndex);
        treesOnTheScene[randomIndex].SetOnFire();
    }

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
        treesOnTheScene.Clear();
        treesOnFire.Clear();
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

            GameObject parent = Instantiate(emptyGameObjectPrefab, temp.pos, Quaternion.identity, TreesParent.transform);

            GameObject tree = Instantiate(trees[temp.type].gameObject, parent.transform);
            GameObject burntIcon = Instantiate(burntIconPrefab.gameObject, parent.transform);
            GameObject fire = Instantiate(firePrefab.gameObject, parent.transform);

            ForestTree forestTree = new ForestTree(tree, burntIcon, fire);
            treesOnTheScene.Add(forestTree);
        }
        foreach (ForestTree forestTree in treesOnTheScene)
        {
            foreach (ForestTree otherForestTree in treesOnTheScene)
            {
                if (forestTree == otherForestTree)
                    continue;
                float dist = Vector3.Distance(otherForestTree.tree.transform.position, forestTree.tree.gameObject.transform.position);
                if (dist < MaxDistanceToSpreadFire)
                    forestTree.closestTrees.Add(otherForestTree);
            }
        }
    }

    public void Update()
    {
        foreach(ForestTree forestTree in treesOnFire)
        {
            forestTree.Update();
        }
    }
}
