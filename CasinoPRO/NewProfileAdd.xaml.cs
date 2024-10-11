using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
using System.Data;

namespace CasinoPRO
{
    /// <summary>
    /// Interaction logic for NewProfileAdd.xaml
    /// </summary>
    public partial class NewProfileAdd : Window
    {
        public string[] Roles = ["User", "Admin", "Organizer"];
        private DatabaseConnection dbContext;
        public Profile NewProfile { get; set; }
        public NewProfileAdd()
        {
            InitializeComponent();
            FillRoleCb();
            dbContext = new DatabaseConnection();
        }

        public void FillRoleCb()
        {
            foreach (var role in Roles)
            {
                RoleComboBox.Items.Add(role);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = null;
            bool isRegistered = false;

            int bettorId = 0;
            string userName = RegisterUsernameTextBox.Text;
            string userPassword = RegisterPasswordTextBox.Text;
            int userBalance = Convert.ToInt32(EgyenlegTextBox.Text);
            string userEmail = RegisterEmailTextBox.Text;
            DateTime joinDate = DateTime.Now;
            string role = RoleComboBox.SelectedItem.ToString();
            try
            {
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string insertQuery = "INSERT INTO Bettors (Username, Password, Balance, Email, JoinDate, IsActive, Role) VALUES (@username, @password, @balance, @email, @joinDate, 1, @role)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@username", userName);
                    insertCmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(userPassword));
                    insertCmd.Parameters.AddWithValue("@balance", userBalance);
                    insertCmd.Parameters.AddWithValue("@Email", userEmail);
                    insertCmd.Parameters.AddWithValue("@joinDate", joinDate);
                    insertCmd.Parameters.AddWithValue("@role", role);

                    int result = insertCmd.ExecuteNonQuery();
                    isRegistered = result > 0;

                    string selectQuery = "SELECT BettorsID FROM Bettors WHERE Username = @username ";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@username", userName);
                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bettorId = Convert.ToInt32(reader["BettorsID"]);
                        }
                    }

                }
                NewProfile = new Profile
                {

                    Id = bettorId,
                    Name = userName,
                    Balance = userBalance,
                    Email = userEmail,
                    JoinDate = joinDate,
                    IsActive = true,
                    Role = role
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during registration: " + ex.Message);
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
                MessageBox.Show("Registration successful!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed.");
            }

        }
    }
}
