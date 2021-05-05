using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// [RequireComponent(typeof(GridMesh))]
// [RequireComponent(typeof(MeshRenderer))]
public class Automata : MonoBehaviour
{
    [Header("Display")]
    // public bool overlayTexture = false;

    [Header("Init")]
    public int gridSize = 100;
    [Tooltip("-1 will input random seed")]
    public int inputSeed = 1;
    [Range(0, 1)]
    public float fillPercentage = 0.5f;
    public bool updateInInspector = true;

    [Header("Update")]
    public bool pulseSystem = false;
    [Range(0.001f, 20f)]
    public float refreshRate = 0.05f;

    [Header("Global Parameters")]
    public Vector2Int windDirection = new Vector2Int(0, 1);
    public int windSpeed = 2;

    [Header("Percentages")]
    public float growSpawn = 0.0001f;
    public float overgrowSpawn = 0.0001f;
    public float burnSpawn = 0.0001f;

    [Range(0f, 1f)] public float growSpread = 0.01f;
    [Range(0f, 1f)] public float overgrowSpread = 0.01f;
    [Range(0f, 1f)] public float burnSpread = 0.1f;
    [Range(0f, 1f)] public float burnWithFuelSpread = 0.2f;

    [Header("Events")]
    public initializeGrid oninitializeGrid = new initializeGrid();
    public CellChange onCellChange = new CellChange(); // This event will be called with the coordinates of a cell when the cell is changed

    public float[,] cellData; // The states are floats instead of ints because I would like to eventually store the transition between states 
    public GridMesh grid; // I am using a Unity terrain to define the position and scale of the system because I will be eventually linking this simulation to a 3D terrain
    Texture2D texture;
    Material mat;

    [System.Serializable]
    public class initializeGrid : UnityEvent { }

    [System.Serializable]
    public class CellChange : UnityEvent<int, int, float> { }

    #region Events
    void Start()
    {
        updateInInspector = false;
        InitGrid();
        CallEvents();
        // StartSimulation();
    }

    private void OnDrawGizmos()
    {
        if (updateInInspector)
        {
            InitGrid();
        }
    }

    private void Update()
    {
        // if (updateInInspector)
        // {
        //     InitGrid();
        // }
        // if (pulseSystem)
        // {
        //     updateInInspector = false;
        //     StopAllCoroutines();
        //     InitGrid();
        //     StartSimulation();
        //     // SparkFire();
        //     pulseSystem = false;
        // }

        // if (Input.GetMouseButton(0))
        // {
        //     RaycastHit hit;
        //     int layerMask = 1 << 8;
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        //     {
        //         Vector2Int selectedCoords = GetClosestCell(hit.point);
        //         if (cellData[selectedCoords.x, selectedCoords.y] != 3)
        //             onCellChange.Invoke(selectedCoords.x, selectedCoords.y, 3);
        //         cellData[selectedCoords.x, selectedCoords.y] = 3;

        //     }

        // }

        // if (Input.GetMouseButton(1))
        // {
        //     RaycastHit hit;
        //     int layerMask = 1 << 8;
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        //     {
        //         Vector2Int selectedCoords = GetClosestCell(hit.point);
        //         cellData[selectedCoords.x, selectedCoords.y] = 1;
        //     }

        // }
    }

    #endregion

    [ContextMenu("Init Grid")]
    void InitGrid()
    {
        RandomSeed();
        CreateGrid();
        FillGrid();
        UpdateTexture();
        oninitializeGrid.Invoke();
    }

    // This method creates a seed for randomness
    void RandomSeed()
    {
        if (inputSeed == -1)
        {
            int seed = System.DateTime.UtcNow.Millisecond;
            Debug.Log("Random seed: " + seed);
            Random.InitState(seed);
        }
        else
        {
            Random.InitState(inputSeed);
        }
    }

    // This method creates the cell data and communicates with the Automata grid shader to create the overlay texture
    void CreateGrid()
    {
        cellData = new float[gridSize, gridSize];
        texture = new Texture2D(gridSize, gridSize, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;

        grid = GetComponent<GridMesh>();
        mat = GetComponent<Renderer>().sharedMaterial;

        // mat.SetTexture("Texture2D_7A430E2A", texture);
        // mat.SetFloat("Vector1_B11F30B6", grid.terrain.terrainData.size.x / gridSize);
        // mat.SetFloat("Vector1_7001725E", gridSize);
        // mat.SetInt("Boolean_4535CF09", overlayTexture ? 1 : 0);
    }

    // This method fills a grid with grown and overgrown cells
    void FillGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                cellData[j, i] = Random.value < fillPercentage ? 2 : 1;
            }
        }
    }

    void CallEvents()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                onCellChange.Invoke(j, i, cellData[j, i]);
                // Debug.Log("Change made");
            }
        }
    }

    /// <summary> This method is what calculates the state of a cell for the next generation.
    /// States are loosely based on https://nhess.copernicus.org/preprints/nhess-2018-227/nhess-2018-227.pdf
    /// Rules are tuned to facilitate fun/cool simulation results
    /// States: 0 = burned, 1 = grown, 2 = overgrown, 3 = burning
    /// If state = burned, it has a chance of growing. It cannot burn in this state.
    /// If state = grown, it has a chance of burning if cells around it are burning. Otherwise it has a chance of overgrowing if cells around it are overgrown. It can also overgrow just by chance.
    /// If state = overgrown, it has a higher chance of burning if cells around it are burning than if it were just grown.
    /// If state = burning, it will become burned in the next cycle
    /// </summary>
    void CalculateStep()
    {
        float[,] newCellData = new float[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                float cell = cellData[j, i];
                List<float> neighbors = CalculateNeighbors(j, i);

                // Calculate Rates based on neighbors
                float growthRate = 0;
                float overGrowthRate = 0;
                float burnRate = 0;
                foreach (float n in neighbors)
                {
                    if (n == 0) { }
                    else if (n == 1) { growthRate++; }
                    else if (n == 2) { overGrowthRate++; }
                    else if (n == 3) { burnRate++; }
                }

                // Calculate new state based on current state and rates from neighbors
                if (cell == 0)
                {
                    if (Random.value / (growthRate + overGrowthRate) < growSpread)
                        newCellData[j, i] = 1;
                    else if (Random.value < growSpawn)
                        newCellData[j, i] = 1;
                    else
                        newCellData[j, i] = 0;
                }
                else if (cell == 1)
                {
                    if (Random.value / burnRate < burnSpread)
                        newCellData[j, i] = 3;
                    else if (Random.value < burnSpawn)
                        newCellData[j, i] = 3;
                    else if (Random.value < overgrowSpawn)
                        newCellData[j, i] = 2;
                    else if (Random.value / overGrowthRate < overgrowSpread)
                        newCellData[j, i] = 2;
                    else
                        newCellData[j, i] = 1;
                }
                else if (cell == 2)
                {
                    if (Random.value / burnRate < burnWithFuelSpread)
                        newCellData[j, i] = 3;
                    else if (Random.value < burnSpawn)
                        newCellData[j, i] = 3;
                    else
                        newCellData[j, i] = 2;
                }
                else if (cell == 3)
                {
                    newCellData[j, i] = 0;
                }

                // If new state is different from old, invoke the onCellChange event
                if (cell != newCellData[j, i])
                    onCellChange.Invoke(j, i, newCellData[j, i]);

            }
        }

        // Finally, replace the old cell data with the new cell data
        cellData = newCellData;
    }

    void UpdateTexture()
    {
        // Color[] cols = texture.GetPixels();
        // for (int i = 0; i < gridSize; i++)
        // {
        //     for (int j = 0; j < gridSize; j++)
        //     {
        //         float cell = cellData[j, i];

        //         if (cell == 0)
        //             cols[j + gridSize * i] = Color.black;
        //         else if (cell == 1)
        //             cols[j + gridSize * i] = new Color(11f / 255f, 102f / 255f, 35f / 255f);
        //         else if (cell == 2)
        //             cols[j + gridSize * i] = new Color(0f / 255f, 78f / 255f, 56f / 255f);
        //         else if (cell == 3)
        //             cols[j + gridSize * i] = Color.red;
        //     }
        // }
        // texture.SetPixels(cols);
        // texture.Apply(false);
    }

    IEnumerator UpdateSystem()
    {
        while (true)
        {
            // if (overlayTexture)
            // UpdateTexture();
            CalculateStep();
            yield return new WaitForSeconds(refreshRate);
        }
    }

    void StartSimulation()
    {
        StartCoroutine(UpdateSystem());
    }

    [ContextMenu("Burn")]
    void SparkFire()
    {
        cellData[2, 2] = 3;
        onCellChange.Invoke(2, 2, 3);
    }

    #region Helper Functions

    // This method creates a list of neighbors including the surrounding cells. It also uses the wind variable to add cells to the list of neighbors
    List<float> CalculateNeighbors(int x, int y)
    {
        List<float> neighbors = new List<float>();

        int leftIndex, rightIndex, upIndex, downIndex;
        leftIndex = x - 1;
        rightIndex = x + 1;
        upIndex = y + 1;
        downIndex = y - 1;

        // Top row
        if (upIndex < cellData.GetLength(1))
        {
            // Left
            if (leftIndex >= 0) { neighbors.Add(cellData[leftIndex, upIndex]); }
            // Middle
            neighbors.Add(cellData[x, upIndex]);
            // Right
            if (rightIndex < cellData.GetLength(0)) { neighbors.Add(cellData[rightIndex, upIndex]); }
        }

        //  Middle row
        if (leftIndex >= 0) { neighbors.Add(cellData[leftIndex, y]); }
        if (rightIndex < cellData.GetLength(0)) { neighbors.Add(cellData[rightIndex, y]); }

        // Bottom row
        if (downIndex >= 0)
        {
            // Left
            if (leftIndex >= 0) { neighbors.Add(cellData[leftIndex, downIndex]); }
            // Middle
            neighbors.Add(cellData[x, downIndex]);
            // Right
            if (rightIndex < cellData.GetLength(0)) { neighbors.Add(cellData[rightIndex, downIndex]); }
        }

        // Add more neighbors given the wind direction and strength
        Vector2Int index;
        for (int i = 1; i <= windSpeed; i++)
        {
            index = new Vector2Int(x - (windDirection.x * (i + 1)), y - (windDirection.y * (i + 1)));

            if (index.x >= 0 && index.y >= 0 && index.x < cellData.GetLength(0) && index.y < cellData.GetLength(1))
            {
                neighbors.Add(cellData[index.x, index.y]);
            }
        }

        return neighbors;
    }

    public Vector3 CellToWorldSpace(int j, int i)
    {
        float x = ((grid.terrain.terrainData.size.x / (gridSize)) * j) + (grid.terrain.terrainData.size.x / (gridSize) / 2);
        float z = ((grid.terrain.terrainData.size.z / (gridSize)) * i) + (grid.terrain.terrainData.size.z / (gridSize) / 2);
        float y = grid.terrain.SampleHeight(new Vector3(x, 0, z));
        return new Vector3(x, y, z);
    }

    public Vector2Int GetClosestCell(Vector3 pos, bool calculateY = false)
    {
        Vector2Int minCoord = new Vector2Int(1000000, 1000000);
        float minDist = Mathf.Infinity;

        for (int i = 0; i < cellData.GetLength(1); i++)
        {
            for (int j = 0; j < cellData.GetLength(0); j++)
            {

                Vector3 worldSpace = CellToWorldSpace(j, i);
                float dist = calculateY ? Vector3.Distance(pos, worldSpace) : Vector2.Distance(new Vector2(pos.x, pos.z), new Vector3(worldSpace.x, worldSpace.z));

                if (dist < minDist)
                {
                    minCoord = new Vector2Int(j, i);
                    minDist = dist;
                }
            }
        }
        return minCoord;
    }

    void ChangeSelectedCoord(float state)
    {

    }

    #endregion

}
