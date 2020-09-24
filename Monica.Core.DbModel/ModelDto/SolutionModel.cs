using System;
using System.Collections.Generic;
using System.Text;

namespace Monica.Core.DbModel.ModelDto
{
    public class SolutionModel
    {
        public int Id { get; set; }
        public int IdModel { get; set; }
        public string Text { get; set; }
        public bool Expanded { get; set; }
        public string TableName { get; set; }
        public int ParentId { get; set; }
        public string VueComponent { get; set; }
        public string FieldWhere { get; set; }
    }
}
