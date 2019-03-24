using Forms.Helpers;
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
        private byte[] _profilePhotoBytes;

        public RegisterPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            SubmitPersonalInfo = new DelegateCommand(NavigateToSummary, CanSubmit);
            ObserveProperties(SubmitPersonalInfo);
            TakePhotoCommand = new DelegateCommand(TakePhoto);
            UploadPhotoCommand = new DelegateCommand(UploadPhoto);
        }

        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string IDPassport { get => _idPassport; set => SetProperty(ref _idPassport, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

        public DelegateCommand SubmitPersonalInfo { get; set; }
        public DelegateCommand TakePhotoCommand { get; set; }
        public DelegateCommand UploadPhotoCommand { get; set; }

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
                { "idPassport", IDPassport },
                { "profilePhotoBytes", _profilePhotoBytes }
            };

            await _navigationService.NavigateAsync(nameof(SummaryPage), personNavigationParams);
        }

        private void ObserveProperties(DelegateCommand command)
        {
            command.ObservesProperty(() => FirstName);
            command.ObservesProperty(() => LastName);
            command.ObservesProperty(() => IDPassport);
        }

        private async void TakePhoto()
        {
            _profilePhotoBytes = await ImageOperations.GetCameraPhotoBytes();
        }

        private async void UploadPhoto()
        {
            _profilePhotoBytes = await ImageOperations.GetGalleryPhotoBytes();
        }
    }
}