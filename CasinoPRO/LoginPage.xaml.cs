using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace CasinoPRO
{
    public partial class LoginPage : Window
    {
        private string registeredUsername = string.Empty;
        private DatabaseConnection dbContext;
        public double UserBalance { get; private set; }
        public LoginPage()
        {
            InitializeComponent();
            dbContext = new DatabaseConnection();
        }



        public bool ValidateLogin(string username, string password)
        {
            bool isValid = false;
            MySqlConnection conn = null;

            try
            {
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT Password FROM Bettors WHERE Username = @username AND IsActive = 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    string storedHash = null;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            storedHash = reader["Password"].ToString();
                        }
                    }

                    if (storedHash != null && BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        isValid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during login validation: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    dbContext.CloseConnection();
                }
            }

            return isValid;
        }
        public bool ValidateAdmin(string username)
        {
            bool isAdmin = false;

            MySqlConnection conn = null;

            try
            {
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT Role FROM Bettors WHERE Username = @username AND IsActive = 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    string role = null;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = reader["Role"].ToString();
                        }
                    }

                    if (role.ToLower() == "admin")
                    {
                        isAdmin = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during login validation: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    dbContext.CloseConnection();
                }
            }

            return isAdmin;
        }
        public bool ValidateOrganizer(string username)
        {
            bool isOrganizer = false;

            MySqlConnection conn = null;

            try
            {
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT Role FROM Bettors WHERE Username = @username AND IsActive = 1";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    string role = null;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = reader["Role"].ToString();
                        }
                    }

                    if (role.ToLower() == "organizer")
                    {
                        isOrganizer = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during login validation: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    dbContext.CloseConnection();
                }
            }

            return isOrganizer;
        }

        private void LoadUserBalance(string username)
        {
            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                MySqlConnection conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT Balance FROM Bettors WHERE Username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Store the balance in UserBalance
                            UserBalance = Convert.ToDouble(reader["Balance"]);
                        }
                    }
                }
                dbContext.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading balance: " + ex.Message);
            }
        }
        // Bejelentkezés gomb eseménykezelő
        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            // Call ValidateLogin to check if the login is valid
            bool isValid = ValidateLogin(username, password);
            bool isAdmin = ValidateAdmin(username);
            bool isOrganizer = ValidateOrganizer(username);

            if (isValid)
            {
                if (isAdmin)
                {
                    SessionManager.LoggedInUsername = username;
                    MessageBox.Show("Login successful!");

                    AdminPanel adminPanel = new AdminPanel();
                    adminPanel.Show();

                    var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    if (mainWindow != null)
                    {
                        mainWindow.Close();
                    }

                    this.DialogResult = true;
                    this.Close();
                }
                else if (isOrganizer)
                {
                    SessionManager.LoggedInUsername = username;
                    MessageBox.Show("Login successful!");

                    OrganizerPage organizerPage = new OrganizerPage();
                    organizerPage.Show();

                    var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    if (mainWindow != null)
                    {
                        mainWindow.Close();
                    }

                    this.DialogResult = true;
                    this.Close();
                }
                else
                {      
                    // If login is successful, show success message
                    SessionManager.LoggedInUsername = username;
                    MessageBox.Show("Login successful!");

                    // Load the user balance
                    LoadUserBalance(username);

                    // Set DialogResult to true to signal successful login
                    this.DialogResult = true;

                    // Close the login window and pass control back to MainWindow
                    this.Close();
                }
            }
            else
            {
                // If login fails, show failure message
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

        // Elfelejtett jelszó link eseménykezelő
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Jelszó visszaállítás funkció hamarosan..."); 
        }
        // Regisztráció link eseménykezelő (váltás a regisztrációs felületre)
        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            LoginGrid.Visibility = Visibility.Collapsed;
            RegistrationGrid.Visibility = Visibility.Visible;
        }

        // Vissza a bejelentkezéshez link eseménykezelő
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            RegistrationGrid.Visibility = Visibility.Collapsed;
            LoginGrid.Visibility = Visibility.Visible;
        }

        // Regisztráció gomb eseménykezelő
        public void RegisterUser(string username, string email, string password)
        {
            bool isRegistered = false;
            MySqlConnection conn = null;
            DateTime joinDate = DateTime.Now;

            try
            {
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "INSERT INTO Bettors (Username, Email, Password, IsActive, JoinDate) VALUES (@username, @Email, @Password, 1, @joinDate)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(password));
                    cmd.Parameters.AddWithValue("@joinDate", joinDate);

                    int result = cmd.ExecuteNonQuery();
                    isRegistered = result > 0;
                }
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
            }
            else
            {
                MessageBox.Show("Registration failed.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Get input values from the UI
            string username = RegisterUsernameTextBox.Text;
            string email = RegisterEmailTextBox.Text;
            string confirmEmail = ConfirmEmailTextBox.Text;
            string password = RegisterPasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;

            // Perform basic validation on the input
            if (email != confirmEmail)
            {
                MessageBox.Show("The emails do not match!");
            }
            else if (password != confirmPassword)
            {
                MessageBox.Show("The passwords do not match!");
            }
            else if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("All fields must be filled out!");
            }
            else
            {
                // Call the RegisterUser method to handle the registration
                RegisterUser(username, email, password);
            }
        }

        // Ablak bezárásának kezelése
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}




