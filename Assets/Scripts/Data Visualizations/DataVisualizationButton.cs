using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TerminalEden.Terrain;

public class DataVisualizationButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Material dataVisualizationMat;

    [System.Serializable] public class MaterialEvent : UnityEvent<Material> { }
    public MaterialEvent onVisualizationSelected = new MaterialEvent();

    public void OnPointerEnter(PointerEventData pointerEventData)
    {

    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onVisualizationSelected.Invoke(dataVisualizationMat);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {

    }
}
