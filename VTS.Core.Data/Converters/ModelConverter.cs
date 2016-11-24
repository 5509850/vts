using System;
using System.Text;
using VTS.Core.CrossCutting;
using VTS.Core.CrossCutting.Extensions;
using VTS.Core.CrossCutting.Helpers;
using VTS.Core.Data.MockModel;
using VTS.Core.Data.Models;

namespace VTS.Core.Data
{
    public class ModelConverter
    {
        private ILocalizeService _localizer;

        public ModelConverter(ILocalizeService localizer)
        {
            _localizer = localizer;
        }
        public VTSModel ConvertToVTSModel(VacationInfoModel vacationInfo)
        {
            VTSModel vtsModel = new VTSModel();
            vtsModel.Id = vacationInfo.Id;
            if (vacationInfo.Type.Value != null)
            {
                vtsModel.VacationType = vacationInfo.Type.Value;
            }
            vtsModel.StartDate = vacationInfo.StartDate;
            vtsModel.EndDate = vacationInfo.EndDate;
            var startDate = LocalizeMonth((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(vacationInfo.StartDate).ToLocalTime().ToString("MMM dd, yyyy"));
            var endDate = LocalizeMonth((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(vacationInfo.EndDate).ToLocalTime().ToString("MMM dd, yyyy"));
            var _startDate = ConverterHelper.ConvertMillisecToDateTime(vacationInfo.StartDate);
            var _endDate = ConverterHelper.ConvertMillisecToDateTime(vacationInfo.EndDate);
            var dayCount = ConverterHelper.CalculateDuration(_startDate, _endDate);
            var days = string.Empty;
            if (dayCount == 1)
            {
                days = _localizer.Localize("day");
            }
            else
            {
                if (dayCount > 1 && dayCount < 5)
                {
                    days = _localizer.Localize("day24");
                }
                else
                {
                    days = _localizer.Localize("days");
                }
            }
            vtsModel.Date = startDate + " - " + endDate + " (" + dayCount + " " + days + ")";           
            if (vacationInfo.Status != null)
            {
                vtsModel.Status = vacationInfo.Status.Value;
            }

            if (vacationInfo.Type.Value != null)
            {
                vtsModel.VacationType = vacationInfo.Type.Value;
            }            
            return vtsModel;
        }
        public VacationInfoModel ConvertToVacationInfoModel(VTSModel vtsModel)
        {
            VacationInfoModel info = new VacationInfoModel();
            info.Id = vtsModel.Id;
            info.Status = VacationInfoMockModel.VAC;
            return info;
        }
        public VacationInfoDTO ConvertToVacationInfoDTO(VTSModel vts)
        {
            VacationInfoDTO vacationInfo = new VacationInfoDTO();
            vacationInfo.Id = vts.Id;
            vacationInfo.Date = vts.Date;
            vacationInfo.Status = vts.Status;
            vacationInfo.VacationType = vts.VacationType;
            vacationInfo.ImageSRC = (vts.ImageSRC == null) ? null : vts.ImageSRC.ToString();
            vacationInfo.Image = vts.Image;
            return vacationInfo;
        }
        public VTSModel ConvertToVTSModel(VacationInfoDTO vacationInfo)
        {
            VTSModel vts = new VTSModel();
            vts.Id = vacationInfo.Id;
            vts.Date = vacationInfo.Date;
            vts.Status = vacationInfo.Status;
            vts.VacationType = vacationInfo.VacationType;
            vts.Image = vacationInfo.Image;
            if (vacationInfo.ImageSRC != null)
            {
                vts.ImageSRC = new Uri(vacationInfo.ImageSRC);
            }

            return vts;
        }
        public string ConvertDateToString(long StartDate, long EndDate)
        {
            var start = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(StartDate);
            var end = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(EndDate);
            var startDate = start.ToLocalTime().ToString("MMM dd, yyyy");
            var endDate = end.ToLocalTime().ToString("MMM dd, yyyy");
            var dayCount = (end - start).TotalDays + 1;
            return startDate + " - " + endDate + " (" + dayCount + " days)";
        }
        private string LocalizeMonth(string month)
        {
            if (string.IsNullOrEmpty(month) || month.Length < 4)
            {
                return string.Empty;
            }

            string oldmonth = string.Format("{0}{1}{2}", month[0], month[1], month[2]);
            string newmonth = oldmonth;
            if (!_localizer.Localize(oldmonth).Equals("N/A"))
            {
                newmonth = _localizer.Localize(oldmonth);
            }            
            var sb = new StringBuilder();
            sb.Append(month);
            sb.Replace(oldmonth, newmonth);           
            return sb.ToString();
        }
        public VacationInfoModelDTO ConvertToVacationInfoModelDTO(VacationInfoModel vim)
        {
            VacationInfoModelDTO info = new VacationInfoModelDTO();
            //if (vim.Approver != null)
            //{
           
            //}
            if (vim != null)
            {
                info.Id = vim.Id;
                info.ConfirmationDocumentAvailable = vim.ConfirmationDocumentAvailable;
                info.Duration = vim.Duration;
                info.DurationStr = vim.DurationStr;
                info.EndDate = vim.EndDate;
                info.StartDate = vim.StartDate;
                info.ProcessInstanceId = vim.ProcessInstanceId;               
                info.Approver = (vim.Approver == null) ? null : JsonConvertorExtention.ToJsonString(vim.Approver);
                info.Employee = (vim.Employee == null) ? null : JsonConvertorExtention.ToJsonString(vim.Employee);
                info.Status = (vim.Status == null) ? null : JsonConvertorExtention.ToJsonString(vim.Status);
                info.Type = (vim.Type == null) ? null : JsonConvertorExtention.ToJsonString(vim.Type);
                info.VacationForm = (vim.VacationForm == null) ? null : JsonConvertorExtention.FromJsonString<byte[]>(vim.VacationForm.ToJsonString());                
            }
            return info;
        }
        public VacationInfoModel ConvertToVacationInfoModel(VacationInfoModelDTO vimDTO)
        {
            VacationInfoModel info = new VacationInfoModel();
            try
            {
                if (vimDTO != null)
                {
                    info.Id = vimDTO.Id;
                    info.ConfirmationDocumentAvailable = vimDTO.ConfirmationDocumentAvailable;
                    info.Duration = vimDTO.Duration;
                    info.DurationStr = vimDTO.DurationStr;
                    info.EndDate = vimDTO.EndDate;
                    info.StartDate = vimDTO.StartDate;
                    info.ProcessInstanceId = vimDTO.ProcessInstanceId;                   
                    info.Approver = (vimDTO.Approver == null) ? null : JsonConvertorExtention.FromJsonString<PersonModel>(vimDTO.Approver);
                    info.Employee = (vimDTO.Employee == null) ? null : JsonConvertorExtention.FromJsonString<PersonModel>(vimDTO.Employee);
                    info.Status = (vimDTO.Status == null) ? null : JsonConvertorExtention.FromJsonString<IconedValueModel>(vimDTO.Status);
                    info.Type = (vimDTO.Type == null) ? null : JsonConvertorExtention.FromJsonString<IconedValueModel>(vimDTO.Type);
                    info.VacationForm = vimDTO.VacationForm;
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }

            return info;
        }
    }
}