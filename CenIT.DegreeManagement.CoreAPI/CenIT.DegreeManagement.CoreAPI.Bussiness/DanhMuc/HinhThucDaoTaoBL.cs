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
    public class HinhThucDaoTaoBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private IMongoDatabase _mongoDatabase;

        public HinhThucDaoTaoBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        /// <summary>
        /// Thêm mới hình thức đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(HinhThucDaoTaoInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)HinhThucDaoTaoEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)HinhThucDaoTaoEnum.ExistName; 
            try
            {
                var hinhThucDaoTaoModel = new HinhThucDaoTaoModel();
                ModelProvider.MapProperties(model, hinhThucDaoTaoModel);
                hinhThucDaoTaoModel.Ma = hinhThucDaoTaoModel.Ma.ToUpper();
                hinhThucDaoTaoModel.NguoiTao = model.NguoiThucHien;
                hinhThucDaoTaoModel.NgayTao = DateTime.Now;
                await _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao).InsertOneAsync(hinhThucDaoTaoModel);
                return (int)HinhThucDaoTaoEnum.Success;
            }
            catch
            {
                return (int)HinhThucDaoTaoEnum.Fail;
            }
        }

        /// <summary>
        /// Cập nhật hình thức đào tạo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(HinhThucDaoTaoInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)HinhThucDaoTaoEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)HinhThucDaoTaoEnum.ExistName;
            try
            {
                var filter = Builders<HinhThucDaoTaoModel>.Filter.And(
                                Builders<HinhThucDaoTaoModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<HinhThucDaoTaoModel>.Filter.Eq(hdt => hdt.Id, model.Id)
                              );

                var hinhThucDaoTao = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao)
                                            .Find(filter).FirstOrDefault();
                if (hinhThucDaoTao == null)
                    return (int)HinhThucDaoTaoEnum.NotFound;
                
                ModelProvider.MapProperties(model, hinhThucDaoTao);
                hinhThucDaoTao.Ma = hinhThucDaoTao.Ma.ToUpper();
                hinhThucDaoTao.NguoiCapNhat = model.NguoiThucHien;
                hinhThucDaoTao.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao)
                                                        .ReplaceOneAsync(filter, hinhThucDaoTao);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)HinhThucDaoTaoEnum.Fail;
                }
                return (int)HinhThucDaoTaoEnum.Success;
            }
            catch
            {
                return (int)HinhThucDaoTaoEnum.Fail;
            }
        }

        /// <summary>
        /// Xóa hình thức đào tạo
        /// </summary>
        /// <param name="nguoiThucHien"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<HinhThucDaoTaoModel>.Filter.And(
                                Builders<HinhThucDaoTaoModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<HinhThucDaoTaoModel>.Filter.Eq(hdt => hdt.Id, id)
                              );

                var hinhThucDaoTao = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao)
                                            .Find(filter).FirstOrDefault();
                if (hinhThucDaoTao == null)
                    return (int)HeDaoTaoEnum.NotFound;

                hinhThucDaoTao.Xoa = true;
                hinhThucDaoTao.NgayXoa = DateTime.Now;
                hinhThucDaoTao.NguoiXoa = nguoiThucHien;

                var updateResult = await _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao)
                                                        .ReplaceOneAsync(filter, hinhThucDaoTao);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)HinhThucDaoTaoEnum.Fail;
                }
                return (int)HinhThucDaoTaoEnum.Success;
            }
            catch
            {
                return (int)HinhThucDaoTaoEnum.Fail;
            }
        }

        /// <summary>
        /// Lấy tất cả HinhThucDaoTao
        /// </summary>
        /// <returns></returns>
        public List<HinhThucDaoTaoModel> GetAll()
        {
            var namThis = new List<HinhThucDaoTaoModel>();
            namThis = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao)
                .Find(p => p.Xoa == false)
                .ToList();
            return namThis.OrderByDescending(x => x.Ten).ToList();
        }

        /// <summary>
        /// Lấy danh sách HinhThucDaoTao (param)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<HinhThucDaoTaoModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);
            var filterBuilder = Builders<HinhThucDaoTaoModel>.Filter;

            var filters = new List<FilterDefinition<HinhThucDaoTaoModel>>
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
            var hinThucDaoTaos = hinhThucDaoTaoCollection.Find(combinedFilter).ToList();


            total = hinThucDaoTaos.Count();

            //sắp xếp
            switch (modelSearch.Order.ToLower())
            {
                case "0":
                    hinThucDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hinThucDaoTaos.OrderBy(x => x.Ma).ToList()
                        : hinThucDaoTaos.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "1":
                    hinThucDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hinThucDaoTaos.OrderBy(x => x.Ten).ToList()
                        : hinThucDaoTaos.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "ma":
                    hinThucDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hinThucDaoTaos.OrderBy(x => x.Ma).ToList()
                        : hinThucDaoTaos.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "ten":
                    hinThucDaoTaos = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hinThucDaoTaos.OrderBy(x => x.Ten).ToList()
                        : hinThucDaoTaos.OrderByDescending(x => x.Ten).ToList();
                    break;
            }

            //phân trang
            if (modelSearch.PageSize > 0)
            {
                hinThucDaoTaos = hinThucDaoTaos.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hinThucDaoTaos;
        }

        /// <summary>
        /// Lấy HinhThucDaoTao theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HinhThucDaoTaoModel GetById(string id)
        {
            var clt = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao)
                .Find(n => true && n.Id == id && n.Xoa == false)
                .FirstOrDefault();
            return clt;
        }

        #region private
        private bool TrungMa(string Ma, string Id)
        {
            var htdt = Id == null
                ? _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao).Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao).Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower() && n.Id != Id).FirstOrDefault();
            return htdt != null ? true : false;
        }

        private bool TrungTen(string Ten, string Id)
        {
            var htdt = Id == null
                ? _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower() && n.Id != Id).FirstOrDefault();
            return htdt != null ? true : false;
        }
        #endregion
    }
}
