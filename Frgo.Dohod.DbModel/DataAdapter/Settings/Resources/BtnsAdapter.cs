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
    public class BtnsAdapter : IBtnsAdapter
    {
        private DohodDbContext _crmDbContext;
        public BtnsAdapter(DohodDbContext crmDbContext)
        {
            _crmDbContext = crmDbContext;
        }
        public async Task<ButtonAccessDto> GetButtonsTreeAsync(int idRole)
        {
            List<ButtonItem> items = new List<ButtonItem>();
            List<int> selected = new List<int>();
            int i = 1;
            foreach (var f in _crmDbContext.formModel)
            {
                var item = new ButtonItem() { Id = i++, IsButton = false, ButtonId = -1, FormId = f.Id, ParentId = 0, Text = f.Caption };
                items.Add(item);
            }
            foreach (var b in _crmDbContext.buttonForm)
            {
                var form = items.FirstOrDefault(f => f.FormId == b.FormId);
                var parent = form != null ? form.Id : 0;
                var item = new ButtonItem() { Id = i++, IsButton = true, ButtonId = b.Id, FormId = -1, ParentId = parent, Text = b.ToolTip };
                items.Add(item);
            }
            foreach (var s in _crmDbContext.accessForm.Where(ur => ur.UserRoleId == idRole))
                selected.AddRange(items.Where(m => m.ButtonId == s.ButtonFormId).Select(el => el.Id));
            return new ButtonAccessDto() { Data = items, Selected = selected };
        }
        public async Task<ResultCrmDb> EditAccessAsync(int idRole, int[] selected)
        {
            var result = new ResultCrmDb();
            try
            {

                List<AccessForm> access = new List<AccessForm>();
                _crmDbContext.accessForm.RemoveRange(_crmDbContext.accessForm.Where(x => x.UserRoleId == idRole & x.ButtonFormId != null));
                foreach (var idButton in selected)
                    access.Add(new AccessForm { ButtonFormId = idButton, UserRoleId = idRole });
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
