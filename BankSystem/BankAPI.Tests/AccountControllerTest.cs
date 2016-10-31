using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;
using BankAPI.Controllers;
using BankAPI.Models;
using BankData.Helper;
using Newtonsoft.Json.Linq;

namespace BankAPI.Tests
{
    [TestClass]
    public class AccountControllerTest
    {
        
        [TestMethod]
        public void Test_GetAccounts()
        {
            //Arrange
            var acctController = new AccountController();

            //Act
            var result = acctController.GetAccounts();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual("123456", result[0].AccountNumber);
            Assert.AreEqual("987654", result[1].AccountNumber);
        }

        [TestMethod]
        public void Test_Withdraw()
        {
            //Arrange
            var acctController = new AccountController();
            var acct = acctController.GetAccount(1);
            var amt = 10;
            var obj = new JObject(
                new JProperty("AccountID", acct.AccountID),
                new JProperty("AccountNumber", acct.AccountNumber),
                new JProperty("Amount", amt)
            );
            
            //Act
            var result = acctController.Withdraw(obj) as JsonResult<ResponseModel>;
            var acct2 = acctController.GetAccount(1);
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ResponseCode.Success, result.Content.Status);
            Assert.AreEqual(acct2.Balance, acct.Balance - amt);
        }

        [TestMethod]
        public void Test_Deposit()
        {
            //Arrange
            var acctController = new AccountController();
            var acct = acctController.GetAccount(1);
            var amt = 10;
            var obj = new JObject(
                new JProperty("AccountID", acct.AccountID),
                new JProperty("AccountNumber", acct.AccountNumber),
                new JProperty("Amount", amt)
            );

            //Act
            var result = acctController.Deposit(obj) as JsonResult<ResponseModel>;
            var acct2 = acctController.GetAccount(1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ResponseCode.Success, result.Content.Status);
            Assert.AreEqual(acct2.Balance, acct.Balance + amt);
        }

        [TestMethod]
        public void Test_Transfer()
        {
            //Arrange
            var acctController = new AccountController();
            var acctFrom = acctController.GetAccount(1);
            var acctTo = acctController.GetAccount(2);
            var amt = 10;
            var obj = new JObject(
                new JProperty("AccountID_From", acctFrom.AccountID),
                new JProperty("AccountNumber_From", acctFrom.AccountNumber),
                new JProperty("AccountID_To", acctTo.AccountID),
                new JProperty("AccountNumber_To", acctTo.AccountNumber),
                new JProperty("Amount", amt)
            );

            //Act
            var result = acctController.Transfer(obj) as JsonResult<ResponseModel>;
            var acctFrom2 = acctController.GetAccount(1);
            var acctTo2 = acctController.GetAccount(2);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ResponseCode.Success, result.Content.Status);
            Assert.AreEqual(acctFrom2.Balance, acctFrom.Balance - amt);
            Assert.AreEqual(acctTo2.Balance, acctTo.Balance + amt);
        }


        [TestMethod]
        public async Task Test_Concurrency()
        {
            await Task.Run(
                () =>
                {
                    Task.Run(() => Test_Withdraw());
                    Task.Run(() => Test_Deposit());
                    Test_Transfer();
                }
            );
        }


    }
}
