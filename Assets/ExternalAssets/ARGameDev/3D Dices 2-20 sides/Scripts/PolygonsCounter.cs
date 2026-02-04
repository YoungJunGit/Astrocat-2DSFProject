using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ARGameDev
{
    public class PolygonsCounter : MonoBehaviour
    {
        [SerializeField] private List<MeshFilter> meshFilters;
        [SerializeField] private int totalPolygons;
        [SerializeField] private TMP_Text totalPolygonsText;

        private void OnValidate() => CountPolygons();

        private void CountPolygons()
        {
            if (meshFilters == null || meshFilters.Count == 0) return;
            totalPolygons = 0;
            for (int i = 0; i < meshFilters.Count; i++)
            {
                if (meshFilters[i] != null && meshFilters[i].sharedMesh != null)
                {
                    int polyCount = meshFilters[i].sharedMesh.triangles.Length / 3;
                    totalPolygons += polyCount;
                }
            }

            if (totalPolygonsText != null) totalPolygonsText.text = $"Polygons:\n{totalPolygons}";
        }
    }
}