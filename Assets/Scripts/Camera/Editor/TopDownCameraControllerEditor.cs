using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TopDownCameraController))]
public class TopDownCameraControllerEditor : Editor
{
    TopDownCameraController _cam;
    SerializedProperty onMouseClick;
    SerializedProperty onCameraMove;

    void OnEnable()
    {
        _cam = (TopDownCameraController)target;
        onCameraMove = serializedObject.FindProperty("onCameraMove");
        onMouseClick = serializedObject.FindProperty("onMouseClick");
    }

    public override void OnInspectorGUI()
    {
        // Monobehavior Header
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((TopDownCameraController)target), typeof(TopDownCameraController), false);
        EditorGUILayout.Space();

        // Transforms
        EditorGUILayout.ObjectField("Camera", _cam.cam, typeof(Camera), true);
        EditorGUILayout.ObjectField("Camera Transform", _cam.cameraTransform, typeof(Transform), true);
        EditorGUILayout.ObjectField("Crane Transform", _cam.craneTransform, typeof(Transform), true);
        EditorGUILayout.Space();
        GUI.enabled = true;

        // Create styles
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;

        // Input/Output Blocking
        _cam.inputBlocked = EditorGUILayout.Toggle("Input Block", _cam.inputBlocked);
        _cam.outputBlocked = EditorGUILayout.Toggle("Output Block", _cam.outputBlocked);

        // FOV slider
        _cam.cameraFOV = EditorGUILayout.Slider("Field of View", _cam.cameraFOV, 0f, 100f);

        // Smoothing slider
        _cam.cameraSmoothingSpeed = EditorGUILayout.Slider("Camera Smoothing Speed", _cam.cameraSmoothingSpeed, 0.1f, 20f);
        EditorGUILayout.Space();

        // Camera Limits (Angle and Zoom)
        GUILayout.Label("Camera Limits", headerStyle);

        _cam.maxPos = EditorGUILayout.Slider("Maximum Distance", _cam.maxPos, 1f, 20f);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Camera Angle", GUILayout.Width(125));
        _cam.craneAngleMin = EditorGUILayout.FloatField(_cam.craneAngleMin, GUILayout.Width(50));
        EditorGUILayout.MinMaxSlider(ref _cam.craneAngleMin, ref _cam.craneAngleMax, 0, 90);
        _cam.craneAngleMax = EditorGUILayout.FloatField(_cam.craneAngleMax, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Camera Distance", GUILayout.Width(125));
        _cam.cameraDistanceMin = EditorGUILayout.FloatField(_cam.cameraDistanceMin, GUILayout.Width(50));
        EditorGUILayout.MinMaxSlider(ref _cam.cameraDistanceMin, ref _cam.cameraDistanceMax, 0, 200);
        _cam.cameraDistanceMax = EditorGUILayout.FloatField(_cam.cameraDistanceMax, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Mouse Inputs
        GUILayout.Label("Mouse Input", headerStyle);
        _cam.mouseRotationSpeed = EditorGUILayout.Slider("Rotation Speed", _cam.mouseRotationSpeed, 0.05f, 0.5f);
        _cam.mouseZoomSpeed = EditorGUILayout.Slider("Zoom Speed", _cam.mouseZoomSpeed, 0.1f, 10f);
        EditorGUILayout.Space();

        // Keyboard Inputs
        GUILayout.Label("Keyboard Input", headerStyle);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Movement Speed", GUILayout.Width(125));
        _cam.keyboardMovementSpeedSlow = EditorGUILayout.FloatField(_cam.keyboardMovementSpeedSlow, GUILayout.Width(50));
        EditorGUILayout.MinMaxSlider(ref _cam.keyboardMovementSpeedSlow, ref _cam.keyboardMovementSpeedFast, 0.01f, 0.5f);
        _cam.keyboardMovementSpeedFast = EditorGUILayout.FloatField(_cam.keyboardMovementSpeedFast, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Rotation Speed", GUILayout.Width(125));
        _cam.keyboardRotationSpeedSlow = EditorGUILayout.FloatField(_cam.keyboardRotationSpeedSlow, GUILayout.Width(50));
        EditorGUILayout.MinMaxSlider(ref _cam.keyboardRotationSpeedSlow, ref _cam.keyboardRotationSpeedFast, 0.1f, 1f);
        _cam.keyboardRotationSpeedFast = EditorGUILayout.FloatField(_cam.keyboardRotationSpeedFast, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Zoom Speed", GUILayout.Width(125));
        _cam.keyboardZoomSpeedSlow = EditorGUILayout.FloatField(_cam.keyboardZoomSpeedSlow, GUILayout.Width(50));
        EditorGUILayout.MinMaxSlider(ref _cam.keyboardZoomSpeedSlow, ref _cam.keyboardZoomSpeedFast, 0.01f, 5f);
        _cam.keyboardZoomSpeedFast = EditorGUILayout.FloatField(_cam.keyboardZoomSpeedFast, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(onCameraMove);
        EditorGUILayout.PropertyField(onMouseClick);

        serializedObject.ApplyModifiedProperties();
    }
}
