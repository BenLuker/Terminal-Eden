using System.Threading;
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
        public Material calculateOverallLevels;

        // Simulation Settings
        public RenderTexture[] sim;
        public Texture2D readState;
        RenderTexture[] persistentLevels;
        RenderTexture displayTexture;
        RenderTexture tempState;

        Coroutine SC;
        public int textureResolution = 1024;
        public int simStates = 26;
        public int currentState = 0;
        public int historyTracker;
        public Texture2D initialState;

        public bool generateStart = true;
        public bool updateOverTime = true;
        public int refreshRate = 30;
        public int readRate = 30;
        public int visibleCellsWidth;

        // Ability Settings
        public Ability heldAbility;
        Vector2 selectionCenter;
        List<Vector2> selection = new List<Vector2>();

        // Events
        [System.Serializable] public class RenderTextureEvent : UnityEvent<RenderTexture> { }
        [System.Serializable] public class FloatEvent : UnityEvent<float> { }
        [System.Serializable] public class Vector2ListEvent : UnityEvent<List<Vector2>> { }
        [System.Serializable] public class AbilityEvent : UnityEvent<Ability> { }

        public RenderTextureEvent onSimulationTextureCreated = new RenderTextureEvent();
        public UnityEvent onStep = new UnityEvent();
        public Vector2ListEvent onCellsSelected = new Vector2ListEvent();
        public FloatEvent onVisibleCellsChanged = new FloatEvent();
        public FloatEvent onForestCounted = new FloatEvent();
        public AbilityEvent onAbilityCasted = new AbilityEvent();

        #region Events

        private void Start()
        {
            InitSimulation();
            if (updateOverTime) ExecuteSimulation();
            StartCoroutine(ReadForestLevels());
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

        public void PlayPauseSimulation()
        {
            if (updateOverTime)
            {
                TerminateSimulation();
            }
            else
            {
                ExecuteSimulation();
            }
            updateOverTime = !updateOverTime;
        }

        public void HoldAbility(Ability ability)
        {
            heldAbility = ability;
        }

        public void HighlightCells(Vector2 uv)
        {
            if (heldAbility == null)
            {
                selection.Clear();
            }
            else
            {
                selectionCenter = new Vector2(Mathf.Ceil(uv.x * textureResolution), Mathf.Ceil(uv.y * textureResolution));
                selection = heldAbility.GetSelection(selectionCenter);
            }
            onCellsSelected.Invoke(selection);
        }

        public void CastAbility()
        {
            if (heldAbility == null)
            {
                Debug.LogWarning("No held ability to cast");
                return;
            }

            if (!isInRevealedCells(selectionCenter)) return;

            // Get current state of simulation and save as temp state
            Graphics.Blit(sim[currentState], tempState);

            // Give the held ability all of the variables it needs
            heldAbility.Prepare(selectionCenter, sim[currentState]);

            // Then blit with the prepared material
            Graphics.Blit(tempState, sim[currentState], heldAbility.material);

            // Invoke Ability Casted and Unselect Ability
            onAbilityCasted.Invoke(heldAbility);
            heldAbility = null;
        }

        public void CastEvent(EdenEvent newEvent)
        {
            Graphics.Blit(sim[currentState], tempState);
            newEvent.Prepare(new Vector2(textureResolution / 2, textureResolution / 2), sim[currentState]);
            Graphics.Blit(tempState, sim[currentState], newEvent.material);
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

            // Create Texture2D for reading the simulation on the CPU level
            readState = new Texture2D(textureResolution, textureResolution);
            readState.filterMode = FilterMode.Point;

            // Create Render Textures for gathering the overall state of the simulation on the CPU level
            int persistentLevelsSize = (int)Mathf.Round(Mathf.Log(textureResolution) / Mathf.Log(2));
            persistentLevels = new RenderTexture[persistentLevelsSize];
            for (int i = 0; i < persistentLevelsSize; i++)
            {
                persistentLevels[i] = new RenderTexture((int)Mathf.Pow(2, i), (int)Mathf.Pow(2, i), 1);
                persistentLevels[i].filterMode = FilterMode.Point;
                persistentLevels[i].Create();
            }

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
            onStep.Invoke();
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

        public Texture2D ReadSimulation()
        {
            RenderTexture.active = sim[currentState];
            // RenderTexture.active = persistentLevels[persistentLevels.Length - 1];
            readState.ReadPixels(new Rect(0, 0, textureResolution, textureResolution), 0, 0);
            readState.Apply();
            RenderTexture.active = null;
            return readState;
        }

        // I feel like there's a better way of doing this, but I have no idea what that would be.
        IEnumerator ReadForestLevels()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                // calculateOverallLevels.SetInt("fromSize", textureResolution);
                // calculateOverallLevels.SetInt("toSize", persistentLevels[persistentLevels.Length - 1].width);
                // Graphics.Blit(sim[currentState], persistentLevels[persistentLevels.Length - 1], calculateOverallLevels);

                // for (int i = persistentLevels.Length - 1; i >= 0; i--)
                // {
                //     // Initial Blit
                //     if (i == persistentLevels.Length - 1)
                //     {
                //         // Blit simulation onto biggest persistent level texture
                //         Debug.LogFormat("Blitting from simulation to texture {0}", i);

                //         calculateOverallLevels.SetInt("fromSize", textureResolution);
                //         calculateOverallLevels.SetInt("toSize", persistentLevels[i].width);
                //         Graphics.Blit(sim[currentState], persistentLevels[persistentLevels.Length - 1], calculateOverallLevels);
                //     }

                //     // Subsequent Blits
                //     else
                //     {
                //         // Blit previous texture onto new texture
                //         Debug.LogFormat("Blitting from texture {0} to texture {1}", i + 1, i);

                //         calculateOverallLevels.SetInt("fromSize", persistentLevels[i + 1].width);
                //         calculateOverallLevels.SetInt("toSize", persistentLevels[i].width);
                //         Graphics.Blit(sim[i + 1], persistentLevels[i], calculateOverallLevels);
                //     }
                // }

                // Graphics.Blit(sim[currentState], persistentLevels[persistentLevels.Length - 1], calculateOverallLevels);

                // RenderTexture.active = persistentLevels[persistentLevels.Length - 1];
                RenderTexture.active = sim[currentState];
                readState.ReadPixels(new Rect(0, 0, textureResolution, textureResolution), 0, 0);
                RenderTexture.active = null;

                // Wait for specified amount of time before reading again
                for (int i = 0; i < readRate / 2; i++)
                {
                    yield return null;
                }

                // readState.Apply();

                for (int i = 0; i < readRate / 2; i++)
                {
                    yield return null;
                }
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
            SC = StartCoroutine(ExecuteSimulationCoroutine());
        }

        public void TerminateSimulation()
        {
            StopCoroutine(SC);
        }

        IEnumerator ExecuteSimulationCoroutine()
        {
            while (true)
            {
                CalculateStep();
                yield return new WaitForSeconds(1 / (float)refreshRate);
            }
        }

        bool isInRevealedCells(Vector2 coords)
        {
            if (coords.x < textureResolution / 2 + visibleCellsWidth / 2 &&
                coords.x > textureResolution / 2 - visibleCellsWidth / 2 &&
                coords.y < textureResolution / 2 + visibleCellsWidth / 2 &&
                coords.y > textureResolution / 2 - visibleCellsWidth / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
