using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestExample.Controllers;

namespace UnitTestExample.Test
{
    public class AccountControllerTestFixture
    {
        [
            Test,
            TestCase("abcd1234", false),
            TestCase("irf@uni-corvinus", false),
            TestCase("irf.uni-corvinus.hu", false),
            TestCase("irf@uni-corvinus.hu", true)
        ]
        public void TestValidateEmail(string email,bool expectedResult)
        {
            // Arrange
            var accountController = new AccountController();

            // Act
            var actualResult = accountController.ValidateEmail(email);

            // Assert
            Assert.AreEqual(expectedResult, actualResult);

        }
        [
            Test,
            TestCase("abcd", false),
            TestCase("ABC123", false),
            TestCase("abc123", false),
            TestCase("a", true),
            TestCase("bbbbbbbbbbbbb", true)
        ]
        public void TestValidatePassword(string password, bool expectedResult)
        {
            
        }
    }
}
