using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class SysPermissionCL
    {
        private string _masterCacheKey = "SysPermissionCL";
        private CacheLayer _cache;
        private SysPermissionBL _sysPermissionBL;

        public SysPermissionCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysPermissionBL = new SysPermissionBL(connectDBString ?? "");
        }

        /// <summary>
        /// Kiểm tra quyền truy cập theo username, function, action
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="function"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public int Permission_IsAllow(string UserName, string function, string action)
        {
            var result = _sysPermissionBL.Permission_IsAllow(UserName, function, action);
            return result;
        }


        /// <summary>
        /// Lấy danh sách permission theo roleid 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<PermissionModel> GetPermissionByRoleID(int id, SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(string.Concat(model));
            var rawKey = string.Concat("GetAllFunction-", hashKey);

            //Get item from cache
            List<PermissionModel> item = _cache.GetCacheKey<List<PermissionModel>>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysPermissionBL.GetPermissionById(id, model);
                _cache.AddCacheItem(rawKey, item);
            }

            return item;
        }


        /// <summary>
        /// Lưu permission
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(PermissionInputModel model)
        {

            var result = _sysPermissionBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache();
            }

            return result;
        }


        /// <summary>
        /// Xóa permission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            var result = _sysPermissionBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache();
            }

            return result;
        }

    }
}
