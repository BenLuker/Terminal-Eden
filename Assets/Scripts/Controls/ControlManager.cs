using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : SingletonBehaviour<ControlManager>
{
    public bool unlockAtStart;
    public GameObject controls;

    private void Start()
    {
        if (!unlockAtStart) HideControls(true);
    }

    public void HideControls(bool b)
    {
        controls.SetActive(!b);
    }
}
