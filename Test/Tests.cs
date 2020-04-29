using CsvHelper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
                    ArreteCata newArreteCata = new ArreteCata();
                    List<string> CataSplit = new List<string>(ligne.Split(';'));
                    newArreteCata.DeclareArreteCata(CataSplit);
                    //newArreteCata.AfficheArreteCata();

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
                List<string> ListLines = new List<string>(System.IO.File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\correspondance-code-insee-code-postal.csv"));
                ListLines.RemoveAt(0);
                foreach (string ligne in ListLines)
                {
                    InseePostal newInseePostal = new InseePostal();
                    List<string> InseePostalSplit = new List<string>(ligne.Split(';'));
                    newInseePostal.DeclareInseePostal(InseePostalSplit);

                    MySqlCommand query = new MySqlCommand("INSERT INTO inseepostal VALUES (@CodeInsee, @CodePostal,@LibCommune)", connexion);
                    query.Parameters.AddWithValue("@CodeInsee", newInseePostal.codeInsee);
                    query.Parameters.AddWithValue("@CodePostal", newInseePostal.codePostal);
                    query.Parameters.AddWithValue("@LibCommune", newInseePostal.libCommune);
                    query.ExecuteNonQuery();
                    query.Parameters.Clear();
                }
            }
            catch
            {
                Console.WriteLine("le fichier ne peut pas être lu\n LE FICHIER EST-IL FERME ?");
            }
            stopwatch.Stop();

            // IHM.
            Console.WriteLine("Durée d'exécution: {0}", stopwatch.Elapsed.TotalSeconds);
        }

        public static void InitArreteCata(MySqlConnection connexion)
        {
            try
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
            catch
            {
                Console.WriteLine("le fichier ne peut pas être lu");
            }
        }

        public static void InitRisques(MySqlConnection connexion)
        {
            Dictionary<string, string> dicoRisques = new Dictionary<string, string>();
            MySqlCommand query;

            List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
            ListLines.RemoveAt(0);
            foreach (string ligne in ListLines)
            {
                List<string> ligneSplit = new List<string>(ligne.Split(';'));

                if (!dicoRisques.ContainsKey(ligneSplit[3]))
                {
                    dicoRisques.Add(ligneSplit[3], ligneSplit[4]);
                    query = new MySqlCommand("INSERT INTO libellerisque VALUES (@id, @lib_risque)", connexion);
                    query.Parameters.AddWithValue("@id", Convert.ToInt32(ligneSplit[3]));
                    query.Parameters.AddWithValue("@lib_risque", ligneSplit[4]);
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
    }
}
