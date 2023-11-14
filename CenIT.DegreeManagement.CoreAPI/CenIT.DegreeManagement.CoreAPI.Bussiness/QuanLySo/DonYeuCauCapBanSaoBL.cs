using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo
{
    public class DonYeuCauCapBanSaoBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private readonly string collectionNameDonYeuCauCapBanSao = "DonYeuCauCapBanSao";
        private readonly string collectionNameHocSinh = "HocSinh";
        private readonly string collectionNameSoGoc = "SoGoc";
        private readonly string collectionNamePhoiBanSao = "PhoiBanSao";
        private readonly string collectionNameSoCapBanSao = "SoCapBanSao";


        private IMongoDatabase _mongoDatabase;

        public DonYeuCauCapBanSaoBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<HocSinhResult> CreateDonYeuCau(DonYeuCauCapBanSaoInputModel model, string idDonViQuanLy)
        {
            try
            {
                var colectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(collectionNameHocSinh);
                var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(collectionNameDonYeuCauCapBanSao);

                var filter = Builders<HocSinhModel>.Filter.And(
                          Builders<HocSinhModel>.Filter.Eq(h => h.Xoa, false),
                          Builders<HocSinhModel>.Filter.Eq(h => h.CCCD, model.CCCD),
                          Builders<HocSinhModel>.Filter.Eq(h => h.IdTruong, model.IdTruong),
                          Builders<HocSinhModel>.Filter.And(
                              Builders<HocSinhModel>.Filter.Exists(h => h.IdSoGoc, true),
                              Builders<HocSinhModel>.Filter.Not(Builders<HocSinhModel>.Filter.Eq(h => h.IdSoGoc, ""))
                          )
                      );


                var hocSinh = colectionHocSinh.Find(filter).FirstOrDefault();
                var countDonYeuCau = colectionDonYeuCauCapBanSao.Find(d => d.Xoa == false).Count();

                if (hocSinh == null)
                    return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.InforStudentWrong };
                if(hocSinh.HoTen != model.HoTen)
                    return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.FullNameIncorrect };
                if (hocSinh.DanToc != model.DanToc)
                    return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.NationIncorrect };
                if (hocSinh.GioiTinh != model.GioiTinh)
                    return new HocSinhResult() { MaLoi =  (int)DonYeuCauCapBanSaoEnum.GenderIncorrect};
                if (hocSinh.XepLoai.ToLower() != model.XepLoai.ToLower())
                    return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.ClassificationIncorrect };
                if (hocSinh.NoiSinh != model.NoiSinh)
                    return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.PlaceIncorrect };
                if (hocSinh.NgaySinh.Date != model.NgaySinh.Date && hocSinh.NgaySinh.Date != model.NgaySinh.ToUniversalTime().Date)
                    return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.BirthDayIncorrect };

                string soHieu = "";
                string soVaoSo = "";
                string namHt = "";

                var randomSoVaoSo = SoVaoSoBanSao(out soVaoSo, out soHieu, out namHt ,idDonViQuanLy, model.IdTruong);

                var donYeuCau = new DonYeuCauCapBanSaoModel();
                ModelProvider.MapProperties(model, donYeuCau);
                ModelProvider.MapProperties(model, donYeuCau.ThongTinNguoiYeuCau);
                donYeuCau.Ma = model.CCCD + countDonYeuCau.ToString().PadLeft(2, '0');
                donYeuCau.IdHocSinh = hocSinh.Id;
                donYeuCau.NgayTao = DateTime.Now;
                donYeuCau.NguoiTao = model.NguoiThucHien;
                donYeuCau.SoVaoSoBanSao = soVaoSo;
                donYeuCau.SoHieu = soHieu;
                
                await colectionDonYeuCauCapBanSao.InsertOneAsync(donYeuCau);
                if (donYeuCau.Id != null)
                {
                    return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.Success, MaDonYeuCau = donYeuCau.Ma, Nam = namHt };
                }
                return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.Fail };
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.Fail };
            }
        }

        public string GetSerachDonYeuCapBanSao(DonYeuCauCapBanSaoParamModel modelSearch, TruongModel donVi)
        {
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");
            string pagination = MongoPipeline.GeneratePaginationPipeline(modelSearch.PageSize, modelSearch.StartIndex, "$donYeuCaus");
            //int skip = (modelSearch.StartIndex - 1) * modelSearch.PageSize;
            string matchMa = string.IsNullOrEmpty(modelSearch.Ma) ? "" : $@"'Ma': '{modelSearch.Ma}',";
            string matchHoTen = string.IsNullOrEmpty(modelSearch.HoTen) ? "" : $@"'HoTen': '{modelSearch.HoTen}',";
            string matchCCCD = string.IsNullOrEmpty(modelSearch.CCCD) ? "" : $@"'CCCD': '{modelSearch.CCCD}',";

            var truongs = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                         .Find(x => x.Xoa == false && x.IdCha == donVi.Id)
                         .ToList()
                         .Select(x => x.Id)
                         .ToArray();
            var truongIdsString = string.Join(",", truongs.Select(x => $"'{x}'"));

            var cmdRes = $@"
                        {{
                            'aggregate': 'DonYeuCauCapBanSao', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $match: {{
                                      Xoa: false,
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      IdHocSinh: {{ $toObjectId: '$IdHocSinh' }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: 'HocSinh',
                                      localField: 'IdHocSinh',
                                      foreignField: '_id',
                                      as: 'HocSinh',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      HocSinh: {{ $arrayElemAt: ['$HocSinh', 0] }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      _id: 0,
                                      Id: '$_id',
                                      Ma: '$Ma',
                                      HoTenNguoiYeuCau: '$ThongTinNguoiYeuCau.HoTenNguoiYeuCau',
                                      NgayYeuCau: '$NgayTao',
                                      HoTen: '$HocSinh.HoTen',
                                      CCCD: '$HocSinh.CCCD',
                                      GioiTinh: '$HocSinh.GioiTinh',
                                      NgaySinh: '$HocSinh.NgaySinh',
                                      NoiSinh: '$HocSinh.NoiSinh',
                                      SoLuongBanSao: '$SoLuongBanSao',
                                      SoDienThoaiNguoiYeuCau: '$ThongTinNguoiYeuCau.SoDienThoaiNguoiYeuCau',
                                      TrangThai: '$TrangThai',
                                      PhuongThucNhan: '$PhuongThucNhan',
                                      IdTruong: '$HocSinh.IdTruong'
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      TenCuoi: {{ $arrayElemAt: [{{ $split: ['$HoTen', ' '] }}, -1] }},
                                    }},
                                  }},
                                  {order}
                                 {{
                                    $match: {{

                                            'TrangThai': {(int)modelSearch.TrangThai},
                                            {matchHoTen}
                                            {matchMa}
                                            {matchCCCD}
                                            'IdTruong' : {{$in : [{truongIdsString}]  }}
                                    }},
                                  }},
                                  {{
                                    $group: {{
                                      _id: null,
                                      totalRow: {{ $sum: 1 }},
                                      donYeuCaus: {{
                                        $push: {{
                                          id: '$Id',
                                          ma: '$Ma',
                                          hoTenNguoiYeuCau: '$HoTenNguoiYeuCau',
                                          ngayYeuCau: '$NgayYeuCau',
                                          HoTen: '$HoTen',
                                          cccd: '$CCCD',
                                          gioiTinh: '$GioiTinh',
                                          ngaySinh: '$NgaySinh',
                                          noiSinh: '$NoiSinh',
                                          soLuongBanSao: '$SoLuongBanSao',
                                          soDienThoaiNguoiYeuCau: '$SoDienThoaiNguoiYeuCau',
                                          trangThai: '$TrangThai',
                                          phuongThucNhan: '$PhuongThucNhan'
                                        }},
                                      }},
                                    }},
                                  }},
                                  {pagination}
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        public List<DonYeuCauCapBanSaoViewModel> GetSearchDonYeuCau(out int total, DonYeuCauCapBanSaoParamModel modelSearch, TruongModel donVi)
        {
            var filterBuilder = Builders<DonYeuCauCapBanSaoViewModel>.Filter;
            var colectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(collectionNameHocSinh);
            var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoViewModel>(collectionNameDonYeuCauCapBanSao);
            var colectionTruongHoc = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

            var filters = new List<FilterDefinition<DonYeuCauCapBanSaoViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Ma)
                    ? filterBuilder.Eq("Ma", modelSearch.Ma)
                    : null,
    
                modelSearch.TrangThai == null ? filterBuilder.In("TrangThai", new List<TrangThaiDonYeuCauEnum>
                {
                        TrangThaiDonYeuCauEnum.DaDuyet,
                        TrangThaiDonYeuCauEnum.ChuaDuyet,
                        TrangThaiDonYeuCauEnum.TuChoi,
                }) : filterBuilder.Eq("TrangThai", modelSearch.TrangThai),
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var donYeuCaus = colectionDonYeuCauCapBanSao
                                .Find(combinedFilter)
                                .ToList();

            var donYeuCauVM = donYeuCaus.Join(
                          colectionHocSinh.AsQueryable(),
                          d => d.IdHocSinh,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.HocSinh = h;
                              return d;
                          }
                      ).Join(
                          colectionTruongHoc.AsQueryable(),
                          d => d.IdTruong,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.Truong = h;
                              return d;
                          }
                      ).ToList();
            total = donYeuCauVM.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            donYeuCauVM = donYeuCauVM.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return donYeuCauVM;
        }

        public List<DonYeuCauCapBanSaoViewModel> GetSearchDonYeuCauDaDuyet(out int total, HocSinhCapBanSaoParamModel modelSearch, TruongModel donVi)
        {
            var filterBuilder = Builders<DonYeuCauCapBanSaoViewModel>.Filter;
            var colectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(collectionNameHocSinh);
            var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoViewModel>(collectionNameDonYeuCauCapBanSao);
            var colectionTruongHoc = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

            var filters = new List<FilterDefinition<DonYeuCauCapBanSaoViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Ma)
                    ? filterBuilder.Eq("Ma", modelSearch.Ma)
                    : null,
                modelSearch.TrangThai == null ? filterBuilder.In("TrangThai", new List<TrangThaiDonYeuCauEnum>
                {
                        TrangThaiDonYeuCauEnum.DaDuyet,
                        TrangThaiDonYeuCauEnum.DaIn,
                        TrangThaiDonYeuCauEnum.DaPhat,

                }) : filterBuilder.Eq("TrangThai", modelSearch.TrangThai),
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var donYeuCaus = colectionDonYeuCauCapBanSao
                                .Find(combinedFilter)
                                
                                .ToList();

            var donYeuCauVM = donYeuCaus.Join(
                          colectionHocSinh.AsQueryable(),
                          d => d.IdHocSinh,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.HocSinh = h;
                              return d;
                          }
                      ).Join(
                          colectionTruongHoc.AsQueryable(),
                          d => d.IdTruong,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.Truong = h;
                              return d;
                          }
                      ).ToList();

            if(!string.IsNullOrEmpty(modelSearch.SoDienThoaiNguoiYeuCau))
            {
                donYeuCauVM = donYeuCauVM
                                .Where(x => x.ThongTinNguoiYeuCau.SoDienThoaiNguoiYeuCau == modelSearch.SoDienThoaiNguoiYeuCau)
                                .ToList();
            }


            if (modelSearch.NgayDuyet != null)
            {
                donYeuCauVM = donYeuCauVM.Where(x =>
                               (x.NgayDuyet?.Date == modelSearch.NgayDuyet?.Date))
                               .ToList();
            }
       

            if (!string.IsNullOrEmpty(modelSearch.HoTen))
            {
                donYeuCauVM = donYeuCauVM.Where(x => x.HocSinh.HoTen == modelSearch.HoTen).ToList();
            }
            if (!string.IsNullOrEmpty(modelSearch.CCCD))
            {
                donYeuCauVM = donYeuCauVM.Where(x => x.HocSinh.CCCD == modelSearch.CCCD).ToList();
            }
            donYeuCauVM = donYeuCauVM.Where(x => x.Truong.IdCha == donVi.Id).ToList();

            total = donYeuCauVM.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            donYeuCauVM = donYeuCauVM.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return donYeuCauVM;
        }

        public List<DonYeuCauCapBanSaoViewModel> GetSearchDonYeuCauCongThongTin(out int total, TraCuuDonYeuCau modelSearch)
        {
            var filterBuilder = Builders<DonYeuCauCapBanSaoViewModel>.Filter;
            var colectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(collectionNameHocSinh);
            var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoViewModel>(collectionNameDonYeuCauCapBanSao);

            var filters = new List<FilterDefinition<DonYeuCauCapBanSaoViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var donYeuCaus = colectionDonYeuCauCapBanSao
                                .Find(combinedFilter)
                                .ToList();

            var donYeuCauVM = donYeuCaus.Join(
                          colectionHocSinh.AsQueryable(),
                          d => d.IdHocSinh,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.HocSinh = h;
                              return d;
                          }
                      ).ToList();

    
            if (!string.IsNullOrEmpty(modelSearch.Cccd))
            {
                donYeuCauVM = donYeuCauVM.Where(x => x.HocSinh.CCCD == modelSearch.Cccd).ToList();
            }

            //donYeuCauVM = donYeuCauVM.Where(x => x.HocSinh.NgaySinh.Date == modelSearch.NgaySinh.ToUniversalTime().Date 
            //                                && x.HocSinh.HoTen == modelSearch.HoTen 
            //                                && x.IdNamThi == modelSearch.IdNamThi).ToList();
            donYeuCauVM = donYeuCauVM.Where(x =>
                                        (x.HocSinh.NgaySinh.Date == modelSearch.NgaySinh.ToUniversalTime().Date ||
                                        x.HocSinh.NgaySinh.Date == modelSearch.NgaySinh.Date) &&
                                        x.HocSinh.HoTen == modelSearch.HoTen &&
                                        x.IdNamThi == modelSearch.IdNamThi)
                                        .ToList();

            total = donYeuCauVM.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            donYeuCauVM = donYeuCauVM.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return donYeuCauVM;
        }

        public DonYeuCauCapBanSaoViewModel GetById(string idDonYeuCauCapBanSao)
        {
            var filterBuilder = Builders<DonYeuCauCapBanSaoViewModel>.Filter;
            var colectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(collectionNameHocSinh);
            var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoViewModel>(collectionNameDonYeuCauCapBanSao);

            var filter = Builders<DonYeuCauCapBanSaoViewModel>.Filter.And(
               Builders<DonYeuCauCapBanSaoViewModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
               Builders<DonYeuCauCapBanSaoViewModel>.Filter.Eq(dmtn => dmtn.Id, idDonYeuCauCapBanSao)
             );

            var donYeuCau = colectionDonYeuCauCapBanSao
                                           .Find(filter)
                                           .ToList();

            var donYeuCauVM = donYeuCau.Join(
                          colectionHocSinh.AsQueryable(),
                          d => d.IdHocSinh,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.HocSinh = h;
                              return d;
                          }
                      ).FirstOrDefault();
            
            return donYeuCauVM;
        }

        public async Task<DonYeuCauOutPutResult> TuChoiDoYeuCau(string idDonYeuCauCapBanSao, string lyDoTuChoi, string nguoiThucHien)
        {
            try
            {
                var filterBuilder = Builders<DonYeuCauCapBanSaoModel>.Filter;
                var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(collectionNameDonYeuCauCapBanSao);

                var filter = Builders<DonYeuCauCapBanSaoModel>.Filter.And(
                   Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                   Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(dmtn => dmtn.Id, idDonYeuCauCapBanSao)
                 );

                var donYeuCau = colectionDonYeuCauCapBanSao
                                               .Find(filter)
                                               .FirstOrDefault();
                if (donYeuCau == null)
                {
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.NotFound };
                }

                donYeuCau.TrangThai = TrangThaiDonYeuCauEnum.TuChoi;
                donYeuCau.NguoiCapNhat = nguoiThucHien;
                donYeuCau.NgayCapNhat = DateTime.Now;
                donYeuCau.LyDoTuChoi = lyDoTuChoi;
                donYeuCau.NgayDuyet = DateTime.Now;
                donYeuCau.NguoiDuyet = nguoiThucHien;

                var updateResult = await colectionDonYeuCauCapBanSao.ReplaceOneAsync(filter, donYeuCau);
                if (updateResult.ModifiedCount == 0)
                {
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.Fail };

                }
                return new DonYeuCauOutPutResult() 
                { 
                    MaLoi = (int)DonYeuCauCapBanSaoEnum.Success,
                    EmailNguoiYeuCau = donYeuCau.ThongTinNguoiYeuCau.EmailNguoiYeuCau,
                    HoTenNguoiYeuCau = donYeuCau.ThongTinNguoiYeuCau.HoTenNguoiYeuCau
                };

            }
            catch
            {
                return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.Fail };
            }

        }

        public async Task<DonYeuCauOutPutResult> DuyetDonYeuCau(DuyetDonYeuCauInputModel model)
        {
            try
            {
                var filterBuilder = Builders<DonYeuCauCapBanSaoModel>.Filter;
                var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(collectionNameDonYeuCauCapBanSao);
                var colectionHocSinh= _mongoDatabase.GetCollection<HocSinhModel>(collectionNameHocSinh);
                var colectionPhoiBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(collectionNamePhoiBanSao);
                var colectionSoCapBanSao = _mongoDatabase.GetCollection<SoCapBanSaoModel>(collectionNameSoCapBanSao);

                var filter = Builders<DonYeuCauCapBanSaoModel>.Filter.And(
                   Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                   Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(dmtn => dmtn.Id, model.IdDonYeuCauCapBanSao),
                   Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(dmtn => dmtn.TrangThai, TrangThaiDonYeuCauEnum.ChuaDuyet)
                 );


                var filterHocSinh = Builders<HocSinhModel>.Filter.And(
                          Builders<HocSinhModel>.Filter.Eq(h => h.Xoa, false),
                          Builders<HocSinhModel>.Filter.Eq(h => h.Id, model.IdHocSinh),
                          Builders<HocSinhModel>.Filter.And(
                              Builders<HocSinhModel>.Filter.Exists(h => h.IdSoGoc, true),
                              Builders<HocSinhModel>.Filter.Not(Builders<HocSinhModel>.Filter.Eq(h => h.IdSoGoc, ""))
                          )
                      );

                var hocSinh = colectionHocSinh.Find(filterHocSinh).FirstOrDefault();
                if (hocSinh == null)
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.NotExistHocSinh };

                var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(t => t.Xoa == false && t.Id == hocSinh.IdTruong).FirstOrDefault();
                var phoiBanSao = colectionPhoiBanSao.Find(p => p.Xoa == false && p.TinhTrang == TinhTrangPhoiEnum.DangSuDung && p.MaHeDaoTao == truong.MaHeDaoTao).FirstOrDefault();

                if (phoiBanSao == null)
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.NotExistPhoi };
                if (phoiBanSao.SoLuongPhoi < 1)
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.ExceedsPhoiGocLimit };

                var donYeuCau = colectionDonYeuCauCapBanSao
                                               .Find(filter)
                                               .FirstOrDefault();
                if (donYeuCau == null)
                {
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.NotFound };
                }

                var soBanSao = colectionSoCapBanSao.Find(h => h.Xoa == false && h.IdNamThi == donYeuCau.IdNamThi).FirstOrDefault();
                if (soBanSao == null)
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.NotExistHocSinh };


                donYeuCau.TrangThai = TrangThaiDonYeuCauEnum.DaDuyet;
                donYeuCau.NguoiCapNhat = model.NguoiThucHien;
                donYeuCau.NguoiDuyet = model.NguoiThucHien;
                donYeuCau.IdPhoiBanSao = phoiBanSao.Id;
                donYeuCau.IdSoCapBanSao = soBanSao.Id;
                donYeuCau.NgayDuyet = DateTime.Now;
                donYeuCau.NgayCapNhat = DateTime.Now;

                if (string.IsNullOrEmpty(hocSinh.IdSoCapBanSao)){
                    //hocSinh.SoVaoSoBanSao = Regex.Replace(hocSinh.SoVaoSoCapBang, @"\/", "");
                    //hocSinh.IdSoCapBanSao = soBanSao.Id;
                    //hocSinh.IdPhoiBanSao = phoiBanSao.Id;
                    phoiBanSao.SoLuongPhoi -= 1;
                    phoiBanSao.SoLuongPhoiDaSuDung += 1;
                }
         
                var updateResult = await colectionDonYeuCauCapBanSao.ReplaceOneAsync(filter, donYeuCau);
                if (updateResult.ModifiedCount == 0)
                {
                    return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.Fail };
                }
                await colectionHocSinh.ReplaceOneAsync((x=>x.Xoa == false && x.Id == hocSinh.Id), hocSinh);
                await colectionPhoiBanSao.ReplaceOneAsync((x => x.Xoa == false && x.Id == phoiBanSao.Id), phoiBanSao);

                return new DonYeuCauOutPutResult()
                {
                    MaLoi = (int)DonYeuCauCapBanSaoEnum.Success,
                    MaDon = donYeuCau.Ma,
                    HoTenNguoiYeuCau = donYeuCau.ThongTinNguoiYeuCau.HoTenNguoiYeuCau,
                    EmailNguoiYeuCau = donYeuCau.ThongTinNguoiYeuCau.EmailNguoiYeuCau,
                    SoLuongBanSao = donYeuCau.SoLuongBanSao,
                    LePhi = model.LePhi,
                    PhuongThucNhan = donYeuCau.PhuongThucNhan,
                    DiaChiNhan = donYeuCau.DiaChiNhan
                };

            }
            catch
            {
                return new DonYeuCauOutPutResult() { MaLoi = (int)DonYeuCauCapBanSaoEnum.Fail };
            }
        }

        public async Task<int> XacNhanIn(string idDonYeuCauCapBanSao, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<DonYeuCauCapBanSaoModel>.Filter.And(
                     Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiDonYeuCauEnum.DaDuyet),
                      Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(hs => hs.Xoa, false),
                      Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(hs => hs.Id, idDonYeuCauCapBanSao)
                    );

                var update = Builders<DonYeuCauCapBanSaoModel>.Update
                        .Set(nt => nt.TrangThai, TrangThaiDonYeuCauEnum.DaIn);

                var result = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(collectionNameDonYeuCauCapBanSao).UpdateMany(filter, update);

                if ((int)result.ModifiedCount == 0)
                {
                    return (int)DonYeuCauCapBanSaoEnum.ListEmpty;
                }
                return (int)DonYeuCauCapBanSaoEnum.Success;
            }
            catch
            {
                return (int)DonYeuCauCapBanSaoEnum.Fail;
            }
        }

        public async Task<int> XacNhanDaPhat(string idDonYeuCauCapBanSao)
        {
            try
            {
                var filter = Builders<DonYeuCauCapBanSaoModel>.Filter.And(
                     Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiDonYeuCauEnum.DaIn),
                      Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(hs => hs.Xoa, false),
                      Builders<DonYeuCauCapBanSaoModel>.Filter.Eq(hs => hs.Id, idDonYeuCauCapBanSao)
                    );

                var update = Builders<DonYeuCauCapBanSaoModel>.Update
                        .Set(nt => nt.TrangThai, TrangThaiDonYeuCauEnum.DaPhat);

                var result = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(collectionNameDonYeuCauCapBanSao).UpdateMany(filter, update);

                if ((int)result.ModifiedCount == 0)
                {
                    return (int)DonYeuCauCapBanSaoEnum.ListEmpty;
                }
                return (int)DonYeuCauCapBanSaoEnum.Success;
            }
            catch
            {
                return (int)DonYeuCauCapBanSaoEnum.Fail;
            }
        }

        public List<DonYeuCauCapBanSaoViewModel> GetSearchLichSuDonYeuCau(out int total,string idHocSinh ,SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<DonYeuCauCapBanSaoViewModel>.Filter;
            var colectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(collectionNameHocSinh);
            var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoViewModel>(collectionNameDonYeuCauCapBanSao);

            var filters = new List<FilterDefinition<DonYeuCauCapBanSaoViewModel>>
            {
                filterBuilder.In("TrangThai", new List<TrangThaiDonYeuCauEnum>
                {
                        TrangThaiDonYeuCauEnum.DaDuyet,
                        TrangThaiDonYeuCauEnum.DaIn,
                        TrangThaiDonYeuCauEnum.TuChoi,
                        TrangThaiDonYeuCauEnum.DaPhat,

                }),
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("IdHocSinh", idHocSinh)
            };

            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var donYeuCaus = colectionDonYeuCauCapBanSao
                                .Find(combinedFilter)
                                .ToList();

            var donYeuCauVM = donYeuCaus.Join(
                          colectionHocSinh.AsQueryable(),
                          d => d.IdHocSinh,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.HocSinh = h;
                              return d;
                          }
                      ).ToList();

            total = donYeuCauVM.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    donYeuCauVM = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? donYeuCauVM.OrderBy(x => x.HocSinh.NgayTao).ToList()
                        : donYeuCauVM.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            donYeuCauVM = donYeuCauVM.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return donYeuCauVM;
        }


        private bool SoVaoSoBanSao(out string soVaoSo, out string soHieu, out string namHT , string idDonViQuanLy, string idTruong)
        {

            var filter = Builders<TruongModel>.Filter.And(
                               Builders<TruongModel>.Filter.Eq(t => t.Xoa, false),
                               Builders<TruongModel>.Filter.Eq(t => t.Id, idDonViQuanLy)
                             );

            var donViQuanLy = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                        .Find(filter).FirstOrDefault();

            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                        .Find(Builders<TruongModel>.Filter.And(
                                           Builders<TruongModel>.Filter.Eq(t => t.Xoa, false),
                                           Builders<TruongModel>.Filter.Eq(t => t.Id, idTruong)
                                         )).FirstOrDefault();

            int stt = 0;
            var nam = donViQuanLy.CauHinh.Nam;
            if (donViQuanLy.CauHinh.Nam == DateTime.Now.Year.ToString())
            {
                stt = donViQuanLy.CauHinh.SoDonYeuCau;
            }
            else
            {
                nam = DateTime.Now.Year.ToString();
            }

            //// tiền tố + mã trường + loại bằng(THCS:2, THPT:1) + SỐ TỰ RANDOM + /NĂM
            string soBatDau = (donViQuanLy.CauHinh.SoBatDau + stt).ToString();
            string chuoiSo = soBatDau.PadLeft(donViQuanLy.CauHinh.SoKyTu, '0');
            soVaoSo = donViQuanLy.CauHinh.TienToBanSao + "-" + chuoiSo + "/" + nam + "-" + truong.Ma;
            soHieu = truong.Ma  + chuoiSo;

            namHT = nam;
            return true;
        }

    }
}
