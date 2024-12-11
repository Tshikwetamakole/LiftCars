using LiftCars.Model;
using LiftCars.ViewModel;
using static LiftCars.Model.ModelCustomers;

namespace LiftCars.View;


public partial class ViewForgetPassword : ContentPage
{
    public ViewForgetPassword()
	{
		InitializeComponent();

        BindingContext = new ViewModelUpdatePassword(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Subscribe to the messaging center to show alerts
        MessagingCenter.Subscribe<ViewModelUpdatePassword, string>(this, "DisplayAlert", async (sender, message) =>
        {
            // Show the alert when the message is received
            await DisplayAlert("Message", message, "OK");
        });
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Unsubscribe from the MessagingCenter to prevent memory leaks
        MessagingCenter.Unsubscribe<ViewModelUpdatePassword, string>(this, "DisplayAlert");
    }

    private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var viewModel = (ViewModelUpdatePassword)BindingContext;
        await viewModel.OnCustomerTapped((Customer)e.Item);
    }
}
