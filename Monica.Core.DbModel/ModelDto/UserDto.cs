namespace Monica.Core.DbModel.ModelDto
{
    public class UserDto 
    {
        /// <summary>
        /// Уникальный номер пользователя
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Аккаунт пользователя
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Короткое имя пользователя
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Тип пользователя
        /// </summary>
        public string TypeUser { get; set; }
        /// <summary>
        /// Тип пользователя
        /// </summary>
        public int TypeUserId { get; set; }
        /// <summary>
        /// Дата создания пользователя
        /// </summary>
        public string CreateDate { get; set; }
    }
}
