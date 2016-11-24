using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VTS.Tests.Ninject;
using VTS.Core.Data.Models;
using System.Collections.Generic;
using System.IO;
using VTS.Core.Business.ViewModel;
using VTS.Core.Data;
using VTS.Core.Data.WebServices.Concrete;
using VTS.Core.Data.MockModel;
using VTS.Core.CrossCutting;
using System;

namespace VTS.Tests
{  
    public class Test
    {
        public static bool onlineMode = true;
        public int id = 1;

        public async Task LoginViewModel()
        {            
            if (!onlineMode)
            {
                //for offline mode need first time online sign and save data for offline check in
                var lwmodel = FactorySingleton.Factory.Get<LoginViewModel>();
                Assert.IsNotNull(lwmodel, "Message: Get<LoginViewModel> is NULL");
                await lwmodel.SignIn("vasya", "secret");
            }
            LoginViewModel loginViewModel = (onlineMode) ? FactorySingleton.Factory.Get<LoginViewModel>() : FactorySingleton.FactoryOffline.Get<LoginViewModel>();
            Assert.IsNotNull(loginViewModel, "Message: Get<LoginViewModel> is NULL");
            Assert.IsFalse((await loginViewModel.SignIn("vasya1", "secret")), "Message: error wrong login data");
            Assert.IsFalse((await loginViewModel.SignIn("vasya", "secret1")), "Message: error wrong password data");
            Assert.IsTrue((await loginViewModel.SignIn("vasya", "secret")), "Message: error login data");
        }

        public async Task VacationsViewModel()
        {           
            VacationsViewModel vacationsViewModel = (onlineMode) ? FactorySingleton.Factory.Get<VacationsViewModel>() : FactorySingleton.FactoryOffline.Get<VacationsViewModel>();
            Assert.IsNotNull(vacationsViewModel, "Message: Get<VacationsViewModel> is NULL");
            List<VTSModel> vtsList = await vacationsViewModel.GetVTSList();
            Assert.IsNotNull(vtsList, "Message: GetVTSList error");
            Assert.IsNotNull((await vacationsViewModel.GetVacationInfo(vtsList[0].Id)), "Message: GetVacationInfo error");
            Assert.IsNull((await vacationsViewModel.GetVacationInfo(0)), "Message: GetVacationInfo error null by zero ID");
        }

        public async Task IntegrationCreateVacation()
        {          
            VacationsViewModel vacationsViewModel = (onlineMode) ? FactorySingleton.Factory.Get<VacationsViewModel>() : FactorySingleton.FactoryOffline.Get<VacationsViewModel>();
            List<VTSModel> vtsList = await vacationsViewModel.GetVTSList();
            Assert.IsNotNull(vtsList, "Message: GetVTSList error");

            VacationInfoModel vacationInfo = await vacationsViewModel.GetVacationInfo(vtsList[0].Id);
            Assert.IsNotNull(vacationInfo, "Message: GetVacationInfo error");

            var oldcount = vtsList.Count;
            VacationInfoModel vacationInfoNew = await vacationsViewModel.CreateDraftVacationInfo();
            Assert.IsNotNull(vacationInfoNew, "Message: CreateDraftVacationInfo error");
            vacationInfoNew.Type.Value = "TESTING";
            await vacationsViewModel.UpdateDraftVacationInfo(vacationInfoNew);
            if (onlineMode)
            {
                Assert.IsTrue((await vacationsViewModel.SendDraftVacationInfo()), "Message: error SendDraftVacationInfo");
                vtsList = await vacationsViewModel.GetVTSList();
                Assert.AreEqual(vtsList.Count, (oldcount + 1), "Message: error after Create newcount != oldcount + 1");
                VacationInfoModel vacationInfoSaved = await vacationsViewModel.GetVacationInfo(vtsList[vtsList.Count - 1].Id);
                Assert.IsNotNull(vacationInfoSaved, "Message: GetVacationInfo2 error");
                Assert.AreEqual(vacationInfoNew.Type.Value, vacationInfoSaved.Type.Value, "Message: error vacationInfoNew not Equals Type.Value vacationInfoSaved");
            }
        }

        public async Task IntegrationUpdateVacation()
        {            
            VacationsViewModel vacationsViewModel = (onlineMode) ? FactorySingleton.Factory.Get<VacationsViewModel>() : FactorySingleton.FactoryOffline.Get<VacationsViewModel>();
            List<VTSModel> vtsList = await vacationsViewModel.GetVTSList();
            Assert.IsNotNull(vtsList, "Message: GetVTSList error");
            var oldcount = vtsList.Count;
            VacationInfoModel vacationInfo = await vacationsViewModel.GetVacationInfo(vtsList[0].Id);
            Assert.IsNotNull(vacationInfo, "Message: GetVacationInfo error");
            vacationInfo.Type.Value = "updating";
            Assert.IsTrue((await vacationsViewModel.UpdateOrCreateVacationInfo(vacationInfo)), "Message: error UpdateOrCreateVacationInfo");
            Assert.AreEqual((await vacationsViewModel.GetVTSList()).Count, oldcount, "Message: error after Update newcount != oldcount");
            VacationInfoModel vacationInfoUpdated = await vacationsViewModel.GetVacationInfo(vtsList[0].Id);
            Assert.AreEqual(vacationInfoUpdated.Type.Value, vacationInfo.Type.Value, "Message: error vacationInfoNew not Equals Type.Value vacationInfoSaved");
        }

        public async Task IntegrationSaveImage()
        {           
            VacationsViewModel vacationsViewModel = (onlineMode) ? FactorySingleton.Factory.Get<VacationsViewModel>() : FactorySingleton.FactoryOffline.Get<VacationsViewModel>();
            List<VTSModel> vtsList = await vacationsViewModel.GetVTSList();
            Assert.IsNotNull(vtsList, "Message: GetVTSList error");
            VacationInfoModel vacationInfo = await vacationsViewModel.GetVacationInfo(vtsList[0].Id);
            Assert.IsNotNull(vacationInfo, "Message: GetVacationInfo error");
            string path = Path.Combine(Directory.GetCurrentDirectory(), "person.png");
            Assert.IsTrue(File.Exists(path), "Message: test image file not found in TEST project");
            byte[] imageByte = File.ReadAllBytes(path);
            Assert.IsNotNull(imageByte, "Message: File.ReadAllBytes file in byte is NULL");
            vacationsViewModel.Image = imageByte;
            Assert.IsNotNull(vacationsViewModel.Image, "Message: test image byte[] is Null");
            Assert.IsTrue((await vacationsViewModel.UpdateOrCreateVacationInfo(vacationInfo)), "Message: error UpdateOrCreateVacationInfo");
            vacationsViewModel.Image = null;
            VacationInfoModel vacationInfoUpdated = await vacationsViewModel.GetVacationInfo(vtsList[0].Id);
            Assert.IsNotNull(vacationsViewModel.Image, "Message:vacationsViewModel.Image after GetVacationInfo Is Null");
            Assert.AreEqual(vacationsViewModel.Image.Length, imageByte.Length, "Message: Not equal Length Image byte[] from viewmodel and real image");
            Assert.AreEqual(vacationsViewModel.Image[0], imageByte[0], "Message: Not equal first byte Image byte[] from viewmodel and real image");

            vacationsViewModel.Image = null;//delete Image
            Assert.IsNull(vacationsViewModel.Image, "Message: test image byte[] is not Null");
            Assert.IsTrue((await vacationsViewModel.UpdateOrCreateVacationInfo(vacationInfo)), "Message: error UpdateOrCreateVacationInfo");
            vacationInfoUpdated = await vacationsViewModel.GetVacationInfo(vtsList[0].Id);
            Assert.IsNull(vacationsViewModel.Image, "Message:vacationsViewModel.Image after GetVacationInfo Is not Null");
        }

        public async Task IntegrationDeleteVacation()
        {         
            VacationsViewModel vacationsViewModel = (onlineMode) ? FactorySingleton.Factory.Get<VacationsViewModel>() : FactorySingleton.FactoryOffline.Get<VacationsViewModel>();
            List<VTSModel> vtsList = await vacationsViewModel.GetVTSList();
            Assert.IsNotNull(vtsList, "Message: GetVTSList error");
            var oldcount = vtsList.Count;
            Assert.IsTrue(await vacationsViewModel.DeleteVacationInfo(vtsList[vtsList.Count - 1]), "Message: error DeleteVacationInfo");
            Assert.AreNotEqual((await vacationsViewModel.GetVTSList()).Count, oldcount, "Message: error after Delete newcount == oldcount");
        }

        public async Task InitData()
        {
            await FactorySingleton.FactoryOffline.Get<LoginDataService>().SaveLoginModelToSQLite("vasya", "secret");
            List<VacationInfoModel> _vacationList = new VacationInfoMockModel().Vacations;
            List<VTSModel> _vtsModelList = new List<VTSModel>();
            ModelConverter converter = new ModelConverter(FactorySingleton.FactoryOffline.Get<LocalizeService>());
            if (_vacationList != null)
            {
                foreach (VacationInfoModel info in _vacationList)
                {
                    _vtsModelList.Add(converter.ConvertToVTSModel(info));
                }
            }
            if (_vtsModelList != null && _vtsModelList.Count != 0)
            {
                await FactorySingleton.FactoryOffline.Get<VacationsDataService>().SaveVacationsToSql(_vtsModelList);
            }

            VacationInfoModel mockVacationInfoModel = new VacationInfoModel();
            mockVacationInfoModel.Id = id;
            mockVacationInfoModel.ConfirmationDocumentAvailable = true;
            mockVacationInfoModel.Duration = 28800000;
            mockVacationInfoModel.DurationStr = string.Empty;
            mockVacationInfoModel.EndDate = (long)(DateTime.Now.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
            mockVacationInfoModel.StartDate = (long)(DateTime.Now.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
            mockVacationInfoModel.ProcessInstanceId = string.Empty;
            mockVacationInfoModel.Approver = new PersonModel()
            {
                Email = "Dasha_Pupkina@epam.com",
                FullName = "Dasha Pupkina",
                Id = "12837487532",
                Region = ""
            };
            mockVacationInfoModel.Employee = new PersonModel()
            {
                Email = "Vasil_Pupkin@epam.com",
                FullName = "Vasil Pupkin",
                Id = "123438723984",
                Region = ""
            };
            mockVacationInfoModel.Status = new IconedValueModel()
            {
                Icon = "red-circle",
                Key = "waiting",
                Value = VacationStatus.Draft.ToString()
            };
            mockVacationInfoModel.Type = new IconedValueModel()
            {
                Icon = "",
                Key = "VAC",
                Value = "Regular (VAC)"
            };
            mockVacationInfoModel.VacationForm = null;
            await FactorySingleton.FactoryOffline.Get<VacationsDataService>().UpdateOrCreateVacationsSql(mockVacationInfoModel);
            await FactorySingleton.FactoryOffline.Get<VacationsDataService>().DeleteVacationsInfoInSqlById(0);//delete temp item for create draft
        }
    }
}

