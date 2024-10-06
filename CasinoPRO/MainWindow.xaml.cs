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
        private List<string> finalizedBets = new List<string>();
        private double balance = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.PreviewMouseDown += MainWindow_PreviewMouseDown;
                       
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.ShowDialog();
            // Felhasználó sikeresen bejelentkezett (példa)
            bool loginSuccess = true;

            if (loginSuccess)
            {
                LoginButton.Visibility = Visibility.Collapsed; // Rejtse el a bejelentkezés gombot
                UserIcon.Visibility = Visibility.Visible; // Mutassa meg a felhasználói ikont
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
            // Hide other panels
            BetOptionsPanel.Visibility = Visibility.Collapsed;
            LiveBetsPanel.Visibility = Visibility.Collapsed;
            OptionalBets.Visibility = Visibility.Collapsed;
            UserSidebar.Visibility = Visibility.Collapsed;

            // Display user information
            StackPanel infoPanel = new StackPanel() { Margin = new Thickness(50), };
            TextBlock header = new TextBlock
            {
                Text = "Adataim",
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                Margin = new Thickness(0, 0, 0, 10)
            };
            infoPanel.Children.Add(header);
            string tesztFelh = "asdasd";
            string tesztEmail = "asd@gmail.com";

            // Eredeti TextBlock elemek létrehozása
            usernameText = new TextBlock
            {
                Text = $"Felhasználónév: {tesztFelh}",
                Margin = new Thickness(0, 5, 0, 5)
            };
            infoPanel.Children.Add(usernameText);

            emailText = new TextBlock
            {
                Text = $"E-mail cím: {tesztEmail}",
                Margin = new Thickness(0, 5, 0, 5)
            };
            infoPanel.Children.Add(emailText);

            // Módosítás gomb
            editButton = new Button
            {
                Content = "Módosítás",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0)
            };
            editButton.Click += EditButton_Click;
            infoPanel.Children.Add(editButton);

            // Mentés gomb (kezdetben rejtve)
            saveButton = new Button
            {
                Content = "Mentés",
                Width = 100,
                Margin = new Thickness(0, 20, 0, 0),
                Visibility = Visibility.Collapsed // Elrejtve
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
            LoginButton.Visibility = Visibility.Visible; // Bejelentkezés gomb megjelenítése
        }
        
        // Fogadási lehetőségre kattintás esemény
        private void BetOption_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                // Beállítjuk a kiválasztott fogadást
                SelectedBetText.Text = $"Választott fogadás: {clickedButton.Content}";

                // Megjelenítjük a kosarat
                BetCart.Visibility = Visibility.Visible;
            }
        }

        // Csak számok beírásának engedélyezése
        private void BetAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _); // Csak számok engedélyezése
        }

        // Fogadás véglegesítése
        private void FinalizeBet_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(BetAmount.Text, out int betAmount) && betAmount > 0)
            {
                // Mentjük a véglegesített fogadást
                string finalizedBet = $"{SelectedBetText.Text}, Összeg: {betAmount} HUF";
                finalizedBets.Add(finalizedBet);

                MessageBox.Show($"Sikeres fogadás: {SelectedBetText.Text}, Összeg: {betAmount} HUF");

                // Kosár elrejtése a véglegesítés után
                BetCart.Visibility = Visibility.Collapsed;
                BetAmount.Clear(); // Beviteli mező törlése
            }
            else
            {
                MessageBox.Show("Kérem, adjon meg egy érvényes összeget.");
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
            // Rejtsd el a fő tartalmat
            BetOptionsPanel.Visibility = Visibility.Collapsed;
            LiveBetsPanel.Visibility = Visibility.Collapsed;
            OptionalBets.Visibility = Visibility.Collapsed;
            UserSidebar.Visibility = Visibility.Collapsed;

            // Mutasd a befizetési panelt
            DepositPanel.Visibility = Visibility.Visible;
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