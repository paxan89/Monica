using System;

namespace Monica.Core.ModelParametrs.ModelsArgs
{
    /// <summary>
    /// Класс параметров для выбора данных полезователей системы
    /// </summary>
    public class UserArgs
    {
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public bool? IsDeleted { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Password { get; set; }
    }
}
