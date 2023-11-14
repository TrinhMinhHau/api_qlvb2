using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;


namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class SysMenuCL
    {
        private string _masterCacheKey = "SysMenu";
        private CacheLayer _cache;
        private SysMenuBL _sysMenuBL;
        public SysMenuCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysMenuBL = new SysMenuBL(connectDBString ?? "");
        }


        /// <summary>
        /// Lấy danh sách menu theo username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<MenuModel> GetMenuByUsername(string username, string token)
        {
            var hashKey = EHashMd5.FromObject(token);
            var rawKey = string.Concat("GetMenuByUsername-", hashKey);

            //Get item from cache
            List<MenuModel> menu = _cache.GetCacheKey<List<MenuModel>>(rawKey, _masterCacheKey)!;
            if (menu == null)
            {
                menu = _sysMenuBL.GetMenuByUserName(username);
                _cache.AddCacheItem(rawKey, menu, _masterCacheKey);
            };

            return menu;
        }

        /// <summary>
        /// Lấy menu theo id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>

        public MenuModel GetByID(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetMenuByID-", hashKey);

            //Get item from cache
            MenuModel item = _cache.GetCacheKey<MenuModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysMenuBL.GetById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }

        /// <summary>
        /// Lấy danh sách tất cả menu
        /// </summary>
        /// <returns></returns>

        public List<MenuModel> GetAllMenu()
        {
            var rawKey = "GetAllMenu";

            //Get item from cache
            List<MenuModel> menu = _cache.GetCacheKey<List<MenuModel>>(rawKey, _masterCacheKey)!;
            if (menu == null)
            {
                menu = _sysMenuBL.GetAllMenu();
                _cache.AddCacheItem(rawKey, menu, _masterCacheKey);
            };

            return menu;
        }

        /// <summary>
        /// Lưu menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveMenu(MenuInputModel model)
        {
            var result = _sysMenuBL.SaveMenu(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        /// <summary>
        /// Xóa menu
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public int DeleteMenu(int menuId)
        {
            var result = _sysMenuBL.DeleteMenu(menuId);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }
    }
}
