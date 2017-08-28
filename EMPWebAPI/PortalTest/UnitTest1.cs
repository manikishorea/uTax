using System;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EMPPortalWebAPI.APIControllers;
using EMPPortal.Transactions.Account.Model;
using System.Web.Http.Results;
using System.Threading.Tasks;
using EMPPortal.Transactions.CustomerInformation;

namespace PortalTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void LoginMethod()
        {
            try
            {
                var account = getAccount();
                var controller = new CustomerLoginController();

                IHttpActionResult result = controller.Get(new CustomerLoginModel() { EMPUserId = "support", EMPPassword = "admin1" });
                var contentResult = result as OkNegotiatedContentResult<CustomerLoginModel>;
                Assert.AreEqual(account.Id, contentResult.Content.Id);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}", e.GetType(), e.Message));
            }
        }

        [TestMethod]
        public async Task GetCustomerInformation()
        {
            try
            {
                var controller = new CustomerInformationController();
                Task<IHttpActionResult> result = controller.Getemp_CustomerInformation(new Guid("85b2b509-303b-4880-9e7b-074eabd6178c"));
                var contentResult = await result as OkNegotiatedContentResult<CustomerInformationModel>;
                Assert.AreEqual(contentResult.Content.EFIN, 963710);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}", e.GetType(), e.Message));
            }
        }

        [TestMethod]
        public void ChangePassword()
        {
            try
            {
                var controller = new ChangePasswordController();
                IHttpActionResult result = controller.PostChangePasswordUpdate(new ChangePasswordModel() { CurrentPassword = "admin1", EMPPassword = "admin", Id= new Guid("85b2b509-303b-4880-9e7b-074eabd6178c") });
                var contentResult = result as OkNegotiatedContentResult<bool>;
                if (contentResult == null)
                    Assert.Fail();
                Assert.AreEqual(contentResult.Content, true);
            }
            catch (Exception e)
            {
                Assert.Fail(string.Format("Unexpected exception of type {0} caught: {1}", e.GetType(), e.Message));
            }
        }

        public CustomerLoginModel getAccount()
        {
            return new CustomerLoginModel() { Id = new Guid("51d15331-bd6c-4f7f-b82b-a783ec8f25ec") };
        }
    }
}
