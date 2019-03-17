using Forms.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace Forms.ViewModels
{
    public class RegisterPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;
        private string _firstName;
        private string _idPassport;
        private string _lastName;

        public RegisterPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SubmitPersonalInfo = new DelegateCommand(NavigateToSummary, CanSubmit);
            ObserveProperties(SubmitPersonalInfo);
        }

        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string IDPassport { get => _idPassport; set => SetProperty(ref _idPassport, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public DelegateCommand SubmitPersonalInfo { get; set; }

        private bool CanSubmit()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                    !string.IsNullOrWhiteSpace(LastName) &&
                    !string.IsNullOrWhiteSpace(IDPassport) &&
                    IDPassport.Length > 1;
        }

        private async void NavigateToSummary()
        {
            var personNavigationParams = new NavigationParameters
            {
                { "firstName", FirstName },
                { "lastName", LastName },
                { "idPassport", IDPassport }
            };

            await _navigationService.NavigateAsync(nameof(SummaryPage), personNavigationParams);
        }

        private void ObserveProperties(DelegateCommand command)
        {
            command.ObservesProperty(() => FirstName);
            command.ObservesProperty(() => LastName);
            command.ObservesProperty(() => IDPassport);
        }
    }
}