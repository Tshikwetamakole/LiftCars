


using LiftCars.DataBase;
using LiftCars.Model;
using LiftCars.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static LiftCars.Model.ModelCustomers;

namespace LiftCars.ViewModel;


public class ViewModelUpdatePassword : INotifyPropertyChanged
{
    private readonly DatabaseStorage _storage = new DatabaseStorage();

    //Using ObservableCollection to notify the UI of changes
    public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

    private string _password;
    public string _email;
    public string _name;

    //Reference for editing
    public bool IsEditMode { get; set; }
    public Customer EditingCustomer { get; set; }

    //Define
    public string Password 
    {
        get => _password; 
        set { _password = value; OnPropertyChanged(); }
    }

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


    public ICommand Savecommand { get; }
    public ICommand BackToLogin {  get; }

    //DispalyAlert
    private Page _page;

    //Class constructor
    public ViewModelUpdatePassword(Page page) 
    {
        _page = page;
        Savecommand = new Command(async () => await UpdateMethod());
        BackToLogin = new Command(async () => await BackToLoginMethod());

        //Load initial data
        Loadcustomer();
    }

    private async Task BackToLoginMethod() 
    {
        await Application.Current.MainPage.Navigation.PushAsync(new ViewUserInterface());

    }

    private async Task UpdateMethod() 
    {
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Email)) 
        {
            MessagingCenter.Send(this, "DisplayAlert", "Please capture all the fields");
        }

        if (IsEditMode && EditingCustomer != null)
        {
            //Update properties of the existing customers
            EditingCustomer.Password = Password;
            EditingCustomer.Email = Email;
            EditingCustomer.CustomerName = Name;

            //Save the updated data
            await _storage.Update(EditingCustomer);

            //Refresh the ObservableCollection
            int index = Customers.IndexOf(EditingCustomer);
            if (index >= 0)
            {
                Customers.RemoveAt(index);
                Customers.Insert(index, EditingCustomer);
            }

            MessagingCenter.Send(this, "DisplayAlert", "Customer updated successfully");
            IsEditMode = false;
            EditingCustomer = null;
        }
        else 
        {
            var existingCustomers = await _storage.GetCustomers();
            bool emailExists = existingCustomers.Any(c => c.Email == Email);
            if (emailExists)
            {
                MessagingCenter.Send(this, "DisplayAlert", "This email is already registered.");
                return;
            }

            var newCustomer = new Customer { CustomerName = Name, Email = Email, Password = Password };
            await _storage.Create(newCustomer);
            Customers.Add(newCustomer);
            MessagingCenter.Send(this, "DisplayAlert", "Customer added successfully.");
        }
        // Clear fields and reset
        Name = Email = Password = string.Empty;
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Email));
        OnPropertyChanged(nameof(Password));
    }

    private async void Loadcustomer()
    {
        var customerList = await _storage.GetCustomers();
        Customers.Clear();

        foreach (var customer in customerList)
        {
            Customers.Add(customer);
        }
    }

    public async Task OnCustomerTapped(Customer customer)
    {
        var action = await _page.DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

        switch (action)
        {
            case "Edit":
                Name = customer.CustomerName;
                Email = customer.Email;
                Password = customer.Password;
                IsEditMode = true;
                EditingCustomer = customer;
                break;

            case "Delete":
                await _storage.Delete(customer);
                Customers.Remove(customer); // Remove from collection to refresh view
                break;
        }
    }
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
