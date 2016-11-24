using SQLite.Net.Attributes;

namespace VTS.Core.Data.Models
{
    /// <summary>
    /// Full VacationInfoModel
    /// </summary>
    [Table("VacationInfoModel")]
    public class VacationInfoModelDTO
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Approver { get; set; }

        public bool ConfirmationDocumentAvailable { get; set; }

        public long Duration { get; set; }

        public string DurationStr { get; set; }

        public string Employee { get; set; }

        public long EndDate { get; set; }        

        public string ProcessInstanceId { get; set; }

        public long StartDate { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public byte[] VacationForm { get; set; }
    }
}
