using Prism;
using Prism.Ioc;
using Forms.ViewModels;
using Forms.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;
using Forms.Configuration;
using System.IO;
using Newtonsoft.Json;
using Autofac;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Forms
{
    public partial class App  
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<SummaryPage, SummaryPageViewModel>();
            containerRegistry.RegisterInstance<IConfiguration>(GetConfiguration());

            containerRegistry.RegisterForNavigation<ListAccountsPage, ListAccountsPageViewModel>();
        }

        private Config GetConfiguration()
        {
            var config = new Config();

            var embeddedResource = Assembly.GetAssembly(typeof(IConfiguration))
                            .GetManifestResourceStream("Forms.Configuration.config.json");

            if (embeddedResource == null)
                return config;

            using (StreamReader reader = new StreamReader(embeddedResource))
            {
                var jsonString = reader.ReadToEnd();
                config = JsonConvert.DeserializeObject<Config>(jsonString);
            }

            return config;
        }

    }
}
