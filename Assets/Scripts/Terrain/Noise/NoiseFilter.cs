using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum NoiseType { Perlin, Simplex, PerlinRidge, SimplexRidge }

[System.Serializable]
public class NoiseLayer
{
    public bool enabled = true;
    public NoiseType noiseType;
    public Vector2 tiling;
    public float roughness;
    public float strength;

    public float Evaluate(Vector2 point, float seed)
    {
        float noiseValue = 0;
        Vector2 coords = new Vector2((point.x + tiling.x) * roughness, (point.y + tiling.y) * roughness);

        if (enabled)
        {
            switch (noiseType)
            {
                case NoiseType.Perlin:
                    noiseValue = Mathf.PerlinNoise(coords.x, coords.y) * strength;
                    break;
                case NoiseType.Simplex:
                    noiseValue = SimplexNoise.Noise.Generate(coords.x + seed, coords.y - seed) * strength;
                    break;
                case NoiseType.PerlinRidge:
                    noiseValue = Mathf.Pow(1 - Mathf.Abs(Mathf.PerlinNoise(coords.x, coords.y)), 2) * strength;
                    break;
                case NoiseType.SimplexRidge:
                    noiseValue = Mathf.Pow(1 - Mathf.Abs(SimplexNoise.Noise.Generate(coords.x - seed, coords.y + seed)), 2) * strength;
                    break;
            }
        }
        return noiseValue;
    }
}

[CreateAssetMenu(fileName = "New Noise Filter", menuName = "Terminal Eden/Noise Filter", order = 1)]
public class NoiseFilter : ScriptableObject
{
    public bool showFeatures = true;
    public bool showMask = true;
    public bool showDetails = true;

    public float seed;

    public List<NoiseLayer> featureLayers;
    public List<NoiseLayer> maskLayers;
    public List<NoiseLayer> detailLayers;

    public float Evaluate(Vector2 point)
    {
        float noiseValue = 0;

        if (showFeatures)
        {
            foreach (NoiseLayer feature in featureLayers)
            {
                noiseValue += feature.Evaluate(point, seed);
            }
        }

        if (showMask)
        {
            foreach (NoiseLayer mask in maskLayers)
            {
                noiseValue *= Mathf.Clamp(mask.Evaluate(point, seed), 0, 1);
            }
        }

        if (showDetails)
        {
            foreach (NoiseLayer detail in detailLayers)
            {
                noiseValue += detail.Evaluate(point, seed);
            }
        }

        return noiseValue;
    }
}
