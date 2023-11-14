using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc
{
    public class DanTocBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private IMongoDatabase _mongoDatabase;

        public DanTocBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }


        public async Task<int> Create(DanTocInputModel model)
        {
            if (TrungTen(model.Ten, model.Id)) return (int)DanTocEnum.ExistName;
            try
            {
                var danTocModel = new DanTocModel();
                danTocModel.Ten = model.Ten;
                danTocModel.NguoiTao = model.NguoiThucHien;
                danTocModel.NgayTao = DateTime.Now;
                await _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName).InsertOneAsync(danTocModel);
                return (int)DanTocEnum.Success;
            }
            catch
            {
                return (int)DanTocEnum.Fail;
            }
        }

        public async Task<int> Modify(DanTocInputModel model)
        {
            if (TrungTen(model.Ten, model.Id)) return (int)DanTocEnum.ExistName;
            try
            {
                var filter = Builders<DanTocModel>.Filter.And(
                               Builders<DanTocModel>.Filter.Eq(hdt => hdt.Xoa, false),
                               Builders<DanTocModel>.Filter.Eq(hdt => hdt.Id, model.Id)
                             );

                var danToc = _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                                            .Find(filter).FirstOrDefault();

                if (danToc == null)
                    return (int)DanTocEnum.NotFound;

                ModelProvider.MapProperties(model, danToc);
                danToc.NguoiCapNhat = model.NguoiThucHien;
                danToc.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                                                        .ReplaceOneAsync(filter, danToc);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)DanTocEnum.Fail;
                }
                return (int)DanTocEnum.Success;
            }
            catch
            {
                return (int)DanTocEnum.Fail;
            }
        }

        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<DanTocModel>.Filter.And(
                                Builders<DanTocModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<DanTocModel>.Filter.Eq(hdt => hdt.Id, id)
                              );

                var danToc = _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                                            .Find(filter).FirstOrDefault();

                if (danToc == null)
                    return (int)DanTocEnum.NotFound;
                danToc.NguoiXoa = nguoiThucHien;
                danToc.NgayXoa = DateTime.Now;
                danToc.Xoa = true;

                var updateResult = await _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                                                        .ReplaceOneAsync(filter, danToc);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)DanTocEnum.Fail;
                }
                return (int)DanTocEnum.Success;
            }
            catch
            {
                return (int)DanTocEnum.Fail;
            }
        }


        public string[] GetAllTenDanToc()
        {
            var danTocs = _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                     .Find(p => p.Xoa == false)
                     .ToList();

            string[] tenDanTocArray = danTocs.Select(x => x.Ten).OrderByDescending(x => x).ToArray();

            return tenDanTocArray;
        }

        public List<DanTocModel> GetAll()
        {
            var danTocs = _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                .Find(p => p.Xoa == false)
                .ToList();
            return danTocs.OrderByDescending(x => x.Ten).ToList();
        }

        public List<DanTocModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<DanTocModel>.Filter;
            var danTocTaoCollection = _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName);

            var filters = new List<FilterDefinition<DanTocModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Regex("Ten", new BsonRegularExpression(modelSearch.Search, "i"))
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var danTocs = danTocTaoCollection.Find(combinedFilter).ToList();


            total = danTocs.Count();

            //sắp xếp
            switch (modelSearch.Order)
            {
                case "0":
                    danTocs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? danTocs.OrderBy(x => x.Ten).ToList()
                        : danTocs.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "1":
                    danTocs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? danTocs.OrderBy(x => x.Ten).ToList()
                        : danTocs.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "ten":
                    danTocs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? danTocs.OrderBy(x => x.Ten).ToList()
                        : danTocs.OrderByDescending(x => x.Ten).ToList();
                    break;
            }

            //phân trang
            if (modelSearch.PageSize > 0)
            {
                danTocs = danTocs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return danTocs;
                
        }

        public DanTocModel GetById(string id)
        {
            var filter = Builders<DanTocModel>.Filter.And(
                             Builders<DanTocModel>.Filter.Eq(hdt => hdt.Xoa, false),
                             Builders<DanTocModel>.Filter.Eq(hdt => hdt.Id, id)
                           );

            var danToc = _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                                        .Find(filter).FirstOrDefault();
            return danToc;
        }

        private bool TrungTen(string ten, string id)
        {
            var danToc = id == null
                ? _mongoDatabase.GetCollection<DanTocModel>(_collectionDanTocName)
                                .Find(n => true && n.Xoa == false && n.Ten.ToLower() == ten.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<DanTocModel>(_collectionNamHeDaoTao)
                                .Find(n => true && n.Xoa == false && n.Ten.ToLower() == ten.ToLower() && n.Id != id).FirstOrDefault();
            return danToc != null ? true : false;
        }

    }
}
