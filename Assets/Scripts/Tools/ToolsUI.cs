using Renderers;
using Structures_Geométriques.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
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

    // Sélectionner la face suivante dans la liste
    IndexedPolygon SelectNextFace(ref IndexedPolygon selectedFace)
    {
        
            int currentIndex = System.Array.IndexOf(_relativeGeometry._polygons.ToArray(), selectedFace);
            int nextIndex = (currentIndex + 1) % _relativeGeometry._polygons.ToArray().Length; // Boucle pour revenir au début
            return _relativeGeometry._polygons[nextIndex];

    }

    // Sélectionner le point suivant dans la liste
    Point SelectNextPoint(ref Point selectedPoint)
    {
        
        int currentIndex = System.Array.IndexOf(_relativeGeometry._points.ToArray(), selectedPoint);
        int nextIndex = (currentIndex + 1) % _relativeGeometry._points.ToArray().Length; // Boucle pour revenir au début

        return _relativeGeometry._points[nextIndex];

    }

    // Sélectionner le point précédent dans la liste
    Point SelectPreviousPoint(ref Point selectedPoint)
    {
       
         int currentIndex = System.Array.IndexOf(_relativeGeometry._points.ToArray(), selectedPoint);
         int previousIndex = (currentIndex - 1 + _relativeGeometry._points.ToArray().Length) % _relativeGeometry._points.ToArray().Length; // Boucle pour revenir à la fin
         return _relativeGeometry._points[previousIndex];

    }

    // Sélectionner la face précédente dans la liste
    IndexedPolygon SelectPreviousFace(ref IndexedPolygon selectedFace)
    {

        int currentIndex = System.Array.IndexOf(_relativeGeometry._polygons.ToArray(), selectedFace);
        int previousIndex = (currentIndex - 1 + _relativeGeometry._polygons.ToArray().Length) % _relativeGeometry._polygons.ToArray().Length; // Boucle pour revenir à la fin
        return _relativeGeometry._polygons[previousIndex];

    }

}

// creer onjet vide 
// Creer nouveau point



