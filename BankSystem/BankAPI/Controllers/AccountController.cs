using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using BankAPI.Helper;
using BankAPI.Models;
using BankData;
using BankData.Core;
using Newtonsoft.Json.Linq;
using NLog;

namespace BankAPI.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
      
        //GET Insert data
        [HttpGet]
        [Route("GetAccounts")]
        public List<Account> GetAccounts()
        {
            using (var uow = new UnitOfWork())
            {
                var lst = uow.AccountRepository.GetAll().ToList();
                return lst;
            }
        }


        // POST api/account/Transfer
        [HttpPost]
        [Route("Transfer")]
        public IHttpActionResult Transfer([FromBody] JObject data)
        {
            try
            {
                Logger.Info(data);
                
                dynamic json = data;
                var accFrom = new Account()
                {
                    AccountID = json.AccountID_From,
                    AccountNumber = json.AccountNumber_From
                };
                var accTo = new Account()
                {
                    AccountID = json.AccountID_To,
                    AccountNumber = json.AccountNumber_To
                };
                decimal amt = json.Amount;

                var respCode = ProcessTransfer(accFrom, accTo, amt);
                return Json(new ResponseModel(respCode));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new ResponseModel(ResponseCode.Fail));
            }
        }

        // POST api/account/withdraw
        [HttpPost]
        [Route("Withdraw")]
        public IHttpActionResult Withdraw([FromBody] JObject data)
        {
            try
            {
                Logger.Info(data);

                dynamic json = data;
                Account acct = GetAccountByData(json);
                decimal amt = json.Amount;
                
                var respCode = Process(acct, amt, TransactionType.Withdraw);
                return Json(new ResponseModel(respCode));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new ResponseModel(ResponseCode.Fail));
            }
        }

        // POST api/account/Deposit
        [HttpPost]
        [Route("Deposit")]
        public IHttpActionResult Deposit([FromBody] JObject data)
        {
            try
            {
                Logger.Info(data);

                dynamic json = data;
                Account acct = GetAccountByData(json);
                decimal amt = json.Amount;

                var respCode = Process(acct, amt, TransactionType.Deposit);
                return Json(new ResponseModel(respCode));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new ResponseModel(ResponseCode.Fail));
            }
        }


        #region Private Function
        [NonAction]
        private Account GetAccountByData(dynamic json)
        {
            var account = new Account()
            {
                AccountID = json.AccountID,
                AccountNumber = json.AccountNumber
            };
            return account;
        }

        [NonAction]
        private ResponseCode ValidateAccount(Account account, decimal amount, TransactionType type, out Account acct)
        {
            using (var uow = new UnitOfWork())
            {
                acct =
                    uow.AccountRepository.SingleOrDefault(
                        x => x.AccountID == account.AccountID && x.AccountNumber == account.AccountNumber);

                var resp = Utilities.Validation(acct, amount, type);
                return resp;
            }
        }

        [NonAction]
        private ResponseCode Process(Account account, decimal amount, TransactionType type)
        {
            //Validate account
            var resp = ValidateAccount(account, amount, type, out account);
            if (resp != ResponseCode.Validate)
                return resp;

            //processing
            using (var uow = new UnitOfWork())
            {
                switch (type)
                {
                    case TransactionType.Withdraw:
                        uow.AccountRepository.Withdraw(account, amount);
                        break;
                    case TransactionType.Deposit:
                        uow.AccountRepository.Deposit(account, amount);
                        break;
                }
                uow.Commit();

                return ResponseCode.Success;
            }
        }

        [NonAction]
        private ResponseCode ProcessTransfer(Account accountFrom, Account accountTo, decimal amount)
        {
            //Validate account
            var respFrom = ValidateAccount(accountFrom, amount, TransactionType.Withdraw, out accountFrom);
            if (respFrom != ResponseCode.Validate)
                return respFrom;
            
            var respTo = ValidateAccount(accountTo, amount, TransactionType.Deposit, out accountTo);
            if (respTo != ResponseCode.Validate)
                return respTo;

            //processing
            using (var uow = new UnitOfWork())
            {
                uow.AccountRepository.FundTransfer(accountFrom, accountTo, amount);
                uow.Commit();

                return ResponseCode.Success;
            }
        }
        
        #endregion

    }
}
