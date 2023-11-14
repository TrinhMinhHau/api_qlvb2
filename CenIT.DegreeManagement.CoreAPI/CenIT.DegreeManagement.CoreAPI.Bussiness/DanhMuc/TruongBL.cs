using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.XacMinhVanBang;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IO;
using static System.Net.WebRequestMethods;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc
{
    public class TruongBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public TruongBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        #region Truong

        /// <summary>
        /// Thêm trường
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(TruongInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)TruongEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)TruongEnum.ExistName;
            if (!KiemTraMaHDT(model.MaHeDaoTao)) return (int)TruongEnum.NotExistHDT;
            if (model.LaPhong == false)
            {
                if (!KiemTraHTDT(model.MaHinhThucDaoTao)) return (int)TruongEnum.NotExistHTDT;
            } else
            {
                if (IsSo(model.Id, model.DonViQuanLy)) return (int)TruongEnum.ExistSo;
            }

            try
            {
                var maxStt = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                        .Find(x => x.Xoa == false)
                        .SortByDescending(x => x.STT)
                        .Limit(1)
                        .FirstOrDefault()?.STT ?? 0;
                var truongModel = new TruongModel();
                ModelProvider.MapProperties(model, truongModel);
                truongModel.NgayTao = DateTime.Now;
                truongModel.NguoiTao = model.NguoiThucHien;
                truongModel.STT = maxStt + 1;
                truongModel.CauHinh.Nam = DateTime.Now.Year.ToString();
                // insert
                await _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).InsertOneAsync(truongModel);
                return (int)TruongEnum.Success;
            }
            catch
            {
                return (int)TruongEnum.Fail;
            }
        }

        /// <summary>
        /// Cập nhật trường
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(TruongInputModel model)
        {
            if (TrungMa(model.Ma, model.Id)) return (int)TruongEnum.ExistCode;
            if (TrungTen(model.Ten, model.Id)) return (int)TruongEnum.ExistName;
            if (!KiemTraMaHDT(model.MaHeDaoTao)) return (int)TruongEnum.NotExistHDT;
            if (model.LaPhong == false)
            {
                if (!KiemTraHTDT(model.MaHinhThucDaoTao)) return (int)TruongEnum.NotExistHTDT;
            }

            try
            {
                var filter = Builders<TruongModel>.Filter.And(
                                Builders<TruongModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<TruongModel>.Filter.Eq(hdt => hdt.Id, model.Id)
                              );

                var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                            .Find(filter).FirstOrDefault();
                if (truong == null)
                    return (int)TruongEnum.NotFound;

                ModelProvider.MapProperties(model, truong);
                truong.NguoiCapNhat = model.NguoiThucHien;
                truong.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                                        .ReplaceOneAsync(filter, truong);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)TruongEnum.Fail;
                }
                return (int)TruongEnum.Success;
            }
            catch
            {
                return (int)TruongEnum.Fail;
            }
        }

        /// <summary>
        /// Xóa trường
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<TruongModel>.Filter.And(
                                Builders<TruongModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<TruongModel>.Filter.Eq(hdt => hdt.Id, id)
                              );

                var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                            .Find(filter).FirstOrDefault();
                if (truong == null)
                    return (int)TruongEnum.NotFound;

                truong.Xoa = true;
                truong.NgayXoa = DateTime.Now;
                truong.NguoiXoa = nguoiThucHien;

                var updateResult = await _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                                        .ReplaceOneAsync(filter, truong);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)TruongEnum.Fail;
                }
                return (int)TruongEnum.Success;
            }
            catch
            {
                return (int)TruongEnum.Fail;
            }
        }

        /// <summary>
        /// GetAll trường
        /// </summary>
        /// <returns></returns>
        public List<TruongViewModel> GetAll(string idHTDT = null)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongViewModel>(_collectionNameTruong);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);
            var heDaoTaoCollection = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao);

            var htdt = hinhThucDaoTaoCollection.Find(x => x.Id == idHTDT).FirstOrDefault();

            var truongs = truongCollection.AsQueryable()
                .Where(t => t.Xoa == false && t.LaPhong == false)
                .OrderBy(hs => hs.Ten)
                .ToList();

            if (!string.IsNullOrEmpty(idHTDT))
            {
                truongs.Where(x=>x.HinhThucDaoTao == htdt.Ma).ToList();
            }

            return truongs;
        }

        public List<TruongViewModel> GetByMaHeDaoTao(string maHeDaoTao)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongViewModel>(_collectionNameTruong);

            var truongs = truongCollection.AsQueryable()
                .Where(t => t.Xoa == false && t.LaPhong == false && t.MaHeDaoTao == maHeDaoTao)
                .OrderBy(hs => hs.Ten)
                .ToList();

            return truongs;
        }

        /// <summary>
        /// GetAll trường
        /// </summary>
        /// <returns></returns>
        public List<TruongModel> GetAllHavePhong()
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
          

            var truongs = truongCollection.AsQueryable()
                .Where(t => t.Xoa == false)
                .OrderBy(hs => hs.Ten)
                .ToList();

            return truongs;
        }

        /// <summary>
        /// GetAll trường
        /// </summary>
        /// <returns></returns>
        public List<TruongModel> GetAllDonViCha()
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);

            var truongs = truongCollection.AsQueryable()
                .Where(t => t.Xoa == false && t.LaPhong == true)
                .OrderBy(hs => hs.Ten)
                .ToList();

            return truongs;
        }

        /// <summary>
        /// GetAll trường
        /// </summary>
        /// <returns></returns>
        public List<TruongModel> GetAllTruong(string idDonVi)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var donVi = truongCollection.Find(x => x.Id == idDonVi && x.Xoa == false).FirstOrDefault();
            if (donVi == null) return null;

            var truongs = truongCollection.AsQueryable()
                .Where(t => t.Xoa == false && t.DonViQuanLy == donVi.DonViQuanLy && t.LaPhong == false && t.IdCha == donVi.Id)
                .OrderBy(hs => hs.Ten)
                .ToList();

            return truongs;
        }

        /// <summary>
        /// GetSearch trường 
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<TruongViewModel> GetSearch(out int total, SearchParamModel modelSearch, string idDonVi)
        {
            var HDTs = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao).Find(h => true && h.Xoa == false).ToList();
            var HTDTs = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao).Find(ht => true && ht.Xoa == false).ToList();

            //Get dữ liệu từ nội dung tìm kiếm
            var clt = _mongoDatabase.GetCollection<TruongViewModel>(_collectionNameTruong)
                .Find(p => p.Ten.Contains(modelSearch.Search ?? "") && p.Xoa == false)
                .ToList()
                 .GroupJoin(
                          HDTs,
                          tr => tr.MaHeDaoTao,
                          hdt => hdt.Ma,
                          (tr, hdtGroup) =>
                          {
                              var hdt = hdtGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                              if (hdt == null)
                              {
                                  tr.HeDaoTao = null; // Create a new instance of SoGocModel
                              }
                              else
                              {
                                  tr.HeDaoTao = hdt.Ten;
                              }
                              return tr;
                          }
                      )
               .GroupJoin(
                          HTDTs,
                          tr => tr.MaHinhThucDaoTao,
                          htdt => htdt.Ma,
                          (tr, htdtGroup) =>
                          {
                              var htdt = htdtGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                              if (htdt == null)
                              {
                                  tr.HinhThucDaoTao = null; // Create a new instance of SoGocModel
                              }
                              else
                              {
                                  tr.HinhThucDaoTao = htdt.Ten;
                              }
                              return tr;
                          }
                      ).ToList();

            if (!string.IsNullOrEmpty(idDonVi))
            {
              clt = clt.Where(x =>x.Xoa == false && (x.Id == idDonVi || x.IdCha == idDonVi)).ToList();
            }

            total = clt.Count();


            // order & OrderDir
            switch (modelSearch.Order.ToLower())
            {
                case "0":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ma).ToList() : clt.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "1":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ten).ToList() : clt.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "ma":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ma).ToList() : clt.OrderByDescending(x => x.Ma).ToList();
                    break;
                case "ten":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ten).ToList() : clt.OrderByDescending(x => x.Ten).ToList();
                    break;
            }

            if(modelSearch.PageSize != -1)
            {
                clt = clt.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();

            }

            return clt;
        }

        /// <summary>
        /// Lấy trường theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TruongViewModel? GetById(string id)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongViewModel>(_collectionNameTruong);
            var heDaoTaoCollection = _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);

            var truongVM = truongCollection.Find(x => x.Xoa == false && x.Id == id).ToList()
                                   .GroupJoin(
                          heDaoTaoCollection.AsQueryable(),
                          tr => tr.MaHeDaoTao,
                          hdt => hdt.Ma,
                          (tr, hdtGroup) =>
                          {
                              var hdt = hdtGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                              if (hdt == null)
                              {
                                  tr.HeDaoTao = null; // Create a new instance of SoGocModel
                              }
                              else
                              {
                                  tr.HeDaoTao = hdt.Ten;
                              }
                              return tr;
                          }
                      )
               .GroupJoin(
                          hinhThucDaoTaoCollection.AsQueryable(),
                          tr => tr.MaHinhThucDaoTao,
                          htdt => htdt.Ma,
                          (tr, htdtGroup) =>
                          {
                              var htdt = htdtGroup.FirstOrDefault(); // Use FirstOrDefault to handle null case
                              if (htdt == null)
                              {
                                  tr.HinhThucDaoTao = null; // Create a new instance of SoGocModel
                              }
                              else
                              {
                                  tr.HinhThucDaoTao = htdt.Ten;
                              }
                              return tr;
                          }
                      ).FirstOrDefault();
            return truongVM;
        }

        public TruongViewModel? GetPhong(string idDonVi)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongViewModel>(_collectionNameTruong);
            var donVi = truongCollection.Find(x => x.Id == idDonVi && x.Xoa == false).FirstOrDefault();
            if (donVi == null) return null;

            var truongVM = truongCollection.Find(x => x.Xoa == false  && x.LaPhong == true && donVi.IdCha == x.Id)
                                  .FirstOrDefault();
            return truongVM;
        }

        public TruongViewModel? GetDonViQuanLySo()
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongViewModel>(_collectionNameTruong);
            var donVi = truongCollection.Find(x=> x.Xoa == false && x.DonViQuanLy == 1 && x.LaPhong == true).FirstOrDefault();
            if (donVi == null) return null;
            return donVi;
        }


        /// <summary>
        /// Lấy tất cả các đơn vị theo id đơn vị quản lý
        /// </summary>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public List<TruongModel>? GetAllDonViByUsername(string idDonVi)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var donVis = new List<TruongModel>();
            if (string.IsNullOrEmpty(idDonVi))
            {  
                // trường hợp là super admin không có idDonvi sẽ lấy tất cả các đơn vị
               donVis = truongCollection.Find(x => x.Xoa == false)
                    .ToList()
                    .OrderByDescending(x=>x.LaPhong)
                    .ThenBy(x=>x.DonViQuanLy)
                    .ThenBy(x=>x.Ten)
                    .ToList();
            } else
            {
                // trường hợp là đơn vị quản lý thì lấy đơn vị đó và các đơn vị con
                var donVi = truongCollection.Find(x => x.Xoa == false && x.Id == idDonVi).FirstOrDefault();
                if (donVi == null) return null;
                donVis.Add(donVi);
                donVis.AddRange(truongCollection.Find(x => x.IdCha == donVi.Id && x.Xoa == false).ToList());

                donVis.OrderByDescending(x => x.LaPhong)
                    .ThenBy(x => x.DonViQuanLy)
                    .ThenBy(x => x.Ten).ToList();
            }
            return donVis;
        }



        #endregion

        #region CauHinhTheoTruong
        /// <summary>
        /// Thêm mới cau hinh
        /// </summary>
        /// <param name="idTruong"></param>
        /// <param name="cauHinhModel"></param>
        /// <returns></returns>
        public int SaveCauHinh(string idTruong, CauHinhInputModel cauHinhModel)
        {
            try
            {
                var cauHinh = GetById(idTruong).CauHinh;
                cauHinh.MaCoQuanCapBang = cauHinhModel.MaCoQuanCapBang;
                cauHinh.MaCoQuanCapBang = cauHinhModel.MaCoQuanCapBang;
                cauHinh.LogoDonvi = cauHinhModel.LogoDonvi;
                cauHinh.TenCoQuanCapBang = cauHinhModel.TenCoQuanCapBang;
                cauHinh.TenDiaPhuongCapBang = cauHinhModel.TenDiaPhuongCapBang;
                cauHinh.TenUyBanNhanDan = cauHinhModel.TenUyBanNhanDan;
                cauHinh.HoTenNguoiKySoGoc = cauHinhModel.HoTenNguoiKySoGoc;
                cauHinh.SoBatDau = cauHinhModel.SoBatDau;
                cauHinh.SoKyTu = cauHinhModel.SoKyTu;
                cauHinh.TienToBanSao = cauHinhModel.TienToBanSao;
                cauHinh.HieuTruong = cauHinhModel.HieuTruong;
                cauHinh.NgayBanHanh = cauHinhModel.NgayBanHanh;
                cauHinh.NgayBanHanh = cauHinhModel.NgayBanHanh;
                cauHinh.TenDiaPhuong = cauHinhModel.TenDiaPhuong;

                // tìm trường cần cập nhật
                var filter = Builders<TruongModel>.Filter.Eq(t => t.Id, idTruong);

                //giá trị cập nhật
                var update = Builders<TruongModel>.Update.Set(t => t.CauHinh, cauHinh);

                // cập nhật
                UpdateResult u = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).UpdateOne(filter, update);
                if (u.ModifiedCount == 0)
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Lấy thông tin cấu hình theo id trường
        /// </summary>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public CauHinhModel? GetCauHinhByIdTruong(string idTruong)
        {
            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
              .Find(n => true && n.Id == idTruong && n.Xoa == false)
              .FirstOrDefault();
            if (truong == null)
            {
                return null;
            }
            return truong.CauHinh;
        }


        public CauHinhXacMinhVanBangModel? GetCauHinhXacMinhByIdDonVi(string idDonVi)
        {
            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
              .Find(n => true && n.Id == idDonVi && n.Xoa == false && n.DonViQuanLy > (int)TypeUnitEnum.Truong)
              .FirstOrDefault();

            if (truong == null)
            {
                return null;
            }

            var cauHinhXacMinh = new CauHinhXacMinhVanBangModel()
            {
                NguoiKyBang = truong.CauHinh.HoTenNguoiKySoGoc,
                DiaPhuongCapBang = truong.CauHinh.TenDiaPhuongCapBang,
                CoQuanCapBang = truong.CauHinh.TenCoQuanCapBang,
                UyBanNhanDan = truong.CauHinh.TenUyBanNhanDan,
            };

            return cauHinhXacMinh;
        }


        /// <summary>
        /// Lấy thông tin cấu hình theo mã trường
        /// </summary>
        /// <param name="maTruong"></param>
        /// <returns></returns>
        public CauHinhModel? GetCauHinhByMaTruong(string maTruong)
        {
            var truong = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
              .Find(n => true && n.Ma == maTruong && n.Xoa == false)
              .FirstOrDefault();
            if (truong == null)
            {
                return null;
            
            }

            return truong.CauHinh;
        }

        public int UpdateSoDonYeuCau(string idTruong)
        {
            var cauHinh = GetById(idTruong).CauHinh;
            cauHinh.SoDonYeuCau += 1;


            // tìm trường cần cập nhật
            var filter = Builders<TruongModel>.Filter.Eq(t => t.Id, idTruong);

            //giá trị cập nhật
            var update = Builders<TruongModel>.Update.Set(t => t.CauHinh, cauHinh);

            UpdateResult u = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).UpdateOne(filter, update);
            return 1;
        }

        public int UpdateCauHinhSoVaoSo(UpdateCauHinhSoVaoSoInputModel model)
        {
            var cauHinh = GetById(model.IdTruong).CauHinh;

            if(model.Nam != cauHinh.Nam)
            {
                cauHinh.Nam = model.Nam;
                if(model.LoaiHanhDong == SoVaoSoEnum.SoVaoSoGoc)
                {
                    cauHinh.DinhDangSoThuTuSoGoc = model.DinhDangSoThuTuSoGoc;
                    cauHinh.DinhDangSoThuTuCapLai = 0;
                    cauHinh.SoDonYeuCau = 0;
                }

                if (model.LoaiHanhDong == SoVaoSoEnum.SoVaoSoBanSao)
                {
                    cauHinh.DinhDangSoThuTuSoGoc = 0;
                    cauHinh.DinhDangSoThuTuCapLai = 0;
                    cauHinh.SoDonYeuCau = model.SoDonYeuCau;
                }
                else
                {
                    cauHinh.DinhDangSoThuTuSoGoc = 0;
                    cauHinh.DinhDangSoThuTuCapLai = model.DinhDangSoThuTuCapLai;
                    cauHinh.SoDonYeuCau = 0;
                }
            }
            else
            {
                if (model.LoaiHanhDong == SoVaoSoEnum.SoVaoSoGoc) cauHinh.DinhDangSoThuTuSoGoc += model.DinhDangSoThuTuSoGoc;
                if (model.LoaiHanhDong == SoVaoSoEnum.SoVaoSoBanSao) cauHinh.SoDonYeuCau += model.SoDonYeuCau;
                if (model.LoaiHanhDong == SoVaoSoEnum.SoVaoSoCapLai) cauHinh.DinhDangSoThuTuCapLai += model.DinhDangSoThuTuCapLai;
            }

            // tìm trường cần cập nhật
            var filter = Builders<TruongModel>.Filter.Eq(t => t.Id, model.IdTruong);

            //giá trị cập nhật
            var update = Builders<TruongModel>.Update.Set(t => t.CauHinh, cauHinh);

            UpdateResult u = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).UpdateOne(filter, update);
            return 1;
        }
        #endregion

        #region private
        private bool TrungMa(string Ma, string Id)
        {
            var mt = Id == null
                ? _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(n => true && n.Xoa == false && n.Ma.ToLower() == Ma.ToLower() && n.Id != Id).FirstOrDefault();
            return mt != null ? true : false;
        }

        private bool IsSo(string Id, int donViQuanLy)
        {
          
            if(donViQuanLy == (int)TypeUnitEnum.So)
            {
                var mt = Id == null
                ? _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                                  .Find(t => t.DonViQuanLy == (int)TypeUnitEnum.So && t.LaPhong == true && t.Xoa == false).FirstOrDefault()
                : _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(n => true && n.Xoa == false && n.LaPhong == true && n.DonViQuanLy == (int)TypeUnitEnum.So && n.Id != Id).FirstOrDefault();
                    return mt != null ? true : false;
            }

            return false;
        }

        private bool TrungTen(string Ten, string Id)
        {
            var mt = Id == null
                ? _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower() && n.Id != Id).FirstOrDefault();
            return mt != null ? true : false;
        }
        private bool TrungUrl(string Url, string Id)
        {
            var mt = Id == null
                ? _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(n => true && n.Xoa == false && n.URL == Url).FirstOrDefault()
                : _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(n => true && n.Xoa == false && n.URL == Url && n.Id != Id).FirstOrDefault();
            return mt != null ? true : false;
        }

        private bool KiemTraMaHDT(string maHDT)
        {
            var heDaoTao =  _mongoDatabase.GetCollection<HeDaoTaoModel>(_collectionNamHeDaoTao)
                                           .Find(h=>h.Xoa == false && h.Ma == maHDT).FirstOrDefault();
            return heDaoTao != null ? true : false;
        }

        private bool KiemTraHTDT(string maHTDT)
        {
            var htdt = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao)
                                           .Find(h => h.Xoa == false && h.Ma == maHTDT).FirstOrDefault();
            return htdt != null ? true : false;
        }

        #endregion
    }
}