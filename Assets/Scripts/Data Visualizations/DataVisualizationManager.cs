﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataVisualizationManager : SingletonBehaviour<DataVisualizationManager>
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

    public void UnlockVisualization(int child)
    {
        transform.GetChild(child).gameObject.SetActive(true);
    }
}
