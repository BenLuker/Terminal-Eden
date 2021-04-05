using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerminalEden.Simulation;

public class EventManager : MonoBehaviour
{
    public GameObject eventPanel;
    public EdenEvent heldEvent;

    public void EventCheck()
    {
        if (GameManager.Instance.act == 1 && CyclesManager.Instance.cycle == 100)
        {
            OpenEvent();
        }
    }

    public void OpenEvent()
    {
        WildfireSimulation.Instance.PlayPauseSimulation(false);
        eventPanel.SetActive(true);
        WildfireSimulation.Instance.CastEvent(heldEvent);
    }

    public void CloseEvent()
    {
        WildfireSimulation.Instance.PlayPauseSimulation(true);
        eventPanel.SetActive(false);
    }
}
