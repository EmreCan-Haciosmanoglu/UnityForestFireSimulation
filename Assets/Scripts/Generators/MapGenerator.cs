using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Texture2D terrainMap;
    public Texture2D roadMap;

    public GameObject[] trees;
    [Range(1f, 30f)]
    public float BurnTime = 15f;
    [Range(.5f, 5f)]
    public float SpreadSpeed = 2f;
    [Range(1, 10)]
    public int passes = 3;
    public bool level = true;
    [Range(0f, 1f)]
    public float percentage = 0.1f;
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
        int randomIndex = UnityEngine.Random.Range(0, treesOnTheScene.Count);
        Debug.Log("Count : " + treesOnTheScene.Count + "\nIndex: " + randomIndex);
        treesOnTheScene[randomIndex].SetOnFire();
    }

    float[,] DecodeFloatTexture(Texture2D texture)
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

        float[,] heightMap = DecodeFloatTexture(terrainMap);
        float[,] roadMask = DecodeFloatTexture(roadMap);
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap, roadMask, passes, level), terrainMap);

        List<Temp> result = ForestGenerator.GenerateTreeLocations(percentage, heightMap, trees.Length);
        GameObject[] temps = new GameObject[trees.Length];
        for (int i = 0; i < trees.Length; i++)
        {
            temps[i] = Instantiate(trees[i], Vector3.zero, Quaternion.identity, TreesParent.transform);
        }

        for (int i = 0; i < result.Count; i++)
        {
            Temp temp = result[i];
            if (roadMask[(int)temp.pos.x, (int)temp.pos.z] > .5f)
                continue;
            GameObject parent = Instantiate(emptyGameObjectPrefab, temp.pos, Quaternion.identity, TreesParent.transform);
            

            bool next = false;
            MeshData meshData = MeshGenerator.CombineMeshData(
                temps[temp.type].GetComponent<MeshFilter>().mesh, 
                trees[temp.type].GetComponent<MeshFilter>().sharedMesh, 
                temp.pos,
                trees[temp.type].transform.localScale,
                out next
                );

            temps[temp.type].GetComponent<MeshFilter>().mesh = meshData.CreateMesh();
            
            meshData = null;
            if (next)
            {
                Debug.Log("Next");
                temps[temp.type] = Instantiate(trees[temp.type], Vector3.zero, Quaternion.identity, TreesParent.transform);
            }

            GameObject burntIcon = Instantiate(burntIconPrefab.gameObject, parent.transform);
            GameObject fire = Instantiate(firePrefab.gameObject, parent.transform);

            ForestTree forestTree = new ForestTree(burntIcon, fire, BurnTime, SpreadSpeed);
            treesOnTheScene.Add(forestTree);

        }

        foreach (ForestTree forestTree in treesOnTheScene)
        {
            foreach (ForestTree otherForestTree in treesOnTheScene)
            {
                if (forestTree == otherForestTree)
                    continue;
                float dist = Vector3.Distance(otherForestTree.burntIcon.transform.position, forestTree.burntIcon.gameObject.transform.position);
                if (dist < MaxDistanceToSpreadFire)
                    forestTree.closestTrees.Add(otherForestTree);
            }
        }
    }

    public void Update()
    {
        for (int i = 0; i < treesOnFire.Count; i++)
        {
            treesOnFire[i].Update();
        }
    }

    public void CombineMesh(GameObject obj)
    {

    }
}
