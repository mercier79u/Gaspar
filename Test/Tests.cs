using CsvHelper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Tests
    {
        public static void LireCata1()
        {
            // Création du chronomètre.
            Stopwatch stopwatch = new Stopwatch();

            // Démarrage du chronomètre.
            stopwatch.Start();
            try
            {
                //StreamReader file = new StreamReader("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv");
                List<ArreteCata> ListeCata;
                var reader = new StreamReader("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv");
                var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                csv.Configuration.HasHeaderRecord = true;
                csv.Configuration.Delimiter = ";";
                var records = csv.GetRecords<ArreteCata>();
                ListeCata = records.ToList();
                foreach (ArreteCata Cata in ListeCata)
                {
                    //Cata.AfficheArreteCata();
                }

            }
            catch
            {
                Console.WriteLine("Le fichier ne peut pas être lu");
            }
            stopwatch.Stop();

            // IHM.
            Console.WriteLine("Durée d'exécution: {0}", stopwatch.Elapsed.TotalSeconds);
        }

        public static void LireCata2()
        {
            // Création du chronomètre.
            Stopwatch stopwatch = new Stopwatch();

            // Démarrage du chronomètre.
            stopwatch.Start();
            try
            {
                List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
                ListLines.RemoveAt(0);
                foreach (string ligne in ListLines)
                {
                    
                   
                }
            }
            catch
            {
                Console.WriteLine("le fichier ne peut pas être lu");
            }
            stopwatch.Stop();

            // IHM.
            Console.WriteLine("Durée d'exécution: {0}", stopwatch.Elapsed.TotalSeconds);
        }
        public static void InitInseePostal(MySqlConnection connexion)
        {
            // Création du chronomètre.
            Stopwatch stopwatch = new Stopwatch();

            // Démarrage du chronomètre.
            stopwatch.Start();
            try
            {
                //Lecture du fichier, récupération d'une liste de lignes
                List<string> ListLines = new List<string>(System.IO.File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\correspondance-code-insee-code-postal.csv"));
                //Remove de la ligne 0 qui est l'entête du fichier excel avec les noms des colonnes Excel
                ListLines.RemoveAt(0);
                //Ligne par ligne on fait les traitements
                foreach (string ligne in ListLines)
                {   
                    //Création d'un objet de correspondance InseePostal
                    InseePostal newInseePostal = new InseePostal(ligne);

                    //Création requête sql avec passage de paramètre
                    MySqlCommand query = new MySqlCommand("INSERT INTO inseepostal VALUES (@CodeInsee, @CodePostal,@LibCommune)", connexion);
                    query.Parameters.AddWithValue("@CodeInsee", newInseePostal.codeInsee);
                    query.Parameters.AddWithValue("@CodePostal", newInseePostal.codePostal);
                    query.Parameters.AddWithValue("@LibCommune", newInseePostal.libCommune);
                    //Exécution de la requête
                    query.ExecuteNonQuery();
                    //Clear des paramètres
                    query.Parameters.Clear();
                }
            }
            catch
            {
                Console.WriteLine("le fichier ne peut pas être lu\n LE FICHIER EST-IL FERME ?");
            }
            stopwatch.Stop();

            Console.WriteLine("Durée d'exécution: {0}", stopwatch.Elapsed.TotalSeconds);
        }

        public static void InitArreteCata(MySqlConnection connexion)
        {
            int count = 1;
            StringBuilder myRequest = new StringBuilder();
            string DateMaj = null;
            MySqlCommand query;
            //try
            //{
                List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
                ListLines.RemoveAt(0);
                foreach (string ligne in ListLines)
                {
                    ArreteCata Cata = new ArreteCata(ligne);

                    if (String.IsNullOrEmpty(Cata.dat_maj))
                    {
                        DateMaj = null;
                    }
                    else
                    {
                        DateMaj = null;
                    }
                    //myRequest.Append(("({0},{1},{2},{3},{4},{5},{6}),", Cata.cod_nat_catnat,
                    //                                                    Cata.cod_commune,
                    //                                                    Cata.num_risque_jo,
                    //                                                    DateTime.Parse(Cata.dat_deb),
                    //                                                    DateTime.Parse(Cata.dat_fin),
                    //                                                    DateTime.Parse(Cata.dat_pub_arrete),
                    //                                                    DateMaj));
                myRequest.Append($"({Cata.cod_nat_catnat},{Cata.cod_commune},{Cata.num_risque_jo},{DateTime.Parse(Cata.dat_deb)},{DateTime.Parse(Cata.dat_fin)},{DateTime.Parse(Cata.dat_pub_arrete)},),");
                if (count%5==0)
                    {
                    myRequest.Insert(0, "INSERT INTO arretecata VALUES ");
                    Console.WriteLine(myRequest.Remove(myRequest.Length-1, 1));

                    Console.WriteLine(myRequest.ToString());                 
                        query = new MySqlCommand(myRequest.ToString(), connexion);
                        query.ExecuteNonQuery();
                        myRequest.Clear();
                    }
                count++;
                    
                }
                //myRequest.Insert(0, "INSERT INTO arretecata VALUES");
                //myRequest.Append(";");
                //query = new MySqlCommand(myRequest.ToString(), connexion);
                //query.ExecuteNonQuery();
                //myRequest.Clear();
            //}
            //catch
            //{
                Console.WriteLine("le fichier ne peut pas être lu");
            //}
        }

        //Fonction servant une unique fois, pour initialiser une potentielle liste GESPAR
        public static void InitRisques(MySqlConnection connexion)
        {
            //Création d'un dictionnaire destiné à contenir les risques une seule fois
            Dictionary<string, string> dicoRisques = new Dictionary<string, string>();
            MySqlCommand query;
            //Lecture du fichier, et récupération en une liste de ligne
            List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
            //Remove de la première ligne, qui correspond au nom des colonnes Excel 
            ListLines.RemoveAt(0);
            //Traitement ligne par ligne
            foreach (string ligne in ListLines)
            {
                //Création nouvel objet risque
                Risque risque = new Risque(ligne);
                //Si notre dictionnaire ne connait pas ce risque, on l'insert dans notre base de données et à notre dictionnaire, sinon on passe à la ligne suivante
                if (!dicoRisques.ContainsKey(risque.id))
                {
                    dicoRisques.Add(risque.id, risque.libelle);
                    query = new MySqlCommand("INSERT INTO libellerisque VALUES (@id, @lib_risque)", connexion);
                    query.Parameters.AddWithValue("@id", Convert.ToInt32(risque.id));
                    query.Parameters.AddWithValue("@lib_risque", risque.libelle);
                    query.ExecuteNonQuery();
                    query.Parameters.Clear();
                }
            }
        }
        public static void MajBDD(MySqlConnection connexion)
        {
            DateTime dateLastMaj = new DateTime(2020,02,01);
            DateTime datePub = new DateTime();
            DateTime dateMaj = new DateTime();
            MySqlCommand query;


            List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
            ListLines.RemoveAt(0);
            foreach (string ligne in ListLines)
            {
                List<string> ligneSplit = new List<string>(ligne.Split(';'));
                datePub = DateTime.Parse(ligne[7].ToString());
                dateMaj = DateTime.Parse(ligne[9].ToString());
                if (datePub>dateLastMaj)
                {
                    query = new MySqlCommand("INSERT INTO arretecata VALUES (@code_nat_catnat, @cod_insee,@num_risque, @dat_deb, @dat_fin, @dat_pub_arrete,@dat_maj)", connexion);
                    query.Parameters.AddWithValue("@code_nat_cata", ligneSplit[0]);
                    query.Parameters.AddWithValue("@cod_insee", ligneSplit[1]);
                    query.Parameters.AddWithValue("@num_risque", ligneSplit[3]);
                    query.Parameters.AddWithValue("@dat_deb", ligneSplit[5]);
                    query.Parameters.AddWithValue("@dat_fin", ligneSplit[6]);
                    query.Parameters.AddWithValue("@dat_pub_arrete", ligneSplit[7]);
                    query.Parameters.AddWithValue("@dat_maj", ligneSplit[9]);
                    query.ExecuteNonQuery();
                    query.Parameters.Clear();
                }
                else if(dateMaj > dateLastMaj)
                {
                    query = new MySqlCommand("UPDATE arretecata SET cod_insee=@cod_insee, num_risque=@num_risque, date_deb=@date_deb, date_fin=@date_fin, date_pub_arrete=@date_pub_arrete,date_maj=@date_maj " +
                        "WHERE code_nat_cata=@code_nat_cata)", connexion);
                    query.Parameters.AddWithValue("@code_nat_cata", ligneSplit[0]);
                    query.Parameters.AddWithValue("@cod_insee", ligneSplit[1]);
                    query.Parameters.AddWithValue("@num_risque", ligneSplit[3]);
                    query.Parameters.AddWithValue("@date_deb", ligneSplit[5]);
                    query.Parameters.AddWithValue("@date_fin", ligneSplit[6]);
                    query.Parameters.AddWithValue("@date_pub_arrete", ligneSplit[7]);
                    query.Parameters.AddWithValue("@date_maj", ligneSplit[9]);
                    query.ExecuteNonQuery();
                    query.Parameters.Clear();
                }
            }
        }



        public static void fonctiondemerde(MySqlConnection connexion)
        {
            List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
            ListLines.RemoveAt(0);
            foreach (string ligne in ListLines)
            {

                List<string> ligneSplit = new List<string>(ligne.Split(';'));

                //newArreteCata.DeclareArreteCata(CataSplit);
                MySqlCommand query = new MySqlCommand("INSERT INTO arretecata VALUES (@code_nat_cata, @cod_insee,@num_risque, @date_deb, @date_fin, @date_pub_arrete, @date_maj)", connexion);
                query.Parameters.AddWithValue("@code_nat_cata", ligneSplit[0]);
                query.Parameters.AddWithValue("@cod_insee", ligneSplit[1]);
                query.Parameters.AddWithValue("@num_risque", ligneSplit[3]);
                query.Parameters.AddWithValue("@date_deb", DateTime.Parse(ligneSplit[5]));
                query.Parameters.AddWithValue("@date_fin", DateTime.Parse(ligneSplit[6]));
                query.Parameters.AddWithValue("@date_pub_arrete", DateTime.Parse(ligneSplit[7]));
                if (String.IsNullOrEmpty(ligneSplit[9]))
                {
                    query.Parameters.AddWithValue("@date_maj", null);
                }
                else
                {
                    query.Parameters.AddWithValue("@date_maj", ligneSplit[9]);
                }
                query.ExecuteNonQuery();
                query.Parameters.Clear();
            }
        }
    }
}
