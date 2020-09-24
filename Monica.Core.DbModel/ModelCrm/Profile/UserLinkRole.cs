using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm.Profile
{
    /// <summary>
    /// Связь многие ко многим между пользователем и ролью
    /// </summary>
    public class UserLinkRole : BaseModel
    {
        [ForeignKey("UserId")]
        public User User { get; set; }
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
