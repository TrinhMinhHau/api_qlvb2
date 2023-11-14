    using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Account;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class SysUserCL
    {
        private string _masterCacheKey = "SysUserCL";
        private CacheLayer _cache;
        private SysUserBL _userBL;

        public SysUserCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);
            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _userBL = new SysUserBL(connectDBString ?? "");
        }


        /// <summary>
        /// Lưu user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(UserInputModel model)
        {
            var result = _userBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách user SearchParamModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<UserModel> GetAll(SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllUser-", hashKey);
            //Get item from cache
            List<UserModel> users = _cache.GetCacheKey<List<UserModel>>(rawKey, _masterCacheKey)!;
            if (users == null)
            {
                users = _userBL.GetAll(model);
                _cache.AddCacheItem(rawKey, users);
            }
            return users;
        }

        /// <summary>
        /// Lấy danh sách user SearchParamModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<UserModel> GetSearch(SearchParamModel model, string idTruongs)
        {
            var hashKey = EHashMd5.FromObject(model) + idTruongs;
            var rawKey = string.Concat("GetAllUser-", hashKey);
            //Get item from cache
            List<UserModel> users = _cache.GetCacheKey<List<UserModel>>(rawKey, _masterCacheKey)!;
            if (users == null)
            {
                users = _userBL.GetSearch(model, idTruongs);
                _cache.AddCacheItem(rawKey, users);
            }
            return users;
        }

        /// <summary>
        /// Lấy user theo id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserModel GetByID(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetUser-", hashKey);

            //Get item from cache
            UserModel item = _cache.GetCacheKey<UserModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _userBL.GetById(id);
                _cache.AddCacheItem(rawKey, item);
            }

            return item;
        }


        /// <summary>
        /// Xóa user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            var result = _userBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Kích hoặc trạng thái
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Active(int id)
        {
            var result = _userBL.Active(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Ngưng hoạt động
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeActive(int id)
        {
            var result = _userBL.DeActive(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Reset lại mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int ResetPassword(int id, string password)
        {
            var result = _userBL.ResetPassword(id, password);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Lấy danh sách UserRole
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="model"></param>
        /// <returns> </returns>
        public List<UserRolesModel> GetUserRoles(int user_id, SearchParamModel model)
        {
            var rawKey = string.Concat("GetUserRoles-", user_id);
            //Get item from cache
            List<UserRolesModel> userRoles = _cache.GetCacheKey<List<UserRolesModel>>(rawKey, _masterCacheKey)!;
            if (userRoles == null)
            {
                userRoles = _userBL.GetUserRoles(user_id, model);
                _cache.AddCacheItem(rawKey, userRoles, _masterCacheKey);
            }
            return userRoles;
        }

        public List<UserReportModel> GetUserReports(int user_id, SearchParamModel model)
        {
            var rawKey = string.Concat("GetUserReports-", user_id);
            //Get item from cache
            List<UserReportModel> userReport = _cache.GetCacheKey<List<UserReportModel>>(rawKey, _masterCacheKey)!;
            if (userReport == null)
            {
                userReport = _userBL.GetUserReports(user_id, model);
                _cache.AddCacheItem(rawKey, userReport, _masterCacheKey);
            }
            return userReport;
        }

        public List<ReportModel> GetReportByUserId(int user_id)
        {
            var rawKey = string.Concat("GetReportByUserId-", user_id);
            //Get item from cache
            List<ReportModel> userReport = _cache.GetCacheKey<List<ReportModel>>(rawKey, _masterCacheKey)!;
            if (userReport == null)
            {
                userReport = _userBL.GetReportByUserID(user_id);
                _cache.AddCacheItem(rawKey, userReport, _masterCacheKey);
            }
            return userReport;
        }



        /// <summary>
        /// Lưu userRole
        /// </summary>
        /// <param name="model"></param>
        /// <returns> </returns>
        public int SaveUserRole(UserRoleInputModel model)
        {
            var result = _userBL.SaveUserRole(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public int SaveUserReport(UserReportInputModel model)
        {
            var result = _userBL.SaveUserReport(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
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
            int result = _userBL.ChangePassword(username, model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Lấy user theo username
        /// </summary>
        /// <param name="username"></param>
        /// <returns> </returns>
        public UserModel GetByUsername(string username)
        {
            var hashKey = EHashMd5.FromObject(username);
            var rawKey = string.Concat("GetUserByUsername-", hashKey);

            UserModel item = _cache.GetCacheKey<UserModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _userBL.GetByUsername(username);
                if(item.UserId > 0)
                {
                    // Thêm cache
                    _cache.AddCacheItem(rawKey, item, _masterCacheKey);
                }
            }

            return item;
        }

        /// <summary>
        /// Lấy user theo email
        /// </summary>
        /// <param name="username"></param>
        /// <returns> </returns>
        public UserModel GetByEmail(string email)
        {
            var hashKey = EHashMd5.FromObject(email);
            var rawKey = string.Concat("GetUserByEmail-", hashKey);

            UserModel item = _cache.GetCacheKey<UserModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _userBL.GetByEmail(email);
                if (item.UserId > 0)
                {
                    _cache.AddCacheItem(rawKey, item, _masterCacheKey);
                }
            }

            return item;
        }
    }
}
