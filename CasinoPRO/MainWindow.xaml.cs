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
        public class BetCartItem
        {
            public string TeamName { get; set; }
            public double BetAmount { get; set; }
        }
        private bool isLoggedIn = false;
        public ICommand FinalizeBetCommand { get; set; }
        private List<string> finalizedBets = new List<string>();
        private double balance = 0;
        public ObservableCollection<BetCartItem> Bets { get; set; }
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
            LoginPage loginPage = new LoginPage();
            loginPage.ShowDialog();
            isLoggedIn = true;
            // Felhasználó sikeresen bejelentkezett (példa)
            if (isLoggedIn == true) {
                Bejelentkezes();
            }

        }



        private void Bejelentkezes()
        {
            if (isLoggedIn == true)
            {
                LoginButton.Visibility = Visibility.Collapsed; // Rejtse el a bejelentkezés gombot
                UserIcon.Visibility = Visibility.Visible;
                // Mutassa meg a felhasználói ikont
            }
        }
        // Globális változók létrehozása a TextBlock-ok és TextBox-okhoz
        TextBlock usernameText;
        TextBlock emailText;
        TextBox usernameTextBox;
        TextBox emailTextBox;
        Button saveButton;
        Button editButton;

        private void AdataimButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection conn = null;
            string userName = null; // This will store the fetched username
            string userEmail = null; // This will store the fetched email
            string loggedInUsername = SessionManager.LoggedInUsername;
            try
            {
                DatabaseConnection dbContext = new DatabaseConnection();
                conn = dbContext.OpenConnection();

                if (conn != null && conn.State == System.Data.ConnectionState.Open)
                {
                    // Query to fetch user data (replace with your actual query)
                    string query = "SELECT Username, Email FROM Bettors WHERE Username = @username";  // Assuming 'Username' is unique
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Assuming you have stored the username of the logged-in user  // Replace with actual logic for fetching current username
                    cmd.Parameters.AddWithValue("@username", loggedInUsername);

                    // Execute the query and read data
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Assign the values from the database to userName and userEmail
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

            // Hide other panels
            BetOptionsPanel.Visibility = Visibility.Collapsed;
            LiveBetsPanel.Visibility = Visibility.Collapsed;
            OptionalBets.Visibility = Visibility.Collapsed;
            UserSidebar.Visibility = Visibility.Collapsed;

            // Display user information
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
        }

            // Módosítás gomb eseménykezelője
            private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            usernameTextBox = new TextBox
            {
                Text = "asdasd", // Az aktuális felhasználónév
                Margin = new Thickness(0, 5, 0, 5)
            };

            emailTextBox = new TextBox
            {
                Text = "asd@gmail.com", // Az aktuális e-mail cím
                Margin = new Thickness(0, 5, 0, 5)
            };

            // A TextBlock-okat eltávolítjuk
            StackPanel parentPanel = usernameText.Parent as StackPanel;
            parentPanel.Children.Remove(usernameText);
            parentPanel.Children.Remove(emailText);

            // Hozzáadjuk a TextBox-okat
            parentPanel.Children.Insert(1, usernameTextBox);
            parentPanel.Children.Insert(2, emailTextBox);

            // Elrejtjük a Módosítás gombot és megjelenítjük a Mentés gombot
            editButton.Visibility = Visibility.Collapsed;
            saveButton.Visibility = Visibility.Visible;
        }

        // Mentés gomb eseménykezelője
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = usernameTextBox.Text;
            string newEmail = emailTextBox.Text;

            // Visszaalakítjuk a TextBox-okat TextBlock-okká
            usernameText.Text = $"Felhasználónév: {newUsername}";
            emailText.Text = $"E-mail cím: {newEmail}";

            StackPanel parentPanel = usernameTextBox.Parent as StackPanel;
            parentPanel.Children.Remove(usernameTextBox);
            parentPanel.Children.Remove(emailTextBox);

            parentPanel.Children.Insert(1, usernameText);
            parentPanel.Children.Insert(2, emailText);

            // A mentés gomb elrejtése és a módosítás gomb megjelenítése
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
                MessageBox.Show("A fogadáshoz be kell jelentkeznie.");
                LoginPage loginPage = new LoginPage();
                loginPage.ShowDialog(); // Átirányít a bejelentkezési felületre
                isLoggedIn = true;
                Bejelentkezes();
                return;
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

        // Fogadás véglegesítése
        

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
                MessageBox.Show("A fogadáshoz be kell jelentkeznie.");
                LoginPage loginPage = new LoginPage();
                loginPage.ShowDialog(); // Átirányít a bejelentkezési felületre
                isLoggedIn = true;
                Bejelentkezes();
                return;
            }
            // Rejtsd el a fő tartalmat
            // Mutasd a befizetési panelt
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
                BalanceTxt.Content = balance.ToString("")+" HUF";

                MessageBox.Show($"Sikeresen befizetett {depositAmount} HUF");

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
            kifizetesAmountBox.PreviewTextInput += BetAmount_PreviewTextInput; // Csak számokat fogadjon el
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
                        balance -= kifizetesAmount; // Levonjuk az összeget az egyenlegből
                        BalanceTxt.Content = $"{balance} HUF"; // Frissítjük a felhasználói egyenleget a UI-ban
                        MessageBox.Show($"Sikeres kifizetés: {kifizetesAmount} HUF");
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
            backButton.Click += BackButton_Click; // Visszatérés a főoldalra
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