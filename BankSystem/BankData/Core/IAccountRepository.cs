using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankData.Helper;

namespace BankData.Core
{
    public interface IAccountRepository : IRepository<Account>
    {
        ResponseCode Withdraw(Account account, decimal amount);
        ResponseCode Deposit(Account account, decimal amount);
        ResponseCode FundTransfer(Account accFrom, Account accTo, decimal amount);
    }
}
