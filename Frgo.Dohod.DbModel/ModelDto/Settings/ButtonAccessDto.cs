using Frgo.Dohod.DbModel.Models.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frgo.Dohod.DbModel.ModelDto.Settings
{
    public class ButtonAccessDto
    {
        public IEnumerable<ButtonItem> Data { get; set; }
        public IEnumerable<int> Selected { get; set; }
    }
}
