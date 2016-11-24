using System.Collections.Generic;
using System.Threading.Tasks;
using VTS.Core.Data.Models;
using VTS.Core.CrossCutting;
using System;
using VTS.Core.CrossCutting.Helpers;
using VTS.Core.CrossCutting.Extensions;

namespace VTS.Core.Business.ViewModel
{
    public class VacationsViewModel : BaseViewModel
    {
        private INetworkReachability _networkReachability;
        private IVacationManager _vacationManager;
        private string _errorMessage;
        public VacationsViewModel
            (
            IVacationManager vacationManager,
            ILocalizeService localizer,
            INetworkReachability networkReachability
            )
        {
            _vacationManager = vacationManager;
            this.Localaizer = localizer;
            _networkReachability = networkReachability;
            LocalizationModel();
        }        
        #region public property
        public UserState State { get; internal set; }
        public ILocalizeService Localaizer { get; set; }
        public string Header { get; internal set; }
        public string Employee { get; internal set; }
        public string VacationType { get; internal set; }
        public string Approver { get; internal set; }        
        public string Duration { get; internal set; }
        public string Status { get; internal set; }
        public string Attachments { get; internal set; }
        public string Submit { get; internal set; }
        public string Gallery { get; internal set; }
        public string Camera { get; internal set; }
        public byte[] Image { get; set; }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
        #endregion
        #region public Methods
        public bool IsOnlineMode
        {
            get
            {
                return _networkReachability.IsOnlineMode;
            }
        }      
        public async Task<List<VTSModel>> GetVTSList()
        {
            List<VTSModel> vacationsViewModelList = new List<VTSModel>();
            try
            {
                if (IsOnlineMode)
                {
                    vacationsViewModelList = await _vacationManager.GetVacationListFromRest(this);
                    if (vacationsViewModelList != null)
                    {
                        await _vacationManager.SaveVacationListToSql(vacationsViewModelList);
                    }
                }
                else
                {
                    vacationsViewModelList = await _vacationManager.GetVacationListFromSQL();
                }
            }
            catch (Exception ex)
            {
                throw ex; 
            }            
            return LocalizeVTSModel(vacationsViewModelList);
        }       
        public async Task<bool> UpdateOrCreateVacationInfo(VacationInfoModel oldtem)
        {
            if (oldtem == null)
            {
                return false;
            }
            if (Image != null && Image.Length != 0)
            {
                oldtem.VacationForm = Image;
            }
            else
            {
                oldtem.VacationForm = null;
            }
            bool result = false;
            if (IsOnlineMode)
            {
                result = await _vacationManager.UpdateOrCreateVacationInRest(this, oldtem);
                await _vacationManager.UpdateOrCreateVacationInSql(oldtem);
            }
            else
            {
                await _vacationManager.UpdateOrCreateVacationInSql(oldtem);
                result = true;
            }
            return await Helper.Complete(result);
        }
        public async Task<bool> DeleteVacationInfo(VTSModel item)
        {
            return await _vacationManager.DeleteVacation(this, item);
        }
        public async Task<VacationInfoModel> GetVacationInfo(int id)
        {
            VacationInfoModel vacationInfoModel = null;
            if (IsOnlineMode)
            {
                vacationInfoModel = await _vacationManager.GetVacationFromRestByID(this, id);
                if (vacationInfoModel != null)
                {
                    await _vacationManager.UpdateOrCreateVacationInSql(vacationInfoModel);
                }
            }
            else
            {
                vacationInfoModel = await _vacationManager.GetVacationFromSqlByID(this, id);
            }

            if (vacationInfoModel != null && vacationInfoModel.VacationForm != null)
            {
                Image = JsonConvertorExtention.FromJsonString<byte[]>(vacationInfoModel.VacationForm.ToJsonString());
            }
            else
            {
                Image = null;
            }             
            return vacationInfoModel;
        }
        public async Task<VacationInfoModel> CreateDraftVacationInfo()
        {
            VacationInfoModel vacInfo = await _vacationManager.GetVacationFromSqlByID(this, 0);
            if (vacInfo == null)
            {
                vacInfo = new VacationInfoModel();
                vacInfo.Id = 0;
                vacInfo.ConfirmationDocumentAvailable = true;
                vacInfo.Duration = 28800000;
                vacInfo.DurationStr = string.Empty;
                vacInfo.EndDate = (long)(DateTime.Now.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
                vacInfo.StartDate = (long)(DateTime.Now.Date - new DateTime(1970, 1, 1)).TotalMilliseconds;
                vacInfo.ProcessInstanceId = string.Empty;
                vacInfo.Approver = new PersonModel()
                {
                    Email = "Dasha_Pupkina@epam.com",
                    FullName = "Dasha Pupkina",
                    Id = "12837487532",
                    Region = ""
                };
                vacInfo.Employee = new PersonModel()
                {
                    Email = "Vasil_Pupkin@epam.com",
                    FullName = "Vasil Pupkin",
                    Id = "123438723984",
                    Region = ""
                };
                vacInfo.Status = new IconedValueModel()
                {
                    Icon = "red-circle",
                    Key = "waiting",
                    Value = VacationStatus.Draft.ToString()
                };
                vacInfo.Type = new IconedValueModel()
                {
                    Icon = "",
                    Key = "VAC",
                    Value = "Regular (VAC)"
                };
                vacInfo.VacationForm = null;
                await _vacationManager.UpdateOrCreateVacationInSql(vacInfo);
            }
            else
            {
                vacInfo.Status = new IconedValueModel()
                {
                    Icon = "red-circle",
                    Key = "waiting",
                    Value = VacationStatus.Draft.ToString()
                };
            }
            return await Helper.Complete(vacInfo);
        }
        public async Task UpdateDraftVacationInfo(VacationInfoModel vacInfo)
        {         
            await _vacationManager.UpdateOrCreateVacationInSql(vacInfo);
            return;
        }
        public async Task<bool> SendDraftVacationInfo()
        {
            VacationInfoModel vacationInfoModel = await _vacationManager.GetVacationFromSqlByID(this, 0);
            if (vacationInfoModel == null)
            {
                return false;
            }
            if (Image != null && Image.Length != 0)
            {
                vacationInfoModel.VacationForm = Image;
            }
            bool result = false;
            if (IsOnlineMode)
            {
                result = await _vacationManager.UpdateOrCreateVacationInRest(this, vacationInfoModel);
                if (result)
                {
                    await _vacationManager.DeleteVacationsInfoInSqlById(0);
                }
            }
            else
            {                
                result = false;
            }
            return await Helper.Complete(result);
        }
        #endregion

        #region private fields and property
        private void LocalizationModel()
        {
            Header = Localaizer.Localize("RegularVacation");
            Employee = Localaizer.Localize("EmployeeName");
            VacationType = Localaizer.Localize("Type");
            Approver = Localaizer.Localize("Approver");            
            Duration = Localaizer.Localize("Duration");
            Status = Localaizer.Localize("status");
            Attachments = Localaizer.Localize("Attachments");
            Submit = Localaizer.Localize("SubmitForRequest");
            Camera = Localaizer.Localize("camera");
            Gallery = Localaizer.Localize("gallery");
        }

        private List<VTSModel> LocalizeVTSModel(List<VTSModel> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                list[i].VacationType = Localaizer.Localize(list[i].VacationType);                
            }
            return list;
        }
        #endregion
    }
}