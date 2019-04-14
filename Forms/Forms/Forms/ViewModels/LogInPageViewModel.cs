using Forms.Helpers;
using Plugin.Fingerprint;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;

namespace Forms.ViewModels
{
    public class LogInPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        public LogInPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LogInCommand = new DelegateCommand(LogIn);

            DetermineFingerprintLogIn();
        }

        private string _username;
        private string _password;

        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public DelegateCommand LogInCommand { get; set; }

        private async void LogIn()
        {
            await Authenticate();
        }

        private async Task Authenticate()
        {
            if (Username == "test" && Password == "pass") // Mock Equivalent of: "Is User Authenticated"
            {
                if (FingerPrintOperations.SupportsFingerScan())
                {
                    var storedUsername = await FingerPrintOperations.GetValueFromSecureStorage("user_name");
                    var storedPassword = await FingerPrintOperations.GetValueFromSecureStorage("password");

                    if (storedUsername == null || storedUsername != Username || storedPassword != Password)
                    {
                        var useFingerprintNextTime = await App.Current.MainPage.DisplayAlert("Fingerprint", "We've detected you have Fingerprint autenthication enabled on your device. Would you like to log in with them next time ?", "Yes", "No");

                        if (useFingerprintNextTime)
                        {
                            await FingerPrintOperations.AddToSecureStorage("user_name", Username);
                            await FingerPrintOperations.AddToSecureStorage("password", Password);
                        }
                    }
                }

                await NavigateToMainPage();
            }
        }

        private async Task DetermineFingerprintLogIn()
        {
            if (!FingerPrintOperations.SupportsFingerScan())
                return;

            var storedUsername = await FingerPrintOperations.GetValueFromSecureStorage("user_name");

            if (storedUsername == null)
                return;

            var result = await CrossFingerprint.Current.AuthenticateAsync("Please place your finger on the fingerprint scanner");

            if (result.Authenticated)
            {
                await NavigateToMainPage();
            }
        }

        private async Task NavigateToMainPage()
        {
            await _navigationService.NavigateAsync("/NavigationPage/MainPage");
        }
    }
}