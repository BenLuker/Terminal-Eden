using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DatabaseManager))]
public class DatabaseLibrettoCommands : LibrettoCommands<DatabaseManager>
{
    public void UnlockDatabase(string arguments)
    {
        subject.UnlockDatabase();
        onCommandComplete.Invoke();
    }

    public void UnlockTab(string arguments)
    {
        string[] a = arguments.Split(' ');

        int tab = int.Parse(a[0]);

        subject.UnlockTab(tab);
        onCommandComplete.Invoke();
    }
}