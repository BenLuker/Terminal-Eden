using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TerminalEden.Simulation
{

    public class WildfireSimulation : SingletonBehaviour<WildfireSimulation>
    {
        // Sim Materials
        public Material simMat;
        public Material initMat;

        // Simulation Settings
        public RenderTexture[] sim;
        RenderTexture displayTexture;
        RenderTexture tempState;

        public int textureResolution = 1024;
        public int simStates = 26;
        public int currentState = 0;
        public int historyTracker;
        public Texture2D initialState;

        public bool displaySimulation = true;
        public bool generateStart = true;
        public bool updateOverTime = true;
        public int refreshRate = 30;
        public int visibleCellsWidth;

        // Ability Settings
        public Ability heldAbility;
        Vector2 selectionCenter;
        List<Vector2> selection = new List<Vector2>();

        // Events
        [System.Serializable] public class RenderTextureEvent : UnityEvent<RenderTexture> { }
        [System.Serializable] public class FloatEvent : UnityEvent<float> { }
        [System.Serializable] public class Vector2ListEvent : UnityEvent<List<Vector2>> { }

        public RenderTextureEvent onSimulationTextureCreated = new RenderTextureEvent();
        public Vector2ListEvent onCellsSelected = new Vector2ListEvent();
        public FloatEvent onVisibleCellsChanged = new FloatEvent();

        #region Events

        private void Start()
        {
            InitSimulation();
            if (updateOverTime) ExecuteSimulation();
            ChangeCellWidth(textureResolution);
        }

        private void Update()
        {
            Graphics.Blit(sim[currentState], displayTexture);
        }

        #endregion

        #region Public Methods

        public void PlayPauseSimulation(bool play)
        {
            updateOverTime = play;

            if (updateOverTime)
            {
                ExecuteSimulation();
            }
            else
            {
                TerminateSimulation();
            }
        }

        public void HoldAbility(Ability ability)
        {
            heldAbility = ability;
        }

        public void HighlightCells(Vector2 uv)
        {
            if (heldAbility == null) return;
            selectionCenter = new Vector2(Mathf.Ceil(uv.x * textureResolution), Mathf.Ceil(uv.y * textureResolution));
            selection = heldAbility.GetSelection(selectionCenter);
            onCellsSelected.Invoke(selection);
        }

        public void CastAbility()
        {
            if (heldAbility == null)
            {
                Debug.LogWarning("No held ability to cast");
                return;
            }

            // Get current state of simulation and save as temp state
            Graphics.Blit(sim[currentState], tempState);

            // Give the held ability all of the variables it needs
            heldAbility.Prepare(selectionCenter, sim[currentState]);

            // Then blit with the prepared material
            Graphics.Blit(tempState, sim[currentState], heldAbility.material);
        }

        public void ChangeCellWidth(int cellWidth)
        {
            onVisibleCellsChanged.Invoke((float)cellWidth / (float)textureResolution);
            visibleCellsWidth = cellWidth;
        }

        #endregion

        void InitSimulation()
        {
            // Create Display Render Texture
            displayTexture = new RenderTexture(textureResolution, textureResolution, 1);
            displayTexture.filterMode = FilterMode.Point;
            displayTexture.Create();
            onSimulationTextureCreated.Invoke(displayTexture);

            // Create Simulation Render Textures
            sim = new RenderTexture[simStates];
            for (int i = 0; i < simStates; i++)
            {
                sim[i] = new RenderTexture(textureResolution, textureResolution, 1);
                sim[i].filterMode = FilterMode.Point;
                sim[i].Create();
            }

            // Create Temp State Render Texture
            tempState = new RenderTexture(textureResolution, textureResolution, 1);
            tempState.filterMode = FilterMode.Point;
            tempState.Create();

            // reset current state index to 0 
            currentState = 0;
            historyTracker = 0;

            // If starting is not generated, blit the initial state. Otherwise, blit a generated init
            if (generateStart)
            {
                Graphics.Blit(sim[currentState], sim[currentState], initMat);
            }
            else
            {
                Graphics.Blit(initialState, sim[currentState]);
            }

            // Set simulation variables to all textures
            simMat.SetInt("textureSize", textureResolution);
        }

        [ContextMenu("Calculate Step")]
        public void CalculateStep()
        {
            int prevState = currentState;
            currentState = (currentState + 1) % simStates;
            historyTracker += historyTracker < simStates - 1 ? 1 : 0;

            Graphics.Blit(sim[prevState], sim[currentState], simMat);
        }

        [ContextMenu("Undo Step")]
        public void UndoStep()
        {
            if (historyTracker > 0)
            {
                historyTracker--;
                currentState = ((currentState - 1) + simStates) % simStates;
            }
            else
            {
                Debug.LogError("Cannot undo");
            }
        }

        [ContextMenu("Save Texture")]
        public void SaveTexture()
        {
            Texture2D tex = new Texture2D(textureResolution, textureResolution, TextureFormat.RGB24, false);

            RenderTexture.active = sim[currentState];
            tex.ReadPixels(new Rect(0, 0, textureResolution, textureResolution), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            byte[] bytes = tex.EncodeToPNG();
            string path = Application.dataPath + "/Scripts/Simulation";
            System.IO.Directory.CreateDirectory(path);
            System.IO.File.WriteAllBytes(path + "/SavedTexture.png", bytes);
        }

        public void ExecuteSimulation()
        {
            StartCoroutine(ExecuteSimulationCoroutine());
        }

        public void TerminateSimulation()
        {
            StopAllCoroutines();
        }

        IEnumerator ExecuteSimulationCoroutine()
        {
            while (true)
            {
                CalculateStep();
                yield return new WaitForSeconds(1 / (float)refreshRate);
            }
        }
    }
}
