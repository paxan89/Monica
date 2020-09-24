using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Frgo.Dohod.DbModel.ModelCrm.Profile
{
    /// <summary>
    /// Связь многие ко многим между пользователем и ролью
    /// </summary>
    public class UserLinkRole : BaseModel
    {
        [ForeignKey("UserId")]
        public User user { get; set; }
        [ForeignKey("UserRoleId")]
        public UserRole UserRole { get; set; }
        /// <summary>
        /// Ссылка на пользователя
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Ссылка на роль
        /// </summary>
        public int UserRoleId { get; set; }
    }
}
