using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysMessageConfigBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _sys_message_config_getbyactionname = "fn_sys_message_config_getbyactionname";
        private string _sys_message_config_getall = "fn_sys_message_config_getall";
        private string _sys_message_config_getbyid = "fn_sys_message_config_getbyid";
        private string _message_config_save = "sys_message_config_save";
        private string _message_config_delete = "p_sys_message_config_delete";

        #endregion

        #region Parameter
        private string p_message_config_id = "@p_message_config_id";
        private string p_action_name = "@p_action_name";
        private string p_description = "@p_description";
        private string p_title = "@p_title";
        private string p_body = "@p_body";
        private string p_url = "@p_url";
        private string p_color = "@p_color";


        private string p_user_action = "@p_user_action";

        #endregion

        public SysMessageConfigBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        public MessageConfigModel GetByActionName(string actionName)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_action_name,actionName),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_message_config_getbyactionname, parameters);

            var list = ModelProvider.CreateListFromTable<MessageConfigModel>(returnValue);
            MessageConfigModel messageConfig = list.FirstOrDefault()!;

            return messageConfig;
        }

        public MessageConfigModel GetById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_message_config_id, id),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_message_config_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<MessageConfigModel>(returnValue);
            MessageConfigModel messageConfig = list.FirstOrDefault()!;

            return messageConfig;
        }

        public List<MessageConfigModel> GetAll(SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_message_config_getall, parameters);

            var list = ModelProvider.CreateListFromTable<MessageConfigModel>(returnValue);

            return list;

        }

        public int Save(MessageConfigInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(p_message_config_id, model.MessageConfiId),
                    new NpgsqlParameter(p_action_name, model.ActionName),
                    new NpgsqlParameter(p_description, string.IsNullOrEmpty(model.Description) ? DBNull.Value : model.Description),
                    new NpgsqlParameter(p_title, model.Title),
                    new NpgsqlParameter(p_body, model.Body),
                    new NpgsqlParameter(p_url, string.IsNullOrEmpty(model.URL) ? DBNull.Value : model.URL),
                    new NpgsqlParameter(p_color, string.IsNullOrEmpty(model.Color) ? DBNull.Value : model.Color),
                    new NpgsqlParameter(p_user_action, model.UserAction),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_message_config_save, parameters);

            return returnValue;
        }

        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_message_config_id, id),

               };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_message_config_delete, parameters);
            return returnValue;

        }
    }
}
