using Forms.Android.Helpers;
using Forms.Configuration;
using Forms.Dto;
using Forms.Views;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;

namespace Forms.ViewModels
{
    public class SummaryPageViewModel : BindableBase, INavigatingAware
    {
        private string _dateOfBirth;
        private string _firstName;
        private string _idPassport;
        private string _lastName;
        private Person _person;
        private HttpClient _client;
        private readonly BypassSslValidationClientHandler _bypassSslHandler;
        private readonly INavigationService _navigationService;
        private readonly IConfiguration _configuration;

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
        public DelegateCommand Register { get; set; }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            string firstName = parameters.GetValue<string>("firstName");
            string lastName = parameters.GetValue<string>("lastName");
            string idPassport = parameters.GetValue<string>("idPassport");

            _person = new Person { FirstName = firstName, LastName = lastName, IdPassport = idPassport };
            DisplaySummary();
        }

        private void DisplaySummary()
        {
            FirstName = _person.FirstName;
            LastName = _person.LastName;
            IDPassport = _person.IdPassport;

            if (DateTime.TryParseExact(_person.IdPassport.Substring(0, 6), "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                DateOfBirth = dateOfBirth.ToString("dd MMM yyyy");
        }

        public async void RegisterPerson()
        {
            string url = _configuration.ApiBaseAddress;
            string personJson = JsonConvert.SerializeObject(_person);
            var result = await _client.PostAsync(url, new StringContent(personJson, Encoding.UTF8, "application/json"));

            string message = result.IsSuccessStatusCode ?
                "Successful Registration" :
                $"An error has occured: {result.ReasonPhrase}";

            if (result.IsSuccessStatusCode)
            {
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
                await _navigationService.NavigateAsync(nameof(MainPage));
            }
        }
    }
}