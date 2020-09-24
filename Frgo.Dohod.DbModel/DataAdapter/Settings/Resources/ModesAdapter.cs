using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.ModelCrm;
using Frgo.Dohod.DbModel.ModelCrm.Core;
using Frgo.Dohod.DbModel.ModelDto.Settings;
using Frgo.Dohod.DbModel.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frgo.Dohod.DbModel.DataAdapter.Settings.Resources
{
    public class ModesAdapter : IModesAdapter
    {
        private DohodDbContext _crmDbContext;
        public ModesAdapter(DohodDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<ModeAccessDto> GetModesTreeAsync(int idRole)
        {
            List<ModeItem> modes = new List<ModeItem>();
            List<int> selected = new List<int>();
            int i = 1;
            foreach (var t in _crmDbContext.typeForm)
            {
                var mode = new ModeItem() { Id = i++, ModeId = -1, TypeId = t.Id, IsMode = false, Text = t.DisplayName, ParentId = 0 };
                modes.Add(mode);
            }
            foreach (var m in _crmDbContext.formModel)
            {
                var typeForm = modes.FirstOrDefault(t => t.TypeId == m.TypeFormId);
                var parent = typeForm != null ? typeForm.Id : 0;
                var mode = new ModeItem() { Id = i++, ModeId = m.Id, TypeId = -1, IsMode = true, Text = m.Caption, ParentId = parent };
                modes.Add(mode);
            }
            foreach (var s in _crmDbContext.accessForm.Where(ur => ur.UserRoleId == idRole))
            {
               // if (modes.FirstOrDefault(m => m.ModeId == s.FormModelId) != null)
               // {
                    //selected.Add((int)s.FormModelId);
                    selected.AddRange(modes.Where(m => m.ModeId == s.FormModelId).Select(el => el.Id));
                //}
                    
             //   else
               //     continue;
            }
            return new ModeAccessDto() { Data = modes, Selected = selected };
        }
        public async Task<ResultCrmDb> EditAccessAsync(int idRole, int[] selected)
        {
            var result = new ResultCrmDb();
            try
            {
                List<AccessForm> access = new List<AccessForm>();
                _crmDbContext.accessForm.RemoveRange( _crmDbContext.accessForm.Where(x => x.UserRoleId == idRole & x.FormModelId!=null));
                foreach (var idMode in selected)
                    access.Add(new AccessForm { FormModelId = idMode, UserRoleId = idRole });
                await _crmDbContext.accessForm.AddRangeAsync(access);
                await _crmDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                result.AddError("", e.Message);
            }
            return result;
        }
    }
}
