using System;
using System.Windows;

namespace CasinoPRO
{
    public partial class LoginPage : Window
    {
        private string registeredUsername = string.Empty;

        public LoginPage()
        {
            InitializeComponent();
        }

        // Bejelentkezés gomb eseménykezelő
        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            string email = RegisterEmailTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            if (username == registeredUsername && password == "password")
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
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsernameTextBox.Text;
            string email = RegisterEmailTextBox.Text;
            string confirmEmail = ConfirmEmailTextBox.Text;
            string password = RegisterPasswordTextBox.Password;
            string confirmPassword = ConfirmPasswordTextBox.Password;

            if (email != confirmEmail)
            {
                MessageBox.Show("Az e-mail címek nem egyeznek!");
            }
            else if (password != confirmPassword)
            {
                MessageBox.Show("A jelszavak nem egyeznek!");
            }
            else if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Minden mezőt ki kell tölteni!");
            }
            else
            {
                // Regisztráció sikeres, a felhasználónév mentése és visszatérés a bejelentkezéshez
                registeredUsername = username;
                MessageBox.Show("Sikeres regisztráció!");

                // A regisztrált felhasználónév beállítása a bejelentkezési felület felhasználónév mezőjébe
                UsernameTextBox.Text = registeredUsername;

                // Visszatérés a bejelentkezési felületre
                RegistrationGrid.Visibility = Visibility.Collapsed;
                LoginGrid.Visibility = Visibility.Visible;
                
            }
        }

        // Ablak bezárásának kezelése
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}




