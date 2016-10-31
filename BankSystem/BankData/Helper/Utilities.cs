using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankData.Helper
{
    public enum TransactionType
    {
        Withdraw = 1,
        Deposit = 2
    }

    public enum ResponseCode
    {
        Success = 0,
        Fail = 1,
        AccountNotExist = 2,
        AccountOverdrawn = 3,
        Insufficient = 4,
        Validate = 5,
        InvalidTransfer = 6,
        Unknow = -1
    }

    public class Utilities
    {
        public static Dictionary<ResponseCode, string> ResponseDictionary = new Dictionary<ResponseCode, string>
        {
            { ResponseCode.Success, "Successfully"},
            { ResponseCode.Fail, "Failure"},
            { ResponseCode.AccountNotExist, "Account is not exist"},
            { ResponseCode.AccountOverdrawn, "Account overdrawn"},
            { ResponseCode.Insufficient, "Account is insufficient"},
            { ResponseCode.InvalidTransfer, "Initiator and counterparty accounts are same"},
            { ResponseCode.Unknow, "Unknow exception"}
        };



        private static readonly Dictionary<Func<Account, decimal, bool>, ResponseCode> ValidateDictionary = new Dictionary
            <Func<Account, decimal, bool>, ResponseCode>
            {
                {ValidateAccountExist, ResponseCode.AccountNotExist},
                {ValidateInsufficient, ResponseCode.Insufficient},
                {ValidateOverdraw, ResponseCode.AccountOverdrawn}
            };

        private static bool ValidateAccountExist(Account account, decimal amount)
        {
            return account != null;
        }

        private static bool ValidateInsufficient(Account account, decimal amount)
        {
            return account.Balance > 0;
        }

        private static bool ValidateOverdraw(Account account, decimal amount)
        {
            return account.Balance - amount >= 0;
        }


        public static ResponseCode Validation(Account account, decimal amount, TransactionType type)
        {
            var obj =
                ValidateDictionary.Keys.FirstOrDefault(
                    x =>
                        x.Invoke(account, amount) == false &&
                        (type == TransactionType.Deposit ? x != ValidateOverdraw : true));
            return obj != null ? ValidateDictionary[obj] : ResponseCode.Validate;
        }

    }
}
