using System.ComponentModel.DataAnnotations;
using Frgo.Dohod.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.ModelCrm.Profile
{
    /// <summary>
    /// Тип пользователя системы.
    /// </summary>
    public class TypeUser : BaseModel
    {
        /// <summary>
        /// Имя типа пользователя. Менеджер, Клиент, Руководитель и т.д.
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// Системное имя 
        /// </summary>
        [MaxLength(50)]
        public string Sysname { get; set; }
    }
}
