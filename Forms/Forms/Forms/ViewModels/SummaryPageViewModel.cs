using Forms.Android.Helpers;
using Forms.Configuration;
using Forms.Dto;
using Forms.Essentials;
using Forms.State;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Forms.ViewModels
{
    public class SummaryPageViewModel : BindableBase, INavigatedAware
    {
        private string _dateOfBirth;
        private string _firstName;
        private string _idPassport;
        private string _lastName;
        private string _address;
        private string _fullName;
        private ImageSource _profileImageSource;
        private Account _person;
        private HttpClient _client;
        private byte[] _profileImageBytes;
        private readonly BypassSslValidationClientHandler _bypassSslHandler;
        private readonly INavigationService _navigationService;
        private readonly IConfiguration _configuration;
        private bool _isLoading;

        public SummaryPageViewModel(INavigationService navigationService, IConfiguration configuration)
        {
            Register = new DelegateCommand(RegisterPerson);
            _bypassSslHandler = new BypassSslValidationClientHandler();
            _client = new HttpClient(_bypassSslHandler);
            _navigationService = navigationService;
            _configuration = configuration;
        }

        public string DateOfBirth { get => _dateOfBirth; set => SetProperty(ref _dateOfBirth, value); }
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string IDPassport { get => _idPassport; set => SetProperty(ref _idPassport, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Address { get => _address; set => SetProperty(ref _address, value); }
        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public ImageSource ProfileImageSource { get => _profileImageSource; set => SetProperty(ref _profileImageSource, value); }
        public DelegateCommand Register { get; set; }
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            var currentThread = Environment.CurrentManagedThreadId;

            IsLoading = true;

            string address = null;

            var firstName = parameters.GetValue<string>("firstName");
            var lastName = parameters.GetValue<string>("lastName");
            var idPassport = parameters.GetValue<string>("idPassport");
            _profileImageBytes = parameters.GetValue<byte[]>("profilePhotoBytes");

            var profileImageBase64 = Convert.ToBase64String(_profileImageBytes);
            var location = await GeolocationHelper.GetCurrentLocation();

            if (location != null)
                address = await GeolocationHelper.GetLocationAddress(location.Latitude, location.Longitude);

            // For Display Purposes only
            await Task.Delay(800);

            DateTime.TryParseExact(idPassport.Substring(0, 6), "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            _person = new Account { FirstName = firstName, LastName = lastName, IdPassport = idPassport, DateOfBirth = dateOfBirth, Address = address ?? "Not Found", ProfileImageBase64 = profileImageBase64 };

            DisplaySummary();
        }

        private void DisplaySummary()
        {
            FirstName = _person.FirstName;
            LastName = _person.LastName;
            FullName = $"{FirstName} {LastName}";
            IDPassport = _person.IdPassport;
            Address = _person.Address;
            ProfileImageSource = ImageSource.FromStream(() => new MemoryStream(_profileImageBytes));
            DateOfBirth = _person.DateOfBirth.ToString("dd MMM yyyy");
            IsLoading = false;
        }

        public async void RegisterPerson()
        {
            DateTime dateTimeStamp = DateTime.Now;
            _person.DateTimeStamp = dateTimeStamp;

            string url = $"{_configuration.ApiBaseAddress}/account/register";
            string personJson = JsonConvert.SerializeObject(_person);

            var result = await _client.PostAsync(url, new StringContent(personJson, Encoding.UTF8, "application/json"));

            string message = result.IsSuccessStatusCode ?
                "Successful Registration" :
                $"An error has occured: {result.ReasonPhrase}";

            if (result.IsSuccessStatusCode)
            {
                var accountDto = new AccountDto
                {
                    FullName = $"{FirstName} {LastName}",
                    Address = Address,
                    IdPassport = IDPassport,
                    DateOfBirth = DateOfBirth,
                    ProfileImage = ProfileImageSource,
                    DateTimeStamp = dateTimeStamp
                };
                AccountStateManager.SaveAccounts(new List<AccountDto> { accountDto });

                await App.Current.MainPage.DisplayAlert("", message, "Ok");
                NavigateHome();
            }
            else
            {
                var stayOnCurrentPage = await App.Current.MainPage.DisplayAlert("", message, "Stay", "Go to Homepage");

                if (!stayOnCurrentPage)
                    NavigateHome();
            }

            async void NavigateHome()
            {
                await _navigationService.NavigateAsync("/NavigationPage/MainPage");
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }
    }
}