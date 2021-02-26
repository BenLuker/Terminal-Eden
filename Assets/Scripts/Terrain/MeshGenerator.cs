using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    [Header("Mesh Settings")]
    public float size = 20f;
    public int divisions = 1;
    [Range(10, 250)]
    public int resolution = 250;

    [Header("Terrain Settings")]
    [Range(1, 10)] public float height = 2f;
    [Range(0, 20)] public float noiseScale = 0.3f;

    Mesh mesh;
    MeshFilter meshFilter;

    Vector3[] verts;
    int[] tris;
    Vector2[] uvs;

    [ContextMenu("Create Mesh")]
    public void CreateMesh()
    {
        DestroyImmediate(gameObject.GetComponent<MeshCollider>());
        SetReferences();
        ConstructMesh();
    }

    void SetReferences()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void ConstructMesh()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Create Vertices
        verts = new Vector3[(resolution + 1) * (resolution + 1)];
        for (int i = 0, v = 0; i <= resolution; i++)
        {
            for (int j = 0; j <= resolution; j++, v++)
            {
                float x = ((j * size) / resolution) - (size / 2);
                float z = ((i * size) / resolution) - (size / 2);
                float y = Mathf.PerlinNoise(j * size * noiseScale / resolution, i * size * noiseScale / resolution) * height;

                verts[v] = new Vector3(x, y, z);
            }
        }

        // Create Tris
        tris = new int[resolution * resolution * 6];
        for (int j = 0, v = 0, t = 0; j < resolution; j++)
        {
            for (int i = 0; i < resolution; i++, v++, t += 6)
            {
                tris[t + 0] = v + 0;
                tris[t + 1] = v + resolution + 1;
                tris[t + 2] = v + 1;
                tris[t + 3] = v + 1;
                tris[t + 4] = v + resolution + 1;
                tris[t + 5] = v + resolution + 2;
            }
            v++;
        }

        // Create UVs
        uvs = new Vector2[verts.Length];
        for (int i = 0, u = 0; i <= resolution; i++)
        {
            for (int j = 0; j <= resolution; j++, u++)
            {
                uvs[u] = new Vector2((float)j / resolution, (float)i / resolution);
            }
        }

        // Apply Changes to Mesh
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        // Create Mesh Collider
        gameObject.AddComponent<MeshCollider>();
    }
}
