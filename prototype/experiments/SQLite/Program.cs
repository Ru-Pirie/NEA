using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string databaseName = "URI=file:planmty.db";
            var con = new SQLiteConnection(databaseName);
            con.Open();

            new SQLiteCommand(@"CREATE TABLE IF NOT EXISTS Names ( 
                name varchar(255) not null primary key
            );", con).ExecuteScalar();

            //new SQLiteCommand("INSERT INTO Names (name) VALUES('jannet')", con).ExecuteNonQuery();
            //new SQLiteCommand("INSERT INTO Names (name) VALUES('fob')", con).ExecuteNonQuery();
            //new SQLiteCommand("INSERT INTO Names (name) VALUES('eob')", con).ExecuteNonQuery();
            //new SQLiteCommand("INSERT INTO Names (name) VALUES('gob')", con).ExecuteNonQuery();

            var command = new SQLiteCommand("SELECT * FROM Names", con);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader[0]);
                }
            }
            Console.ReadLine();
        }
    }
}
