using Frgo.Dohod.DbModel.ModelDto.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frgo.Dohod.DbModel.ModelDto.Users
{
    public class UsersRemoveArgs : BaseRemoveArgs
    {
        public int RoleId { get; set; }
        public IEnumerable<string> Accounts { get; set; }
    }
}
