using System;
using System.Linq;
using Monica.Core.Service.Paging;

namespace Monica.Core.Service.Extension
{
    public static class PagingExtension
    {
        /// <summary>
        /// Получить данные для страницы  просмотра
        /// </summary>
        /// <typeparam name="T">Модель данных запроса</typeparam>
        /// <typeparam name="TConvertModel">Модель данных, после пост обработки запроса</typeparam>
        /// <param name="query">Полученный запрос</param>
        /// <param name="page">Номер странницы для которой нужно получить данные</param>
        /// <param name="pageSize">Размер строк на странице</param>
        /// <param name="convertModelFunc"></param>
        /// <returns></returns>
        public static PagedResult<TConvertModel> GetPaged<T,TConvertModel>(this IQueryable<T> query,
            int page, int pageSize, Func<IQueryable<T>, IQueryable<TConvertModel>> convertModelFunc) 
            where T : class
            where TConvertModel : class
        {
            var result = new PagedResult<TConvertModel>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();


            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = convertModelFunc?.Invoke(query.Skip(skip).Take(pageSize)).ToList();

            return result;
        }

    }
}
