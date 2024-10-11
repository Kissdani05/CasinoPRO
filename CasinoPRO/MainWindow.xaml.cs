using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CasinoPRO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Global Variables
        private double balance = 0;
        private bool isLoggedIn = false;
        TextBlock usernameTextBlock;
        public ICommand FinalizeBetCommand { get; set; }
        private List<string> finalizedBets = new List<string>();
        public ObservableCollection<BetCartItem> Bets { get; set; }
        TextBlock usernameText;
        TextBlock emailText;
        TextBox usernameTextBox;
        TextBox emailTextBox;
        Button saveButton;
        Button editButton;
        #endregion

        public class BetCartItem
        {
            public string TeamName { get; set; }
            public double BetAmount { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.PreviewMouseDown += MainWindow_PreviewMouseDown;
            Bets = new ObservableCollection<BetCartItem>();
            CartItemsControl.ItemsSource = Bets;
            FinalizeBetCommand = new RelayCommand(FinalizeBet);
        }
        private void Admin_Click(object sender, RoutedEventArgs e)
        { 
            AdminPanel adminPage = new AdminPanel();
            adminPage.Show();
        }
            private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and show the login page
            LoginPage loginPage = new LoginPage();

            // Show the login window as a dialog
            bool? loginResult = loginPage.ShowDialog();

            // Check if login was successful
            if (loginResult == true)
            {
                // Set the logged-in status only if the login is actually successful
                isLoggedIn = true;

                // Get the balance from the LoginPage and update it in MainWindow
                balance = loginPage.UserBalance;  // Retrieve the balance from LoginPage

                // Update balance in the UI
                BalanceTxt.Content = balance.ToString() + " HUF";

                // Call any other post-login actions
                Bejelentkezes();
            }
        }



        private void Bejelentkezes()
        {
            if (isLoggedIn == true)
            {

                try
                {
                    DatabaseConnection dbContext = new DatabaseConnection();
                    MySqlConnection conn = dbContext.OpenConnection();
                    string loggedInUsername = SessionManager.LoggedInUsername;
                    if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    {
                        string query = "SELECT Balance FROM Bettors WHERE Username = @username";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@username", loggedInUsername);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Store the balance in the 'balance' variable and update the UI
                                balance = Convert.ToDouble(reader["Balance"]);
                                BalanceTxt.Content = balance.ToString() + " HUF";  // Update balance in UI
                            }
                        }
                    }
                    dbContext.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading balance: " + ex.Message);
                }

                // Hide the login button
                LoginButton.Visibility = Visibility.Collapsed;

                // Show the user icon
                UserIcon.Visibility = Visibility.Visible;

                // Dynamically create the username label (TextBlock)
                usernameTextBlock = new TextBlock
                {
                    Text = $"Welcome, {SessionManager.LoggedInUsername}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10),
                    Visibility = Visibility.Collapsed
                };

                // Add the dynamic username label to your UI (for example, a StackPanel)
                UserSidebar.Children.Add(usernameTextBlock);
            }
            }
        // Globális változók létrehozása a TextBlock-ok és TextBox-okhoz

        private void AdataimButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = null;
            string userName = null;
            string userEmail = null;
            double userBalance = 0;  // Store the fetched balance
            string loggedInUsername = SessionManager.LoggedInUsername;

            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Query to fetch username, email, and balance
                    string query = "SELECT Username, Email, Balance FROM Bettors WHERE Username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", loggedInUsername);

                    // Execute the query and read data
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userName = reader["Username"].ToString();
                            userEmail = reader["Email"].ToString();
                            userBalance = Convert.ToDouble(reader["Balance"]);  // Get balance from DB
                        }
                    }
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

            // Update the balance in the UI
            balance = userBalance;  // Set the balance in your app
            BalanceTxt.Content = balance.ToString() + " HUF";  // Update balance label

            // Hide other panels
            BetOptionsPanel.Visibility = Visibility.Collapsed;
            LiveBetsPanel.Visibility = Visibility.Collapsed;
            OptionalBets.Visibility = Visibility.Collapsed;
            UserSidebar.Visibility = Visibility.Collapsed;

            // Display user information
            #region User info
            StackPanel infoPanel = new StackPanel() { Margin = new Thickness(50) };
            TextBlock header = new TextBlock
            {
                Text = "Adataim",
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                Margin = new Thickness(0, 0, 0, 10)
            };
            infoPanel.Children.Add(header);

            // Display the fetched username and email
            usernameText = new TextBlock
            {
                Text = $"Felhasználónév: {userName}", // Use the value from the database
                Margin = new Thickness(0, 5, 0, 5)
            };
            infoPanel.Children.Add(usernameText);

            emailText = new TextBlock
            {
                Text = $"E-mail cím: {userEmail}", // Use the value from the database
                Margin = new Thickness(0, 5, 0, 5)
            };
            infoPanel.Children.Add(emailText);

            // Edit button
            editButton = new Button
            {
                Content = "Módosítás",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0)
            };
            editButton.Click += EditButton_Click;
            infoPanel.Children.Add(editButton);

            // Save button (initially hidden)
            saveButton = new Button
            {
                Content = "Mentés",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0),
                Visibility = Visibility.Collapsed // Initially hidden
            };
            saveButton.Click += SaveButton_Click;
            infoPanel.Children.Add(saveButton);

            // Back button
            Button backButton = new Button
            {
                Content = "Vissza",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0)
            };
            backButton.Click += BackButton_Click;
            infoPanel.Children.Add(backButton);

            // Add to the main grid
            Grid mainGrid = this.Content as Grid;
            mainGrid.Children.Add(infoPanel);
            #endregion


        }

        // Módosítás gomb eseménykezelője
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = null;
            string userName = null;
            string userEmail = null;
            string newUserName = null;
            string newUserEmail = null;
            string loggedInUsername = SessionManager.LoggedInUsername;

            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Query to fetch username, email, and balance
                    string query = "SELECT Username, Email FROM Bettors WHERE Username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", loggedInUsername);

                    // Execute the query and read data
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userName = reader["Username"].ToString();
                            userEmail = reader["Email"].ToString();
                        }
                    }
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


            usernameTextBox = new TextBox
            {
                Text = $"{userName}", // Az aktuális felhasználónév
                Margin = new Thickness(0, 5, 0, 5)
            };

            emailTextBox = new TextBox
            {
                Text = $"{userEmail}", // Az aktuális e-mail cím
                Margin = new Thickness(0, 5, 0, 5)
            };



            #region Test codes
            //A TextBlock-okat eltávolítjuk
            StackPanel parentPanel = usernameText.Parent as StackPanel;
            parentPanel.Children.Remove(usernameText);
            parentPanel.Children.Remove(emailText);

            //Hozzáadjuk a TextBox - okat
            parentPanel.Children.Insert(1, usernameTextBox);
            parentPanel.Children.Insert(2, emailTextBox);
            #endregion
            // Elrejtjük a Módosítás gombot és megjelenítjük a Mentés gombot
            editButton.Visibility = Visibility.Collapsed;
            saveButton.Visibility = Visibility.Visible;
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
                            // Store the balance in the 'balance' variable and update the UI
                            balance = Convert.ToDouble(reader["Balance"]);
                            BalanceTxt.Content = balance.ToString() + " HUF";  // Update balance in UI
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

        // Mentés gomb eseménykezelője
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = usernameTextBox.Text;
            string newEmail = emailTextBox.Text;

            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                MySqlConnection conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Update the username and email in the database
                    string query = "UPDATE Bettors SET Username = @newUsername, Email = @newEmail WHERE Username = @oldUsername";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@newUsername", newUsername);
                    cmd.Parameters.AddWithValue("@newEmail", newEmail);
                    cmd.Parameters.AddWithValue("@oldUsername", SessionManager.LoggedInUsername);
                    cmd.ExecuteNonQuery();

                    // Update SessionManager with the new username
                    SessionManager.LoggedInUsername = newUsername;

                    // Refresh user balance or other data if needed
                    LoadUserBalance(newUsername);
                    MessageBox.Show("User information updated successfully!");

                    // Update the dynamically generated username label (TextBlock)
                    if (usernameTextBlock != null)
                    {
                        usernameTextBlock.Text = $"Welcome, {newUsername}";
                    }
                }

                dbContext.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user data: " + ex.Message);
            }

            // Update the UI elements to reflect the new data
            usernameText.Text = $"Felhasználónév: {newUsername}";
            emailText.Text = $"E-mail cím: {newEmail}";

            saveButton.Visibility = Visibility.Collapsed;
            editButton.Visibility = Visibility.Visible;
        }

        // Felhasználói ikonra kattintás esemény
        private void UserIcon_Click(object sender, RoutedEventArgs e)
        {
            // Oldalsáv megjelenítése/elrejtése
            if (UserSidebar.Visibility == Visibility.Collapsed)
            {
                UserSidebar.Visibility = Visibility.Visible;
            }
            else
            {
                UserSidebar.Visibility = Visibility.Collapsed;
            }
        }

        private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Ha a kattintás nem az oldalsó sávon történik, rejtse el azt
            if (UserSidebar.Visibility == Visibility.Visible)
            {
                // Ellenőrizzük, hogy a kattintás az oldalsó sávon kívül történt-e
                if (!IsMouseOverSidebar(e))
                {
                    UserSidebar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool IsMouseOverSidebar(MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(UserSidebar);
            return (mousePos.X >= 0 && mousePos.X <= UserSidebar.ActualWidth &&
                    mousePos.Y >= 0 && mousePos.Y <= UserSidebar.ActualHeight);
        }

        // Kijelentkezés logikája
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Kijelentkezés művelet
            UserSidebar.Visibility = Visibility.Collapsed; // Oldalsáv elrejtése
            UserIcon.Visibility = Visibility.Collapsed; // Ikon elrejtése
            LoginButton.Visibility = Visibility.Visible;
            isLoggedIn = false;// Bejelentkezés gomb megjelenítése
            BalanceTxt.Content = "0 HUF";
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the sidebar and overlay
            RightSidebar.Visibility = Visibility.Visible;
            RightSidebarOverlay.Visibility = Visibility.Visible;
        }

        // This method handles clicking outside the sidebar (on the overlay)
        private void RightSidebarOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Hide the sidebar and overlay when clicked outside
            RightSidebar.Visibility = Visibility.Collapsed;
            RightSidebarOverlay.Visibility = Visibility.Collapsed;
        }

        // Fogadási lehetőségre kattintás esemény
        private void BetOption_Click(object sender, RoutedEventArgs e)
        {
            
            if (!isLoggedIn)
            {
                MessageBox.Show("You need to log in to deposit.");
                LoginPage loginPage = new LoginPage();
                bool? loginResult = loginPage.ShowDialog();
                
                if (loginResult == true)
                {
                    isLoggedIn = true;
                    Bejelentkezes();
                }
                else
                {
                    // Handle case where user closes the login window without logging in
                    MessageBox.Show("Login was unsuccessful. Please try again.");
                }
            }
            else {
                var button = sender as Button;
                if (button != null)
                {
                    // Use button content (team name) to add a bet
                    string teamName = button.Content.ToString();
                    OnTeamSelected(teamName); // Call the method to add the selected bet
                }
            }
        }

        // Csak számok beírásának engedélyezése
        private void BetAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _); // Csak számok engedélyezése
        }

        private void OnTeamSelected(string teamName)
        {
            var existingBet = Bets.FirstOrDefault(b => b.TeamName == teamName);
            if (existingBet == null)
            {
                var betItem = new BetCartItem
                {
                    TeamName = teamName,
                    BetAmount = 0 // Default value
                };
                Bets.Add(betItem);
            }
        }

        // Fogadás véglegesítése
        private void FinalizeBet(object bet)
        {
            
            var betItem = bet as BetCartItem;
            if (betItem != null)
            {
                // Finalize the bet (e.g., save to database or show a message)
                MessageBox.Show($"Bet on {betItem.TeamName} finalized with amount: {betItem.BetAmount}");

                // Remove the bet from the list after finalization
                Bets.Remove(betItem);
            }
        }

        // Eddigi fogadások megjelenítése
        private void EddigiFogadasaim_Click(object sender, RoutedEventArgs e)
        {
            // Tartalom eltávolítása (kivéve a felső sávot)
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                BetOptionsPanel.Visibility = Visibility.Collapsed;
                LiveBetsPanel.Visibility = Visibility.Collapsed;
                OptionalBets.Visibility = Visibility.Collapsed;
                UserSidebar.Visibility = Visibility.Collapsed;

                // Új lista a fogadások megjelenítéséhez
                StackPanel betsPanel = new StackPanel() { Margin = new Thickness(50), };
                TextBlock header = new TextBlock
                {
                    Text = "Eddigi fogadásaim",
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                betsPanel.Children.Add(header);

                if (finalizedBets.Count > 0)
                {
                    foreach (var bet in finalizedBets)
                    {
                        TextBlock betText = new TextBlock
                        {
                            Text = bet,
                            Margin = new Thickness(0, 5, 0, 35)
                        };
                        betsPanel.Children.Add(betText);
                    }
                }
                else
                {
                    TextBlock noBetsText = new TextBlock
                    {
                        Text = "Még nincs véglegesített fogadás.",
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    betsPanel.Children.Add(noBetsText);
                }

                // Vissza gomb hozzáadása
                Button backButton = new Button
                {
                    Content = "Vissza",
                    Width = 100,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                backButton.Click += BackButton_Click;
                betsPanel.Children.Add(backButton);

                mainGrid.Children.Add(betsPanel);
            }
            if ()
            {
                
            }
        }

        // "Vissza" gomb eseménykezelője
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                // Töröljük az "Eddigi fogadásaim" oldal tartalmát
                for (int i = mainGrid.Children.Count - 1; i >= 0; i--)
                {
                    var element = mainGrid.Children[i];
                    if (element is StackPanel) // Eltávolítjuk a fogadások oldalt
                    {
                        
                        element.Visibility = Visibility.Collapsed;
                    }
                }
                

                // Főoldali elemek visszaállítása
                
                if (BetOptionsPanel != null && LiveBetsPanel != null && OptionalBets != null)
                {
                    BetOptionsPanel.Visibility = Visibility.Visible;
                    LiveBetsPanel.Visibility = Visibility.Visible;
                    OptionalBets.Visibility = Visibility.Visible;

                }
            }
        }

        private void ShowDepositPanel_Click(object sender, RoutedEventArgs e)
        {
            if (isLoggedIn == true)
            {
                BetOptionsPanel.Visibility = Visibility.Collapsed;
                LiveBetsPanel.Visibility = Visibility.Collapsed;
                OptionalBets.Visibility = Visibility.Collapsed;
                UserSidebar.Visibility = Visibility.Collapsed;
                DepositPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("You need to log in to deposit.");
                LoginPage loginPage = new LoginPage();
                bool? loginResult = loginPage.ShowDialog();
                if (loginResult == true)
                {
                    isLoggedIn = true;
                    Bejelentkezes();
                }
                else
                {
                    // Handle case where user closes the login window without logging in
                    MessageBox.Show("Login was unsuccessful. Please try again.");
                }
            }
        }

        private void BackFromDeposit_Click(object sender, RoutedEventArgs e)
        {
            // Rejtsd el a befizetési panelt
            DepositPanel.Visibility = Visibility.Collapsed;

            // Visszaállítod a fő UI elemeket
            BetOptionsPanel.Visibility = Visibility.Visible;
            LiveBetsPanel.Visibility = Visibility.Visible;
            OptionalBets.Visibility = Visibility.Visible;
        }

        private void ConfirmDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(DepositAmount.Text, out int depositAmount) && depositAmount > 0)
            {
                // Befizetési összeg hozzáadása a balance-hoz
                balance += depositAmount;

                // Frissítjük a balance kijelzését
                BalanceTxt.Content = balance.ToString("") + " HUF";

                // Update the balance in the database
                try
                {
                    // Get the logged-in user's username
                    string loggedInUsername = SessionManager.LoggedInUsername;
                    MessageBox.Show(loggedInUsername);

                    // Open database connection
                    DatabaseConnection dbContext = new DatabaseConnection();
                    MySqlConnection conn = dbContext.OpenConnection();

                    if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    {
                        // Update query to modify the balance for the logged-in user
                        string query = "UPDATE Bettors SET Balance = @balance WHERE Username = @username";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@balance", balance);
                        cmd.Parameters.AddWithValue("@username", loggedInUsername);

                        // Execute the query
                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"Sikeresen befizetett {depositAmount} HUF, egyenlege frissítve.");
                    }

                    // Close the connection
                    dbContext.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt a befizetés során: " + ex.Message);
                }

                // Rejtsd el a befizetési panelt és térj vissza a fő UI-hoz
                DepositPanel.Visibility = Visibility.Collapsed;
                BetOptionsPanel.Visibility = Visibility.Visible;
                LiveBetsPanel.Visibility = Visibility.Visible;
                OptionalBets.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Kérem, adjon meg egy érvényes összeget.");
            }
        }

        private void DepositAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _); // Csak számok engedélyezése
        }

        private void KifizetesButton_Click(object sender, RoutedEventArgs e)
        { 

            // Hide other panels
            BetOptionsPanel.Visibility = Visibility.Collapsed;
            LiveBetsPanel.Visibility = Visibility.Collapsed;
            OptionalBets.Visibility = Visibility.Collapsed;
            UserSidebar.Visibility = Visibility.Collapsed;

            // Create a StackPanel for withdrawal inputs
            StackPanel kifizetesPanel = new StackPanel() { Margin = new Thickness(50) };
            TextBlock header = new TextBlock
            {
                Text = "Kifizetés",
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                Margin = new Thickness(0, 0, 0, 10)
            };
            kifizetesPanel.Children.Add(header);

            // Create the input field for amount (numeric input only)
            TextBox kifizetesAmountBox = new TextBox
            {
                Width = 150,
                Margin = new Thickness(0, 5, 0, 5)
            };
            kifizetesAmountBox.PreviewTextInput += BetAmount_PreviewTextInput; // Only allow numeric input
            kifizetesPanel.Children.Add(kifizetesAmountBox);

            // "Kifizetés" button
            Button kifizetesButton = new Button
            {
                Content = "Kifizetés",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0)
            };
            kifizetesButton.Click += (s, args) =>
            {
                if (double.TryParse(kifizetesAmountBox.Text, out double kifizetesAmount) && kifizetesAmount > 0)
                {
                    if (balance >= kifizetesAmount)
                    {
                        balance -= kifizetesAmount; // Subtract amount from balance
                        BalanceTxt.Content = $"{balance} HUF"; // Update UI balance

                        // Update the balance in the database
                        try
                        {
                            // Get the logged-in user's username
                            string loggedInUsername = SessionManager.LoggedInUsername;

                            // Open database connection
                            DatabaseConnection dbContext = new DatabaseConnection();
                            MySqlConnection conn = dbContext.OpenConnection();

                            if (conn != null && conn.State == System.Data.ConnectionState.Open)
                            {
                                // Update query to modify the balance for the logged-in user
                                string query = "UPDATE Bettors SET Balance = @balance WHERE Username = @username";
                                MySqlCommand cmd = new MySqlCommand(query, conn);
                                cmd.Parameters.AddWithValue("@balance", balance);
                                cmd.Parameters.AddWithValue("@username", loggedInUsername);

                                // Execute the query
                                cmd.ExecuteNonQuery();

                                MessageBox.Show($"Sikeres kifizetés: {kifizetesAmount} HUF, egyenlege frissítve.");
                            }

                            // Close the connection
                            dbContext.CloseConnection();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hiba történt a kifizetés során: " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nincs elegendő egyenleg.");
                    }
                }
                else
                {
                    MessageBox.Show("Kérem, adjon meg egy érvényes összeget.");
                }
            };
            kifizetesPanel.Children.Add(kifizetesButton);

            // "Vissza" button
            Button backButton = new Button
            {
                Content = "Vissza",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0)
            };
            backButton.Click += BackButton_Click; // Return to the main page
            kifizetesPanel.Children.Add(backButton);

            // Add the panel to the main grid
            Grid mainGrid = this.Content as Grid;
            mainGrid.Children.Add(kifizetesPanel);
        }

        // Gombok létrehozása a fogadási lehetőségekhez
        private Button CreateBetButton(string content)
        {
            return new Button
            {
                Content = content,
                Width = 100,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
        }
    }
}