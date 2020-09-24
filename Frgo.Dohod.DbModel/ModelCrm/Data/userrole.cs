using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frgo.Dohod.DbModel.ModelCrm.Data
{
    public class userrole
    {
        /// <summary>
        /// Системный номер записи
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Ссылка на t_levelorg
        /// </summary>
        public int LevelOrgId { get; set; }
        /// <summary>
        /// Наименование роли
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Системное имя
        /// </summary>
        public string Sysname { get; set; }
        /// <summary>
        /// Признак удаленной записи
        /// </summary>
        public bool? IsDeleted { get; set; }
        [ForeignKey("LevelOrgId")]
        public t_levelorg Levelorg { get; set; }
    }
}
