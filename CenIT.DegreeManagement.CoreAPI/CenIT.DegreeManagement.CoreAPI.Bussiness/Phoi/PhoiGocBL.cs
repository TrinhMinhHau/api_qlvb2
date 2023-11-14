using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Phoi
{
    public class PhoiGocBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public PhoiGocBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        #region PhoGoc

        /// <summary>
        /// Thêm mới phôi gốc
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cauHinhPhoiGocs"></param>
        /// <returns></returns>
        public async Task<int> Create(PhoiGocInputModel model, List<CauHinhPhoiGocModel> cauHinhPhoiGocs)
        {
            var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);
            var conllectionPhoiGocLog = _mongoDatabase.GetCollection<PhoiGocLogModel>(_collectionPhoiGocLogName);

            if (TrungTenPhoi(model.TenPhoi, model.Id)) return (int)PhoiEnum.ExistName;
            if (TrungNgayApDung(model.NgayApDung, model.Id)) return (int)PhoiEnum.ExistDate;
            if (TrungSoHieuPhoi(model.SoHieuPhoi, model.Id)) return (int)PhoiEnum.ExistNumber;
            if (CheckPhoiDangSuDung(model.MaHeDaoTao)) return (int)PhoiEnum.ExistInUse;

            try
            {
                var phoiGocModel = new PhoiGocModel();
                ModelProvider.MapProperties(model, phoiGocModel);
                phoiGocModel.NgayTao = DateTime.Now;
                phoiGocModel.NguoiTao = model.NguoiThucHien;
                phoiGocModel.CauHinhPhoiGocs = cauHinhPhoiGocs;

                await conllectionPhoiGoc.InsertOneAsync(phoiGocModel);
                // Lấy ID sau khi thêm thành công
                var insertedId = phoiGocModel.Id.ToString();
                if(string.IsNullOrEmpty(insertedId))
                    return (int)PhoiEnum.Fail;
                
                //log
                var phoiGocLog = new PhoiGocLogModel();
                ModelProvider.MapProperties(phoiGocModel, phoiGocLog);
                phoiGocLog.IdPhoiGoc = insertedId;
                phoiGocLog.NguoiThucHien = model.NguoiThucHien;
                phoiGocLog.HanhDong = EnumExtensions.ToStringValue(HanhDongLogPhoiGocEnum.CREATE);
                await conllectionPhoiGocLog.InsertOneAsync(phoiGocLog);

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        /// <summary>
        /// Chỉnh sửa phôi gốc
        /// </summary>
        /// <param name="model"></param>
        /// <param name="lyDo"></param>
        /// <returns></returns>
        public async Task<int> Modify(PhoiGocInputModel model, string lyDo)
        {
            var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);
            var conllectionPhoiGocLog = _mongoDatabase.GetCollection<PhoiGocLogModel>(_collectionPhoiGocLogName);

            if (TrungTenPhoi(model.TenPhoi, model.Id)) return (int)PhoiEnum.ExistName;
            if (TrungNgayApDung(model.NgayApDung, model.Id)) return (int)PhoiEnum.ExistDate;
            if (TrungSoHieuPhoi(model.SoHieuPhoi, model.Id)) return (int)PhoiEnum.ExistNumber;

            try
            {
                var filter = Builders<PhoiGocModel>.Filter.And(
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Id, model.Id)
                 );

                var phoiGoc = conllectionPhoiGoc.Find(filter).FirstOrDefault();

                if (phoiGoc == null)
                    return (int)PhoiEnum.NotFound;

                ModelProvider.MapProperties(model, phoiGoc);
                phoiGoc.NgayCapNhat = DateTime.Now;
                phoiGoc.NguoiCapNhat = model.NguoiThucHien;


                var updateResult = await conllectionPhoiGoc.ReplaceOneAsync(filter, phoiGoc);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)PhoiEnum.Fail;
                }

                // log
                var phoiGocLog = new PhoiGocLogModel();
                ModelProvider.MapProperties(phoiGoc, phoiGocLog);
                phoiGocLog.IdPhoiGoc = phoiGoc.Id;
                phoiGocLog.NguoiThucHien = model.NguoiThucHien;
                phoiGocLog.HanhDong = EnumExtensions.ToStringValue(HanhDongLogPhoiGocEnum.UPDATE);
                phoiGocLog.LyDo = lyDo; 
                await conllectionPhoiGocLog.InsertOneAsync(phoiGocLog);

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        /// <summary>
        /// Xóa phôi gốc
        /// </summary>
        /// <param name="idPhoiGoc"></param>
        /// <param name="lyDo"></param>
        /// <returns></returns>
        public async Task<int> Delete(string idPhoiGoc, string nguoiThucHien, string lyDo)
        {
            try
            {
                var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);
                var conllectionPhoiGocLog = _mongoDatabase.GetCollection<PhoiGocLogModel>(_collectionPhoiGocLogName);
                var filter = Builders<PhoiGocModel>.Filter.And(
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Id, idPhoiGoc)
                 );

                var phoiGoc = conllectionPhoiGoc.Find(filter).FirstOrDefault();

                if (phoiGoc == null)
                    return (int)PhoiEnum.NotFound;

                phoiGoc.Xoa = true;
                phoiGoc.NgayXoa = DateTime.Now;
                phoiGoc.NguoiXoa = nguoiThucHien;


                var updateResult = await conllectionPhoiGoc.ReplaceOneAsync(filter, phoiGoc);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)PhoiEnum.Fail;
                }

                // log
                var phoiGocLog = new PhoiGocLogModel();
                ModelProvider.MapProperties(phoiGoc, phoiGocLog);
                phoiGocLog.IdPhoiGoc = phoiGoc.Id;
                phoiGocLog.NguoiThucHien = nguoiThucHien;
                phoiGocLog.HanhDong = EnumExtensions.ToStringValue(HanhDongLogPhoiGocEnum.DELETE);
                phoiGocLog.LyDo = lyDo;
                await conllectionPhoiGocLog.InsertOneAsync(phoiGocLog);

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        /// <summary>
        /// Lấy phôi gốc theo id
        /// </summary>
        /// <param name="idPhoi"></param>
        /// <returns></returns>
        public PhoiGocModel GetById(string idPhoi)
        {
            var filter = Builders<PhoiGocModel>.Filter.And(
                Builders<PhoiGocModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                Builders<PhoiGocModel>.Filter.Eq(dmtn => dmtn.Id, idPhoi)
              );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                        .Find(filter).FirstOrDefault();
            return phoiGoc;
        }

        /// <summary>
        /// Lấy danh sách phôi gốc (tìm kiếm)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<PhoiGocModel> GetSearchPhoiGoc(out int total, SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<PhoiGocModel>.Filter;

            var filters = new List<FilterDefinition<PhoiGocModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Eq("TenPhoi", modelSearch.Search)
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                .Find(combinedFilter).ToList();

            total = phoiGoc.Count();

            switch (modelSearch.Order)
            {
                case "0":
                    phoiGoc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phoiGoc.OrderBy(x => x.TenPhoi.Split(' ').LastOrDefault()).ToList()
                        : phoiGoc.OrderByDescending(x => x.TenPhoi.Split(' ').LastOrDefault()).ToList();
                    break;
                case "1":
                    phoiGoc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phoiGoc.OrderBy(x => x.NgayApDung).ToList()
                        : phoiGoc.OrderByDescending(x => x.NgayApDung).ToList();
                    break;
            }
            phoiGoc = phoiGoc.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return phoiGoc;
        }

        /// <summary>
        /// Lấy tất cả phôi gốc (tìm kiếm)
        /// </summary>
        /// <returns></returns>
        public List<PhoiGocModel> GetAll()
        {
            var filterBuilder = Builders<PhoiGocModel>.Filter;

            var filters = new List<FilterDefinition<PhoiGocModel>>
            {
                filterBuilder.Eq("Xoa", false)
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var phoiGocs = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                .Find(combinedFilter).ToList();

            return phoiGocs;
        }

        /// <summary>
        /// Thêm hoạt cập nhật tất cả cấu hình phôi
        /// </summary>
        /// <param name="idPhoiGoc"></param>
        /// <param name="cauHinhPhoiGocModels"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> CauHinhPhoiGoc(string idPhoiGoc, string nguoiThucHien, List<CauHinhPhoiGocModel> cauHinhPhoiGocModels)
        {
            try
            {
                var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);
                var conllectionPhoiGocLog = _mongoDatabase.GetCollection<PhoiGocLogModel>(_collectionPhoiGocLogName);

                var filter = Builders<PhoiGocModel>.Filter.And(
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Id, idPhoiGoc)
                 );

                var phoiGoc = conllectionPhoiGoc.Find(filter).FirstOrDefault();

                if (phoiGoc == null)
                    return (int)PhoiEnum.NotFound;

                foreach (var cauHinh in cauHinhPhoiGocModels)
                {
                     Guid guid = Guid.NewGuid();
                    cauHinh.Id = guid.ToString();
                }

                phoiGoc.CauHinhPhoiGocs = cauHinhPhoiGocModels;


                var updateResult = await conllectionPhoiGoc.ReplaceOneAsync(filter, phoiGoc);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)PhoiEnum.Fail;
                }

                // log
                var phoiGocLog = new PhoiGocLogModel();
                phoiGoc.CauHinhPhoiGocs = cauHinhPhoiGocModels;
                phoiGocLog.IdPhoiGoc = phoiGoc.Id;
                phoiGocLog.NguoiThucHien = nguoiThucHien;
                phoiGocLog.HanhDong = EnumExtensions.ToStringValue(HanhDongLogPhoiGocEnum.UPDATE_CONFIG);
                await conllectionPhoiGocLog.InsertOneAsync(phoiGocLog);

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        /// <summary>
        /// Hủy phôi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> HuyPhoi(HuyPhoiGocInputModel model)
        {
            try
            {
                var filter = Builders<PhoiGocModel>.Filter.And(
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiGocModel>.Filter.Eq(pg => pg.Id, model.Id)
                 );

                var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                            .Find(filter).FirstOrDefault();

                if (phoiGoc == null)
                    return (int)PhoiEnum.NotFound;

                ModelProvider.MapProperties(model, phoiGoc.BienBanHuyPhoi);
                phoiGoc.TinhTrang = TinhTrangPhoiEnum.DaHuy;
                phoiGoc.BienBanHuyPhoi.NgayHuy = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).ReplaceOneAsync(filter, phoiGoc);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)PhoiEnum.Fail;
                }
                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        #endregion

        #region CẤU HÌNH PHÔI
        /// <summary>
        /// Lấy danh sách cấu hình phôi
        /// </summary>
        /// <param name="idphoiGoc"></param>
        /// <returns></returns>
        public List<CauHinhPhoiGocModel> GetCauHinhPhoiGoc(string idphoiGoc)
        {
            var filter = Builders<PhoiGocModel>.Filter.And(
                    Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
                    Builders<PhoiGocModel>.Filter.Eq(pg => pg.Id, idphoiGoc)
                  );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                        .Find(filter).FirstOrDefault();

            if (phoiGoc == null)
                return null;

            var cauHinhPhoi = phoiGoc.CauHinhPhoiGocs.ToList();

            return cauHinhPhoi;
        }

        /// <summary>
        /// Lấy thông tin cấu hình phôi theo id
        /// </summary>
        /// <param name="idphoiGoc"></param>
        /// <param name="idCauHinhPhoi"></param>
        /// <returns></returns>
        public CauHinhPhoiGocModel? GetCauHinhPhoiGocById(string idphoiGoc, string idCauHinhPhoi)
        {
            var filter = Builders<PhoiGocModel>.Filter.And(
                    Builders<PhoiGocModel>.Filter.Eq(x => x.Xoa, false),
                    Builders<PhoiGocModel>.Filter.Eq(x => x.Id, idphoiGoc),
                    Builders<PhoiGocModel>.Filter.ElemMatch(x => x.CauHinhPhoiGocs,
                        Builders<CauHinhPhoiGocModel>.Filter.And(
                                Builders<CauHinhPhoiGocModel>.Filter.Eq(x => x.Id, idCauHinhPhoi)
                        )
                    )
                );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).Find(filter).FirstOrDefault();
            return phoiGoc != null ? phoiGoc.CauHinhPhoiGocs.Find(x => x.Id == idCauHinhPhoi) : null;
        }

        /// <summary>
        /// Cập nhật cấu hình phôi
        /// </summary>
        /// <param name="idphoiGoc"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> ModifyCauHinhPhoiGoc(string idphoiGoc, CauHinhPhoiGocInputModel model)
        {
            try
            {
                var filter = Builders<PhoiGocModel>.Filter.Eq(p => p.Id, idphoiGoc);

                var update = Builders<PhoiGocModel>.Update
                    .Set("CauHinhPhoiGocs.$[f].KieuChu", model.KieuChu)
                    .Set("CauHinhPhoiGocs.$[f].CoChu", model.CoChu)
                    .Set("CauHinhPhoiGocs.$[f].DinhDangKieuChu", model.DinhDangKieuChu)
                    .Set("CauHinhPhoiGocs.$[f].MauChu", model.MauChu)
                    .Set("CauHinhPhoiGocs.$[f].ViTriTren", model.ViTriTren)
                    .Set("CauHinhPhoiGocs.$[f].ViTriTrai", model.ViTriTrai);

                var arrayFilters = new[]
                        {
                            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                new BsonDocument("f._id",model.Id))
                        };

                UpdateResult u = await _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).UpdateOneAsync(filter, update, new UpdateOptions() { ArrayFilters = arrayFilters });
                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
            
        }

        /// <summary>
        /// Lấy danh sách các phôi gốc đã hủy
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<PhoiGocModel> GetSearchPhoiDaHuy(out int total ,SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<PhoiGocModel>.Filter;

            var filters = new List<FilterDefinition<PhoiGocModel>>
            {
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("TinhTrang", TinhTrangPhoiEnum.DaHuy),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Eq("TenPhoi", modelSearch.Search)
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                .Find(combinedFilter).ToList();

            total = phoiGoc.Count();

            switch (modelSearch.Order)
            {
                case "0":
                    phoiGoc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phoiGoc.OrderBy(x => x.TenPhoi.Split(' ').LastOrDefault()).ToList()
                        : phoiGoc.OrderByDescending(x => x.TenPhoi.Split(' ').LastOrDefault()).ToList();
                    break;
                case "1":
                    phoiGoc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phoiGoc.OrderBy(x => x.Id).ToList()
                        : phoiGoc.OrderByDescending(x => x.Id).ToList();
                    break;
            }
            phoiGoc = phoiGoc.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return phoiGoc;
        }

        /// <summary>
        /// Lấy thông tin phôi đang sử dụng
        /// </summary>
        /// <returns></returns>
        public PhoiGocModel GetPhoiDangSuDung(string idTruong)
        {
            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(t => t.Xoa == false && t.Id == idTruong).FirstOrDefault();


            var filter = Builders<PhoiGocModel>.Filter.And(
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.TinhTrang, TinhTrangPhoiEnum.DangSuDung),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.MaHeDaoTao, truong.MaHeDaoTao)
            );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                        .Find(filter).FirstOrDefault();
            return phoiGoc;
        }

        public PhoiGocModel GetPhoiDangSuDungByHDT(string maHeDaoTao)
        {

            var filter = Builders<PhoiGocModel>.Filter.And(
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.TinhTrang, TinhTrangPhoiEnum.DangSuDung),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.MaHeDaoTao, maHeDaoTao)
            );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                        .Find(filter).FirstOrDefault();
            return phoiGoc;
        }



        /// <summary>
        /// Cập nhật lại số bắt đầu
        /// </summary>
        /// <returns></returns>
        public async Task<int> CapNhatThongSoPhoi(string soBatDau, string idPhoi, int soluongphoi)
        {
            var filter = Builders<PhoiGocModel>.Filter.And(
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.Id, idPhoi)
            );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                        .Find(filter).FirstOrDefault();

            if (phoiGoc == null)
                return (int)PhoiEnum.NotFound;

            //phoiGoc.SoBatDau = soBatDau;
            phoiGoc.SoLuongPhoi -= soluongphoi;
            phoiGoc.SoLuongPhoiDaSuDung += soluongphoi;

            var updateResult = await _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).ReplaceOneAsync(filter, phoiGoc);
            if (updateResult.ModifiedCount == 0)
            {
                return (int)PhoiEnum.Fail;
            }
            return (int)PhoiEnum.Success;
        }

        /// <summary>
        /// Cập nhật lại số bắt đầu
        /// </summary>
        /// <returns></returns>
        public async Task<int> CapNhatSoLuongPhoiGoc(int soHocSinh, string idPhoi)
        {
            var filter = Builders<PhoiGocModel>.Filter.And(
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.Id, idPhoi)
            );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                        .Find(filter).FirstOrDefault();

            if (phoiGoc == null)
                return (int)PhoiEnum.NotFound;

            phoiGoc.SoLuongPhoi -= soHocSinh;
            phoiGoc.SoLuongPhoiDaSuDung += soHocSinh;

            var updateResult = await _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).ReplaceOneAsync(filter, phoiGoc);
            if (updateResult.ModifiedCount == 0)
            {
                return (int)PhoiEnum.Fail;
            }
            return (int)PhoiEnum.Success;
        }
        #endregion

        #region Thống kê phôi

       

        #endregion

        #region private
        public bool TrungTenPhoi(string tenPhoi, string idPhoiGoc)
        {
            var mt = idPhoiGoc == null
                ? _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).Find(n => true && n.Xoa == false && n.TenPhoi == tenPhoi).FirstOrDefault()
                : _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).Find(n => true && n.Xoa == false && n.TenPhoi == tenPhoi && n.Id != idPhoiGoc).FirstOrDefault();
            return mt != null ? true : false;
        }

        public bool TrungSoHieuPhoi(string soHieuPhoi, string idPhoiGoc)
        {
            var mt = idPhoiGoc == null
                ? _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).Find(n => true && n.Xoa == false && n.SoHieuPhoi == soHieuPhoi).FirstOrDefault()
                : _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).Find(n => true && n.Xoa == false && n.SoHieuPhoi == soHieuPhoi && n.Id != idPhoiGoc).FirstOrDefault();
            return mt != null ? true : false;
        }

        public bool CheckPhoiDangSuDung(string maHeDaoTao)
        {
            var filter = Builders<PhoiGocModel>.Filter.And(
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.Xoa, false),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.TinhTrang, TinhTrangPhoiEnum.DangSuDung),
               Builders<PhoiGocModel>.Filter.Eq(pg => pg.MaHeDaoTao, maHeDaoTao)
            );

            var existingPhoi = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc)
                                            .Find(filter).FirstOrDefault();

            return existingPhoi != null;
        }

        public bool TrungNgayApDung(DateTime ngayApDung, string idPhoiGoc)
        {
            var mt = idPhoiGoc == null
                ? _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).Find(n => true && n.Xoa == false && n.NgayApDung == ngayApDung).FirstOrDefault()
                : _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc).Find(n => true && n.Xoa == false && n.NgayApDung == ngayApDung && n.Id != idPhoiGoc).FirstOrDefault();
            return mt != null ? true : false;
        }

        public List<T> MapFromMongoDB<T>(object mongoResult)
        {
            dynamic result = mongoResult;

            List<T> resultList = new List<T>();

            foreach (var item in result)
            {
                T model = Activator.CreateInstance<T>();

                foreach (var prop in typeof(T).GetProperties())
                {
                    var propName = prop.Name;
                    prop.SetValue(model, item[propName]);
                }

                resultList.Add(model);
            }

            return resultList;
        }

        #endregion
    }
}
