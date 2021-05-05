using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Automata))]
[RequireComponent(typeof(MeshFilter))]
public class GridMesh : MonoBehaviour
{
    public Terrain terrain;
    [Range(10, 250)]
    public int resolution = 250;

    Mesh mesh;
    MeshFilter meshFilter;

    Vector3[] verts;
    int[] tris;

    [ContextMenu("Create Grid Mesh")]
    void CreateGridMesh()
    {
        DestroyImmediate(gameObject.GetComponent<MeshCollider>());
        SetReferences();
        ConstructMesh();
        gameObject.AddComponent<MeshCollider>();
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
                float x = (j * terrain.terrainData.size.x) / resolution;
                float z = (i * terrain.terrainData.size.z) / resolution;
                float y = terrain.SampleHeight(new Vector3(x, 0, z));

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

        // Apply Changes to Mesh
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }
}
