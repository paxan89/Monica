using System;
using System.ComponentModel.DataAnnotations;

namespace Frgo.Dohod.DbModel.ModelCrm.Core
{
    public static class Constant
    {
        /// <summary>
        /// тип импорта: предварительный, основной, полный
        /// </summary>
        public const string UnknownOrg = "Неизвестный плательщик";
        public const string VidDocKorOrg = "Корректирующий платеж";
        public const string NpCodeMask = "???11701*";

        public const string ErrCodeSetting = "1001";
        public const string ErrCodeImport = "1002";
        public const string ErrCodeStackTrace = "1111";

        public const string MarkerXlHeader = "HEADER";
        public const string MarkerXlTable = "TB";
        public const string MarkerDocxHeader = "HEADER";
        public const string MarkerDocxTable = "TB";
        public const string MarkerFormatInfo = "FORMATINFO";
        public const string FldFormatInfoInFile = "FORMATINFILE";
        public const string FldFormatInfoUsed = "FORMATUSED";


        private static System.Globalization.CultureInfo _cu;
        public static System.Globalization.CultureInfo CU
        {
            get
            {
                if (_cu == null)
                {
                    _cu = new System.Globalization.CultureInfo("en-US");
                    _cu.NumberFormat.NumberGroupSeparator = "";
                }

                return _cu;
            }
        }

    }
}
