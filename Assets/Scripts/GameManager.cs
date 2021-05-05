using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TerminalEden.Simulation;
using TerminalEden.Terrain;
using Libretto;

public class GameManager : SingletonBehaviour<GameManager>
{
    public VNDialogue vNDialogue;
    public bool playTutorial = true;

    public int act;
    public int block;
    public int totalCycles;
    public UnityEvent onGameEnd = new UnityEvent();

    GameManagerLibrettoCommands librettoCommands;

    // Game checks
    bool firstVisualizationUsed = false;
    bool firstAbilityUsed = false;
    bool firstEvaluationReached = false;

    private void Start()
    {
        librettoCommands = GetComponent<GameManagerLibrettoCommands>();

        if (playTutorial)
        {
            GameStart();
        }
        else
        {

        }
    }

    [ContextMenu("Turntable Simulation")]
    public void TurntableSimulation()
    {
        librettoCommands.UnlockTerrain("1024 1");
        TopDownCameraController.Instance.StartTurntable(-0.1f);
        TopDownCameraController.Instance.TweenYRotation(30, 2);
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
                        TerminalEden.Terrain.TerrainManager.Instance.rend.material.SetVector("_Wind", new Vector3(0, 1, 1));
                        break;
                    default:
                        NewAct();
                        return;
                }
                break;
            case 2:
                switch (block)
                {
                    case 0:
                        LevelLibretto.Instance.ProcessScene("UnlockEverything");
                        break;
                    default:
                        NewAct();
                        return;
                }
                break;
            case 3:
                switch (block)
                {
                    case 0:
                        librettoCommands.UnlockTerrain("512 1");
                        break;
                    default:
                        NewAct();
                        return;
                }
                break;
            case 4:
                switch (block)
                {
                    case 0:
                        librettoCommands.UnlockTerrain("1024 1");
                        break;
                    case 1:
                        LevelLibretto.Instance.ProcessScene("End");
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

    public void EndGame()
    {
        vNDialogue.NarratorTalk(String.Format("Congratulations. You have grown the garden to the required threshold after {0} cycles.", totalCycles));
        onGameEnd.Invoke();
    }

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

    public void AddToCyclesCount()
    {
        totalCycles++;
    }
}
