using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Caching.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Caching.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Caching.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Processor.UploadFile;
using CenIT.DegreeManagement.CoreAPI.Resources;
using CenIT.DegreeManagement.CoreAPI.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using static CenIT.DegreeManagement.CoreAPI.Core.Helpers.DataTableValidatorHelper;

namespace CenIT.DegreeManagement.CoreAPI.Controllers.XacMinhVanBang
{
    [Route("api/[controller]")]
    [ApiController]
    public class XacMinhVanBangController : BaseAppController
    {

        private readonly ShareResource _localizer;
        private HocSinhCL _cacheLayer;
        private XacMinhVanBangCL _cacheXacMinhVB;
        private TruongCL _truongCL;
        private DanTocCL _danTocCL;
        private MonThiCL _monThiCL;
        private HocSinhTmpCl _hocSinhTmpCl;
        private SysUserCL _sysUserCL;
        private readonly IMapper _mapper;
        private ILogger<XacMinhVanBangController> _logger;
        private readonly IFileService _fileService;

        public XacMinhVanBangController(ICacheService cacheService,IMapper mapper ,IConfiguration configuration, IFileService fileService, ShareResource shareResource, ILogger<XacMinhVanBangController> logger) : base(cacheService, configuration)
        {
            _cacheLayer = new HocSinhCL(cacheService, configuration);
            _logger = logger;
            _localizer = shareResource;
            _fileService = fileService;
            _cacheXacMinhVB = new XacMinhVanBangCL(cacheService, configuration);
            _truongCL = new TruongCL(cacheService, configuration);
            _danTocCL = new DanTocCL(cacheService, configuration);
            _monThiCL = new MonThiCL(cacheService, configuration);
            _mapper = mapper;
            _hocSinhTmpCl = new HocSinhTmpCl(cacheService, configuration);
            _sysUserCL = new SysUserCL(cacheService);

        }

        [HttpGet("GetSearchHocSinhXacMinhVanBang")]
        [AllowAnonymous]
        public IActionResult GetSearchHocSinhXacMinhVanBang([FromQuery] HocSinhSearchXacMinhVBModel model)
        {
            int total;
            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);

            var data = _cacheLayer.GetSearchHocSinhXacMinhVB(out total, model, donVi.Id);

            var outputData = new
            {
                HocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        /// <summary>
        /// Lấy thông tin học sinh theo cccd
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetByCccd")]
        [AllowAnonymous]
        public IActionResult GetByCccd(string cccd)
        {
            if (cccd == null) return ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()));
            var hocSinh = _cacheLayer.GetHocSinhByCccd(cccd);
            return hocSinh != null ? ResponseHelper.Ok(hocSinh)
                : ResponseHelper.NotFound(_localizer.GetNotExistMessage(NameControllerEnum.HocSinh.ToStringValue()), cccd);
        }


        #region Xác minh văn bằng

        /// <summary>
        /// Lấy thông tin học sinh theo cccd
        /// </summary>
        /// <returns></returns>
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromForm] XacMinhVangBangInputModel model)
        {

            if (model.FileYeuCau != null)
            {
                string folderName = "DonYeuCau";
                var fileResult = _fileService.SaveFile(model.FileYeuCau, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileYeuCau = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);

            var data = await _cacheXacMinhVB.Create(model, donVi);
            if (data == (int)XacMinhVanBangEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage("Xác minh thất bại"));
            return ResponseHelper.Success(_localizer.GetAddSuccessMessage("Xác minh thành công"));

        }

        [HttpPost("CreateList")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateList([FromForm] XacMinhVangBangListInputModel model)
        {
            
            if (model.FileYeuCau != null)
            {
                string folderName = "DonYeuCau";
                var fileResult = _fileService.SaveFile(model.FileYeuCau, folderName);
                if (fileResult.Item1 == 1)
                {
                    model.PathFileYeuCau = fileResult.Item2;
                }
                else
                {
                    return ResponseHelper.BadRequest(fileResult.Item2);
                }
            }

            var user = _sysUserCL.GetByUsername(model.NguoiThucHien);
            var donVi = _truongCL.GetById(user.TruongID);
            var data = await _cacheXacMinhVB.CreateList(model, donVi);

            if (data == (int)XacMinhVanBangEnum.Fail)
                return ResponseHelper.BadRequest(_localizer.GetAddErrorMessage("Thêm lịch sử xác minh thất bại"));
            return ResponseHelper.Success(_localizer.GetAddSuccessMessage("Thêm lịch sử xác minh thành công"));
        }

        [HttpGet("CauHinhXacMinhVB")]
        [AllowAnonymous]
        public IActionResult CauHinhXacMinhVB(string idDonVi)
        {
            
           var cauHinh = _truongCL.GetCauHinhXacMinhByIdDonVi(idDonVi);
            if(cauHinh == null) return ResponseHelper.NotFound("Không tìm thấy cấu hình");

            var model = new CauHinhXacMinhVanBangModel()
            {
                NguoiKyBang = cauHinh.NguoiKyBang,
                CoQuanCapBang = cauHinh.CoQuanCapBang,
                DiaPhuongCapBang = cauHinh.DiaPhuongCapBang,
                UyBanNhanDan = cauHinh.UyBanNhanDan
            };
            return ResponseHelper.Ok(model);
        }


        [HttpGet("GetLichSuPhatMinhVanbang")]
        [AllowAnonymous]
        public IActionResult GetLichSuPhatMinhVanbang([FromQuery]LichSuXacMinhVanBangSearchModel modelSearch)
        {
            var result = _cacheXacMinhVB.GetSearchLichSuXacMinh(modelSearch);

            return Ok(ResponseHelper.ResultJson(result));
        }


        [HttpGet("GetLichSuXacMinhById")]
        [AllowAnonymous]
        public IActionResult GetLichSuXacMinhById(string id)
        {
            if (id == null) return ResponseHelper.NotFound("Lần xác mình không tồn tại");
            var result = _cacheXacMinhVB.GetLichSuXacMinhById(id);

            return result != null ? ResponseHelper.Ok(result)
              : ResponseHelper.NotFound(_localizer.GetAddErrorMessage("Lần xác mình không tồn tại"));
        }

        #endregion

        #region Import

        /// <summary>
        /// Thêm danh sách học sinh từ excel
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost("ImportHocSinh")]
        [AllowAnonymous]
        public async Task<IActionResult> ImportHocSinh([FromForm] HocSinhImportModel model)
        {
            try
            {
                var deleted = await _hocSinhTmpCl.Delete(model.NguoiThucHien);

                //Kiểm tra định dạng file excel
                var checkFile = FileHelper.CheckValidFileExcel(model.FileExcel);
                if (checkFile.Item1 == (int)ErrorFileEnum.InvalidFileImage)
                {
                    return ResponseHelper.BadRequest(checkFile.Item2);
                }

                //Đọc file excel
                DataTable dataFromFile = ReadExcelData(model.FileExcel);

                var monThis = _monThiCL.GetAllMaMonThi();
                var danTocs = _danTocCL.GetAllTenDanToc();
                var hocSinhs = _cacheLayer.GetAllHocSinhDaCoSoHieu();
                string[] arrayCccd = hocSinhs.Select(x => x.CCCD).OrderByDescending(x => x).ToArray();
                string[] arraySoHieu = hocSinhs.Select(x => x.SoHieuVanBang).OrderByDescending(x => x).ToArray();
                string[] arraySoVaoSo = hocSinhs.Select(x => x.SoVaoSoCapBang).OrderByDescending(x => x).ToArray();

                var validationRules = HocSinhValidationRules.GetRuleHocSinhOld(monThis, danTocs, arrayCccd, arraySoHieu, arraySoVaoSo);
                //// Kiểm tra dữ liệu
                DataTable checkValue = CheckValidateDataTable(dataFromFile, validationRules);

                //Map từ datatable sang List<HocSinhImportViewModel>
                List<HocSinhImportVM> hocSinhImports = ModelProvider.CreateListFromTable<HocSinhImportVM>(checkValue);
                hocSinhImports.ForEach(hocSinh =>
                {
                    hocSinh.IdTruong = model.IdTruong;
                    hocSinh.IdDanhMucTotNghiep = model.IdDanhMucTotNghiep;
                    hocSinh.NgayTao = DateTime.Now;
                    hocSinh.NguoiTao = model.NguoiThucHien;
                    hocSinh.IdKhoaThi = model.IdKhoaThi;
                    hocSinh.TrangThai = TrangThaiHocSinhEnum.DaNhanBang;
                    hocSinh.KetQuaHocTaps = new List<KetQuaHocTapModel>
                    {
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon1, Diem = hocSinh.DiemMon1 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon2, Diem = hocSinh.DiemMon2 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon3, Diem = hocSinh.DiemMon3 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon4, Diem = hocSinh.DiemMon4 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon5, Diem = hocSinh.DiemMon5 ?? 0 },
                        new KetQuaHocTapModel { MaMon = hocSinh.Mon6, Diem = hocSinh.DiemMon6 ?? 0 }
                    };
                });

                List<HocSinhTmpModel> hocSinhTmp = _mapper.Map<List<HocSinhTmpModel>>(hocSinhImports);

                var response = await _hocSinhTmpCl.SaveImport(hocSinhTmp);
                if(response == (int)HocSinhEnum.Fail)
                    return ResponseHelper.BadRequest(_localizer.GetImportErrorMessage(NameControllerEnum.HocSinh.ToStringValue()));
                return ResponseHelper.Success(_localizer.GetImportSuccessMessage(NameControllerEnum.HocSinh.ToStringValue()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ResponseHelper.Error500(ex.Message);
            }
        }

        [HttpGet("GetSearchHocSinhTmp")]
        [AllowAnonymous]
        public IActionResult GetSearchHocSinhTmp([FromQuery] HocSinhTmpModelParamModel model)
        {
            int total;

            var data = _hocSinhTmpCl.GetSearchHocSinhTmp(out total, model);

            var outputData = new
            {
                HocSinhs = data,
                totalRow = total,
                searchParam = model
            };
            return ResponseHelper.Ok(outputData);
        }

        [HttpGet("GetThongKeHocSinhTmp")]
        [AllowAnonymous]
        public IActionResult GetThongKeHocSinhTmp(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {

            var data = _hocSinhTmpCl.GetThongKeHocSinhTmp(idTruong, nguoiThucHien, idDanhMucTotNghiep);
            return ResponseHelper.Ok(data);
        }

        [HttpPost("DeleteImport")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteImport(string nguoiThucHien)
        {

            var response = await _hocSinhTmpCl.Delete(nguoiThucHien);
            if (response == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("Xóa Thất BẠI");
            return ResponseHelper.Success("Xóa thành công");
        }


        [HttpPost("SaveImport")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveImport(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {

            var hocSinhs =  _hocSinhTmpCl.GetAllHocSinhTmpSave(idTruong, nguoiThucHien, idDanhMucTotNghiep);
            if(hocSinhs.Where(x=>x.ErrorCode == -1).Count() > 0) return ResponseHelper.BadRequest("Cố các hàng bị lỗi");

            List<HocSinhModel> hocSinhTmp = _mapper.Map<List<HocSinhModel>>(hocSinhs);

            var response = await _cacheLayer.SaveImport(idTruong, idDanhMucTotNghiep, hocSinhTmp);

            if (response.MaLoi == (int)HocSinhEnum.Fail)
                return ResponseHelper.BadRequest("Thêm danh sách học sinh thất bại");
            if (response.MaLoi == (int)HocSinhEnum.NotExistPhoi)
                return ResponseHelper.BadRequest("Phôi gốc không tồn tại");
            var deleted = await _hocSinhTmpCl.Delete(nguoiThucHien);
            return ResponseHelper.Success("Thêm danh sách học sinh thành công");
        }

        #endregion

        private DataTable ReadExcelData(IFormFile file)
        {
            var dataTable = new DataTable();

            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Lấy sheet đầu tiên

                // Tạo cột cho DataTable từ dòng đầu tiên của sheet (tên cột)
                IRow headerRow = sheet.GetRow(0);
                foreach (var cell in headerRow.Cells)
                {
                    dataTable.Columns.Add(cell.ToString());
                }

                // Lặp qua các hàng trong sheet từ dòng thứ 2 (dữ liệu)
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    // Tạo một hàng mới trong DataTable
                    DataRow dataRow = dataTable.NewRow();

                    // Lặp qua các ô trong hàng và đổ dữ liệu vào DataRow
                    for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                    {
                        ICell cell = row.GetCell(cellIndex);
                        dataRow[cellIndex] = cell?.ToString() ?? string.Empty;
                    }

                    // Thêm DataRow vào DataTable
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

    }
}
