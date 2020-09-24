using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_calendar : BaseModel
    {
        public DateTime? Date { get; set; }
        public bool? IsWeekend { get; set; }
        public bool? IsNoPostDay { get; set; }
        public decimal Summa { get; set; } = 0.0M;
        public decimal SummaZf { get; set; } = 0.0M;
        public decimal SummaIp { get; set; } = 0.0M;
        public decimal SummaReestr { get; set; } = 0.0M;
    }
}
