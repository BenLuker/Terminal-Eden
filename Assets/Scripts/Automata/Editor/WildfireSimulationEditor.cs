using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WildfireSimulation))]
public class WildfireSimulationEditor : Editor
{
    WildfireSimulation _sim;

    // Texture resolution
    string[] resolutionOptions = new string[] { "16 x 16", "32 x 32", "64 x 64", "128 x 128", "256 x 256", "512 x 512", "1024 x 1024", "2048 x 2048", "4096 x 4096" };
    int resolutionIndex;

    void OnEnable()
    {
        _sim = (WildfireSimulation)target;

        // Texture resolution
        resolutionIndex = (int)Mathf.Round((-4 + (Mathf.Log(_sim.textureResolution) / Mathf.Log(2)))); // Resolution to index number
    }

    public override void OnInspectorGUI()
    {
        // Texture resolution
        resolutionIndex = EditorGUILayout.Popup("Texture Resolution", resolutionIndex, resolutionOptions);
        _sim.textureResolution = (int)Mathf.Round((16 * Mathf.Pow(2, resolutionIndex))); // Index to resolution
    }
}