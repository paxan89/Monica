using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Fk;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.File
{
    public class ParseFileFk : IParseFile
    {
        private const string versionMarker = "FK";
        private DohodDbContext _dohodDbContext;
        public byte[] FileData { get; set; }
        public ImportParam ImportParam { get; set; }
        
        public ParseFileFk(DohodDbContext dohodDbContext)
        {
            _dohodDbContext = dohodDbContext;
        }
        public async Task<ResultCrmDb> ParseFile()
        {
            var result = new ResultCrmDb();
            var utf = Encoding.UTF8.GetString(FileData, 0, FileData.Length);
            var version = await GetFkVersion(utf);
            if (string.IsNullOrWhiteSpace(version))
            {
                result.AddError(Constant.ErrCodeImport, $"Требуется файл формата: {ImportParam.Format}");
                return result;
            }

            var fkDoc = await _dohodDbContext.fk_doc.Where(x => (x.Group ?? "") == ImportParam.FormatGroup &&
                                                               (x.Format ?? "") == ImportParam.Format &&
                                                                x.Nfrm.CompareTo(version) <= 0)
                                      .OrderBy(x => x.Nfrm).LastOrDefaultAsync();
            if (fkDoc == null)
            {
                result.AddError(Constant.ErrCodeImport, $"Импорт данного формата {ImportParam.Format} версии {version} не предусмотрен");
                return result;
            }

            var fkCode = await _dohodDbContext.fk_code.Where(x => x.DocId == fkDoc.Sysid).ToListAsync();
            if (fkCode == null || fkCode.Count == 0)
            {
                result.AddError(Constant.ErrCodeImport, $"Не заполнена структура формата {ImportParam.Format} версии {version} в fk_doc");
                return result;
            }

            result.Result = await GetFkObject(utf, fkDoc, fkCode);
            AddFormatInfo(result.Result, version, fkDoc.Nfrm);

            return result;
        }

        public async Task<string> GetFkVersion(string txt)
        {
            using (StringReader reader = new StringReader(txt))
            {
                for (var i = 0; i < 5; i++)
                {
                    var aline = (await reader.ReadLineAsync() ?? "").Split('|');
                    if (aline[0] == versionMarker && aline[1].Length > 4)
                        return aline[1].Substring(4);
                }
                return "";
            }
        }
        private void AddFormatInfo(object doc, string formatinfile, string formatused)
        {
            dynamic obj = new ExpandoObject();
            obj.Marker = Constant.MarkerFormatInfo;
            ((IDictionary<string, object>)obj).Add(Constant.FldFormatInfoInFile, formatinfile);
            ((IDictionary<string, object>)obj).Add(Constant.FldFormatInfoUsed, formatused);
            ((List<ExpandoObject>)doc).Add(obj);
            return;
        }

        private async Task<List<ExpandoObject>> GetFkObject(string txt, fk_doc fkDoc, List<fk_code> fkCodes)
        {
            var maket = ImportParam.TypeImp == ImportType.Import  ? fkDoc.MaketImport :
                        ImportParam.TypeImp == ImportType.Preview ? fkDoc.MaketPreview : fkDoc.MaketFull;
            var dicScheme = FkDictionary(maket);
            var doc = new List<ExpandoObject>();
            using (StringReader reader = new StringReader(txt))
            {
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null)
                        break;

                    var aline = line.Split('|');
                    var marker = aline[0];
                    if (string.IsNullOrWhiteSpace(marker) ||
                        marker == versionMarker ||
                        !dicScheme.ContainsKey(marker))
                        continue;

                    var markerpar = dicScheme[marker];
                    dynamic obj = new ExpandoObject();
                    obj.Marker = marker;
                    foreach (var fkcode in fkCodes.Where(x => x.Marker == marker && (x.NumOrder ?? 0) > 0)
                                                 .OrderBy(x => x.NumOrder))
                    {
                        if (!markerpar.Flds.Contains(fkcode.Code))
                            continue;

                        var val = aline[fkcode.NumOrder ?? 0].ToFkTypeVal(fkcode);
                        ((IDictionary<string, object>)obj).Add(fkcode.Code, val);
                    }
                    doc.Add(obj);
                }
                return doc;
            }

        }

        private Dictionary<string, MarkerPar> FkDictionary(string scheme)
        {
            var dic = new Dictionary<string, MarkerPar>();
            using (StringReader reader = new StringReader(scheme))
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        break;
                    var aline = line.Split('|').ToArray();
                    var nn = aline.Length;
                    var marker = aline[0].Split('(')[0];
                    dic[marker] = new MarkerPar()
                    {
                        MarkerParent = marker.Contains("(+")
                                        ? new string(marker.SkipWhile(x => x != '+').Skip(1).TakeWhile(x => x != ')').ToArray()).Trim()
                                        : "",
                        Flds = aline.Where((x, i) => i != 0 && i != nn - 1)
                                    .Select(x => x.Split('(')[0]).ToArray(),
                        FldsCount = nn - 2,
                        MarkerNext = aline[nn - 1].Split('(')[0],
                        IsCollectionNext = aline[nn - 1].Contains("(*)")
                    };
                }
            }
            return dic;
        }

        protected class MarkerPar
        {
            public string MarkerParent { get; set; }
            public string[] Flds { get; set; }
            public int FldsCount { get; set; }
            public string MarkerNext { get; set; }
            public bool IsCollectionNext { get; set; }
        }

    }
}
