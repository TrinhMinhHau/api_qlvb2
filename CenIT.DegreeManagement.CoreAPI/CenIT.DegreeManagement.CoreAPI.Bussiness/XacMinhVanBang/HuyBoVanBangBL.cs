using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang
{
    public class HuyBoVanBangBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public HuyBoVanBangBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<int> Create(HuyBoVangBangInputModel model)
        {

            try
            {
                var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
                var conllectionHuyBo = _mongoDatabase.GetCollection<LichSuHuyBoModel>(_collectionHuyBoName);

                var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.Id == model.IdHocSinh).FirstOrDefault();

                if (hocSinh == null) return (int)LichSuHuyBoVanBangEnum.NotExist;

                var huyBoVB = new LichSuHuyBoModel()
                {
                    IdHocSinh = hocSinh.Id,
                    PathFileVanBan = model.PathFileVanBan,
                    LyDo = model.LyDo,
                    NguoiTao = model.NguoiThucHien,
                    NgayTao = DateTime.Now
                };

                hocSinh.TrangThai = TrangThaiHocSinhEnum.HuyBo;
                await _mongoDatabase.GetCollection<LichSuHuyBoModel>(_collectionHuyBoName).InsertOneAsync(huyBoVB);
                var updateResult = await conllectionHocSinh.ReplaceOneAsync(h => h.Id == hocSinh.Id, hocSinh);

                if (updateResult.ModifiedCount == 0)
                {
                    return (int)LichSuHuyBoVanBangEnum.Fail;
                }
                return (int)LichSuHuyBoVanBangEnum.Success;

            }
            catch
            {
                return (int)LichSuHuyBoVanBangEnum.Fail;

            }
        }

        public List<LichSuHuyBoViewModel> GetSerachHuyBoVanBangByIdHocSinh(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var conllectionHuyBo = _mongoDatabase.GetCollection<LichSuHuyBoViewModel>(_collectionHuyBoName);

            var hocSinhs = conllectionHuyBo.Find(t => t.Xoa == false && t.IdHocSinh == idHocSinh)
                                .ToList()
                                .Join(
                                      conllectionHocSinh.AsQueryable(),
                                      h => h.IdHocSinh,
                                      hs => hs.Id,
                                      (h, hs) =>
                                      {
                                          h.HocSinh = hs;
                                          return h;
                                      }
                                  ).ToList();


            total = hocSinhs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.NgayTao).ToList()
                        : hocSinhs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                hocSinhs = hocSinhs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinhs;
        }


        public List<LichSuHuyBoViewModel> GetSerachHuyBoVanBang(out int total, SearchParamModel modelSearch)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
            var conllectionHuyBo = _mongoDatabase.GetCollection<LichSuHuyBoViewModel>(_collectionHuyBoName);

            var hocSinhs = conllectionHuyBo.Find(t => t.Xoa == false)
                                .ToList()
                                .Join(
                                      conllectionHocSinh.AsQueryable(),
                                      h => h.IdHocSinh,
                                      hs => hs.Id,
                                      (h, hs) =>
                                      {
                                          h.HocSinh = hs;
                                          return h;
                                      }
                                  ).ToList();


            total = hocSinhs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinhs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinhs.OrderBy(x => x.NgayTao).ToList()
                        : hocSinhs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                hocSinhs = hocSinhs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinhs;
        }

        //public HuyBoVangBangModel GetHuyBoVanBangById(string cccd, string idLichSuHuyBo)
        //{
        //    var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);
        //    var hocSinh = conllectionHocSinh.Find(t => t.Xoa == false && t.CCCD == cccd).FirstOrDefault();

        //    var huyBoVangBangs = hocSinh.LichSuHuyBoVanBang.OrderBy(x => x.NgayTao).ToList();
        //    var huyBoVangBang = huyBoVangBangs.Where(x => x.Id == idLichSuHuyBo).FirstOrDefault();


        //    return huyBoVangBang;
        //}

    }
}
