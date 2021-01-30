using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WildfireSimulation))]
public class WildfireSimulationEditor : Editor
{
    WildfireSimulation _sim;

    // Texture resolution
    // string[] resolutionOptions = new string[] { "16 x 16", "32 x 32", "64 x 64", "128 x 128", "256 x 256", "512 x 512", "1024 x 1024", "2048 x 2048", "4096 x 4096" };
    // int resolutionIndex;

    void OnEnable()
    {
        _sim = (WildfireSimulation)target;

        // Texture resolution
        // resolutionIndex = (int)Mathf.Round((-4 + (Mathf.Log(_sim.textureResolution) / Mathf.Log(2)))); // Convert resolution size to an index option number
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        // Monobehavior Header
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((WildfireSimulation)target), typeof(WildfireSimulation), false);
        EditorGUILayout.Space();
        GUI.enabled = true;

        // Create styles
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;

        // Realtime Controls
        GUILayout.Label("Realtime Controls", headerStyle);

        // Pause Toggle
        bool pauseToggle = _sim.updateOverTime;
        _sim.updateOverTime = !EditorGUILayout.Toggle("Pause", !_sim.updateOverTime);
        if (pauseToggle == false && _sim.updateOverTime == true) _sim.ExecuteSimulation();
        if (pauseToggle == true && _sim.updateOverTime == false) _sim.TerminateSimulation();

        // Refresh Rate
        _sim.refreshRate = EditorGUILayout.IntSlider("Steps per second", _sim.refreshRate, 1, 60);

        // Calculate Step and Undo buttons
        GUI.enabled = EditorApplication.isPlaying && !_sim.updateOverTime ? true : false;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Calculate Step")) _sim.CalculateStep();
        GUI.enabled = EditorApplication.isPlaying && !_sim.updateOverTime && _sim.historyTracker > 0 ? true : false;
        if (GUILayout.Button(String.Format("Undo ({0})", _sim.historyTracker))) _sim.UndoStep();
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
        EditorGUILayout.Space(10);

        #region Simulation Shader Settings

        // Simulation Shader Settings
        GUILayout.Label("Simulation Shader Settings", headerStyle);

        // Simulation Shader and Render Texture
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Shader", _sim.simShader, typeof(Shader), true);
        EditorGUILayout.Space();
        GUI.enabled = EditorApplication.isPlaying ? false : true;

        // Initial State Texture
        _sim.initialState = (Texture2D)EditorGUILayout.ObjectField("Initial State", _sim.initialState, typeof(Texture2D), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));

        // Texture Resolution
        _sim.textureResolution = EditorGUILayout.IntField("Texture Resolution", _sim.textureResolution);

        // Undo History
        _sim.simStates = EditorGUILayout.IntSlider("Undo History Limit", _sim.simStates - 1, 1, 50) + 1;

        GUI.enabled = true;

        #endregion
    }
}