using System;

namespace VTS.Core.Data.Models
{
    public class VTSModel
    {
        public int Id { get; set; }

        public Uri ImageSRC { get; set; }

        public string VacationType { get; set; }

        public string Date { get; set; }

        public string Status { get; set; }

        public long StartDate { get; set; }

        public long EndDate { get; set; }

        public byte[] Image { get; set; }
    }
}

