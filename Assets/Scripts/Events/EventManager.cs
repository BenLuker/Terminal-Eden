using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TerminalEden.Simulation;

[System.Serializable]
public class EdenEventsArray
{
    public EdenEvent[] edenEvents;
}

public class EventManager : SingletonBehaviour<EventManager>
{
    public GameObject eventPanel;
    public TerrainManager terrainAssetsManager;
    // public EdenEvent heldEvent;

    public TextMeshProUGUI title;
    public TextMeshProUGUI copy;

    public EdenEventsArray[] edenEventsPerPhase = new EdenEventsArray[5];
    public float[] eventPhaseSpawnChance;
    public int gracePeriod;
    public int latePeriod;
    int cyclesSinceLastEvent;
    bool inputBlocked;

    public List<Vector2Int> coordsList = new List<Vector2Int>();

    public void EventCheck()
    {
        if (GameManager.Instance.act > 0) { cyclesSinceLastEvent++; }
        if ((Random.value < eventPhaseSpawnChance[GameManager.Instance.act] && cyclesSinceLastEvent > gracePeriod) || (cyclesSinceLastEvent >= latePeriod && GameManager.Instance.act > 0))
        {
            OpenEvent();
            cyclesSinceLastEvent = 0;
        }
    }

    public void OpenEvent()
    {
        EdenEvent newEvent = edenEventsPerPhase[GameManager.Instance.act].edenEvents[Random.Range(0, edenEventsPerPhase[GameManager.Instance.act].edenEvents.Length)];
        WildfireSimulation.Instance.PlayPauseSimulation(false);
        inputBlocked = TopDownCameraController.Instance.inputBlocked;
        TopDownCameraController.Instance.SetInputBlock(true);
        WildfireSimulation.Instance.CastEvent(newEvent);
        // SetupAssets(newEvent.setupState0);
        SetupAssets(newEvent.setupState);
        StartCoroutine(PlayAssets(newEvent.affectState));
        title.text = newEvent.title;
        copy.text = newEvent.copy + " Click to continue...";
        eventPanel.SetActive(true);
    }

    public void CloseEvent()
    {
        // WildfireSimulation.Instance.PlayPauseSimulation(true);
        TopDownCameraController.Instance.SetInputBlock(inputBlocked);
        eventPanel.SetActive(false);
    }

    void SetupAssets(int state)
    {
        terrainAssetsManager.AffectAllCells(state);
    }

    IEnumerator PlayAssets(int state)
    {
        yield return new WaitForSeconds(1.5f);
        Vector2Int coords = coordsList[Random.Range(0, coordsList.Count)];
        terrainAssetsManager.AffectOneCell(coords.x, coords.y, state);
    }
}
