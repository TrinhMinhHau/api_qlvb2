using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.TinTuc
{
    public class TinTucBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private readonly string collectionName = "TinTuc";
        private readonly string collectionNameLoaiTinTuc = "LoaiTinTuc";

        private IMongoDatabase _mongoDatabase;

        public TinTucBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> Create(TinTucInputModel model)
        {
            try
            {
                if (!KiemTraLoaiTin(model.IdLoaiTinTuc)) return (int)TinTucEnum.NotExistLoaiTin;
                var tinTuc = new TinTucModel();
                ModelProvider.MapProperties(model, tinTuc);
                tinTuc.NguoiTao = model.NguoiThucHien;
                tinTuc.NgayTao = DateTime.Now;

                // insert
                await _mongoDatabase.GetCollection<TinTucModel>(collectionName).InsertOneAsync(tinTuc);
                if (tinTuc.Id != null)
                {
                    return (int)TinTucEnum.Success;

                }
                return (int)TinTucEnum.Fail;
            }
            catch
            {
                return (int)TinTucEnum.Fail;
            }
        }

        public async Task<int> Modify(TinTucInputModel model)
        {
            try
            {
                if (!KiemTraLoaiTin(model.IdLoaiTinTuc)) return (int)TinTucEnum.NotExistLoaiTin;

                var tinTuc = _mongoDatabase.GetCollection<TinTucModel>(collectionName)
                                        .Find(t=>t.Xoa == false && t.Id == model.Id).FirstOrDefault();

                if (tinTuc == null)
                {
                    return (int)TinTucEnum.ExistName;
                }

                ModelProvider.MapProperties(model, tinTuc);
                tinTuc.NguoiCapNhat = model.NguoiThucHien;
                tinTuc.NgayCapNhat = DateTime.Now;

                // insert
                var updateResult = _mongoDatabase.GetCollection<TinTucModel>(collectionName)
                                                            .ReplaceOne((t => t.Xoa == false && t.Id == model.Id), tinTuc);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)TinTucEnum.Fail;
                }
                return (int)TinTucEnum.Success;
            }
            catch
            {
                return (int)TinTucEnum.Fail;
            }
        }

        public async Task<int> Delete(string idTinTuc, string nguoiThucHien)
        {
            try
            {
                var loaiTin = _mongoDatabase.GetCollection<TinTucModel>(collectionName)
                             .Find(lt => lt.Xoa == false && lt.Id == idTinTuc).FirstOrDefault();

                if (loaiTin == null)
                {
                    return (int)LoaiTinTucEnum.NotFound; // không tồn tại
                }

                loaiTin.Xoa = true;
                loaiTin.NguoiCapNhat = nguoiThucHien;
                loaiTin.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<TinTucModel>(collectionName)
                                .ReplaceOneAsync((lt => lt.Xoa == false && lt.Id == idTinTuc), loaiTin);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)TinTucEnum.Fail;
                }
                return (int)TinTucEnum.Success;
            }
            catch
            {
                return (int)TinTucEnum.Fail;
            }
        }

        public async Task<int> HideTinTuc(string idTinTuc, bool isHide = true)
        {
            try
            {
                var loaiTin = _mongoDatabase.GetCollection<TinTucModel>(collectionName)
                             .Find(lt => lt.Xoa == false && lt.Id == idTinTuc).FirstOrDefault();

                if (loaiTin == null)
                {
                    return (int)LoaiTinTucEnum.NotFound; // không tồn tại
                }

                if (isHide)
                {
                    loaiTin.TrangThai = TrangThaiTinTucEnum.An;
                }
                else
                {
                    loaiTin.TrangThai = TrangThaiTinTucEnum.Hien;
                }

                var updateResult = await _mongoDatabase.GetCollection<TinTucModel>(collectionName)
                                .ReplaceOneAsync((lt => lt.Xoa == false && lt.Id == idTinTuc), loaiTin);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)TinTucEnum.Fail;
                }
                return (int)TinTucEnum.Success;
            }
            catch
            {
                return (int)TinTucEnum.Fail;
            }
        }

        public List<TinTucViewModel> GetSearchTinTuc(out int total, SearchParamModel modelSearch, bool isPubLish = false)
        {
            var filterBuilder = Builders<TinTucViewModel>.Filter;

            var filters = new List<FilterDefinition<TinTucViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Regex("TieuDe", new BsonRegularExpression(modelSearch.Search, "i"))
                    : null,
                 isPubLish == true
                    ?    filterBuilder.Eq("TrangThai", (int)TrangThaiTinTucEnum.Hien)
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var tinTuc = _mongoDatabase.GetCollection<TinTucViewModel>(collectionName)
                                .Find(combinedFilter).ToList();

            total = tinTuc.Count();

            switch (modelSearch.Order)
            {
                case "0":
                    tinTuc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? tinTuc.OrderBy(x => x.TieuDe).ToList()
                        : tinTuc.OrderByDescending(x => x.TieuDe).ToList();
                    break;
                case "1":
                    tinTuc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? tinTuc.OrderBy(x => x.TieuDe).ToList()
                        : tinTuc.OrderByDescending(x => x.TieuDe).ToList();
                    break;
            }
            if (modelSearch.PageSize > 1)
            {
                tinTuc = tinTuc.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return tinTuc;
        }

        public List<TinTucViewModel> GetSearchTinTucByIdLoaiTin(out int total,string idLoaiTin, SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<TinTucViewModel>.Filter;
            var collectionLoaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionNameLoaiTinTuc);


            var filters = new List<FilterDefinition<TinTucViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("IdLoaiTinTuc", idLoaiTin),
                filterBuilder.Eq("TrangThai", TrangThaiTinTucEnum.Hien),

                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Regex("TieuDe", new BsonRegularExpression(modelSearch.Search, "i"))
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var tinTuc = _mongoDatabase.GetCollection<TinTucViewModel>(collectionName)
                                .Find(combinedFilter).ToList()
                                .Join(
                                      collectionLoaiTinTuc.AsQueryable(),
                                      t => t.IdLoaiTinTuc,
                                      l => l.Id,
                                      (t, l) =>
                                      {
                                          t.TenLoaiTinTuc = l.TieuDe;
                                          return t;
                                      }).ToList(); ;

            total = tinTuc.Count();

            switch (modelSearch.Order)
            {
                case "0":
                    tinTuc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? tinTuc.OrderBy(x => x.TieuDe).ToList()
                        : tinTuc.OrderByDescending(x => x.TieuDe).ToList();
                    break;
                case "1":
                    tinTuc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? tinTuc.OrderBy(x => x.TieuDe).ToList()
                        : tinTuc.OrderByDescending(x => x.TieuDe).ToList();
                    break;
            }
            if(modelSearch.PageSize > 1)
            {
                tinTuc = tinTuc.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return tinTuc;
        }

        public List<TinTucViewModel> GetAll()
        {
            var filterBuilder = Builders<TinTucViewModel>.Filter;
            var collectionLoaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionNameLoaiTinTuc);


            var filters = new List<FilterDefinition<TinTucViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("TrangThai", TrangThaiTinTucEnum.Hien)
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var tinTucs = _mongoDatabase.GetCollection<TinTucViewModel>(collectionName)
                                .Find(combinedFilter).ToList()
                                 .Join(
                                      collectionLoaiTinTuc.AsQueryable(),
                                      t => t.IdLoaiTinTuc,
                                      l => l.Id,
                                      (t, l) =>
                                      {
                                          t.TenLoaiTinTuc = l.TieuDe;
                                          return t;
                                      }).ToList();
            return tinTucs;
        }

        public List<TinTucViewModel> GetLatest4News()
        {
            var filterBuilder = Builders<TinTucViewModel>.Filter;
            var collectionLoaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionNameLoaiTinTuc);

            var filters = new List<FilterDefinition<TinTucViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("TrangThai", (int)TrangThaiTinTucEnum.Hien),
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var tinTuc = _mongoDatabase.GetCollection<TinTucViewModel>(collectionName)
                   .Find(combinedFilter)
                   .SortByDescending(t => t.NgayTao)  
                   .Limit(4)                          
                   .ToList()
                   .Where(x=>x.TrangThai == TrangThaiTinTucEnum.Hien)
                   .Join(
                          collectionLoaiTinTuc.AsQueryable(),
                          t => t.IdLoaiTinTuc,
                          l => l.Id,
                          (t, l) =>
                          {
                            t.TenLoaiTinTuc = l.TieuDe;
                            return t;
                          }).ToList(); 
            return tinTuc;
        }

        public TinTucViewModel GetById(string idTinTuc)
        {
            var collectionLoaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionNameLoaiTinTuc);
            var tinTuc = _mongoDatabase.GetCollection<TinTucViewModel>(collectionName)
                                .Find(l => l.Xoa == false && l.Id == idTinTuc)
                                .ToList()
                                .Join(
                                    collectionLoaiTinTuc.AsQueryable(),
                                    t=>t.IdLoaiTinTuc,
                                    l=>l.Id,
                                    (t, l) =>
                                    {
                                        t.TenLoaiTinTuc = l.TieuDe;
                                        return t;
                                    }).FirstOrDefault();

            return tinTuc;
        }


      

        private bool KiemTraLoaiTin(string idLoaiTin)
        {
            var loaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionNameLoaiTinTuc)
                                           .Find(h => h.Xoa == false && h.Id == idLoaiTin).FirstOrDefault();
            return loaiTinTuc != null ? true : false;
        }
    }
}
