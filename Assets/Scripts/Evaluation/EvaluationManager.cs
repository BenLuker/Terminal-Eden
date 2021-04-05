using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using TerminalEden.Simulation;

public class EvaluationManager : SingletonBehaviour<EvaluationManager>
{

    public int[] thresholds;
    public int[] cycles;
    [Space()]
    public GameObject evaluationPanel;
    public TextMeshProUGUI currentLevels;
    public TextMeshProUGUI requiredLevels;
    public TextMeshProUGUI flavorText;

    float forestLevels;

    Texture2D simulationTexture;

    private void Start()
    {
        simulationTexture = new Texture2D(WildfireSimulation.Instance.textureResolution, WildfireSimulation.Instance.textureResolution);
        simulationTexture.filterMode = FilterMode.Point;
    }

    public void Evaluate()
    {
        // Unhide UI
        evaluationPanel.SetActive(true);

        // Read simulation
        simulationTexture = WildfireSimulation.Instance.ReadSimulation();

        // Determine vegetation levels
        Color[] cells = simulationTexture.GetPixels();

        int forest = 0;
        foreach (Color cell in cells)
        {
            if (cell == new Color(0, 1, 0, 1))
            {
                forest++;
            }
        }

        forestLevels = ((float)forest / (float)Mathf.Pow(WildfireSimulation.Instance.visibleCellsWidth, 2)) * 100;
        float threshold = thresholds[GameManager.Instance.act];

        // Set Evaluation Text
        currentLevels.text = forestLevels.ToString("F2") + "%";
        requiredLevels.text = threshold.ToString("F2") + "%";
        flavorText.text = Succeed() ? "Good job. You may now access more terrain." : "Grow your forest to the required threshold to continue.";
    }

    public void OnContinue()
    {
        if (Succeed())
        {
            GameManager.Instance.NextPhase();
        }

        // Reset the cycle number
        CyclesManager.Instance.SetCycle(cycles[GameManager.Instance.act]);
    }

    public bool Succeed()
    {
        return forestLevels > thresholds[GameManager.Instance.act];
    }

}
