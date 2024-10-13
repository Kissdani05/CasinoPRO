using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Reflection;
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
using CasinoPRO.Models;
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
        public string BetName;

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

        public ObservableCollection<Matches> Match { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            this.PreviewMouseDown += MainWindow_PreviewMouseDown;
            Bets = new ObservableCollection<BetCartItem>();
            CartItemsControl.ItemsSource = Bets;
            LoadEvents();
        }
        private void Admin_Click(object sender, RoutedEventArgs e)
        { 
            AdminPanel adminPage = new AdminPanel();
            adminPage.Show();
        }
            private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Login oldal elkészítése
            LoginPage loginPage = new LoginPage();
            bool? loginResult = loginPage.ShowDialog();

            // Login sikeres?
            if (loginResult == true)
            {
                isLoggedIn = true;
                balance = loginPage.UserBalance; 
                BalanceTxt.Content = balance.ToString() + " HUF";
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
                                
                                balance = Convert.ToDouble(reader["Balance"]);
                                BalanceTxt.Content = balance.ToString() + " HUF"; 
                            }
                        }
                    }
                    dbContext.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading balance: " + ex.Message);
                }
                LoginButton.Visibility = Visibility.Collapsed;
                UserIcon.Visibility = Visibility.Visible;
                usernameTextBlock = new TextBlock
                {
                    Text = $"Welcome, {SessionManager.LoggedInUsername}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(10),
                    Visibility = Visibility.Collapsed
                };
                UserSidebar.Children.Add(usernameTextBlock);
            }
            }
        private void AdataimButton_Click(object sender, RoutedEventArgs e)
        {
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                // Töröljük az előző dinamikus paneleket
                ClearDynamicPanels(mainGrid);
                DepositPanel.Visibility = Visibility.Collapsed;
                BetOptionsPanel.Visibility = Visibility.Collapsed;
                LiveBetsPanel.Visibility = Visibility.Collapsed;
                OptionalBets.Visibility = Visibility.Collapsed;
                UserSidebar.Visibility = Visibility.Collapsed;

                MySqlConnection conn = null;
                string userName = null;
                string userEmail = null;
                double userBalance = 0;
                string loggedInUsername = SessionManager.LoggedInUsername;

                try
                {
                    DatabaseConnection dbContext = new DatabaseConnection();
                    conn = dbContext.OpenConnection();

                    if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    {
                        string query = "SELECT Username, Email, Balance FROM Bettors WHERE Username = @username";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@username", loggedInUsername);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userName = reader["Username"].ToString();
                                userEmail = reader["Email"].ToString();
                                userBalance = Convert.ToDouble(reader["Balance"]);
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
                balance = userBalance;
                BalanceTxt.Content = balance.ToString() + " HUF";
                StackPanel infoPanel = new StackPanel() { Name = "DynamicPanel", Margin = new Thickness(50) };
                TextBlock header = new TextBlock
                {
                    Text = "Adataim",
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                infoPanel.Children.Add(header);

                usernameText = new TextBlock
                {
                    Text = $"Felhasználónév: {userName}",
                    Margin = new Thickness(0, 5, 0, 5)
                };
                infoPanel.Children.Add(usernameText);

                emailText = new TextBlock
                {
                    Text = $"E-mail cím: {userEmail}",
                    Margin = new Thickness(0, 5, 0, 5)
                };
                infoPanel.Children.Add(emailText);

                editButton = new Button
                {
                    Content = "Módosítás",
                    Width = 100,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                editButton.Click += EditButton_Click;
                infoPanel.Children.Add(editButton);

                saveButton = new Button
                {
                    Content = "Mentés",
                    Width = 100,
                    Margin = new Thickness(0, 20, 0, 0),
                    Visibility = Visibility.Collapsed
                };
                saveButton.Click += SaveButton_Click;
                infoPanel.Children.Add(saveButton);

                Button backButton = new Button
                {
                    Content = "Vissza",
                    Width = 100,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                backButton.Click += BackButton_Click;
                infoPanel.Children.Add(backButton);

                mainGrid.Children.Add(infoPanel);
            }
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
                    string query = "SELECT Username, Email FROM Bettors WHERE Username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", loggedInUsername);
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
                Text = $"{userName}", 
                Margin = new Thickness(0, 5, 0, 5)
            };

            emailTextBox = new TextBox
            {
                Text = $"{userEmail}", 
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
                            balance = Convert.ToDouble(reader["Balance"]);
                            BalanceTxt.Content = balance.ToString() + " HUF";  
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
            Match = new();
            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                MySqlConnection conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string query = "UPDATE Bettors SET Username = @newUsername, Email = @newEmail WHERE Username = @oldUsername";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@newUsername", newUsername);
                    cmd.Parameters.AddWithValue("@newEmail", newEmail);
                    cmd.Parameters.AddWithValue("@oldUsername", SessionManager.LoggedInUsername);
                    cmd.ExecuteNonQuery();
                    SessionManager.LoggedInUsername = newUsername;
                    LoadUserBalance(newUsername);
                    MessageBox.Show("User information updated successfully!");
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
            if (UserSidebar.Visibility == Visibility.Visible)
            {
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
            UserSidebar.Visibility = Visibility.Collapsed;
            UserIcon.Visibility = Visibility.Collapsed; 
            LoginButton.Visibility = Visibility.Visible;
            isLoggedIn = false;
            BalanceTxt.Content = "0 HUF";
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            RightSidebar.Visibility = Visibility.Visible;
            RightSidebarOverlay.Visibility = Visibility.Visible;
        }

        private void RightSidebarOverlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RightSidebar.Visibility = Visibility.Collapsed;
            RightSidebarOverlay.Visibility = Visibility.Collapsed;
        }


        public void LoadEvents()
        {
            MySqlConnection conn = null;
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
                    string query = "SELECT EventName, EventDate, Category, Location FROM events";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Retrieve data from each column
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
        //Fogadási lehetőségre kattintás esemény
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
                    MessageBox.Show("Login was unsuccessful. Please try again.");
                }
            }
            else
            {
                var button = sender as Button;
                var stk = button.Parent as StackPanel;
                var betNameTextBlock = stk.Children[0] as TextBlock;
                if (betNameTextBlock != null)
                {
                    BetName = betNameTextBlock.Text; // Assign value to the global variable
                    OnTeamSelected(BetName); // Example event that might use BetName
                }
            }
        }

        // Csak számok beírásának engedélyezése
        private void BetAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _); 
        }

        private void OnTeamSelected(string teamName)
        {
            var existingBet = Bets.FirstOrDefault(b => b.TeamName == teamName);
            if (existingBet == null)
            {
                var betItem = new BetCartItem
                {
                    TeamName = teamName,
                    BetAmount = 0 
                };
                Bets.Add(betItem);
            }
        }

        // Fogadás véglegesítése
        private void FinalizeBet_CLick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BetCartItem betItem)
            {
                if (balance >= betItem.BetAmount)
                {
                    balance -= betItem.BetAmount;
                    BalanceTxt.Content = $"{balance} HUF";
                    try
                    {
                        string loggedInUsername = SessionManager.LoggedInUsername;
                        DatabaseConnection dbContext = new DatabaseConnection();
                        MySqlConnection conn = dbContext.OpenConnection();

                        if (conn != null && conn.State == System.Data.ConnectionState.Open)
                        {
                            // Update Bettor's balance
                            string query = "UPDATE Bettors SET Balance = @balance WHERE Username = @username";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@balance", balance);
                            cmd.Parameters.AddWithValue("@username", loggedInUsername);
                            cmd.ExecuteNonQuery();

                            // Call FinalizeBets method to process the bet
                            FinalizeBets(betItem.BetAmount);

                            // Show success message and remove the bet item from Bets
                            MessageBox.Show($"Sikeres fogadás: {betItem.BetAmount} HUF, egyenlege frissítve.");
                            Bets.Remove(betItem);
                        }

                        dbContext.CloseConnection();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hiba történt a fogadás során: " + ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Nincs elég egyenleged a fogadásra");
                }
            }
        }

        public void FinalizeBets(double BetAmount)
        {
            try
            {
                string loggedInUsername = SessionManager.LoggedInUsername;
                DatabaseConnection dbContext = new DatabaseConnection();
                MySqlConnection conn = dbContext.OpenConnection();

                #region User Vars
                DateTime betDate = DateTime.Now;
                double odds = GetRandomNumber(1.00, 8.00);
                double Amount = BetAmount;
                int bettorsId = 0;
                int eventId = 0;
                DateTime eventDate = DateTime.MinValue;
                bool status = false;
                #endregion

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    string selectUserIdQuery = "SELECT BettorsID FROM Bettors WHERE Username = @username";
                    MySqlCommand selectUserIdCmd = new MySqlCommand(selectUserIdQuery, conn);
                    selectUserIdCmd.Parameters.AddWithValue("@username", loggedInUsername);
                    using (MySqlDataReader reader = selectUserIdCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bettorsId = Convert.ToInt32(reader["BettorsID"]);
                        }
                    }
                    string selectEventIdQuery = "SELECT EventID, EventDate FROM Events WHERE EventName = @eventname";
                    MySqlCommand selectEventIdCmd = new MySqlCommand(selectEventIdQuery, conn);
                    if (!string.IsNullOrEmpty(BetName))
                    {
                        selectEventIdCmd.Parameters.AddWithValue("@eventname", BetName);
                    }
                    else
                    {
                        throw new Exception("BetName is not assigned.");
                    }

                    using (MySqlDataReader eventReader = selectEventIdCmd.ExecuteReader())
                    {
                        if (eventReader.Read())
                        {
                            eventId = Convert.ToInt32(eventReader["EventID"]);
                            eventDate = Convert.ToDateTime(eventReader["EventDate"]);
                        }
                    }

                    string insertQuery = "INSERT INTO Bets (BetDate, Odds, Amount, BettorsID, EventID, Status) VALUES (@betdate, @odds, @amount, @bettorsid, @eventid, @status)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@betdate", betDate);
                    insertCmd.Parameters.AddWithValue("@odds", odds);
                    insertCmd.Parameters.AddWithValue("@amount", Amount);
                    insertCmd.Parameters.AddWithValue("@bettorsid", bettorsId);
                    insertCmd.Parameters.AddWithValue("@eventid", eventId);
                    int comparisonResult = DateTime.Compare(eventDate, DateTime.Now);
                    if (comparisonResult < 0)
                    {
                        status = false;
                    }
                    else
                    {
                        status = true;
                    }
                    insertCmd.Parameters.AddWithValue("@status", status);
                    insertCmd.ExecuteNonQuery();
                }
                dbContext.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt a fogadás során: " + ex.ToString());
            }
        }

        private void ClearDynamicPanels(Grid mainGrid)
        {
            var dynamicPanels = mainGrid.Children
                                        .OfType<UIElement>()
                                        .Where(x => x is StackPanel && ((StackPanel)x).Name == "DynamicPanel")
                                        .ToList();

            foreach (var panel in dynamicPanels)
            {
                mainGrid.Children.Remove(panel);
            }
        }
        private void EddigiFogadasaim_Click(object sender, RoutedEventArgs e)
        {
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                ClearDynamicPanels(mainGrid);
                DepositPanel.Visibility = Visibility.Collapsed;
                BetOptionsPanel.Visibility = Visibility.Collapsed;
                LiveBetsPanel.Visibility = Visibility.Collapsed;
                OptionalBets.Visibility = Visibility.Collapsed;
                UserSidebar.Visibility = Visibility.Collapsed;
                StackPanel betsPanel = new StackPanel() { Name = "DynamicPanel", Margin = new Thickness(50) };
                TextBlock header = new TextBlock
                {
                    Text = "Eddigi fogadásaim",
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                betsPanel.Children.Add(header);

                // Retrieve bets from the database
                try
                {
                    string loggedInUsername = SessionManager.LoggedInUsername;
                    DatabaseConnection dbContext = new DatabaseConnection();
                    MySqlConnection conn = dbContext.OpenConnection();

                    if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    {
                        // Fetch finalized bets for the logged-in user
                        string selectBetsQuery = @"
                    SELECT b.BetDate, b.Odds, b.Amount, e.EventName, b.Status
                    FROM Bets b
                    JOIN Bettors bt ON b.BettorsID = bt.BettorsID
                    JOIN Events e ON b.EventID = e.EventID
                    WHERE bt.Username = @username";

                        MySqlCommand cmd = new MySqlCommand(selectBetsQuery, conn);
                        cmd.Parameters.AddWithValue("@username", loggedInUsername);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    DateTime betDate = reader.GetDateTime("BetDate");
                                    double odds = reader.GetDouble("Odds");
                                    double amount = reader.GetDouble("Amount");
                                    string eventName = reader.GetString("EventName");
                                    bool status = reader.GetBoolean("Status");

                                    // Format bet details
                                    string betDetails = $"Dátum: {betDate}, Esemény: {eventName}, Odds: {odds}, Összeg: {amount} HUF, " +
                                                        $"Státusz: {(status ? "Aktív" : "Lejárt")}";

                                    // Add the bet details to the panel
                                    TextBlock betText = new TextBlock
                                    {
                                        Text = betDetails,
                                        Margin = new Thickness(0, 5, 0, 35)
                                    };
                                    betsPanel.Children.Add(betText);
                                }
                            }
                            else
                            {
                                // No finalized bets
                                TextBlock noBetsText = new TextBlock
                                {
                                    Text = "Még nincs véglegesített fogadás.",
                                    FontStyle = FontStyles.Italic,
                                    Margin = new Thickness(0, 5, 0, 5)
                                };
                                betsPanel.Children.Add(noBetsText);
                            }
                        }
                    }

                    dbContext.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt a fogadások lekérdezése során: " + ex.ToString());
                }

                // Add back button
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
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                for (int i = mainGrid.Children.Count - 1; i >= 0; i--)
                {
                    var element = mainGrid.Children[i];
                    if (element is StackPanel) 
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                }
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
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                ClearDynamicPanels(mainGrid);

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
                        MessageBox.Show("Login was unsuccessful. Please try again.");
                    }
                }
            }
        }

        private void BackFromDeposit_Click(object sender, RoutedEventArgs e)
        {
            DepositPanel.Visibility = Visibility.Collapsed;
            BetOptionsPanel.Visibility = Visibility.Visible;
            LiveBetsPanel.Visibility = Visibility.Visible;
            OptionalBets.Visibility = Visibility.Visible;
        }

        private void ConfirmDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(DepositAmount.Text, out int depositAmount) && depositAmount > 0)
            {
                balance += depositAmount;
                BalanceTxt.Content = balance.ToString("") + " HUF";
                try
                {
                    string loggedInUsername = SessionManager.LoggedInUsername;
                    MessageBox.Show(loggedInUsername);
                    DatabaseConnection dbContext = new DatabaseConnection();
                    MySqlConnection conn = dbContext.OpenConnection();
                    if (conn != null && conn.State == System.Data.ConnectionState.Open)
                    {
                        string query = "UPDATE Bettors SET Balance = @balance WHERE Username = @username";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@balance", balance);
                        cmd.Parameters.AddWithValue("@username", loggedInUsername);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Sikeresen befizetett {depositAmount} HUF, egyenlege frissítve.");
                    }
                    dbContext.CloseConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt a befizetés során: " + ex.Message);
                }
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
            Grid mainGrid = this.Content as Grid;
            if (mainGrid != null)
            {
                // Töröljük az előző dinamikus paneleket
                ClearDynamicPanels(mainGrid);
                DepositPanel.Visibility = Visibility.Collapsed;
                // Elrejtjük a többi panelt
                BetOptionsPanel.Visibility = Visibility.Collapsed;
                LiveBetsPanel.Visibility = Visibility.Collapsed;
                OptionalBets.Visibility = Visibility.Collapsed;
                UserSidebar.Visibility = Visibility.Collapsed;

                // Létrehozunk egy StackPanel-t a kifizetési beállításokhoz
                StackPanel kifizetesPanel = new StackPanel() { Name = "DynamicPanel", Margin = new Thickness(50) };
                TextBlock header = new TextBlock
                {
                    Text = "Kifizetés",
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    Margin = new Thickness(0, 0, 0, 10)
                };
                kifizetesPanel.Children.Add(header);

                // Létrehozzuk a bemeneti mezőt az összeghez (csak numerikus bevitel)
                TextBox kifizetesAmountBox = new TextBox
                {
                    Width = 150,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                kifizetesAmountBox.PreviewTextInput += BetAmount_PreviewTextInput; // Csak numerikus bevitel engedélyezése
                kifizetesPanel.Children.Add(kifizetesAmountBox);

                // "Kifizetés" gomb
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
                            balance -= kifizetesAmount;
                            BalanceTxt.Content = $"{balance} HUF"; // Frissítjük az egyenleget a UI-n

                            try
                            {
                                string loggedInUsername = SessionManager.LoggedInUsername;

                                DatabaseConnection dbContext = new DatabaseConnection();
                                MySqlConnection conn = dbContext.OpenConnection();

                                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                                {
                                    string query = "UPDATE Bettors SET Balance = @balance WHERE Username = @username";
                                    MySqlCommand cmd = new MySqlCommand(query, conn);
                                    cmd.Parameters.AddWithValue("@balance", balance);
                                    cmd.Parameters.AddWithValue("@username", loggedInUsername);

                                    cmd.ExecuteNonQuery();

                                    MessageBox.Show($"Sikeres kifizetés: {kifizetesAmount} HUF, egyenlege frissítve.");
                                }

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

                // "Vissza" gomb
                Button backButton = new Button
                {
                    Content = "Vissza",
                    Width = 100,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                backButton.Click += BackButton_Click; // Visszatérünk a főoldalra
                kifizetesPanel.Children.Add(backButton);

                // Hozzáadjuk a panelt a fő gridhez
                mainGrid.Children.Add(kifizetesPanel);
            }
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
        

        public double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}