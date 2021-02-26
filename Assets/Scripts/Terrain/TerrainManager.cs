using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Visualization { None, Color, VegetationDensity, Topography }

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshGenerator))]
public class TerrainManager : MonoBehaviour
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

    // Data Visualizations
    public Visualization vis;
    Visualization prevVis;
    public Material visualizationNone;
    public Material visualizationColor;
    public Material visualizationVegetationDensity;
    public Material visualizationTopography;

    public UnityEvent onVisualizationChange = new UnityEvent();

    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        generator = GetComponent<MeshGenerator>();
        CreateMesh();
    }

    void Update()
    {
        if (prevVis != vis)
            ChangeVisualization();
        UpdateVisualization();
    }

    private void FixedUpdate()
    {
        visibleTerrainPercent = Vector2.Lerp(new Vector2(visibleTerrainPercent, 0), new Vector2(targetVisibleTerrainPercent, 0), Time.deltaTime * 10).x;
    }

    void CreateMesh()
    {
        generator.CreateMesh();
        meshSize = generator.size;
    }

    void ChangeVisualization()
    {
        switch (vis)
        {
            case Visualization.None:
                rend.material = visualizationNone;
                break;
            case Visualization.Color:
                rend.material = visualizationColor;
                rend.material.mainTexture = displayTexture;
                break;
            case Visualization.VegetationDensity:
                rend.material = visualizationVegetationDensity;
                rend.material.mainTexture = displayTexture;
                rend.material.SetInt("textureSize", textureSize);
                break;
        }
        onVisualizationChange.Invoke();
        prevVis = vis;
    }

    void UpdateVisualization()
    {
        switch (vis)
        {
            case Visualization.None:
                break;
            case Visualization.Color:
                rend.material.SetFloat("percentageVisible", visibleTerrainPercent);
                break;
            case Visualization.VegetationDensity:
                rend.material.SetFloat("percentageVisible", visibleTerrainPercent);
                break;
        }
    }

    #region Public Methods

    public void SetDisplayTexture(RenderTexture rt)
    {
        displayTexture = rt;
        textureSize = rt.width;
    }

    public void ChangeVisualization(string name)
    {
        Visualization v = (Visualization)Enum.Parse(typeof(Visualization), name, true);
        if (Enum.IsDefined(typeof(Visualization), v))
        {
            vis = v;
        }
        else
        {
            Debug.LogErrorFormat("{0} could not be parsed as an enum.", name);
        }
    }

    public void SetVisibleTerrainWidthPercentage(float percentage)
    {
        targetVisibleTerrainPercent = percentage;
    }

    #endregion

    // public Vector2Int WorldToCell(Vector3 pos)
    // {
    //     int x = (int)Mathf.Round(pos.x.Remap(transform.position.x - transform.localScale.x / 2, transform.position.x + transform.localScale.x / 2, 0, textureResolution));
    //     int z = (int)Mathf.Round(pos.y.Remap(transform.position.z - transform.localScale.y / 2, transform.position.z + transform.localScale.y / 2, 0, textureResolution));

    //     // x.Remap(transform.position.x - transform.localScale.x / 2, transform.position.x + transform.localScale.x / 2, 0, textureResolution);
    //     // z.Remap(transform.position.z - transform.localScale.y / 2, transform.position.z + transform.localScale.y / 2, 0, textureResolution);
    //     // float xPos = Mathf.InverseLerp(transform.position.x - transform.localScale.x / 2, transform.position.x + transform.localScale.x / 2, pos.x);
    //     // float zPos = Mathf.InverseLerp(transform.position.z - transform.localScale.y / 2, transform.position.z + transform.localScale.y / 2, pos.z);

    //     // int x = (int)Mathf.Round(Mathf.Lerp(0, textureResolution, xPos));
    //     // int z = (int)Mathf.Round(Mathf.Lerp(0, textureResolution, zPos));

    //     return new Vector2Int(x, z);
    // }

}
