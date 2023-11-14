using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.TinTuc
{
    public class LoaiTinTucBL
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private readonly string collectionName = "LoaiTinTuc";
        private IMongoDatabase _mongoDatabase;

        public LoaiTinTucBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> Create(LoaiTinTucInputModel model)
        {
            try
            {
                if (TrungTieuDe(model.TieuDe, model.Id)) return (int)LoaiTinTucEnum.ExistName;

                var loaiTin = new LoaiTinTucModel();
                ModelProvider.MapProperties(model, loaiTin);
                loaiTin.NguoiTao = model.NguoiThucHien;
                loaiTin.NgayTao = DateTime.Now;

                // insert
                await _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName).InsertOneAsync(loaiTin);
                if(loaiTin.Id != null)
                {
                    return (int)LoaiTinTucEnum.Success;

                }
                return (int)LoaiTinTucEnum.Fail;
            }
            catch
            {
                return (int)LoaiTinTucEnum.Fail;
            }
        }

        public async Task<int> Modify(LoaiTinTucInputModel model)
        {
            try
            {
                if (TrungTieuDe(model.TieuDe, model.Id)) return (int)LoaiTinTucEnum.ExistName;

                var loaiTin = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName)
                                        .Find(lt=>lt.Xoa == false && lt.Id == model.Id).FirstOrDefault();

                if (loaiTin == null)
                {
                    return (int)LoaiTinTucEnum.NotFound; // không tồn tại
                }

                ModelProvider.MapProperties(model, loaiTin);
                loaiTin.NguoiCapNhat = model.NguoiThucHien;
                loaiTin.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName)
                                                        .ReplaceOneAsync((lt => lt.Xoa == false && lt.Id == model.Id), loaiTin);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)LoaiTinTucEnum.Fail;
                }
                return (int)LoaiTinTucEnum.Success;
            }
            catch
            {
                return (int)LoaiTinTucEnum.Fail;
            }
        }

        public async Task<int> Delete(string idLoaiTinTuc, string nguoiThucHien)
        {
            try
            {
                var loaiTin = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName)
                             .Find(lt => lt.Xoa == false && lt.Id == idLoaiTinTuc).FirstOrDefault();

                if (loaiTin == null)
                {
                    return (int)LoaiTinTucEnum.NotFound; // không tồn tại
                }

                loaiTin.Xoa = true;
                loaiTin.NguoiCapNhat = nguoiThucHien;
                loaiTin.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName)
                                .ReplaceOneAsync((lt => lt.Xoa == false && lt.Id == idLoaiTinTuc), loaiTin);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)LoaiTinTucEnum.Fail;
                }
                return (int)LoaiTinTucEnum.Success;
            }
            catch
            {
                return (int)LoaiTinTucEnum.Fail;
            }
        }

        public List<LoaiTinTucModel> GetSearchLoaiTinTuc(out int total, SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<LoaiTinTucModel>.Filter;

            var filters = new List<FilterDefinition<LoaiTinTucModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Regex("TieuDe", new BsonRegularExpression(modelSearch.Search, "i"))
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var loaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName)
                                .Find(combinedFilter).ToList();

            total = loaiTinTuc.Count();

            switch (modelSearch.Order)
            {
                case "0":
                    loaiTinTuc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? loaiTinTuc.OrderBy(x => x.TieuDe).ToList()
                        : loaiTinTuc.OrderByDescending(x => x.TieuDe).ToList();
                    break;
                case "1":
                    loaiTinTuc = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? loaiTinTuc.OrderBy(x => x.TieuDe).ToList()
                        : loaiTinTuc.OrderByDescending(x => x.TieuDe).ToList();
                    break;
            }
            loaiTinTuc = loaiTinTuc.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return loaiTinTuc;
        }

        public List<LoaiTinTucModel> GetAll()
        {
            var filterBuilder = Builders<LoaiTinTucModel>.Filter;

            var filters = new List<FilterDefinition<LoaiTinTucModel>>
            {
                filterBuilder.Eq("Xoa", false),
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var loaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName)
                                .Find(combinedFilter).ToList();
            return loaiTinTuc;
        }

        public LoaiTinTucModel GetById(string idLoaiTinTuc)
        {
            var loaiTinTuc = _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName)
                                .Find(l=>l.Xoa == false && l.Id == idLoaiTinTuc).FirstOrDefault();
            return loaiTinTuc;
        }

        public bool TrungTieuDe(string tieuDe, string idLoaiTinTuc)
        {
            var mt = idLoaiTinTuc == null
                ? _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName).Find(n => true && n.Xoa == false && n.TieuDe == tieuDe).FirstOrDefault()
                : _mongoDatabase.GetCollection<LoaiTinTucModel>(collectionName).Find(n => true && n.Xoa == false && n.TieuDe == tieuDe && n.Id != idLoaiTinTuc).FirstOrDefault();
            return mt != null ? true : false;
        }
    }
}
