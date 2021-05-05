using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TerminalEden.Simulation;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Ability ability;

    Image image;
    Button button;
    float cooldown;

    public float highlightFadeSpeed = 100;
    public float selectFadeSpeed = 10;
    float highlightFade;
    float selectFade;
    float targetHighlight;
    float targetSelect;


    [System.Serializable] public class AbilityEvent : UnityEvent<Ability> { }
    public AbilityEvent onAbilitySelected = new AbilityEvent();

    private void OnEnable()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        Material mat = Instantiate(image.material);
        image.material = mat;
        image.material.SetFloat("_Cooldown", 0f);
    }

    private void Update()
    {
        highlightFade = Vector2.Lerp(new Vector2(highlightFade, 0), new Vector2(targetHighlight, 0), Time.deltaTime * highlightFadeSpeed).x;
        selectFade = Vector2.Lerp(new Vector2(selectFade, 0), new Vector2(targetSelect, 0), Time.deltaTime * selectFadeSpeed).x;
        image.material.SetFloat("_HighlightFade", highlightFade);
        image.material.SetFloat("_SelectFade", selectFade);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        targetHighlight = 1;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        targetHighlight = 0;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        targetSelect = 1;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        targetSelect = 0;
    }

    public void SelectAbility()
    {
        onAbilitySelected.Invoke(ability);
    }

    public void AbilityUsed(Ability a)
    {
        if (a == ability)
        {
            button.interactable = false;
            cooldown = ability.coolDown;
            image.material.SetFloat("_Cooldown", 1f);
        }
    }

    public void CoolDown()
    {
        if (gameObject.activeSelf)
        {
            cooldown--;
            if (cooldown <= 0)
            {
                button.interactable = true;
                image.material.SetFloat("_Cooldown", 0f);
            }
            else
            {
                image.material.SetFloat("_Cooldown", (float)cooldown / (float)ability.coolDown);
            }
        }
    }
}
