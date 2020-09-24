using Frgo.Dohod.DbModel.ModelCrm.Core;
using System;
using System.Collections.Generic;

namespace Frgo.Dohod.DbModel.ModelCrm.ImportPreviw
{
    public class ImpPreviewData
    {
        /// <summary>
        /// Preview Output параметры
        /// </summary>
        public DateTime Date { get; set; }
        public decimal Summa { get; set; } = 0.0M;
        public bool IsOk { get; set; }
        public int RecCount { get; set; }
        public bool HasData { get; set; }
        public int WriteCount { get; set; }
        public List<string> InfoLines { get; set; } = new List<string>();
        /// <summary>
        /// Import Output параметры
        /// </summary>
        public int recsDel { get; set; }
        public int recsAdd { get; set; }
        public int recsUpd { get; set; }
        public int recsSkip { get; set; }

    }
}
