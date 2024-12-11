using LiftCars.View;
using LiftCars.ViewModel;

namespace LiftCars
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ViewUserInterface());
        }
    }
}
