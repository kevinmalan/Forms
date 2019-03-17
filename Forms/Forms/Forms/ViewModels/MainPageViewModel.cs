using Forms.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        INavigationService _navigationService;
        public DelegateCommand BeginForm { get; set; }

        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            BeginForm = new DelegateCommand(NavigateRegistrationForm);
        }

        private async void NavigateRegistrationForm()
        {
            await _navigationService.NavigateAsync(nameof(RegisterPage));
        }
    }
}
