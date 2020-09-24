using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_orgokved : BaseModel
    {
        public int OrgId { get; set; }
        public string Okved { get; set; }
        public bool IsOsn { get; set; }
    }
}
