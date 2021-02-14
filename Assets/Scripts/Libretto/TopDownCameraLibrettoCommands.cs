using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TopDownCameraController))]
public class TopDownCameraLibrettoCommands : MonoBehaviour
{
    public UnityEvent commandComplete;

    #region Methods for Libretto

    #region Settings

    // No Arguments
    public void BlockInput(string arguments)
    {
        TopDownCameraController.Instance.SetInputBlock(true);
        commandComplete.Invoke();
    }

    // No Arguments
    public void UnblockInput(string arguments)
    {
        TopDownCameraController.Instance.SetInputBlock(false);
        commandComplete.Invoke();
    }

    // No Arguments
    public void BlockOutput(string arguments)
    {
        TopDownCameraController.Instance.SetBlockOutput(true);
        commandComplete.Invoke();
    }

    // No Arguments
    public void UnblockOutput(string arguments)
    {
        TopDownCameraController.Instance.SetBlockOutput(false);
        commandComplete.Invoke();
    }

    // Command speed
    public void SetCameraSmoothingSpeed(string arguments)
    {
        float speed = float.Parse(arguments);
        TopDownCameraController.Instance.SetCameraSmoothingSpeed(speed);
        commandComplete.Invoke();
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

        TopDownCameraController.Instance.SetPosition(new Vector3(x, y, z), smooth);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.SetPositionRelative(new Vector3(x, y, z), smooth);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.SetZoom(-distance, smooth);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.TweenPosition(new Vector3(x, y, z), time, ease);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.TweenPositionRelative(new Vector3(x, y, z), time, ease);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.TweenXRotation(angle, time, ease);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.TweenYRotation(angle, time, ease);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.TweenZoom(-distance, seconds, ease);

        if (noWait)
        {
            commandComplete.Invoke();
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

        TopDownCameraController.Instance.StartTurntable(speed);
        commandComplete.Invoke();
    }

    // No Arguments
    public void StopTurntable(string arguments)
    {
        TopDownCameraController.Instance.StopTurntable();
        commandComplete.Invoke();
    }

    #endregion

    #endregion

    #region Helper Functions

    IEnumerator ReturnMoveCompleted()
    {
        while (TopDownCameraController.Instance.isMoving()) yield return null;
        commandComplete.Invoke();
    }

    IEnumerator ReturnRotateXCompleted()
    {
        while (TopDownCameraController.Instance.isRotatingX()) yield return null;
        commandComplete.Invoke();
    }

    IEnumerator ReturnRotateYCompleted()
    {
        while (TopDownCameraController.Instance.isRotatingY()) yield return null;
        commandComplete.Invoke();
    }

    IEnumerator ReturnZoomCompleted()
    {
        while (TopDownCameraController.Instance.isZooming()) yield return null;
        commandComplete.Invoke();
    }

    #endregion
}
