using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankData.Core;
using System.Threading;
using BankData.Helper;

namespace BankData.Repository
{
    internal class AccountRepository : Repository<Account>, IAccountRepository
    {
        internal AccountRepository(DbContext context) : base(context)
        {
        }
        
        public ResponseCode Withdraw(Account account, decimal amount)
        {
            lock (obj_lock)
            {
                var resp = ValidateAccount(account, amount, TransactionType.Withdraw, out account);
                if (resp != ResponseCode.Validate)
                    return resp;

                account.Balance -= amount;
                Update(account);
                Context.SaveChanges();

                return ResponseCode.Success;
            }
        }

        public ResponseCode Deposit(Account account, decimal amount)
        {
            lock (obj_lock)
            {
                var resp = ValidateAccount(account, amount, TransactionType.Deposit, out account);
                if (resp != ResponseCode.Validate)
                    return resp;

                account.Balance += amount;
                Update(account);
                Context.SaveChanges();

                return ResponseCode.Success;
            }
        }

        public ResponseCode FundTransfer(Account accFrom, Account accTo, decimal amount)
        {
            lock (obj_lock)
            {
                //Validate account
                var respFrom = ValidateAccount(accFrom, amount, TransactionType.Withdraw, out accFrom);
                if (respFrom != ResponseCode.Validate)
                    return respFrom;

                var respTo = ValidateAccount(accTo, amount, TransactionType.Deposit, out accTo);
                if (respTo != ResponseCode.Validate)
                    return respTo;

                //Process
                accFrom.Balance -= amount;
                Update(accFrom);
                accTo.Balance += amount;
                Update(accTo);
                Context.SaveChanges();

                return ResponseCode.Success;
            }
        }

        private ResponseCode ValidateAccount(Account account, decimal amount, TransactionType type, out Account acct)
        {
            acct = SingleOrDefault(
                x => x.AccountID == account.AccountID && x.AccountNumber == account.AccountNumber);

            var resp = Utilities.Validation(acct, amount, type);
            return resp;
        }



    }
}
