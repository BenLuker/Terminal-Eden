using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public bool unlockAtStart;
    public GameObject DBButton;
    public GameObject DBPanel;
    public GameObject DBBackgroundPanel;

    private void Start()
    {
        if (!unlockAtStart)
        {
            DBButton.SetActive(false);
            DBBackgroundPanel.SetActive(false);
        }
    }

    public void UnlockDatabase()
    {
        DBButton.SetActive(true);
    }

    public void Open(bool open)
    {
        DBBackgroundPanel.SetActive(open);
    }

    public void UnlockTab(int tab)
    {
        DBPanel.transform.GetChild(tab * 2 + 1).gameObject.SetActive(true);
    }
}