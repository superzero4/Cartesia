using System.Collections.Generic;

namespace Structures_Geométriques
{
    public class Polyedre
    {
        private List<Polygone> faces;
        public Polyedre(IEnumerable<Polygone> faces)
        {
            this.faces = new List<Polygone>(faces);
        }

        public List<Polygone> Faces => faces;
    }
}