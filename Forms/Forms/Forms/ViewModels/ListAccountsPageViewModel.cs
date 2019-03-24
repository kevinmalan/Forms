using Forms.Android.Helpers;
using Forms.Configuration;
using Forms.Dto;
using Forms.Parsers;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Forms.ViewModels
{
    public class ListAccountsPageViewModel : BindableBase, INavigatingAware
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly BypassSslValidationClientHandler _bypassSslHandler;
        private IList<Account> _accounts;

        public ListAccountsPageViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _bypassSslHandler = new BypassSslValidationClientHandler();
            _client = new HttpClient(_bypassSslHandler);
        }

        public IList<Account> Accounts { get => _accounts; set => SetProperty(ref _accounts, value); }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            await GetAccounts();
        }

        private async Task GetAccounts()
        {
            string url = $"{_configuration.ApiBaseAddress}/account/index";
            var accountsResponse = await _client.GetAsync(url);
            var accountsStream = await accountsResponse.Content.ReadAsStreamAsync();

            Accounts = JsonHelper<IList<Account>>.Deserialize(accountsStream);
        }
    }
}