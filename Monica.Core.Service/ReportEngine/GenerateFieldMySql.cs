using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DataBaseUtils;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.EngineReport;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Exceptions;
using MySql.Data.MySqlClient;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Генерация схемы для полей режимов на основании схемы БД
    /// </summary>
    public class GenerateFieldMySql : IGenerateField
    {
        private readonly IDataBaseMain _dataBaseMain;
        private readonly ReportDbContext _reportDbContext;

        public GenerateFieldMySql(IDataBaseMain dataBaseMain, ReportDbContext reportDbContext)
        {
            _dataBaseMain = dataBaseMain;
            _reportDbContext = reportDbContext;
        }

        /// <summary>
        /// Генерация полей для режима на основании схемы БД
        /// </summary>
        /// <param name="formModelId"></param>
        /// <returns></returns>
        public async Task GenerateField(int formModelId)
        {
            var formModel = await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId);
            if (formModel == null)
                return;
            if (formModel.TableName == null)
                throw new UserMessageException("Не удалось определить таблицу");
            using (MySqlConnection connection = new MySqlConnection(_dataBaseMain.ConntectionString))
            {
                int i = 10;
                var reuslt =
                    await connection.QueryAsync<ColumnTable>(string.Format(SelectField, connection.Database, formModel.TableName));
                foreach (var columnTable in reuslt)
                {
                    var field = await _reportDbContext.Field.FirstOrDefaultAsync(f => f.FormModelId == formModel.Id && f.Name == columnTable.ColumnName);
                    if (field != null)
                        continue;
                    field = new Field();
                    field.Name = columnTable.ColumnName;
                    field.FormModel = formModel;
                    field.MaxLength = columnTable.Maxlength;
                    field.Order = i;
                    field.OrderDetail = i;
                    field.DisplayName = columnTable.ColumnName;
                    field.TypeControl = GetTypeControl(columnTable.DataType);
                    field.TypeField = TypeField.ListAndEdit;
                    field.DefaultTypeAccec = TypeAccec.Full;
                    field.IsKey = !string.IsNullOrEmpty(columnTable.ColumnKey);
                    field.IsVisibleList = false;
                    field.WidthList = 20;
                    field.IsDetail = true;
                    field.TypeGroup = TypeGroup.None;
                    i = i + 10;
                    _reportDbContext.Add(field);
                }

                await _reportDbContext.SaveChangesAsync();
            }
        }

        public async Task GenerateDefaultBtn(int formModelId)
        {
            var formModel = await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == formModelId);
            if (formModel == null)
                return;
            await GenerateDefaultBtn(formModel);
        }

        private async Task GenerateDefaultBtn(FormModel formModel)
        {
            if (formModel.TableName == null)
                throw new UserMessageException("Не удалось определить таблицу");
            var btns = new[]
            {
                new { SysName = "Add", Name = "Добавить запись", IconName = "add", IsDetail = false, TypeBtn = TypeBtn.Normal, StylingMode = StylingMode.Contained, Location = Location.Before },
                new { SysName = "Edit", Name = "Редактировать запись", IconName = "edit", IsDetail = false, TypeBtn = TypeBtn.Normal, StylingMode = StylingMode.Contained, Location = Location.Before },
                new { SysName = "Copy", Name = "Копировать запись", IconName = "copy", IsDetail = false, TypeBtn = TypeBtn.Normal, StylingMode = StylingMode.Contained, Location = Location.Before },
                new { SysName = "Save", Name = "Сохранить", IconName = "save", IsDetail = true, TypeBtn = TypeBtn.Success, StylingMode = StylingMode.Outlined, Location = Location.After },
                new { SysName = "Delete", Name = "Удалить запись", IconName = "remove", IsDetail = false, TypeBtn = TypeBtn.Normal, StylingMode = StylingMode.Contained, Location = Location.Before },
                new { SysName = "Refresh", Name = "Обновить данные", IconName = "refresh", IsDetail = false, TypeBtn = TypeBtn.Normal, StylingMode = StylingMode.Contained, Location = Location.Before }
            };
            foreach (var btn in btns)
            {
                var btnModel = await _reportDbContext.ButtonForm.FirstOrDefaultAsync(f => f.FormId == formModel.Id && f.SysName == btn.SysName);
                if (btnModel != null)
                    continue;
                btnModel = new ButtonForm();
                btnModel.SysName = btn.SysName;
                btnModel.Height = 32;
                btnModel.Width = 32;
                btnModel.DisplayName = "";
                btnModel.ToolTip = btn.Name;
                btnModel.IsDetail = btn.IsDetail;
                btnModel.FormModel = formModel;
                btnModel.IconName = btn.IconName;
                btnModel.TypeBtn = btn.TypeBtn;
                btnModel.StylingMode = btn.StylingMode;
                btnModel.Location = btn.Location;
                await _reportDbContext.AddAsync(btnModel);
            }

            await _reportDbContext.SaveChangesAsync();
        }


        private TypeControl GetTypeControl(string columnType)
        {
            switch (columnType)
            {
                case "datetime" :
                    return TypeControl.DateEdit;
                case "bit":
                    return TypeControl.CheckBox;
                case "varchar":
                    return TypeControl.TextEdit;
                case "int":
                    return TypeControl.NumericEdit;
                case "decimal":
                    return TypeControl.NumericEdit;
                default: return TypeControl.TextEdit;
            }
        }


        private const string SelectField = @"SELECT COLUMN_NAME as ColumnName
                                                    ,DATA_TYPE as DataType
                                                    ,CHARACTER_MAXIMUM_LENGTH as MaxLength
                                                    ,COLUMN_KEY AS ColumnKey
                                                  FROM INFORMATION_SCHEMA.COLUMNS
                                                  WHERE TABLE_NAME = '{1}' AND TABLE_SCHEMA = '{0}'";
    }
}
