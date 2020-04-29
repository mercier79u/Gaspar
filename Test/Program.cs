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

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {           

            MySqlConnection connexion = new MySqlConnection("DataBase=cat_nat; Server=localhost; User Id=root; PassWord=root");
           

            try
            {
                connexion.Open();
                Console.WriteLine("connexion établie");
            }
            catch
            {
                Console.WriteLine("connexion échouée");
            }
            Console.ReadKey();
        }
    }
}
