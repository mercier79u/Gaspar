using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Net;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {           

            //MySqlConnection connexion = new MySqlConnection("DataBase=cat_nat; Server=localhost; User Id=root; PassWord=root");
            //connexion.Open();
            //Initialisation de la table gespar codeInseePostal une seule fois
            //Tests.InitInseePostal(connexion);
            //Initialisation de la table gespar risque une seule fois
            //Tests.InitInseePostal(connexion);
            //Initialisation de la base de données contenant les arrêtés une seule fois
            //Tests.InitInseePostal(connexion);

            //Mise à jour de la base de données contenant les arrêtés de façon périodique
            //Tests.InitInseePostal(connexion);


            //Exemple de téléchargement d'un fichier depuis un lien, et l'enregistre avec un nom donné
            //WebClient webClient = new WebClient();
            //webClient.DownloadFile("https://files.georisques.fr/GASPAR/CATNAT/catnat_mayotte.csv", @"C:\Users\Antoine\Desktop\toto.csv");

            Console.ReadKey();
        }
    }
}
