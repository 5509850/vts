using SQLite.Net.Attributes;

namespace VTS.Core.Data.Models
{
    /// <summary>
    /// Short VacationInfoModel
    /// </summary>
    [Table("VacationInfo")]
    public class VacationInfoDTO
    {

        [PrimaryKey]
        public int Id { get; set; }
        public string ImageSRC { get; set; }

        public string VacationType { get; set; }

        public string Date { get; set; }

        public string Status { get; set; }       

        public byte[] Image { get; set; }
    }
}

