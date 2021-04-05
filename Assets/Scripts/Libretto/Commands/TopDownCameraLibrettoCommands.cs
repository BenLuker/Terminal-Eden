using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TopDownCameraController))]
public class TopDownCameraLibrettoCommands : LibrettoCommands<TopDownCameraController>
{

    #region Settings

    // No Arguments
    public void BlockInput(string arguments)
    {
        subject.SetInputBlock(true);
        onCommandComplete.Invoke();
    }

    // No Arguments
    public void UnblockInput(string arguments)
    {
        subject.SetInputBlock(false);
        onCommandComplete.Invoke();
    }

    // No Arguments
    public void BlockOutput(string arguments)
    {
        subject.SetBlockOutput(true);
        onCommandComplete.Invoke();
    }

    // No Arguments
    public void UnblockOutput(string arguments)
    {
        subject.SetBlockOutput(false);
        onCommandComplete.Invoke();
    }

    // Command speed
    public void SetCameraSmoothingSpeed(string arguments)
    {
        float speed = float.Parse(arguments);
        subject.SetCameraSmoothingSpeed(speed);
        onCommandComplete.Invoke();
    }

    #endregion

    #region Set Transforms

    // Command x y z smooth(keyword) nowait(keyword)
    public void SetPosition(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        float x = float.Parse(a[0]);
        float y = float.Parse(a[1]);
        float z = float.Parse(a[2]);

        // Optional Keywords
        bool smooth = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.SetPosition(new Vector3(x, y, z), smooth);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnMoveCompleted());
        }
    }

    // Command x y z smooth(keyword) nowait(keyword)
    public void SetPositionRelative(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        float x = float.Parse(a[0]);
        float y = float.Parse(a[1]);
        float z = float.Parse(a[2]);

        // Optional Keywords
        bool smooth = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.SetPositionRelative(new Vector3(x, y, z), smooth);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnMoveCompleted());
        }
    }

    // Command distance smooth(keyword) nowait(keyword)
    public void SetZoom(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        float distance = float.Parse(a[0]);

        // Optional Keywords
        bool smooth = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.SetZoom(-distance, smooth);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnZoomCompleted());
        }
    }

    #endregion

    #region Tween Transforms

    // Command x y z time(seconds) smooth(keyword) nowait(keyword)
    public void TweenPosition(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        float x = float.Parse(a[0]);
        float y = float.Parse(a[1]);
        float z = float.Parse(a[2]);
        float time = float.Parse(a[3]);

        // Optional Keywords
        bool ease = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.TweenPosition(new Vector3(x, y, z), time, ease);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnMoveCompleted());
        }
    }

    // Command x y z time(seconds) smooth(keyword) nowait(keyword)
    public void TweenPositionRelative(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        float x = float.Parse(a[0]);
        float y = float.Parse(a[1]);
        float z = float.Parse(a[2]);
        float time = float.Parse(a[3]);

        // Optional Keywords
        bool ease = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.TweenPositionRelative(new Vector3(x, y, z), time, ease);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnMoveCompleted());
        }
    }

    // Command angle time(seconds) smooth(keyword) nowait(keyword)
    public void TweenRotationX(string arguments)
    {

        string[] a = arguments.Split(' ');

        // Required Arguments
        float angle = float.Parse(a[0]);
        float time = float.Parse(a[1]);

        // Optional Keywords
        bool ease = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.TweenXRotation(angle, time, ease);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnRotateXCompleted());
        }
    }

    // Command angle time(seconds) smooth(keyword) nowait(keyword)
    public void TweenRotationY(string arguments)
    {

        string[] a = arguments.Split(' ');

        // Required Arguments
        float angle = float.Parse(a[0]);
        float time = float.Parse(a[1]);

        // Optional Keywords
        bool ease = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.TweenYRotation(angle, time, ease);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnRotateYCompleted());
        }
    }

    // Command distance time(seconds) smooth(keyword) nowait(keyword)
    public void TweenZoom(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        float distance = float.Parse(a[0]);
        float seconds = float.Parse(a[1]);

        // Optional Keywords
        bool ease = System.Array.IndexOf(a, "smooth") > -1 ? true : false;
        bool noWait = System.Array.IndexOf(a, "nowait") > -1 ? true : false;

        subject.TweenZoom(-distance, seconds, ease);

        if (noWait)
        {
            onCommandComplete.Invoke();
        }
        else
        {
            StartCoroutine(ReturnZoomCompleted());
        }
    }

    #endregion

    #region Fun Features

    // Command speed
    public void StartTurntable(string arguments)
    {
        string[] a = arguments.Split(' ');

        // Required Arguments
        float speed = float.Parse(a[0]);

        subject.StartTurntable(speed);
        onCommandComplete.Invoke();
    }

    // No Arguments
    public void StopTurntable(string arguments)
    {
        subject.StopTurntable();
        onCommandComplete.Invoke();
    }

    #endregion

    #region Helper Functions

    IEnumerator ReturnMoveCompleted()
    {
        while (subject.isMoving()) yield return null;
        onCommandComplete.Invoke();
    }

    IEnumerator ReturnRotateXCompleted()
    {
        while (subject.isRotatingX()) yield return null;
        onCommandComplete.Invoke();
    }

    IEnumerator ReturnRotateYCompleted()
    {
        while (subject.isRotatingY()) yield return null;
        onCommandComplete.Invoke();
    }

    IEnumerator ReturnZoomCompleted()
    {
        while (subject.isZooming()) yield return null;
        onCommandComplete.Invoke();
    }

    #endregion
}
