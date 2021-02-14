using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    public static bool IsInitialized
    {
        get
        {
            return instance != null;
        }
    }

    public virtual void Awake()
    {
        if (instance != null)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot have more than one instance of the SingletonBehaviour {0}", typeof(T).Name));
        }

        instance = (T)this;
    }

    private static T instance;
}