using CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Data;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh
{
    public class HocSinhBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public HocSinhBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        #region Dùng chung

        /// <summary>
        /// Thêm danh sách học sinh
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task<ImportResultModel> ImportHocSinh(List<HocSinhModel> models, string idTruong, string idDanhMucTotNghiep, bool phong =false)
        {
            try
            {
                var collectionHoSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                // kiểm tra HTDT có tồn tại
                var checkExistHTDT = await KiemTraHinhThucDaoTaoCoTonTai(idTruong, idDanhMucTotNghiep);
                if (checkExistHTDT < 0)
                    return new ImportResultModel { ErrorCode = checkExistHTDT };

 

                int maxSTT = collectionHoSinh.Find(x => x.Xoa == false && x.IdTruong == idTruong)
                                     .Sort(Builders<HocSinhModel>.Sort.Descending(x => x.STT))
                                     .Limit(1)
                                     .FirstOrDefault()?.STT ?? 0;

                // Increment STT for new entries
                foreach (var model in models)
                {
                    model.STT = ++maxSTT;
                }

                collectionHoSinh.InsertMany(models);
                return new ImportResultModel { ErrorCode = (int)HocSinhEnum.Success };
            }
            catch
            {
                return new ImportResultModel { ErrorCode = (int)HocSinhEnum.Fail };
            }
        }

        /// <summary>
        /// Cập nhật học sinh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(HocSinhInputModel model)
        {
            try
            {
                var checkExistHTDT = await KiemTraHinhThucDaoTaoCoTonTai(model.IdTruong, model.IdDanhMucTotNghiep);
                if (checkExistHTDT < 0)
                    return checkExistHTDT;
                var collectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                if (TrungCCCD(model.CCCD, model.Id)) return (int)HocSinhEnum.ExistCccd;

                var dmtn = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                           .Find(h => h.Xoa == false && h.Id == model.IdDanhMucTotNghiep).FirstOrDefault();
                if (dmtn == null)
                {
                    return (int)HocSinhEnum.NotExistDanhMucTotNghiep;
                }

                if (!KiemTraTruong(model.IdTruong)) return (int)HocSinhEnum.NotExistTruong;
                if (!KiemTraDanToc(model.DanToc)) return (int)HocSinhEnum.NotExistDanToc;
                if (!KiemTraKhoaThi(dmtn.IdNamThi, model.IdKhoaThi)) return (int)HocSinhEnum.NotExist;

                var hocSinh = collectionHocSinh.Find(h => h.Xoa == false && h.Id == model.Id).FirstOrDefault();
                if (hocSinh == null) return (int)HocSinhEnum.NotExist;


                ModelProvider.MapProperties(model, hocSinh);
                hocSinh.NgayCapNhat = DateTime.Now;
                hocSinh.NguoiCapNhat = model.NguoiThucHien;

                var updateResult = await collectionHocSinh.ReplaceOneAsync(h=>h.Id == hocSinh.Id, hocSinh);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)HocSinhEnum.Fail;
                }
                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        
        }

        /// <summary>
        /// Lấy học sinh theo cccd 
        /// </summary>
        /// <param name="cccd"></param>
        /// <returns></returns>
        public HocSinhViewModel? GetHocSinhByCccd(string cccd)
        {
            var filter = Builders<HocSinhViewModel>.Filter.And(
                           Builders<HocSinhViewModel>.Filter.Eq(hs => hs.Xoa, false),
                           Builders<HocSinhViewModel>.Filter.Eq(hs => hs.CCCD, cccd)
                         );

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var htdtCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);
            var hdtCollection = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao);


            var hocSinhs = hocSinhCollection.Find(filter) .ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var hocSinh = hocSinhs
                      .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong;
                              return hs;
                          }
                      )
                      .Join(
                          hdtCollection.AsQueryable(),
                          hs => hs.Truong.MaHeDaoTao,
                          hdt => hdt.Ma,
                          (hs, hdt) =>
                          {
                              hs.TenHeDaoTao = hdt.Ten;
                              return hs;
                          }
                      )
                      .Join(
                          danhMucTotNghiepCollection.AsQueryable(),
                          hs => hs.IdDanhMucTotNghiep,
                          dmtn => dmtn.Id,
                          (hs, dmtn) =>
                          {
                              hs.DanhMucTotNghiep = dmtn;
                              return hs;
                          }
                      )
                      .Join(
                          htdtCollection.AsQueryable(),
                          hs => hs.DanhMucTotNghiep.IdHinhThucDaoTao,
                          htdt => htdt.Id,
                          (hs, htdt) =>
                          {
                              hs.MaHinhThucDaotao = htdt.Ma;
                              return hs;
                          }
                      )
                      .Join(
                            namThiCollection.AsQueryable(),
                            hs => hs.DanhMucTotNghiep.IdNamThi,
                            nt => nt.Id,
                            (hs, n) =>
                            {
                                hs.NamThi = n.Ten;
                                hs.KhoaThi = n.KhoaThis.Where(x => x.Id == hs.IdKhoaThi).FirstOrDefault().Ngay;
                                return hs;
                            } 
                        )
                      .FirstOrDefault();

            var cauHinhTruong = GetCauHinhByDonViQuanLY(hocSinh.IdTruong);
            hocSinh.SoGoc.UyBanNhanDan = cauHinhTruong.TenUyBanNhanDan;
            hocSinh.SoGoc.CoQuanCapBang = cauHinhTruong.TenCoQuanCapBang;
            hocSinh.SoGoc.DiaPhuongCapBang = cauHinhTruong.TenDiaPhuongCapBang;
            hocSinh.SoGoc.NguoiKyBang = cauHinhTruong.HoTenNguoiKySoGoc;

            return hocSinh;
        }

        /// <summary>
        /// Thêm học sinh
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(HocSinhInputModel model, bool phong = false)
        {
            try
            {
                var checkExistHTDT = await KiemTraHinhThucDaoTaoCoTonTai(model.IdTruong, model.IdDanhMucTotNghiep);
                if (checkExistHTDT < 0)
                    return checkExistHTDT;

                var collectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                if (TrungCCCD(model.CCCD, model.Id)) return (int)HocSinhEnum.ExistCccd;

                var dmtn = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                           .Find(h => h.Xoa == false && h.Id == model.IdDanhMucTotNghiep).FirstOrDefault();
                if(dmtn == null)
                {
                    return (int)HocSinhEnum.NotExistDanhMucTotNghiep;
                }

                if (!KiemTraTruong(model.IdTruong)) return (int)HocSinhEnum.NotExistTruong;
                if (!KiemTraDanToc(model.DanToc)) return (int)HocSinhEnum.NotExistDanToc;
                if (!KiemTraKhoaThi(dmtn.IdNamThi, model.IdKhoaThi)) return (int)HocSinhEnum.NotExist;

                var hocSinhs = collectionHocSinh
                                        .Find(h => h.Xoa == false && h.IdTruong == model.IdTruong)
                                        .ToList();
                int stt = 1;
                if(hocSinhs.Count > 0)
                {
                  stt =  hocSinhs.Max(h => h.STT) + 1;
                }

                var hocSinhModel = new HocSinhModel();
                model.HoTen.Trim();
                ModelProvider.MapProperties(model, hocSinhModel);
                hocSinhModel.STT = stt;
                hocSinhModel.NgayTao = DateTime.Now;
                hocSinhModel.NguoiTao = model.NguoiThucHien;
                if (phong)
                {
                    hocSinhModel.TrangThai = TrangThaiHocSinhEnum.ChoDuyet;
                }else
                {
                    hocSinhModel.TrangThai = TrangThaiHocSinhEnum.ChuaXacNhan;
                }


                await _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).InsertOneAsync(hocSinhModel);
                if (hocSinhModel.Id != null)
                {
                    return (int)HocSinhEnum.Success;

                }
                return (int)HocSinhEnum.Fail;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }

        /// <summary>
        ///  Tổng số học sinh theo trạng thái
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int TongSoHocSinhTheoTrangThai(string idDanhMucTotNghiep, string idTruong, List<TrangThaiHocSinhEnum> trangThais)
        {
            var filterBuilder = Builders<HocSinhModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhModel>>
            {
                filterBuilder.In("TrangThai", trangThais),
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("IdDanhMucTotNghiep", idDanhMucTotNghiep),
                filterBuilder.Eq("IdTruong", idTruong)

            };

            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var hocSinhs = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                                .Find(combinedFilter)
                                .Count();
            return (int)hocSinhs;
        }
        public string[] GetAllArrCCCD()
        {
            var filterBuilder = Builders<HocSinhModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhModel>>
            {
                filterBuilder.Eq("Xoa", false),
            };

            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var arrCCCD = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                                .Find(combinedFilter)
                                .ToList().Select(x => x.CCCD).OrderByDescending(x => x).ToArray(); ;
            return arrCCCD;
        }

        #endregion

        #region Học sinh Trường
        /// <summary>
        /// Xóa hoc sinh chưa xác nhận theo danh sách
        /// </summary>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public async Task<int> XoaHocSinhChuaXacNhan(string idTruong, List<string> listCCCD)
        {
            try
            {
                var filter = Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong)
                     & Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChuaXacNhan);

                if (listCCCD != null && listCCCD.Count > 0)
                {
                    var cccdFilter = Builders<HocSinhModel>.Filter.In(hs => hs.CCCD, listCCCD);
                    filter &= cccdFilter;
                }

                var deleteResult = await _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).DeleteManyAsync(filter);

                if (deleteResult.DeletedCount == 0)
                {
                    return (int)HocSinhEnum.ListEmpty;
                }
                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }

        /// <summary>
        /// Xóa tất cả học sinh chưa xác nhận
        /// </summary>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public async Task<int> XoaTatCaHocSinhChuaXacNhan(string idTruong)
        {
            try
            {
                var filter = Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong)
                    & Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChuaXacNhan);

                var deleteResult = await _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).DeleteManyAsync(filter);

                if (deleteResult.DeletedCount == 0)
                {
                    return (int)HocSinhEnum.ListEmpty;
                }
                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }

        /// <summary>
        /// Lấy danh sách học sinh by trường (Chưa xác nhận, đang chờ duyệt, đã duyệt, đã đưa vào sổ gốc)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSearchHocSinhByTruong(string idTruong, HocSinhParamModel modelSearch)
        {
            //Phân trang
            int skip = ((modelSearch.StartIndex - 1) * modelSearch.PageSize) + modelSearch.PageSize;
            string pagination = modelSearch.PageSize < 0 ? $@"hocSinhs: '$hocSinhs'" : $@"hocSinhs: {{ $slice: ['$hocSinhs', {skip}, {modelSearch.PageSize}] }},";
            // Sắp xếp
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");
            //Tìm kiếm và lọc
            string matchTrangThai = modelSearch.TrangThai == null ? $@"TrangThai : {{$in: [0,1,2,3]}}" : $@"'TrangThai': {modelSearch.TrangThai},";
            string matchHoTen = string.IsNullOrEmpty(modelSearch.HoTen) ? "" : $@"'HoTen': '{modelSearch.HoTen}',";
            string matchCCCD = string.IsNullOrEmpty(modelSearch.CCCD) ? "" : $@"'CCCD': '{modelSearch.CCCD}',";
            string matchNoiSinh = string.IsNullOrEmpty(modelSearch.NoiSinh) ? "" : $@"'NoiSinh': '{modelSearch.NoiSinh}',";
            string matchSearch = string.IsNullOrEmpty(modelSearch.Search) ? "" : $@"'HoTen': '{modelSearch.HoTen}',";
            string matchIdDanhMucTotNghiep = string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep) ? "" : $@"'IdDanhMucTotNghiep': '{modelSearch.IdDanhMucTotNghiep}',";

            
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $match: {{
                                      Xoa: false,
                                      IdTruong: '{idTruong}', 
                                       {matchTrangThai}
                                       {matchHoTen}
                                       {matchCCCD}
                                       {matchNoiSinh}
                                       {matchSearch}
                                       {matchIdDanhMucTotNghiep}
                                    }},
                                  }},
                                    {{
                                        $addFields: {{
                                          IdTruong: {{ $toObjectId: '$IdTruong' }},
                                        }},
                                      }},
                                      {{
                                        $lookup: {{
                                          from: 'Truong',
                                          localField: 'IdTruong',
                                          foreignField: '_id',
                                          as: 'Truongs',
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          Truong: {{ $arrayElemAt: ['$Truongs', 0] }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          Truongs: 0,
                                        }},
                                      }},
                                    {order}
                                  {{
                                    $group: {{
                                      _id: null,
                                      totalRow: {{ $sum: 1 }},
                                      hocSinhs: {{
                                        $push: {{
                                          id: '$_id',
                                          hoTen: '$HoTen',
                                          noiSinh: '$NoiSinh',
                                          cccd: '$CCCD',
                                          gioiTinh: '$GioiTinh',
                                          ngaySinh: '$NgaySinh',
                                          soHieuVanBang: '$SoHieuVanBang',
                                          soVaoSoCapBang: '$SoVaoSoCapBang',
                                          trangThai: '$TrangThai',
                                          tenTruong: '$Truong.Ten',
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

            return json;
        }

        /// <summary>
        /// Xác nhận tất cả học sinh
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> XacNhanTatCaHocSinh(string idTruong, string idDanhMucTotNghiep)
        {
            try
            {
                //if (KiemTraTruongDaDuyetDanhSach(idTruong, idDanhMucTotNghiep))
                //    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Approved };

                var conllectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionTruong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

                var danhMucTotNghiep = conllectionDanhMucTotNghiep.Find(dm => dm.Xoa == false && dm.Id == idDanhMucTotNghiep).FirstOrDefault();
                var truong = conllectionTruong.Find(t => t.Xoa == false && t.Id == idTruong).FirstOrDefault();
                var hocSinhs = conllectionHocSinh.Find(h => h.Xoa == false && h.IdTruong == idTruong
                                               && h.IdDanhMucTotNghiep == idDanhMucTotNghiep
                                               && h.TrangThai == TrangThaiHocSinhEnum.ChoDuyet)
                                               .ToList();
                if (danhMucTotNghiep == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };
                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };

                var filter = Builders<HocSinhModel>.Filter.And(
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChuaXacNhan),
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)
                 );

                var update = Builders<HocSinhModel>.Update
                    .Set(nt => nt.TrangThai, TrangThaiHocSinhEnum.ChoDuyet);

                UpdateResult result = conllectionHocSinh.UpdateMany(filter, update);
                if ((int)result.ModifiedCount == 0)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty};
                }

                if(hocSinhs.Count > 0)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Success, DaGui = true, TenTruong = truong.Ten, MaHeDaoTao = truong.MaHeDaoTao };
                }
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Success, MaHeDaoTao = truong.MaHeDaoTao};

            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }

        }

        /// <summary>
        /// Xác nhận học sinh theo danh sách
        /// </summary>
        /// <param name="maTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> XacNhanHocSinh(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            try
            {
                //if (KiemTraTruongDaDuyetDanhSach(idTruong, idDanhMucTotNghiep))
                //    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Approved };


                var conllectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionTruong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

                var filter = Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong)
                    & Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)
                     & Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChuaXacNhan);

                var danhMucTotNghiep = conllectionDanhMucTotNghiep.Find(dm => dm.Xoa == false && dm.Id == idDanhMucTotNghiep).FirstOrDefault();
                var truong = conllectionTruong.Find(t => t.Xoa == false && t.Id == idTruong).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };
                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };

                var hocSinhs = conllectionHocSinh.Find(h => h.Xoa == false && h.IdTruong == idTruong
                                    && h.IdDanhMucTotNghiep == idDanhMucTotNghiep
                                    && h.TrangThai == TrangThaiHocSinhEnum.ChoDuyet)
                                    .ToList();

                if (listCCCD != null && listCCCD.Count > 0)
                {
                    var cccdFilter = Builders<HocSinhModel>.Filter.In(hs => hs.CCCD, listCCCD);
                    filter &= cccdFilter;
                }

                var update = Builders<HocSinhModel>.Update
                        .Set(nt => nt.TrangThai, TrangThaiHocSinhEnum.ChoDuyet);

                var result = await _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).UpdateManyAsync(filter, update);

                if ((int)result.ModifiedCount == 0)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty };
                }

                if (hocSinhs.Count > 0)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Success, DaGui = true, TenTruong = truong.Ten, MaHeDaoTao = truong.MaHeDaoTao };
                }
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Success, MaHeDaoTao = truong.MaHeDaoTao };
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }
        }

        /// <summary>
        /// Lấy tất cả danh sách học sinh đã duyệt the id truong và danh mục tốt nghiệp để in bằng tạm thời
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public List<HocSinhViewModel> GetAllHocSinhDaDuyet(string idTruong, string idDanhMucTotNghiep)
        {
            var trangThais = new List<TrangThaiHocSinhEnum>
                            {
                                TrangThaiHocSinhEnum.DaDuyet,
                                TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                             };
            var filter = Builders<HocSinhViewModel>.Filter.And(
                       Builders<HocSinhViewModel>.Filter.Eq(hs => hs.Xoa, false),
                       Builders<HocSinhViewModel>.Filter.In(hs => hs.TrangThai, trangThais),
                       Builders<HocSinhViewModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                       Builders<HocSinhViewModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)

                     );

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);

            var hocSinhs = hocSinhCollection.Find(filter).ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var hocSinhVMs = hocSinhs
                      .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong;
                              return hs;
                          }
                      )
                      .Join(
                          danhMucTotNghiepCollection.AsQueryable(),
                          hs => hs.IdDanhMucTotNghiep,
                          dmtn => dmtn.Id,
                          (hs, dmtn) =>
                          {
                              hs.DanhMucTotNghiep = dmtn;
                              return hs;
                          }
                      )
                      .GroupJoin(
                          soGocCollection.AsQueryable(),
                          hs => hs.IdSoGoc,
                          s => s.Id,
                          (hs, sGroup) =>
                          {
                              var soGoc = sGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                              if (soGoc == null)
                              {
                                  hs.SoGoc = new SoGocModel(); // Create a new instance of SoGocModel
                              }
                              else
                              {
                                  hs.SoGoc = soGoc;
                              }
                              return hs;
                          }
                      )
                      .ToList();
            return hocSinhVMs;
        }

        /// <summary>
        /// Lấy thông tin học sinh theo listCCCD
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public List<HocSinhViewModel> GetHocSinhDaDuyetByCCCD(string idTruong, List<string> listCCCD)
        {
            var trangThais = new List<TrangThaiHocSinhEnum>
                            {
                                TrangThaiHocSinhEnum.DaDuyet,
                                TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                             };
            var filter = Builders<HocSinhViewModel>.Filter.And(
                       Builders<HocSinhViewModel>.Filter.Eq(hs => hs.Xoa, false),
                       Builders<HocSinhViewModel>.Filter.In(hs => hs.TrangThai, trangThais),
                       Builders<HocSinhViewModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                        Builders<HocSinhViewModel>.Filter.In(hs => hs.CCCD, listCCCD) // Change from Eq to In
                     );

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);

            var hocSinhs = hocSinhCollection.Find(filter).ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var hocSinhVMs = hocSinhs
                      .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong;
                              return hs;
                          }
                      )
                      .Join(
                          danhMucTotNghiepCollection.AsQueryable(),
                          hs => hs.IdDanhMucTotNghiep,
                          dmtn => dmtn.Id,
                          (hs, dmtn) =>
                          {
                              hs.DanhMucTotNghiep = dmtn;
                              return hs;
                          }
                      )
                      .GroupJoin(
                          soGocCollection.AsQueryable(),
                          hs => hs.IdSoGoc,
                          s => s.Id,
                          (hs, sGroup) =>
                          {
                              var soGoc = sGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                              if (soGoc == null)
                              {
                                  hs.SoGoc = new SoGocModel(); // Create a new instance of SoGocModel
                              }
                              else
                              {
                                  hs.SoGoc = soGoc;
                              }
                              return hs;
                          }
                      )
                      .ToList();
            return hocSinhVMs;
        }
        #endregion

        #region Học Sinh phòng
        /// <summary>
        /// Lấy danh sách học sinh đang chờ duyệt
        /// (Phòng giáo dục và đào tạo)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSearchHocSinhChoDuyetByPhong(string? idTruong, HocSinhParamModel modelSearch)
        {
            //Phân trang
            int skip = ((modelSearch.StartIndex - 1) * modelSearch.PageSize) + modelSearch.PageSize;
            string pagination = modelSearch.PageSize < 0 ? $@"hocSinhs: '$hocSinhs'" : $@"hocSinhs: {{ $slice: ['$hocSinhs', {skip}, {modelSearch.PageSize}] }},";
            // Sắp xếp
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");
            //Tìm kiếm và lọc
            string matchTrangThai = modelSearch.TrangThai == null ? $@"TrangThai : {{$in: [1]}}" : $@"'TrangThai': {modelSearch.TrangThai},";
            string matchHoTen = string.IsNullOrEmpty(modelSearch.HoTen) ? "" : $@"'HoTen': '{modelSearch.HoTen}',";
            string matchCCCD = string.IsNullOrEmpty(modelSearch.CCCD) ? "" : $@"'CCCD': '{modelSearch.CCCD}',";
            string matchNoiSinh = string.IsNullOrEmpty(modelSearch.NoiSinh) ? "" : $@"'NoiSinh': '{modelSearch.NoiSinh}',";
            string matchSearch = string.IsNullOrEmpty(modelSearch.Search) ? "" : $@"'HoTen': '{modelSearch.HoTen}',";
            string matchIdDanhMucTotNghiep = string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep) ? "" : $@"'IdDanhMucTotNghiep': '{modelSearch.IdDanhMucTotNghiep}',";

            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $match: {{
                                      Xoa: false,
                                      IdTruong: '{idTruong}', 
                                       {matchTrangThai}
                                       {matchHoTen}
                                       {matchCCCD}                                   
                                       {matchNoiSinh}
                                       {matchSearch}
                                       {matchIdDanhMucTotNghiep}
                                    }},
                                  }},
                                    {{
                                        $addFields: {{
                                          IdTruong: {{ $toObjectId: '$IdTruong' }},
                                        }},
                                      }},
                                      {{
                                        $lookup: {{
                                          from: 'Truong',
                                          localField: 'IdTruong',
                                          foreignField: '_id',
                                          as: 'Truongs',
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          Truong: {{ $arrayElemAt: ['$Truongs', 0] }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          Truongs: 0,
                                        }},
                                      }},
                                    {order}
                                  {{
                                    $group: {{
                                      _id: null,
                                      totalRow: {{ $sum: 1 }},
                                      hocSinhs: {{
                                        $push: {{
                                          id: '$_id',
                                          hoTen: '$HoTen',
                                          noiSinh: '$NoiSinh',
                                          cccd: '$CCCD',
                                          gioiTinh: '$GioiTinh',
                                          ngaySinh: '$NgaySinh',
                                          soHieuVanBang: '$SoHieuVanBang',
                                          soVaoSoCapBang: '$SoVaoSoCapBang',
                                          trangThai: '$TrangThai',
                                          tenTruong: '$Truong.Ten',
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

            return json;
        }

        /// <summary>
        /// Trả lại tất cả học sinh theo trường và danh mục tốt nghiệp
        /// </summary>
        /// <param name="maTruong"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> TraLaiTatCaHocSinh(string idTruong, string idDanhMucTotNghiep)
        {
            try
            {
                var conllectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionTruong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

                var danhMucTotNghiep = conllectionDanhMucTotNghiep.Find(dm => dm.Xoa == false && dm.Id == idDanhMucTotNghiep).FirstOrDefault();
                var truong = conllectionTruong.Find(t => t.Xoa == false && t.Id == idTruong).FirstOrDefault();


                // Lấy danh sách học sinh đang chờ duyệt
                var hocSinhDaGui = conllectionHocSinh.Find(h => h.Xoa == false
                            && h.IdDanhMucTotNghiep == idDanhMucTotNghiep
                            && h.IdTruong == idTruong
                            && h.TrangThai == TrangThaiHocSinhEnum.ChoDuyet).ToList();

                if (danhMucTotNghiep == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };
                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong }; 

                var filter = Builders<HocSinhModel>.Filter.And(
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChoDuyet),
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)
                 );

                var update = Builders<HocSinhModel>.Update
                    .Set(nt => nt.TrangThai, TrangThaiHocSinhEnum.ChuaXacNhan);

                UpdateResult result = conllectionHocSinh.UpdateMany(filter, update);
                if ((int)result.ModifiedCount == 0)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty };
                }

                if (hocSinhDaGui.Count > 0)
                {
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.Success,
                        DaGui = true,
                    };
                }

                return new HocSinhResult()
                {
                    MaLoi = (int)HocSinhEnum.Success,
                }; ;
            }
            catch
            {
                return new HocSinhResult()
                {
                    MaLoi = (int)HocSinhEnum.Fail,
                }; 
            }
        }

        /// <summary>
        /// Trả lại theo danh sách
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> TraLaiHocSinh(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            try
            {
                var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
                // Lọc danh sách học sinh
                var filterHocSinh = Builders<HocSinhModel>.Filter.And(
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChoDuyet),
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep),
                            Builders<HocSinhModel>.Filter.In(hs => hs.CCCD, listCCCD)
                         );

                var hocSinhs = hocSinhCollection.Find(filterHocSinh).ToList();
                var truong = truongCollection.Find(dm => dm.Xoa == false && dm.Id == idTruong).FirstOrDefault();

                // Lấy danh sách học sinh đang chờ duyệt
                var hocSinhDaGui = hocSinhCollection.Find(h => h.Xoa == false
                            && h.IdDanhMucTotNghiep == idDanhMucTotNghiep
                            && h.IdTruong == idTruong
                            && h.TrangThai == TrangThaiHocSinhEnum.ChoDuyet).ToList();

                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };

           
                if (hocSinhs.Count > 0)
                {
                    foreach (var hocSinh in hocSinhs)
                    {
                        hocSinh.TrangThai = TrangThaiHocSinhEnum.ChuaXacNhan;
                    }

                    var models = new List<WriteModel<HocSinhModel>>();

                    foreach (var hocSinh in hocSinhs)
                    {

                        var upsert = new ReplaceOneModel<HocSinhModel>(
                                        filter: Builders<HocSinhModel>.Filter.Eq(p => p.Id, hocSinh.Id),
                                        replacement: hocSinh)
                        { IsUpsert = true };

                        models.Add(upsert);
                    }

                    await hocSinhCollection.BulkWriteAsync(models);
                    if (hocSinhDaGui.Count > 0)
                    {
                        return new HocSinhResult()
                        {
                            MaLoi = (int)HocSinhEnum.Success,
                            DaGui = true,
                        };
                    }
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.Success,
                    };
                }
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty };
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }
        }

        /// <summary>
        /// Duyệt tất cả học sinh (Tự động đánh số hiệu văn bằng và số vào sổ cấp bằng)
        /// </summary>
        /// <param name="truong"></param>
        /// <param name="truong"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> DuyetTatCaHocSinh(string idTruong, string idDanhMucTotNghiep)
        {
            try
            {
                var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);
                var conllectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionTruong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);


                // Lấy thông tin phôi gốc đang sử dụng
                var danhMucTotNghiep = conllectionDanhMucTotNghiep.Find(dm => dm.Xoa == false && dm.Id == idDanhMucTotNghiep).FirstOrDefault();
                var truong = conllectionTruong.Find(dm => dm.Xoa == false && dm.Id == idTruong).FirstOrDefault();
                var hocSinhDaDuyets = conllectionHocSinh.Find(h => h.Xoa == false
                                            && h.IdDanhMucTotNghiep == idDanhMucTotNghiep
                                            && h.IdTruong == idTruong
                                            && h.TrangThai == TrangThaiHocSinhEnum.DaDuyet).ToList();
       

                if (danhMucTotNghiep == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };
                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };

                var phoiGoc = conllectionPhoiGoc.Find(pg => pg.Xoa == false
                && pg.TinhTrang == TinhTrangPhoiEnum.DangSuDung && pg.MaHeDaoTao == truong.MaHeDaoTao).FirstOrDefault();
                if (phoiGoc == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistPhoi };

                var filterHocSinh = Builders<HocSinhModel>.Filter.And(
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChoDuyet), // Trạng thái là "ChoGui"
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                     Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep
                 ));

                //Lấy danh sách học sinh đang chờ duyệt theo truong và danh mục
                var hocSinhs = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                           .Find(filterHocSinh)
                           .ToList()
                           .OrderBy(h => h.STT)
                           .ToList();

                if (hocSinhs.Count > phoiGoc.SoLuongPhoi)
                {
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.ExceedsPhoiGocLimit
                    };
                }


                if (hocSinhs.Count > 0)
                {

                    int soBatDau = int.Parse(phoiGoc.SoBatDau) + phoiGoc.SoLuongPhoiDaSuDung;
                    int soBatDauLength = phoiGoc.SoBatDau.Length;

                    // Cấp số hiệu văn bằng và số vào sổ cấp bằng
                    foreach (var hocSinh in hocSinhs)
                    {
                        hocSinh.IdPhoiGoc = phoiGoc.Id;
                        hocSinh.SoHieuVanBang = $"{phoiGoc.SoHieuPhoi}{soBatDau.ToString().PadLeft(soBatDauLength, '0')}";
                        hocSinh.TrangThai = TrangThaiHocSinhEnum.DaDuyet;
                        soBatDau++;
                    }

                    var models = new List<WriteModel<HocSinhModel>>();

                    foreach (var hocSinh in hocSinhs)
                    {

                        var upsert = new ReplaceOneModel<HocSinhModel>(
                                        filter: Builders<HocSinhModel>.Filter.Eq(p => p.Id, hocSinh.Id),
                                        replacement: hocSinh)
                        { IsUpsert = true };

                        models.Add(upsert);
                    }

                    await conllectionHocSinh.BulkWriteAsync(models);
                    if(hocSinhDaDuyets.Count > 0)
                    {
                        return new HocSinhResult()
                        {
                            MaLoi = (int)HocSinhEnum.Success,
                            IdPhoi = phoiGoc.Id,
                            SoluongHocSinh = hocSinhs.Count(),
                            DaDuyet = true,
                            SoBatDau = soBatDau.ToString().PadLeft(soBatDauLength, '0')
                        };
                    }
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.Success,
                        IdPhoi = phoiGoc.Id, 
                        SoluongHocSinh = hocSinhs.Count(),
                        SoBatDau = soBatDau.ToString().PadLeft(soBatDauLength, '0') ,
                        TenTruong = truong.Ten
                    };
                }
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty };
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }
        }

        /// <summary>
        /// Duyệt theo danh sách
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> DuyetDanhSachHocSinh(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            try
            {
                var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
                var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);

                //var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
                // Lọc danh sách học sinh
                var filterHocSinh = Builders<HocSinhModel>.Filter.And(
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.ChoDuyet),
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                           Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep),
                            Builders<HocSinhModel>.Filter.In(hs => hs.CCCD, listCCCD) 
                         );

                var hocSinhs = hocSinhCollection.Find(filterHocSinh).ToList();
                var truong = truongCollection.Find(dm => dm.Xoa == false && dm.Id == idTruong).FirstOrDefault();
                // Lấy danh sách học sinh đã duyệt
                var hocSinhDaDuyets = hocSinhCollection.Find(h => h.Xoa == false
                            && h.IdDanhMucTotNghiep == idDanhMucTotNghiep
                            && h.IdTruong == idTruong
                            && h.TrangThai == TrangThaiHocSinhEnum.DaDuyet).ToList();

                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };

                var phoiGoc = conllectionPhoiGoc.Find(pg => pg.Xoa == false && pg.TinhTrang == TinhTrangPhoiEnum.DangSuDung && pg.MaHeDaoTao == truong.MaHeDaoTao).FirstOrDefault();
                if (phoiGoc == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistPhoi };

                if (hocSinhs.Count > phoiGoc.SoLuongPhoi)
                {
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.ExceedsPhoiGocLimit
                    };
                }


                if (hocSinhs.Count > 0)
                {

                    int soBatDau = int.Parse(phoiGoc.SoBatDau) + phoiGoc.SoLuongPhoiDaSuDung;
                    int soBatDauLength = phoiGoc.SoBatDau.Length;

                    // Cấp số hiệu văn bằng và số vào sổ cấp bằng
                    foreach (var hocSinh in hocSinhs)
                    {
                        hocSinh.IdPhoiGoc = phoiGoc.Id;
                        hocSinh.SoHieuVanBang = $"{phoiGoc.SoHieuPhoi}{soBatDau.ToString().PadLeft(soBatDauLength, '0')}";
                        hocSinh.TrangThai = TrangThaiHocSinhEnum.DaDuyet;
                        soBatDau++;
                    }

                    var models = new List<WriteModel<HocSinhModel>>();

                    foreach (var hocSinh in hocSinhs)
                    {

                        var upsert = new ReplaceOneModel<HocSinhModel>(
                                        filter: Builders<HocSinhModel>.Filter.Eq(p => p.Id, hocSinh.Id),
                                        replacement: hocSinh)
                        { IsUpsert = true };

                        models.Add(upsert);
                    }

                    await hocSinhCollection.BulkWriteAsync(models);
                    if (hocSinhDaDuyets.Count > 0)
                    {
                        return new HocSinhResult()
                        {
                            MaLoi = (int)HocSinhEnum.Success,
                            IdPhoi = phoiGoc.Id,
                            SoluongHocSinh = hocSinhs.Count(),
                            DaDuyet = true,
                            SoBatDau = soBatDau.ToString().PadLeft(soBatDauLength, '0'),
                            TenTruong = truong.Ten
                        };
                    }
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.Success,
                        IdPhoi = phoiGoc.Id,
                        SoluongHocSinh = hocSinhs.Count(),
                        SoBatDau = soBatDau.ToString().PadLeft(soBatDauLength, '0')
                    };
                }
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty };
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }
        }

        #endregion

        #region Cấp bằng

        /// <summary>
        /// Lấy danh sách học sinh đang chờ duyệt (chức năng cấp bằng)
        /// (Phòng giáo dục và đào tạo)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public List<HocSinhModel> GetSearchHocSinhCapBangByPhong(out int total, string? idTruong, HocSinhParamModel modelSearch)
        {
            var filterBuilder = Builders<HocSinhModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhModel>>
            {
                modelSearch.TrangThai == null ? filterBuilder.In("TrangThai", new List<TrangThaiHocSinhEnum>
                {
                        TrangThaiHocSinhEnum.DaDuyet,
                        TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                        TrangThaiHocSinhEnum.DaInBang,
                        TrangThaiHocSinhEnum.DaNhanBang
                }) : filterBuilder.Eq("TrangThai", modelSearch.TrangThai),
                !string.IsNullOrEmpty(modelSearch.HoTen)
                    ? filterBuilder.Regex("HoTen", new BsonRegularExpression(modelSearch.HoTen, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep)
                    ? filterBuilder.Eq("IdDanhMucTotNghiep", modelSearch.IdDanhMucTotNghiep)
                    : null,
                !string.IsNullOrEmpty(idTruong)
                    ? filterBuilder.Eq("IdTruong", idTruong)    
                    : null,
                !string.IsNullOrEmpty(modelSearch.DanToc)
                    ? filterBuilder.Regex("DanToc", new BsonRegularExpression(modelSearch.DanToc, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.NoiSinh)
                    ? filterBuilder.Regex("NoiSinh", new BsonRegularExpression(modelSearch.NoiSinh, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.CCCD)
                    ? filterBuilder.Regex("CCCD", new BsonRegularExpression(modelSearch.CCCD, "i"))
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var hocSinhs = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                                .Find(combinedFilter)
                                .ToList();

            total = hocSinhs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.STT).ToList()
                        : hocSinhs.OrderByDescending(x => x.STT).ToList();
                    break;
                case "1":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.HoTen.Split(' ').LastOrDefault()).ToList()
                        : hocSinhs.OrderByDescending(x => x.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
                case "2":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.CCCD).ToList()
                        : hocSinhs.OrderByDescending(x => x.CCCD).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                hocSinhs = hocSinhs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinhs;
        }

        public HocSinhResult GetAllPreviewHocSinhVaoSoGoc(string idTruong, string idDanhMucTotNghiep)
        {
            try
            {
                var conllectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionTruong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
                var conllectionNamThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);

                // Lấy thông tin phôi gốc đang sử dụng
                var danhMucTotNghiep = conllectionDanhMucTotNghiep.Find(dm => dm.Xoa == false && dm.Id == idDanhMucTotNghiep).FirstOrDefault();
                var truong = conllectionTruong.Find(dm => dm.Xoa == false && dm.Id == idTruong).FirstOrDefault();
              

                if (danhMucTotNghiep == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };
                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };

                var namThi = conllectionNamThi.Find(n => n.Xoa == false && n.Id == danhMucTotNghiep.IdNamThi).FirstOrDefault();
                if (namThi == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistYear };

                var filter = Builders<HocSinhModel>.Filter.And(
                    Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaDuyet),
                    Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                    Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)
                );

                var hocSinhs = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                               .Find(filter)
                               .ToList();


                if (hocSinhs.Count == 0)
                {
                    return new HocSinhResult
                    {
                        MaLoi = (int)HocSinhEnum.ListEmpty,
                    };
                }

                int stt = 0;
                var nam = truong.CauHinh.Nam;
                if(truong.CauHinh.Nam == DateTime.Now.Year.ToString())
                {
                    stt = truong.CauHinh.DinhDangSoThuTuSoGoc + 1;
                }else
                {
                    nam = DateTime.Now.Year.ToString();
                    stt += 1; 
                }

                foreach (var hocSinh in hocSinhs)
                {
                    hocSinh.SoVaoSoCapBang = $"{namThi.Ten}/{truong.Ma}/{stt.ToString().PadLeft(4, '0')}";
                    hocSinh.TrangThai = TrangThaiHocSinhEnum.DaDuaVaoSoGoc;
                    stt++;
                }

                return new HocSinhResult
                {
                    MaLoi = (int)HocSinhEnum.Success,
                    HocSinhs = hocSinhs,
                    Nam = nam
                };
            }
            catch
            {
                return new HocSinhResult
                {
                    MaLoi = (int)HocSinhEnum.Fail,
                };
            }
        }

        /// <summary>
        /// Đưa danh sách học sinh vào sổ gốc
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> PutIntoSoGoc(SoGocInputModel model, List<HocSinhModel> hocSinhs)
        {
            try
            {
                var conllectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionSoGoc = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
                var conllectionSoBanSao = _mongoDatabase.GetCollection<SoCapBanSaoModel>(_collectionNameSoCaoBanSao);
                var conllectionSoCapPhatBang = _mongoDatabase.GetCollection<SoCapPhatBangModel>(_collectionNameSoCapPhatBang);


                if (hocSinhs == null)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty};
                }

                var danhMucTotNghiep = conllectionDanhMucTotNghiep.Find(tr => true && tr.Xoa == false && tr.Id == model.IdDanhMucTotNghiep).FirstOrDefault();
                if (danhMucTotNghiep == null)
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep
                    };
                var soGoc = conllectionSoGoc.Find(tr => true && tr.Xoa == false && tr.IdNamThi == danhMucTotNghiep.IdNamThi).FirstOrDefault();
                var soBanSao = conllectionSoBanSao.Find(tr => true && tr.Xoa == false && tr.IdNamThi == danhMucTotNghiep.IdNamThi).FirstOrDefault();
                var soCapPhatBang = conllectionSoCapPhatBang.Find(tr => true && tr.Xoa == false && tr.IdNamThi == danhMucTotNghiep.IdNamThi).FirstOrDefault();

                if (soGoc == null)
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.NotExistSoGoc
                    };

             
                if (hocSinhs.Count > 0)
                {
                

                    ModelProvider.MapProperties(model, soGoc);
                    ModelProvider.MapProperties(model, soBanSao);
                    ModelProvider.MapProperties(model, soCapPhatBang);



                    foreach (var hocSinh in hocSinhs)
                    {
                        hocSinh.IdSoGoc = soGoc.Id;
                    }

                    var models = new List<WriteModel<HocSinhModel>>();

                    foreach (var hocSinh in hocSinhs)
                    {
                        var upsert = new ReplaceOneModel<HocSinhModel>(
                                        filter: Builders<HocSinhModel>.Filter.Eq(p => p.Id, hocSinh.Id),
                                        replacement: hocSinh)
                        { IsUpsert = true };

                        models.Add(upsert);
                    }

                    await conllectionSoGoc.ReplaceOneAsync((s=> s.Xoa == false && s.Id == soGoc.Id), soGoc);
                    await conllectionSoBanSao.ReplaceOneAsync((s => s.Xoa == false && s.Id == soBanSao.Id), soBanSao);
                    await conllectionSoCapPhatBang.ReplaceOneAsync((s => s.Xoa == false && s.Id == soCapPhatBang.Id), soCapPhatBang);

                    await conllectionHocSinh.BulkWriteAsync(models);
                    return new HocSinhResult()
                    {
                        MaLoi = (int)HocSinhEnum.Success,
                    };
                }
                return new HocSinhResult()
                {
                    MaLoi = (int)HocSinhEnum.ListEmpty
                };
            }

            catch
            {
                return new HocSinhResult()
                {
                    MaLoi = (int)HocSinhEnum.Fail
                };
            }
        }

        /// <summary>
        /// Lấy danh sách thông tin học sinh in bằng
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public List<HocSinhInBangModel> GetAllHocSinhDaDuaVaoSo(string idTruong, string idDanhMucTotNghiep)
        {
            var filter = Builders<HocSinhInBangModel>.Filter.And(
                       Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.Xoa, false),
                       Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaDuaVaoSoGoc),
                       Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                       Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)

                     );

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhInBangModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var HTDTCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);


            var hocSinhs = hocSinhCollection.Find(filter).ToList();

    

            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(d => d.Xoa == false)
                .ToList()
                 .Join(
                          namThiCollection.AsQueryable(),
                          d => d.IdNamThi,
                          n => n.Id,
                          (d, n) =>
                          {
                              d.NamThi = n.Ten;
                              return d;
                          }
                      ).Join(
                          HTDTCollection.AsQueryable(),
                          d => d.IdHinhThucDaoTao,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.HinhThucDaoTao = h.Ten;
                              return d;
                          }
                      ).ToList();

            // Join với bảng hình thức đào tạo và nam thi
            var hocSinhVMs = hocSinhs
                      .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong.Ten;
                              return hs;
                          }
                      )
                      .Join(
                          danhMucTotNghieps.AsQueryable(),
                          hs => hs.IdDanhMucTotNghiep,
                          dmtn => dmtn.Id,
                          (hs, dmtn) =>
                          {
                              hs.HinhThucDaoTao = dmtn.HinhThucDaoTao;
                              hs.NamThi = dmtn.NamThi;
                              hs.NgayCapBang = dmtn.NgayCapBang;
                              return hs;
                          }
                      )
                      .ToList();

            hocSinhVMs.ForEach(hocSinhVM =>
            {
                var cauHinhTruong = GetCauHinhByDonViQuanLY(idTruong);

                if (cauHinhTruong != null)
                {
                    hocSinhVM.UyBanNhanDan = cauHinhTruong.TenUyBanNhanDan;
                    hocSinhVM.DiaPhuongCapBang = cauHinhTruong.TenDiaPhuongCapBang;
                    hocSinhVM.NguoiKyBang = cauHinhTruong.HoTenNguoiKySoGoc;
                    hocSinhVM.CoQuanCapBang = cauHinhTruong.TenCoQuanCapBang;
                }
            });

            return hocSinhVMs;
        }

        public List<HocSinhInBangModel> GetListHocSinhDaDuaVaoSoByCCCD(string idTruong, string idDanhMucTotNghiep, List<string> listCCCD)
        {
            var filter = Builders<HocSinhInBangModel>.Filter.And(
                         Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.Xoa, false),
                         Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaDuaVaoSoGoc),
                         Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                         Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep),
                            Builders<HocSinhInBangModel>.Filter.In(hs => hs.CCCD, listCCCD) //

                       );
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhInBangModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var HTDTCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);

            var hocSinhs = hocSinhCollection.Find(filter).ToList();

            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(d => d.Xoa == false)
                .ToList()
                 .Join(
                          namThiCollection.AsQueryable(),
                          d => d.IdNamThi,
                          n => n.Id,
                          (d, n) =>
                          {
                              d.NamThi = n.Ten;
                              return d;
                          }
                      ).Join(
                          HTDTCollection.AsQueryable(),
                          d => d.IdHinhThucDaoTao,
                          h => h.Id,
                          (d, h) =>
                          {
                              d.HinhThucDaoTao = h.Ten;
                              return d;
                          }
                      ).ToList();

            // Join với bảng hình thức đào tạo và nam thi
            var hocSinhVMs = hocSinhs
                      .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong.Ten;
                              return hs;
                          }
                      )
                      .Join(
                          danhMucTotNghieps.AsQueryable(),
                          hs => hs.IdDanhMucTotNghiep,
                          dmtn => dmtn.Id,
                          (hs, dmtn) =>
                          {
                              hs.HinhThucDaoTao = dmtn.HinhThucDaoTao;
                              hs.NamThi = dmtn.NamThi;
                              hs.NgayCapBang = dmtn.NgayCapBang;
                              return hs;
                          }
                      )
                      .ToList();


            hocSinhVMs.ForEach(hocSinhVM =>
            {
                var cauHinhTruong = GetCauHinhByDonViQuanLY(idTruong);

                if (cauHinhTruong != null)
                {
                    hocSinhVM.UyBanNhanDan = cauHinhTruong.TenUyBanNhanDan;
                    hocSinhVM.DiaPhuongCapBang = cauHinhTruong.TenDiaPhuongCapBang;
                    hocSinhVM.NguoiKyBang = cauHinhTruong.HoTenNguoiKySoGoc;
                    hocSinhVM.CoQuanCapBang = cauHinhTruong.TenCoQuanCapBang;
                }
            });

            return hocSinhVMs;
        }

        public HocSinhInBangModel GetHocSinhDaDuaVaoSoGocById(string idHocSinh)
        {
            List<TrangThaiHocSinhEnum> trangThais = new List<TrangThaiHocSinhEnum>() { TrangThaiHocSinhEnum.DaDuaVaoSoGoc, TrangThaiHocSinhEnum.DaCapBang };

            var filter = Builders<HocSinhInBangModel>.Filter.And(
                                  Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.Xoa, false),
                                  Builders<HocSinhInBangModel>.Filter.In(hs => hs.TrangThai, trangThais),
                                  Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.Id, idHocSinh)
                                );
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhInBangModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var HTDTCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);

            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(d => d.Xoa == false)
               .ToList()
                .Join(
                         namThiCollection.AsQueryable(),
                         d => d.IdNamThi,
                         n => n.Id,
                         (d, n) =>
                         {
                             d.NamThi = n.Ten;
                             return d;
                         }
                     ).Join(
                         HTDTCollection.AsQueryable(),
                         d => d.IdHinhThucDaoTao,
                         h => h.Id,
                         (d, h) =>
                         {
                             d.HinhThucDaoTao = h.Ten;
                             return d;
                         }
                     ).ToList();
            var hocSinh = hocSinhCollection.Find(filter).ToList();
            var hocSinhVM = hocSinh
                               .Join(
                                     truongCollection.AsQueryable(),
                                     hs => hs.IdTruong,
                                     truong => truong.Id,
                                     (hs, truong) =>
                                     {
                                         hs.Truong = truong.Ten;
                                         return hs;
                                     }
                                 )
                                   .Join(
                                  danhMucTotNghieps.AsQueryable(),
                                  hs => hs.IdDanhMucTotNghiep,
                                  dmtn => dmtn.Id,
                                  (hs, dmtn) =>
                                  {
                                      hs.HinhThucDaoTao = dmtn.HinhThucDaoTao;
                                      hs.NamThi = dmtn.NamThi;
                                      hs.NgayCapBang = dmtn.NgayCapBang;
                                      return hs;
                                  }
                              )
                                 .FirstOrDefault();


            var cauHinhTruong = GetCauHinhByDonViQuanLY(hocSinhVM.IdTruong);
            hocSinhVM.UyBanNhanDan = cauHinhTruong.TenUyBanNhanDan;
            hocSinhVM.CoQuanCapBang = cauHinhTruong.TenCoQuanCapBang;
            hocSinhVM.DiaPhuongCapBang = cauHinhTruong.TenDiaPhuongCapBang;
            hocSinhVM.NguoiKyBang = cauHinhTruong.HoTenNguoiKySoGoc;

            return hocSinhVM;
        }


        /// <summary>
        /// Cập nhật Trạng thái đã in 
        /// </summary>
        /// <param name="listCCCD"></param>
        /// <returns></returns>
        public async Task<HocSinhResult> XacNhanInBang(string idDanhMucTotNghiep, string idTruong,List<string> listCCCD)
        {
            try
            {
                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                        .Find(d => d.Xoa == false && d.Id == idDanhMucTotNghiep).FirstOrDefault();
                var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                  .Find(d => d.Xoa == false && d.Id == idTruong).FirstOrDefault();

                if(truong == null)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };
                }

                if (danhMucTotNghiep == null)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };
                }

                var filter = Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaCapBang);

                if (listCCCD != null && listCCCD.Count > 0)
                {
                    var cccdFilter = Builders<HocSinhModel>.Filter.In(hs => hs.CCCD, listCCCD);
                    filter &= cccdFilter;
                }

                var update = Builders<HocSinhModel>.Update
                        .Set(nt => nt.TrangThai, TrangThaiHocSinhEnum.DaInBang);

                var result = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).UpdateMany(filter, update);

                if ((int)result.ModifiedCount == 0)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty };
                }

                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Success, 
                        IdNamThi = danhMucTotNghiep.IdNamThi, MaHeDaoTao = truong.MaHeDaoTao,
                        MaHinhThucDaoTao = truong.MaHinhThucDaoTao};
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }   
        }

        public async Task<HocSinhResult> XacNhanInBangTatCa(string idTruong, string idDanhMucTotNghiep)
        {
            try
            {
                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                       .Find(d => d.Xoa == false && d.Id == idDanhMucTotNghiep).FirstOrDefault();
                var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                  .Find(d => d.Xoa == false && d.Id == idTruong).FirstOrDefault();

                if (truong == null)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };
                }

                if (danhMucTotNghiep == null)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };
                }

                var trangThais = new List<TrangThaiHocSinhEnum>
                            {
                                TrangThaiHocSinhEnum.DaCapBang,
                             };

                var filter = Builders<HocSinhModel>.Filter.And(
                     Builders<HocSinhModel>.Filter.In(hs => hs.TrangThai, trangThais),
                      Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                      Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)
                    );

                var update = Builders<HocSinhModel>.Update
                        .Set(nt => nt.TrangThai, TrangThaiHocSinhEnum.DaInBang);

                var result = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).UpdateMany(filter, update);

                if ((int)result.ModifiedCount == 0)
                {
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.ListEmpty };
                }
                return new HocSinhResult()
                {
                    MaLoi = (int)HocSinhEnum.Success,
                    IdNamThi = danhMucTotNghiep.IdNamThi,
                    MaHeDaoTao = truong.MaHeDaoTao,
                    MaHinhThucDaoTao = truong.MaHinhThucDaoTao
                };
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }
        }

        public async Task<int> CapBangTatCa(string idTruong, string idDanhMucTotNghiep)
        {
            try
            {
                var trangThais = new List<TrangThaiHocSinhEnum>
                            {
                                TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                                TrangThaiHocSinhEnum.DaCapBang,
                             };

                var filter = Builders<HocSinhModel>.Filter.And(
                    Builders<HocSinhModel>.Filter.In(hs => hs.TrangThai, trangThais),
                    Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                    Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)
                );

                var update = Builders<HocSinhModel>.Update
                    .Set(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaCapBang)
                    .Inc(hs => hs.SoLanIn, 1);  // Sử dụng Inc để tăng giá trị SoLanIn thay vì Set

                var result = await _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).UpdateManyAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    return (int)HocSinhEnum.ListEmpty;
                }
                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }

        public async Task<int> CapBang(string idTruong, string idDanhMucTotNghiep, List<string> cccd)
        {
            try
            {
                var trangThais = new List<TrangThaiHocSinhEnum>
                            {
                                TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                                TrangThaiHocSinhEnum.DaCapBang,
                             };
                var filter = Builders<HocSinhModel>.Filter.And(
                    Builders<HocSinhModel>.Filter.In(hs => hs.TrangThai, trangThais),
                    Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                    Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep),
                    Builders<HocSinhModel>.Filter.In(hs => hs.CCCD, cccd)

                );

                var update = Builders<HocSinhModel>.Update
                    .Set(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaCapBang)
                    .Inc(hs => hs.SoLanIn, 1);  // Sử dụng Inc để tăng giá trị SoLanIn thay vì Set

                var result = await _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).UpdateManyAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    return (int)HocSinhEnum.ListEmpty;
                }
                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }

        #endregion

        #region Cấp bản sao
        public HocSinhInBangModel GetHocSinhDaDuaVaoSoBanSao(string idHocSinh ,string idDonYeuCau, TruongModel donVi)
        {
            var filter = Builders<HocSinhInBangModel>.Filter.And(
                       Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.Xoa, false),
                       Builders<HocSinhInBangModel>.Filter.Eq(hs => hs.Id, idHocSinh)
                     );

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhInBangModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soCapBanSaoCollection = _mongoDatabase.GetCollection<SoCapBanSaoModel>(_collectionNameSoCaoBanSao);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var HTDTCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);


            var donYeuCau = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(_collectionDonYeuCauCapBanSaoName)
                                .Find(d=>d.Xoa == false && d.Id == idDonYeuCau).FirstOrDefault();

            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(d => d.Xoa == false)
               .ToList()
                .Join(
                         namThiCollection.AsQueryable(),
                         d => d.IdNamThi,
                         n => n.Id,
                         (d, n) =>
                         {
                             d.NamThi = n.Ten;
                             return d;
                         }
                     ).Join(
                         HTDTCollection.AsQueryable(),
                         d => d.IdHinhThucDaoTao,
                         h => h.Id,
                         (d, h) =>
                         {
                             d.HinhThucDaoTao = h.Ten;
                             return d;
                         }
                     ).ToList();
            var hocSinhs = hocSinhCollection.Find(filter).ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var hocSinhVM = hocSinhs
                      .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong.Ten;
                              return hs;
                          }
                      )
                      .Join(
                           danhMucTotNghieps.AsQueryable(),
                           hs => hs.IdDanhMucTotNghiep,
                           dmtn => dmtn.Id,
                                 (hs, dmtn) =>
                                  {
                                      hs.HinhThucDaoTao = dmtn.HinhThucDaoTao;
                                      hs.NamThi = dmtn.NamThi;
                                      hs.NgayCapBang = dmtn.NgayCapBang;
                                      return hs;
                                  }
                              )
                      .FirstOrDefault();
            hocSinhVM.SoLuongBanSao = donYeuCau.SoLuongBanSao;
            hocSinhVM.SoVaoSoBanSao = donYeuCau.SoVaoSoBanSao;
            hocSinhVM.IdPhoiBanSao = donYeuCau.IdPhoiBanSao;
            hocSinhVM.UyBanNhanDan = donVi.CauHinh.TenUyBanNhanDan;
            hocSinhVM.DiaPhuongCapBang = donVi.CauHinh.TenDiaPhuongCapBang;
            hocSinhVM.NguoiKyBang = donVi.CauHinh.HoTenNguoiKySoGoc;
            hocSinhVM.CoQuanCapBang = donVi.CauHinh.TenCoQuanCapBang;

            return hocSinhVM;
        }
        #endregion

        #region Cấp phát bằng

        public List<HocSinhModel> GetSearchHocSinhCapPhatBangByTruong(out int total, string idTruong, HocSinhCapPhatBangParamModel modelSearch)
        {
            var filterBuilder = Builders<HocSinhModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhModel>>
            {
                modelSearch.TrangThai == null ? filterBuilder.In("TrangThai", new List<TrangThaiHocSinhEnum>
                {
                    TrangThaiHocSinhEnum.DaInBang,
                    TrangThaiHocSinhEnum.DaNhanBang,
                }) : filterBuilder.Eq("TrangThai", modelSearch.TrangThai),
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("IdTruong", idTruong),
                !string.IsNullOrEmpty(modelSearch.HoTen)
                    ? filterBuilder.Regex("HoTen", new BsonRegularExpression(modelSearch.HoTen, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep)
                    ? filterBuilder.Eq("IdDanhMucTotNghiep", modelSearch.IdDanhMucTotNghiep)
                    : null,
                !string.IsNullOrEmpty(modelSearch.SoHieuVanBang)
                    ? filterBuilder.Regex("SoHieuVanBang", new BsonRegularExpression(modelSearch.SoHieuVanBang, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.SoVaoSoCapBang)
                    ? filterBuilder.Regex("SoVaoSoCapBang", new BsonRegularExpression(modelSearch.SoVaoSoCapBang, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.CCCD)
                    ? filterBuilder.Regex("CCCD", new BsonRegularExpression(modelSearch.CCCD, "i"))
                    : null,
                 !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Regex("HoTen", new BsonRegularExpression(modelSearch.Search, "i"))
                    : null,
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var hocSinhs = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                                .Find(combinedFilter)
                                .ToList();

            total = hocSinhs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.STT).ToList()
                        : hocSinhs.OrderByDescending(x => x.STT).ToList();
                    break;
                case "1":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.HoTen.Split(' ').LastOrDefault()).ToList()
                        : hocSinhs.OrderByDescending(x => x.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
                case "2":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.CCCD).ToList()
                        : hocSinhs.OrderByDescending(x => x.CCCD).ToList();
                    break;
            }

            if (modelSearch.PageSize >= 0)
            {
                hocSinhs = hocSinhs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinhs;
        }

        public  async Task<int> CapPhatBang(ThongTinPhatBangInputModel model)
        {
            try
            {
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);

                var hocSinh = conllectionHocSinh.Find(h => h.Xoa == false && h.Id == model.IdHocSinh).FirstOrDefault();
                if (hocSinh == null)
                    return (int)HocSinhEnum.NotExist;
                var dmtn = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep).Find(x=>x.Xoa == false && x.Id == hocSinh.IdDanhMucTotNghiep).FirstOrDefault();
                var soCapPhatBang = _mongoDatabase.GetCollection<SoCapPhatBangModel>(_collectionNameSoCapPhatBang).Find(x => x.Xoa == false && x.IdNamThi == dmtn.IdNamThi).FirstOrDefault();

                ModelProvider.MapProperties(model, hocSinh.ThongTinPhatBang);
                hocSinh.TrangThai = TrangThaiHocSinhEnum.DaNhanBang;
                hocSinh.ThongTinPhatBang.NgayNhanBang = DateTime.Now;
                hocSinh.IdSoCapPhatBang = soCapPhatBang.Id;

                var updateResult = await conllectionHocSinh.ReplaceOneAsync((h=>h.Xoa == false && h.Id == hocSinh.Id), hocSinh);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)HocSinhEnum.Fail;
                }
                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }


        /// <summary>
        /// Lấy học sinh theo cccd 
        /// </summary>
        /// <param name="cccd"></param>
        /// <returns></returns>
        public HocSinhCapPhatBangViewModel? GetHocSinhCapPhatBangByCccd(string cccd)
        {
            var filter = Builders<HocSinhCapPhatBangViewModel>.Filter.And(
                           Builders<HocSinhCapPhatBangViewModel>.Filter.Eq(hs => hs.Xoa, false),
                           Builders<HocSinhCapPhatBangViewModel>.Filter.Eq(hs => hs.CCCD, cccd)
                         );

            var khoaThiCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhCapPhatBangViewModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongViewModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
            var collectionHTDT = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);
            var collectionHDT = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao);
            var collectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(_collectionNameDanhMucTotNghiep);
            var collectionNamThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);

            var truong = truongCollection.Find(t => t.Xoa == false).ToList()
                        .Join(
                              collectionHDT.AsQueryable(),
                              t => t.MaHeDaoTao,
                              hdt => hdt.Ma,
                              (t, hdt) =>
                              {
                                  t.HeDaoTao = hdt.Ten;
                                  return t;
                              }
                          )
                        .Join(
                              collectionHTDT.AsQueryable(),
                              t => t.MaHinhThucDaoTao,
                              htdt => htdt.Ma,
                              (t, htdt) =>
                              {
                                  t.HinhThucDaoTao = htdt.Ten;
                                  return t;
                              }
                          ).ToList();


            var danhMucTotNghieps = collectionDanhMucTotNghiep.Find(t => t.Xoa == false).ToList()
                        .Join(
                              collectionNamThi.AsQueryable(),
                              d => d.IdNamThi,
                              n => n.Id,
                              (d, n) =>
                              {
                                  d.NamThi = n.Ten;
                                  return d;
                              }
                          ).ToList();


            var hocSinhs = hocSinhCollection.Find(filter).ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var hocSinh = hocSinhs
                      .Join(
                          truong.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.HeDaoTao = truong.HeDaoTao;
                              hs.HinhThucDaoTao = truong.HinhThucDaoTao;
                              hs.Truong = truong.Ten;
                              return hs;
                          }
                      )
                      .Join(
                          danhMucTotNghieps.AsQueryable(),
                          hs => hs.IdDanhMucTotNghiep,
                          d => d.Id,
                          (hs, d) =>
                          {
                              hs.NamThi = d.NamThi;
                              hs.NgayCapBang = d.NgayCapBang;
                              return hs;
                          }
                      )
                      .GroupJoin(
                          soGocCollection.AsQueryable(),
                          hs => hs.IdSoGoc,
                          s => s.Id,
                          (hs, sGroup) =>
                          {
                              var soGoc = sGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                              if (soGoc == null)
                              {
                                  hs.SoGoc = new SoGocModel(); // Create a new instance of SoGocModel
                              }
                              else
                              {
                                  hs.SoGoc = soGoc;
                              }
                              return hs;
                          }
                      )
                      .FirstOrDefault();
            return hocSinh;
        }
        #endregion

        #region Cổng thông tin
        public List<HocSinhViewModel> GetSearchVBCC(out int total, TraCuuVBCCModel modelSearch)
        {
            var filterBuilder = Builders<HocSinhViewModel>.Filter;
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);

            var filters = new List<FilterDefinition<HocSinhViewModel>>
            {
                filterBuilder.In("TrangThai", new List<TrangThaiHocSinhEnum>
                {
                   TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                   TrangThaiHocSinhEnum.DaInBang,
                   TrangThaiHocSinhEnum.DaNhanBang,
                }),

                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("HoTen", modelSearch.HoTen),

                !string.IsNullOrEmpty(modelSearch.Cccd)
                    ? filterBuilder.Eq("CCCD", modelSearch.Cccd)
                    : null,
                !string.IsNullOrEmpty(modelSearch.SoHieuVanBang)
                    ? filterBuilder.Eq("SoHieuVanBang", modelSearch.SoHieuVanBang)
                    : null,
            };

            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var hocSinhs = hocSinhCollection

                        .Find(combinedFilter)
                        .ToList();

            var ngaySinh = modelSearch.NgaySinh.ToUniversalTime().Date;

            var hocSinh = hocSinhs
                        .Where(x=>x.NgaySinh.Date == modelSearch.NgaySinh.Date ||
                                x.NgaySinh.Date == ngaySinh)
                         .Join(
                             truongCollection.AsQueryable(),
                             hs => hs.IdTruong,
                             truong => truong.Id,
                             (hs, truong) =>
                             {
                                 hs.Truong = truong;
                                 return hs;
                             }
                         )
                         .Join(
                             danhMucTotNghiepCollection.AsQueryable(),
                             hs => hs.IdDanhMucTotNghiep,
                             dmtn => dmtn.Id,
                             (hs, dmtn) =>
                             {
                                 hs.DanhMucTotNghiep = dmtn;
                                 return hs;
                             }
                         )
                         .GroupJoin(
                             soGocCollection.AsQueryable(),
                             hs => hs.IdSoGoc,
                             s => s.Id,
                             (hs, sGroup) =>
                             {
                                 var soGoc = sGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                                 if (soGoc == null)
                                 {
                                     hs.SoGoc = new SoGocModel(); // Create a new instance of SoGocModel
                                 }
                                 else
                                 {
                                     hs.SoGoc = soGoc;
                                 }
                                 return hs;
                             }
                         )
                         .Where(h=>h.DanhMucTotNghiep.IdNamThi == modelSearch.IdNamThi).ToList();

            total = hocSinh.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.HoTen.Split(' ').LastOrDefault()).ToList()
                        : hocSinh.OrderByDescending(x => x.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
                case "1":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.HoTen.Split(' ').LastOrDefault()).ToList()
                        : hocSinh.OrderByDescending(x => x.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
            }

            if (modelSearch.PageSize >= 0)
            {
                hocSinh = hocSinh.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinh;
        }
        #endregion

        #region Tra Cứu
        public string GetSearchHocSinhXacMinhVanBang(HocSinhSearchXacMinhVBModel modelSearch)
        {
            //Phân trang
            int skip = ((modelSearch.StartIndex - 1) * modelSearch.PageSize) + modelSearch.PageSize;
            string pagination = modelSearch.PageSize < 0 ? $@"hocSinhs: '$hocSinhs'" : $@"hocSinhs: {{ $slice: ['$hocSinhs', {skip}, {modelSearch.PageSize}] }},";
            // Sắp xếp
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");

            ////Tìm kiếm và lọc
            string matchHoTen = string.IsNullOrEmpty(modelSearch.HoTen) ? "" : $@"'HoTen': '{modelSearch.HoTen}',";
            string matchCCCD = string.IsNullOrEmpty(modelSearch.CCCD) ? "" : $@"'CCCD': '{modelSearch.CCCD}',";
            string matchNoiSinh = string.IsNullOrEmpty(modelSearch.NoiSinh) ? "" : $@"'NoiSinh': '{modelSearch.NoiSinh}',";
            string matchIdDanhMucTotNghiep = string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep) ? "" : $@"'IdDanhMucTotNghiep': '{modelSearch.IdDanhMucTotNghiep}',";
            string matchIdTruong = string.IsNullOrEmpty(modelSearch.IdTruong) ? "" : $@"'IdTruong': '{modelSearch.IdTruong}',";
            string matchDanToc = string.IsNullOrEmpty(modelSearch.IdTruong) ? "" : $@"'DanToc': '{modelSearch.DanToc}',";


            string matchIdNamThi = string.IsNullOrEmpty(modelSearch.IdNamThi) ? "" : $@"'DanhMucTotNghiep.IdNamThi': '{modelSearch.IdNamThi}',";
            string matchIdHinhThucDaoTao = string.IsNullOrEmpty(modelSearch.IdHinhThucDaoTao) ? "" : $@"'DanhMucTotNghiep.IdHinhThucDaoTao': '{modelSearch.IdHinhThucDaoTao}',";

            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $match: {{
                                      Xoa: false,
                                      TrangThai: {{ $in: [3, 4, 5, 6] }},
                                      {matchHoTen}
                                      {matchCCCD}
                                      {matchNoiSinh}
                                     {matchIdDanhMucTotNghiep}
                                    {matchIdTruong}
                                    {matchDanToc}
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                      IdTruong: {{ $toObjectId: '$IdTruong' }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: 'DanhMucTotNghiep',
                                      localField: 'IdDanhMucTotNghiep',
                                      foreignField: '_id',
                                      as: 'DanhMucTotNghieps',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: 'Truong',
                                      localField: 'IdTruong',
                                      foreignField: '_id',
                                      as: 'Truongs',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      Truong: {{ $arrayElemAt: ['$Truongs', 0] }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      Truongs: 0,
                                    }},
                                  }},
                                 {{
                                    $match: {{
                                         {matchIdNamThi}
                                         {matchIdHinhThucDaoTao}
                                    }},
                                  }},
                                    {order}
                                  {{
                                    $group: {{
                                      _id: null,
                                      totalRow: {{ $sum: 1 }},
                                      hocSinhs: {{
                                        $push: {{
                                          id: '$_id',
                                          hoTen: '$HoTen',
                                          noiSinh: '$NoiSinh',
                                          cccd: '$CCCD',
                                          gioiTinh: '$GioiTinh',
                                          ngaySinh: '$NgaySinh',
                                          soHieuVanBang: '$SoHieuVanBang',
                                          soVaoSoCapBang: '$SoVaoSoCapBang',
                                          trangThai: '$TrangThai',
                                          tenTruong: '$Truong.Ten',
                                          tieuDe: '$DanhMucTotNghiep.TieuDe',
                                          namThi: '$DanhMucTotNghiep.IdNamThi',
                                          idKhoaThi: '$IdKhoaThi',
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

            return json;
        }

        /// <summary>
        /// Lấy danh sách học sinh đang chờ duyệt (chức năng cấp bằng)
        /// (Phòng giáo dục và đào tạo)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public List<HocSinhXMVBModel> GetSearchHocSinhXacMinhVB(out int total, HocSinhSearchXacMinhVBModel modelSearch, string idDonVi)
        {
            var truongs = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                           .Find(x=>x.Xoa == false && x.IdCha == idDonVi)
                           .ToList();
            var truongIds = truongs.Select(x => x.Id).ToList();

            var filterBuilder = Builders<HocSinhXMVBModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhXMVBModel>>
            {
                 filterBuilder.In("TrangThai", new List<TrangThaiHocSinhEnum>
                {
                        TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                        TrangThaiHocSinhEnum.DaCapBang,
                        TrangThaiHocSinhEnum.DaInBang,
                        TrangThaiHocSinhEnum.DaNhanBang,
                        TrangThaiHocSinhEnum.HuyBo
                }),

                !string.IsNullOrEmpty(modelSearch.HoTen)
                    ? filterBuilder.Regex("HoTen", new BsonRegularExpression(modelSearch.HoTen, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep)
                    ? filterBuilder.Eq("IdDanhMucTotNghiep", modelSearch.IdDanhMucTotNghiep)
                    : null,
                !string.IsNullOrEmpty(modelSearch.IdTruong)
                    ? filterBuilder.Eq("IdTruong", modelSearch.IdTruong)
                    : filterBuilder.In("IdTruong", truongIds),
                !string.IsNullOrEmpty(modelSearch.DanToc)
                    ? filterBuilder.Regex("DanToc", new BsonRegularExpression(modelSearch.DanToc, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.NoiSinh)
                    ? filterBuilder.Regex("NoiSinh", new BsonRegularExpression(modelSearch.NoiSinh, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.CCCD)
                    ? filterBuilder.Regex("CCCD", new BsonRegularExpression(modelSearch.CCCD, "i"))
                    : null,

                 !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Or(
                        filterBuilder.Regex("HoTen", new BsonRegularExpression(modelSearch.Search, "i")),
                        filterBuilder.Regex("DanToc", new BsonRegularExpression(modelSearch.Search, "i")),
                        filterBuilder.Regex("CCCD", new BsonRegularExpression(modelSearch.Search, "i")),
                        filterBuilder.Regex("SoHieuVanBang", new BsonRegularExpression(modelSearch.Search, "i")),
                        filterBuilder.Regex("SoVaoSoCapBang", new BsonRegularExpression(modelSearch.Search, "i"))
                    )
                    : null
            };


            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var hocSinhs = _mongoDatabase.GetCollection<HocSinhXMVBModel>(_collectionHocSinhName)
                                .Find(combinedFilter)
                                .ToList();

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);


            var result = hocSinhs
                         .Join(
                             danhMucTotNghiepCollection.AsQueryable(),
                             hs => hs.IdDanhMucTotNghiep,
                             dmtn => dmtn.Id,
                             (hs, dmtn) =>
                             {
                                 hs.DanhMucTotNghiep = dmtn;
                                 return hs;
                             }
                         )
                         .Join(
                             namThiCollection.AsQueryable(),
                             hs => hs.DanhMucTotNghiep.IdNamThi,
                             n => n.Id,
                             (hs, n) =>
                             {
                                 hs.NamThi = n;
                                 hs.KhoaThi = n.KhoaThis.Where(k => k.Id == hs.IdKhoaThi).FirstOrDefault().Ngay;
                                 return hs;
                             }
                         ).ToList();

            if (!string.IsNullOrEmpty(modelSearch.IdNamThi))
            {
               result = result.Where(h => h.DanhMucTotNghiep.IdNamThi == modelSearch.IdNamThi).ToList();
            }

            total = result.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                       ? result.OrderBy(x => x.HoTen.Split(' ').LastOrDefault()).ToList()
                       : result.OrderByDescending(x => x.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
                case "1":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? result.OrderBy(x => x.HoTen.Split(' ').LastOrDefault()).ToList()
                        : result.OrderByDescending(x => x.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
                case "hoTen":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                       ? result.OrderBy(x => x.HoTen.Split(' ').LastOrDefault()).ToList()
                       : result.OrderByDescending(x => x.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
                case "cccd":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                       ? result.OrderBy(x => x.CCCD).ToList()
                       : result.OrderByDescending(x =>x.CCCD).ToList();
                    break;
                case "gioiTinh_fm":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                       ? result.OrderBy(x => x.GioiTinh).ToList()
                       : result.OrderByDescending(x => x.GioiTinh).ToList();
                    break;
                case "ngaySinh_fm":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                       ? result.OrderBy(x => x.NgaySinh).ToList()
                       : result.OrderByDescending(x => x.NgaySinh).ToList();
                    break;
                case "soHieuVanBang":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                       ? result.OrderBy(x => x.SoHieuVanBang).ToList()
                       : result.OrderByDescending(x => x.SoHieuVanBang).ToList();
                    break;
                case "soVaoSoCapBang":
                    result = modelSearch.OrderDir.ToUpper() == "ASC"
                       ? result.OrderBy(x => x.SoHieuVanBang).ToList()
                       : result.OrderByDescending(x => x.NgaySinh).ToList();
                    break;

            }
            if (modelSearch.PageSize > 0)
            {
                result = result.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return result;
        }

        public List<HocSinhModel> GetAllHocSinhDaCoSoHieu()
        {
            var filterBuilder = Builders<HocSinhModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhModel>>
            {
                 filterBuilder.In("TrangThai", new List<TrangThaiHocSinhEnum>
                {
                        TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                        TrangThaiHocSinhEnum.DaCapBang,
                        TrangThaiHocSinhEnum.DaInBang,
                        TrangThaiHocSinhEnum.DaNhanBang
                }),
            };

            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var hocSinhs = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                                .Find(combinedFilter)
                                .ToList();
            return hocSinhs;
        }

        public async Task<HocSinhResult> SaveImport(string idTruong, string idDanhMucTotNghiep ,List<HocSinhModel> models)
        {
            try
            {
                var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionDMTN = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
                var conllectionSoGoc = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);


                var conllectionTruong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

                var truong = conllectionTruong.Find(dm => dm.Xoa == false && dm.Id == idTruong).FirstOrDefault();
                var dmtn = conllectionDMTN.Find(dm => dm.Xoa == false && dm.Id == idDanhMucTotNghiep).FirstOrDefault();



                if (truong == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistTruong };
                if (dmtn == null)
                    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistDanhMucTotNghiep };

                var soGoc = conllectionSoGoc.Find(dm => dm.Xoa == false && dm.IdNamThi == dmtn.IdNamThi).FirstOrDefault();


                //var phoiGoc = conllectionPhoiGoc.Find(pg => pg.Xoa == false
                //&& pg.TinhTrang == TinhTrangPhoiEnum.DangSuDung && pg.MaHeDaoTao == truong.MaHeDaoTao).FirstOrDefault();
                //if (phoiGoc == null)
                //    return new HocSinhResult() { MaLoi = (int)HocSinhEnum.NotExistPhoi };

                int maxSTT = conllectionHocSinh.Find(x => x.Xoa == false && x.IdTruong == idTruong)
                                    .Sort(Builders<HocSinhModel>.Sort.Descending(x => x.STT))
                                    .Limit(1)
                                    .FirstOrDefault()?.STT ?? 0;

                // Increment STT for new entries
                foreach (var model in models)
                {
                    model.STT = ++maxSTT;
                    //model.IdPhoiGoc = phoiGoc.Id;
                    model.IdSoGoc = soGoc.Id;
                }

                conllectionHocSinh.InsertMany(models);
                return new HocSinhResult { MaLoi = (int)HocSinhEnum.Success };
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)HocSinhEnum.Fail };
            }
        }

        #endregion

        #region Trang Chủ


        /// <summary>
        /// Lấy học sinh theo cccd 
        /// </summary>
        /// <param name="cccd"></param>
        /// <returns></returns>
        public ThongKeSoLuongBangModel GetThongKeSoLuongHocSinhNhanBang(string idTruong)
        {
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

            var truong = truongCollection.Find(x => x.Xoa == false && x.Id == idTruong).FirstOrDefault();



            if (truong.LaPhong == true)
            {
                var filterSoLuongChuaPhat = Builders<HocSinhModel>.Filter.And(
                         Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                         Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaInBang)
                       );

                var filterSoLuongDaPhat = Builders<HocSinhModel>.Filter.And(
                          Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                          Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaNhanBang)
                        );
                int slHocSinhChuaPhat = hocSinhCollection.Find(filterSoLuongChuaPhat).ToList().Count();
                int slHocSinhDaPhat = hocSinhCollection.Find(filterSoLuongDaPhat).ToList().Count();
                var result = new ThongKeSoLuongBangModel()
                {
                    SoLuongChuaPhat = slHocSinhChuaPhat,
                    SoLuongDaPhat = slHocSinhDaPhat

                };

                return result;
            }
            else
            {
                var filterSoLuongChuaPhat = Builders<HocSinhModel>.Filter.And(
                        Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                        Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaInBang),
                        Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong)

                      );

                var filterSoLuongDaPhat = Builders<HocSinhModel>.Filter.And(
                          Builders<HocSinhModel>.Filter.Eq(hs => hs.Xoa, false),
                          Builders<HocSinhModel>.Filter.Eq(hs => hs.TrangThai, TrangThaiHocSinhEnum.DaNhanBang),
                          Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong)
                        );
                int slHocSinhChuaPhat = hocSinhCollection.Find(filterSoLuongChuaPhat).ToList().Count();
                int slHocSinhDaPhat = hocSinhCollection.Find(filterSoLuongDaPhat).ToList().Count();
                var result = new ThongKeSoLuongBangModel()
                {
                    SoLuongChuaPhat = slHocSinhChuaPhat,
                    SoLuongDaPhat = slHocSinhDaPhat

                };

                return result;
            }
        }

        private int GetTotalByIdNamThi(string idNamThi)
        {
            var colectionDonYeuCauCapBanSao = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(_collectionDonYeuCauCapBanSaoName);
            int total = colectionDonYeuCauCapBanSao.Find(x => x.Xoa == false && x.IdNamThi == idNamThi).ToList().Count();
            return total;
        }

        private int GetTotalTruongDaGui(string idNamThi)
        {
            var colectionDanhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);

            // Lọc danh sách theo điều kiện: không bị xóa và thuộc cùng IdNamThi
            var filteredList = colectionDanhMucTotNghiep
                .Find(x => !x.Xoa && x.IdNamThi == idNamThi)
                .ToList();

            // Tính tổng số trường đã duyệt
            int totalTruongDaDuyet = filteredList.Sum(x => x.TongSoTruongDaGui);

            return totalTruongDaDuyet;
        }

        public ThongKeHocSinhTongQuatModel GetThongKeHocSinhTongQuat(string idTruong, string idNamThi)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var phoiGocCollection = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);


            var truong = truongCollection.Find(x => x.Xoa == false && x.Id == idTruong).FirstOrDefault();
           
           
            if (truong.LaPhong == true)
            {
                var filter = Builders<HocSinhViewModel>.Filter.And(
                        Builders<HocSinhViewModel>.Filter.Eq(hs => hs.Xoa, false)
                      );

                var hocSinhs = hocSinhCollection.Find(filter)
                               .ToList()
                                .Join(
                                  danhMucTotNghiepCollection.AsQueryable(),
                                  hs => hs.IdDanhMucTotNghiep,
                                  dmtn => dmtn.Id,
                                  (hs, dmtn) =>
                                  {
                                      hs.DanhMucTotNghiep = dmtn;
                                      return hs;
                                  }
                              ).ToList();
                int tongSoHSDaDuyet = hocSinhs
                    .Where(h => h.TrangThai >= TrangThaiHocSinhEnum.DaDuyet && h.DanhMucTotNghiep.IdNamThi == idNamThi)
                    .Count();
                int tongSoPhoiDaIn = hocSinhs
                                .Where(h => !string.IsNullOrEmpty(h.IdPhoiGoc) && h.DanhMucTotNghiep.IdNamThi == idNamThi)
                                .Count();

                var result = new ThongKeHocSinhTongQuatModel()
                {
                    TongSoHocSinhDaDuyet = tongSoHSDaDuyet,
                    TongSoDonYeuCauCapBanSao = GetTotalByIdNamThi(idNamThi),
                    TongSoTruongDaGui = GetTotalTruongDaGui(idNamThi),
                    TongSoPhoiDaIn = tongSoPhoiDaIn
                };

                return result;
            } 
            else
            {
                var filter = Builders<HocSinhViewModel>.Filter.And(
                      Builders<HocSinhViewModel>.Filter.Eq(hs => hs.Xoa, false),
                      Builders<HocSinhViewModel>.Filter.Eq(hs => hs.IdTruong, idTruong)
                    );
                var hocSinhs = hocSinhCollection.Find(filter)
                               .ToList()
                                .Join(
                                  danhMucTotNghiepCollection.AsQueryable(),
                                  hs => hs.IdDanhMucTotNghiep,
                                  dmtn => dmtn.Id,
                                  (hs, dmtn) =>
                                  {
                                      hs.DanhMucTotNghiep = dmtn;
                                      return hs;
                                  }
                              ).ToList();
                int tongSoHS = hocSinhs.Count();
                int tongSoHSChoDuyet = hocSinhs.Where(h => h.TrangThai == TrangThaiHocSinhEnum.ChoDuyet && h.DanhMucTotNghiep.IdNamThi == idNamThi).Count();
                int tongSoHSDaDuyet = hocSinhs.Where(h => h.TrangThai >= TrangThaiHocSinhEnum.DaDuyet && h.DanhMucTotNghiep.IdNamThi == idNamThi).Count();
                int tongSoHSDaNhanBnag = hocSinhs.Where(h => h.TrangThai == TrangThaiHocSinhEnum.DaNhanBang && h.DanhMucTotNghiep.IdNamThi == idNamThi).Count();
                var result = new ThongKeHocSinhTongQuatModel()
                {
                    TongSoHocSinh = tongSoHS,
                    TongSoHocSinhChoDuyet = tongSoHSChoDuyet,
                    TongSoHocSinhDaDuyet = tongSoHSDaDuyet,
                    TongSoHocSinhDaNhanBang = tongSoHSDaNhanBnag,
                };

                return result;
            }  
        }



        public List<ThongKeSoLuongXepLoaiTheoNamModel> ThongKeSoLuongXepLoaiTheoNam(string idTruong)
        {
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var dmtnCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(_collectionNameDanhMucTotNghiep);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

            var truong = truongCollection.Find(x => x.Xoa == false && x.Id == idTruong).FirstOrDefault();

            var danhMucTotNghieps = dmtnCollection.Find(t => t.Xoa == false).ToList()
                .Join(
                    namThiCollection.AsQueryable(),
                    d => d.IdNamThi,
                    n => n.Id,
                    (d, n) =>
                    {
                        d.NamThi = n.Ten;
                        return d;
                    }
                ).ToList();

            var thongKeList = new List<ThongKeSoLuongXepLoaiTheoNamModel>();
            foreach (var dmtn in danhMucTotNghieps)
            {
                var hocSinhList = hocSinhCollection
                  .Find(hs => hs.Xoa == false && hs.IdDanhMucTotNghiep == dmtn.Id && hs.TrangThai > TrangThaiHocSinhEnum.ChuaXacNhan).ToList();
                if (truong.LaPhong == false)
                {
                    hocSinhList = hocSinhList.Where(h => h.IdTruong == idTruong).ToList();
                }
              

                var thongKeNam = thongKeList.FirstOrDefault(tk => tk.NamThi == dmtn.NamThi);

                if (thongKeNam == null)
                {
                    thongKeNam = new ThongKeSoLuongXepLoaiTheoNamModel
                    {
                        NamThi = dmtn.NamThi,
                        SoLuongHocSinhGioi = hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.Gioi.ToStringValue()),
                        SoLuongHocSinhKha = hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.Kha.ToStringValue()),
                        SoLuongHocSinhTrungBinh = hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.TrungBinh.ToStringValue()),
                        SoLuongHocSinhYeu = hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.Yeu.ToStringValue())
                    };
                    thongKeList.Add(thongKeNam);
                }
                else
                {
                    thongKeNam.SoLuongHocSinhGioi += hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.Gioi.ToStringValue());
                    thongKeNam.SoLuongHocSinhKha += hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.Kha.ToStringValue());
                    thongKeNam.SoLuongHocSinhTrungBinh += hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.TrungBinh.ToStringValue());
                    thongKeNam.SoLuongHocSinhYeu += hocSinhList.Count(hs => hs.XepLoai == LoaiXepLoatEnum.Yeu.ToStringValue());
                }
            }

            return thongKeList;
        }

        public List<ThongKeSoLuongHocSinhTheoNamModel> ThongKeSoLuongHocSinhTheoNam(string idTruong)
        {
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var dmtnCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(_collectionNameDanhMucTotNghiep);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

            var truong = truongCollection.Find(x => x.Xoa == false && x.Id == idTruong).FirstOrDefault();

            var danhMucTotNghieps = dmtnCollection.Find(t => t.Xoa == false).ToList()
                .Join(
                    namThiCollection.AsQueryable(),
                    d => d.IdNamThi,
                    n => n.Id,
                    (d, n) =>
                    {
                        d.NamThi = n.Ten;
                        return d;
                    }
                ).ToList();

            var thongKeList = new List<ThongKeSoLuongHocSinhTheoNamModel>();
            foreach (var dmtn in danhMucTotNghieps)
            {
                var hocSinhList = hocSinhCollection
                   .Find(hs => hs.Xoa == false && hs.IdDanhMucTotNghiep == dmtn.Id && hs.TrangThai > TrangThaiHocSinhEnum.ChuaXacNhan).ToList();
                if (truong.LaPhong == false)
                {
                    hocSinhList = hocSinhList.Where(h => h.IdTruong == idTruong).ToList();
                }

                var thongKeNam = thongKeList.FirstOrDefault(tk => tk.NamThi == dmtn.NamThi);

                if (thongKeNam == null)
                {
                    thongKeNam = new ThongKeSoLuongHocSinhTheoNamModel
                    {
                        NamThi = dmtn.NamThi,
                        SoLuong = hocSinhList.Count(),
                    };
                    thongKeList.Add(thongKeNam);
                }
                else
                {
                    thongKeNam.SoLuong += hocSinhList.Count();
                }
            }

            return thongKeList;
        }

        #endregion

        #region Private

        /// <summary>
        /// Hàm kiểm tra trùng cccd
        /// </summary>
        /// <param name="cccd"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        private bool TrungCCCD(string cccd, string Id)
        {
            var mt = String.IsNullOrEmpty(Id)
                ? _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).Find(n => true && n.Xoa == false && n.CCCD == cccd).FirstOrDefault()
                : _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName).Find(n => true && n.Xoa == false && n.CCCD == cccd && n.Id != Id).FirstOrDefault();
            return mt != null ? true : false;
        }

        private bool KiemTraTruongDaDuyetDanhSach(string idTruong, string idDanhMucTotNghiep)
        {
            var trangThais = new List<TrangThaiHocSinhEnum>
                {
                        TrangThaiHocSinhEnum.DaDuyet,
                        TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                        TrangThaiHocSinhEnum.DaInBang,
                };

            var filterHocSinh = Builders<HocSinhModel>.Filter.And(
                 Builders<HocSinhModel>.Filter.In(hs => hs.TrangThai, trangThais), // Trạng thái là "ChoGui"
                 Builders<HocSinhModel>.Filter.Eq(hs => hs.IdTruong, idTruong),
                 Builders<HocSinhModel>.Filter.Eq(hs => hs.IdDanhMucTotNghiep, idDanhMucTotNghiep)
             );

            var result = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName)
                .Find(filterHocSinh).FirstOrDefault();

            return result != null ? true : false;
        }

        private async Task<int> KiemTraHinhThucDaoTaoCoTonTai(string idTruong, string idDanhMucTotNghiep)
        {

            var colectionDMTN = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var colectionTruong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var colectionHTDT = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);

            var dmtn = await colectionDMTN.Find(d => d.Xoa == false && d.Id == idDanhMucTotNghiep).FirstOrDefaultAsync();
            var truong = await colectionTruong.Find(t => t.Xoa == false && t.Id == idTruong).FirstOrDefaultAsync();

            if (dmtn == null)
                return (int)HocSinhEnum.NotExistDanhMucTotNghiep;
            if (truong == null)
                return (int)HocSinhEnum.NotExistTruong;

            var htdt = await colectionHTDT.Find(h => h.Xoa == false && h.Id == dmtn.IdHinhThucDaoTao).FirstOrDefaultAsync();
            if (htdt == null)
                return (int)HocSinhEnum.NotExistHTDT;
            if (htdt.Ma != truong.MaHinhThucDaoTao)
                return (int)HocSinhEnum.NotMatchHtdt;

            return (int)HocSinhEnum.Success;
        }

        private bool KiemTraDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var dmtn = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                           .Find(h => h.Xoa == false && h.Id == idDanhMucTotNghiep).FirstOrDefault();
            return dmtn != null ? true : false;
        }

        private bool KiemTraDanToc(string ten)
        {
            var danToc = _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                                           .Find(h => h.Xoa == false && h.Ten == ten).FirstOrDefault();
            return danToc != null ? true : false;
        }

        private bool KiemTraTruong(string idTruong)
        {
            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                           .Find(h => h.Xoa == false && h.Id == idTruong).FirstOrDefault();
            return truong != null ? true : false;
        }

        private bool KiemTraKhoaThi(string idNamThi, string idKhoaThi)
        {
            var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                           .Find(h => h.Xoa == false && h.Id == idNamThi).FirstOrDefault();

            var khoaThi = namThi.KhoaThis.Where(k=>k.Xoa == false && k.Id == idKhoaThi).FirstOrDefault();

            return khoaThi != null ? true : false;
        }

        private CauHinhModel GetCauHinhByDonViQuanLY(string idTruong)
        {
            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(x => x.Id == idTruong).FirstOrDefault();
            var donViQL = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(x => x.Id == truong.IdCha).FirstOrDefault();


            return donViQL.CauHinh;
        }


        #endregion

    }

}
