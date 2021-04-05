using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class AbilitiesManager : SingletonBehaviour<AbilitiesManager>
{
    public bool hideOnStart;

    private void Start()
    {
        if (hideOnStart)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void UnlockAbility(int child)
    {
        transform.GetChild(child).gameObject.SetActive(true);
    }
}
