using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankData.Core;
using System.Threading;

namespace BankData.Repository
{
    internal class AccountRepository : Repository<Account>, IAccountRepository
    {
        internal AccountRepository(DbContext context) : base(context)
        {
        }

        public void Withdraw(Account account, decimal amount)
        {
            lock (obj_lock)
            {
                account.Balance -= amount;
                Update(account);
            }
        }

        public void Deposit(Account account, decimal amount)
        {
            lock (obj_lock)
            {
                account.Balance += amount;
                Update(account);
            }
        }

        public void FundTransfer(Account accFrom, Account accTo, decimal amount)
        {
            lock (obj_lock)
            {
                Withdraw(accFrom, amount);
                Deposit(accTo, amount);
            }
        }

    }
}
