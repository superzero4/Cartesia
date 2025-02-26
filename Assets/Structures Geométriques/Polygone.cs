using UnityEngine;
using System.Collections.Generic;

public struct Polygon
{
    public List<Point2D> sommets; // Liste des sommets du polygone

    public Polygon(List<Point2D> sommets)
    {
        this.sommets = new List<Point2D>(sommets);
    }

    // Calculer le périmètre du polygone
    public float Perimeter()
    {
        if (sommets.Count < 3) return 0; 

        float perimeter = 0;
        for (int i = 0; i < sommets.Count; i++)
        {
            Point2D current = sommets[i];
            Point2D next = sommets[(i + 1) % sommets.Count]; 

            perimeter += Vector2.Distance(new Vector2(current.x, current.y), new Vector2(next.x, next.y));
        }
        return perimeter;
    }

    // Afficher les points du polygone
    public override string ToString()
    {
        string result = "Polygone : ";
        foreach (var point in sommets)
        {
            result += point.ToString() + " -> ";
        }
        return result + sommets[0].ToString(); 
    }
}
