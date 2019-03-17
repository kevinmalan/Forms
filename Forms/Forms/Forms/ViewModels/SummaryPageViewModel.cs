using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Globalization;

namespace Forms.ViewModels
{
    public class Person
    {
        public string FirstName { get; set; }
        public string IdPassport { get; set; }
        public string LastName { get; set; }
    }

    public class SummaryPageViewModel : BindableBase, INavigatingAware
    {
        private string _dateOfBirth;
        private string _firstName;
        private string _idPassport;
        private string _lastName;

        public SummaryPageViewModel()
        {
        }

        public string DateOfBirth { get => _dateOfBirth; set => SetProperty(ref _dateOfBirth, value); }
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string IDPassport { get => _idPassport; set => SetProperty(ref _idPassport, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            string firstName = parameters.GetValue<string>("firstName");
            string lastName = parameters.GetValue<string>("lastName");
            string idPassport = parameters.GetValue<string>("idPassport");

            DisplaySummary(new Person { FirstName = firstName, LastName = lastName, IdPassport = idPassport });
        }

        private void DisplaySummary(Person person)
        {
            FirstName = person.FirstName;
            LastName = person.LastName;
            IDPassport = person.IdPassport;

            if (DateTime.TryParseExact(person?.IdPassport?.Substring(0,6), "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                DateOfBirth = dateOfBirth.ToString("dd MMM yyyy");
        }
    }
}