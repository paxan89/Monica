using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_kbkdop : BaseModel
    {
        public int KbkId { get; set; }
        [ForeignKey("KbkId")]
        public s_kbk Kbk { get; set; }
        public int? OktmoId { get; set; }
        public DateTime? DateBeg { get; set; }
        public DateTime? DateEnd { get; set; }
        public double ProcFed { get; set; }
        public double ProcObl { get; set; }
        public double ProcMest { get; set; }
        public double ProcPos1 { get; set; }
        public double ProcPos2 { get; set; }
        public double ProcFond { get; set; }
        public double ProcSmol { get; set; }
        public int? NumBudgKop { get; set; }
        public bool? IsPriznak { get; set; }
        public bool? IsItog { get; set; }
        public bool? IsGr { get; set; }
        public DateTime? DateRad { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
