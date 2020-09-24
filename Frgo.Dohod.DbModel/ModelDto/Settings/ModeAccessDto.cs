using Frgo.Dohod.DbModel.Models.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frgo.Dohod.DbModel.ModelDto.Settings
{
    public class ModeAccessDto
    {
        public IEnumerable<ModeItem> Data { get; set; }
        public IEnumerable<int> Selected { get; set; }
    }
}
