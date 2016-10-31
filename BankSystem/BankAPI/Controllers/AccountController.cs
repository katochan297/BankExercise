using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using BankData.Helper;
using BankAPI.Models;
using BankData;
using Newtonsoft.Json.Linq;
using NLog;

namespace BankAPI.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        //GET data
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

        [HttpGet]
        [Route("GetAccount")]
        public Account GetAccount(int id)
        {
            using (var uow = new UnitOfWork())
            {
                var acct = uow.AccountRepository.SingleOrDefault(x => x.AccountID == id);
                return acct;
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
        private ResponseCode Process(Account account, decimal amount, TransactionType type)
        {
            var resp = ResponseCode.Unknow;
            using (var uow = new UnitOfWork())
            {
                switch (type)
                {
                    case TransactionType.Withdraw:
                        resp = uow.AccountRepository.Withdraw(account, amount);
                        break;
                    case TransactionType.Deposit:
                        resp = uow.AccountRepository.Deposit(account, amount);
                        break;
                }
            }
            return resp;
        }

        [NonAction]
        private ResponseCode ProcessTransfer(Account accountFrom, Account accountTo, decimal amount)
        {
            //Validate account
            if (accountFrom.AccountID == accountTo.AccountID || accountFrom.AccountNumber == accountTo.AccountNumber)
                return ResponseCode.InvalidTransfer;

            //processing
            ResponseCode resp;
            using (var uow = new UnitOfWork())
            {
                resp = uow.AccountRepository.FundTransfer(accountFrom, accountTo, amount);
            }
            return resp;
        }

        #endregion

    }
}
