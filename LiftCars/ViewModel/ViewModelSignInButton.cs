using LiftCars.DataBase;
using LiftCars.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static LiftCars.Model.ModelCustomers;

namespace LiftCars.ViewModel
{
    internal class ViewModelSignInButton : INotifyPropertyChanged
    {
        private readonly DatabaseStorage  _dbService = new DatabaseStorage();

        private string _email;
        private string _password;

        //checkbox
        private bool _ShowPassword;
        private bool _HidePassword = true;

        
        public bool ShowPassword
        {
            get => _ShowPassword;
            set
            {
                if (_ShowPassword != value)
                {
                    _ShowPassword = value;
                    IsPasswordHidden = !_ShowPassword;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsPasswordHidden
        {
            get => _HidePassword;
            set
            {
                if (_HidePassword != value)
                {
                    _HidePassword = value;
                    OnPropertyChanged();
                }
            }
        }


        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand SignInCommand { get; }
        public ICommand SignUpHere {  get; }
        public ICommand UpdatePassword { get; }


        public ViewModelSignInButton() 
        {
            _dbService = new DatabaseStorage();
            SignInCommand = new Command(async () => await SignInAsync());
            SignUpHere = new Command(async () => await SignUpHereAsync());
            UpdatePassword = new Command(async () => await ForgetPassword());

        }

        private async void OnSendResetLink()
        {
            // Add logic to send the reset link here.
            await Application.Current.MainPage.DisplayAlert("Reset Link Sent", $"A reset link has been sent to {Email}.", "OK");
            // Optionally navigate back to the login page or another page.
        }

        //Navigate to create account page
        private async Task SignUpHereAsync() 
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ViewCreateAccount());
        }

        private async Task SignInAsync() 
        {
            Debug.WriteLine($"Email: {Email}, Password: {Password}");

            // 1. Check if both fields are filled
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessagingCenter.Send(this, "DisplayAlert", "Please fill in all fields.");
                return;
            }

            // 2. Query database to check if user exists
            var existingCustomers = await _dbService.GetCustomers();
            var user = existingCustomers.FirstOrDefault(c => c.Email == Email && c.Password == Password);
            if (user != null)
            {

                if (user.RoleCode == "Rider")
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ViewRiderDashboard());
                }
                else if (user.RoleCode == "Driver")
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ViewdriverDashBoard());
                }

                // Clear fields after saving
                Email = string.Empty;
                Password = string.Empty;
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Password));

            }

            else
            {
                // User does not exist
                MessagingCenter.Send(this, "DisplayAlert", "User does not exist. Please register first.");
            }


        }

        //Forget Password
        string resultMessage;
        private int _editCustomerId;

        private async Task ForgetPassword()
        {
            // Check if the email field is empty
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter your email address.", "OK");
                return;
            }

            // Check if the email exists in the database
            var existingCustomers = await _dbService.GetCustomers();
            var customer = existingCustomers.FirstOrDefault(c => c.Email == Email);

            if (customer == null)
            {
                // Email does not exist in the database
                await Application.Current.MainPage.DisplayAlert("Error", "Email not found. Please check your email or register first.", "OK");
                return;
            }

            // Email exists, navigate to the Sign-Up page and pass the customer data
            await Application.Current.MainPage.Navigation.PushAsync(new ViewForgetPassword());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
