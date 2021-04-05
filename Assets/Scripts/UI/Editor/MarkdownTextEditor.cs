using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MarkdownText))]
public class MarkdownTextEditor : Editor
{
    MarkdownText _md;

    void OnEnable()
    {
        _md = (MarkdownText)target;
    }

    public override void OnInspectorGUI()
    {

        _md.textFile = (TextAsset)EditorGUILayout.ObjectField("Markdown File", _md.textFile, typeof(TextAsset), false);

        if (_md.textFile != null)
        {
            if (_md.textFile.text != _md.text)
            {
                _md.CreateTextObjects();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please insert Markdown File", MessageType.Warning);
        }
    }
}
