using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.XacMinhVanBang
{
    public class ChinhSuaVanBangBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public ChinhSuaVanBangBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public async Task<HocSinhResult> Create(ChinhSuaVanBangInputModel model, TruongModel donVi)
        {
            try
            {
                //var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);
                var conllectionPhoGoc = _mongoDatabase.GetCollection<PhoiGocModel>(_collectionNamePhoiGoc);
                var conllectionPhuLuc = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName);


                var phoiGocDangSuDung = conllectionPhoGoc.Find(t => t.Xoa == false && t.TinhTrang == TinhTrangPhoiEnum.DangSuDung && t.MaHeDaoTao == donVi.MaHeDaoTao).FirstOrDefault();

                var hocSinh = GetHocSinhById(model.IdHocSinh);
                var phuLucMoiNhat = conllectionPhuLuc
                               .Find(x => x.IdHocSinh == model.IdHocSinh)
                               .ToList()
                               .OrderByDescending(x => x.NgayTao)
                               .FirstOrDefault();

                if(phuLucMoiNhat != null)
                {
                    ModelProvider.MapProperties(phuLucMoiNhat, hocSinh);
                    hocSinh.HoiDong = phuLucMoiNhat.HoiDongThi;
                    hocSinh.DanhMucTotNghiep.NgayCapBang = phuLucMoiNhat.NgayCap;
                    hocSinh.DanhMucTotNghiep.IdNamThi = phuLucMoiNhat.IdNamThi;
                    hocSinh.MaHinhThucDaotao = phuLucMoiNhat.MaHTDT;
                    hocSinh.Id = phuLucMoiNhat.IdHocSinh;
                    hocSinh.SoHieuVanBang = string.IsNullOrEmpty(phuLucMoiNhat.SoHieuVanBangCapLai) ? phuLucMoiNhat.SoHieuVanBangCu : phuLucMoiNhat.SoHieuVanBangCapLai;
                    hocSinh.SoVaoSoCapBang = string.IsNullOrEmpty(phuLucMoiNhat.SoVaoSoCapBangCapLai) ? phuLucMoiNhat.SoVaoSoCapBangCu : phuLucMoiNhat.SoVaoSoCapBangCapLai;
                }

                if (hocSinh == null) return new HocSinhResult() { MaLoi = (int)LichSuChinhSuaVanBangEnum.NotExist };
                int countEdit = 0;
                string noiDungChinhSua = NoiDungChinhSua(model, hocSinh, out countEdit);

                if(countEdit == 0) return new HocSinhResult() { MaLoi = (int)LichSuChinhSuaVanBangEnum.NotEdit };

                var chinhSuaVanBang = new PhuLucSoGocModel();
                ModelProvider.MapProperties(model, chinhSuaVanBang);
                chinhSuaVanBang.NguoiTao = model.NguoiThucHien;
                chinhSuaVanBang.NgayTao = DateTime.Now;
                chinhSuaVanBang.SoHieuVanBangCu = hocSinh.SoHieuVanBang;
                chinhSuaVanBang.SoVaoSoCapBangCu = hocSinh.SoVaoSoCapBang;
                chinhSuaVanBang.IdHocSinh = hocSinh.Id;
                chinhSuaVanBang.NoiDungChinhSua = noiDungChinhSua;
                string nam = donVi.CauHinh.Nam;
                if (model.LoaiHanhDong == LoaiHanhDongEnum.CapLai)
                {


                    int soBatDau = int.Parse(phoiGocDangSuDung.SoBatDau) + phoiGocDangSuDung.SoLuongPhoiDaSuDung;
                    int soBatDauLength = phoiGocDangSuDung.SoBatDau.Length;

                    chinhSuaVanBang.SoHieuVanBangCapLai = $"{phoiGocDangSuDung.SoHieuPhoi}{soBatDau.ToString().PadLeft(soBatDauLength, '0')}";
                    chinhSuaVanBang.IdPhoiGoc = phoiGocDangSuDung.Id;
                    int stt = 0;
             
                    if (donVi.CauHinh.Nam == DateTime.Now.Year.ToString())
                    {
                        stt = donVi.CauHinh.DinhDangSoThuTuCapLai + 1;
                    }
                    else
                    {
                        nam = DateTime.Now.Year.ToString();
                        stt += 1;
                    }

                    chinhSuaVanBang.SoVaoSoCapBangCapLai = $"{hocSinh.SoVaoSoCapBang}/{stt.ToString().PadLeft(3, '0')}";
                }

                await _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName).InsertOneAsync(chinhSuaVanBang);
                if (chinhSuaVanBang.Id != null)
                {
                    return new HocSinhResult() { MaLoi = (int)LichSuChinhSuaVanBangEnum.Success, Nam = nam, IdPhoi = phoiGocDangSuDung.Id };

                }
                return new HocSinhResult() { MaLoi = (int)LichSuChinhSuaVanBangEnum.Fail};
            }
            catch
            {
                return new HocSinhResult() { MaLoi = (int)LichSuChinhSuaVanBangEnum.Fail };

            }
        }

        public List<PhuLucSoGocModel> GetSerachChinhSuaVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            var conllectionPhuLucSoGoc = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName);
            var hocSinh = conllectionPhuLucSoGoc.Find(t => t.Xoa == false && t.IdHocSinh == idHocSinh && t.LoaiHanhDong == LoaiHanhDongEnum.ChinhSua).ToList();

            //var chinhSuaVanBang = hocSinh.LichSuChinhSuaVanBang.OrderBy(x=>x.NgayTao).ToList();

            total = hocSinh.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.NgayTao).ToList()
                        : hocSinh.OrderByDescending(x => x.NgayTao).ToList();
                    break;

                case "1":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.NgayTao).ToList()
                        : hocSinh.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                hocSinh = hocSinh.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinh;
        }

        public LichSuChinhSuaModel GetChinhSuaVanBangById(string idPhuLuc)
        {
            var conllectionPhuLucSoGoc = _mongoDatabase.GetCollection<LichSuChinhSuaModel>(_collectionPhuLucSoGocName);
            var hocSinh = conllectionPhuLucSoGoc.Find(t => t.Xoa == false && t.Id == idPhuLuc).ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var htdtCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);
            hocSinh = hocSinh
                      .Join(
                          htdtCollection.AsQueryable(),
                          hs => hs.MaHTDT,
                          htdt => htdt.Ma,
                          (hs, htdt) =>
                          {
                              hs.TenHinhThucDaoTao = htdt.Ten;
                              return hs;
                          }
                      )
                      .Join(
                          namThiCollection.AsQueryable(),
                          hs => hs.IdNamThi,
                          nt => nt.Id,
                          (hs, nt) =>
                          {
                              hs.NamThi = nt.Ten;
                              hs.KhoaThi = nt.KhoaThis.Where(x=>x.Id == hs.IdKhoaThi).FirstOrDefault().Ngay;
                              return hs;
                          }
                      ).ToList();
                 
            return hocSinh.FirstOrDefault();
        }

        public PhuLucSoGocModel GetThongTinChinhSuaMoiNhat(string idHocSinh)
        {

            var phuLucMoiNhat = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName)
                                 .Find(x => x.Xoa == false && x.IdHocSinh == idHocSinh)
                                 .SortByDescending(x => x.NgayTao)
                                 .FirstOrDefault();

           
            return phuLucMoiNhat;
        }

        #region Phụ Lục
        public List<PhuLucSoGocViewModel> GetSerachPhuLuc(out int total, PhuLucSoGocSearchModel searchModel)
        {
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);


            var phuLucs = _mongoDatabase.GetCollection<PhuLucSoGocViewModel>(_collectionPhuLucSoGocName)
                                .Find(x=>x.Xoa == false)
                                .ToList();

            phuLucs = phuLucs.Join(
                          hocSinhCollection.AsQueryable(),
                          hs => hs.IdHocSinh,
                          h => h.Id,
                          (hs, h) =>
                          {
                              hs.HocSinh = h;
                              return hs;
                          }
                      ).ToList();

            if (!string.IsNullOrEmpty(searchModel.IdDanhMucTotNghiep))
            {

                phuLucs = phuLucs.Where(x => x.HocSinh.IdDanhMucTotNghiep == searchModel.IdDanhMucTotNghiep).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel.IdTruong))
            {

                phuLucs = phuLucs.Where(x => x.HocSinh.IdTruong == searchModel.IdTruong).ToList();
            }

            total = phuLucs.Count;

            switch (searchModel.Order)
            {
                case "0":
                    phuLucs = searchModel.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    phuLucs = searchModel.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (searchModel.PageSize > 0)
            {
                phuLucs = phuLucs.Skip(searchModel.PageSize * searchModel.StartIndex).Take(searchModel.PageSize).ToList();
            }
            return phuLucs;
        }

        #endregion

        #region Cổng thông tin
        public List<PhuLucSoGocModel> GetSerachPhuLucChinhSuaVanBang(out int total, SearchParamModel modelSearch)
        {

            var phuLucs = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName)
                                .Find(x => x.Xoa == false && x.LoaiHanhDong == LoaiHanhDongEnum.ChinhSua)
                                .ToList();
            total = phuLucs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    phuLucs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    phuLucs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                phuLucs = phuLucs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return phuLucs;
        }


        public List<PhuLucSoGocModel> GetSerachPhuLucCapLaiVanBang(out int total, SearchParamModel modelSearch)
        {

            var phuLucs = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName)
                                .Find(x => x.Xoa == false && x.LoaiHanhDong == LoaiHanhDongEnum.CapLai)
                                .ToList();
            total = phuLucs.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    phuLucs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
                case "1":
                    phuLucs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? phuLucs.OrderBy(x => x.NgayTao).ToList()
                        : phuLucs.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                phuLucs = phuLucs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return phuLucs;
        }
        #endregion


        #region CapLai

        public List<PhuLucSoGocModel> GetSerachCapLaiVanBang(out int total, string idHocSinh, SearchParamModel modelSearch)
        {
            var conllectionPhuLucSoGoc = _mongoDatabase.GetCollection<PhuLucSoGocModel>(_collectionPhuLucSoGocName);
            var hocSinh = conllectionPhuLucSoGoc.Find(t => t.Xoa == false && t.IdHocSinh == idHocSinh && t.LoaiHanhDong == LoaiHanhDongEnum.CapLai).ToList();

            //var chinhSuaVanBang = hocSinh.LichSuChinhSuaVanBang.OrderBy(x=>x.NgayTao).ToList();

            total = hocSinh.Count;

            switch (modelSearch.Order)
            {
                case "0":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.NgayTao).ToList()
                        : hocSinh.OrderByDescending(x => x.NgayTao).ToList();
                    break;

                case "1":
                    hocSinh = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? hocSinh.OrderBy(x => x.NgayTao).ToList()
                        : hocSinh.OrderByDescending(x => x.NgayTao).ToList();
                    break;
            }
            if (modelSearch.PageSize > 0)
            {
                hocSinh = hocSinh.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinh;
        }

        #endregion

        private bool KiemTraSoHieuVanBang(string soHieuVanBang, string idHocSinh)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);

            var checkSoHieuVanBang = conllectionHocSinh.Find(t => t.Xoa == false && t.Id != idHocSinh && t.SoHieuVanBang == soHieuVanBang).FirstOrDefault();

            return checkSoHieuVanBang != null ? true : false;
        }

        private bool KiemTraSoVaoSo(string soVaoSo, string idHocSinh)
        {
            var conllectionHocSinh = _mongoDatabase.GetCollection<HocSinhModel>(_collectionHocSinhName);

            var checkSoHieuVanBang = conllectionHocSinh.Find(t => t.Xoa == false && t.Id != idHocSinh && t.SoVaoSoCapBang == soVaoSo).FirstOrDefault();

            return checkSoHieuVanBang != null ? true : false;
        }

        private HocSinhViewModel? GetHocSinhById(string id)
        {
            var filter = Builders<HocSinhViewModel>.Filter.And(
                           Builders<HocSinhViewModel>.Filter.Eq(hs => hs.Xoa, false),
                           Builders<HocSinhViewModel>.Filter.Eq(hs => hs.Id, id)
                         );

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var hocSinhCollection = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var htdtCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);

            var hocSinhs = hocSinhCollection.Find(filter).ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var hocSinh = hocSinhs
                      .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong;
                              return hs;
                          }
                      )
                      .Join(
                          danhMucTotNghiepCollection.AsQueryable(),
                          hs => hs.IdDanhMucTotNghiep,
                          dmtn => dmtn.Id,
                          (hs, dmtn) =>
                          {
                              hs.DanhMucTotNghiep = dmtn;
                              return hs;
                          }
                      )
                      .Join(
                          htdtCollection.AsQueryable(),
                          hs => hs.DanhMucTotNghiep.IdHinhThucDaoTao,
                          htdt => htdt.Id,
                          (hs, htdt) =>
                          {
                              hs.MaHinhThucDaotao = htdt.Ma;
                              return hs;
                          }
                      )
                      .Join(
                            namThiCollection.AsQueryable(),
                            hs => hs.DanhMucTotNghiep.IdNamThi,
                            nt => nt.Id,
                            (hs, n) =>
                            {
                                hs.NamThi = n.Ten;
                                hs.KhoaThi = n.KhoaThis.Where(x => x.Id == hs.IdKhoaThi).FirstOrDefault().Ngay;
                                return hs;
                            }
                        )
                      .FirstOrDefault();

            return hocSinh;
        }

        private string NoiDungChinhSua( ChinhSuaVanBangInputModel newModel, HocSinhViewModel oldModel, out int countEdit)
        {
            string noiDungChinhSua = "Thay đổi:\n";
            int edits = 0;
            if (oldModel.HoTen != newModel.HoTen)
            {
                noiDungChinhSua += $"Họ tên:\n + {oldModel.HoTen} => {newModel.HoTen}\n";
                edits += 1;
            }

            if (oldModel.CCCD != newModel.CCCD)
            {
                noiDungChinhSua += $"CCCD:\n + {oldModel.CCCD} => {newModel.CCCD}\n";
                edits += 1;
            }

            if (oldModel.NgaySinh.Date != newModel.NgaySinh.Date)
            {
                noiDungChinhSua += $"Ngày sinh:\n + {oldModel.NgaySinh.ToString("dd/MM/yyyy")} => {newModel.NgaySinh.ToString("dd/MM/yyyy")}\n";
                edits += 1;
            }

            if (oldModel.GioiTinh != newModel.GioiTinh)
            {
                string oldGender = oldModel.GioiTinh == true ? "Nam" : "Nữ";
                string newGender = newModel.GioiTinh == true ? "Nam" : "Nữ";

                noiDungChinhSua += $"Giới tính:\n + {oldGender} => {newGender}\n";
                edits += 1;
            }

            if (oldModel.NoiSinh != newModel.NoiSinh)
            {
             
                noiDungChinhSua += $"Nơi sinh:\n + {oldModel.NoiSinh} => {newModel.NoiSinh}\n";
                edits += 1;
            }

            if (oldModel.DanToc != newModel.DanToc)
            {

                noiDungChinhSua += $"Dân tộc:\n + {oldModel.DanToc} => {newModel.DanToc}\n";
                edits += 1;
            }

            if (oldModel.SoVaoSoCapBang != newModel.SoVaoSoCapBang)
            {

                noiDungChinhSua += $"Số vào sổ cấp bằng:\n + {oldModel.SoVaoSoCapBang} => {newModel.SoVaoSoCapBang}\n";
                edits += 1;
            }

            if (oldModel.SoHieuVanBang != newModel.SoHieuVanBang)
            {

                noiDungChinhSua += $"Số hiệu:\n + {oldModel.SoHieuVanBang} => {newModel.SoHieuVanBang}\n";
                edits += 1;
            }

            if (oldModel.HoiDong != newModel.HoiDongThi && !string.IsNullOrEmpty(oldModel.HoiDong))
            {

                noiDungChinhSua += $"Hội đồng thi:\n + {oldModel.HoiDong} => {newModel.HoiDongThi}\n";
                edits += 1;
            }

            if (oldModel.XepLoai != newModel.XepLoai)
            {

                noiDungChinhSua += $"Xếp loại:\n + {oldModel.XepLoai} => {newModel.XepLoai}\n";
                edits += 1;
            }

            if (oldModel.DanhMucTotNghiep.IdNamThi != newModel.IdNamThi)
            {
                var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
                var oldNamThi = namThiCollection.Find(x => x.Xoa == false && x.Id == oldModel.DanhMucTotNghiep.IdNamThi).FirstOrDefault();
                var newNamThi = namThiCollection.Find(x => x.Xoa == false && x.Id == newModel.IdNamThi).FirstOrDefault();

                noiDungChinhSua += $"Năm thi:\n + {oldNamThi.Ten} => {newNamThi.Ten}\n";
                noiDungChinhSua += $"Khóa thi:\n + {oldNamThi.KhoaThis.Where(x=>x.Id == oldModel.IdKhoaThi).FirstOrDefault().Ngay.Date.ToString()} " +
                    $"                                      => {oldNamThi.KhoaThis.Where(x => x.Id == newModel.IdKhoaThi).FirstOrDefault().Ngay.Date.ToString()}\n";

                edits += 1;
            }

            if (oldModel.DanhMucTotNghiep.NgayCapBang.Date != newModel.NgayCap.Date)
            {
                noiDungChinhSua += $"Ngày cấp:\n + {oldModel.DanhMucTotNghiep.NgayCapBang.ToString("dd/MM/yyyy")} => {newModel.NgayCap.ToString("dd/MM/yyyy")}\n";
                edits += 1;
            }

            if (oldModel.MaHinhThucDaotao != newModel.MaHTDT)
            {
                var htdtCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);
                var oldHtdt = htdtCollection.Find(x => x.Xoa == false && x.Ma == oldModel.MaHinhThucDaotao).FirstOrDefault();
                var newHtdt = htdtCollection.Find(x => x.Xoa == false && x.Id == newModel.MaHTDT).FirstOrDefault();

                noiDungChinhSua += $"Hình thức đào tạo:\n + {oldHtdt.Ten} => {newHtdt.Ten}\n";
                edits += 1;
            }

            countEdit = edits;

            return noiDungChinhSua;
        }

    }
}
