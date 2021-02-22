using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Perception { Color, VegetationDensity }

public class WildfireSimulation : SingletonBehaviour<WildfireSimulation>
{
    // Display Material
    public Material displayMat;
    RenderTexture displayTexture;

    // Sim Materials
    public Material simMat;
    public Material initMat;

    // Perception Materials
    public Perception perception;
    public Material colorPerceptionMat;
    public Material vegetationDensityPerceptionMat;

    // Simulation Settings
    public RenderTexture[] sim;

    public int textureResolution = 1024;
    public int simStates = 26;
    public int currentState = 0;
    public int historyTracker;
    public Texture2D initialState;

    public bool displaySimulation = true;
    public bool generateStart = true;
    public bool updateOverTime = true;
    public int refreshRate = 30;

    #region Events

    private void Start()
    {
        InitSimulation();
        if (updateOverTime) ExecuteSimulation();
        UpdateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    #endregion

    #region Public Methods and Variables

    public void PlayPauseSimulation(bool play)
    {
        updateOverTime = play;

        if (updateOverTime)
        {
            ExecuteSimulation();
        }
        else
        {
            TerminateSimulation();
        }
    }

    #endregion

    void InitSimulation()
    {
        // Create Display Render Texture
        displayTexture = new RenderTexture(textureResolution, textureResolution, 1);
        displayTexture.filterMode = FilterMode.Point;
        displayTexture.Create();
        displayMat.mainTexture = displayTexture;

        // Create Simulation Render Textures
        sim = new RenderTexture[simStates];
        for (int i = 0; i < simStates; i++)
        {
            sim[i] = new RenderTexture(textureResolution, textureResolution, 1);
            sim[i].filterMode = FilterMode.Point;
            sim[i].Create();
        }

        // reset current state index to 0 
        currentState = 0;
        historyTracker = 0;

        // If starting is not generated, blit the initial state. Otherwise, blit a generated init
        if (generateStart)
        {
            Graphics.Blit(sim[currentState], sim[currentState], initMat);
        }
        else
        {
            Graphics.Blit(initialState, sim[currentState]);
        }

        // Set simulation variables to all textures
        simMat.SetInt("textureSize", textureResolution);
        vegetationDensityPerceptionMat.SetInt("textureSize", textureResolution);
    }

    [ContextMenu("Calculate Step")]
    public void CalculateStep()
    {
        int prevState = currentState;
        currentState = (currentState + 1) % simStates;
        historyTracker += historyTracker < simStates - 1 ? 1 : 0;

        Graphics.Blit(sim[prevState], sim[currentState], simMat);
    }

    [ContextMenu("Undo Step")]
    public void UndoStep()
    {
        if (historyTracker > 0)
        {
            historyTracker--;
            currentState = ((currentState - 1) + simStates) % simStates;
        }
        else
        {
            Debug.LogError("Cannot undo");
        }
    }

    [ContextMenu("Save Texture")]
    public void SaveTexture()
    {
        Texture2D tex = new Texture2D(textureResolution, textureResolution, TextureFormat.RGB24, false);

        RenderTexture.active = sim[currentState];
        tex.ReadPixels(new Rect(0, 0, textureResolution, textureResolution), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        byte[] bytes = tex.EncodeToPNG();
        string path = Application.dataPath + "/Scripts/Simulation";
        System.IO.Directory.CreateDirectory(path);
        System.IO.File.WriteAllBytes(path + "/SavedTexture.png", bytes);
    }

    void UpdateDisplay()
    {
        switch (perception)
        {
            case Perception.Color:
                Graphics.Blit(sim[currentState], displayTexture, colorPerceptionMat);
                break;
            case Perception.VegetationDensity:
                Graphics.Blit(sim[currentState], displayTexture, vegetationDensityPerceptionMat);
                break;
        }
    }

    public void ExecuteSimulation()
    {
        StartCoroutine(ExecuteSimulationCoroutine());
    }

    public void TerminateSimulation()
    {
        StopAllCoroutines();
    }

    IEnumerator ExecuteSimulationCoroutine()
    {
        while (true)
        {
            CalculateStep();
            yield return new WaitForSeconds(1 / (float)refreshRate);
        }
    }

    #region Helper Functions

    public Vector2Int WorldToCell(Vector3 pos)
    {
        float xPos = Mathf.InverseLerp(transform.position.x - transform.localScale.x / 2, transform.position.x + transform.localScale.x / 2, pos.x);
        float zPos = Mathf.InverseLerp(transform.position.z - transform.localScale.y / 2, transform.position.z + transform.localScale.y / 2, pos.z);

        int x = (int)Mathf.Round(Mathf.Lerp(0, textureResolution, xPos));
        int z = (int)Mathf.Round(Mathf.Lerp(0, textureResolution, zPos));

        return new Vector2Int(x, z);
    }

    #endregion
}
