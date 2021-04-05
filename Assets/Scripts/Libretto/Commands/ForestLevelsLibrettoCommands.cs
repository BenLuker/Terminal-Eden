using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ForestLevelsManager))]
public class ForestLevelsLibrettoCommands : LibrettoCommands<ForestLevelsManager>
{
    public void UnlockForestLevels(string arguments)
    {
        subject.UnlockForestLevels();
        onCommandComplete.Invoke();
    }
}
