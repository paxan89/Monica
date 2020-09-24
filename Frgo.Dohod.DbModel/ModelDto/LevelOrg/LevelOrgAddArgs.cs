using Frgo.Dohod.DbModel.ModelCrm.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frgo.Dohod.DbModel.ModelDto.LevelOrg
{
    public class LevelOrgAddArgs
    {
        public string TypeLevel { get; set; }
        /// <summary>
        /// Ссылка на вышестоящую организацию
        /// </summary>
        public int Parent { get; set; }
        /// <summary>
        /// Наименование орг
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// ИНН орг(12 символов)
        /// </summary>
        public string Inn { get; set; }
        /// <summary>
        /// КПП орг ( 9 символов)
        /// </summary>
        public string Kpp { get; set; }
        /// <summary>
        /// ОКТМО орг (11 символов)
        /// </summary>
        public string Oktmo { get; set; }
        public static implicit operator t_levelorg(LevelOrgAddArgs args)
        {
            return new t_levelorg()
            {
                TypeLevel = args.TypeLevel,
                Parent = args.Parent,
                Caption = args.Caption,
                Inn = args.Inn,
                Kpp = args.Kpp,
                Oktmo = args.Oktmo
            };
        }
    }
}
