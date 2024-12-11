using LiftCars.DataBase;
using LiftCars.ViewModel;
using Microsoft.Maui.Storage;
using static LiftCars.Model.ModelCustomers;

namespace LiftCars.View;

public partial class ViewCreateAccount : ContentPage
{

    public ViewCreateAccount()
	{
		InitializeComponent();

        var viewModel = new ViewMobelSaveButton();
        BindingContext = viewModel;

        MessagingCenter.Subscribe<ViewMobelSaveButton, string>(this, "DisplayAlert", async (sender, message) =>
        {
            await DisplayAlert("Info", message, "OK");
        });


    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<ViewMobelSaveButton, string>(this, "DisplayAlert");
    }
    

}