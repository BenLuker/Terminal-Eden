using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilitiesManager))]
public class AbilitiesLibrettoCommands : LibrettoCommands<AbilitiesManager>
{
    public void UnlockAbility(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        int child = int.Parse(a[0]);

        subject.UnlockAbility(child);
        onCommandComplete.Invoke();
    }
}