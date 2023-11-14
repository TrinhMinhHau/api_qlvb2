using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Phoi
{
    public class PhoiBanSaoBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public PhoiBanSaoBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> Create(PhoiBanSaoInputModel model, List<CauHinhPhoiGocModel> cauHinhPhoiBanSaos)
        {
            var conllectionBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao);
            var conllectionBanSaoLog = _mongoDatabase.GetCollection<PhoiBanSaoLogModel>(_collectionPhoiBanSaoLogName);

            if (TrungTenPhoi(model.TenPhoi, model.Id)) return (int)PhoiEnum.ExistName;
            if (CheckPhoiDangSuDung(model.MaHeDaoTao)) return (int)PhoiEnum.ExistInUse;
            try
            {
                var phoiBanSaoModel = new PhoiBanSaoModel();
                ModelProvider.MapProperties(model, phoiBanSaoModel);
                phoiBanSaoModel.NgayTao = DateTime.Now;
                phoiBanSaoModel.NguoiTao = model.NguoiThucHien;
                phoiBanSaoModel.CauHinhPhoi = cauHinhPhoiBanSaos;

                await conllectionBanSao.InsertOneAsync(phoiBanSaoModel);
                // Lấy ID sau khi thêm thành công
                var insertedId = phoiBanSaoModel.Id.ToString();
                if (string.IsNullOrEmpty(insertedId))
                    return (int)PhoiEnum.Fail;

                var phoiBanSaoLog = new PhoiBanSaoLogModel();
                ModelProvider.MapProperties(phoiBanSaoModel, phoiBanSaoLog);
                phoiBanSaoLog.IdPhoiGoc = insertedId;
                phoiBanSaoLog.NguoiThucHien = model.NguoiThucHien;
                phoiBanSaoLog.HanhDong = EnumExtensions.ToStringValue(HanhDongLogPhoiGocEnum.CREATE);
                await conllectionBanSaoLog.InsertOneAsync(phoiBanSaoLog);

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        public async Task<int> Modify(PhoiBanSaoInputModel model, string lyDo)
        {
            var conllectionBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao);
            var conllectionBanSaoLog = _mongoDatabase.GetCollection<PhoiBanSaoLogModel>(_collectionPhoiBanSaoLogName);


            if (TrungTenPhoi(model.TenPhoi, model.Id)) return (int)PhoiEnum.ExistName;
            try
            {
                var filter = Builders<PhoiBanSaoModel>.Filter.And(
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Id, model.Id)
                 );

                var phoiBanSao = conllectionBanSao.Find(filter).FirstOrDefault();

                if (phoiBanSao == null)
                    return (int)PhoiEnum.NotFound;

                ModelProvider.MapProperties(model, phoiBanSao);
                phoiBanSao.NgayCapNhat = DateTime.Now;
                phoiBanSao.NguoiCapNhat = model.NguoiThucHien;


                var updateResult = await conllectionBanSao.ReplaceOneAsync(filter, phoiBanSao);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)PhoiEnum.Fail;
                }

                // log
                var phoiBanSaoLog = new PhoiBanSaoLogModel();
                ModelProvider.MapProperties(phoiBanSao, phoiBanSaoLog);
                phoiBanSaoLog.IdPhoiGoc = phoiBanSao.Id;
                phoiBanSaoLog.NguoiThucHien = model.NguoiThucHien;
                phoiBanSaoLog.HanhDong = EnumExtensions.ToStringValue(HanhDongLogPhoiGocEnum.UPDATE);
                phoiBanSaoLog.LyDo = lyDo;
                await conllectionBanSaoLog.InsertOneAsync(phoiBanSaoLog);

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        public async Task<int> Delete(string idPhoiBanSao, string nguoiThucHien, string lyDo)
        {
            try
            {
                var conllectionBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao);
                var conllectionBanSaoLog = _mongoDatabase.GetCollection<PhoiBanSaoLogModel>(_collectionPhoiBanSaoLogName);

                var filter = Builders<PhoiBanSaoModel>.Filter.And(
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Id, idPhoiBanSao)
                 );

                var phoiBanSao = conllectionBanSao.Find(filter).FirstOrDefault();

                if (phoiBanSao == null)
                    return (int)PhoiEnum.NotFound;

                phoiBanSao.Xoa = true;
                phoiBanSao.NgayXoa = DateTime.Now;
                phoiBanSao.NguoiXoa = nguoiThucHien;


                var updateResult = await conllectionBanSao.ReplaceOneAsync(filter, phoiBanSao);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)PhoiEnum.Fail;
                }

                // log
                var phoiBanSaoLog = new PhoiBanSaoLogModel();
                ModelProvider.MapProperties(phoiBanSao, phoiBanSaoLog);
                phoiBanSaoLog.IdPhoiGoc = phoiBanSao.Id;
                phoiBanSaoLog.NguoiThucHien = nguoiThucHien;
                phoiBanSaoLog.HanhDong = EnumExtensions.ToStringValue(HanhDongLogPhoiGocEnum.DELETE);
                phoiBanSaoLog.LyDo = lyDo;
                await conllectionBanSaoLog.InsertOneAsync(phoiBanSaoLog);

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }
        }

        public PhoiBanSaoModel GetById(string idPhoi)
        {
            var filter = Builders<PhoiBanSaoModel>.Filter.And(
                Builders<PhoiBanSaoModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                Builders<PhoiBanSaoModel>.Filter.Eq(dmtn => dmtn.Id, idPhoi)
              );

            var phoiBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao)
                                        .Find(filter).FirstOrDefault();
            if (phoiBanSao == null)
                return null;

            return phoiBanSao;
        }

        public List<PhoiBanSaoModel> GetSearchPhoiGoc(out int total, SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<PhoiBanSaoModel>.Filter;

            var filters = new List<FilterDefinition<PhoiBanSaoModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Eq("TenPhoi", modelSearch.Search)
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var phoiBanSaos = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao)
                                .Find(combinedFilter).ToList();

            total = phoiBanSaos.Count();

            switch (modelSearch.Order)
            {
                case "0":
                    phoiBanSaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phoiBanSaos.OrderBy(x => x.TenPhoi.Split(' ').LastOrDefault()).ToList()
                        : phoiBanSaos.OrderByDescending(x => x.TenPhoi.Split(' ').LastOrDefault()).ToList();
                    break;
                case "1":
                    phoiBanSaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phoiBanSaos.OrderBy(x => x.TenPhoi).ToList()
                        : phoiBanSaos.OrderByDescending(x => x.TenPhoi).ToList();
                    break;
            }
            phoiBanSaos = phoiBanSaos.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return phoiBanSaos;
        }

        public List<PhoiBanSaoModel> GetAll()
        {
            var filterBuilder = Builders<PhoiBanSaoModel>.Filter;

            var filters = new List<FilterDefinition<PhoiBanSaoModel>>
            {
                filterBuilder.Eq("Xoa", false)
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var phoiBanSaos = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao)
                                .Find(combinedFilter).ToList();

            return phoiBanSaos;
        }

        public async Task<int> CauHinhPhoiBanSao(string idPhoiBanSao, string nguoiThucHien, List<CauHinhPhoiGocModel> cauHinhPhoiBanSaoModels)
        {
            try
            {
                var conllectionPhoiGoc = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao);

                var filter = Builders<PhoiBanSaoModel>.Filter.And(
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Id, idPhoiBanSao)
                 );

                var phoiGoc = conllectionPhoiGoc.Find(filter).FirstOrDefault();

                if (phoiGoc == null)
                    return (int)PhoiEnum.NotFound;

                foreach (var cauHinh in cauHinhPhoiBanSaoModels)
                {
                    Guid guid = Guid.NewGuid();
                    cauHinh.Id = guid.ToString();
                }

                phoiGoc.CauHinhPhoi = cauHinhPhoiBanSaoModels;


                var updateResult = await conllectionPhoiGoc.ReplaceOneAsync(filter, phoiGoc);
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

        public async Task<int> HuyPhoi(string id)
        {
            try
            {
                var filter = Builders<PhoiBanSaoModel>.Filter.And(
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Xoa, false),
                   Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Id, id)
                 );

                var phoiBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao)
                                            .Find(filter).FirstOrDefault();

                if (phoiBanSao == null)
                    return (int)PhoiEnum.NotFound;

                phoiBanSao.TinhTrang = TinhTrangPhoiEnum.DaHuy;

                var updateResult = await _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao).ReplaceOneAsync(filter, phoiBanSao);
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

        public List<CauHinhPhoiGocModel> GetCauHinhPhoiBanSao(string idPhoi)
        {
            var filter = Builders<PhoiBanSaoModel>.Filter.And(
                    Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Xoa, false),
                    Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Id, idPhoi)
                  );

            var phoiGoc = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao)
                                        .Find(filter).FirstOrDefault();

            if (phoiGoc == null)
                return null;

            var cauHinhPhoi = phoiGoc.CauHinhPhoi.ToList();

            return cauHinhPhoi;
        }


        public CauHinhPhoiGocModel? GetCauHinhBanSaoById(string idphoiBanSao, string idCauHinhPhoi)
        {
            var filter = Builders<PhoiBanSaoModel>.Filter.And(
                    Builders<PhoiBanSaoModel>.Filter.Eq(x => x.Xoa, false),
                    Builders<PhoiBanSaoModel>.Filter.Eq(x => x.Id, idphoiBanSao),
                    Builders<PhoiBanSaoModel>.Filter.ElemMatch(x => x.CauHinhPhoi,
                        Builders<CauHinhPhoiGocModel>.Filter.And(
                                Builders<CauHinhPhoiGocModel>.Filter.Eq(x => x.Id, idCauHinhPhoi)
                        )
                    )
                );

            var phoiBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao).Find(filter).FirstOrDefault();
            return phoiBanSao != null ? phoiBanSao.CauHinhPhoi.Find(x => x.Id == idCauHinhPhoi) : null;
        }

        public async Task<int> ModifyCauHinhBanSao(string idPhoiBanSao, CauHinhPhoiGocInputModel model)
        {
            try
            {
                var filter = Builders<PhoiBanSaoModel>.Filter.Eq(p => p.Id, idPhoiBanSao);

                var update = Builders<PhoiBanSaoModel>.Update
                    .Set("CauHinhPhoi.$[f].KieuChu", model.KieuChu)
                    .Set("CauHinhPhoi.$[f].CoChu", model.CoChu)
                    .Set("CauHinhPhoi.$[f].DinhDangKieuChu", model.DinhDangKieuChu)
                    .Set("CauHinhPhoi.$[f].ViTriTren", model.ViTriTren)
                    .Set("CauHinhPhoi.$[f].MauChu", model.MauChu)
                    .Set("CauHinhPhoi.$[f].ViTriTrai", model.ViTriTrai);

                var arrayFilters = new[]
                        {
                            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                new BsonDocument("f._id",model.Id))
                        };

                UpdateResult u = await _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao).UpdateOneAsync(filter, update, new UpdateOptions() { ArrayFilters = arrayFilters });

                return (int)PhoiEnum.Success;
            }
            catch
            {
                return (int)PhoiEnum.Fail;
            }

        }

        /// <summary>
        /// Lấy thông tin phôi đang sử dụng
        /// </summary>
        /// <returns></returns>
        public PhoiBanSaoModel GetPhoiDangSuDung(string idTruong)
        {
            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(t => t.Xoa == false && t.Id == idTruong).FirstOrDefault();

            var filter = Builders<PhoiBanSaoModel>.Filter.And(
               Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Xoa, false),
               Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.TinhTrang, TinhTrangPhoiEnum.DangSuDung),
               Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.MaHeDaoTao, truong.MaHeDaoTao)
            );

            var phoiBanSao = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao)
                                        .Find(filter).FirstOrDefault();

            return phoiBanSao;
        }

        public bool TrungTenPhoi(string tenPhoi, string idPhoiBanSao)
        {
            var mt = idPhoiBanSao == null
                ? _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao).Find(n => true && n.Xoa == false && n.TenPhoi == tenPhoi).FirstOrDefault()
                : _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao).Find(n => true && n.Xoa == false && n.TenPhoi == tenPhoi && n.Id != idPhoiBanSao).FirstOrDefault();
            return mt != null ? true : false;
        }

        public bool CheckPhoiDangSuDung(string maHeDaoTao)
        {
            var filter = Builders<PhoiBanSaoModel>.Filter.And(
               Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.Xoa, false),
               Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.TinhTrang, TinhTrangPhoiEnum.DangSuDung),
               Builders<PhoiBanSaoModel>.Filter.Eq(pg => pg.MaHeDaoTao, maHeDaoTao)

            );

            var existingPhoi = _mongoDatabase.GetCollection<PhoiBanSaoModel>(_collectionNamePhoiBanSao)
                                            .Find(filter).FirstOrDefault();

            return existingPhoi != null;
        }
    }
}
