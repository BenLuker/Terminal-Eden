using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TerrainManager : MonoBehaviour
{

    public int testXCoord;
    public int testYCoord;
    public int testState;

    public Automata automata;
    public GameObject terrainAssets;
    public Terrain compositeTerrain;
    public Terrain[] terrainParts = new Terrain[4];

    public GameObject prefab;
    public float spawnNumber;

    List<Animator>[,] vegetation;
    int alphaScale = 1;

    #region Events

    public void UpdateTerrainAndAssets(int x, int y, float v)
    {
        float[,,] splatMap = terrainParts[(int)v].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
        compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);

        for (int i = 0; i < vegetation[x, y].Count; i++)
        {
            if (v == 0)
            {
                if (vegetation[x, y][i].GetCurrentAnimatorStateInfo(0).IsName("Overgrown Burning"))
                {
                    vegetation[x, y][i].Play("Overgrown Burned");
                }
                else
                {
                    vegetation[x, y][i].Play("Burned");
                }
            }
            else if (v == 1)
            {
                vegetation[x, y][i].Play("Grown");
            }
            else if (v == 2)
            {
                vegetation[x, y][i].Play("Overgrown");
            }
            else if (v == 3)
            {
                if (vegetation[x, y][i].GetCurrentAnimatorStateInfo(0).IsName("Overgrown"))
                {
                    vegetation[x, y][i].Play("Overgrown Burning");
                }
                else
                {
                    vegetation[x, y][i].Play("Burning");
                }
            }
        }
    }

    #endregion

    [ContextMenu("Clump Vegetation")]
    public void InitVegetation()
    {
        CreateVegetation();
        ClumpVegetation();
    }

    void CreateVegetation()
    {
        vegetation = new List<Animator>[automata.gridSize, automata.gridSize];

        for (int i = 0; i < automata.gridSize; i++)
        {
            for (int j = 0; j < automata.gridSize; j++)
            {
                vegetation[j, i] = new List<Animator>();
            }
        }
    }

    void ClumpVegetation()
    {

        for (int i = 0; i < terrainAssets.transform.childCount; i++)
        {
            // Get child
            Transform child = terrainAssets.transform.GetChild(i);

            // Get closest cell coordinates to transform
            Vector2Int coords = automata.GetClosestCell(child.transform.position);

            // Add child to list at those coordinates in the array
            vegetation[coords.x, coords.y].Add(child.gameObject.GetComponent<Animator>());
        }
    }

    [ContextMenu("Spawn Trees")]
    void SpawnVegetationRandomly()
    {
        int seed = System.DateTime.UtcNow.Millisecond;
        Random.InitState(seed);
        for (int i = 0; i < spawnNumber; i++)
        {
            float x = Random.Range(0, automata.grid.terrain.terrainData.size.x);
            float z = Random.Range(0, automata.grid.terrain.terrainData.size.z);
            float y = automata.grid.terrain.SampleHeight(new Vector3(x, 0, z));
            Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity, terrainAssets.transform);
        }
    }

    [ContextMenu("Spawn Uniform")]
    void SpawnVegetationUniform()
    {
        for (int i = 0; i < automata.gridSize; i++)
        {
            for (int j = 0; j < automata.gridSize; j++)
            {
                Instantiate(prefab, automata.CellToWorldSpace(j, i), Quaternion.identity, terrainAssets.transform);
            }
        }
    }

    [ContextMenu("Spawn Grass")]
    void SpawnUniformlyRandomRotations()
    {
        int seed = System.DateTime.UtcNow.Millisecond;
        Random.InitState(seed);
        for (int i = 0; i < automata.gridSize; i++)
        {
            for (int j = 0; j < automata.gridSize; j++)
            {
                int multiplier = Random.Range(0, 4);
                Quaternion rot = Quaternion.Euler(0, 90 * multiplier, 0);
                Instantiate(prefab, automata.CellToWorldSpace(j, i), rot, terrainAssets.transform);
            }
        }
    }

    [ContextMenu("DeleteVegetation")]
    void DeleteVegetation()
    {
        foreach (Transform child in terrainAssets.transform.Cast<Transform>().ToList())
        {
            DestroyImmediate(child.gameObject);
        }
    }

    void FillFirstHalf()
    {
        for (int i = 0; i < automata.gridSize / 2; i++)
        {
            for (int j = 0; j < automata.gridSize; j++)
            {
                Instantiate(prefab, automata.CellToWorldSpace(j, i), Quaternion.identity, terrainAssets.transform);
            }
        }
    }

    void FillSecondHalf()
    {
        for (int i = automata.gridSize / 2; i < automata.gridSize; i++)
        {
            for (int j = 0; j < automata.gridSize; j++)
            {
                Instantiate(prefab, automata.CellToWorldSpace(j, i), Quaternion.identity, terrainAssets.transform);
            }
        }
    }

    [ContextMenu("Snap to Grid")]
    void SnapToGrid()
    {
        for (int i = 0; i < terrainAssets.transform.childCount; i++)
        {
            // Get child
            Transform child = terrainAssets.transform.GetChild(i);

            // Get closest cell coordinates to transform
            Vector2Int coords = automata.GetClosestCell(child.transform.position);

            // Add child to list at those coordinates in the array
            vegetation[coords.x, coords.y].Add(child.gameObject.GetComponent<Animator>());

            // Move them to those coordinates
            child.transform.position = automata.CellToWorldSpace(coords.x, coords.y);

            Quaternion rot = Quaternion.Euler(0, 90, 0);
            child.transform.rotation = rot;
        }
    }

    [ContextMenu("Burn Second Half")]
    void BurnSecondHalf()
    {
        for (int y = automata.gridSize / 2; y < automata.gridSize; y++)
        {
            for (int x = 0; x < automata.gridSize; x++)
            {
                for (int i = 0; i < vegetation[x, y].Count; i++)
                {
                    float[,,] splatMap = terrainParts[3].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
                    compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);
                    vegetation[x, y][i].Play("Burned");
                }
            }
        }
    }

    [ContextMenu("Burn All")]
    void BurnAll()
    {
        for (int y = 0; y < automata.gridSize; y++)
        {
            for (int x = 0; x < automata.gridSize; x++)
            {
                for (int i = 0; i < vegetation[x, y].Count; i++)
                {
                    float[,,] splatMap = terrainParts[3].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
                    compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);
                    vegetation[x, y][i].Play("Burning");
                }
            }
        }
    }

    [ContextMenu("Burned All")]
    void BurnedAll()
    {
        for (int y = 0; y < automata.gridSize; y++)
        {
            for (int x = 0; x < automata.gridSize; x++)
            {
                for (int i = 0; i < vegetation[x, y].Count; i++)
                {
                    float[,,] splatMap = terrainParts[0].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
                    compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);
                    vegetation[x, y][i].Play("Burned");
                }
            }
        }
    }

    [ContextMenu("Grow All")]
    void GrowAll()
    {
        for (int y = 0; y < automata.gridSize; y++)
        {
            for (int x = 0; x < automata.gridSize; x++)
            {
                for (int i = 0; i < vegetation[x, y].Count; i++)
                {
                    float[,,] splatMap = terrainParts[1].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
                    compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);
                    vegetation[x, y][i].Play("Grown");
                }
            }
        }
    }

    [ContextMenu("Overgrow All")]
    void OvergrowAll()
    {
        for (int y = 0; y < automata.gridSize; y++)
        {
            for (int x = 0; x < automata.gridSize; x++)
            {
                for (int i = 0; i < vegetation[x, y].Count; i++)
                {
                    float[,,] splatMap = terrainParts[2].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
                    compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);
                    vegetation[x, y][i].Play("Overgrown");
                }
            }
        }
    }

    public void AffectAllCells(int v)
    {
        for (int y = 0; y < automata.gridSize; y++)
        {
            for (int x = 0; x < automata.gridSize; x++)
            {
                for (int i = 0; i < vegetation[x, y].Count; i++)
                {
                    float[,,] splatMap = terrainParts[v].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
                    compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);
                    if (v == 0)
                    {
                        vegetation[x, y][i].Play("Burned");
                    }
                    else if (v == 1)
                    {
                        vegetation[x, y][i].Play("Grown");
                    }
                    else
                    {
                        vegetation[x, y][i].Play("Burning");
                    }
                }
            }
        }
    }

    public void AffectOneCell(int x, int y, int v)
    {
        for (int i = 0; i < vegetation[x, y].Count; i++)
        {
            float[,,] splatMap = terrainParts[v].terrainData.GetAlphamaps(x * alphaScale, y * alphaScale, alphaScale, alphaScale);
            compositeTerrain.terrainData.SetAlphamaps(x * alphaScale, y * alphaScale, splatMap);
            if (v == 0)
            {
                vegetation[x, y][i].Play("Burned");
            }
            else if (v == 1)
            {
                vegetation[x, y][i].Play("Grown");
            }
            else
            {
                vegetation[x, y][i].Play("Burning");
            }
        }
    }

    [ContextMenu("Test Affecting All States")]
    public void TestAffectingAllStates()
    {
        AffectAllCells(testState);
    }

    [ContextMenu("Test Affecting One State")]
    public void TestAffectingOneState()
    {
        AffectOneCell(testXCoord, testYCoord, testState);
    }
}
