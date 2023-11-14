using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class SysRoleCL
    {
        private string _masterCacheKey = "SysRoleCL";
        private CacheLayer _cache;
        private SysRoleBL _sysRoleBL;

        public SysRoleCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysRoleBL = new SysRoleBL(connectDBString ?? "");
        }

        /// <summary>
        /// Lấy danh sách quyền
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<RoleModel> GetAll(SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllRole-", hashKey);


            //Get item from cache
            List<RoleModel> roles = _cache.GetCacheKey<List<RoleModel>>(rawKey, _masterCacheKey)!;
            if (roles == null)
            {
                roles = _sysRoleBL.GetAll(model);
                _cache.AddCacheItem(rawKey, roles, _masterCacheKey);
            };

            return roles;
        }

        /// <summary>
        /// Lấy quyền theo id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RoleModel GetByID(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetRole-", hashKey);

            //Get item from cache
            RoleModel item = _cache.GetCacheKey<RoleModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysRoleBL.GetById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }

        /// <summary>
        /// Lưu quyền
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(RoleInputModel model)
        {
            var result = _sysRoleBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Xóa quyền
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            var result = _sysRoleBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }
    }
}
