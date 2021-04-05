using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libretto;

[RequireComponent(typeof(LevelLibretto))]
public class MiscellaneousLibrettoCommands : LibrettoCommands<LevelLibretto>
{
    private IEnumerator wait;

    #region Methods for Libretto

    // Wait Command
    public void Wait(string time)
    {
        float seconds = float.Parse(time);
        wait = ReturnAfterSeconds(seconds);
        StartCoroutine(wait);
    }

    #endregion

    // A simple IEnumerator that returns after a specified number of seconds
    IEnumerator ReturnAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        onCommandComplete.Invoke();
    }
}
