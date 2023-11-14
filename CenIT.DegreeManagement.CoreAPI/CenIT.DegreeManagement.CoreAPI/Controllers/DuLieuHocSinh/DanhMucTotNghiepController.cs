using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.DuLieuHocSinh
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucTotNghiepController : BaseAppController
    {
        private DanhMucTotNghiepCL _cacheLayer;
        private NamThiCL _namThiCL;
        private TruongCL _truongCL;


        private readonly IMapper _mapper;
        private ILogger<DanhMucTotNghiepController> _logger;
        private readonly ShareResource _localizer;
        private readonly IFileService _fileService;

        private readonly string _nameController = "Danh mục tốt nghiệp";

        public DanhMucTotNghiepController(ICacheService cacheService, IConfiguration configuration, ShareResource shareResource, ILogger<DanhMucTotNghiepController> logger, IFileService fileService, IMapper imapper) : base(cacheService, configuration)
        {
            _cacheLayer = new DanhMucTotNghiepCL(cacheService, configuration);
            _namThiCL = new NamThiCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _mapper = imapper;
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
        }


        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _cacheLayer.GetAll();
            var tongSoTruong = _truongCL.GetAll().Count();
            data.ForEach(danhMucTotNghep =>
            {
                danhMucTotNghep.TongSoTruong = tongSoTruong;
            });
            return ResponseHelper.Ok(data);
        }

        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp theo search param
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSearch")]
        [AllowAnonymous]
        public IActionResult GetSearch([FromQuery] DanhMucTotNghiepSearchParam model)
        {
            int total;
            var data = _cacheLayer.GetSearch(out total, model);
            var namThi = _namThiCL.GetAll();
            var tongSoTruong = _truongCL.GetAll().Count();
            if(namThi != null)
            {
                data.ForEach(danhMucTotNghep =>
                {
                    danhMucTotNghep.TongSoTruong = tongSoTruong;
                    danhMucTotNghep.NamThi = namThi.FirstOrDefault(d => d.Id == danhMucTotNghep.IdNamThi).Ten;
                });
            }
          
            var outputData = new
            {
                DanhMucTotNghieps = data,
                totalRow = total,
                searchParam = model,
            };
            return ResponseHelper.Ok(outputData);
        }


        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp chưa khóa
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUnBlock")]
        public IActionResult GetAllUnBlock()
        {
            var data = _cacheLayer.GetAllUnBlock();
            var tongSoTruong = _truongCL.GetAll().Count();
            data.ForEach(danhMucTotNghep =>
            {
                danhMucTotNghep.TongSoTruong = tongSoTruong;
            });
            return ResponseHelper.Ok(data);
        }


        /// <summary>
        /// Thêm danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] DanhMucTotNghiepInputModel model)
        {
            var response = await _cacheLayer.Create(model);
            if (response == (int)EnumDanhMucTotNghiep.Fail) 
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage(_nameController), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.YearNotMatchDate)
                return ResponseHelper.BadRequest("Năm thi không khớp với ngày cấp bằng");
            if (response == (int)EnumDanhMucTotNghiep.ExistName) 
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(_nameController, EnumExtensions.ToStringValue(DanhMucTotNghiepInfoEnum.TieuDe)), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.ExistYearAndHTDT)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage(_nameController, EnumExtensions.ToStringValue(DanhMucTotNghiepInfoEnum.IdNamVaIdHTDT)), model.IdNamThi + "," + model.IdHinhThucDaoTao);
            if (response == (int)EnumDanhMucTotNghiep.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("HTDT "));
            if (response == (int)EnumDanhMucTotNghiep.NotExistNamThi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("NamThi "));
            else 
                return ResponseHelper.Success(_localizer.GetAddSuccessMessage(_nameController), model.TieuDe);
        }

        /// <summary>
        /// Cập nhật danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromForm] DanhMucTotNghiepInputModel model)
        {
            var response = await _cacheLayer.Modify(model);
            if (response == (int)EnumDanhMucTotNghiep.Fail) 
                return ResponseHelper.BadRequest(_localizer.GetUpdateErrorMessage(_nameController), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.ExistName) 
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage("Tiêu đề " + _nameController), model.TieuDe);
            if (response == (int)EnumDanhMucTotNghiep.YearNotMatchDate)
                return ResponseHelper.BadRequest("Năm thi không khớp với ngày cấp bằng");
            if (response == (int)EnumDanhMucTotNghiep.NotFound) 
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            if (response == (int)EnumDanhMucTotNghiep.Locked) 
                return ResponseHelper.BadRequest("Đã khóa");
            if (response == (int)EnumDanhMucTotNghiep.Printed) 
                return ResponseHelper.BadRequest("Đã In bằng");
            if (response == (int)EnumDanhMucTotNghiep.ExistYearAndHTDT)
                return ResponseHelper.BadRequest(_localizer.GetAlreadyExistMessage("Năm học " + _nameController));
            if (response == (int)EnumDanhMucTotNghiep.NotExistHTDT)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("HTDT "));
            if (response == (int)EnumDanhMucTotNghiep.NotExistNamThi)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage("NamThi "));
            else 
                return ResponseHelper.Success(_localizer.GetUpdateSuccessMessage(_nameController), model.TieuDe);
        }

        /// <summary>
        /// Xóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhmucTotNghiep"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string idDanhmucTotNghiep, string nguoiThucHien)
        {
            var response = await _cacheLayer.Delete(idDanhmucTotNghiep, nguoiThucHien);
            if (response == (int)EnumDanhMucTotNghiep.Fail)
                return ResponseHelper.BadRequest(_localizer.GetDeleteErrorMessage(_nameController));
            if (response == (int)EnumDanhMucTotNghiep.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));
            if (response == (int)EnumDanhMucTotNghiep.Locked)
                return ResponseHelper.BadRequest("Đã khóa");
            if (response == (int)EnumDanhMucTotNghiep.Printed)
                return ResponseHelper.BadRequest("Đã In bằng");
            else 
                return ResponseHelper.Success(_localizer.GetDeleteSuccessMessage(_nameController));
        }

        /// <summary>
        /// Lấy danh mục tốt nghiệp theo idDanhMucTotNghiep
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpGet("GetById/{idDanhMucTotNghiep}")]
        [AllowAnonymous]
        public IActionResult GetById(string idDanhMucTotNghiep)
        {
            var data = _cacheLayer.GetById(idDanhMucTotNghiep);
            var tongSoTruong = _truongCL.GetAll(data.IdHinhThucDaoTao).Count();

            if (data == null)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));

            data.TongSoTruong = tongSoTruong;

            return ResponseHelper.Ok(data);
        }

        ///// <summary>
        ///// Lấy danh mục tốt nghiệp theo idNamThi và idHinhThucDaoTao
        ///// </summary>
        ///// <param name="idDanhMucTotNghiep"></param>
        ///// <returns></returns>
        //[HttpGet("GetByIdNamThi/{idNamThi}/{maHinhThucDaoTao}")]
        //public IActionResult GetByIdNamThiAndMaHinhThucDaoTao(string idNamThi, string maHinhThucDaoTao)
        //{
        //    var danhMucTotNghieps = _cacheLayer.GetByIdNamThiAndMaHinhThucDaoTao(idNamThi, maHinhThucDaoTao);
        //    var tongSoTruong = _truongCL.GetAll().Count();

        //    danhMucTotNghieps.ForEach(danhMucTotNghiep =>
        //    {
        //        danhMucTotNghiep.TongSoTruong = tongSoTruong;

        //    });
        //    return ResponseHelper.Ok(danhMucTotNghieps);
        //}

        /// <summary>
        /// Khóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpPost("Lock")]
        public async Task<IActionResult> Lock(string idDanhMucTotNghiep, string nguoiThucHien)
        {
            var response = await _cacheLayer.KhoaDanhMucTotNghiep(idDanhMucTotNghiep, nguoiThucHien, true);
            if(response == (int)EnumDanhMucTotNghiep.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));

            return ResponseHelper.Success(string.Format("Khóa [{0}] thành công", _nameController));
        }

        /// <summary>
        /// Mở khóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        [HttpPost("UnLock")]
        public async Task<IActionResult> UnLock(string idDanhMucTotNghiep, string nguoiThucHien)
        {
            var response = await _cacheLayer.KhoaDanhMucTotNghiep(idDanhMucTotNghiep, nguoiThucHien, false);
            if (response == (int)EnumDanhMucTotNghiep.NotFound)
                return ResponseHelper.NotFound(_localizer.GetNotExistMessage(_nameController));

            return ResponseHelper.Success(string.Format("Mở khóa [{0}] thành công", _nameController));
        }

    }
}