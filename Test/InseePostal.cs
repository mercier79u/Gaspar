using Renci.SshNet.Security.Cryptography.Ciphers.Modes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class InseePostal
    {
        public string codeInsee { get; set; }
        public string codePostal { get; set; }
        public string libCommune { get; set; }

        public InseePostal(string ligne)
        {
            List<string> Insee = new List<string>(ligne.Split(';')); ;

            codeInsee = Insee[0];

            codePostal = Insee[1];

            libCommune = Insee[2];

        }   
        
        public void AfficheArreteCata()
        {
            Console.WriteLine($"{codeInsee}\t{codePostal}\t{libCommune}");
        }
    }
}
