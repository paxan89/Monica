using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Preview
{
    public class BasePreviewFormat 
    {
        protected DohodDbContext _dohodDbContext;
        protected List<s_oktmo> _lstOktmo;

        public BasePreviewFormat(DohodDbContext dohodDbContext)
        {
            _dohodDbContext = dohodDbContext;
        }



        protected s_oktmo GetOktmo(string code, ImportParam importParam, bool isEmptyForEmpty = false)
        {
            if(_lstOktmo==null)
               _lstOktmo =  _dohodDbContext.s_oktmo.AsNoTracking().Where(x => x.CodeAte == importParam.CodeAte).ToList();
 
            if (!string.IsNullOrWhiteSpace(code))
                return _lstOktmo.Where(x => x.Oktmo == code).FirstOrDefault();

            // c 2015.03.19 - ОКТМО в файле может быть не указан - а данные нужны
            //code = importParam.TheOktmo.Oktmo;
            //TODO когда были поселения подбирали по 'Uved_LsADB', сейчас наверно просто TheOktmo ???? 
            return isEmptyForEmpty ? null : importParam.TheOktmo;
        }

        protected string  InfoText(bool isError, int reccount, int writecount, DateTime datemin, DateTime datemax)
        {
            if (isError)
                return  "Нет данных. Ошибка чтения или не правильный формат файла ";

            if (writecount == 0)
                return  reccount == 0 ? "Нет платежей !" : "Нет платежей для записи !";
           
            return "";
        }
        protected string  InfoFormat(object doc, bool isError, int reccount, int writecount, ImportParam importParam)
        {
            var obj = ((List<ExpandoObject>)doc).FirstOrDefault(x=>((dynamic)((IDictionary<string, object>)x)).Marker==Constant.MarkerFormatInfo);
            if (obj == null)
                return string.Empty; // Нет информации о форматах
            
            object formatinfile;
            object formatused;
            var ok1 = ((IDictionary<string, object>)obj).TryGetValue(Constant.FldFormatInfoInFile, out formatinfile);
            var ok2 = ((IDictionary<string, object>)obj).TryGetValue(Constant.FldFormatInfoUsed, out formatused);
            if (!ok1 || !ok2 )
                return string.Empty;

            if ((string)formatinfile == (string)formatused)
                return $"Формат: <b>{importParam.Format}{(string)formatinfile}</b>";
            else 
                return $"Формат файла: <b>{importParam.Format}{(string)formatinfile}</b>, для импорта использован описатель формата: <b>{importParam.Format}{formatused}</b>. Возможны ошибки !";
        }


        protected ImpPreviewData InfoPreviewData(object doc, bool isError, int reccount, int writecount, decimal summa, DateTime datemin, DateTime datemax, ImportParam importParam)
        {
            var delcount = _dohodDbContext.t_dohod.Where(x => x.ImportFile == importParam.FileName)?.Count() ?? 0;
            var infolines = new List<string>();

            if (delcount > 0)
                infolines.Add($"Повторный импорт из файла! Импортированые из него записи будет удалены:<b>{delcount}</b>");
            //infolines.Add($"Повторный импорт из файла! Импортированые из него записи будет удалены:<span style = 'color:blue;font-weight:bold'>{delcount}</span>");

            var infoFormat = InfoFormat(doc, isError, reccount, writecount, importParam);
            if (!string.IsNullOrEmpty(infoFormat))
                infolines.Add($"{infoFormat}");

            infolines.Add($"Записей в файле.Всего: <b>{reccount}</b>,  к импорту: <b>{writecount}</b>");

            var cdates = datemin == datemax ? $"<b>{datemin:dd.MM.yyyy}</b>" : $"с <b>{datemin:dd.MM.yyyy}</b> по <b>{datemax:dd.MM.yyyy}</b>";
            infolines.Add($"Дата документов: <b>{cdates}</b>");
            infolines.Add($"Сумма документов: <b>{summa:C}</b>");

            var infoText = InfoText(isError || datemin == default(DateTime), reccount, writecount, datemin, datemax);
            if (!string.IsNullOrEmpty(infoText))
                infolines.Add($"Примечание: <b>{infoText}</b>");


            return new ImpPreviewData()
            {
                Date = datemin,
                Summa = Math.Round(summa, 2),
                RecCount = reccount,
                WriteCount = writecount,
                IsOk = reccount > 0,
                HasData = writecount > 0,
                recsDel = delcount,
                InfoLines = infolines
            };
        }

    }
}