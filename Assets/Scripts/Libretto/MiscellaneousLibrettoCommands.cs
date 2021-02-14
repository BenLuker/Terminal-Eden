using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiscellaneousLibrettoCommands : MonoBehaviour
{

    public UnityEvent commandComplete;

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
        commandComplete.Invoke();
    }
}
