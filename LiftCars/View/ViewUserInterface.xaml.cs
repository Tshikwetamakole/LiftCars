
using LiftCars.ViewModel;

namespace LiftCars.View;

public partial class ViewUserInterface : ContentPage
{
	public ViewUserInterface()
	{
		InitializeComponent();

        var viewModel = new ViewModelSignInButton();
        BindingContext = viewModel;
        BindingContext = new ViewModelSignInButton();


        MessagingCenter.Subscribe<ViewModelSignInButton, string>(this, "DisplayAlert", async (sender, message) =>
        {
            await DisplayAlert("Info", message, "OK");
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<ViewModelSignInButton, string>(this, "DisplayAlert");
    }
}