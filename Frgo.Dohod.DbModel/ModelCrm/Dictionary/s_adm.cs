using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_adm : BaseModel
    {
        public string CodeAdm { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

       // public bool? IsDeleted { get; set; }
    }
}
