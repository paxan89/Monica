using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm.Profile
{
    /// <summary>
    /// Пользователи системы
    /// </summary>
    public class User : BaseModel
    {
        /// <summary>
        /// Уникальное имя пользователя
        /// </summary>
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string Middlename { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email адрес обязательно с подтверждением
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Тип пользователя. Игрок, Менеджер
        /// </summary>
        [ForeignKey("TypeUserId")]
        public TypeUser TypeUser { get; set; }
        /// <summary>
        /// Тип пользователя
        /// </summary>
        public int? TypeUserId { get; set; }
    }
}
