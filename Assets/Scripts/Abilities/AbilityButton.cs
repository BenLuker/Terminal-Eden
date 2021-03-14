using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TerminalEden.Simulation;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public Ability ability;

    [System.Serializable] public class AbilityEvent : UnityEvent<Ability> { }
    public AbilityEvent onAbilitySelected = new AbilityEvent();

    public void OnPointerEnter(PointerEventData pointerEventData)
    {

    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onAbilitySelected.Invoke(ability);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {

    }
}
