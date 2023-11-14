using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class MessageBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _message_getbypagesize = "fn_sys_message_getbypagesize";
        private string _message_update_status = "p_sys_message_update_status";
        private string _sys_getall_message = "fn_sys_getall_message";
        private string _message_save = "message_save";
        private string _message_save_new = "message_save_new";
        private string _sys_message_getsearch_byuserid = "fn_sys_message_getsearch_byuserid";


        private string _message_update_status_all = "p_sys_message_update_status_all";
        private string _count_message_unread = "p_sys_count_message_unread";
        private string _sys_message_getsearch = "fn_sys_message_getsearch";
        private string _message_getbyid = "fn_message_getbyid";


        #endregion

        #region Parameter
        private string _user_id = "@_user_id";
        private string page_size = "@page_size";
        private string p_id_message = "@p_id_message";
        private string p_message_id = "@p_message_id";
        private string p_action = "@p_action";
        private string p_message_type = "@p_message_type";
        private string p_sending_method = "@p_sending_method";
        private string p_title = "@p_title";
        private string p_content = "@p_content";
        private string p_color = "@p_color";
        private string p_recipient = "@p_recipient";
        private string p_url = "@p_url";
        private string p_is_notification = "@p_is_notification";
        private string p_id_donvi = "@p_id_donvi";
        private string _fromDate = "@_fromDate";
        private string _toDate = "@_toDate";
        private string p_value_redirect = "@p_value_redirect";


        #endregion

        public MessageBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }


        public List<MessageModel> GetByPageSize(int userId, int pageSize)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(_user_id, userId),
                    new NpgsqlParameter(page_size, pageSize),

                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_message_getbypagesize, parameters);

            var list = ModelProvider.CreateListFromTable<MessageModel>(returnValue);

            return list;

        }

        /// <summary>
        /// Lấy danh sách nin nhắn
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<NotificationModel> GetAllMessages(SearchParamFilterDateModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                    new NpgsqlParameter(_fromDate, model.FromDate == null ? DBNull.Value : model.FromDate),
                    new NpgsqlParameter(_toDate,  model.ToDate == null ? DBNull.Value : model.ToDate),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_message_getsearch, parameters);

            var list = ModelProvider.CreateListFromTable<NotificationModel>(returnValue);

            return list;
        }

        /// <summary>
        /// Lấy danh sách tin nhắn theo userId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<NotificationModel> GetAllMessagesByUserId(int userId, SearchParamFilterDateModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(_user_id, userId),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                    new NpgsqlParameter(_fromDate, model.FromDate == null ? DBNull.Value : model.FromDate),
                    new NpgsqlParameter(_toDate,  model.ToDate == null ? DBNull.Value : model.ToDate),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_message_getsearch_byuserid, parameters);

            var list = ModelProvider.CreateListFromTable<NotificationModel>(returnValue);

            return list;
        }


        public int Save(MessageInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(p_id_message, model.IdMessage),
                    new NpgsqlParameter(p_action, model.Action),
                    new NpgsqlParameter(p_message_type, model.MessageType),
                    new NpgsqlParameter(p_sending_method, model.SendingMethod),
                    new NpgsqlParameter(p_title, model.Title),
                    new NpgsqlParameter(p_content, model.Content),
                    new NpgsqlParameter(p_color, string.IsNullOrEmpty(model.Color) ? DBNull.Value : model.Color),
                    new NpgsqlParameter(p_recipient, string.IsNullOrEmpty(model.Recipient) ? DBNull.Value : model.Recipient),
                     new NpgsqlParameter(p_url, string.IsNullOrEmpty(model.Url) ? DBNull.Value : model.Url),
                     new NpgsqlParameter(p_is_notification, model.IsNotification),
                     new NpgsqlParameter(p_id_donvi, string.IsNullOrEmpty(model.IDDonVi) ? DBNull.Value : model.IDDonVi),
                     new NpgsqlParameter(p_value_redirect, string.IsNullOrEmpty(model.ValueRedirect) ? DBNull.Value : model.ValueRedirect),
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_message_save_new, parameters);

            return returnValue;
        }

        public MessageModel GetMessageById(string id)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(p_message_id,id),
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_message_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<MessageModel>(returnValue);
            MessageModel message = list.FirstOrDefault()!;

            return message;
        }

        public int UpdateReadStatus(string idMessage)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(p_id_message, idMessage)
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_message_update_status, parameters);

            return returnValue;
        }

        public int UpdateAllReadStatus(int userId)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(_user_id, userId)
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_message_update_status_all, parameters);

            return returnValue;
        }

        public int GetUnreadMessagesCount(int? userId)
        {
            DbParameter[] parameters = new DbParameter[]
            {
                    new NpgsqlParameter(_user_id, userId)
            };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_count_message_unread, parameters);

            return returnValue;
        }

        public List<MessageModel> GetAll(int userId)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(_user_id, userId),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_getall_message, parameters);

            var list = ModelProvider.CreateListFromTable<MessageModel>(returnValue);

            return list;

        }
    }
}
