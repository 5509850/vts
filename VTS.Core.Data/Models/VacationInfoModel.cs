namespace VTS.Core.Data.Models
{   
    public class VacationInfoModel
    {
        public PersonModel Approver { get; set; }

        public bool ConfirmationDocumentAvailable { get; set; }

        public long Duration { get; set; }

        public string DurationStr { get; set; }

        public PersonModel Employee { get; set; }

        public long EndDate { get; set; }

        public int Id { get; set; }

        public string ProcessInstanceId { get; set; }

        public long StartDate { get; set; }

        public IconedValueModel Status { get; set; }

        public IconedValueModel Type { get; set; }

        public object VacationForm { get; set; }
    }
}