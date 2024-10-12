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
using CasinoPRO.Models;
using System.Text.RegularExpressions;
namespace CasinoPRO
{
    /// <summary>
    /// Interaction logic for OrganizerPage.xaml
    /// </summary>
    public partial class OrganizerPage : Window
    {
        public ObservableCollection<Sport> Sports { get; set; }
        public Sport SelectedSport { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; }
        public ObservableCollection<Matches> Match { get; set; }

        public OrganizerPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadEvents();

        }

        public void LoadEvents()
        {
            MySqlConnection conn = null;
            int eventId = 0;
            string eventName = null;
            DateTime eventDate = DateTime.MinValue;
            string category = null;
            string location = null;
            Match = new ObservableCollection<Matches>();
            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT EventID, EventName, EventDate, Category, Location FROM events";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Retrieve data from each column
                            eventId = Convert.ToInt32(reader["EventID"]);
                            eventName = reader["EventName"].ToString();
                            category = reader["Category"].ToString();
                            location = reader["Location"].ToString();

                            // Handle nullable EventDate
                            if (!reader.IsDBNull(reader.GetOrdinal("EventDate")))
                            {

                                try
                                {
                                    eventDate = Convert.ToDateTime(reader["EventDate"]);
                                }
                                catch (Exception innerEx)
                                {
                                    // Log the actual JoinDate value for debugging
                                    string rawJoinDate = reader["EventDate"].ToString();
                                    MessageBox.Show($"Error parsing JoinDate: {rawJoinDate}. Exception: {innerEx.Message}");

                                    // Use TryParse for manual parsing as a fallback
                                    if (DateTime.TryParse(rawJoinDate, out DateTime parsedDate))
                                    {
                                        eventDate = parsedDate;
                                    }
                                    else
                                    {
                                        eventDate = DateTime.MinValue; // Fallback to a default value
                                    }
                                }
                            }
                            else
                            {
                                eventDate = DateTime.MinValue;  // Default value
                            }

                            // Add the event data to the Match collection
                            Match.Add(new Matches
                            {
                                EventId = eventId,
                                EventName = eventName,
                                EventDate = eventDate,
                                Category = category,
                                Location = location
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading events: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed after use
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        private void Sportok_Click(object sender, RoutedEventArgs e)
        {
            SportsGrid.Visibility = Visibility.Visible;
        }

        // Kijelentkezés
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Új sport hozzáadása
        private void AddNewSport_Click(object sender, RoutedEventArgs e)
        {
            string sportName = Microsoft.VisualBasic.Interaction.InputBox("Add meg az új sport nevét:", "Új sport hozzáadása", "Új sport");

            if (!string.IsNullOrWhiteSpace(sportName))
            {
                var newSport = new Sport(sportName);
                Sports.Add(newSport);
                SelectedSport = newSport;
            }
        }

        // Új fogadás hozzáadása a kiválasztott sporthoz
        private void AddNewBet_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSport != null)
            {
                SelectedSport.Bets.Add(new Bet { Match = "Új valaki vs valaki", HomeOdds = 1.00m, AwayOdds = 2.00m, DrawOdds = 3.00m });
            }
        }

        // Sport kiválasztása
        private void SportSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedSport = comboBox.SelectedItem as Sport;

            if (selectedSport != null)
            {
                SelectedSport = selectedSport;
            }
        }

        // Fogadás mentése
        private void SaveBet_Click(object sender, RoutedEventArgs e)
        {
            Button SaveBtn = sender as Button;
            StackPanel stk = SaveBtn.Parent as StackPanel;

            TextBlock EventIdTextB = stk.Children[0] as TextBlock;
            TextBox EventNameTextB = stk.Children[1] as TextBox;
            TextBox EventDateTextB = stk.Children[2] as TextBox;
            TextBox CategoryTextB = stk.Children[3] as TextBox;
            TextBox LocationTextB = stk.Children[4] as TextBox;

            int eventId = Convert.ToInt32(EventIdTextB.Text);
            string eventName = EventNameTextB.Text;
            DateTime eventDate = Convert.ToDateTime(EventDateTextB.Text);
            string category = CategoryTextB.Text;  // Store the fetched balance
            string location = LocationTextB.Text;

            DatabaseConnection dbContext = new DatabaseConnection();
            MySqlConnection conn = dbContext.OpenConnection();

            try
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    

                    string query = "UPDATE Events SET EventName = @eventname, EventDate = @eventdate, Category = @category, Location = @location WHERE EventID = @eventid";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@eventname", eventName);
                    cmd.Parameters.AddWithValue("@eventdate", eventDate);
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.Parameters.AddWithValue("@location", location);
                    cmd.Parameters.AddWithValue("@eventid", eventId);

                    cmd.ExecuteNonQuery();


                    MessageBox.Show("Esemény módosítva!");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching user data: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        // Fogadás törlése
        private void DeleteBet_Click(object sender, RoutedEventArgs e)
        {
            Button DeleteBtn = sender as Button;
            StackPanel stk = DeleteBtn.Parent as StackPanel;
            TextBlock IdTextB = stk.Children[0] as TextBlock;
            int eventID = Convert.ToInt32(IdTextB.Text);

            MySqlConnection conn = null;
            try
            {
                // Initialize database connection
                DatabaseConnection dbContext = new DatabaseConnection();
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Query to fetch user information
                    string deleteQuery = "DELETE FROM Events WHERE EventID = @id";
                    MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@id", eventID);
                    deleteCmd.ExecuteNonQuery();

                    string resetProcedureQuery = "CALL reset_auto_increment_bettors();";
                    MySqlCommand resetCmd = new MySqlCommand(resetProcedureQuery, conn);
                    resetCmd.ExecuteNonQuery();

                    Matches MatchToRemove = Match.FirstOrDefault(x => x.EventId == eventID);

                    if (MatchToRemove != null)
                    {
                        Match.Remove(MatchToRemove);
                    }

                    MessageBox.Show("Esemény sikeresen törölve!");
                    // Execute query and read data

                }
            }
            catch (Exception ex)
            {
                // Catch and display any general errors during fetching
                MessageBox.Show("Error fetching user data: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed after use
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }


            var button = sender as Button;
            var bet = button.DataContext as Bet;

            if (bet != null && SelectedSport != null)
            {
                SelectedSport.Bets.Remove(bet);
            }
        }
    }
}
