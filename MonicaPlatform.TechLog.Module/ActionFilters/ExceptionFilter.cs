using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MonicaPlatform.TechLog.Module.ActionFilters
{
    /// <summary>
    /// Фильтр для обработки возникающих исключений в функциях Action,
    /// лог файлы располагаются в папке logs\ApiTech.Log\
    /// </summary>
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        #region IExceptionFilter

        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Items.Add("excpetionReceived", context.Exception);

            var badResult = new BadRequestObjectResult("")
            {
                StatusCode = 500
            };

            context.Result = badResult;
            context.ExceptionHandled = true;
        }

        #endregion
    }
}
