using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TerminalEden.Terrain;

public class DataVisualizationButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Material dataVisualizationMat;

    GameObject parent;
    float pos;

    // private void Awake()
    // {
    //     pos = GetComponent<RectTransform>().anchoredPosition.x;
    //     gameObject.SetActive(false);
    // }

    private void OnEnable()
    {
        // // Get RectTransform and the Current Position
        // RectTransform trans = GetComponent<RectTransform>();

        // // Get Parent Position
        // Vector2 parentPos = transform.parent.GetComponent<RectTransform>().anchoredPosition;

        // // Free from HorizontalLayoutGroup
        // transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;

        // // Set position to offscreen
        // trans.anchoredPosition = new Vector2(-parentPos.x - 10, 0);

        // // Tween from left side of screen to current position
        // LeanTween.moveLocalX(this.gameObject, pos, 2);

        // // LeanTween.move(gameObject, position, seconds).setEaseInOutQuad();
        // // Tween from 0 alpha to 1 alpha
        // LeanTween.alpha(GetComponent<RectTransform>(), 1f, 1f).setEase(LeanTweenType.easeOutCubic);
    }

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
