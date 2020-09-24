using System;
using System.ComponentModel.DataAnnotations;

namespace Frgo.Dohod.DbModel.ModelCrm.Core
{
    /// <summary>
    /// Базовая модель для EnitityCore
    /// </summary>
    public class BaseModelGuid
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public BaseModelGuid()
        {
            CreateDate = DateTime.Now;
            IsDeleted = false;
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Первичный ключ модели
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// Дата добавления записи
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Признак удаленной записи
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}