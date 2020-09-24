using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Extension.Txt;
using Frgo.Dohod.DbModel.Manager.Parse;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Frgo.Dohod.DbModel.ModelCrm.Fk;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Format
{
    public class ParserFkDocs00 : BaseParserFormat, IParserFormat
    {
        public ParserFkDocs00(DohodDbContext dohodDbContext) : base(dohodDbContext)
        {
        }

        public async Task<object> ParserFormat(object doc, ImportParam importParam)
        {
            var fkcodes = new List<fk_code>();
            dynamic header = ((List<ExpandoObject>)doc).FirstOrDefault(x => ((dynamic)x).Marker.Trim() == Constant.MarkerDocxHeader);
            var maket = (string)header.MAKET;
            string formatnom = CheckFormatInFile(header, importParam);
            if (string.IsNullOrWhiteSpace(formatnom))
                return fkcodes;

            var fkDoc = await _dohodDbContext.fk_doc.Where(x => (x.Group ?? "") == importParam.FormatGroup &&
                                                                (x.Format ?? "") == importParam.Format)
                                         .OrderBy(x => x.Nfrm).LastOrDefaultAsync(x => x.Nfrm == formatnom);
            if (fkDoc == null)
                fkDoc = new fk_doc()
                {
                    Group = importParam.FormatGroup,
                    Format = importParam.Format,
                    Nfrm = formatnom,
                    Type = "IMP",
                };
            // Эти поля корректируется при переимпорте
            fkDoc.MaketImport = maket;
            fkDoc.MaketPreview = maket;
            fkDoc.MaketFull = maket;
            fkDoc.Name = ((string)header.NAME).Substr(0,80) ;
 
            //await DeleteTheFileData(importParam);
            //
            foreach (dynamic obj in (List<ExpandoObject>)doc)
            {
                if (obj.Marker.Trim() == Constant.MarkerDocxHeader)
                    continue;
                // TB
                var fkcode = FkCodeRecord(fkDoc, obj, importParam);
                if (fkcode != null)
                    fkcodes.Add(fkcode);
            }

            return fkcodes;
        }

        private fk_code FkCodeRecord(fk_doc fkDoc, dynamic objTb, ImportParam importParam)
        {
            if (fkDoc == null)
                return null;
            //if (string.IsNullOrWhiteSpace(objTb.NumOrder)) // филтр 
            //    return null;

            fk_code fkCode = new fk_code()
            {
                DocId = fkDoc.Sysid,
                Marker = objTb.Marker,
                NumPos = objTb.NumPos,
                NumOrder = objTb.NumOrder,
                Code = objTb.Code,
                FldType = objTb.FldType,
                FldLen = objTb.FldLen,
                IsFilled = ((string)objTb.IsFilled) == "Да",
                Descript = objTb.Descript,
                Comments = objTb.Comments
            };                     
            //
            fkCode.FkDoc = fkDoc;

            return fkCode;
        }
        protected string CheckFormatInFile(dynamic header, ImportParam importParam)
        {
            string formatstr = (string)header.VERSIONSTR;
            if (formatstr == null)
                return null;
            var formatinfile =  formatstr.To("|")+formatstr.Between("|","|");
            if (formatinfile.Length != 12)
                return null;
 
            var formatinfilename = Path.GetFileNameWithoutExtension(importParam.FileName);
            if (!formatinfilename.StartsWith("FK"))
                formatinfilename = "FK" + formatinfilename;
            if(formatinfilename.Length != 12)
                return null;

            if(formatinfile != formatinfilename)
                return null;

            return formatinfilename.Substring(6);
        }
    }
}
