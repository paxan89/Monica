using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frgo.Dohod.DbModel.ModelCrm.Fk
{
    public class fk_code 
    {
        [Key]
        public int Sysid { get; set; }
        public int? DocId { get; set; }
        public string Marker { get; set; }
        public int? NumPos { get; set; }
        public int? NumOrder { get; set; }
        public string Code { get; set; }
        public string FldType { get; set; }
        public string FldLen { get; set; }
        public bool? IsFilled { get; set; }
        public string Descript { get; set; }
        public string Comments { get; set; }
        [ForeignKey("DocId")]
        public fk_doc FkDoc { get; set; }

    }
}
