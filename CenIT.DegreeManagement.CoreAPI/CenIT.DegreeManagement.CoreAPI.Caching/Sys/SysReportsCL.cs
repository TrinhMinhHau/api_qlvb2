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
    public class SysReportsCL
    {
        private string _masterCacheKey = "SysReportsCL";
        private CacheLayer _cache;
        private SysReportsBL _sysReportsBL;

        public SysReportsCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _sysReportsBL = new SysReportsBL(connectDBString ?? "");
        }

        /// <summary>
        /// Lấy danh sách quyền
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ReportModel> GetAll(SearchParamModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAll-", hashKey);

            //Get item from cache
            List<ReportModel> reports = _cache.GetCacheKey<List<ReportModel>>(rawKey, _masterCacheKey)!;
            if (reports == null)
            {
                reports = _sysReportsBL.GetAll(model);
                _cache.AddCacheItem(rawKey, reports, _masterCacheKey);
            };

            return reports;
        }

        /// <summary>
        /// Lưu bao cáo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Save(ReportInputModel model)
        {
            var result = _sysReportsBL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public int Delete(int id)
        {
            var result = _sysReportsBL.Delete(id);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public ReportModel GetByID(int? id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetByID-", hashKey);

            //Get item from cache
            ReportModel item = _cache.GetCacheKey<ReportModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _sysReportsBL.GetById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }
    }
}
