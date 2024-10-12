using CasinoPRO.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CasinoPRO
{
    /// <summary>
    /// Interaction logic for NewMatch.xaml
    /// </summary>
    public partial class NewMatch : Window
    {
        private DatabaseConnection dbContext;
        public Matches NewMatches { get; set; }
        public NewMatch()
        {
            InitializeComponent();
            dbContext = new DatabaseConnection();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = null;
            bool isRegistered = false;

            int eventId = 0;
            string EventName = txtEventname.Text;
            DateTime EventDate = Convert.ToDateTime(txtDate.Text);
            string Category = txtCategory.Text;
            string location = txtLocation.Text;
            try
            {
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string insertQuery = "INSERT INTO Events (EventName, EventDate, Category, Location) VALUES (@eventname, @eventdate, @category, @location)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@eventname", EventName);
                    insertCmd.Parameters.AddWithValue("@eventdate", EventDate);
                    insertCmd.Parameters.AddWithValue("@category", Category);
                    insertCmd.Parameters.AddWithValue("@location", location);

                    int result = insertCmd.ExecuteNonQuery();
                    isRegistered = result > 0;

                    string selectQuery = "SELECT EventID FROM Events WHERE EventName = @eventname ";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@eventname", EventName);
                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            eventId = Convert.ToInt32(reader["EventID"]);
                        }
                    }
                }
                    NewMatches = new Matches
                    {
                        EventId = eventId,
                        EventName = EventName,
                        EventDate = EventDate,
                        Category = Category,
                        Location = location,
                    };
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Új eseménye nem lehetséges: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    dbContext.CloseConnection();
                }
            }
            if (isRegistered)
            {
                MessageBox.Show("Új esemény hozzáadva!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Új esemény hozzáadása sikertelen.");
            }
        }
    }
}
