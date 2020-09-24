using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_kceli : BaseModel
    {
        public string CodeCeli { get; set; }
        public DateTime? DateBeg { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
