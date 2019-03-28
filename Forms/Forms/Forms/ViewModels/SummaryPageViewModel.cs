﻿using Forms.Android.Helpers;
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
using Xamarin.Forms;

namespace Forms.ViewModels
{
    public class SummaryPageViewModel : BindableBase, INavigatingAware
    {
        private string _dateOfBirth;
        private string _firstName;
        private string _idPassport;
        private string _lastName;
        private string _address;
        private ImageSource _profileImageSource;
        private Account _person;
        private HttpClient _client;
        private byte[] _profileImageBytes;
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
        public string Address { get => _address; set => SetProperty(ref _address, value); }
        public ImageSource ProfileImageSource { get => _profileImageSource; set => SetProperty(ref _profileImageSource, value); }
        public DelegateCommand Register { get; set; }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            var firstName = parameters.GetValue<string>("firstName");
            var lastName = parameters.GetValue<string>("lastName");
            var idPassport = parameters.GetValue<string>("idPassport");
            _profileImageBytes = parameters.GetValue<byte[]>("profilePhotoBytes");

            var profileImageBase64 = Convert.ToBase64String(_profileImageBytes);
            var location = await GeolocationHelper.GetCurrentLocation();
            var address = await GeolocationHelper.GetLocationAddress(location.Latitude, location.Longitude);

            DateTime.TryParseExact(idPassport.Substring(0, 6), "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth);

            _person = new Account { FirstName = firstName, LastName = lastName, IdPassport = idPassport, DateOfBirth = dateOfBirth, Address = address, ProfileImageBase64 = profileImageBase64 };
            DisplaySummary();
        }

        private void DisplaySummary()
        {
            FirstName = _person.FirstName;
            LastName = _person.LastName;
            IDPassport = _person.IdPassport;
            Address = _person.Address;
            ProfileImageSource = ImageSource.FromStream(() => new MemoryStream(_profileImageBytes));
            DateOfBirth = _person.DateOfBirth.ToString("dd MMM yyyy");
        }

        public async void RegisterPerson()
        {
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
                    FirstName = FirstName,
                    LastName = LastName,
                    Address = Address,
                    IdPassport = IDPassport,
                    DateOfBirth = DateOfBirth,
                    ProfileImage = ProfileImageSource
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
    }
}