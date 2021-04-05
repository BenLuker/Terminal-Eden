using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CyclesManager : SingletonBehaviour<CyclesManager>
{
    public int cycle;
    public TextMeshProUGUI number;
    public bool hideOnStart;

    public UnityEvent onCyclesCompleted = new UnityEvent();

    private void Start()
    {
        if (hideOnStart)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        UpdateDisplay();
    }

    public void UnlockCycles()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
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
        number.text = cycle.ToString();
    }

}
