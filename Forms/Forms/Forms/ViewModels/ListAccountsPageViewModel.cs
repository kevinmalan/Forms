using Forms.Android.Helpers;
using Forms.Configuration;
using Forms.Dto;
using Forms.Parsers;
using Forms.State;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Forms.ViewModels
{
    public class ListAccountsPageViewModel : BindableBase, INavigatingAware
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly BypassSslValidationClientHandler _bypassSslHandler;
        private IList<AccountDto> _accounts;

        public ListAccountsPageViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _bypassSslHandler = new BypassSslValidationClientHandler();
            _client = new HttpClient(_bypassSslHandler);
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

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            if (Accounts.Count == 0 || await IsAccountsSyncedWithServer() == false)
                await GetAccounts();
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
                    DateTimeStamp = account.DateTimeStamp,
                    ProfileImage = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(account.ProfileImageBase64)))
                });
            }

            AccountStateManager.SaveAccounts(accountDtos, overwrite: true);
            Accounts = accountDtos.OrderByDescending(a => a.DateTimeStamp).ToList();
        }
    }
}