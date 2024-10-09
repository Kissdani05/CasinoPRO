using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoPRO
{
    internal class DatabaseConnection
    {
        private string connectionString;
        private MySqlConnection connection;

        public DatabaseConnection()
        {
            // Initialize the connection string (you can also store this in a config file)
            connectionString = "Server=localhost;Database=Bets; Uid=root;Pwd=;Port=3306";
            connection = new MySqlConnection(connectionString);
        }

        // Open the connection
        public MySqlConnection OpenConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
            }
            catch (MySqlException ex)
            {
                // Handle exceptions (log the error or show a message to the user)
                Console.WriteLine("Error: " + ex.Message);
            }

            return connection;
        }

        // Close the connection
        public void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        // Get the connection for usage in other classes
        public MySqlConnection GetConnection()
        {
            return connection;
        }
    }
}
