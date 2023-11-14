using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang
{
    public class XacMinhVanBangBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public XacMinhVanBangBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> Create(XacMinhVangBangInputModel model, TruongModel donVi)
        {

            var trangThaiValues = new List<TrangThaiHocSinhEnum> {
                            TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                            TrangThaiHocSinhEnum.DaCapBang,
                            TrangThaiHocSinhEnum.DaInBang,
                             TrangThaiHocSinhEnum.DaNhanBang, };

            var hocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                                           .Find(h => h.Xoa == false && h.Id == model.IdHocSinh && trangThaiValues.Contains(h.TrangThai)).FirstOrDefault();
            if (hocSinh == null) return (int)XacMinhVanBangEnum.HocSinhNotExist;
         
            var xacMinhVanBangModel = new XacMinhVanBangModel()
            {
                IdHocSinh = hocSinh.Id,
                NguoiKyBang = donVi.CauHinh.HoTenNguoiKySoGoc,
                CoQuanCapBang = donVi.CauHinh.TenCoQuanCapBang,
                DiaPhuongCapBang = donVi.CauHinh.TenDiaPhuongCapBang,
                UyBanNhanDan = donVi.CauHinh.TenUyBanNhanDan,
                DonViYeuCauXacMinh = model.DonViYeuCauXacMinh,
                CongVanSo = model.CongVanSo,
                NgayTrenCongVan = model.NgayTrenCongVan,
            };

            try
            {
                var xacMinhVanBang = new LichSuXacMinhVanBangModel();
                ModelProvider.MapProperties(xacMinhVanBangModel, xacMinhVanBang);
                xacMinhVanBang.NgayTao = DateTime.Now;
                xacMinhVanBang.NguoiTao = model.NguoiThucHien;
                xacMinhVanBang.PathFileYeuCau = model.PathFileYeuCau;
                xacMinhVanBang.IdHocSinhs = new List<string> { hocSinh.Id };

                await _mongoDatabase.GetCollection<LichSuXacMinhVanBangModel>(_collectionLichSuXacMinhVanBangName).InsertOneAsync(xacMinhVanBang);
                if (xacMinhVanBang.Id != null)
                {
                    return (int)XacMinhVanBangEnum.Success;

                }
                return (int)XacMinhVanBangEnum.Fail;
            }
            catch
            {
                return (int)XacMinhVanBangEnum.Fail;
            }
        }

        public async Task<int> CreateList(XacMinhVangBangListInputModel model, TruongModel donVi)
        {
            var xacMinhVangBangListModel = new XacMinhVanBangListModel()
            {
                NguoiKyBang = donVi.CauHinh.HoTenNguoiKySoGoc,
                CoQuanCapBang = donVi.CauHinh.TenCoQuanCapBang,
                DiaPhuongCapBang = donVi.CauHinh.TenDiaPhuongCapBang,
                UyBanNhanDan = donVi.CauHinh.TenUyBanNhanDan,
                DonViYeuCauXacMinh = model.DonViYeuCauXacMinh,
                CongVanSo = model.CongVanSo,
                NgayTrenCongVan = model.NgayTrenCongVan
            };

            try
            {
                var lichSuXacMinh = new LichSuXacMinhVanBangModel();
                ModelProvider.MapProperties(xacMinhVangBangListModel, lichSuXacMinh);
                lichSuXacMinh.NgayTao = DateTime.Now;
                lichSuXacMinh.NguoiTao = model.NguoiThucHien;
                lichSuXacMinh.PathFileYeuCau = model.PathFileYeuCau;
                lichSuXacMinh.IdHocSinhs = model.IdHocSinhs;

                await _mongoDatabase.GetCollection<LichSuXacMinhVanBangModel>(_collectionLichSuXacMinhVanBangName).InsertOneAsync(lichSuXacMinh);
                if (lichSuXacMinh.Id != null)
                {
                    return (int)XacMinhVanBangEnum.Success;

                }
                return (int)XacMinhVanBangEnum.Fail;
            }
            catch
            {
                return (int)XacMinhVanBangEnum.Fail;
            }
        }


        public string GetSearchLichSuXacMinh(LichSuXacMinhVanBangSearchModel modelSearch)
        {
            var trangThaiValues = new List<TrangThaiHocSinhEnum> {
                            TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                            TrangThaiHocSinhEnum.DaCapBang,
                            TrangThaiHocSinhEnum.DaInBang,
                             TrangThaiHocSinhEnum.DaNhanBang, };
            var hocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                                   .Find(h => h.Xoa == false && h.Id == modelSearch.IdHocSinh && trangThaiValues.Contains(h.TrangThai)).FirstOrDefault();


            if (hocSinh == null)
            {
                return null;
            }

            //Phân trang
            int skip = ((modelSearch.StartIndex - 1) * modelSearch.PageSize) + modelSearch.PageSize;
            string pagination = modelSearch.PageSize < 0 ? $@"lichSuXacMinhVanBangs: '$LichSuXacMinhVanBang'" : $@"lichSuXacMinhVanBangs: {{ $slice: ['$LichSuXacMinhVanBang', {skip}, {modelSearch.PageSize}] }},";
            // Sắp xếp
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "DonViYeuCauXacMinh");

            string matchIdHocSinh = string.IsNullOrEmpty(modelSearch.IdHocSinh) ? "" : $@"'IdHocSinhs': '{modelSearch.IdHocSinh}',";


            string fromDateToDate = MongoPipeline.GenerateFilterFromDateToDatePipeline(modelSearch.TuNgay, modelSearch.DenNgay, "NgayTao");


            var cmdRes = $@"
                            {{
                                'aggregate': 'LichSuXacMinhVanBang', 
                                'allowDiskUse': true,
                                'pipeline':[
                                      {{
                                        $match: {{
                                          Xoa: false,
                                            {fromDateToDate}
                                            {matchIdHocSinh}
                        
                                        }},
                                      }},
                                        {order}
                                      {{
                                        $group: {{
                                          _id: null,
                                          totalRow: {{ $sum: 1 }},
                                          LichSuXacMinhVanBang: {{
                                            $push: {{
                                              id: '$_id',
                                              idHocSinhs: '$IdHocSinhs',
                                              ma: '$Ma',
                                              donViYeuCauXacMinh: '$DonViYeuCauXacMinh',
                                              congVanSo: '$CongVanSo',
                                              ngayTrenCongVan: '$NgayTrenCongVan',
                                              pathFileYeuCau: '$PathFileYeuCau',
                                              ngayTao: '$NgayTao',
                                              nguoiTao: '$NguoiTao',
                                            }},
                                          }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          _id: 0,
                                          totalRow: 1,
                                          {pagination}
                                        }},
                                      }},
                                  ],
                                'cursor': {{ 'batchSize': 25 }},
                            }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);
            var hoTenJson = JsonConvert.SerializeObject(hocSinh.HoTen);

            if (json == null)
            {
                return null;
            }

            string finalJson = $"{{\"HoTen\": {hoTenJson}, \"XacMinhVanBang\": {json}}}";

            return finalJson;
        }


        public XacMinhVanBangListModel GetLichSuXacMinhById(string id)
        {

            var lichSuPhatMinh = _mongoDatabase.GetCollection<LichSuXacMinhVanBangModel>(_collectionLichSuXacMinhVanBangName)
                                  .Find(h => h.Xoa == false && h.Id == id).FirstOrDefault();

            if (lichSuPhatMinh == null) return null;

            // Lọc danh sách học sinh
            var filterHocSinh = Builders<HocSinhModel>.Filter.And(
                       Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                        Builders<HocSinhModel>.Filter.In(hs => hs.Id, lichSuPhatMinh.IdHocSinhs)
                     );
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var hocSinhs = hocSinhCollection.Find(filterHocSinh).ToList();
            var xacMinhVangBangListModel = new XacMinhVanBangListModel()
            {
                NguoiKyBang = lichSuPhatMinh.NguoiKyBang,
                CoQuanCapBang = lichSuPhatMinh.CoQuanCapBang,
                DiaPhuongCapBang = lichSuPhatMinh.DiaPhuongCapBang,
                UyBanNhanDan = lichSuPhatMinh.UyBanNhanDan,
                DonViYeuCauXacMinh = lichSuPhatMinh.DonViYeuCauXacMinh,
                CongVanSo = lichSuPhatMinh.CongVanSo,
                NgayTrenCongVan = lichSuPhatMinh.NgayTrenCongVan,
                PathFileYeuCau = lichSuPhatMinh.PathFileYeuCau
            };

            xacMinhVangBangListModel.HocSinhs = hocSinhs.Select(hs => new HocSinhXacMinhVBViewModel
            {
                Id = hs.Id,
                HoTen = hs.HoTen,
                NgaySinh = hs.NgaySinh,
                HoiDong = hs.HoiDong,
                CCCD = hs.CCCD,
                NoiSinh = hs.NoiSinh,
                GioiTinh = hs.GioiTinh,
                KhoaThi = GetKhoaThiByIdDanhMucTotNghiep(hs.IdDanhMucTotNghiep).Where(k => k.Id == hs.IdKhoaThi).FirstOrDefault().Ngay
            }).ToList();

            return xacMinhVangBangListModel;
        }

        private List<KhoaThiModel> GetKhoaThiByIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var dmtn = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                    .Find(d => d.Id == idDanhMucTotNghiep).FirstOrDefault();

            if (dmtn == null)
                return new List<KhoaThiModel>();

            var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                    .Find(d => d.Id == dmtn.IdNamThi).FirstOrDefault();

            var khoaThiModel = new List<KhoaThiModel>();
            khoaThiModel = namThi.KhoaThis
                .Where(k => k.Xoa == false)
                .OrderByDescending(k => k.Ngay)
                .ToList();
            return khoaThiModel;
        }

    }
}
