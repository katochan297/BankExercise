using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http.Results;
using BankAPI.Controllers;
using BankAPI.Helper;
using BankAPI.Models;
using Newtonsoft.Json.Linq;

namespace BankAPI.Tests
{
    [TestClass]
    public class AccountControllerTest
    {
        private AccountController _acctController;

        [TestInitialize]
        public void Initialize()
        {
            _acctController = new AccountController();
        }

        [TestMethod]
        public void Test_GetAccounts()
        {
            //Arrange

            //Act
            var result = _acctController.GetAccounts();

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
            var obj = new JObject(
                new JProperty("AccountID", "1"),
                new JProperty("AccountNumber", "123456"),
                new JProperty("Amount", "10")
            );

            //Act
            var result = _acctController.Withdraw(obj) as JsonResult<ResponseModel>;
            
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ResponseCode.Success, result.Content.Status);
        }

        [TestMethod]
        public void Test_Deposit()
        {
            //Arrange
            var obj = new JObject(
                new JProperty("AccountID", "1"),
                new JProperty("AccountNumber", "123456"),
                new JProperty("Amount", "10")
            );

            //Act
            var result = _acctController.Deposit(obj) as JsonResult<ResponseModel>;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ResponseCode.Success, result.Content.Status);
        }

        [TestMethod]
        public void Test_Transfer()
        {
            //Arrange
            var obj = new JObject(
                new JProperty("AccountID_From", "1"),
                new JProperty("AccountNumber_From", "123456"),
                new JProperty("AccountID_To", "2"),
                new JProperty("AccountNumber_To", "987654"),
                new JProperty("Amount", "10")
            );

            //Act
            var result = _acctController.Transfer(obj) as JsonResult<ResponseModel>;

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(ResponseCode.Success, result.Content.Status);
        }


        [TestMethod]
        public void Test_Concurrency()
        {
            Test_Transfer();
            Task.Run(() => Test_Transfer());
            Test_Withdraw();
            Task.Run(() => Test_Withdraw());
            Test_Deposit();
        }


    }
}
