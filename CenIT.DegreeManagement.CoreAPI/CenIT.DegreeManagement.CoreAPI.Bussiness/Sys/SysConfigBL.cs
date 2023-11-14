using Amazon.Runtime.Internal.Util;
using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Npgsql;
using System.Data.Common;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysConfigBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _config_save = "p_sys_config_save";
        private string _config_delete = "p_sys_config_delete";
        private string _config_getbyid = "fn_sys_config_getbyid";
        private string _config_getbykey = "fn_sys_config_getbykey";
        private string _config_getall = "fn_sys_config_getall";
        #endregion

        #region parameter
        private string p_config_id = "@p_config_id";
        private string p_config_key = "@p_config_key";
        private string p_config_value = "@p_config_value";
        private string p_config_desc = "@p_config_desc";
        private string p_created_by = "@p_created_by";
        private string p_last_modified_by = "@p_last_modified_by";
        #endregion

        public SysConfigBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách cấu hình
        /// </summary>
        /// <param name="SearchParamModel"></param>
        /// <returns></returns>
        public List<ConfigModel> GetAllConfig(SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_config_getall, parameters);

            var list = ModelProvider.CreateListFromTable<ConfigModel>(returnValue);

            return list;
        }

        /// <summary>
        /// Lưu cấu hình
        /// </summary>
        /// <param name="ConfigInputModel"></param>
        /// <returns>
        ///  returnValue > 0 : Lưu thành công
        ///  returnValue == -9: Cấu hình tồn tại
        ///  returnValue == -10: Cấu hình không tồn tại
        ///  returnValue == -1: Lưu thất bại
        /// </returns>
        public int Save(ConfigInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_config_id, model.ConfigId),
                    new NpgsqlParameter(p_config_key, model.ConfigKey),
                    new NpgsqlParameter(p_config_value,string.IsNullOrEmpty(model.ConfigValue) ? DBNull.Value : model.ConfigValue),
                    new NpgsqlParameter(p_config_desc, string.IsNullOrEmpty(model.ConfigDesc) ? DBNull.Value : model.ConfigDesc),
                    new NpgsqlParameter(p_created_by, model.CreatedBy),
                    new NpgsqlParameter(p_last_modified_by, model.LastModifiedBy),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_config_save, parameters);

            return returnValue;
        }

        /// <summary>
        /// Xóa cấu hình
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///  returnValue > 0 : Xóa thành công
        ///  returnValue < 0: Cấu hình không tồn tại
        /// </returns>
        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_config_id, id)
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_config_delete, parameters);
            return returnValue;

        }

        /// <summary>
        /// Lấy cấu hình theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>/// </returns>
        public ConfigModel GetConfigById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(p_config_id,id),
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_config_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<ConfigModel>(returnValue);
            ConfigModel function = list.FirstOrDefault()!;

            return function;
        }

        /// <summary>
        /// Lấy cấu hình theo key
        /// </summary>
        /// <param name="configkey"></param>
        /// <returns>/// </returns>
        public ConfigModel GetConfigByKey(string? configKey)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(p_config_key, configKey),
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_config_getbykey, parameters);

            var list = ModelProvider.CreateListFromTable<ConfigModel>(returnValue);
            ConfigModel function = list.FirstOrDefault()!;

            return function;
        }

    }
}
