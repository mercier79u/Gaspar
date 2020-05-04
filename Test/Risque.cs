using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Risque
    {
        public string id { get; set; }
        public string libelle { get; set; }

        public Risque(string ligne)
        {
            List<string> Cata = new List<string>(ligne.Split(';')); ;

            id = Cata[3];

            libelle = Cata[4];
        }
    }
}
