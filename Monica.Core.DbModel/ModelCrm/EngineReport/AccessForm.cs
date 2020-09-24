using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelCrm.Profile;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    /// <summary>
    /// Основной класс для получения доступа к элементам. На основании этого описателя можно будет определять,
    /// кто может получить доступ, а кто нет
    /// </summary>
    public class AccessForm : BaseModel
    {
        /// <summary>
        /// Ссылка на форму
        /// </summary>
        [ForeignKey("FormModelId")]
        public FormModel FormModel { get; set; }
        /// <summary>
        /// Ссылка на форму
        /// </summary>
        public int? FormModelId { get; set; }
        /// <summary>
        /// Сссылка на поле, если нужно доступ наложить на конкретное поле 
        /// </summary>
        [ForeignKey("FieldId")]
        public Field Field { get; set; }
        /// <summary>
        /// Ссылка на поле
        /// </summary>
        public int? FieldId { get; set; }
        /// <summary>
        /// Тип наклажываемого ограничения
        /// </summary>
        public TypeAccec TypeAccec { get; set; }
        /// <summary>
        /// Ссылка на кнопку
        /// </summary>
        public int? ButtonFormId { get; set; }
        [ForeignKey("ButtonFormId")]
        public ButtonForm ButtonForm { get; set; }
        /// <summary>
        /// Ссылка на роль пользователя для которой вступают ограничения
        /// </summary>
        public int? UserRoleId { get; set; }
        [ForeignKey("UserRoleId")]
        public UserRole UserRole { get; set; }
    }
}
