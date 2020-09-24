using Frgo.Dohod.DbModel.ModelCrm.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frgo.Dohod.DbModel.ModelCrm.Data
{
    public class t_dohorig : BaseModel
    {
        public string Oktmo { get; set; }
        public string OktmoPp { get; set; }
        public string Kbk { get; set; }
        public string KbkPp { get; set; }
        public string CodeCeli { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string PlatName { get; set; }
        public string Rs { get; set; }
        public string Bic { get; set; }
        public string KorSchet { get; set; }
        public string BankName { get; set; }
        public int DohodId { get; set; }
        [ForeignKey("DohodId")]
        public t_dohod Dohod { get; set; }
    }
}
