using CenIT.DegreeManagement.CoreAPI.Caching;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.ThongKe
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : BaseAppController
    {
        private ThongKeCL _thongKeCL;
        private ILogger<ThongKeController> _logger;
        private readonly ShareResource _localizer;
        private TruongCL _truongCL;
        private SysUserCL _sysUserCL;
        public ThongKeController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<ThongKeController> logger) : base(cacheService, configuration)
        {
            _logger = logger;
            _localizer = shareResource;
            _thongKeCL = new ThongKeCL(cacheService, configuration);
            _sysUserCL = new SysUserCL(cacheService);
            _truongCL = new TruongCL(cacheService, configuration);

        }

        #region PHÒNG

        /// <summary>
        /// Thống kê học sinh đã đổ tốt nghiệp theo trường và năm thi
        /// API: /api/ThongKe/GetHocSinhDoTotNghiepByTruongAndNam
        /// </summary>
        ///  <param name="modelSearch"></param>
        /// <returns></returns>

        [HttpGet("GetHocSinhDoTotNghiepByTruongAndNam")]
        [AllowAnonymous]
        public IActionResult GetHocSinhDoTotNghiepByTruongAndNam([FromQuery]HSByTruongNamSearchModel modelSearch)
        {
            int total;
            var data = _thongKeCL.GetHocSinhDoTotNghiepByTruongAndNam(out total,modelSearch);

            var outputData = new
            {
                hocSinh = data,
                totalRow = total,
            };
            return ResponseHelper.Ok(outputData);
        }


        /// <summary>
        /// Thống kê in phôi bằng
        /// API: /api/ThongKe/GetThongKeInPhoiBang
        /// </summary>
        ///  <param name="modelSearch"></param>
        /// <returns></returns>
        [HttpGet("GetThongKeInPhoiBang")]
        [AllowAnonymous]
        public IActionResult GetThongKeInPhoiBang([FromQuery] ThongKeInPhoiBangSearchModel modelSearch)
        {
            int total;
            var user = _sysUserCL.GetByUsername(modelSearch.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            modelSearch.MaHeDaoTao = donVi.MaHeDaoTao;

            var data = _thongKeCL.GetThongKeInPhoiBang(out total, modelSearch);

            var outputData = new
            {
                phoiGocs = data,
                totalRow = total,
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Thống kê phát bằng
        /// API: /api/ThongKe/GetThongKePhatBang
        /// </summary>
        ///  <param name="modelSearch"></param>
        /// <returns></returns>

        [HttpGet("GetThongKePhatBang")]
        [AllowAnonymous]
        public IActionResult GetThongKePhatBang([FromQuery] ThongKePhatBangSearchModel modelSearch)
        {
            int total;
          
            var user = _sysUserCL.GetByUsername(modelSearch.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            modelSearch.MaHeDaoTao = donVi.MaHeDaoTao;

            var data = _thongKeCL.GetThongKePhatBang(out total, modelSearch, donVi.Id);

            var outputData = new
            {
                truongs = data,
                totalRow = total,
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Thống kê học sinh đỗ tốt nghiệp theo năm thi hoặc hệ đào tạo, khóa thi, hình thức đào tạo
        /// API: /api/ThongKe/GetHocSinhDoTotNghiep
        /// </summary>
        ///  <param name="modelSearch"></param>
        /// <returns></returns>
        [HttpGet("GetHocSinhDoTotNghiep")]
        [AllowAnonymous]
        public IActionResult GetHocSinhDoTotNghiep([FromQuery] HocSinhTotNghiepSearchModel modelSearch)
        {
            int total;
            var user = _sysUserCL.GetByUsername(modelSearch.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            modelSearch.MaHeDaoTao = donVi.MaHeDaoTao;
            var data = _thongKeCL.GetHocSinhDoTotNghiep(out total, modelSearch);

            var outputData = new
            {
                hocSinh = data,
                totalRow = total,
            };
            return ResponseHelper.Ok(outputData);
        }
        #endregion

        #region TRƯỜNG
        /// <summary>
        /// Thống kê học sinh đỗ tốt nghiệp theo trường, danh mục tốt nghiệp hoặc năm thi
        /// API: /api/ThongKe/GetHocSinhDTNByTruongAndNamOrDMTN
        /// </summary>
        ///  <param name="modelSearch"></param>
        /// <returns></returns>

        [HttpGet("GetHocSinhDTNByTruongAndNamOrDMTN")]
        public IActionResult GetHocSinhDTNByTruongAndNamOrDMTN([FromQuery] HSByTruongNamOrDMTNSearchModel modelSearch)
        {
            int total;
            var data = _thongKeCL.GetHocSinhDTNByTruongAndNamOrDMTN(out total, modelSearch);

            var outputData = new
            {
                hocSinh = data,
                totalRow = total,
            };
            return ResponseHelper.Ok(outputData);
        }
        #endregion
    }
}
