using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QFGreenBean.Controllers;

namespace QFGreenBean.Tests
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void TestLogin()
        {
            AccountController controller = new AccountController();

            Assert.IsNotNull(controller.Login("john"));

        }
    }
}
