using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestLevelsManager : SingletonBehaviour<ForestLevelsManager>
{
    public bool hideOnStart;

    private void Start()
    {
        if (hideOnStart)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void UnlockForestLevels()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
