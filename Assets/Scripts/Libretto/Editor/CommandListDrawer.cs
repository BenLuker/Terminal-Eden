using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Libretto;

[CustomPropertyDrawer(typeof(CommandsList))]
public class CommandListDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Serialize commands list
        SerializedProperty commands = property.FindPropertyRelative("commands");

        // Create GUI Styles
        GUIStyle labelStyle = new GUIStyle();

        // Draw label name
        labelStyle.fontSize = 14;
        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, labelStyle);

        // Draw label type
        float labelTypeSpace = labelStyle.CalcSize(label).x + 25;
        labelStyle.fontSize = 12;
        labelStyle.fontStyle = FontStyle.Bold;
        EditorGUI.LabelField(new Rect(labelTypeSpace, position.y + 2, 100, position.height), property.FindPropertyRelative("type").stringValue, labelStyle);

        // Grab current index level and add to it
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        // Draw commands
        for (int i = 0; i < commands.arraySize; i++)
        {
            // Create GUI Styles
            GUIStyle keywordStyle = new GUIStyle();
            keywordStyle.fontStyle = FontStyle.Bold;

            // Calculate rects
            var keywordRect = new Rect(20, position.y + 5 + ((i + 1) * EditorGUIUtility.singleLineHeight), 100, position.height);
            var argumentsRect = new Rect(120, position.y + 5 + ((i + 1) * EditorGUIUtility.singleLineHeight), position.width - 35, position.height);

            // Draw labels
            EditorGUI.LabelField(keywordRect, commands.GetArrayElementAtIndex(i).FindPropertyRelative("keyword").FindPropertyRelative("name").stringValue, keywordStyle);
            EditorGUI.LabelField(argumentsRect, commands.GetArrayElementAtIndex(i).FindPropertyRelative("arguments").stringValue, GUIStyle.none);
        }

        // Set the index level back
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty commands = property.FindPropertyRelative("commands");
        return EditorGUIUtility.singleLineHeight * (commands.arraySize + 1) + 5;
    }
}
