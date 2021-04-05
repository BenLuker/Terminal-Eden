using UnityEngine;
using UnityEngine.Events;
using TerminalEden.Simulation;

[RequireComponent(typeof(WildfireSimulation))]
public class SimulationLibrettoCommands : LibrettoCommands<WildfireSimulation>
{
    // No Arguments
    public void PlaySimulation(string arguments)
    {
        subject.PlayPauseSimulation(true);
        onCommandComplete.Invoke();
    }

    // No Arguments
    public void PauseSimulation(string arguments)
    {
        subject.PlayPauseSimulation(false);
        onCommandComplete.Invoke();
    }
}