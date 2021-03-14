﻿using UnityEngine;
using UnityEngine.Events;
using TerminalEden.Simulation;

[RequireComponent(typeof(WildfireSimulation))]
public class SimulationLibrettoCommands : MonoBehaviour
{

    public UnityEvent commandComplete;

    #region Methods for Libretto

    #region Settings

    // No Arguments
    public void PlaySimulation(string arguments)
    {
        WildfireSimulation.Instance.PlayPauseSimulation(true);
        commandComplete.Invoke();
    }

    // No Arguments
    public void PauseSimulation(string arguments)
    {
        WildfireSimulation.Instance.PlayPauseSimulation(false);
        commandComplete.Invoke();
    }

    #endregion

    #endregion

}