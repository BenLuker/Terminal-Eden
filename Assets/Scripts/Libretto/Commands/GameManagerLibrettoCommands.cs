using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class GameManagerLibrettoCommands : LibrettoCommands<GameManager>
{
    private IEnumerator wait;

    // Command cells
    public void UnlockTerrain(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        int cells = int.Parse(a[0]);
        int seconds = int.Parse(a[1]);

        subject.SetNewCameraLimits(cells);
        wait = RevealTerrainAfterSeconds(cells, seconds);
        StartCoroutine(wait);

    }

    // A simple IEnumerator that returns after a specified number of seconds
    IEnumerator RevealTerrainAfterSeconds(int cells, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        subject.RevealNewCells(cells);
        onCommandComplete.Invoke();
    }

}