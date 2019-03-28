using Forms.Dto;
using System.Collections.Generic;

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

        public static IList<AccountDto> GetAccounts()
        {
            return Accounts;
        }

        public static int GetSize() => Accounts.Count;
    }
}