using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelCrm.Data
{
    public class t_uv : BaseModel
    {
        public DateTime? DateUv { get; set; }
        public string NomUv { get; set; }
        public decimal Summa { get; set; } = 0.0M;
        public string NoteText { get; set; }
        public string LsAdb { get; set; }
        public DateTime? DateExp { get; set; }
       
    }
}
