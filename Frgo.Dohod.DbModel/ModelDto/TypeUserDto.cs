using Monica.CrmDbModel.ModelDto.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelDto
{
    public class UserRoleDto : BaseModelDto
    {
        /// <summary>
        /// Название роли
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// SysId роли
        /// </summary>
        public int SysIsd { get; set; }
        // public string Sysname { get; set; }
    }
}
