using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankData.Core
{
    public interface IAccountRepository : IRepository<Account>
    {
        void Withdraw(Account account, decimal amount);
        void Deposit(Account account, decimal amount);
        void FundTransfer(Account accFrom, Account accTo, decimal amount);
    }
}
