using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class ArreteCata
    {
        [Index(0)]
        public string cod_nat_catnat { get; set; }
        [Index(1)]
        public string cod_commune { get; set; }
        [Index(2)]
        public string lib_commune { get; set; }
        [Index(3)]
        public string num_risque_jo { get; set; }
        [Index(4)]
        public string lib_risque_jo { get; set; }
        [Index(5)]
        public string dat_deb { get; set; }
        [Index(6)]
        public string dat_fin { get; set; }
        [Index(7)]
        public string dat_pub_arrete { get; set; }
        [Index(8)]
        public string dat_pub_jo { get; set; }
        [Index(9)]
        public string dat_maj { get; set; }

        public ArreteCata(string ligne)
        {
            List<string> Cata = new List<string>(ligne.Split(';'));

            cod_nat_catnat = Cata[0];

            cod_commune = Cata[1];

            lib_commune = Cata[2];

            num_risque_jo = Cata[3];

            lib_risque_jo = Cata[4];

            dat_deb = Cata[5];

            dat_fin = Cata[6];

            dat_pub_arrete = Cata[7];

            dat_pub_jo = Cata[8];

            dat_maj = Cata[9];
        }
        public void AfficheArreteCata()
        {
            Console.WriteLine($"{cod_nat_catnat}\t{lib_commune}\t{dat_deb}");
        }
    }
}
