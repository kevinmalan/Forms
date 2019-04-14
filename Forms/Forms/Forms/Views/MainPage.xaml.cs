using Xamarin.Forms;

namespace Forms.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed() => true;
    }
}