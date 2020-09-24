using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_kbk : BaseModel
    {
        public string Kbk { get; set; }
        public int? OktmoId { get; set; } = null;
        public DateTime? DateBeg { get; set; } = null;
        public DateTime? DateEnd { get; set; } = null;
        public double ProcFed { get; set; } = 100.0;
        public double ProcObl { get; set; } = 0.0;
        public double ProcMest { get; set; } = 0.0;
        public double ProcPos1 { get; set; } = 0.0;
        public double ProcPos2 { get; set; } = 0.0;
        public double ProcFond { get; set; } = 0.0;
        public double ProcSmol { get; set; } = 0.0;
        public int? NumBudgKop { get; set; }
        public bool? IsPriznak { get; set; } 
        public bool? IsItog { get; set; }
        public bool? IsGr { get; set; }
        public DateTime? DateRad { get; set; } = null;
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
