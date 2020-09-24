using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using System;
using System.ComponentModel.DataAnnotations;

namespace Frgo.Dohod.DbModel.ModelCrm.Core
{
    /// <summary>
    /// Параметры импортируемого файла
    /// </summary>
    public class ImportParam
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public ImportParam()
        {
        }

        /// <summary>
        /// Файл наименование
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Группа форматов: MFXL, FKTX, ...
        /// </summary>
        public string FormatGroup { get; set; }

        /// <summary>
        /// Формат (в группе) BD, ZR, IP, ...
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// тип импорта: предварительный, основной, полный
        /// </summary>
        public ImportType TypeImp { get; set; } 

        /// <summary>
        /// Основной ОКТМО 
        /// </summary>
        /// 
        public s_oktmo TheOktmo { get; set; } 
       
        /// <summary>
        /// настройка - пока здесь
        /// </summary>
        public string CodeAte { get; set; } 
        public string CodePos { get; set; }
        public int DaysToSkip { get; set; }

   }
}
