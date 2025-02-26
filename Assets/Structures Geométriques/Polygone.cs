using UnityEngine;
using System.Collections.Generic;

public struct Polygone
{
    public List<Point> sommets; // Liste des sommets du polygone

    public Polygone(List<Point> sommets)
    {
        this.sommets = new List<Point>(sommets);
    }

    // Calculer le périmètre du polygone
    public float Perimeter()
    {
        if (sommets.Count < 3) return 0; 

        float perimeter = 0;
        for (int i = 0; i < sommets.Count; i++)
        {
            Point current = sommets[i];
            Point next = sommets[(i + 1) % sommets.Count]; 

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
