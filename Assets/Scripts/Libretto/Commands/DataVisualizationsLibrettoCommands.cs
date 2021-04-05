using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DataVisualizationManager))]
public class DataVisualizationsLibrettoCommands : LibrettoCommands<DataVisualizationManager>
{
    public void UnlockVisualization(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        int child = int.Parse(a[0]);

        subject.UnlockVisualization(child);
        onCommandComplete.Invoke();
    }
}