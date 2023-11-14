using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Requests.BatchRequest;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.XacMinhVanBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChinhSuaVanBangController : BaseAppController
    {
        private readonly ShareResource _localizer;
        private HocSinhCL _cacheLayer;
        private TruongCL _truongCL;
        private SysUserCL _sysUserCL;
        private PhoiGocCL _phoiGocCL;


        private ChinhSuaVanBangCL _cacheChinhSuaVanBang;

        private readonly IMapper _mapper;
        private ILogger<ChinhSuaVanBangController> _logger;
        private readonly IFileService _fileService;
        public ChinhSuaVanBangController(ICacheService cacheService, IMapper mapper, IConfiguration configuration, IFileService fileService, ShareResource shareResource, ILogger<ChinhSuaVanBangController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _cacheChinhSuaVanBang = new ChinhSuaVanBangCL(cacheService, configuration);
            _mapper = mapper;
            _truongCL = new TruongCL(cacheService, configuration);
            _sysUserCL = new SysUserCL(cacheService);
            _phoiGocCL = new PhoiGocCL(cacheService, configuration);
        }


        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] ChinhSuaVanBangInputModel model)
        {

            #region Save File

            if (model.FileVanBan != null)
            {
                string folderName = "DonYeuCau/VanBanChinhSua";
                var fileResult = _fileService.SaveFile(model.FileVanBan, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileVanBan = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            #endregion
            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);

            var data = await _cacheChinhSuaVanBang.Create(model, donVi);
            if (data.MaLoi == (int)LichSuChinhSuaVanBangEnum.Fail)
                return ResponseHelper.BadRequest("Chỉnh sửa văn bằng thất bại");
            if (data.MaLoi == (int)LichSuChinhSuaVanBangEnum.NotEdit)
                return ResponseHelper.BadRequest("Không có thông tin nào chỉnh sửa");
            if (data.MaLoi == (int)LichSuChinhSuaVanBangEnum.NotExist)
                return ResponseHelper.BadRequest("Học sinh không tồn tại");
            if (data.MaLoi == (int)LichSuChinhSuaVanBangEnum.SoHieuExist)
                return ResponseHelper.BadRequest("Số hiệu đã tồn tại");
            if (data.MaLoi == (int)LichSuChinhSuaVanBangEnum.SoVaoSoExist)
                return ResponseHelper.BadRequest("Số vào sổ đã tồn tại");
            else
            {
                if (model.LoaiHanhDong == LoaiHanhDongEnum.CapLai)
                {
                    var updateSoVaoSo = new UpdateCauHinhSoVaoSoInputModel()
                    {
                        DinhDangSoThuTuSoGoc = 1,
                        Nam = data.Nam,
                        LoaiHanhDong = SoVaoSoEnum.SoVaoSoGoc,
                        IdTruong = donVi.Id
                    };
                    _truongCL.UpdateCauHinhSoVaoSo(updateSoVaoSo);
                    await _phoiGocCL.CapNhatThongSoPhoi("1", data.IdPhoi, 1);

                    return ResponseHelper.Success("Cấp lại văn bằng thành công");
                } 
                else
                {
                    return ResponseHelper.Success("Chỉnh sửa văn bằng thành công");

                }
            }
    
        }

        [HttpGet("GetSearchLichSuChinhSuaVanBang")]
        [AllowAnonymous]
        public IActionResult GetSearchLichSuChinhSuaVanBang(string cccd, [FromQuery] SearchParamModel model)
        {
            int total;

            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            HocSinhDTO hocSinhMp = _mapper.Map<HocSinhDTO>(hocSinh);
            // Tạo một bản sao của hocSinh để giữ nguyên giá trị cũ
            var phucLuc = _cacheChinhSuaVanBang.GetThongTinChinhSuaMoiNhat(hocSinh.Id);
            if (phucLuc != null)
            {
                hocSinhMp = _mapper.Map<HocSinhDTO>(phucLuc);
                hocSinhMp.HoiDong = phucLuc.HoiDongThi;
                hocSinhMp.DanhMucTotNghiep.NgayCapBang = phucLuc.NgayCap;
                hocSinhMp.DanhMucTotNghiep.IdNamThi = phucLuc.IdNamThi;
                hocSinhMp.MaHinhThucDaoTao = phucLuc.MaHTDT;
                hocSinhMp.Id = hocSinh.Id;
                hocSinhMp.SoHieuVanBang = string.IsNullOrEmpty(phucLuc.SoHieuVanBangCapLai) ? phucLuc.SoHieuVanBangCu : phucLuc.SoHieuVanBangCapLai;
                hocSinhMp.SoVaoSoCapBang = string.IsNullOrEmpty(phucLuc.SoVaoSoCapBangCapLai) ? phucLuc.SoVaoSoCapBangCu : phucLuc.SoVaoSoCapBangCapLai;
            }

            var data = _cacheChinhSuaVanBang.GetSearchLichSuChinhSuaVanBang(out total, hocSinhMp.Id, model);

            var outputData = new
            {
                LichSus = data,
                totalRow = total,
                HocSinh = hocSinhMp,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }


        [HttpGet("GetHocSinhByCccd")]
        [AllowAnonymous]
        public IActionResult GetHocSinhByCccd(string cccd)
        {
            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            HocSinhDTO hocSinhMp = _mapper.Map<HocSinhDTO>(hocSinh);
            // Tạo một bản sao của hocSinh để giữ nguyên giá trị cũ
            var phucLuc = _cacheChinhSuaVanBang.GetThongTinChinhSuaMoiNhat(hocSinh.Id);
            if (phucLuc != null)
            {
                hocSinhMp = _mapper.Map<HocSinhDTO>(phucLuc);
                hocSinhMp.HoiDong = phucLuc.HoiDongThi;
                hocSinhMp.DanhMucTotNghiep.NgayCapBang = phucLuc.NgayCap;
                hocSinhMp.DanhMucTotNghiep.IdNamThi = phucLuc.IdNamThi;
                hocSinhMp.MaHinhThucDaoTao = phucLuc.MaHTDT;
                hocSinhMp.Id = hocSinh.Id;
                hocSinhMp.SoHieuVanBang = string.IsNullOrEmpty(phucLuc.SoHieuVanBangCapLai) ? phucLuc.SoHieuVanBangCu : phucLuc.SoHieuVanBangCapLai;
                hocSinhMp.SoVaoSoCapBang = string.IsNullOrEmpty(phucLuc.SoVaoSoCapBangCapLai) ? phucLuc.SoVaoSoCapBangCu : phucLuc.SoVaoSoCapBangCapLai;
            }

            return ResponseHelper.Ok(hocSinhMp);
        }

        [HttpGet("GetChinhSuaVanBangById")]
        [AllowAnonymous]
        public IActionResult GetChinhSuaVanBangById(string cccd, string idPhuLuc)
        {
            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            HocSinhDTO hocSinhMp = _mapper.Map<HocSinhDTO>(hocSinh);
            var data = _cacheChinhSuaVanBang.GetChinhSuaVanBangById(idPhuLuc);

            var outputData = new
            {
                HocSinhs = hocSinhMp,
                LichSus = data,
            };
            return ResponseHelper.Ok(outputData);
        }


    }
}
