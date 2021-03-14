using System;
using UnityEngine;
using UnityEditor;
using TerminalEden.Simulation;

[CustomEditor(typeof(WildfireSimulation))]
public class WildfireSimulationEditor : Editor
{
    WildfireSimulation _sim;

    SerializedProperty onSimulationTextureCreated;
    SerializedProperty onCellsSelected;
    SerializedProperty onVisibleCellsChanged;

    void OnEnable()
    {
        _sim = (WildfireSimulation)target;

        onSimulationTextureCreated = serializedObject.FindProperty("onSimulationTextureCreated");
        onCellsSelected = serializedObject.FindProperty("onCellsSelected");
        onVisibleCellsChanged = serializedObject.FindProperty("onVisibleCellsChanged");
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

        // _sim.displayRenderer = (MeshRenderer)EditorGUILayout.ObjectField("Display Renderer", _sim.displayRenderer, typeof(MeshRenderer), true);
        EditorGUILayout.Space(10);

        #region Realtime Controls

        GUILayout.Label("Realtime Controls", headerStyle);

        // Pause Toggle
        bool pauseToggle = _sim.updateOverTime;
        _sim.updateOverTime = !EditorGUILayout.Toggle("Pause", !_sim.updateOverTime);
        if (EditorApplication.isPlaying)
        {
            if (pauseToggle == false && _sim.updateOverTime == true) _sim.ExecuteSimulation();
            if (pauseToggle == true && _sim.updateOverTime == false) _sim.TerminateSimulation();
        }

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

        #endregion

        #region Simulation Settings

        GUILayout.Label("Simulation Settings", headerStyle);

        // Simulation Shader and Render Texture
        _sim.simMat = (Material)EditorGUILayout.ObjectField("Simulation Material", _sim.simMat, typeof(Material), true);

        int visibleCellsWidth = _sim.visibleCellsWidth;
        visibleCellsWidth = (int)Mathf.Round(EditorGUILayout.IntSlider("Revealed Cells Width", _sim.visibleCellsWidth, 0, _sim.textureResolution) / 2) * 2;
        if (visibleCellsWidth != _sim.visibleCellsWidth)
            _sim.ChangeCellWidth(visibleCellsWidth);

        EditorGUILayout.Space(10);

        #endregion

        #region Simulation Settings

        GUILayout.Label("Simulation Setup", headerStyle);
        GUI.enabled = EditorApplication.isPlaying ? false : true;

        // Texture Resolution
        _sim.textureResolution = EditorGUILayout.IntField("Texture Resolution", _sim.textureResolution);

        // Undo History
        _sim.simStates = EditorGUILayout.IntSlider("Undo History Limit", _sim.simStates - 1, 1, 50) + 1;

        // Generate or Read from File
        EditorGUILayout.Space();
        _sim.generateStart = EditorGUILayout.Toggle("Generate Start", _sim.generateStart);
        if (_sim.generateStart)
        {
            // Generation Settings
            _sim.initMat = (Material)EditorGUILayout.ObjectField("Generation Material", _sim.initMat, typeof(Material), true);
        }
        else
        {
            // Initial State Texture
            _sim.initialState = (Texture2D)EditorGUILayout.ObjectField("Initial State", _sim.initialState, typeof(Texture2D), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }

        GUI.enabled = true;
        EditorGUILayout.Space(10);

        #endregion

        #region Abilities

        GUILayout.Label("Abilities", headerStyle);

        // Ability Mats
        // _sim.growMat = (Material)EditorGUILayout.ObjectField("Grow Ability Mat", _sim.growMat, typeof(Material), true);
        _sim.heldAbility = (Ability)EditorGUILayout.ObjectField("Held Ability", _sim.heldAbility, typeof(Ability), true);

        EditorGUILayout.Space(10);

        #endregion

        #region Events

        GUILayout.Label("Events", headerStyle);

        EditorGUILayout.PropertyField(onSimulationTextureCreated);
        EditorGUILayout.PropertyField(onCellsSelected);
        EditorGUILayout.PropertyField(onVisibleCellsChanged);

        #endregion

        serializedObject.ApplyModifiedProperties();
    }
}