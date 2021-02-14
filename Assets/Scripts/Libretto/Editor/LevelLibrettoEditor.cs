using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Libretto;

[CustomEditor(typeof(LevelLibretto))]
public class LevelLibrettoEditor : Editor
{
    LevelLibretto _libretto;
    SerializedProperty scenes;
    SerializedProperty events;

    // A command can start with any of the following characters
    private string[] commandStarters = { "  ", "   ", "    ", "     ", "\t", " \t" };

    private void OnEnable()
    {
        _libretto = (LevelLibretto)target;
        scenes = serializedObject.FindProperty("scenes");
        events = serializedObject.FindProperty("events");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle levelInfoStyle = new GUIStyle();
        levelInfoStyle.fontStyle = FontStyle.Bold;

        // The level information text file
        _libretto.levelInfo = (TextAsset)EditorGUILayout.ObjectField("Level Information Text File", _libretto.levelInfo, typeof(TextAsset), false);

        // If there is a text file, parse level and show command events and scenes. Otherwise, display warning.
        if (_libretto.levelInfo != null)
        {
            if (_libretto.levelData != _libretto.levelInfo.text)
            {
                _libretto.levelData = _libretto.levelInfo.text;
                ParseLevel();
            }

            // If there are scenes, draw commands and scenes. Otherwise, display warning
            if (_libretto.scenes.Count > 0)
            {
                if (_libretto.events.Count > 0)
                {
                    DrawCommandEvents();
                }
                else
                {
                    EditorGUILayout.HelpBox("All scenes are missing commands", MessageType.Warning);
                }

                DrawScenes();
            }
            else
            {
                EditorGUILayout.HelpBox("Could not find scenes in Level Information Text File", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please insert Level Information Text File", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();

    }

    private void DrawCommandEvents()
    {
        // Create a style for the command events
        GUIStyle commandEventsStyle = new GUIStyle();
        commandEventsStyle.fontSize = 14;
        commandEventsStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Events", commandEventsStyle);

        commandEventsStyle.fontSize = 12;
        commandEventsStyle.fontStyle = FontStyle.Normal;

        // For each keyword in each glossary, create a property field
        for (int i = 0; i < _libretto.keywords.Count; i++)
        {
            EditorGUILayout.LabelField(_libretto.keywords[i].type, commandEventsStyle);

            if (_libretto.keywords[i].keywords.Count == 0)
            {
                EditorGUILayout.HelpBox("This Scene type is missing commands", MessageType.Warning);
            }
            else
            {

                for (int j = 0; j < _libretto.keywords[i].keywords.Count; j++)
                {
                    string name = _libretto.keywords[i].keywords[j].name;
                    int eventIndex = _libretto.keywords[i].keywords[j].eventIndex;

                    EditorGUILayout.PropertyField(events.GetArrayElementAtIndex(eventIndex), new GUIContent(name));
                }
            }

            EditorGUILayout.Space();
        }
    }

    private void DrawScenes()
    {
        GUIStyle scenesStyle = new GUIStyle();
        scenesStyle.fontSize = 14;
        scenesStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Scenes", scenesStyle);

        for (int i = 0; i < _libretto.scenes.Count; i++)
        {
            EditorGUILayout.PropertyField(scenes.GetArrayElementAtIndex(i));
        }
    }

    #region Level Parsing

    public void ParseLevel()
    {
        // Create new list of scenes and save the previous events and glossary of keywords
        List<CommandsList> newScenes = new List<CommandsList>();
        List<CommandEvent> prevEvents = new List<CommandEvent>(_libretto.events);
        List<KeywordGlossary> prevKeywords = new List<KeywordGlossary>(_libretto.keywords);

        // Clear the old scenes, events, and keywords.
        _libretto.scenes.Clear();
        _libretto.events.Clear();
        _libretto.keywords.Clear();

        // Split file by line 
        string[] commands = _libretto.levelInfo.text.Split('\n');

        // Look for where each scene is located
        List<int> sceneIndices = new List<int>();
        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i].ToLower().StartsWith("scene"))
            {
                sceneIndices.Add(i);
            }
        }

        // Parse each scene
        for (int i = 0; i < sceneIndices.Count; i++)
        {
            // Figure out the starting and ending line number of the current scene
            int sceneStart = sceneIndices[i];

            // If we're on the last scene then the ending line number is the end of the file, otherwise it's the line before the next scene
            int sceneEnd = i == sceneIndices.Count - 1 ? commands.Length : sceneIndices[i + 1] - 1;

            // Create a list of strings containing only the scene information
            List<string> sceneInfo = new List<string>();
            for (int j = sceneStart; j < sceneEnd; j++)
            {
                sceneInfo.Add(commands[j]);
            }

            // Parse scene and add to list
            newScenes.Add(ParseScene(sceneInfo));
        }

        // Assign the scenes to the new commands list we parsed
        _libretto.scenes = newScenes;

        // Finally, if the libretto's new keywords have the same keyword names as the old keyword names,
        // we're going to replace those events with the events it had previously
        for (int i = 0; i < prevKeywords.Count; i++)
        {
            if (ContainsKeywordGlossary(prevKeywords[i].type))
            {
                for (int j = 0; j < prevKeywords[i].keywords.Count; j++)
                {
                    if (ContainsKeyword(prevKeywords[i].type, prevKeywords[i].keywords[j].name))
                    {
                        // Get the new keyword with the matching parameters
                        int index1 = GetKeywordGlossaryIndexByType(prevKeywords[i].type);
                        int index2 = GetKeywordIndexByType(index1, prevKeywords[i].keywords[j].name);

                        // And replace the new event with the old one (so we don't lose the listeners)
                        _libretto.events[_libretto.keywords[index1].keywords[index2].eventIndex] = prevEvents[prevKeywords[i].keywords[j].eventIndex];
                    }
                }
            }
        }
    }

    CommandsList ParseScene(List<string> commands)
    {
        // Our CommandsList we will eventually return
        CommandsList newScene = new CommandsList();

        // Get our commandslist type and name
        string newSceneType = commands[0].Split(' ')[1].Trim();
        string newSceneName = commands[0].Substring(newSceneType.Length + 7).Trim();
        newScene.type = newSceneType;
        newScene.name = newSceneName;

        // If the keyword glossary is NOT in our libretto's keywords, add it
        if (!ContainsKeywordGlossary(newSceneType))
        {
            KeywordGlossary newGlossary = new KeywordGlossary();
            newGlossary.type = newSceneType;
            newGlossary.keywords = new List<Keyword>();
            _libretto.keywords.Add(newGlossary);
        }

        // Parse each command and add it to the commandslist's commands
        for (int i = 1; i < commands.Count; i++)
        {
            // If our command starts with a command starter
            if (CheckCommand(commands[i]))
            {
                // Our command we will eventually add to the commandslist's list of commands
                Command command = new Command();

                // Get the command line
                string commandLine = StripCommandStarters(commands[i]);

                // Separate the command word and the arguments
                string keyword = commandLine.Split(' ')[0].Trim();
                string arguments = commandLine.Substring(keyword.Length + 1).Trim();

                // If the keyword of the command line is NOT in our list of keywords, add a new event and keyword 
                if (!ContainsKeyword(newSceneType, keyword))
                {
                    _libretto.events.Add(new CommandEvent());

                    Keyword newKeyword = new Keyword();
                    newKeyword.name = keyword;
                    newKeyword.eventIndex = _libretto.events.Count - 1;
                    _libretto.keywords[GetKeywordGlossaryIndexByType(newSceneType)].keywords.Add(newKeyword);
                }

                // Give the command the keyword and arguments
                int index1 = GetKeywordGlossaryIndexByType(newSceneType);
                int index2 = GetKeywordIndexByType(index1, keyword);
                command.keyword = _libretto.keywords[index1].keywords[index2];
                command.arguments = arguments;

                // Add our newly made command to the commands of the scene
                newScene.commands.Add(command);
            }
        }

        // Return the new scene
        return newScene;
    }

    #region Helper Functions

    private bool CheckCommand(string command)
    {

        // TODO Fix this entire method
        // If the command starts with any of the command starters (and has something after it), it is a command
        string commandCheck;
        foreach (string s in commandStarters)
        {

            commandCheck = command;

            if (commandCheck.StartsWith(s))
            {
                commandCheck = commandCheck.Replace(s, "");

                // if it doesn't start with a space, tab, or newline, it is a command
                if (!commandCheck.StartsWith(" ") && !commandCheck.StartsWith("\t") && !commandCheck.StartsWith("\n"))
                {
                    return true;
                }
            }
        }

        return false;

    }

    private string StripCommandStarters(string command)
    {
        // While the line starts with a space or tab, delete it
        while (command.StartsWith(" ") || command.StartsWith("\t"))
        {
            command = command.Substring(1);
        }
        return command;
    }

    private int GetKeywordGlossaryIndexByType(string type)
    {
        for (int i = 0; i < _libretto.keywords.Count; i++)
        {
            if (_libretto.keywords[i].type == type)
            {
                return i;
            }
        }
        return -1;
    }

    private int GetKeywordIndexByType(int glossary, string type)
    {
        for (int i = 0; i < _libretto.keywords[glossary].keywords.Count; i++)
        {
            if (_libretto.keywords[glossary].keywords[i].name == type)
            {
                return i;
            }
        }
        return -1;
    }

    private bool ContainsKeywordGlossary(string type)
    {
        return GetKeywordGlossaryIndexByType(type) >= 0 ? true : false;
    }

    private bool ContainsKeyword(string type, string keyword)
    {
        if (ContainsKeywordGlossary(type))
        {
            return GetKeywordIndexByType(GetKeywordGlossaryIndexByType(type), keyword) >= 0 ? true : false;
        }
        return false;
    }

    #endregion

    #endregion

}
