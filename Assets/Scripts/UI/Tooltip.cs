using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class Tooltip : SingletonBehaviour<Tooltip>
{
    Canvas canvas;

    public GameObject tooltip;
    RectTransform rect;
    VerticalLayoutGroup layoutGroup;

    TextMeshProUGUI header;
    TextMeshProUGUI content;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        rect = tooltip.GetComponent<RectTransform>();
        layoutGroup = tooltip.GetComponent<VerticalLayoutGroup>();
        header = tooltip.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        content = tooltip.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(canvas.scaleFactor);
    }

    #region Public Methods

    public void Show(string h = null, string c = null)
    {
        if (String.IsNullOrEmpty(h) && String.IsNullOrEmpty(c)) return;
        tooltip.SetActive(true);

        header.text = h;
        content.text = c;

        header.gameObject.SetActive(true);
        content.gameObject.SetActive(true);
    }

    public void Hide()
    {
        tooltip.SetActive(false);
    }

    public void AlignTooltip(RectTransform origin, TooltipDirection direction, TooltipAlignment alignment, Vector2 offset)
    {
        rect.position = origin.position;
        Vector2 pivotOffset = (new Vector2(0.5f, 0.5f) - origin.pivot) * origin.sizeDelta * canvas.scaleFactor;
        rect.position += new Vector3(pivotOffset.x, pivotOffset.y, 0);

        switch (direction)
        {
            case TooltipDirection.Up:
                switch (alignment)
                {
                    case TooltipAlignment.Centered:
                        rect.position += new Vector3(offset.x, origin.sizeDelta.y / 2 + offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(0.5f, 0);
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                        header.alignment = TextAlignmentOptions.Center;
                        content.alignment = TextAlignmentOptions.Center;
                        break;
                    case TooltipAlignment.Left:
                        rect.position += new Vector3(-origin.sizeDelta.x / 2 + offset.x, origin.sizeDelta.y / 2 + offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(0, 0);
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                        header.alignment = TextAlignmentOptions.Left;
                        content.alignment = TextAlignmentOptions.Left;
                        break;
                    case TooltipAlignment.Right:
                        rect.position += new Vector3(origin.sizeDelta.x / 2 + offset.x, origin.sizeDelta.y / 2 + offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(1, 0);
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                        header.alignment = TextAlignmentOptions.Right;
                        content.alignment = TextAlignmentOptions.Right;
                        break;
                    case TooltipAlignment.Up:
                        Debug.LogError("Cannot have direction and alignment set to up");
                        break;
                    case TooltipAlignment.Down:
                        Debug.LogError("Cannot have direction set to up and alignment set to down");
                        break;
                }
                break;
            case TooltipDirection.Down:
                switch (alignment)
                {
                    case TooltipAlignment.Centered:
                        rect.position += new Vector3(offset.x, -origin.sizeDelta.y / 2 - offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(0.5f, 1);
                        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
                        header.alignment = TextAlignmentOptions.Center;
                        content.alignment = TextAlignmentOptions.Center;
                        break;
                    case TooltipAlignment.Left:
                        rect.position += new Vector3(-origin.sizeDelta.x / 2 + offset.x, -origin.sizeDelta.y / 2 - offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(0, 1);
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                        header.alignment = TextAlignmentOptions.Left;
                        content.alignment = TextAlignmentOptions.Left;
                        break;
                    case TooltipAlignment.Right:
                        rect.position += new Vector3(origin.sizeDelta.x / 2 + offset.x, -origin.sizeDelta.y / 2 - offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(1, 1);
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                        header.alignment = TextAlignmentOptions.Right;
                        content.alignment = TextAlignmentOptions.Right;
                        break;
                    case TooltipAlignment.Up:
                        Debug.LogError("Cannot have direction set to down and alignment set to up");
                        break;
                    case TooltipAlignment.Down:
                        Debug.LogError("Cannot have direction and alignment set to down");
                        break;
                }
                break;
            case TooltipDirection.Left:
                switch (alignment)
                {
                    case TooltipAlignment.Centered:
                        rect.position += new Vector3(-origin.sizeDelta.x / 2 - offset.x, offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(1, 0.5f);
                        layoutGroup.childAlignment = TextAnchor.MiddleRight;
                        header.alignment = TextAlignmentOptions.Right;
                        content.alignment = TextAlignmentOptions.Right;
                        break;
                    case TooltipAlignment.Up:
                        rect.position += new Vector3(-origin.sizeDelta.x / 2 - offset.x, origin.sizeDelta.y / 2 + offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(1, 1);
                        layoutGroup.childAlignment = TextAnchor.UpperRight;
                        header.alignment = TextAlignmentOptions.Right;
                        content.alignment = TextAlignmentOptions.Right;
                        break;
                    case TooltipAlignment.Down:
                        rect.position += new Vector3(-origin.sizeDelta.x / 2 - offset.x, -origin.sizeDelta.y / 2 + offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(1, 0);
                        layoutGroup.childAlignment = TextAnchor.LowerRight;
                        header.alignment = TextAlignmentOptions.Right;
                        content.alignment = TextAlignmentOptions.Right;
                        break;
                    case TooltipAlignment.Left:
                        Debug.LogError("Cannot have direction and alignment set to left");
                        break;
                    case TooltipAlignment.Right:
                        Debug.LogError("Cannot have direction set to left and alignment set to right");
                        break;
                }
                break;
            case TooltipDirection.Right:
                switch (alignment)
                {
                    case TooltipAlignment.Centered:
                        rect.position += new Vector3(origin.sizeDelta.x / 2 + offset.x, offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(0, 0.5f);
                        layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                        header.alignment = TextAlignmentOptions.Left;
                        content.alignment = TextAlignmentOptions.Left;
                        break;
                    case TooltipAlignment.Up:
                        rect.position += new Vector3(origin.sizeDelta.x / 2 + offset.x, origin.sizeDelta.y / 2 + offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(0, 1);
                        layoutGroup.childAlignment = TextAnchor.UpperLeft;
                        header.alignment = TextAlignmentOptions.Left;
                        content.alignment = TextAlignmentOptions.Left;
                        break;
                    case TooltipAlignment.Down:
                        rect.position += new Vector3(origin.sizeDelta.x / 2 + offset.x, -origin.sizeDelta.y / 2 + offset.y, 0) * canvas.scaleFactor;
                        rect.pivot = new Vector2(0, 0);
                        layoutGroup.childAlignment = TextAnchor.LowerLeft;
                        header.alignment = TextAlignmentOptions.Left;
                        content.alignment = TextAlignmentOptions.Left;
                        break;
                    case TooltipAlignment.Left:
                        Debug.LogError("Cannot have direction set to right and alignment set to left");
                        break;
                    case TooltipAlignment.Right:
                        Debug.LogError("Cannot have direction and alignment set to right");
                        break;
                }
                break;
        }
    }

    #endregion

}
