using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
namespace CasinoPRO
{
    /// <summary>
    /// Interaction logic for OrganizerPage.xaml
    /// </summary>
    public partial class OrganizerPage : Window
    {
        public ObservableCollection<Sport> Sports { get; set; }
        public Sport SelectedSport { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; }

        public OrganizerPage()
        {
            InitializeComponent();
            DataContext = this;

        }

        public void LoadBets()
        {

        }
        private void Sportok_Click(object sender, RoutedEventArgs e)
        {
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
