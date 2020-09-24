using System.ComponentModel.DataAnnotations.Schema;
using Monica.Core.DbModel.ModelCrm.Core;

namespace Monica.Core.DbModel.ModelCrm.EngineReport
{
    /// <summary>
    /// Тип формы. Это фактически режим который нужно чтобы открылся
    /// </summary>
    public class TypeForm : BaseModel
    {
        /// <summary>
        /// Отображаемое имя 
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Системное имя группы
        /// </summary>
        public string SysName { get; set; }
        /// <summary>
        /// Ссылка на родителя
        /// </summary>
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public TypeForm Parent { get; set; }

    }
}
