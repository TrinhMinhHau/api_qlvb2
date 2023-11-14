using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Account;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Npgsql;
using System.Data.Common;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysUserBL : NpgsqlConnector
    {
        private string _connectionString;

        #region Name function or procedure
        private string _user_getall = "fn_sys_user_getall";
        private string _user_getall_by_truongs = "fn_sys_user_getall_by_truongs";
        private string _user_save = "p_sys_user_save";
        private string _user_getid = "fn_sys_user_getbyid";
        private string _user_delete = "p_sys_user_delete";
        private string _user_deactive = "p_sys_user_deactive";
        private string _user_active = "p_sys_user_active";
        private string _reset_password = "p_sys_reset_password";
        private string _user_getuserroles = "fn_sys_get_user_roles";
        private string _user_saveuserrole = "p_sys_user_role_save";
        private string _user_getbyusername = "fn_sys_user_getbyusername";
        private string _user_getbyemail = "fn_sys_user_getbyemail";
        private string _user_changepassword = "p_sys_user_changepassword";
        private string _user_report_save = "p_sys_user_report_save";
        private string _sys_get_user_report = "fn_sys_get_user_report";
        private string _sys_report_getbyuserid = "fn_sys_report_getbyuserid";
        #endregion


        #region Parameter
        private string p_username = "@p_username";
        private string p_password = "@p_password";
        private string p_user_id = "@p_user_id";
        private string p_fullname = "@p_fullname";
        private string p_email = "@p_email";
        private string p_salt = "@p_salt";
        private string p_role_ids = "@p_role_ids";
        private string p_report_ids = "@p_report_ids";
        private string p_avatar = "@p_avatar";
        private string p_gender = "@p_gender";
        private string p_birtday = "@p_birtday";
        private string p_phone = "@p_phone";
        private string p_address = "@p_address";
        private string p_cccd = "@p_cccd";
        private string p_truongId = "@p_truong_id";
        private string is_update_profile = "@is_update_profile";
        private string p_old_password = "@p_old_password";
        private string p_new_password = "@p_new_password";
        private string p_id_truongs = "@_id_truongs";

        #endregion

        private string p_created_by = "@p_created_by";
        public SysUserBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        #region User

        /// <summary>
        /// Lấy danh sách user SearchParamModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<UserModel> GetAll(SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_user_getall, parameters);

            var list = ModelProvider.CreateListFromTable<UserModel>(returnValue);

            return list;

        }

        public List<UserModel> GetSearch(SearchParamModel model, string idTruongs)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                    new NpgsqlParameter(p_id_truongs, string.IsNullOrEmpty(idTruongs) ? DBNull.Value : idTruongs),

                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_user_getall_by_truongs, parameters);

            var list = ModelProvider.CreateListFromTable<UserModel>(returnValue);

            return list;

        }

        /// <summary>
        /// Lưu user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(UserInputModel model)
        {
            string salt = SaltGenerator.GenerateSalt(100);
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_user_id, model.UserId),
                    new NpgsqlParameter(p_fullname, model.FullName),
                    new NpgsqlParameter(p_email, model.Email == null ? DBNull.Value : model.Email),
                    new NpgsqlParameter(p_username, model.UserName),
                    new NpgsqlParameter(p_password, model.Password == null ? DBNull.Value : model.Password),
                    new NpgsqlParameter(p_salt, model.Password == null ? DBNull.Value : salt),
                    new NpgsqlParameter(p_avatar,string.IsNullOrEmpty(model.Avatar) ? DBNull.Value : model.Avatar),
                    new NpgsqlParameter(p_gender,model.Gender == null ? DBNull.Value : model.Gender),
                    new NpgsqlParameter(p_birtday,model.Birthday == null ? DBNull.Value : model.Birthday),
                    new NpgsqlParameter(p_phone,string.IsNullOrEmpty(model.Phone) ? DBNull.Value : model.Phone),
                    new NpgsqlParameter(p_address,string.IsNullOrEmpty(model.Address) ? DBNull.Value : model.Address),
                    new NpgsqlParameter(p_cccd,string.IsNullOrEmpty(model.Cccd) ? DBNull.Value : model.Cccd),
                    new NpgsqlParameter(p_truongId,string.IsNullOrEmpty(model.TruongId) ? DBNull.Value : model.TruongId),
                    new NpgsqlParameter(p_created_by, model.CreatedBy == null ? DBNull.Value : model.CreatedBy),
                    new NpgsqlParameter(is_update_profile, model.IsUpdateProfile),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_user_save, parameters);

            return returnValue;

        }

        /// <summary>
        /// Lấy user theo id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserModel GetById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_user_id,id),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_user_getid, parameters);

            var list = ModelProvider.CreateListFromTable<UserModel>(returnValue);
            UserModel function = list.FirstOrDefault()!;

            return function;
        }

        /// <summary>
        /// Xóa user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_user_id, id)
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_user_delete, parameters);
            return returnValue;

        }

        /// <summary>
        /// Kích hoặc trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Active(int id)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_user_id, id)
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_user_active, parameters);
            return returnValue;

        }

        /// <summary>
        /// Ngưng hoạt động
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeActive(int id)
        {

            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_user_id, id)
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_user_deactive, parameters);
            return returnValue;
        }

        /// <summary>
        /// Reset lại mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int ResetPassword(int id, string password)
        {
            string salt = SaltGenerator.GenerateSalt(100);

            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_user_id, id),
                    new NpgsqlParameter(p_password, password),
                    new NpgsqlParameter(p_salt, salt)

               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_reset_password, parameters);
            return returnValue;
        }
        #endregion

        #region User Role
        /// <summary>
        /// Lấy danh sách UserRole
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="model"></param>
        /// <returns> </returns>
        public List<UserRolesModel> GetUserRoles(int user_id, SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(p_user_id, user_id),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_user_getuserroles, parameters);

            var list = ModelProvider.CreateListFromTable<UserRolesModel>(returnValue);

            return list;
        }

        /// <summary>
        /// Lưu userRole
        /// </summary>
        /// <param name="model"></param>
        /// <returns> </returns>
        public int SaveUserRole(UserRoleInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_user_id, model.UserId),
                    new NpgsqlParameter(p_role_ids,  string.IsNullOrEmpty(model.RoleIds) ? DBNull.Value : model.RoleIds),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_user_saveuserrole, parameters);

            return returnValue;

        }
        #endregion

        #region user_report
        public int SaveUserReport(UserReportInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_user_id, model.UserId),
                    new NpgsqlParameter(p_report_ids,  string.IsNullOrEmpty(model.ReportIds) ? DBNull.Value : model.ReportIds),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_user_report_save, parameters);

            return returnValue;

        }

        public List<UserReportModel> GetUserReports(int user_id, SearchParamModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(p_user_id, user_id),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.search), model.Search),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order), model.Order),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.order_dir), model.OrderDir),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_index), model.StartIndex),
                    new NpgsqlParameter(EnumExtensions.ToStringValue(EnumParam.page_size), model.PageSize),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_get_user_report, parameters);

            var list = ModelProvider.CreateListFromTable<UserReportModel>(returnValue);

            return list;
        }

        public List<ReportModel> GetReportByUserID(int user_id)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(p_user_id, user_id),
                 };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_sys_report_getbyuserid, parameters);

            var list = ModelProvider.CreateListFromTable<ReportModel>(returnValue);

            return list;
        }

        #endregion

        #region Manager Acccount
        /// <summary>
        /// Lấy user theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns> </returns>
        public UserModel GetByUsername(string username)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_username,username),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_user_getbyusername, parameters);

            var list = ModelProvider.CreateListFromTable<UserModel>(returnValue);
            UserModel user = list.FirstOrDefault()!;

            return user;
        }

        /// <summary>
        /// Lấy user theo email
        /// </summary>
        /// <param name="email"></param>
        /// <returns> </returns>
        public UserModel GetByEmail(string email)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(p_email,email),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_user_getbyemail, parameters);

            var list = ModelProvider.CreateListFromTable<UserModel>(returnValue);
            UserModel user = list.FirstOrDefault()!;

            return user;
        }

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="model"></param>
        /// <param name="username"></param>
        /// <returns>
        ///     Trả về kiểu int: 
        ///     + > 0 (Thay đổi mật khẩu thành công)
        ///     + == -1 (Mật khẩu hiện tại không đúng)
        ///     + == -9 (Người dùng không tồn tại)
        ///     + == -10 (Đổi mật khẩu thất bại)
        /// </returns>
        public int ChangePassword(string username, AccountInputModel model)
        {
            string salt = SaltGenerator.GenerateSalt(100);
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_username, username),
                    new NpgsqlParameter(p_old_password, model.Password),
                    new NpgsqlParameter(p_new_password, model.NewPassword),
                    new NpgsqlParameter(p_salt, salt),
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_user_changepassword, parameters);
            return returnValue;
        }
        #endregion
    }
}
