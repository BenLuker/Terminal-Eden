using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkdownText : MonoBehaviour
{

    public enum MarkdownElementType { Header, Subtitle, Text, Image, Space };

    public class MarkdownElement
    {
        public MarkdownElementType type;
        public string content;
    }

    public TextAsset textFile;
    public string text;
    public MarkdownElementType lastElement;

    void Start()
    {

    }

    void Update()
    {

    }

    public void CreateTextObjects()
    {
        // Split text file by line 
        string[] lines = text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            // Trim all whitespace
            Debug.Log(lines[i].Trim());
        }
    }
}
