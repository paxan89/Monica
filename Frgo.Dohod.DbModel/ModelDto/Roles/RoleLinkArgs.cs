using System;
using System.Collections.Generic;
using System.Text;

namespace Frgo.Dohod.DbModel.ModelDto.Roles
{
    public class RoleLinkArgs
    {
        public IEnumerable<int> IdUsers { get; set; }
        public int IdRole { get; set; }
    }
}
