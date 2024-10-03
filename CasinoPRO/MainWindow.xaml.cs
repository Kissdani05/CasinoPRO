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

        public MainWindow()
        {
            InitializeComponent();
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
                for (int i = mainGrid.Children.Count - 1; i >= 0; i--)
                {
                    var element = mainGrid.Children[i];
                    if (element is DockPanel == false) // Felső sáv megtartása
                    {
                        mainGrid.Children.Remove(element);
                    }
                }

                // Új lista a fogadások megjelenítéséhez
                StackPanel betsPanel = new StackPanel() { Margin = new Thickness(20) };
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
                            Margin = new Thickness(0, 5, 0, 5)
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

                mainGrid.Children.Add(betsPanel);
            }
        }
    }
}