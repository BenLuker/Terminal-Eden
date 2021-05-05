using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshManager : MonoBehaviour
{
    public bool updateMesh;
    // public Texture2D meshData;

    [Header("Mesh Settings")]
    public int divisions = 1;
    [Range(10, 250)]
    public int resolution = 250;

    [Header("Terrain Settings")]
    public float size = 20f;
    public NoiseFilter noiseFilter;
    public float seedSpeed;

    [Range(0, 10)] public float height = 2f;
    [Range(0, 20)] public float noiseScale = 0.3f;

    Mesh mesh;
    MeshFilter meshFilter;

    Vector3[] verts;
    int[] tris;
    Vector2[] uvs;

    private void Update()
    {
        if (updateMesh) UpdateMesh();
    }

    [ContextMenu("Create Mesh")]
    public void CreateMesh()
    {
        DestroyImmediate(gameObject.GetComponent<MeshCollider>());
        SetReferences();
        ConstructMesh();
        UpdateMesh();
    }

    public Texture2D WriteMesh()
    {
        Texture2D meshData = new Texture2D(resolution + 1, resolution + 1);
        GetComponent<Renderer>().material.mainTexture = meshData;

        Color[] cols = new Color[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            cols[i] = new Color(0, 0, Mathf.InverseLerp(0, 1, verts[i].y));
        }

        meshData.SetPixels(cols);
        meshData.Apply();
        return meshData;
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
    }

    [ContextMenu("Update Mesh")]
    public void UpdateMesh()
    {
        // Go into the noise filters seed and push it up by 0.001
        noiseFilter.seed += seedSpeed;

        // Update Vertex Positions
        for (int i = 0, v = 0; i <= resolution; i++)
        {
            for (int j = 0; j <= resolution; j++, v++)
            {
                float x = ((j * size) / resolution) - (size / 2);
                float z = ((i * size) / resolution) - (size / 2);
                float y = noiseFilter.Evaluate(new Vector2(j * size / resolution, i * size / resolution));

                verts[v] = new Vector3(x, y, z);
            }
        }

        // Update UVs
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
