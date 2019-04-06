using Forms.Views;
using Prism.Commands;
using Prism.Navigation;

namespace Forms.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public DelegateCommand Register { get; set; }
        public DelegateCommand ListAccounts { get; set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Register = new DelegateCommand(NavigateRegistrationForm);
            ListAccounts = new DelegateCommand(NavigateListAccounts);
        }

        private async void NavigateRegistrationForm()
        {
            await _navigationService.NavigateAsync(nameof(RegisterPage));
        }

        private async void NavigateListAccounts()
        {
            await _navigationService.NavigateAsync(nameof(ListAccountsPage));
        }
    }
}