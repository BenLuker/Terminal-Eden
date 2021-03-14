using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerminalEden.Simulation
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Terminal Eden/Ability", order = 1)]
    public class Ability : ScriptableObject
    {
        public Material material;

        public int coolDown;
        public List<Vector2> selectedCells = new List<Vector2>();
        public List<Vector2> affectedCells = new List<Vector2>();

        public List<Vector2> GetSelection(Vector2 center)
        {
            List<Vector2> selection = new List<Vector2>();
            foreach (Vector2 v in selectedCells)
            {
                selection.Add(v + center);
            }
            return selection;
        }

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