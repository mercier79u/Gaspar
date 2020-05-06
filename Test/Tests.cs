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
        //public static void LireCata1()
        //{
        //    // Création du chronomètre.
        //    Stopwatch stopwatch = new Stopwatch();

        //    // Démarrage du chronomètre.
        //    stopwatch.Start();
        //    try
        //    {
        //        //StreamReader file = new StreamReader("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv");
        //        List<ArreteCata> ListeCata;
        //        var reader = new StreamReader("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv");
        //        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        //        csv.Configuration.HasHeaderRecord = true;
        //        csv.Configuration.Delimiter = ";";
        //        var records = csv.GetRecords<ArreteCata>();
        //        ListeCata = records.ToList();
        //        foreach (ArreteCata Cata in ListeCata)
        //        {
        //            //Cata.AfficheArreteCata();
        //        }

        //    }
        //    catch
        //    {
        //        Console.WriteLine("Le fichier ne peut pas être lu");
        //    }
        //    stopwatch.Stop();

        //    // IHM.
        //    Console.WriteLine("Durée d'exécution: {0}", stopwatch.Elapsed.TotalSeconds);
        //}

        //public static void LireCata2()
        //{
        //    // Création du chronomètre.
        //    Stopwatch stopwatch = new Stopwatch();

        //    // Démarrage du chronomètre.
        //    stopwatch.Start();
        //    try
        //    {
        //        List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
        //        ListLines.RemoveAt(0);
        //        foreach (string ligne in ListLines)
        //        {
                    
                   
        //        }
        //    }
        //    catch
        //    {
        //        Console.WriteLine("le fichier ne peut pas être lu");
        //    }
        //    stopwatch.Stop();

        //    // IHM.
        //    Console.WriteLine("Durée d'exécution: {0}", stopwatch.Elapsed.TotalSeconds);
        //}

        //Fonction dont le but est de remplir une table gespar une seule et unique fois avec le code insee, le code postal, et le nom de la commune
        public static void InitInseePostal(MySqlConnection connexion)
        {
            StringBuilder myRequest = new StringBuilder();
            MySqlCommand query;
            //Lecture du fichier, récupération d'une liste de lignes
            List<string> ListLines = new List<string>(System.IO.File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\correspondance-code-insee-code-postal.csv"));
            //Remove de la ligne 0 qui est l'entête du fichier excel avec les noms des colonnes Excel
            ListLines.RemoveAt(0);
            //Ligne par ligne on fait les traitements
            foreach (string ligne in ListLines)
            {   
                //Création d'un objet de correspondance InseePostal
                InseePostal newInseePostal = new InseePostal(ligne);

                myRequest.Append($"('{newInseePostal.codeInsee}','{newInseePostal.codePostal}','{newInseePostal.libCommune}'),");
            }

            myRequest.Insert(0, "INSERT INTO inseepostal VALUES");
            myRequest.Remove(myRequest.Length - 1, 1);

            Console.WriteLine(myRequest.ToString());
            query = new MySqlCommand(myRequest.ToString(), connexion);
            query.ExecuteNonQuery();
            myRequest.Clear();
        }

        //Fonction dont le but est de remplir notre base de donnée 
        public static void InitArreteCata(MySqlConnection connexion)
        {
            int count = 1;
            StringBuilder myRequest = new StringBuilder();
            MySqlCommand query;
            List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
            ListLines.RemoveAt(0);
            foreach (string ligne in ListLines)
            {
                ArreteCata Cata = new ArreteCata(ligne);

                if(Cata.dat_maj.HasValue)
                {
                    myRequest.Append($"('{Cata.cod_nat_catnat}','{Cata.cod_commune}','{Cata.num_risque_jo}', '{Cata.dat_deb.ToString("yyyy-mm-dd")}', '{Cata.dat_fin.ToString("yyyy-mm-dd")}','{Cata.dat_pub_arrete.ToString("yyyy-mm-dd")}', '{Cata.dat_maj.Value.ToString("yyyy-mm-dd")}'),");
                }
                else
                {
                    myRequest.Append($"('{Cata.cod_nat_catnat}','{Cata.cod_commune}','{Cata.num_risque_jo}', '{Cata.dat_deb.ToString("yyyy-mm-dd")}', '{Cata.dat_fin.ToString("yyyy-mm-dd")}','{Cata.dat_pub_arrete.ToString("yyyy-mm-dd")}', null ),");
                }
                
                if (count%5==0)
                {
                    myRequest.Insert(0, "INSERT INTO arretecata VALUES ");
                    myRequest.Remove(myRequest.Length-1, 1);

                    Console.WriteLine(myRequest.ToString());                 
                    query = new MySqlCommand(myRequest.ToString(), connexion);
                    query.ExecuteNonQuery();
                    myRequest.Clear();
                        
                }
                count++;
                    
            }
            myRequest.Insert(0, "INSERT INTO arretecata VALUES");
            myRequest.Remove(myRequest.Length - 1, 1);

            Console.WriteLine(myRequest.ToString());
            query = new MySqlCommand(myRequest.ToString(), connexion);
            query.ExecuteNonQuery();
            myRequest.Clear();
        }

        //Foncion qui sera la plus utilisée, permet de mettre à jour la base de donnée périodiquement
        public static void MajBDD(MySqlConnection connexion)
        {
            DateTime dateLastMaj = new DateTime(2020, 02, 01);
            //String builder pour la requête d'insertion
            StringBuilder myRequestInsert = new StringBuilder();
            //String builder pour la requête de mise à jour
            StringBuilder myRequestUpdate = new StringBuilder();
            MySqlCommand query;
            //Lecture du fichier et stockage dans une liste de lignes
            List<string> ListLines = new List<string>(File.ReadAllLines("C:\\Users\\Antoine\\Desktop\\catnat_guyane.csv"));
            //Remove de la première ligne qui correspond au nom des colonnes d'Excel
            ListLines.RemoveAt(0);
            //Traitement sur chaque ligne
            foreach (string ligne in ListLines)
            {
                //Création d'un objet Cata qui correspon à une ligne
                ArreteCata Cata = new ArreteCata(ligne);
                //Si la date de publication est postiérieure à la dernière date de maj de notre base de donnée, on insert la nouvelle ligne ( un insert seulement pour toutes les nouvelles lignes)
                if (Cata.dat_pub_arrete > dateLastMaj)
                {
                    if (Cata.dat_maj.HasValue)
                    {
                        myRequestInsert.Append($"('{Cata.cod_nat_catnat}','{Cata.cod_commune}','{Cata.num_risque_jo}', '{Cata.dat_deb.ToString("yyyy-mm-dd")}', '{Cata.dat_fin.ToString("yyyy-mm-dd")}','{Cata.dat_pub_arrete.ToString("yyyy-mm-dd")}', '{Cata.dat_maj.Value.ToString("yyyy-mm-dd")}'),");
                    }
                    else
                    {
                        myRequestInsert.Append($"('{Cata.cod_nat_catnat}','{Cata.cod_commune}','{Cata.num_risque_jo}', '{Cata.dat_deb.ToString("yyyy-mm-dd")}', '{Cata.dat_fin.ToString("yyyy-mm-dd")}','{Cata.dat_pub_arrete.ToString("yyyy-mm-dd")}', null ),");
                    }
                }
                //Si la date de maj est postiérieure à la dernière date de maj de notre base de donnée, on update la ligne ( un update par ligne)
                else if (Cata.dat_maj.HasValue && Cata.dat_maj > dateLastMaj)
                {
                    myRequestUpdate.Append($"UPDATE arretecata SET cod_insee='{Cata.cod_commune}',num_risque='{Cata.num_risque_jo}', date_deb='{Cata.dat_deb.ToString("yyyy-mm-dd")}', date_fin'{Cata.dat_fin.ToString("yyyy-mm-dd")}',date_pub_arrete='{Cata.dat_pub_arrete.ToString("yyyy-mm-dd")}', date_maj='{Cata.dat_maj.Value.ToString("yyyy-mm-dd")}' WHERE code_nat_cata='{Cata.cod_nat_catnat}'");
                    query = new MySqlCommand(myRequestUpdate.ToString(), connexion);
                    query.ExecuteNonQuery();
                    myRequestUpdate.Clear();
                }

            }
            //Insertion de toutes les lignes d'un coup
            myRequestInsert.Insert(0, "INSERT INTO arretecata VALUES ");
            myRequestInsert.Remove(myRequestInsert.Length - 1, 1);

            Console.WriteLine(myRequestInsert.ToString());
            query = new MySqlCommand(myRequestInsert.ToString(), connexion);
            query.ExecuteNonQuery();
            myRequestInsert.Clear();
        }


        //Fonction servant une unique fois, pour initialiser une potentielle liste GESPAR contenant le numéro du risque, et le libellé du risque
        public static void InitRisques(MySqlConnection connexion)
        {
            StringBuilder myRequest = new StringBuilder();
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
                    myRequest.Append($"('{Convert.ToInt32(risque.id)}','{risque.libelle}'),");
                    
                }
            }
            myRequest.Insert(0, "INSERT INTO libellerisque VALUES ");
            myRequest.Remove(myRequest.Length - 1, 1);
            query = new MySqlCommand(myRequest.ToString(), connexion);
            query.ExecuteNonQuery();
            query.Parameters.Clear();
        }
     
    }
}
