using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VTS.Core.Business.ViewModel;
using VTS.Tests.Ninject;

namespace VTS.Tests
{
    [TestClass]
    public class IntegrationTest : Test
    {      
        [TestMethod]
        public async Task TestLoginViewModel()
        {
            onlineMode = true;
            await InitLogin();
            await LoginViewModel();
        }

        [TestMethod]
        public async Task TestLoginViewModelOffline()
        {
            onlineMode = false;
            await InitLogin();
            await LoginViewModel();
        }

        [TestMethod]
        public async Task TestVacationsViewModel()
        {
            onlineMode = true;
            await InitLogin();
            await VacationsViewModel();            
        }

        [TestMethod]
        public async Task TestVacationsViewModelOffline()
        {
            onlineMode = false;
            await InitLogin();            
            await VacationsViewModel();
        }

        [TestMethod]
        public async Task TestIntegrationCreateVacation()
        {
            onlineMode = true;            
            await InitLogin();
            await IntegrationCreateVacation();
        }

        [TestMethod]
        public async Task TestIntegrationCreateVacationOffline()
        {
            onlineMode = false;
            await InitLogin();
            await IntegrationCreateVacation();
        }

        [TestMethod]
        public async Task TestIntegrationUpdateVacation()
        {
            onlineMode = true;
            await InitLogin();
            await IntegrationUpdateVacation();
        }

        [TestMethod]
        public async Task TestIntegrationUpdateVacationOffline()
        {
            onlineMode = false;
            await InitLogin();
            await IntegrationUpdateVacation();
        }

        [TestMethod]
        public async Task TestIntegrationSaveImage()
        {
            onlineMode = true;
            await InitLogin();
            await IntegrationSaveImage();
        }

        [TestMethod]
        public async Task TestIntegrationSaveImageOffline()
        {
            onlineMode = false;
            await InitLogin();
            await IntegrationSaveImage();
        }

        [TestMethod]
        public async Task TestIntegrationDeleteVacation()
        {
            onlineMode = true;
            await InitLogin();
            await IntegrationDeleteVacation();
        }

        [TestMethod]
        public async Task TestIntegrationDeleteVacationOffline()
        {
            onlineMode = false;
            await InitLogin();
            await IntegrationDeleteVacation();
        }

        private async Task InitLogin()
        {
            if (onlineMode)
            {
                LoginViewModel loginViewModel = (onlineMode) ? FactorySingleton.Factory.Get<LoginViewModel>() : FactorySingleton.FactoryOffline.Get<LoginViewModel>();
                await loginViewModel.SignIn("vasya", "secret");
            }
            else
            {
                await InitData();//init sql data for offline mode
            }
        }
    }
}
