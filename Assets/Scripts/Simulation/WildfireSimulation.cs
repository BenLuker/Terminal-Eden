using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildfireSimulation : MonoBehaviour
{
    Material mat;

    // Shader Settings
    public Shader simShader;
    Material simMat;
    public RenderTexture[] sim;

    public Texture2D initialState;
    public int textureResolution = 256;
    public int simStates = 2;
    public int historyTracker;
    int currentState = 0;

    public bool updateOverTime = true;
    public int refreshRate = 1;

    private void Reset()
    {
        simShader = Shader.Find("Hidden/WildfireSimulation");
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

        // reset current state index to 0 and copy the initial state to first texture 
        currentState = 0;
        historyTracker = 0;
        Graphics.Blit(initialState, sim[currentState]);

        // Create Material with Simulation Shader
        simMat = new Material(simShader);
        simMat.SetInt("textureSize", textureResolution);

        // Update viewable material
        Renderer rend = GetComponent<Renderer>();
        rend.enabled = true;

        mat = rend.material;
        mat.SetTexture("_BaseMap", sim[currentState]);
    }

    [ContextMenu("Calculate Step")]
    public void CalculateStep()
    {
        int prevState = currentState;
        currentState = (currentState + 1) % simStates;
        historyTracker += historyTracker < simStates - 1 ? 1 : 0;

        Graphics.Blit(sim[prevState], sim[currentState], simMat);

        mat.SetTexture("_BaseMap", sim[currentState]);
    }

    [ContextMenu("Undo Step")]
    public void UndoStep()
    {
        if (historyTracker > 0)
        {
            historyTracker--;
            currentState = ((currentState - 1) + simStates) % simStates;
            mat.SetTexture("_BaseMap", sim[currentState]);
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
