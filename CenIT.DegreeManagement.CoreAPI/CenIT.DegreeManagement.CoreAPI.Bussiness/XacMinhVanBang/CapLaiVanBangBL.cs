//using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
//using CenIT.DegreeManagement.CoreAPI.Core.Enums;
//using CenIT.DegreeManagement.CoreAPI.Core.Models;
//using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
//using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
//using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
//using Microsoft.Extensions.Configuration;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang
//{
//    public class CapLaiVanBangBL : ConfigAppBussiness
//    {
//        private string _connectionString;
//        private IConfiguration _configuration;
//        private readonly string dbName = "nhatrangkha";

//        private IMongoDatabase _mongoDatabase;

//        public CapLaiVanBangBL(IConfiguration configuration)
//        {
//            _configuration = configuration;
//            _connectionString = _configuration["ConnectionStrings:gddt"];

//            //Dùng MongoClient để kết nối tới Server
//            MongoClient client = new MongoClient(_connectionString);

//            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
//            _mongoDatabase = client.GetDatabase(dbName);
//        }

//        public async Task<int> Create(CapLaiVangBangInputModel model)
//        {

//            try
//            {
//                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
//                var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.Id == model.IdHocSinh && t.TrangThai == TrangThaiHocSinhEnum.DaNhanBang).FirstOrDefault();

//                if (hocSinh == null) return (int)LichSuCapLaiVanBangEnum.NotExist;

//                var CapLaiVB = new CapLaiVanBangModel()
//                {
//                    Id = Guid.NewGuid().ToString(),
//                    PathFileVanBan = model.PathFileVanBan,
//                    LyDo = model.LyDo,
//                    NguoiTao = model.NguoiThucHien,
//                    NgayTao = DateTime.Now
//                };

//                hocSinh.LichSuCapLaiVanBang.Add(CapLaiVB);

//                var updateResult = await conllectionHocSinh.ReplaceOneAsync(h => h.Id == hocSinh.Id, hocSinh);

//                if (updateResult.ModifiedCount == 0)
//                {
//                    return (int)LichSuCapLaiVanBangEnum.Fail;
//                }
//                return (int)LichSuCapLaiVanBangEnum.Success;

//            }
//            catch
//            {
//                return (int)LichSuCapLaiVanBangEnum.Fail;

//            }
//        }

//        public List<CapLaiVanBangModel> GetSerachCapLaiVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
//        {
//            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
//            var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.Id == idHocSinh).FirstOrDefault();

//            var CapLaiVangBangs = hocSinh.LichSuCapLaiVanBang.OrderBy(x => x.NgayTao).ToList();

//            total = CapLaiVangBangs.Count;

//            switch (modelSearch.Order)
//            {
//                case "0":
//                    CapLaiVangBangs = modelSearch.OrderDir.ToUpper() == "ASC"
//                        ? CapLaiVangBangs.OrderBy(x => x.NgayTao).ToList()
//                        : CapLaiVangBangs.OrderByDescending(x => x.NgayTao).ToList();
//                    break;
//            }
//            if (modelSearch.PageSize > 0)
//            {
//                CapLaiVangBangs = CapLaiVangBangs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
//            }
//            return CapLaiVangBangs;
//        }

//        public CapLaiVanBangModel GetCapLaiVanBangById(string cccd, string idLichSuCapLai)
//        {
//            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
//            var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.CCCD == cccd).FirstOrDefault();

//            var CapLaiVangBangs = hocSinh.LichSuCapLaiVanBang.OrderBy(x => x.NgayTao).ToList();
//            var CapLaiVangBang = CapLaiVangBangs.Where(x => x.Id == idLichSuCapLai).FirstOrDefault();


//            return CapLaiVangBang;
//        }
//    }
//}
