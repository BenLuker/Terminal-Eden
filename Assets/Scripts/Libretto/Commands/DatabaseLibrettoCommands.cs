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
}