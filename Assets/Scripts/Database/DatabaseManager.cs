using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
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

    public void UnlockDatabase()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Open(bool open)
    {
        transform.GetChild(1).gameObject.SetActive(open);
    }
}