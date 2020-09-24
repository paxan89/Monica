using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Monica.Core.Abstraction.ReportEngine;
using Monica.Core.DbModel.Extension;
using Monica.Core.DbModel.ModelCrm;
using Monica.Core.DbModel.ModelCrm.Core;
using Monica.Core.DbModel.ModelDto;
using Monica.Core.DbModel.ModelDto.Core;
using Monica.Core.DbModel.ModelDto.Report;
using Monica.Core.Service.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlKata;
using SqlKata.Execution;

namespace Monica.Core.Service.ReportEngine
{
    /// <summary>
    /// Формирование данных по умолчанию
    /// </summary>
    public class ReportEngineDefaultData : IReportEngineData
    {
        private readonly ReportDbContext _reportDbContext;
        private readonly IReportManager _reportManager;
        private readonly IConnectorManager _connectorManager;

        public ReportEngineDefaultData(ReportDbContext reportDbContext, IReportManager reportManager, IConnectorManager connectorManager)
        {
            _reportDbContext = reportDbContext;
            _reportManager = reportManager;
            _connectorManager = connectorManager;
        }

        /// <summary>
        /// Получить данные для формы редактирования данных
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<IDictionary<string, object>>> GetDataList(BaseModelReportParam p)
        {
            try
            {
                var formModel = await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == p.FormId);
                if (formModel == null)
                    return null;
                var fileds = (await _reportManager.GetFieldsFormAsync(p.UserName, p.FormId, false, fields =>
                    fields.Where(f => !(f.IsVirtual ?? false)))).ToList();
                if (fileds.Count == 0)
                    return null;
                using var connection = _connectorManager.GetConnection();
                var typeControls = new[] { TypeControl.Details, TypeControl.FormSelect };
                
                var db = new QueryFactory(connection, _connectorManager.Compiler);

                var query = (await GenerateSqlView(fileds.Where(f => !typeControls.Contains(f.TypeControl)),
                    formModel.TableName));
                if (p.FormIdByDetail != 0)
                    query.Where(p.FieldWhere, "=", p.FormIdByDetail);
                query.ToString();
                var resQuery = await db.FromQuery(query).GetAsync();

                return resQuery.Cast<IDictionary<string, object>>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        /// <summary>
        /// Получить модель для редактирования данных
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual async Task<IDictionary<string, object>> GetDataEditModel(BaseModelReportParam p)
        {
            var formModel = await _reportDbContext.FormModel.FirstOrDefaultAsync(f => f.Id == p.FormId);
            if (formModel == null)
                return null;
            var fileds = (await _reportManager.GetFieldsFormAsync(p.UserName, p.FormId, false)).ToList();
            using var connection = _connectorManager.GetConnection();
            var typeControls = new[] { TypeControl.Details, TypeControl.FormSelect };

            var db = new QueryFactory(connection, _connectorManager.Compiler);

            var query = await GenerateSqlViewEditModel(fileds.Where(f => !typeControls.Contains(f.TypeControl)),
                formModel.TableName, p.ModelId);
            var resQuery = await db.FromQuery(query).FirstOrDefaultAsync();
            return resQuery as IDictionary<string, object>;
        }

        /// <summary>
        /// Проверка сохранеяемой модели по предоставленным правилам
        /// </summary>
        /// <param name="saveModel">Модель для сохранения</param>
        /// <param name="formModelId">Системный номер модели данных</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> ValidateModel(dynamic saveModel, FormModelDto formModelId)
        {
            var result = new ResultCrmDb();
            if (!(saveModel is JObject))
                return result;
            var saveRuleModel = saveModel as JObject;
            using (var connection = _connectorManager.GetConnection())
            {
                var models = _reportDbContext.ValidationRuleEntity.Where(f => f.FormModelId == formModelId.Id && f.IsDeleted == false);
                foreach (var ruleEntity in models)
                {
                    if (ruleEntity.TypeValidation == TypeValidation.Component)
                        continue;
                    var sql = ruleEntity.Content;
                    foreach (var jObject in saveRuleModel)
                    {
                        sql = sql.Replace($"[{jObject.Key}]", jObject.Value.ToObject<string>());
                    }

                    var res = await connection.QueryFirstOrDefaultAsync<int>(sql);
                    if (res > 0)
                        result.AddError(ruleEntity.Name, ruleEntity.ToolTip);
                }
            }

            return result;
        }

        /// <summary>
        /// Сохранение измененных данных с фронта
        /// </summary>
        /// <param name="p">Класс параметров для сохранения данных</param>
        /// <param name="userName"></param>
        /// <param name="formModelSave">Модель для сохранения данных</param>
        /// <param name="formModel"></param>
        /// <returns></returns>
        public virtual async Task<ResultCrmDb> SaveModels(FormModelDto formModel, string userName, dynamic formModelSave)
        {
            var result = new ResultCrmDb();
            if (!(formModelSave is JObject))
                return result;
            var modelSave = formModelSave as JObject;
            var fieldsBase = (await _reportManager.GetFieldsFormAsync(userName, formModel.Id, false));
            var fileds = fieldsBase.Where(f => (((f.IsDetail ?? false) && !(f.IsVirtual ?? false)) 
                                                || modelSave.Properties().FirstOrDefault(ff => ff.Name == ColumnHelper.GetFieldName(f.Name)) != null) || (f.IsKey ?? false)).ToList();

            var data = formModelSave as JObject;
            var fieldKey = fileds.FirstOrDefault(f => f.IsKey ?? false);
            if (fieldKey == null)
                return result;

            var valueKey = data.GetValue(fieldKey.Name)?.Value<string>();
            bool isNew = string.IsNullOrWhiteSpace(valueKey) || valueKey == "0";

            using var connection = _connectorManager.GetConnection();
            var db = new QueryFactory(connection, _connectorManager.Compiler);

            var query = db.Query(formModel.TableName);
            var columns = fileds.Select(s => ColumnHelper.GetFieldName(s.Name));
            var dataSave = fileds.Select(s =>
            {
                var value = data.GetValue(ColumnHelper.GetFieldName(s.Name))?.Value<string>();
                return s.TypeControl switch
                {
                    TypeControl.DateEdit => (value == null ? null : data.GetValue(s.Name)?.Value<DateTime>()) as object,
                    TypeControl.NumericEdit => (value == null ? 0 : data.GetValue(s.Name)?.Value<decimal>()) as object,
                    TypeControl.CheckBox => (value == null ? null : data.GetValue(s.Name)?.Value<bool>()) as object,
                    _ => value as object
                };
            }).ToList();
            if (isNew)
            {
                int i = 0;
                var dictionary = new Dictionary<string, object>(columns.Select(s =>
                {
                    var item = new KeyValuePair<string, object>(s, dataSave[i]);
                    i++;
                    return item;
                }));
                var id = await query.InsertGetIdAsync<int>(dictionary);
                result.Result = id;
            }
            else
            {
                query.Where(fieldKey.Name, "=", valueKey).AsUpdate(columns, dataSave);
                await db.ExecuteAsync(query);
                result.Result = valueKey;
            }

            var queryModel = await GenerateSqlView(fileds, formModel.TableName);
            queryModel.Where($"{formModel.TableName}.{fieldKey.Name}", "=", result.Result);
            queryModel = db.FromQuery(queryModel);
            var resultModel = (await queryModel.GetAsync()).FirstOrDefault();
            result.Result = resultModel == null ? "" : JsonConvert.SerializeObject(resultModel);
            return result;
        }
        /// <summary>
        /// Удаление моделей данных
        /// </summary>
        /// <param name="formModel">Сущность для которой нужно удалить данные</param>
        /// <param name="userName">Пользователь, который удаляет данные</param>
        /// <param name="key">Список ключей сущности, которые нужно удалить</param>
        /// <returns></returns>
        public async Task<ResultCrmDb> RemoveEntity(FormModelDto formModel, string userName, string[] key)
        {
            var result = new ResultCrmDb();
            var fileds = (await _reportManager.GetFieldsFormAsync(userName, formModel.Id, false)).Where(f => (f.IsDetail ?? false) && !(f.IsVirtual ?? false)).ToList();

            var fieldKey = fileds.FirstOrDefault(f => f.IsKey ?? false);
            if (fieldKey == null)
                return result;

            using var connection = _connectorManager.GetConnection();
            var db = new QueryFactory(connection, _connectorManager.Compiler);
            var query = db.Query(formModel.TableName).WhereIn(fieldKey.Name, key).AsDelete();
            await db.ExecuteAsync(query);
            return result;
        }

        /// <summary>
        /// Получить модель данных для редактирования детальной информации
        /// </summary>
        /// <param name="formModel">Модель</param>
        /// <returns></returns>
        public async Task<IEnumerable<SolutionModel>> GetDetailInfoModels(FormModelDto formModel)
        {
            using var connection = _connectorManager.GetConnection();
            var db = new QueryFactory(connection, _connectorManager.Compiler);
            var query = db.Query("field as f").Join("formModel as m", "m.Id", "f.FormModelDetailId")
                .Where("FormModelId", formModel.Id)
                .Select(
                    "m.Caption as Text"
                    , "m.Id as IdModel"
                    , "f.Name as FieldWhere"
                    ).SelectRaw("'DesignerDictionary' as VueComponent");
            return await query.GetAsync<SolutionModel>();
        }

        private async Task<Query> GenerateSqlViewEditModel(IEnumerable<FieldAccessDto> fieldAccess, string formModel, string id)
        {
            var fieldAccessDtos = fieldAccess as FieldAccessDto[] ?? fieldAccess.ToArray();

            var tablePk = (await GetTablePrimaryKey(new string[] { }, formModel)).FirstOrDefault();
            var queryTable =
                (await GetTables(
                    fieldAccessDtos.Select(s => new ColumnTable
                    { ColumnName = s.Name, TableName = s.TableName }).Where(f =>
                      !string.IsNullOrEmpty(f.TableName) &&
                      !string.IsNullOrWhiteSpace(f.ColumnName)).ToArray(), formModel))
                .Where($"{formModel}.{tablePk?.ColumnName}", "=", id);
            return queryTable;
        }

        private async Task<Query> GenerateSqlView(IEnumerable<FieldAccessDto> fieldAccess, string formModel)
        {
            var fieldAccessDtos = fieldAccess as FieldAccessDto[] ?? fieldAccess.ToArray();
            var tableName = fieldAccessDtos.GroupBy(g => g.TableName).Select(s => s.Key)
                .Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();
            var queryTable =
                await GetTables(
                    fieldAccessDtos.Select(s => new ColumnTable
                    { ColumnName = s.Name, TableName = s.TableName, IsOneToOne = s.IsOneToOne ?? false }).Where(f =>
                       !string.IsNullOrEmpty(f.TableName) &&
                       !string.IsNullOrWhiteSpace(f.ColumnName)).ToArray(), formModel);
            GetColumn(fieldAccessDtos, formModel, queryTable);

            return queryTable;
        }

        private async Task<Query> GetTables(ColumnTable[] tableName, string formModel)
        {
            var tablePk = (await GetTablePrimaryKey(tableName.Select(s => s.TableName).ToArray(), formModel)).ToList();
            var queryBuilder = new Query($"{formModel} as {formModel}");
            foreach (var tbJoin in tableName.Where(f => f.TableName != formModel).GroupBy(g => g.TableName)
                .Select(s => new { TableName = s.Key, s.FirstOrDefault()?.ColumnName }))
            {

                var pk = tablePk.FirstOrDefault(f => f.TableName == tbJoin.TableName);
                if (pk == null)
                    continue;

                var columnJoin = GetJoinTable(tbJoin.ColumnName);
                var columnPk = GetPkTable(tbJoin.ColumnName, pk.ColumnName);

                queryBuilder.LeftJoin($"{tbJoin.TableName} as {tbJoin.TableName}", $"{tbJoin.TableName}.{columnPk}",
                    $"{formModel}.{columnJoin}");
            }

            return queryBuilder;
        }

        private string GetColumnBySelect(string columnJoin, string tableName, bool isDetail, Query query, bool isSorting)
        {
            var column = GetFieldSelectTable(columnJoin, isDetail);
            string columnResult = $"{tableName}.{column}";

            if (!columnJoin.Contains("{"))
            {
                query.Select(columnResult);
                if (isSorting)
                    query.OrderBy(columnResult);
                return columnResult;
            }

            var obj = JObject.Parse(columnJoin);
            var alias = obj["Alias"];
            if (alias == null)
            {
                query.Select(columnResult);
                if (isSorting)
                    query.OrderBy(columnResult);
                return columnResult;
            }
            query.SelectRaw(column);
            if (isSorting)
                query.OrderBy(obj["Field"]?.ToString());
            return column;
        }

        private string GetFieldSelectTable(string columnJoin, bool isDetail)
        {
            if (!columnJoin.Contains("{"))
                return columnJoin;
            var obj = JObject.Parse(columnJoin);
            var alias = obj["Alias"] ?? obj["Fk"];
            return string.Format("{0} as {1}",isDetail ? obj["DetailField"] : obj["Field"], alias);
        }

        private string GetJoinTable(string columnJoin)
        {
            if (!columnJoin.Contains("{"))
                return columnJoin;
            var obj = JObject.Parse(columnJoin);
            return obj["Fk"]?.ToString();
        }

        private string GetPkTable(string columnJoin, string columnPk)
        {
            if (!columnJoin.Contains("{"))
                return columnPk;
            var obj = JObject.Parse(columnJoin);
            return obj["Pk"]?.ToString();
        }

        private string[] GetColumn(IEnumerable<FieldAccessDto> fieldAccess, string formModel, Query queryTable)
        {
            return fieldAccess.Select(s =>
                    GetColumnBySelect(s.Name, (string.IsNullOrWhiteSpace(s.TableName) ? formModel : s.TableName),
                        false, queryTable, s.Sorting != null && s.Sorting != 0))
                .ToArray();

        }

        private async Task<IEnumerable<ColumnTable>> GetTablePrimaryKey(string[] table, string formModel)
        {
            var tables = string.Join(',', table.Union(new[] { formModel }).Select(s => $"'{s}'"));
            using var connection = _connectorManager.GetConnection();
            return await connection.QueryAsync<ColumnTable>(string.Format(GetTablePk, connection.Database, tables));
        }


        private const string GetTablePk = @"SELECT COLUMN_NAME as ColumnName,TABLE_NAME AS TableName
                                                FROM INFORMATION_SCHEMA.COLUMNS
                                                WHERE TABLE_NAME in ({1}) AND TABLE_SCHEMA ='{0}' and COLUMN_KEY = 'PRI'";


    }
}
