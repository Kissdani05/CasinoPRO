using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ZstdSharp.Unsafe;

namespace CasinoPRO
{
    public partial class AdminPanel : Window
    {
        public class Profile
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Balance { get; set; }
            public string Email { get; set; }
            public DateTime JoinDate { get; set; }
            public bool IsActive { get; set; }
            public string Role { get; set; }
        }

        public class Bet
        {
            public string Match { get; set; }
            public decimal HomeOdds { get; set; }
            public decimal AwayOdds { get; set; }
            public decimal DrawOdds { get; set; }
        }

        public class Sport
        {
            public string Name { get; set; }
            public ObservableCollection<Bet> Bets { get; set; }

            public Sport(string name)
            {
                Name = name;
                Bets = new ObservableCollection<Bet>();
            }
        }

        public ObservableCollection<Sport> Sports { get; set; }
        public Sport SelectedSport { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; }

        public AdminPanel()
        {
            InitializeComponent();

            //Loading users
            LoadUsersFromDatabase();

            // Sportok és fogadások listájának létrehozása
            Sports = new ObservableCollection<Sport>
            {
                new Sport("Foci")
                {
                    Bets = new ObservableCollection<Bet>
                    {
                        new Bet { Match = "Csapat A vs Csapat B", HomeOdds = 1.50m, AwayOdds = 2.50m, DrawOdds = 3.00m },
                        new Bet { Match = "Csapat C vs Csapat D", HomeOdds = 1.75m, AwayOdds = 2.00m, DrawOdds = 3.50m }
                    }
                },
                new Sport("Kosárlabda")
                {
                    Bets = new ObservableCollection<Bet>
                    {
                        new Bet { Match = "Team A vs Team B", HomeOdds = 1.20m, AwayOdds = 3.50m, DrawOdds = 2.10m }
                    }
                },
                new Sport("Tenisz")
                {
                    Bets = new ObservableCollection<Bet>
                    {
                        new Bet { Match = "Player A vs Player B", HomeOdds = 1.30m, AwayOdds = 3.10m, DrawOdds = 0.00m }
                    }
                },
                new Sport("Hoki")
                {
                    Bets = new ObservableCollection<Bet>
                    {
                        new Bet { Match = "Team X vs Team Y", HomeOdds = 2.50m, AwayOdds = 1.80m, DrawOdds = 3.20m }
                    }
                }
            };

            // Alapértelmezett sport a Foci
            SelectedSport = Sports[0];

            // Adatkötés a Gridhez
            DataContext = this;
        }


        public void LoadUsersFromDatabase() 
        {
            MySqlConnection conn = null;
            int bettorID = 0;
            string userName = null;
            double userBalance = 0;  // Store the fetched balance
            string userEmail = null;
            DateTime joinDate = DateTime.MinValue;
            bool isActive = false;
            string role = null;
            string loggedInUsername = SessionManager.LoggedInUsername; // Logged in username
            Profiles = new();

            try
            {
                // Initialize database connection
                DatabaseConnection dbContext = new DatabaseConnection();
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Query to fetch user information
                    string query = "SELECT BettorsID, Username, Balance, Email, IsActive, JoinDate, Role FROM Bettors";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", loggedInUsername);

                    // Execute query and read data
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            bettorID = Convert.ToInt32(reader["BettorsID"]);
                            userName = reader["Username"].ToString();
                            userBalance = Convert.ToDouble(reader["Balance"]);  // Fetch balance from DB
                            userEmail = reader["Email"].ToString();
                            if (!reader.IsDBNull(reader.GetOrdinal("JoinDate")))
                            {
                                try
                                {
                                    joinDate = Convert.ToDateTime(reader["JoinDate"]);
                                }
                                catch (Exception innerEx)
                                {
                                    // Log the actual JoinDate value for debugging
                                    string rawJoinDate = reader["JoinDate"].ToString();
                                    MessageBox.Show($"Error parsing JoinDate: {rawJoinDate}. Exception: {innerEx.Message}");

                                    // Use TryParse for manual parsing as a fallback
                                    if (DateTime.TryParse(rawJoinDate, out DateTime parsedDate))
                                    {
                                        joinDate = parsedDate;
                                    }
                                    else
                                    {
                                        joinDate = DateTime.MinValue; // Fallback to a default value
                                    }
                                }
                            }
                            else
                            {
                                joinDate = DateTime.MinValue; // Default if JoinDate is null
                            }
                            isActive = Convert.ToBoolean(reader["IsActive"]);
                            role = reader["Role"].ToString();

                            // Add the profile to the Profiles collection
                            Profiles.Add(new Profile
                            {
                                Id = bettorID,
                                Name = userName,
                                Balance = userBalance,
                                Email = userEmail,
                                JoinDate = joinDate,
                                IsActive = isActive,
                                Role = role
                            });
                        }
                    }
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
        }

        private void SaveUser_Click()
        {
            int bettorID = 0;
            string userName = null;
            double userBalance = 0;  // Store the fetched balance
            string userEmail = null;
            bool isActive = false;
            string role = null;
            string loggedInUsername = SessionManager.LoggedInUsername; // Logged in username
            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                MySqlConnection conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Update the username and email in the database
                    string query = "UPDATE Bettors SET Username = @userName, Email = @newEmail, UserBalance = @userBalance, IsActive = @isActive, Role = @role";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@newEmail", userEmail);
                    cmd.Parameters.AddWithValue("@oldUsername", SessionManager.LoggedInUsername);
                    cmd.ExecuteNonQuery();

                    // Update SessionManager with the new username
                    SessionManager.LoggedInUsername = userName;

                    // Refresh user balance or other data if needed


                    // Update the dynamically generated username label (TextBlock)

                }

                dbContext.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user data: " + ex.Message);
            }

        }

        private void DeleteUser_Click(object sender,RoutedEventArgs e)
        {

        }
        // Profilok gomb kezelése
        private void Profilok_Click(object sender, RoutedEventArgs e)
        {
            SportsGrid.Visibility = Visibility.Collapsed;
            ProfileGrid.Visibility = Visibility.Visible;
        }

        // Sportok gomb kezelése
        private void Sportok_Click(object sender, RoutedEventArgs e)
        {
            ProfileGrid.Visibility = Visibility.Collapsed;
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
            MessageBox.Show("A fogadás mentve!");
        }

        // Fogadás törlése
        private void DeleteBet_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var bet = button.DataContext as Bet;

            if (bet != null && SelectedSport != null)
            {
                SelectedSport.Bets.Remove(bet);
            }
        }
    }
}
