using Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;
using Control;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class ToolsUI
{
    private RelativeGeometry _relativeGeometry;

    public ToolsUI(RelativeGeometry relative_geometry)
    {
        _relativeGeometry = relative_geometry;
    }

    // S�lectionner la face suivante dans la liste
    IndexedPolygon SelectNextFace(ref IndexedPolygon selectedFace)
    {
        int currentIndex = System.Array.IndexOf(_relativeGeometry._polygons.ToArray(), selectedFace);
        int nextIndex =
            (currentIndex + 1) % _relativeGeometry._polygons.ToArray().Length; // Boucle pour revenir au d�but
        return _relativeGeometry._polygons[nextIndex];
    }

    public void UpdatePointInLine(UiEvents.LineEventData arg0)
    {
        UpdatePointInLine(arg0.objectIndex, arg0.isFirst, arg0.up);
    }

    // S�lectionner le point suivant dans la liste
    void UpdatePointInLine(int lineIndex, bool isFirst, bool increment)
    {
        var line = _relativeGeometry._lines[lineIndex];
        var v2 = line.Data;
        var max = _relativeGeometry._points.Count;
        v2.x = (v2.x + (isFirst ? (increment ? 1 : -1) : 0) + max) % max;
        v2.y = (v2.y + (!isFirst ? (increment ? 1 : -1) : 0) + max) % max;
        line.Data = v2;
    }

    void UpdatePointInPolygon(int polygonIndex, int pointIndex, bool increment)
    {
        var polygon = _relativeGeometry._polygons[polygonIndex];
        polygon.Data.indexes[pointIndex] +=
            (increment ? 1 : -1);
    }

    void UpdatePointInPolyedre(int polyedreIndex, int pointIndex, bool increment)
    {
        var polyedre = _relativeGeometry._polyedres[polyedreIndex];
        polyedre.Data.indexes[pointIndex] +=
            (increment ? 1 : -1);
    }
}