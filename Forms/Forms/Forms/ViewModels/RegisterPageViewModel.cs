using Forms.Helpers;
using Forms.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

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
            TakePhotoCommand = new DelegateCommand<object>(TakePhoto);
            UploadPhotoCommand = new DelegateCommand<object>(UploadPhoto);
        }

        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string IDPassport { get => _idPassport; set => SetProperty(ref _idPassport, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public byte[] ProfilePhotoBytes { get => _profilePhotoBytes; set => SetProperty(ref _profilePhotoBytes, value); }

        public DelegateCommand SubmitPersonalInfo { get; set; }
        public DelegateCommand<object> TakePhotoCommand { get; set; }
        public DelegateCommand<object> UploadPhotoCommand { get; set; }

        private bool CanSubmit()
        {
            return !string.IsNullOrWhiteSpace(FirstName) &&
                    !string.IsNullOrWhiteSpace(LastName) &&
                    !string.IsNullOrWhiteSpace(IDPassport) &&
                    IDPassport.Length >= 6 &&
                    IsPhotoUploaded;
        }

        private async void NavigateToSummary()
        {
            var personNavigationParams = new NavigationParameters
            {
                { "firstName", FirstName },
                { "lastName", LastName },
                { "idPassport", IDPassport },
                { "profilePhotoBytes", ProfilePhotoBytes }
            };

            await _navigationService.NavigateAsync(nameof(SummaryPage), personNavigationParams);
        }

        private void ObserveProperties(DelegateCommand command)
        {
            command.ObservesProperty(() => FirstName);
            command.ObservesProperty(() => LastName);
            command.ObservesProperty(() => IDPassport);
            command.ObservesProperty(() => IsPhotoUploaded);
        }

        private async void TakePhoto(object sv)
        {
            ProfilePhotoBytes = await ImageOperations.GetCameraPhotoBytes();

            if (ProfilePhotoBytes != null)
                IsPhotoUploaded = true;

            ScrollToBottomOfPage(sv);
        }

        private async void UploadPhoto(object sv)
        {
            ProfilePhotoBytes = await ImageOperations.GetGalleryPhotoBytes();

            if (ProfilePhotoBytes != null)
                IsPhotoUploaded = true;

            ScrollToBottomOfPage(sv);
        }

        private async void ScrollToBottomOfPage(object o)
        {
            ScrollView scrollView = o as ScrollView;

            var bottom = scrollView.FindByName<Label>("Bottom");

            await scrollView.ScrollToAsync(bottom, ScrollToPosition.End, true);
        }

        private bool _isPhotoUploaded = false;

        public bool IsPhotoUploaded
        {
            get
            {
                return _isPhotoUploaded;
            }
            set
            {
                SetProperty(ref _isPhotoUploaded, value);
            }
        }
    }
}