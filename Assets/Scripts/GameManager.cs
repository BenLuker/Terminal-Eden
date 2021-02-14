using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Libretto;

public class GameManager : SingletonBehaviour<GameManager>
{
    public LevelLibretto libretto;

    private void Start()
    {
        libretto.ProcessScenes();
    }
}
