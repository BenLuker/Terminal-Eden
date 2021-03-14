using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TooltipDirection { Up, Down, Left, Right };
public enum TooltipAlignment { Centered, Up, Down, Left, Right };

public class TooltipHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rect;

    public string header;
    public string content;

    public TooltipDirection direction;
    public TooltipAlignment alignment;

    public float offset = 10;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Tooltip.Instance.AlignTooltip(rect, direction, alignment, offset);
        Tooltip.Instance.Show(header, content);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Tooltip.Instance.Hide();
    }
}
