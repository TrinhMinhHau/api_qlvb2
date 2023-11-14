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
    public class MonThiBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private IMongoDatabase _mongoDatabase;

        public MonThiBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        /// <summary>
        /// Thêm Mon thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(MonThiInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)MonThiEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)MonThiEnum.ExistName;
            try
            {
                var monThiModel = new MonThiModel();
                ModelProvider.MapProperties(model, monThiModel);
                monThiModel.Ma = model.Ma.ToUpper();
                monThiModel.NguoiTao = model.NguoiThucHien;
                monThiModel.NgayTao = DateTime.Now;
                await _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi).InsertOneAsync(monThiModel);
                return (int)MonThiEnum.Success;
            }
            catch
            {
                return (int)MonThiEnum.Fail;
            }
        }

        /// <summary>
        /// Cập nhật môn thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(MonThiInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)MonThiEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)MonThiEnum.ExistName;
            try
            {
                var filter = Builders<MonThiModel>.Filter.And(
                                Builders<MonThiModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<MonThiModel>.Filter.Eq(hdt => hdt.Id, model.Id)
                              );

                var monThi = _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi)
                                            .Find(filter).FirstOrDefault();
                if (monThi == null)
                    return (int)MonThiEnum.NotFound;

                ModelProvider.MapProperties(model, monThi);
                monThi.Ma = model.Ma.ToUpper();
                monThi.NguoiCapNhat = model.NguoiThucHien;
                monThi.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi)
                                                        .ReplaceOneAsync(filter, monThi);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)MonThiEnum.Fail;
                }
                return (int)MonThiEnum.Success;
            }
            catch
            {
                return (int)MonThiEnum.Fail;
            }
        }

        /// <summary>
        /// Xóa môn thi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<MonThiModel>.Filter.And(
                             Builders<MonThiModel>.Filter.Eq(hdt => hdt.Xoa, false),
                             Builders<MonThiModel>.Filter.Eq(hdt => hdt.Id, id)
                           );

                var monThi = _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi)
                                            .Find(filter).FirstOrDefault();
                if (monThi == null)
                    return (int)MonThiEnum.NotFound;

                monThi.Xoa = true;
                monThi.NgayXoa = DateTime.Now;
                monThi.NguoiXoa = nguoiThucHien;

                var updateResult = await _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi)
                                                        .ReplaceOneAsync(filter, monThi);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)MonThiEnum.Fail;
                }
                return (int)MonThiEnum.Success;
            }
            catch
            {
                return (int)MonThiEnum.Fail;
            }
        }

        /// <summary>
        /// Lấy danh sách mon thi (Tìm kiếm)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<MonThiModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            var monThiCollection = _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi);
            var filterBuilder = Builders<MonThiModel>.Filter;

            var filters = new List<FilterDefinition<MonThiModel>>
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
            var monThis = monThiCollection.Find(combinedFilter).ToList();


            total = monThis.Count();

            //sắp xếp
            switch (modelSearch.Order.ToLower())
            {
                case "0":
                    monThis = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? monThis.OrderBy(x => x.Ma).ToList()
                        : monThis.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "1":
                    monThis = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? monThis.OrderBy(x => x.Ten).ToList()
                        : monThis.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "ma":
                    monThis = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? monThis.OrderBy(x => x.Ma).ToList()
                        : monThis.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "ten":
                    monThis = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? monThis.OrderBy(x => x.Ten).ToList()
                        : monThis.OrderByDescending(x => x.Ten).ToList();
                    break;

            }

            //phân trang
            if (modelSearch.PageSize > 0)
            {
                monThis = monThis.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return monThis;
        }


        /// <summary>
        /// Lấy tất cả môn thi
        /// </summary>
        /// <returns></returns>
        public List<MonThiModel> GetAll()
        {
            var monThis = new List<MonThiModel>();
            monThis = _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi)
                .Find(p => p.Xoa == false)
                .ToList();
            return monThis.OrderByDescending(x => x.Ten).ToList();
        }

        public string[] GetAllMaMonThi()
        {
            var monThis = _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi)
                     .Find(p => p.Xoa == false)
                     .ToList();

            string[] maMonThiArray = monThis.Select(x => x.Ma).OrderByDescending(x => x).ToArray();
            return maMonThiArray;
        }

        /// <summary>
        /// Lấy mon thi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MonThiModel? GetById(string id)
        {
            var monthi = _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi)
                .Find(n => true && n.Id == id && n.Xoa == false)
                .FirstOrDefault();
            return monthi;
        }

        #region private
        private bool TrungMa(string Ma, string Id)
        {
            var mt = Id == null
                ? _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi).Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi).Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower() && n.Id != Id).FirstOrDefault();
            return mt != null ? true : false;
        }
        private bool TrungTen(string Ten, string Id)
        {
            var mt = Id == null
                ? _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<MonThiModel>(_collectionNameMonThi).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower() && n.Id != Id).FirstOrDefault();
            return mt != null ? true : false;
        }
        #endregion
    }
}
