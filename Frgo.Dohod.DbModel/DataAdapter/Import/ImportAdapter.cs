using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.DataAdapter.Parse.File;
using Frgo.Dohod.DbModel.DataAdapter.Parse.Preview;
using Frgo.Dohod.DbModel.Extension;
using Frgo.Dohod.DbModel.Manager.Parse;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Fk;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Utils;

namespace Frgo.Dohod.DbModel.DataAdapter.Import
{
    public class ImportAdapter : IImportAdapter
    {
        private DohodDbContext _dohodDbContext;

        public ImportAdapter(DohodDbContext dohodDbContext)
        {
            _dohodDbContext = dohodDbContext;
        }

        public async Task<ResultCrmDb> ImportFile(byte[] fileData, ImportParam importParam)
        {
            var result = new ResultCrmDb();
            importParam.TypeImp = ImportType.Import;
            try
            {
                // TheOktmo потом перенести в Settings 
                _dohodDbContext.SetTheOktmo(importParam, result);
                if (!result.Succeeded)
                    return result;

                var parserFile = AutoFac.ResolveNamed<IParseFile>(importParam.FormatGroup);
                parserFile.FileData = fileData;
                parserFile.ImportParam = importParam;
                var parserResult = await parserFile.ParseFile();
                if (!parserResult.Succeeded)
                    return parserResult;

                
                var parserFormat = AutoFac.ResolveNamed<IParserFormat>(importParam.FormatGroup + importParam.Format);
                var dohods = (List<t_dohod>)await parserFormat.ParserFormat(parserResult.Result, importParam);
                await WriteImportedData(dohods);
                await parserFormat.AfterDataWrited(dohods, importParam);
                result.Result = ImportInfo(parserFormat.PreviewData, importParam);
            }
            catch (Exception ex)
            {
                result.AddError(Constant.ErrCodeImport, ex.Message);
                result.AddError(Constant.ErrCodeStackTrace, ex.StackTrace);
            }
            //_dohodDbContext.TruncateDbTable("t_dohorig");
            //dohodDbContext.TruncateDbTable("t_dohod");
            return result;
        }

        private ImpPreviewData ImportInfo(ImpPreviewData previewData, ImportParam importParam)
        {
            if(previewData.recsDel>0)
               previewData.InfoLines.Add($"Удалено записей перед повторным импортом файла: <b>{previewData.recsDel}</b>");

            if (previewData.recsAdd>0)
                previewData.InfoLines.Add($"Добавлено новых записей: <b>{ previewData.recsAdd}</b>");
            if (previewData.recsUpd>0)
                previewData.InfoLines.Add($"Обновлено существующих записей: <b>{previewData.recsUpd}</b>");
            if (previewData.recsSkip>0)
                previewData.InfoLines.Add($"Пропущено записей файла: <b>{previewData.recsSkip}</b>");
            
            previewData.InfoLines.Add($"Импорт из файла '{importParam.FileName}' завершен.");
            return previewData;
        }


        public async Task<ResultCrmDb> PreviewFile(byte[] fileData, ImportParam importParam)
        {
            var result = new ResultCrmDb();
            importParam.TypeImp = ImportType.Preview;
            try
            {
                // TheOktmo потом перенести в Settings 
                _dohodDbContext.SetTheOktmo(importParam, result);
                if (!result.Succeeded)
                    return result;

                var parserFile = AutoFac.ResolveNamed<IParseFile>(importParam.FormatGroup);
                parserFile.FileData = fileData;
                parserFile.ImportParam = importParam;
                var parserResult = await parserFile.ParseFile();
                if (!parserResult.Succeeded)
                    return parserResult;

                var previewFormat = AutoFac.ResolveNamed<IPreviewFormat>(importParam.FormatGroup + importParam.Format);
                result.Result = (ImpPreviewData) await previewFormat.PreviewFormat(parserResult.Result, importParam);
            }
            catch (Exception ex)
            {
                result.AddError(Constant.ErrCodeImport,ex.Message);
                result.AddError(Constant.ErrCodeStackTrace, ex.StackTrace);
            }
            return result;
        }
 
        public async Task WriteImportedData(List<t_dohod> dohods)
        {
            // Пусть так пока
            foreach (var doh in dohods.Where(x => x.Sysid > 0))
            {
                var xx = _dohodDbContext.t_dohod.Local.FirstOrDefault(x => x.Sysid == doh.Sysid);
                if (xx != null)
                    _dohodDbContext.Entry(xx).State = EntityState.Detached;
            }
            //foreach (var dohod in dohods.Where(x => x.Sysid == 0))
            //{
            //    var res = await _dohodDbContext.t_dohod.AddAsync(dohod);
            //    dohod.Orig.Dohod = res.Entity;
            //    await _dohodDbContext.t_dohorig.AddAsync(dohod.Orig);
            //}

            _dohodDbContext.t_dohod.AddRange(dohods.Where(x=>x.Sysid==0));
            _dohodDbContext.UpdateRange(dohods.Where(x => x.Sysid > 0));

            await _dohodDbContext.SaveChangesAsync();

         }

        // ======================================== fk_doc === fk_code ==========================

        public async Task<ResultCrmDb> LoadFkDocxFormat(byte[] fileData, ImportParam importParam)
        {
            var result = new ResultCrmDb();
            importParam.TypeImp = ImportType.None;
            try
            {
 
                var parserFile = AutoFac.ResolveNamed<IParseFile>("FKDOCX");
                parserFile.FileData = fileData;
                parserFile.ImportParam = importParam;
                var parserResult = await parserFile.ParseFile();
                if (!parserResult.Succeeded)
                    return parserResult;

                var parserFormat = AutoFac.ResolveNamed<IParserFormat>("FKDOCX00");
                var fkCodes = (List<fk_code>)await parserFormat.ParserFormat(parserResult.Result, importParam);
                await WriteFkCode(fkCodes);
            }
            catch (Exception ex)
            {
                result.AddError(Constant.ErrCodeImport, ex.Message);
                result.AddError(Constant.ErrCodeStackTrace, ex.StackTrace);
            }
            return result;
        }

        public async Task WriteFkCode(List<fk_code> fkCodes)
        {
            if (fkCodes.Count == 0)
                return;

            //// Пусть так пока
            //foreach (var fkcode in fkCodes.Where(x => x.Sysid > 0))
            //{
            //    var xx = _dohodDbContext.fk_code.Local.FirstOrDefault(x => x.Sysid == fkcode.Sysid);
            //    if (xx != null)
            //        _dohodDbContext.Entry(xx).State = EntityState.Detached;
            //}

            var fkDoc = fkCodes[0].FkDoc;
            if (fkDoc.Sysid == 0)
                _dohodDbContext.fk_doc.Add(fkDoc);
            else
            {
                _dohodDbContext.Update<fk_doc>(fkDoc);
                var fkdocrecords = _dohodDbContext.fk_code.Where(x => x.DocId == fkDoc.Sysid);
                if (fkdocrecords.Count() > 0)
                   _dohodDbContext.fk_code.RemoveRange(fkdocrecords);
            }
            await _dohodDbContext.SaveChangesAsync();
            
            _dohodDbContext.fk_code.AddRange(fkCodes.OrderBy(x=>x.NumPos));
            await _dohodDbContext.SaveChangesAsync();

        }
    }
}
