using System.Threading.Tasks;
using HRMv2.Models.TokenAuth;
using HRMv2.Web.Controllers;
using Shouldly;
using Xunit;

namespace HRMv2.Web.Tests.Controllers
{
    public class HomeController_Tests: HRMv2WebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}