using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildfireSimulation : MonoBehaviour
{
    // Shader Settings
    public Shader simShader;
    public Shader initShader;

    Material simMat;
    Material initMat;
    Material displayMat;

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

    private void Reset()
    {
        simShader = Shader.Find("Hidden/WildfireSimulation");
        initShader = Shader.Find("Hidden/GenerateTerrain");
    }

    private void Start()
    {
        InitSimulation();
        if (updateOverTime) ExecuteSimulation();
    }

    void InitSimulation()
    {
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
            Graphics.Blit(sim[currentState], sim[currentState], new Material(initShader));
        }
        else
        {
            Graphics.Blit(initialState, sim[currentState]);
        }

        // Create Material with Simulation Shader
        simMat = new Material(simShader);
        simMat.SetInt("textureSize", textureResolution);

        // Create Material with Display Shader
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = true;

        displayMat = rend.material;
        displayMat.mainTexture = sim[currentState];
    }

    [ContextMenu("Calculate Step")]
    public void CalculateStep()
    {
        int prevState = currentState;
        currentState = (currentState + 1) % simStates;
        historyTracker += historyTracker < simStates - 1 ? 1 : 0;

        Graphics.Blit(sim[prevState], sim[currentState], simMat);

        displayMat.mainTexture = sim[currentState];
    }

    [ContextMenu("Undo Step")]
    public void UndoStep()
    {
        if (historyTracker > 0)
        {
            historyTracker--;
            currentState = ((currentState - 1) + simStates) % simStates;
            displayMat.mainTexture = sim[currentState];
        }
        else
        {
            Debug.LogError("Cannot undo");
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
}
