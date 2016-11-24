using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VTS.Tests.Ninject;
using VTS.Core.Data.Models;
using System.Collections.Generic;
using VTS.Core.CrossCutting;
using VTS.Core.Data.WebServices;
using System.Net.Http;
using System.IO;
using VTS.Core.CrossCutting.Extensions;

namespace VTS.Tests
{
    [TestClass]
    public class TestRestServices
    {
        #region var
        string config = FactorySingleton.Factory.Get<Configuration>().RestServerUrl;
        int timeout = FactorySingleton.Factory.Get<Configuration>().ServerTimeOut; 
        int id = 2;
        RestService restService;
        List<VacationInfoModel> vacationInfoModelsList;
        VacationInfoModel vacationInfoModel;
        HttpResponseMessage responce;
        #endregion

        private async Task InitLogin()
        {
            string config = FactorySingleton.Factory.Get<Configuration>().RestServerUrl;
            var restS = new RestService(config, "login");
            restS.Timeout = FactorySingleton.Factory.Get<Configuration>().ServerTimeOut;
            HttpResponseMessage request = await restS.Post("vasya", "secret");
        }

        [TestMethod]
        public async Task TestRestServiceLogin()
        {
            var restS = new RestService(config, "login2"); //Wrong Model
            restS.Timeout = timeout;
            HttpResponseMessage request = await restS.Post("vasya", "secret");//Right Login
            Assert.AreNotEqual(request.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return error");
            Assert.AreNotEqual(request.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/post return not Forbidden");
            Assert.AreEqual(request.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/post return NotFound");
            Assert.IsFalse(request.IsSuccessStatusCode, "Error: Rest.Post Login Wrong but Success Status Code");

            restS = new RestService(config, "login");
            restS.Timeout = timeout;
            request = await restS.Post("vasya", "secret2"); //Wrong Login
            Assert.AreNotEqual(request.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return error");
            Assert.AreEqual(request.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/post return not Forbidden");
            Assert.AreNotEqual(request.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/post return NotFound");
            Assert.IsFalse(request.IsSuccessStatusCode, "Error: Rest.Post Login Wrong but Success Status Code");

            request = await restS.Post("vasya", "secret");//Right Login
            Assert.AreEqual(request.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return error");
            Assert.AreNotEqual(request.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/post return Forbidden");
            Assert.AreNotEqual(request.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/post return NotFound");
            Assert.IsTrue(request.IsSuccessStatusCode, "Error: Rest.Post Login Is Not Success Status Code");
        }

        [TestMethod]
        public async Task TestRestServiceGetVacation()
        {
            await InitLogin();
            restService = new RestService(config, "Vacations2");//Test Wrong model name
            restService.Timeout = timeout;
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNull(vacationInfoModelsList, "Message: " + config + "/api/vacations2 return not null");
            restService = new RestService(config, "Vacations"); //Right Model get Data               
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
        }

        [TestMethod]
        public async Task TestRestServiceUpdateVacation()
        {
            await InitLogin();
            restService = new RestService(config, "Vacations");
            restService.Timeout = timeout;
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
            vacationInfoModel = await restService.Get<VacationInfoModel>(id);
            Assert.IsNotNull(vacationInfoModel, "Message: " + config + "/api/vacations?id=" + id + " return null");
            var firstcount = vacationInfoModelsList.Count;
            vacationInfoModel.Type.Value = "TestRestService";
            responce = await restService.Post(vacationInfoModel); //update vacation
            Assert.AreEqual(responce.StatusCode, System.Net.HttpStatusCode.OK, "Message: " + config + "/api/vacations/post return error");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return NoContent");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/post return Forbidden");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/post return NotFound");
            Assert.IsTrue(responce.IsSuccessStatusCode, "Error: Rest.Post Is Not Success Status Code");
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
            var secondcount = vacationInfoModelsList.Count;
            Assert.AreEqual(firstcount, secondcount, "Error: firstcount !=  secondcount after update");
            VacationInfoModel newvacation = await restService.Get<VacationInfoModel>(id);
            Assert.IsNotNull(newvacation, "Message: " + config + "/api/vacations?id=" + id + " return null");
            Assert.AreEqual(vacationInfoModel.Type.Value, newvacation.Type.Value, "Error: not Equal VacationInfoModel after update");
        }

        [TestMethod]
        public async Task TestRestServiceCreateVacation()
        {
            await InitLogin();
            restService = new RestService(config, "Vacations");
            restService.Timeout = timeout;
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
            var firstcount = vacationInfoModelsList.Count;
            vacationInfoModel = await restService.Get<VacationInfoModel>(id);
            Assert.IsNotNull(vacationInfoModel, "Message: " + config + "/api/vacations?id=" + id + "return null");
            vacationInfoModel.Id = 0; //create new vacation
            responce = await restService.Post(vacationInfoModel);
            Assert.AreEqual(responce.StatusCode, System.Net.HttpStatusCode.OK, "Message: " + config + "/api/vacations/post return error");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return NoContent");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/post return Forbidden");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/post return NotFound");
            Assert.IsTrue(responce.IsSuccessStatusCode, "Error: Rest.Post Is Not Success Status Code");
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
            var secondcount = vacationInfoModelsList.Count;
            Assert.AreNotEqual(firstcount, secondcount, "Error: firstcount ==  secondcount after create");
        }

        [TestMethod]
        public async Task TestRestServiceDeleteVacation()
        {
            await InitLogin();
            restService = new RestService(config, "Vacations");
            restService.Timeout = timeout;
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
            var firstcount = vacationInfoModelsList.Count;
            responce = await restService.Delete(vacationInfoModelsList[vacationInfoModelsList.Count - 1].Id);//delete last vacation                
            Assert.AreEqual(responce.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return error");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/delete return Forbidden");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/delete return NotFound");
            Assert.IsTrue(responce.IsSuccessStatusCode, "Error: Rest.Delete Is Not Success Status Code");
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
            var secondcount = vacationInfoModelsList.Count;
            Assert.AreNotEqual(firstcount, secondcount, "Error: firstcount !=  secondcount after delete (create and delete vacation)");
        }

        [TestMethod]
        public async Task TestRestServiceSaveImage()
        {
            await InitLogin();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "person.png");
            byte[] image = File.ReadAllBytes(path);
            Assert.IsNotNull(image, "Message: byte[] image IS NULL");
            //Get
            restService = new RestService(config, "Vacations");
            restService.Timeout = timeout;
            vacationInfoModelsList = await restService.Get<VacationInfoModel>();
            Assert.IsNotNull(vacationInfoModelsList, "Message: " + config + "/api/vacations return null");
            vacationInfoModel = await restService.Get<VacationInfoModel>(id);
            Assert.IsNotNull(vacationInfoModel, "Message: " + config + "/api/vacations?id=" + id + " return null");
            Assert.IsNull(vacationInfoModel.VacationForm, "Message: vacationInfoModel.VacationForm IS NOT NULL before saving");
            //Save
            vacationInfoModel.VacationForm = image;
            responce = await restService.Post(vacationInfoModel); //update vacation
            Assert.AreEqual(responce.StatusCode, System.Net.HttpStatusCode.OK, "Message: " + config + "/api/vacations/post return error");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return NoContent");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/post return Forbidden");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/post return NotFound");
            Assert.IsTrue(responce.IsSuccessStatusCode, "Error: Rest.Post Is Not Success Status Code");
            VacationInfoModel newvacation = await restService.Get<VacationInfoModel>(id);
            Assert.IsNotNull(newvacation, "Message: " + config + "/api/vacations?id=" + id + " return null");
            Assert.IsNotNull(newvacation.VacationForm, "Message: newvacation.VacationForm IS NULL after saving");
            byte[] savedimage = JsonConvertorExtention.FromJsonString<byte[]>(newvacation.VacationForm.ToJsonString());
            Assert.IsNotNull(savedimage, "Message: savedimage byte[] IS NULL");
            Assert.AreEqual(image[0], savedimage[0], "Message: image byte[0] NOT Equal after save");
            Assert.AreEqual(image.Length, savedimage.Length, "Message: image byte.length NOT Equal after save");
            //Delete
            newvacation.VacationForm = null;
            responce = await restService.Post(newvacation); //delete Image in vacation
            Assert.AreEqual(responce.StatusCode, System.Net.HttpStatusCode.OK, "Message: " + config + "/api/vacations/post return error");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NoContent, "Message: " + config + "/api/vacations/post return NoContent");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.Forbidden, "Message: " + config + "/api/vacations/post return Forbidden");
            Assert.AreNotEqual(responce.StatusCode, System.Net.HttpStatusCode.NotFound, "Message: " + config + "/api/vacations/post return NotFound");
            Assert.IsTrue(responce.IsSuccessStatusCode, "Error: Rest.Post Is Not Success Status Code");
            VacationInfoModel deletedimagevacation = await restService.Get<VacationInfoModel>(id);
            Assert.IsNotNull(deletedimagevacation, "Message: " + config + "/api/vacations?id=" + id + " return null");
            Assert.IsNull(deletedimagevacation.VacationForm, "Message: newvacation.VacationForm IS NOT NULL after deleting");
        }
    }
}
