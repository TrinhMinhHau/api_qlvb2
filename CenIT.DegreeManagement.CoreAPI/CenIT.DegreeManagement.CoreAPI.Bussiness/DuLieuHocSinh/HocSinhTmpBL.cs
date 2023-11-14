using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Helpers;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh
{
    public class HocSinhTmpBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;


        public HocSinhTmpBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> SaveImport(List<HocSinhTmpModel> models)
        {
            try
            {
                var collectionHoSinh = _mongoDatabase.GetCollection<HocSinhTmpModel>(_collectionHocSinhTmpName);

                collectionHoSinh.InsertMany(models);
                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }

        public List<HocSinhTmpModel> GetSearchHocSinhTmp(out int total, HocSinhTmpModelParamModel modelSearch)
        {
            var filterBuilder = Builders<HocSinhTmpModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhTmpModel>>
            {    
                filterBuilder.Eq("IdTruong", modelSearch.IdTruong),
                filterBuilder.Eq("NguoiTao", modelSearch.NguoiThucHien),
                filterBuilder.Eq("IdDanhMucTotNghiep", modelSearch.IdDanhMucTotNghiep),


                !string.IsNullOrEmpty(modelSearch.HoTen)
                    ? filterBuilder.Regex("HoTen", new BsonRegularExpression(modelSearch.HoTen, "i"))
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
            var hocSinhs = _mongoDatabase.GetCollection<HocSinhTmpModel>(_collectionHocSinhTmpName)
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


        public List<HocSinhTmpModel> GetAllHocSinhTmpSave(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {
            var filterBuilder = Builders<HocSinhTmpModel>.Filter;

            var filters = new List<FilterDefinition<HocSinhTmpModel>>
            {
                filterBuilder.Eq("IdTruong", idTruong),
                filterBuilder.Eq("NguoiTao", nguoiThucHien),
                filterBuilder.Eq("IdDanhMucTotNghiep", idDanhMucTotNghiep),

            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var hocSinhs = _mongoDatabase.GetCollection<HocSinhTmpModel>(_collectionHocSinhTmpName)
                                .Find(combinedFilter)
                                .ToList();
            return hocSinhs;
        }

        public ThongKeHocSinhTmpModel GetThongKeHocSinhTmp(string idTruong, string nguoiThucHien, string idDanhMucTotNghiep)
        {
            var hocSinhList = _mongoDatabase.GetCollection<HocSinhTmpModel>(_collectionHocSinhTmpName)
                                 .Find(x => x.IdTruong == idTruong && x.NguoiTao == nguoiThucHien && x.IdDanhMucTotNghiep == idDanhMucTotNghiep)
                                 .ToList();

            ThongKeHocSinhTmpModel thongKe = new ThongKeHocSinhTmpModel();

            thongKe.TotalRow = hocSinhList.Count;
            thongKe.ErrorRow = hocSinhList.Count(x => x.ErrorCode == -1);
            thongKe.NotErrorRow = hocSinhList.Count(x => x.ErrorCode != -1);

            return thongKe;
        }

        public async Task<int> DeleteImport(string nguoiThucHien)
        {
            try
            {
                var filterBuilder = Builders<HocSinhTmpModel>.Filter;

                var filters = new List<FilterDefinition<HocSinhTmpModel>>
                {
                    filterBuilder.Eq("NguoiTao", nguoiThucHien),
                };
                filters.RemoveAll(filter => filter == null);

                var combinedFilter = filterBuilder.And(filters);
                 var result = _mongoDatabase.GetCollection<HocSinhTmpModel>(_collectionHocSinhTmpName).DeleteMany(combinedFilter);

                return (int)HocSinhEnum.Success;
            }
            catch
            {
                return (int)HocSinhEnum.Fail;
            }
        }
    }
}