using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerminalEden.Simulation;
using TerminalEden.Terrain;
using Libretto;

public class GameManager : SingletonBehaviour<GameManager>
{
    public int act;
    public int block;

    // Game checks
    bool firstVisualizationUsed = false;
    bool firstAbilityUsed = false;
    bool firstEvaluationReached = false;

    private void Start()
    {
        // GameStart();
    }

    public void GameStart()
    {
        act = 0;
        block = 0;
        NextPhase();
    }

    public void NextPhase()
    {
        switch (act)
        {
            case 0:
                switch (block)
                {
                    case 0:
                        LevelLibretto.Instance.ProcessScene("Opening");
                        break;
                    case 1:
                        LevelLibretto.Instance.ProcessScene("FirstDataVisualization");
                        break;
                    case 2:
                        LevelLibretto.Instance.ProcessScene("FirstAbility");
                        break;
                    default:
                        NewAct();
                        return;
                }
                break;
            case 1:
                switch (block)
                {
                    case 0:
                        LevelLibretto.Instance.ProcessScene("NewTerritory");
                        break;
                    default:
                        NewAct();
                        return;
                }
                break;
        }
        block++;
    }

    void NewAct()
    {
        act++;
        block = 0;
        NextPhase();
    }

    // public void UnlockTerrain(int size)
    // {
    //     // Set Camera position
    //     TopDownCameraController.Instance.SetMaxPos(((float)size / (float)WildfireSimulation.Instance.textureResolution) * 10); // TODO Read actual terrain size.

    //     // Set camera limits
    //     TopDownCameraController.Instance.SetCameraDistanceLimits(1, size / 10);

    //     // zoom the camera out to the max
    //     TopDownCameraController.Instance.SetZoom(-size / 10);

    //     StartCoroutine(RevealMoreTerrainAfterSeconds(size, 1));
    //     // TopDownCameraController.Instance.TweenZoom(-size / 10, 2, true);

    //     // Reveal new cell width
    //     // WildfireSimulation.Instance.ChangeCellWidth(size);
    // }

    public void SetNewCameraLimits(int cellWidth)
    {
        // Set Camera position
        TopDownCameraController.Instance.SetMaxPos(((float)cellWidth / (float)WildfireSimulation.Instance.textureResolution) * 10); // TODO Read actual terrain size.

        // Set camera limits
        TopDownCameraController.Instance.SetCameraDistanceLimits(1, cellWidth / 10);

        // zoom the camera out to the max
        TopDownCameraController.Instance.SetZoom(-cellWidth / 10);
    }

    public void RevealNewCells(int cellWidth)
    {
        WildfireSimulation.Instance.ChangeCellWidth(cellWidth);
    }

    public void VisualizationChanged()
    {
        if (!firstVisualizationUsed)
        {
            firstVisualizationUsed = true;
            NextPhase();
        }
    }

    public void AbilityUsed()
    {
        if (!firstAbilityUsed)
        {
            firstAbilityUsed = true;
            NextPhase();
        }
    }

    // public void EvaluationReached()
    // {
    //     if (!firstEvaluationReached)
    //     {
    //         firstEvaluationReached = true;
    //         NextPhase();
    //     }
    // }
}
