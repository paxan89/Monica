using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.OpenXml.Docx;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Fk;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.File
{
    public class ParseFileFkDocx : IParseFile
    {
        public byte[] FileData { get; set; }
        public ImportParam ImportParam { get; set; }
        private DohodDbContext _dohodDbContext;
        public ParseFileFkDocx(DohodDbContext dohodDbContext)
        {
            _dohodDbContext = dohodDbContext;
        }
     
        public async Task<ResultCrmDb> ParseFile()
        {
            var result = new ResultCrmDb();

            var dt = FileData.DocxTableToDataTableByCaptionStart("Описание");
            if (dt.Rows.Count==0 || dt.Columns.Count!=6)
                dt = FileData.DocxTableToDataTableByTbNumber(2);
            if (dt.Rows.Count==0 || dt.Columns.Count!=6)
                dt = FileData.DocxTableToDataTableByTbNumber(1);

            if (dt.Rows.Count==0 || dt.Columns.Count!=6)
            {
                result.AddError(Constant.ErrCodeImport, $"Файл не содержит данных.");
                return result;
            }
 
            var fkDoc = await _dohodDbContext.fk_doc.Where(x => (x.Group ?? "") == "FKDOCX" &&
                                                                (x.Format ?? "") == "00")
                                        .OrderBy(x => x.Nfrm).LastOrDefaultAsync();
 
            if (fkDoc == null)
            {
                result.AddError(Constant.ErrCodeImport, $"Требуется файл формата: {ImportParam.Format}");
                return result;
            }


            var fkCodes = await _dohodDbContext.fk_code.Where(x => x.DocId == fkDoc.Sysid).ToListAsync();
            if (fkCodes == null || fkCodes.Count == 0)
            {
                result.AddError(Constant.ErrCodeImport, $"Не заполнена структура формата {ImportParam.Format} версии {fkDoc.Nfrm} в fk_doc");
                return result;
            }

           
            result.Result =  GetDocsObject(dt, fkDoc, fkCodes);
            return result;
        }

        private ExpandoObject GetMaketObject(DataTable dt, fk_doc fkDoc, List<fk_code> fkCodes)
        {
            dynamic header = new ExpandoObject();
            header.Marker = Constant.MarkerDocxHeader;
            foreach (var fkcode in fkCodes.Where(x => x.Marker.Trim() == Constant.MarkerDocxHeader))
            {
                var val = "";
                if(fkcode.Code=="MAKET") 
                   val = FileData.DocxTextBetweenParagaphs("Макет файла", "Пример файла");
                else if(fkcode.Code=="VERSIONSTR") 
                   val = FileData.DocxTextBetweenParagaphs("Пример файла","FROM", new string[1] { "FK|" });
                else if(fkcode.Code=="NAME") 
                   val = ((string)FileData.DocxTextBetweenParagaphs("", "Назначение и маршрут")).Substr(0,80);
                ((IDictionary<string, object>)header).Add(fkcode.Code, val.ToFkTypeVal(fkcode));
            }


            return header;
        }


        private List<ExpandoObject> GetDocsObject(DataTable dt, fk_doc fkDoc, List<fk_code> fkCodes)
        {
            var doc = new List<ExpandoObject>();
            // MAKET  
            var header = GetMaketObject(dt, fkDoc, fkCodes);
            doc.Add(header);
            // TB
            var startrow = 1; // Пропуская заголовок
            var numorder = 0;
            var prevMarker = "";
            for (var i = startrow; i < dt.Rows.Count; i++)
            {
                dynamic obj = new ExpandoObject();
                foreach (var fkcode in fkCodes.Where(x => x.Marker.Trim() == Constant.MarkerDocxTable) // && (x.NumOrder ?? 0) > 0
                                             .OrderBy(x => x.NumOrder))
                {
                    object val = null;
                    var cellval = dt.Rows[i].ItemArray[(fkcode.NumOrder ?? 0)-1];
                    if (cellval != null)
                        val = ((string)cellval).ToFkTypeVal(fkcode);
                    ((IDictionary<string, object>)obj).Add(fkcode.Code, val);
                }
                var isMarkerHeader = string.IsNullOrWhiteSpace(obj.FldType) &&
                                     string.IsNullOrWhiteSpace(obj.FldLen); // &&  string.IsNullOrWhiteSpace(obj.Comments); ZF - !!!
                var isHeaderComment = isMarkerHeader && string.IsNullOrWhiteSpace(obj.Code);
                if(isMarkerHeader && !isHeaderComment)
                {                   
                    // Начало блока
                    prevMarker = ((string)obj.Code).To("(", true);
                    numorder = 0;
                 }
                obj.Marker = prevMarker;
                obj.NumPos = i;
                obj.NumOrder = isHeaderComment ? 0 : numorder++;  
                doc.Add(obj);
            }
            return doc;

        }


    }
}
