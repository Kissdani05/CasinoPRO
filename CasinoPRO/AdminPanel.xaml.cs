using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ZstdSharp.Unsafe;
using CasinoPRO.Models;
using System.Text.RegularExpressions;

namespace CasinoPRO
{
    public partial class AdminPanel : Window
    {
        public ObservableCollection<Profile> Profiles { get; set; }

        public AdminPanel()
        {
            InitializeComponent();
            
            LoadUsersFromDatabase();
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

        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            Button SaveBtn = sender as Button;
            StackPanel stk = SaveBtn.Parent as StackPanel;
            TextBlock IdTextB = stk.Children[0] as TextBlock;
            TextBox NameTextB = stk.Children[1] as TextBox;
            TextBox BalanceTextB = stk.Children[2] as TextBox;
            TextBox EmailTextB = stk.Children[3] as TextBox;
            TextBox JoinDateTextB = stk.Children[4] as TextBox;
            CheckBox IsActiveTextB = stk.Children[5] as CheckBox;
            TextBox RoleTextB = stk.Children[6] as TextBox;

            int bettorID = Convert.ToInt32(IdTextB.Text);
            string userName = NameTextB.Text;
            double userBalance = Convert.ToDouble(BalanceTextB.Text);  // Store the fetched balance
            string userEmail = EmailTextB.Text;
            DateTime joinDate = Convert.ToDateTime(JoinDateTextB.Text);
            bool isActive = Convert.ToBoolean(IsActiveTextB.IsChecked);
            string role = RoleTextB.Text;
            string loggedInUsername = SessionManager.LoggedInUsername; // Logged in username
            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                MySqlConnection conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "UPDATE Bettors SET Username = @userName, Email = @newEmail, Balance = @userBalance, IsActive = @isActive, Role = @role Where BettorsID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@newEmail", userEmail);
                    cmd.Parameters.AddWithValue("@userBalance", userBalance);
                    cmd.Parameters.AddWithValue("@isActive", isActive);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@id", bettorID);
                    cmd.ExecuteNonQuery();

                    SessionManager.LoggedInUsername = userName;

                    MessageBox.Show("User updated successfully!");

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
            Button SaveBtn = sender as Button;
            StackPanel stk = SaveBtn.Parent as StackPanel;
            TextBlock IdTextB = stk.Children[0] as TextBlock;
            int bettorID = Convert.ToInt32(IdTextB.Text);

            MySqlConnection conn = null;
            try
            {
                // Initialize database connection
                DatabaseConnection dbContext = new DatabaseConnection();
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Query to fetch user information
                    string query = "DELETE FROM Bettors Where BettorsID = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", bettorID);

                    cmd.ExecuteNonQuery();

                    Profile profileToRemove = Profiles.FirstOrDefault(x => x.Id == bettorID);

                    if (profileToRemove != null)
                    {
                        Profiles.Remove(profileToRemove);
                    }

                    MessageBox.Show("User deleted successfully!");
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
        }
        // Profilok gomb kezelése

        private void UjProfil_Click(object sender, EventArgs e)
        {
            NewProfileAdd newProfileAdd = new NewProfileAdd();
            newProfileAdd.Show();
        }
        private void Profilok_Click(object sender, RoutedEventArgs e)
        {
            ProfileGrid.Visibility = Visibility.Visible;
        }

        // Sportok gomb kezelése
        

        // Kijelentkezés
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
