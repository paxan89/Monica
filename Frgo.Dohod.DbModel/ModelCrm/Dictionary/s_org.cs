using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.ModelCrm.Dictionary
{
    public class s_org : BaseModel
    {
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Oktmo { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int? TypeOrg { get; set; }
        public int? Dop1Id { get; set; }
        public int? Dop2Id { get; set; }
        public int? Dop3Id { get; set; }
        public string RukDolzn { get; set; }
        public string Rukovod { get; set; }
        public string Glavbuh { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string AdresJure { get; set; }
        public string AdresFakt { get; set; }
  }
}
