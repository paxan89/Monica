using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frgo.Dohod.DbModel.ModelCrm.Data
{
    public class t_uvdet : BaseModel
    {
        public int UvId { get; set; }
        [ForeignKey("UvId")]
        public t_uv Uv { get; set; }
        public bool? IsNp { get; set; }
        public string VidOper { get; set; }
        public DateTime? DateDoc { get; set; }
        public string NomDoc { get; set; }
        public string PlatName { get; set; }
        public string PlatInn { get; set; }
        public string PlatKpp { get; set; }
        public string PoluchName { get; set; }
        public string PoluchInn { get; set; }
        public string PoluchKpp { get; set; }
        public string Oktmo { get; set; }
        public string Kbk { get; set; }
        public string CodeCeli { get; set; }
        public decimal Summa { get; set; } = 0.0M;
        public DateTime? DateVyp { get; set; }
        public int? LinkExt { get; set; }
    }
}
