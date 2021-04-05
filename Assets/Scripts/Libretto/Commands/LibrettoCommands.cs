using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Libretto;

public class LibrettoCommands<T> : MonoBehaviour
{
    protected T subject;
    protected UnityEvent onCommandComplete = new UnityEvent();

    public virtual void OnEnable()
    {
        subject = GetComponent<T>();
        if (subject == null)
            Debug.LogErrorFormat("Could not find {0} component on {1}", typeof(T).Name, name);
    }

    private void Start()
    {
        onCommandComplete.RemoveAllListeners();
        onCommandComplete.AddListener(LevelLibretto.Instance.CompleteCommand);
    }
}