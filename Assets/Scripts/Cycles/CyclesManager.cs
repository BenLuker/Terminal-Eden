using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CyclesManager : SingletonBehaviour<CyclesManager>
{
    public int cycle;
    public GameObject cycles;
    public TextMeshProUGUI cyclesNumber;
    public bool unlockAtStart;

    public UnityEvent onCyclesCompleted = new UnityEvent();

    private void Start()
    {
        if (!unlockAtStart) HideCycles(true);
        UpdateDisplay();
    }

    public void HideCycles(bool b)
    {
        cycles.SetActive(!b);
    }

    public void Cycle()
    {
        cycle--;
        UpdateDisplay();
        if (cycle == 0)
        {
            onCyclesCompleted.Invoke();
        }
    }

    public void SetCycle(int c)
    {
        cycle = c;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        cyclesNumber.text = cycle.ToString();
    }

}
