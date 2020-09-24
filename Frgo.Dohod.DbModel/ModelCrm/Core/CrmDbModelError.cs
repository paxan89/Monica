using System.Collections.Generic;

namespace Frgo.Dohod.DbModel.ModelCrm.Core
{
    /// <summary>
    /// Модель данных для отображения ошибко при обращении к CRM
    /// </summary>
    public class CrmDbModelError
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Description { get; set; }

    }

    /// <summary>
    /// Результат работы обращения к БД
    /// </summary>
    public class ResultCrmDb
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ResultCrmDb()
        {
            Errors = new List<CrmDbModelError>();
            Succeeded = true;
        }

        /// <summary>
        /// Объект, который был получен как результат работы обращения к БД
        /// </summary>
        public object Result { get; set; }
        /// <summary>
        /// Признак успешного обращения к БД
        /// </summary>
        public bool Succeeded { get; set; }
        /// <summary>
        /// Спиок возникших ошибок
        /// </summary>
        public List<CrmDbModelError> Errors { get; set; }
        /// <summary>
        /// Добавить новый код ошибки
        /// </summary>
        /// <param name="code">Код</param>
        /// <param name="decription">Описание</param>
        public void AddError(string code, string decription)
        {
            Succeeded = false;
            Errors.Add(new CrmDbModelError { Code = code, Description = decription });
        }
    }
}
