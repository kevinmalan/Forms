using Forms.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Forms.State
{
    public class AccountStateManager
    {
        public static List<AccountDto> Accounts = new List<AccountDto>();

        public static void SaveAccounts(IList<AccountDto> accounts, bool overwrite = false)
        {
            if (overwrite)
                Accounts = new List<AccountDto>();

            Accounts.AddRange(accounts);
        }

        public static List<AccountDto> Remove(string idPassport)
        {
            Accounts = Accounts.Where(a => a.IdPassport != idPassport).ToList();

            return Accounts;
        }

        public static IList<AccountDto> GetAccounts()
        {
            var accounts = Accounts.OrderByDescending(a => a.DateTimeStamp).ToList();

            return accounts;
        }

        public static int GetSize() => Accounts.Count;
    }
}