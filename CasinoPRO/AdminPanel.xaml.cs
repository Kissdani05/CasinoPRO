using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CasinoPRO
{
    public partial class AdminPanel : Window
    {
        public class Profile
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Balance { get; set; }
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

            // Profilok listájának létrehozása
            Profiles = new ObservableCollection<Profile>
            {
                new Profile { Id = 1, Name = "John Doe", Balance = 100.50m, Email = "john@example.com", JoinDate = DateTime.Now.AddYears(-2), IsActive = true, Role = "Admin" },
                new Profile { Id = 2, Name = "Jane Smith", Balance = 50.75m, Email = "jane@example.com", JoinDate = DateTime.Now.AddYears(-1), IsActive = true, Role = "User" },
                new Profile { Id = 3, Name = "Mark Lee", Balance = 75.00m, Email = "mark@example.com", JoinDate = DateTime.Now.AddMonths(-6), IsActive = false, Role = "User" }
            };

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
