using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TerminalEden.Terrain
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshGenerator))]
    public class TerrainManager : SingletonBehaviour<TerrainManager>
    {
        MeshRenderer rend;
        MeshGenerator generator;

        // Texture variables
        RenderTexture displayTexture;
        int textureSize;

        // Mesh variables
        float meshSize;
        float targetVisibleTerrainPercent;
        float visibleTerrainPercent;
        List<Vector4> selectedCells;
        float selectedCellsSize;

        [System.Serializable] public class Vector2Event : UnityEvent<Vector2> { }
        public Vector2Event onMouseOverTerrain = new Vector2Event();
        public UnityEvent onVisualizationChange = new UnityEvent();

        #region Events

        void Start()
        {
            rend = GetComponent<MeshRenderer>();
            generator = GetComponent<MeshGenerator>();
            CreateMesh();

            selectedCells = new List<Vector4>();
        }

        void Update()
        {
            DetectMouseOverPosition();
            UpdateMaterial();
        }

        private void FixedUpdate()
        {
            visibleTerrainPercent = Vector2.Lerp(new Vector2(visibleTerrainPercent, 0), new Vector2(targetVisibleTerrainPercent, 0), Time.deltaTime * 10).x;
        }

        #endregion

        void CreateMesh()
        {
            generator.CreateMesh();
            meshSize = generator.size;
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
            rend.material.SetFloat("percentageVisible", visibleTerrainPercent);
            if (selectedCells.Count > 0)
            {
                rend.material.SetVectorArray("_SelectedCells", selectedCells);
                rend.material.SetFloat("_SelectedCellsSize", selectedCellsSize);
            }
        }

        #region Public Methods

        public float GetMeshSize()
        {
            return meshSize;
        }

        public void SetMaterial(Material mat)
        {
            rend.material = mat;
            rend.material.mainTexture = displayTexture;
            rend.material.SetInt("textureSize", textureSize);
        }

        public void SetDisplayTexture(RenderTexture rt)
        {
            displayTexture = rt;
            textureSize = rt.width;
        }

        public void SetSelectedCells(List<Vector2> cells)
        {
            selectedCells.Clear();
            for (int i = 0; i < 100; i++)
            {
                if (i < cells.Count)
                {
                    selectedCells.Add(cells[i]);
                }
                else
                {
                    selectedCells.Add(Vector4.zero);
                }
            }
            selectedCellsSize = cells.Count;
        }

        public void SetVisibleTerrainWidthPercentage(float percentage)
        {
            targetVisibleTerrainPercent = percentage;
        }

        #endregion

    }
}
