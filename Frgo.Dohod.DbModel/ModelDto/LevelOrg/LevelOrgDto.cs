using DocumentFormat.OpenXml.Office2013.PowerPoint;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Monica.CrmDbModel.ModelDto.Core;
using System;

namespace Frgo.Dohod.DbModel.ModelDto.LevelOrg
{
    public class LevelOrgDto : BaseModelDto
    {
        //private int? parent;
        /// <summary>
        /// Тип уровня организации ( f|a )
        /// </summary>
        public string TypeLevel { get; set; }
        /// <summary>
        /// Ссылка на администратора
        /// </summary>
        public int AdmId { get; set; }
        /// <summary>
        /// Ссылка на вышестоящую организацию
        /// </summary>
        public int Parent { get; set; }
        //{
        //    get=> parent; 
        //    set
        //    {
        //        parent = value;
        //        if (parent == null)
        //            parent = 0;
        //    }
        //}
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
        ///// <summary>
        ///// Сортировка структуры орг на экране
        ///// </summary>
        //public int Order { get; set; }
        ///// <summary>
        ///// Дата создания записи
        ///// </summary>
        //public DateTime CreateDate { get; set; }
        public static implicit operator t_levelorg(LevelOrgDto dto)
        {
            return new t_levelorg()
            {
                Sysid = dto.Id,
                TypeLevel = dto.TypeLevel,
                AdmId = dto.AdmId,
                Parent = dto.Parent,
                Caption = dto.Caption,
                Inn = dto.Inn,
                Kpp = dto.Kpp,
                Oktmo = dto.Oktmo,
                IsDeleted = dto.IsDeleted
            };
        }
    }
}
