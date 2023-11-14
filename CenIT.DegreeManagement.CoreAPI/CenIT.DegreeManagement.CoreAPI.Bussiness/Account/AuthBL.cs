using Amazon.Runtime.Internal.Util;
using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Account;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Account
{
    public class AuthBL : NpgsqlConnector
    {
        private string _connectionString;
        private string _user_login = "fn_user_login";
        private string _get_user_by_token = "fn_sys_user_token_getuserbytoken";
        private string _logout = "p_logout";
        private string _save_password_reset_token = "p_save_password_reset_token";
        private string _reset_password = "p_reset_password";

        // Các parametter
        private string p_username = "@p_username";
        private string p_password = "@p_password";
        private string p_token = "@p_token";
        private string p_email = "@p_email";
        private string p_password_reset_token = "@p_password_reset_token";
        private string p_reset_token_expires = "@p_reset_token_expires";
        private string p_salt = "@p_salt";


        public AuthBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns>
        ///     Trả về ResponseLogin trong đó:
        ///     + userId > 0 (Đăng nhập thành công)
        ///     + userId = -9 (Tài khoản đã ngưng hoạt động)
        ///     + userId = -1 (Tài khoản hoặc mật khẩu không tồn tại)
        /// </returns>
        public ResponseLogin Login(AccountInputModel model)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(p_username, model.Username),
                    new NpgsqlParameter(p_password, model.Password)
                 };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_user_login, parameters);

            var list = ModelProvider.CreateListFromTable<ResponseLogin>(returnValue);
            ResponseLogin userLogin = list.FirstOrDefault()!;

            return userLogin;
        }

        /// <summary>
        /// Đăng Xuất
        /// </summary>
        /// <param name="token"></param>
        /// <returns>
        ///     Trả về kiểu int: 
        ///     + > 0 (Đăng xuất thành công)
        ///     + < 0 (Đăng xuất thất bại)
        /// </returns>
        public int Logout(string token)
        {
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_token, token)
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_logout, parameters);
            return returnValue;
        }

        /// <summary>
        /// Lưu token reset password 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <param name="tokenExpires">Thời gian hết hạn của token (1 ngày)</param>
        /// <returns></returns>
        public int SavePasswordResetToken(string email, string token , DateTime tokenExpires)
        {
          
            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_email, email),
                    new NpgsqlParameter(p_password_reset_token, token),
                    new NpgsqlParameter(p_reset_token_expires, tokenExpires)
               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_save_password_reset_token, parameters);
            return returnValue;
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="ResetPasswordModel"></param>
        /// <returns>
        ///    Trả về kiểu int: 
        ///     + > 0 (Reset thành công)
        ///     + == -1 (Token không đúng hoặc token hết hạn)
        ///     + == -9 (email không tồn tại)
        ///     + reset thất bại
        /// </returns>
        public int ResetPassword(AccountInputModel model)
        {
            string salt = SaltGenerator.GenerateSalt(100);

            DbParameter[] parameters = new DbParameter[]
               {
                    new NpgsqlParameter(p_email, model.Email),
                    new NpgsqlParameter(p_password_reset_token, model.Token),
                    new NpgsqlParameter(p_password, model.Password),
                    new NpgsqlParameter(p_salt, salt)

               };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_reset_password, parameters);
            return returnValue;
        }

        public ResponseLogin GetTokenFromDB(string username, string token)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(p_username, username),
                    new NpgsqlParameter(p_token, token)
                 };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_get_user_by_token, parameters);

            var list = ModelProvider.CreateListFromTable<ResponseLogin>(returnValue);
            ResponseLogin user = list.FirstOrDefault()!;

            return user != null ? user : null;
        }
    }
}