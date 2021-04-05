using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CyclesManager))]
public class CyclesLibrettoCommands : LibrettoCommands<CyclesManager>
{
    public void UnlockCycles(string arguments)
    {
        subject.UnlockCycles();
        onCommandComplete.Invoke();
    }

    public void SetCycle(string arguments)
    {
        int c = int.Parse(arguments);
        subject.SetCycle(c);
        onCommandComplete.Invoke();
    }
}
