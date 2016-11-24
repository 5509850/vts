using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VTS.Tests.Ninject;
using VTS.Core.Data.Models;
using System.Collections.Generic;
using VTS.Core.Data.WebServices.Concrete;
using VTS.Core.Data.MockModel;
using System.IO;
using VTS.Core.CrossCutting.Extensions;

namespace VTS.Tests
{
    [TestClass]
    public class TestSqlServices : Test
    {
        [TestMethod]
        public async Task TestSQLServiceLogin()
        {
            await InitData();
            LoginDataService loginDataService = FactorySingleton.FactoryOffline.Get<LoginDataService>();
            var result = await loginDataService.LogInOffline("vasya", "secret");
            Assert.IsNotNull(result, "Error: loginDataService.LogInOffline Is Null");
            Assert.IsTrue(result.LoginSuccess, "Error: loginDataService.LogInOffline is not success");
            result = await loginDataService.LogInOffline("vasya", "secret2");
            Assert.IsNotNull(result, "Error: loginDataService.LogInOffline Is Null");
            Assert.IsFalse(result.LoginSuccess, "Error: loginDataService.LogInOffline wrong password is not false result");
            result = await loginDataService.LogInOffline("vasya2", "secret");
            Assert.IsNotNull(result, "Error: loginDataService.LogInOffline Is Null");
            Assert.IsFalse(result.LoginSuccess, "Error: loginDataService.LogInOffline  wrong name is not false result");
        }

        [TestMethod]
        public async Task TestSQLServiceGetVacations()
        {
            await InitData();
            VacationsDataService _vacationsDataService = FactorySingleton.FactoryOffline.Get<VacationsDataService>();
            List<VTSModel> list = await _vacationsDataService.GetVacationListFromSQL();
            Assert.IsNotNull(list, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.AreEqual(list.Count, (new VacationInfoMockModel().Vacations).Count, "Error: list.Count from SQL != VacationInfoMockModel");
        }

        [TestMethod]
        public async Task TestSQLServiceGetVacationById()
        {
            await InitData();
            VacationsDataService _vacationsDataService = FactorySingleton.FactoryOffline.Get<VacationsDataService>();
            VacationInfoModel _vacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_vacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.AreEqual(_vacationInfoModel.Id, id, "Error: ID != from request");
            _vacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(0);
            Assert.IsNull(_vacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() get infomodel by zero ID is not Null");
        }

        [TestMethod]
        public async Task TestSQLServiceUpdateVacationById()
        {
            await InitData();
            VacationsDataService _vacationsDataService = FactorySingleton.FactoryOffline.Get<VacationsDataService>();
            VacationInfoModel _vacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_vacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            _vacationInfoModel.Status.Value = "sqltest";
            VacationInfoModel _notUpdatedVacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_notUpdatedVacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.AreNotEqual(_notUpdatedVacationInfoModel.Status.Value, _vacationInfoModel.Status.Value, "Error: _notUpdatedVacationInfoModel == _vacationInfoModel (changed field)");
            await _vacationsDataService.UpdateOrCreateVacationsSql(_vacationInfoModel);
            VacationInfoModel _updatedVacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_updatedVacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.AreEqual(_vacationInfoModel.Status.Value, _updatedVacationInfoModel.Status.Value, "Error: _vacationInfoModel != _updatedVacationInfoModel");
        }

        [TestMethod]
        public async Task TestSQLServiceCreateVacation()
        {
            await InitData();
            VacationsDataService _vacationsDataService = FactorySingleton.FactoryOffline.Get<VacationsDataService>();
            VacationInfoModel _vacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(0);
            Assert.IsNull(_vacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() ZERO ID Is not Null");
            _vacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_vacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            _vacationInfoModel.Id = 0;
            _vacationInfoModel.Status.Value = "new";
            await _vacationsDataService.UpdateOrCreateVacationsSql(_vacationInfoModel);
            VacationInfoModel _newVacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(0);
            Assert.IsNotNull(_newVacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() ZERO ID Is Null");
            Assert.AreEqual(_vacationInfoModel.Id, _newVacationInfoModel.Id, "Error: Id not equal");
            Assert.AreEqual(_vacationInfoModel.Status.Value, _newVacationInfoModel.Status.Value, "Error: value not equal");
            await _vacationsDataService.DeleteVacationsInfoInSqlById(0);
            _vacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(0);
            Assert.IsNull(_vacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() ZERO ID Is not Null");
        }

        [TestMethod]
        public async Task TestSQLServiceDeleteVacation()
        {
            await InitData();
            VacationsDataService _vacationsDataService = FactorySingleton.FactoryOffline.Get<VacationsDataService>();
            List<VTSModel> list = await _vacationsDataService.GetVacationListFromSQL();
            Assert.IsNotNull(list, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.AreEqual(list.Count, (new VacationInfoMockModel().Vacations).Count, "Error: list.Count from SQL != VacationInfoMockModel");
            VTSModel deletedVTS = list[list.Count - 1];
            int idDeleted = deletedVTS.Id;
            await _vacationsDataService.DeleteVacationsInSql(deletedVTS);
            list = await _vacationsDataService.GetVacationListFromSQL();
            Assert.IsNotNull(list, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.AreEqual(list.Count, (new VacationInfoMockModel().Vacations).Count - 1, "Error: list.Count from SQL == VacationInfoMockModel after delete");
            foreach (VTSModel vts in list)
            {
                Assert.AreNotEqual(vts.Id, idDeleted, string.Format("Error:  vacationsDataService.DeleteVacationsInSql ID = {0} deleted item is exist after delete", vts.Id));
            }
        }

        [TestMethod]
        public async Task TestSQLServiceSaveImage()
        {
            await InitData();
            string path = Path.Combine(Directory.GetCurrentDirectory(), "person.png");
            byte[] image = File.ReadAllBytes(path);
            //get
            VacationsDataService _vacationsDataService = FactorySingleton.FactoryOffline.Get<VacationsDataService>();
            VacationInfoModel _vacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_vacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.IsNull(_vacationInfoModel.VacationForm, "Message: vacationInfoModel.VacationForm IS NOT NULL before saving");
            Assert.IsNotNull(image, "Error: image byte[] Is Null");
            //save
            _vacationInfoModel.VacationForm = image;
            await _vacationsDataService.UpdateOrCreateVacationsSql(_vacationInfoModel);
            VacationInfoModel _updatedVacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_updatedVacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.IsNotNull(_updatedVacationInfoModel.VacationForm, "Error: VacationForm Is Null");
            byte[] savedimage = JsonConvertorExtention.FromJsonString<byte[]>(_updatedVacationInfoModel.VacationForm.ToJsonString());
            Assert.IsNotNull(savedimage, "Error: savedimage byte[] Is Null");
            Assert.AreEqual(image[0], savedimage[0], "Message: image byte[0] NOT Equal after save");
            Assert.AreEqual(image.Length, savedimage.Length, "Message: image byte.length NOT Equal after save");
            //delete
            _vacationInfoModel.VacationForm = null;
            await _vacationsDataService.UpdateOrCreateVacationsSql(_vacationInfoModel);
            _updatedVacationInfoModel = await _vacationsDataService.GetVacationByIdFromSql(id);
            Assert.IsNotNull(_updatedVacationInfoModel, "Error: vacationsDataService.GetVacationListFromSQL() Is Null");
            Assert.IsNull(_updatedVacationInfoModel.VacationForm, "Error: VacationForm Is Not Null after delete");
        }
    }
}
