using LiftCars.DataBase;
using LiftCars.View;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static LiftCars.Model.ModelCustomers;

namespace LiftCars.ViewModel
{
    public class ViewMobelSaveButton : INotifyPropertyChanged
    {

        private readonly DatabaseStorage _dbService = new DatabaseStorage();

        private string _name;
        private string _email;
        private string _password;
        private string _confirmPassword;

        //Checkbox
        private bool _ShowPassword;
        private bool _HidePassword = true;

        //Navigate to signin page


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

        public List<string> RoleOptions { get; }
        public string RoleCode { get; set; }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand SignInHere {  get; }

        public ViewMobelSaveButton()
        {
            _dbService = new DatabaseStorage();
            // Initialize RoleOptions
            RoleOptions = new List<string> { "Rider", "Driver" };
            SaveCommand = new Command(async () => await SaveAsync());
            SignInHere = new Command(async () => await SignInHereAsync());
        }


        private async Task SignInHereAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ViewUserInterface());
        }

        private async Task SaveAsync()
        {
            // 1. Check if all fields are filled
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword) || string.IsNullOrEmpty(RoleCode))
            {
                MessagingCenter.Send(this, "DisplayAlert", "Please fill in all fields.");
                return;
            }

            // 2. Check if password matches confirm password
            if (Password != ConfirmPassword)
            {
                MessagingCenter.Send(this, "DisplayAlert", "Passwords do not match.");
                return;
            }

            // 3. Check if email already exist in the database
            var existingCustomers = await _dbService.GetCustomers();
            bool emailExists = existingCustomers.Any(c => c.Email == Email);

            if (emailExists)
            {
                MessagingCenter.Send(this, "DisplayAlert", "This email is already registered.");
                return;
            }

            // 4. Save new customer if all validations pass
            string resultMessage = await _dbService.Create(new Customer
            {
                CustomerName = Name,
                Email = Email,
                Password = Password,
                RoleCode = RoleCode
            });

          
            MessagingCenter.Send(this, "DisplayAlert", resultMessage);

            // Clear fields after saving
            Name = Email = Password = ConfirmPassword = string.Empty;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(ConfirmPassword));

            //Back to sign-in page
            await Application.Current.MainPage.Navigation.PushAsync(new ViewUserInterface());

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

    }
}
