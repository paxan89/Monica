using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.OpenXml.Xlsx;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Fk;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.File
{
    public class ParseFileXl : IParseFile
    {
        public byte[] FileData { get; set; }
        public ImportParam ImportParam { get; set; }
        private DohodDbContext _dohodDbContext;
        public ParseFileXl(DohodDbContext dohodDbContext)
        {
            _dohodDbContext = dohodDbContext;
        }
     
        public async Task<ResultCrmDb> ParseFile()
        {
            var result = new ResultCrmDb();

            var dt = FileData.XlToDataTable();
           if (dt.Rows.Count==0)
            {
                result.AddError(Constant.ErrCodeImport, $"Файл не содержит данных.");
                return result;
            }

            var fkDocs = await _dohodDbContext.fk_doc.Where(x => (x.Group ?? "") == ImportParam.FormatGroup &&
                                                                 (x.Format ?? "") == ImportParam.Format)
                                        .OrderBy(x => x.Nfrm).ToListAsync();
            fk_doc fkDoc = null;
            foreach(fk_doc fkdoc in fkDocs)
            {
                if (await IsFoundMfXlVersion(dt, fkdoc))
                {
                    fkDoc = fkdoc;
                    break;
                }
            }
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

           
            result.Result =  await GetXlObject(dt, fkDoc, fkCodes);
            return result;

            return result;
        }

        public async Task<bool> IsFoundMfXlVersion(DataTable dt, fk_doc fkdoc)
        {
            var fkcodes = await _dohodDbContext.fk_code.AsNoTracking().Where(x => x.DocId == fkdoc.Sysid).ToListAsync();
            dynamic header = GetHeaderObject(dt, fkdoc, fkcodes);
            if (header.DOCDATE == null || 
                header.TERRITORY == null ||
                header.DATEVYB == null)
                return false;

            return true;
        }

        private ExpandoObject GetHeaderObject(DataTable dt, fk_doc fkDoc, List<fk_code> fkCodes)
        {
            dynamic header = new ExpandoObject();
            header.Marker = Constant.MarkerXlHeader;
            foreach (var fkcode in fkCodes.Where(x => x.Marker == Constant.MarkerXlHeader))
            {
                var cellname = fkcode.Code.Between("(", ")"); // DOCDATE(A2)
                var cellval = dt.Rows[cellname.GetRowIndex()].ItemArray[cellname.GetColumnIndex()].ToString();
                if (fkcode.Descript.Contains("{}")) // за{}г.
                {
                    var adelimiters = fkcode.Descript.Split("{}");
                    if (!string.IsNullOrWhiteSpace(adelimiters[0]))
                        cellval = cellval.From(adelimiters[0].Trim());
                    if (!string.IsNullOrWhiteSpace(adelimiters[1]))
                        cellval = cellval.To(adelimiters[1].Trim());
                }
                ((IDictionary<string, object>)header).Add(fkcode.Code.To("("), cellval.ToFkTypeVal(fkcode));
            }

            return header;
        }


        private async Task<List<ExpandoObject>> GetXlObject(DataTable dt, fk_doc fkDoc, List<fk_code> fkCodes)
        {
            var maket = ImportParam.TypeImp == ImportType.Import ? fkDoc.MaketImport :
                        ImportParam.TypeImp == ImportType.Preview ? fkDoc.MaketPreview : fkDoc.MaketFull;
            // var dicScheme = FkDictionary(maket);

            var doc = new List<ExpandoObject>();
            // HEADER
            var header = GetHeaderObject(dt, fkDoc, fkCodes);
            doc.Add(header);
            // TB
            var startrow = Convert.ToInt32( fkCodes.Where(x => x.Marker == "STARTROW").FirstOrDefault().Code);
            for (var i = startrow -1; i < dt.Rows.Count; i++)
            {
                dynamic obj = new ExpandoObject();
                obj.Marker = Constant.MarkerXlTable;
                foreach (var fkcode in fkCodes.Where(x => x.Marker == Constant.MarkerXlTable && (x.NumOrder ?? 0) > 0)
                                             .OrderBy(x => x.NumOrder))
                {
                    //if (!markerpar.Flds.Contains(fkcode.Code))
                    //    continue;
                    object val = null;
                    var cellval = dt.Rows[i].ItemArray[(fkcode.NumOrder ?? 0)-1];
                    if (cellval != null)
                        val = cellval.ToString().ToFkTypeVal(fkcode);
                    ((IDictionary<string, object>)obj).Add(fkcode.Code, val);
                }
                doc.Add(obj);
            }
            return doc;

        }


    }
}
