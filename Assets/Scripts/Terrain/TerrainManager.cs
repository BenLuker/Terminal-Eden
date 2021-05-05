using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TerminalEden.Terrain
{
    [RequireComponent(typeof(MeshRenderer))]
    // [RequireComponent(typeof(MeshGenerator))]
    public class TerrainManager : SingletonBehaviour<TerrainManager>
    {
        public MeshRenderer rend;
        public MeshManager meshManager;

        // Mesh variables
        float meshSize;
        float targetVisibleTerrainPercent;
        float visibleTerrainPercent;
        float visualizationTransition;

        [System.Serializable] public class Vector2Event : UnityEvent<Vector2> { }
        [System.Serializable] public class Texture2DEvent : UnityEvent<Texture2D> { }

        public Vector2Event onMouseOverTerrain = new Vector2Event();
        public UnityEvent onVisualizationChange = new UnityEvent();
        public Texture2DEvent onMeshGenerated = new Texture2DEvent();

        #region Events

        void Start()
        {
            CreateMesh();
            WriteMesh();
        }

        void Update()
        {
            DetectMouseOverPosition();
            UpdateMaterial();
        }

        private void FixedUpdate()
        {
            visibleTerrainPercent = Vector2.Lerp(new Vector2(visibleTerrainPercent, 0), new Vector2(targetVisibleTerrainPercent, 0), Time.deltaTime * 10).x;
            visualizationTransition = Vector2.Lerp(new Vector2(visualizationTransition, 0), new Vector2(targetVisibleTerrainPercent / 2, 0), Time.deltaTime * 4).x;
        }

        #endregion

        void CreateMesh()
        {
            meshManager.CreateMesh();
            meshSize = meshManager.size;
        }

        void WriteMesh()
        {
            onMeshGenerated.Invoke(meshManager.WriteMesh());
        }

        void DetectMouseOverPosition()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            LayerMask mask = LayerMask.GetMask("Terrain");
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                onMouseOverTerrain.Invoke(hit.textureCoord);
            }
        }

        public void UpdateMaterial()
        {
            rend.material.SetFloat("_percentageVisible", visibleTerrainPercent);
            rend.material.SetFloat("_wipe", visualizationTransition);
        }

        public void UpdateWindVisualization(float zoom)
        {
            rend.material.SetFloat("_WireframeScale", Mathf.Clamp(Mathf.Lerp(2, 15, Mathf.InverseLerp(10, 100, zoom)), 2, 15));
            rend.material.SetFloat("_WindHeight", Mathf.Clamp(Mathf.Lerp(0.25f, 1, Mathf.InverseLerp(10, 100, zoom)), 0.25f, 1));
        }

        #region Public Methods

        public float GetMeshSize()
        {
            return meshSize;
        }

        public void SetMaterialIndex(float index)
        {
            // Grab current index and set it as the old index
            float prev = rend.material.GetFloat("_index");
            rend.material.SetFloat("_prevIndex", prev);

            // Set transition to 0
            visualizationTransition = 0;

            // Set new index
            rend.material.SetFloat("_index", index);
        }

        public void SetSimulationTexture(RenderTexture rt)
        {
            rend.material.SetTexture("_sim", rt);
            rend.material.SetFloat("_textureSize", rt.width);
        }

        public void SetSelectionTexture(RenderTexture rt)
        {
            rend.material.SetTexture("_selection", rt);
        }

        // public void SetSelectedCells(List<Vector2> cells)
        // {
        //     selectedCells.Clear();
        //     for (int i = 0; i < 100; i++)
        //     {
        //         if (i < cells.Count)
        //         {
        //             selectedCells.Add(cells[i]);
        //         }
        //         else
        //         {
        //             selectedCells.Add(Vector4.zero);
        //         }
        //     }
        //     selectedCellsSize = cells.Count;
        // }

        public void SetVisibleTerrainWidthPercentage(float percentage)
        {
            targetVisibleTerrainPercent = percentage;
            // rend.material.SetFloat("_percentageVisible", percentage);
        }

        // public void SetRevealSpeed(float speed)
        // {
        //     revealSpeed = speed;
        // }

        #endregion

    }
}
