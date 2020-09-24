using System;
using System.ComponentModel.DataAnnotations;

namespace Frgo.Dohod.DbModel.ModelCrm.Fk
{
    public class fk_doc 
    {
        [Key]
        public int Sysid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Nfrm { get; set; }
        public string Group { get; set; }
        public string Format { get; set; }
        public string MaketPreview { get; set; }
        public string MaketImport { get; set; }
        public string MaketFull { get; set; }
    }
}
