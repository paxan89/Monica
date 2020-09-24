using System;
using System.ComponentModel.DataAnnotations;

namespace Frgo.Dohod.DbModel.ModelCrm.Core
{
    /// <summary>
    /// Базовая модель для EnitityCore
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public BaseModel()
        {
            //AppDate = DateTime.Now;
            //IsDeleted = false;
        }

        /// <summary>
        /// Первичный ключ модели
        /// </summary>
        [Key]
        public int Sysid { get; set; }

        /// <summary>
        /// Дата добавления записи
        /// </summary>
        public DateTime? AppDate { get; set; }

        /// <summary>
        /// Id пользователя добавившего запись
        /// </summary>
        public int? AppUser { get; set; }

        /// <summary>
        /// Дата последней коррекции записи
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// Id пользователя последней коррекции записи
        /// </summary>
        public int? EditUser { get; set; }

        /// <summary>
        /// Признак удаленной записи
        /// </summary>
       // public bool? IsDeleted { get; set; }
    }
}
