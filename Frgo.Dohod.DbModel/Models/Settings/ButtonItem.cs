using System;
using System.Collections.Generic;
using System.Text;

namespace Frgo.Dohod.DbModel.Models.Settings
{
    public class ButtonItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int ButtonId { get; set; }
        public int FormId { get; set; }
        public bool IsButton { get; set; }
        public string Text { get; set; }
    }
}
