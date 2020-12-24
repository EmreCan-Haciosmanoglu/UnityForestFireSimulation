using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float[,] roadMask, int passes, bool level)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        MeshData meshData = new MeshData(width, height);
        for (int k = 0; k < passes; k++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (roadMask[x, y] > .5f)
                    {
                        float sum = 0f;
                        for (int i = -1; i <= 1; i++)
                            for (int j = -1; j <= 1; j++)
                                sum += heightMap[x + i, y + j];
                        sum /= 9f;
                        if (level)
                            heightMap[x, y] = heightMap[x - 1, y - 1];
                        else
                            for (int i = -1; i <= 1; i++)
                                for (int j = -1; j <= 1; j++)
                                    heightMap[x + i, y + j] = sum;
                    }
                }
            }
        }
        int vertexIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(x, heightMap[x, y], y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData CombineMeshData(Mesh m1, Mesh m2, Vector3 position, Vector3 scale, out bool next)
    {
        Vector3 scaledPosition = new Vector3(position.x / scale.x, position.y / scale.y, position.z / scale.z);
        int m1vl = m1.vertices.Length;
        int m1uvl = m1.uv.Length;
        int m1tl = m1.triangles.Length;

        int m2vl = m2.vertices.Length;
        int m2uvl = m2.uv.Length;
        int m2tl = m2.triangles.Length;
        Debug.Log(position);
        MeshData meshData = new MeshData(m1vl + m2vl, m1uvl + m2uvl, m1tl + m2tl);

        Array.Copy(m1.vertices, meshData.vertices, m1vl);

        Vector3[] vs = m2.vertices;
        //meshData.vertices[m1vl - 1] = vs[0] + scaledPosition;
        for (int i = 0; i < m2vl; i++)
            meshData.vertices[i + m1vl] = vs[i] + scaledPosition;

        Array.Copy(m1.uv, meshData.uvs, m1uvl);
        Array.Copy(m2.uv, 0, meshData.uvs, m1vl, m2uvl);

        Array.Copy(m1.triangles, meshData.triangles, m1tl);

        int[] ts = m2.triangles;
        for (int i = 0; i < m2tl; i++)
            meshData.triangles[i + m1tl] = ts[i] + m1vl;

        next = meshData.vertices.Length > 20000;
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public MeshData(int vSize, int uvSize, int tSize)
    {
        vertices = new Vector3[vSize];
        uvs = new Vector2[uvSize];
        triangles = new int[tSize];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex++] = a;
        triangles[triangleIndex++] = b;
        triangles[triangleIndex++] = c;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
