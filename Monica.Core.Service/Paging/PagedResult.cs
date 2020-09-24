using System.Collections.Generic;

namespace Monica.Core.Service.Paging
{
    /// <summary>
    /// Результат получения информации о пагинации
    /// </summary>
    /// <typeparam name="T">Модель данных для которой нужно получить пагинацию</typeparam>
    public class PagedResult<T> : PagedResultBase where T : class
    {
        /// <summary>
        /// Полученный результат пагинации данных
        /// </summary>
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}
