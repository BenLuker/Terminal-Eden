using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Libretto
{
    [System.Serializable]
    public class Keyword
    {
        public string name;
        public int eventIndex;
    }

    [System.Serializable]
    public class Command
    {
        public Keyword keyword;
        public string arguments;
    }

    [System.Serializable]
    public class CommandsList
    {
        public string name;
        public string type;
        public List<Command> commands = new List<Command>();
    }

    [System.Serializable]
    public class CommandEvent : UnityEvent<string> { }

    [System.Serializable]
    public class KeywordGlossary
    {
        public string type;
        public List<Keyword> keywords;
    }

    public class LevelLibretto : SingletonBehaviour<LevelLibretto>
    {
        public List<CommandsList> scenes = new List<CommandsList>();
        public List<CommandEvent> events = new List<CommandEvent>();
        public List<KeywordGlossary> keywords = new List<KeywordGlossary>();

        // Editor values
        public TextAsset levelInfo;
        public string levelData;

        IEnumerator processCommands;
        bool completeCommand;
        bool completeScene;

        public void ProcessScenes()
        {
            StartCoroutine(ProcessAllScenes());
        }

        public void ProcessScene(string name)
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].name == name)
                {
                    ProcessScene(i);
                    return;
                }
            }
            Debug.LogErrorFormat("Processing Scene Failed: Scene {0} does not exist", name);
        }

        public void ProcessScene(int index)
        {
            if (index < 0 || index > scenes.Count)
            {
                Debug.LogErrorFormat("Processing Scene Failed: Scene at index {0} does not exist", index);
            }
            else
            {
                List<Command> sceneCommands = scenes[index].commands;
                processCommands = ProcessCommands(sceneCommands);
                StartCoroutine(processCommands);
            }
        }

        IEnumerator ProcessAllScenes()
        {
            for (int i = 0; i < scenes.Count; i++)
            {
                completeScene = false;
                ProcessScene(i);
                while (!completeScene) yield return null;
            }
        }

        IEnumerator ProcessCommands(List<Command> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                Command command = commands[i];
                completeCommand = false;
                events[command.keyword.eventIndex].Invoke(command.arguments);
                while (!completeCommand) yield return null;
            }
            completeScene = true;
        }

        public void CompleteCommand()
        {
            completeCommand = true;
        }
    }
}