using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelCrm.Data;
using Frgo.Dohod.DbModel.ModelCrm.Dictionary;
using Frgo.Dohod.DbModel.ModelCrm.ImportPreviw;
using Microsoft.EntityFrameworkCore;

namespace Frgo.Dohod.DbModel.DataAdapter.Parse.Format
{
    public class BaseParserFormat 
    {
        protected DohodDbContext _dohodDbContext;
        protected List<s_oktmo> _lstOktmo;
        protected ImpPreviewData _previewdata;
        public ImpPreviewData PreviewData  => _previewdata;
        public BaseParserFormat(DohodDbContext dohodDbContext)
        {
            _previewdata = new ImpPreviewData();
            _dohodDbContext = dohodDbContext;
        }



        protected virtual s_oktmo GetOktmo(string code, ImportParam importParam, bool isEmptyForEmpty=false)
        {
            if(_lstOktmo==null)
               _lstOktmo =  _dohodDbContext.s_oktmo.AsNoTracking().Where(x => x.CodeAte == importParam.CodeAte).ToList();
 
            if (!string.IsNullOrWhiteSpace(code))
                return _lstOktmo.Where(x => x.Oktmo == code).FirstOrDefault();

            // c 2015.03.19 - ОКТМО в некоторых файлах может быть не указан - а данные нужны
            //code = importParam.TheOktmo.Oktmo;
            //TODO когда были поселения подбирали по 'Uved_LsADB', сейчас наверно просто TheOktmo ???? 
            return isEmptyForEmpty ? null : importParam.TheOktmo;
        }

        protected async Task DeleteTheFileData(ImportParam importParam)
        {
            var filerecords = _dohodDbContext.t_dohod.Where(x => x.ImportFile == importParam.FileName);
            _previewdata.recsDel = filerecords.Count();
            if (_previewdata.recsDel == 0)
                return;

            _dohodDbContext.t_dohod.RemoveRange(filerecords);
            await _dohodDbContext.SaveChangesAsync();

            // или ??
            //_dohodDbContext.Database.GetDbConnection()
            //               .ExecuteScalar($"DELETE FROM t_dohod where ImportFile = '{importParam.FileName}';");

        }


        protected virtual async Task<t_dohod> ChechRecord(t_dohod dohod, ImportParam importParam)
        {
            if (dohod == null)
                return null;

            // Стандартно: добавить или обновить, ничего не пропуская
            var record = await GetExistingRecord(dohod);
            if (record != null)
            {
              //  var yy =  _dohodDbContext.Entry(dohod);
              //  var xx =  _dohodDbContext.Entry(record);
              //  _dohodDbContext.Entry(record).State = EntityState.Detached;
                //
                _previewdata.recsUpd++;
                dohod.Sysid = record.Sysid;
                dohod.Orig.Sysid = record.Orig.Sysid;
                dohod.Orig.DohodId = record.Sysid;
                //yy.State = EntityState.Modified;
            }
            else
                _previewdata.recsAdd++;
            //importParam.recsSkip = 0;
            return dohod;
        }


        protected virtual Task<t_dohod> GetExistingRecord(t_dohod dohod)
        {
            t_dohod doh = null;
            return Task.FromResult(doh);
        }


        public virtual Task<bool> AfterDataWrited(List<t_dohod> dohods, ImportParam importParam)
        {
            return Task.FromResult(true);
        }

    }
}