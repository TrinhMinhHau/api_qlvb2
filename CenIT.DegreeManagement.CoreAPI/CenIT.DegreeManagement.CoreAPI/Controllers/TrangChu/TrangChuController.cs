using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.TrangChu
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrangChuController : BaseAppController
    {
        private HocSinhCL _hocSinhCL;
        private ILogger<TrangChuController> _logger;
        private readonly ShareResource _localizer;
        private TrangChuCL _trangChuCL;
        private SysUserCL _sysUserCL;
        private TruongCL _truongCL;
        private readonly IMapper _mapper;


        private MessageCL _messageCL;

        public TrangChuController(ICacheService cacheService, IConfiguration configuration, IMapper mapper,ShareResource shareResource, ILogger<TrangChuController> logger) : base(cacheService, configuration)
        {
            _hocSinhCL = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _trangChuCL = new TrangChuCL(cacheService, configuration);
            _messageCL = new MessageCL(cacheService);
            _sysUserCL = new SysUserCL(cacheService);
            _truongCL = new TruongCL(cacheService, configuration);
            _mapper = mapper;
        }
        #region MESSAGE
        /// <summary>
        /// Lấy tất cả thông báo mới nhất
        /// API: /api/TrangChu/GetTop10Message
        /// </summary>
        ///  <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetAllMessage")]
        public IActionResult GetAllMessage(int userId)
        {
            var data = _messageCL.GetAll(userId);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy 10 thông báo mới nhất
        /// API: /api/TrangChu/GetTop10Message
        /// </summary>
        ///  <param name="userId"></param>
        ///  <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetTop10Message")]
        [AllowAnonymous]
        public IActionResult GetTop10Message(int userId, int pageSize)
        {
            var data = _messageCL.GetByPageSize(userId, pageSize);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Đánh dấu tất cả thông báo đã đọc
        /// API: /api/TrangChu/UpdateAllReadStatus
        /// </summary>
        ///  <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("UpdateAllReadStatus")]
        public IActionResult UpdateAllReadStatus(int userId)
        {
            var response = _messageCL.UpdateAllReadStatus(userId);
            if (response == -1)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage("Message"));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage("Message"));
        }

        /// <summary>
        /// Lấy số lượng thông báo chưa đọc theo userid
        /// API: /api/TrangChu/GetUnreadMessagesCount
        /// </summary>
        ///  <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("GetUnreadMessagesCount")]
        public IActionResult GetUnreadMessagesCount(int? userId)
        {
            int messages = _messageCL.GetUnreadMessagesCount(userId);
            return ResponseHelper.Ok(messages);
        }

        /// <summary>
        /// Đanh dấu đã đọc thông báo
        /// API: /api/TrangChu/UpdateReadStatus
        /// </summary>
        /// <param name="idMessage"></param>
        /// <returns></returns>
        [HttpPost("UpdateReadStatus")]
        public IActionResult UpdateReadStatus(string idMessage)
        {
            var response = _messageCL.UpdateReadStatus(idMessage);
            if (response == -1)
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage("Message"));
            else
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage("Message"));
        }

        #endregion

        #region TRA CỨU HỌC SINH


        /// <summary>
        /// Lấy thông tin học sinh theo cccd
        /// API: /api/TrangChu/GetByCCCD
        /// </summary>
        /// <param name="cccd"></param>
        /// <returns></returns>
        [HttpGet("GetByCCCD")]
        public IActionResult GetByCCCD(string cccd)
        {
            var data = _hocSinhCL.GetHocSinhByCccd(cccd);

            return ResponseHelper.Ok(data);
        }
        #endregion

        #region THỐNG KÊ
        /// <summary>
        /// Thống kê số lượng số lượng học sinh qua từng năm
        /// API: /api/TrangChu/GetSoLuongHocSinhQuaTungNam
        /// </summary>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        [HttpGet("GetSoLuongHocSinhQuaTungNam")]
        public IActionResult GetSoLuongHocSinhQuaTungNam(string nguoiThucHien)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            if (donVi == null) return ResponseHelper.BadRequest("Không tìm thấy người thực hiện");
            var data = _trangChuCL.GetSoLuongHocSinhQuaTungNam(donVi.MaHeDaoTao, donVi.Id);

            return Ok(ResponseHelper.ResultJson(data));
        }

        /// <summary>
        /// Thống kê số lượng số lượng học sinh theo xếp loại
        /// API: /api/TrangChu/GetSoLuongHocSinhTheoXepLoai
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        [HttpGet("GetSoLuongHocSinhTheoXepLoai")]
        public IActionResult GetSoLuongHocSinhTheoXepLoai(string idNamThi, string nguoiThucHien)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            if (donVi == null) return ResponseHelper.BadRequest("Không tìm thấy người thực hiện");
            var data = _trangChuCL.GetSoLuongHocSinhTheoXepLoai(idNamThi, donVi.MaHeDaoTao, donVi.Id);

            return Ok(ResponseHelper.ResultJson(data));
        }

        /// <summary>
        /// Thống kê số lượng phát bằng
        /// API: /api/TrangChu/GetSoLuongHocSinhCapPhatBang
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        [HttpGet("GetSoLuongHocSinhCapPhatBang")]
        public IActionResult GetSoLuongHocSinhCapPhatBang(string idNamThi, string nguoiThucHien)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            if (donVi == null) return ResponseHelper.BadRequest("Không tìm thấy người thực hiện");
            var data = _trangChuCL.GetSoLuongHocSinhCapPhatBang(idNamThi, donVi.MaHeDaoTao, donVi.Id);

            return Ok(ResponseHelper.ResultJson(data));
        }

        /// <summary>
        /// Thống kê tổng quát theo phòng
        /// API: /api/TrangChu/GetThongKeTongQuatByPhong
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        [HttpGet("GetThongKeTongQuatByPhong")]
        public IActionResult GetThongKeTongQuatByPhong(string idNamThi, string nguoiThucHien)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            if (donVi == null) return ResponseHelper.BadRequest("Không tìm thấy người thực hiện");

            var data = _trangChuCL.GetThongKeTongQuatByPhong(idNamThi, donVi.MaHeDaoTao, donVi.Id);

            return Ok(ResponseHelper.ResultJson(data));
        }

        /// <summary>
        /// Thống kê tổng quát theo trường 
        /// API: /api/TrangChu/GetThongKeTongQuatByTruong
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idNamThi"></param>
        /// <returns></returns>
        [HttpGet("GetThongKeTongQuatByTruong")]
        public IActionResult GetThongKeTongQuatByTruong(string idTruong, string idNamThi)
        {
            var data = _trangChuCL.GetThongKeTongQuatByTruong(idTruong, idNamThi);

            return Ok(ResponseHelper.ResultJson(data));
        }

        [HttpGet("GetTraCuuHocSinhTotNghiep")]
        [AllowAnonymous]
        public IActionResult GetTraCuuHocSinhTotNghiep([FromQuery] TraCuuHocHinhTotNghiepSearchModel modelSearch)
        {
            int total;
            var user = _sysUserCL.GetByUsername(modelSearch.NguoiThucHien);
            var data = _trangChuCL.GetTraCuuHocSinhDaDuaVaoSo(out total, user.TruongID, modelSearch);
            List<TraCuuHocSinhDTO> traCuuHocSinhDTOs = _mapper.Map<List<TraCuuHocSinhDTO>>(data);
            var outputData = new
            {
                HocSinhs = traCuuHocSinhDTOs,
                totalRow = total,
                searchParam = modelSearch
            };
            return ResponseHelper.Ok(outputData);
        }


        ///// <summary>
        ///// Tra cứu học sinh tốt nghiệp
        ///// API: /api/TrangChu/GetTraCuuHocSinhTotNghiep
        ///// </summary>
        ///// <param name="modelSearch"></param>
        ///// <returns></returns>
        //[HttpGet("GetTraCuuHocSinhTotNghiep")]
        //[AllowAnonymous]
        //public IActionResult GetTraCuuHocSinhTotNghiep([FromQuery] TraCuuHocHinhTotNghiepSearchModel modelSearch)
        //{
        //    var user = _sysUserCL.GetByUsername(modelSearch.NguoiThucHien);
        //    var data = _trangChuCL.GetTraCuuHocSinhTotNghiep(user.TruongID, modelSearch);

        //    return Ok(ResponseHelper.ResultJson(data));
        //}


        #endregion
    }
}
