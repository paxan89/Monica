using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelCrm.Data
{
    public class t_dohod : BaseModel
    {
        public DateTime? Date { get; set; }
        public DateTime? DateOper { get; set; }
        public string NumPp { get; set; }
        public DateTime? DatePp { get; set; }
        public string NumZf { get; set; }
        public DateTime? DateZf { get; set; }
        public int? OktmoId { get; set; }
        public int? KbkId { get; set; }
        public int? CodeCeliId { get; set; }
        public int? PlatId { get; set; }
        public bool? IsRaschet { get; set; }
        public bool? IsSumRazb { get; set; }
        public decimal SumItg { get; set; } = 0.0M;
        public decimal SumFed { get; set; } = 0.0M;
        public decimal SumObl { get; set; } = 0.0M;
        public decimal SumMest { get; set; } = 0.0M;
        public decimal SumPgt { get; set; } = 0.0M;
        public decimal SumPst { get; set; } = 0.0M;
        public decimal SumFond { get; set; } = 0.0M;
        public decimal SumSmol { get; set; } = 0.0M;
        public string ImnsKpp { get; set; }
        public string ImnsInn { get; set; }
        public string LsAdb { get; set; }
        public string OsnPlat { get; set; }
        public string CodeVidOper { get; set; }
        public string VidOper { get; set; }
        public string NaznPlat { get; set; }
        public string ImportFile { get; set; }
        public bool? IsNp { get; set; }
        public int? LinkNp { get; set; }
        public bool? IsVoz { get; set; }
        public int? LinkVoz { get; set; }
        public bool? IsItg { get; set; }
        public int? LinkItg { get; set; }
        public string Guid { get; set; }
        public VidDoc? VidDoc { get; set; }
        public t_dohorig Orig { get; set; }
    }
}
