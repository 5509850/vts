using System.Collections.Generic;
using System.Linq;
using VTS.Core.Data.Models;

namespace VTS.Core.Data.MockModel
{
    public class VacationInfoMockModel
    {
        public List<VacationInfoModel> Vacations = new List<VacationInfoModel>() { };

        public static readonly PersonModel[] PersonModels =
        {
            new PersonModel()
            {
                Email = "Vasil_Pupkin@epam.com",
                FullName = "Vasil Pupkin",
                Id = "123438723984"
            },
            new PersonModel()
            {
                Email = "Dasha_Pupkina@epam.com",
                FullName = "Dasha Pupkina",
                Id = "12837487532"
            },
        };

        public static PersonModel Vasil
        {
            get { return PersonModels[0]; }
        }

        public static PersonModel Dasha
        {
            get { return PersonModels[1]; }
        }

        public static IconedValueModel Closed
        {
            get
            {
                return new IconedValueModel()
                {
                    Icon = "greenCircle.png",
                    Key = "closed",
                    Value = "Closed"
                };
            }
        }

        public static IconedValueModel Cancelled
        {
            get
            {
                return new IconedValueModel()
                {
                    Icon = "redCircle.png",
                    Key = "cancelled",
                    Value = "Cancelled"
                };
            }
        }

        public static IconedValueModel OVT
        {
            get
            {
                return new IconedValueModel()
                {
                    Icon = "",
                    Key = "OVT",
                    Value = "Overtime (OVT)"
                };
            }
        }

        public static IconedValueModel ILL
        {
            get
            {
                return new IconedValueModel()
                {
                    Icon = "",
                    Key = "ILL",
                    Value = "Illness (ILL)"
                };
            }
        }

        public static IconedValueModel VAC
        {
            get
            {
                return new IconedValueModel()
                {
                    Icon = "",
                    Key = "VAC",
                    Value = "Regular (VAC)"
                };
            }
        }

        public VacationInfoMockModel()
        {
            Add(new VacationInfoModel()
            {
                Approver = Vasil,
                ConfirmationDocumentAvailable = true,
                Duration = 28800000,
                Employee = Vasil,
                EndDate = 1429984800000,
                StartDate = 1429952400000,
                Status = Closed,
                Type = OVT,
            });

            Add(new VacationInfoModel()
            {
                Approver = Dasha,
                ConfirmationDocumentAvailable = false,
                Duration = 28800000,
                Employee = Vasil,
                EndDate = 1428084000000,
                StartDate = 1428051600000,
                Status = Cancelled,
                Type = ILL,
            });

            Add(new VacationInfoModel()
            {
                Approver = Dasha,
                ConfirmationDocumentAvailable = false,
                Duration = 57600000,
                Employee = Vasil,
                EndDate = 1422295200000,
                StartDate = 1422003600000,
                Status = Closed,
                Type = VAC,
            });

            Add(new VacationInfoModel()
            {
                Approver = Dasha,
                ConfirmationDocumentAvailable = false,
                Duration = 28800000,
                Employee = Vasil,
                EndDate = 1422036000000,
                StartDate = 1422003600000,
                Status = Closed,
                Type = VAC,
            });

            Add(new VacationInfoModel()
            {
                Approver = Dasha,
                ConfirmationDocumentAvailable = false,
                Duration = 28800000,
                Employee = Vasil,
                EndDate = 1428084000000,
                StartDate = 1428051600000,
                Status = Cancelled,
                Type = ILL,
            });

            Add(new VacationInfoModel()
            {
                Approver = Dasha,
                ConfirmationDocumentAvailable = true,
                Duration = 28800000,
                Employee = Vasil,
                EndDate = 1421776800000,
                StartDate = 1421744400000,
                Status = Closed,
                Type = OVT,
            });

            Add(new VacationInfoModel()
            {
                Approver = Dasha,
                ConfirmationDocumentAvailable = true,
                Duration = 28800000,
                Employee = Vasil,
                EndDate = 1420567200000,
                StartDate = 1419843600000,
                Status = Cancelled,
                Type = VAC,
            });
        }

        public void Add(VacationInfoModel info)
        {
            info.Id = Vacations.Any() ? Vacations.Max(x => x.Id) + 1 : 1;
            info.ProcessInstanceId = "10" + info.Id;
            Vacations.Add(info);
        }

        public IEnumerable<VacationInfoModel> List()
        {
            return Vacations;
        }

        public VacationInfoModel Get(int id)
        {
            return Vacations.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(int id)
        {
            Vacations.RemoveAll(x => x.Id == id);
        }
    }
}


