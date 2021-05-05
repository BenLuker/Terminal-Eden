using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerminalEden.Simulation
{
    [CreateAssetMenu(fileName = "New Event", menuName = "Terminal Eden/Event", order = 1)]
    public class EdenEvent : ScriptableObject
    {
        public string title;
        public string copy;
        public Material material;
        public List<Vector2> affectedCells = new List<Vector2>();
        public int setupState0;
        public int setupState;
        public int affectState;

        public void Prepare(Vector2 center, RenderTexture sim)
        {
            List<Vector4> cells = new List<Vector4>();
            for (int i = 0; i < 100; i++)
            {
                if (i < affectedCells.Count)
                {
                    cells.Add(affectedCells[i] + center);
                }
                else
                {
                    cells.Add(Vector4.zero);
                }
            }

            material.SetInt("textureSize", sim.width);
            material.SetVectorArray("_SelectedCells", cells);
            material.SetFloat("_SelectedCellsSize", affectedCells.Count);
        }
    }
}