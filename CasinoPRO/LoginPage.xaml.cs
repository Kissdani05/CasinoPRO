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

namespace CasinoPRO
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        // Bejelentkezés gomb eseménykezelő
        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            if (username == "test" && password == "password")
            {
                MessageBox.Show("Sikeres bejelentkezés!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Hibás felhasználónév vagy jelszó!");
            }
        }

        // Elfelejtett jelszó link eseménykezelő
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Jelszó visszaállítás funkció hamarosan...");
        }

        // Regisztráció link eseménykezelő
        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Regisztráció funkció hamarosan...");
        }

        // Ablak bezárásának kezelése
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
