using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Phoi;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Models.Shared;
using CenIT.DegreeManagement.CoreAPI.Models.Sys.Truong;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.Shared
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedController : BaseAppController
    {
        private SysConfigCL _sysConfigCL;
        private TruongCL _truongCL;
        private SysUserCL _sysUserCL;
        private HeDaoTaoCL _heDaoTaoCL;
        private HinhThucDaoTaoCL _htdtCl;
        private DanhMucTotNghiepCL _danhMucTotNghiepCL;
        private HocSinhCL _hocSinhCl;
        private DanTocCL _danTocCL;
        private NamThiCL _namThiCL;
        private PhoiGocCL _phoiGocCL;
        private SysFunctionCL _sysFunctionCL;



        private ILogger<SharedController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        public SharedController(ICacheService cacheService, IConfiguration configuration, IMapper imapper, ShareResource shareResource, ILogger<SharedController> logger, IFileService fileService) : base(cacheService, configuration)
        {
            _sysConfigCL = new SysConfigCL(cacheService);
            _sysUserCL = new SysUserCL(cacheService);
            _sysFunctionCL = new SysFunctionCL(cacheService);
            _danhMucTotNghiepCL = new DanhMucTotNghiepCL(cacheService , configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _heDaoTaoCL = new HeDaoTaoCL(cacheService, configuration);
            _htdtCl = new HinhThucDaoTaoCL(cacheService, configuration);
            _hocSinhCl = new HocSinhCL(cacheService, configuration);
            _danTocCL = new DanTocCL(cacheService, configuration);
            _namThiCL = new NamThiCL(cacheService, configuration);
            _phoiGocCL = new PhoiGocCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _mapper = imapper;
        }

        /// <summary>
        /// Thêm phôi gốc
        /// API: /api/PhoiGoc/Create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("GetLoaiDonVi")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoaiDonVi()
        {
            var loaiDonVi = System.Enum.GetValues(typeof(TypeUnitEnum))
                    .Cast<TypeUnitEnum>()
                    .Where(t => (int)t > 0)
                    .Select(t => new LoaiDonViModel
                    {
                        GiaTri = ((int)t),
                        TenLoai = _localizer.MessageName(t.ToStringValue()),
                    }).ToList();

            return ResponseHelper.Ok(loaiDonVi);
        }

        [HttpGet("GetAllTruong")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTruong(string username)
        {
            var user = _sysUserCL.GetByUsername(username);
            var truongs = _truongCL.GetAllTruong(user.TruongID);

            return ResponseHelper.Ok(truongs);
        }

        [HttpGet("GetAllDonViCha")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllDonViCha()
        {

            var truongs = _truongCL.GetAllDonViCha();

            return ResponseHelper.Ok(truongs);
        }

        [HttpGet("GetCauHinhByUsername")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCauHinhByUsername(string username)
        {
            var user = _sysUserCL.GetByUsername(username);
            var truongs = _truongCL.GetCauHinhByIdTruong(user.TruongID);

            return ResponseHelper.Ok(truongs);
        }

        [HttpGet("GetAllDonViByUsername")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllDonViByUsername(string username)
        {
            var user = _sysUserCL.GetByUsername(username);
            var donVis = _truongCL.GetAllDonViByUsername(user.TruongID);

            List<TruongDTO> donViDTOs = _mapper.Map<List<TruongDTO>>(donVis);

            return ResponseHelper.Ok(donViDTOs);
        }


        // Hệ đào tạo
        /// <summary>
        /// Lay tat ca he dao tao
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllHeDaoTao")]
        [AllowAnonymous]
        public IActionResult GetAllHeDaoTao()
        {
            var data = _heDaoTaoCL.GetAll();
            return ResponseHelper.Ok(data);
        }

        //Hình Thức đào tạo
        [HttpGet("GetAllHinhThucDaoTao")]
        [AllowAnonymous]
        public IActionResult GetAllHinhThucDaoTao()
        {
            var data = _htdtCl.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllDanhMucTotNghiep")]
        [AllowAnonymous]
        public IActionResult GetAllDanhMucTotNghiep(string username)
        {
            var user = _sysUserCL.GetByUsername(username);
            var donVi = _truongCL.GetById(user.TruongID);
            string maHeDaotao = "";
            if(donVi != null) { maHeDaotao = donVi.MaHeDaoTao; }

            var listDMTN = _danhMucTotNghiepCL.GetAllByHeDaoTao(maHeDaotao);

            return ResponseHelper.Ok(listDMTN);
        }

        [HttpGet("GetByCccd")]
        [AllowAnonymous]
        public IActionResult GetByCccd(string cccd)
        {
            if (cccd == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()));
            var data = _hocSinhCl.GetHocSinhByCccd(cccd);

            HocSinhDTO hocSinh = _mapper.Map<HocSinhDTO>(data);

            return hocSinh != null ? ResponseHelper.Ok(hocSinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()), cccd);
        }

        [HttpGet("GetAllDanToc")]
        [AllowAnonymous]
        public IActionResult GetAllDanToc()
        {
            var data = _danTocCL.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllNamThi")]
        [AllowAnonymous]
        public IActionResult GetAllNamThi()
        {
            var data = _namThiCL.GetAll();
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetAllKhoaThiByNamThi")]
        [AllowAnonymous]
        public IActionResult GetAllKhoaThiByNamThi(string idNamThi)
        {
            var data = _namThiCL.GetKhoaThisByNam(idNamThi);
            return ResponseHelper.Ok(data);
        }

        [HttpGet("GetPhoiGocDangSuDung")]
        [AllowAnonymous]
        public IActionResult GetPhoiGocDangSuDung(string username)
        {
            var user = _sysUserCL.GetByUsername(username);
            var donVi = _truongCL.GetById(user.TruongID);

            var data = _phoiGocCL.GetPhoiDangSuDungByHDT(donVi.MaHeDaoTao);
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy danh mục tốt nghiệp theo idNamThi và idHinhThucDaoTao
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetByIdNamThi/{idNamThi}/{maHinhThucDaoTao}")]
        [AllowAnonymous]
        public IActionResult GetByIdNamThiAndMaHinhThucDaoTao(string idNamThi, string maHinhThucDaoTao, string nguoiThucHien)
        {
            var user = _sysUserCL.GetByUsername(nguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            var danhMucTotNghieps = _danhMucTotNghiepCL.GetByIdNamThiAndMaHinhThucDaoTao(idNamThi, maHinhThucDaoTao, donVi);
       
            return ResponseHelper.Ok(danhMucTotNghieps);
        }

        /// <summary>
        /// Lấy đanh sách chức năng
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllFunction")]
        [AllowAnonymous]
        public IActionResult GetAllFunction()
        {
            var searchModel = new SearchParamModel() { PageSize = -1 };
            var function = _sysFunctionCL.GetAll(searchModel);
       
            return ResponseHelper.Ok(function);
        }

    }

  
}
