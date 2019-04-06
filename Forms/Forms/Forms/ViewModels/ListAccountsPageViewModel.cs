using Forms.Android.Helpers;
using Forms.Configuration;
using Forms.Dto;
using Forms.Parsers;
using Forms.State;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Forms.ViewModels
{
    public class ListAccountsPageViewModel : BindableBase, INavigatedAware
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly BypassSslValidationClientHandler _bypassSslHandler;
        private IList<AccountDto> _accounts;
        private string _idPassportInputValue;
        private string _selectedIdPassport;
        private string _selectedFullName;
        private bool _shouldShowDeleteForm;
        private bool _isLoading;

        public DelegateCommand<object> AccountTappedCommand { get; set; }
        public DelegateCommand DeleteAccountCommand { get; set; }

        public ListAccountsPageViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _bypassSslHandler = new BypassSslValidationClientHandler();
            _client = new HttpClient(_bypassSslHandler);
            AccountTappedCommand = new DelegateCommand<object>(AccountTapped);
            DeleteAccountCommand = new DelegateCommand(DeleteAccount, IsAuthorizedToDeleteAccount);
            DeleteAccountCommand.ObservesProperty(() => IdPassportInputValue); 
        }

        public bool IsAuthorizedToDeleteAccount()
        {
            return ConfirmedIdPassport != null &&
                ConfirmedIdPassport == IdPassportInputValue;
        }

        public async void DeleteAccount()
        {
            string url = $"{_configuration.ApiBaseAddress}/account/delete";

            var result = await _client.PostAsync(url, new StringContent(ConfirmedIdPassport, Encoding.UTF8, "application/json"));

            if (result.IsSuccessStatusCode)
            {
                Accounts = AccountStateManager.Remove(ConfirmedIdPassport);
                ShouldShowDeleteForm = false;

                await App.Current.MainPage.DisplayAlert("Deleted", $"{ConfirmedFullName}'s Account has been Deleted", "Ok");
                ConfirmedIdPassport = string.Empty;
                ConfirmedFullName = string.Empty;
            }
        }

        public void AccountTapped(object accountObject)
        {
            var account = (AccountDto)accountObject;
            IdPassportInputValue = string.Empty;
            ConfirmedIdPassport = account.IdPassport;
            ConfirmedFullName = account.FullName;
            ShouldShowDeleteForm = true;
        }

        public string ConfirmedIdPassport { get => _selectedIdPassport; set => SetProperty(ref _selectedIdPassport, value); }
        public string ConfirmedFullName { get => _selectedFullName; set => SetProperty(ref _selectedFullName, value); }
        public string IdPassportInputValue { get => _idPassportInputValue; set => SetProperty(ref _idPassportInputValue, value); }
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        public bool ShouldShowDeleteForm
        {
            get => _shouldShowDeleteForm;
            set => SetProperty(ref _shouldShowDeleteForm, value);
        }

        public IList<AccountDto> Accounts
        {
            get
            {
                if (_accounts == null)
                    _accounts = AccountStateManager.GetAccounts();

                return _accounts;
            }

            set
            {
                SetProperty(ref _accounts, value);
            }
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            IsLoading = true;

            if (Accounts.Count == 0 || await IsAccountsSyncedWithServer() == false)
                await GetAccounts();

            IsLoading = false;
        }

        private async Task GetAccounts()
        {
            string url = $"{_configuration.ApiBaseAddress}/account/index";
            var accountsResponse = await _client.GetAsync(url);
            var accountsStream = await accountsResponse.Content.ReadAsStreamAsync();
            var accounts = JsonHelper<IList<Account>>.Deserialize(accountsStream);

            BindAccountToView(accounts);
        }

        private async Task<bool> IsAccountsSyncedWithServer()
        {
            string url = $"{_configuration.ApiBaseAddress}/account/count";
            var response = await _client.GetAsync(url);
            var stream = await response.Content.ReadAsStreamAsync();
            int accountCount = JsonHelper<int>.Deserialize(stream);

            var isSynced = accountCount == Accounts.Count;

            return isSynced;
        }

        private void BindAccountToView(IList<Account> accounts)
        {
            IList<AccountDto> accountDtos = new List<AccountDto>();

            foreach (var account in accounts)
            {
                accountDtos.Add(new AccountDto
                {
                    FullName = $"{account.FirstName} {account.LastName}",
                    IdPassport = account.IdPassport,
                    DateTimeStamp = account.DateTimeStamp,
                    ProfileImage = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(account.ProfileImageBase64)))
                });
            }

            AccountStateManager.SaveAccounts(accountDtos, overwrite: true);
            Accounts = accountDtos.OrderByDescending(a => a.DateTimeStamp).ToList();
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }
    }
}