using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc
{
    public class HeDaoTaoBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private IMongoDatabase _mongoDatabase;

        public HeDaoTaoBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        /// <summary>
        /// Thêm hệ đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(HeDaoTaoInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)HeDaoTaoEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)HeDaoTaoEnum.ExistName;
            try
            {
                var heDaoTaoModel = new HeDaoTaoModel();
                ModelProvider.MapProperties(model, heDaoTaoModel);
                heDaoTaoModel.Ma = heDaoTaoModel.Ma.ToUpper();
                heDaoTaoModel.NguoiTao = model.NguoiThucHien;
                heDaoTaoModel.NgayTao = DateTime.Now;
                await _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao).InsertOneAsync(heDaoTaoModel);
                return (int)HeDaoTaoEnum.Success;
            }
            catch
            {
                return (int)HeDaoTaoEnum.Fail;
            }
        }

        /// <summary>
        /// Cập nhật hệ đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(HeDaoTaoInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)HeDaoTaoEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)HeDaoTaoEnum.ExistName;
            try
            {
                var filter = Builders<HeDaoTaoModel>.Filter.And(
                                Builders<HeDaoTaoModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<HeDaoTaoModel>.Filter.Eq(hdt => hdt.Id, model.Id)
                              );

                var heDaoTao = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                            .Find(filter).FirstOrDefault();
                if (heDaoTao == null)
                    return (int)HeDaoTaoEnum.NotFound;

                ModelProvider.MapProperties(model, heDaoTao);
                heDaoTao.Ma = heDaoTao.Ma.ToUpper();
                heDaoTao.NguoiCapNhat = model.NguoiThucHien;
                heDaoTao.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                                        .ReplaceOneAsync(filter, heDaoTao);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)HeDaoTaoEnum.Fail;
                }
                return (int)HeDaoTaoEnum.Success;
            }
            catch
            {
                return (int)HeDaoTaoEnum.Fail;
            }
        }

        /// <summary>
        /// Xóa hệ đào tạo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<HeDaoTaoModel>.Filter.And(
                                Builders<HeDaoTaoModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<HeDaoTaoModel>.Filter.Eq(hdt => hdt.Id, id)
                              );

                var heDaoTao = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                            .Find(filter).FirstOrDefault();
                if (heDaoTao == null)
                    return (int)HeDaoTaoEnum.NotFound;

                heDaoTao.Xoa = true;
                heDaoTao.NgayXoa = DateTime.Now;
                heDaoTao.NguoiXoa = nguoiThucHien;

                var updateResult = await _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                                        .ReplaceOneAsync(filter, heDaoTao);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)HeDaoTaoEnum.Fail;
                }
                return (int)HeDaoTaoEnum.Success;
            }
            catch
            {
                return (int)HeDaoTaoEnum.Fail;
            }
        }

        /// <summary>
        /// Lấy tất cả Hệ đào tạo
        /// </summary>
        /// <returns></returns>
        public List<HeDaoTaoModel> GetAll()
        {
            var heDaoTaos = new List<HeDaoTaoModel>();
            heDaoTaos = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                .Find(p => p.Xoa == false)
                .ToList();
            return heDaoTaos.OrderByDescending(x => x.Ten).ToList();
        }

        /// <summary>
        /// Lấy tất cả Hệ đào tạo (param)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<HeDaoTaoModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            var heDaoTaoCollection = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao);
            var filterBuilder = Builders<HeDaoTaoModel>.Filter;

            var filters = new List<FilterDefinition<HeDaoTaoModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Or(
                            filterBuilder.Regex("Ten", new BsonRegularExpression(modelSearch.Search, "i")),
                            filterBuilder.Regex("Ma", new BsonRegularExpression(modelSearch.Search, "i"))
                            ) 
                    : null,
                
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var heDaoTaos = heDaoTaoCollection.Find(combinedFilter).ToList();


            total = heDaoTaos.Count();

            //sắp xếp
            switch (modelSearch.Order.ToLower())
            {
                case "0":
                    heDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? heDaoTaos.OrderBy(x => x.Ma).ToList()
                        : heDaoTaos.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "1":
                    heDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? heDaoTaos.OrderBy(x => x.Ten).ToList()
                        : heDaoTaos.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "ma":
                    heDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? heDaoTaos.OrderBy(x => x.Ma).ToList()
                        : heDaoTaos.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "ten":
                    heDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? heDaoTaos.OrderBy(x => x.Ten).ToList()
                        : heDaoTaos.OrderByDescending(x => x.Ten).ToList();
                    break;
            }

            //phân trang
            if (modelSearch.PageSize > 0)
            {
                heDaoTaos = heDaoTaos.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return heDaoTaos;
        }

        /// <summary>
        /// Lấy hệ đào tạo theo ID
        /// </summary>
        /// <param name="idHedaoTao"></param>
        /// <returns></returns>
        public HeDaoTaoModel GetById(string idHedaoTao)
        {
            var heDaoTao = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                .Find(n => true && n.Id == idHedaoTao && n.Xoa == false)
                .FirstOrDefault();

            return heDaoTao;
        }

        #region private
        private bool TrungMa(string Ma, string Id)
        {
            var hdt = Id == null
                ? _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                .Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                .Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower() && n.Id != Id).FirstOrDefault();
            return hdt != null ? true : false;
        }

        public bool TrungTen(string Ten, string Id)
        {
            var hdt = Id == null
                ? _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                .Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                .Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower() && n.Id != Id).FirstOrDefault();
            return hdt != null ? true : false;
        }
        #endregion
    }
}
