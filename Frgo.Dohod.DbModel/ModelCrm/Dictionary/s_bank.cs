using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_bank : BaseModel
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Bic { get; set; }
        public string Town { get; set; }
        public string CorrAcc { get; set; }
        public bool IsDepart { get; set; }
        public DateTime? DateClosed { get; set; }
    }
}
