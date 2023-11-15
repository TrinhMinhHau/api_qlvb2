using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;


namespace CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh
{
    public class DanhMucTotNghiepBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private readonly string collectionDanhMucTotNghiep = "DanhMucTotNghiep";
        private readonly string collectionHinhThucDaoTaoName = "HinhThucDaoTao";
        private readonly string collectionNamThiName = "NamThi";

        private IMongoDatabase _mongoDatabase;

        public DanhMucTotNghiepBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        #region Danh Muc Tot Nghiep Main

        /// <summary>
        /// Lấy danh sách danh mục tốt nghiệp theo search param
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetSearch(out int total, DanhMucTotNghiepSearchParam modelSearch)
        {
            var filterBuilder = Builders<DanhMucTotNghiepViewModel>.Filter;
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(collectionDanhMucTotNghiep);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName);
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName);

            var filters = new List<FilterDefinition<DanhMucTotNghiepViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.IdNamThi)
                    ? filterBuilder.Eq("IdNamThi", modelSearch.IdNamThi)
                    : null,
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Eq("TieuDe", new BsonRegularExpression(modelSearch.Search, "i"))
                    : null,
                  !string.IsNullOrEmpty(modelSearch.IdHinhThucDaoTao)
                    ? filterBuilder.Eq("IdHinhThucDaoTao", modelSearch.IdHinhThucDaoTao)
                    : null,
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(combinedFilter).ToList();

            // Join với bảng hình thức đào tạo và nam thi
            var danhMucTotNghiemVMs = danhMucTotNghieps
                .Join(
                    hinhThucDaoTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdHinhThucDaoTao,
                    htdt => htdt.Id,
                    (dmtn, htdt) =>
                    {
                        dmtn.HinhThucDaoTao = htdt.Ten;
                        return dmtn;
                    }
                )
                 .Join(
                    namThiTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdNamThi,
                    namThi => namThi.Id,
                    (dmtn, namThi) =>
                    {
                        dmtn.NamThi = namThi.Ten;
                        return dmtn;
                    }
                ).ToList();
                

            total = danhMucTotNghiemVMs.Count();

            //sắp xếp
            switch (modelSearch.Order)
            {
                case "0":
                    danhMucTotNghiemVMs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? danhMucTotNghiemVMs.OrderBy(x => x.TieuDe.Split(' ').LastOrDefault()).ToList()
                        : danhMucTotNghiemVMs.OrderByDescending(x => x.TieuDe.Split(' ').LastOrDefault()).ToList();
                    break;
                case "1":
                    danhMucTotNghiemVMs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? danhMucTotNghiemVMs.OrderBy(x => x.TieuDe).ToList()
                        : danhMucTotNghiemVMs.OrderByDescending(x => x.TieuDe).ToList();
                    break;
                case "2":
                    danhMucTotNghiemVMs = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? danhMucTotNghiemVMs.OrderBy(x => x.NamThi).ToList()
                        : danhMucTotNghiemVMs.OrderByDescending(x => x.NamThi).ToList();
                    break;
            }

            //phân trang
            if (modelSearch.PageSize > 0)
            {
                danhMucTotNghiemVMs = danhMucTotNghiemVMs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return danhMucTotNghiemVMs;
        }

        /// <summary>
        /// Lấy tất cả danh mục tốt nghiệp
        /// </summary>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetAll()
        {
            var filterBuilder = Builders<DanhMucTotNghiepViewModel>.Filter;
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(collectionDanhMucTotNghiep);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName);
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName);

            var filters = new List<FilterDefinition<DanhMucTotNghiepViewModel>>
            {
                filterBuilder.Eq("Xoa", false)
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(combinedFilter).ToList();

            // Join với bảng hình thức đào tạo và nam thi
            var danhMucTotNghiemVMs = danhMucTotNghieps
                .Join(
                    hinhThucDaoTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdHinhThucDaoTao,
                    htdt => htdt.Id,
                    (dmtn, htdt) =>
                    {
                        dmtn.HinhThucDaoTao = htdt.Ten;
                        return dmtn;
                    }
                )
                 .Join(
                    namThiTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdNamThi,
                    namThi => namThi.Id,
                    (dmtn, namThi) =>
                    {
                        dmtn.NamThi = namThi.Ten;
                        return dmtn;
                    }
                ).ToList();

            return danhMucTotNghiemVMs;
        }

        /// <summary>
        /// Lấy danh sách danh mục tốt nghiệp chưa khóa
        /// </summary>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetAllUnBlock()
        {
            var filterBuilder = Builders<DanhMucTotNghiepViewModel>.Filter;
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(collectionDanhMucTotNghiep);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName);
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName);


            var filters = new List<FilterDefinition<DanhMucTotNghiepViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                filterBuilder.Eq("Khoa", false)
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(combinedFilter).ToList();

            // Join với bảng hình thức đào tạo và nam thi
            var danhMucTotNghiemVMs = danhMucTotNghieps
                .Join(
                    hinhThucDaoTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdHinhThucDaoTao,
                    htdt => htdt.Id,
                    (dmtn, htdt) =>
                    {
                        dmtn.HinhThucDaoTao = htdt.Ten;
                        return dmtn;
                    }
                )
                 .Join(
                    namThiTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdNamThi,
                    namThi => namThi.Id,
                    (dmtn, namThi) =>
                    {
                        dmtn.NamThi = namThi.Ten;
                        return dmtn;
                    }
                ).ToList();
            return danhMucTotNghiemVMs;
        }

        /// <summary>
        /// Lấy danh mục tốt nghiệp theo id
        /// </summary>
        /// <returns></returns>
        public DanhMucTotNghiepViewModel GetByID(string idDanhMucTotNghiep)
        {
            var filter = Builders<DanhMucTotNghiepViewModel>.Filter.And(
                 Builders<DanhMucTotNghiepViewModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                 Builders<DanhMucTotNghiepViewModel>.Filter.Eq(dmtn => dmtn.Id, idDanhMucTotNghiep)
               );

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(collectionDanhMucTotNghiep);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName);
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName);

            var danhMucTotNghieps = danhMucTotNghiepCollection
                                        .Find(filter)                                  
                                        .ToList();
            // Join với bảng hình thức đào tạo và nam thi
            var danhMucTotNghiep = danhMucTotNghieps
                .Join(
                    hinhThucDaoTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdHinhThucDaoTao,
                    htdt => htdt.Id,
                    (dmtn, htdt) =>
                    {
                        dmtn.HinhThucDaoTao = htdt.Ten;
                        return dmtn;
                    }
                )
                 .Join(
                    namThiTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdNamThi,
                    namThi => namThi.Id,
                    (dmtn, namThi) =>
                    {
                        dmtn.NamThi = namThi.Ten;
                        return dmtn;
                    }
                ).FirstOrDefault();

            if (danhMucTotNghiep == null)
                return null;

            return danhMucTotNghiep;
        }

        /// <summary>
        /// Lấy danh mục tốt nghiệp theo idnamThi và idHinhThucDaoTao
        /// </summary>
        /// <returns></returns>
        public List<DanhMucTotNghiepViewModel> GetByIdNamThiAndMaHinhThucDaoTao(string idNamThi, string maHinhThucDaoTao, TruongModel donVi)
        {
          

            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(collectionDanhMucTotNghiep);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName);
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName);
            var hinhThucDaoTao = hinhThucDaoTaoCollection.Find(x => x.Xoa == false && x.Ma == maHinhThucDaoTao).FirstOrDefault();
            if(hinhThucDaoTao == null)
            {
                return new List<DanhMucTotNghiepViewModel>();
            }

            var filter = Builders<DanhMucTotNghiepViewModel>.Filter.And(
               Builders<DanhMucTotNghiepViewModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
               Builders<DanhMucTotNghiepViewModel>.Filter.Eq(dmtn => dmtn.IdNamThi, idNamThi),
               Builders<DanhMucTotNghiepViewModel>.Filter.Eq(dmtn => dmtn.IdHinhThucDaoTao, hinhThucDaoTao.Id),
               Builders<DanhMucTotNghiepViewModel>.Filter.Eq(dmtn => dmtn.MaHeDaoTao, donVi.MaHeDaoTao),
               Builders<DanhMucTotNghiepViewModel>.Filter.Eq(dmtn => dmtn.Khoa, false)
             );

            var danhMucTotNghieps = danhMucTotNghiepCollection
                                        .Find(filter)                                  
                                        .ToList();


            // Join với bảng hình thức đào tạo và nam thi
            var danhMucTotNghiep = danhMucTotNghieps
                .Join(
                    hinhThucDaoTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdHinhThucDaoTao,
                    htdt => htdt.Id,
                    (dmtn, htdt) =>
                    {
                        dmtn.HinhThucDaoTao = htdt.Ten;
                        return dmtn;
                    }
                )
                 .Join(
                    namThiTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdNamThi,
                    namThi => namThi.Id,
                    (dmtn, namThi) =>
                    {
                        dmtn.NamThi = namThi.Ten;
                        return dmtn;
                    }
                ).ToList();

            if (danhMucTotNghiep == null)
                return null;

            return danhMucTotNghiep;
        }

        /// <summary>
        /// Thêm danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Create(DanhMucTotNghiepInputModel model)
        {
            if (!KiemTraHTDT(model.IdHinhThucDaoTao)) return (int)EnumDanhMucTotNghiep.NotExistHTDT;
            string namThi = NamThi(model.IdNamThi);
            if (string.IsNullOrEmpty(namThi)) return (int)EnumDanhMucTotNghiep.NotExistNamThi;
            if (!KiemTraNgayCapBang(namThi, model.NgayCapBang)) return (int)EnumDanhMucTotNghiep.YearNotMatchDate;
            if (TrungTieuDe(model.TieuDe, model.Id)) return (int)EnumDanhMucTotNghiep.ExistName;
            //if (TrungIdNamAndIdHinhThucDaoTao(model.IdNamThi, model.IdHinhThucDaoTao, model.Id, model.MaHeDaoTao)) return (int)EnumDanhMucTotNghiep.ExistYearAndHTDT;
            try
            {
                var danhMucTotNghiepModel = new DanhMucTotNghiepModel();
                ModelProvider.MapProperties(model, danhMucTotNghiepModel);
                danhMucTotNghiepModel.NgayTao = DateTime.Now;
                danhMucTotNghiepModel.NguoiTao = model.NguoiThucHien;
      
                await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).InsertOneAsync(danhMucTotNghiepModel);
                if(danhMucTotNghiepModel.Id != null)
                {
                    return (int)EnumDanhMucTotNghiep.Success;

                }
                return (int)EnumDanhMucTotNghiep.Fail;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }

        /// <summary>
        /// Cập nhật danh mục tốt nghiệp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(DanhMucTotNghiepInputModel model)
        {
            if (!KiemTraHTDT(model.IdHinhThucDaoTao)) return (int)EnumDanhMucTotNghiep.NotExistHTDT;
            string namThi = NamThi(model.IdNamThi);
            if (string.IsNullOrEmpty(namThi)) return (int)EnumDanhMucTotNghiep.NotExistNamThi;
            if (!KiemTraNgayCapBang(namThi, model.NgayCapBang)) return (int)EnumDanhMucTotNghiep.YearNotMatchDate;
            if (TrungTieuDe(model.TieuDe, model.Id)) return (int)EnumDanhMucTotNghiep.ExistName;
            //if (TrungIdNamAndIdHinhThucDaoTao(model.IdNamThi, model.IdHinhThucDaoTao ,model.Id, model.MaHeDaoTao)) return (int)EnumDanhMucTotNghiep.ExistYearAndHTDT;
            try
            {
                var filter = Builders<DanhMucTotNghiepModel>.Filter.And(
                   Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                   Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Id, model.Id)
                 );

                var danhMucTotNghiep =  _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep)
                                            .Find(filter).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return  (int)EnumDanhMucTotNghiep.NotFound;
                if (danhMucTotNghiep.Khoa == true)
                    return (int)EnumDanhMucTotNghiep.Locked;
                if (danhMucTotNghiep.TrangThai == TrangThaiDanhMucTotNghiepEnum.DaInBang)
                    return (int)EnumDanhMucTotNghiep.Printed;

                ModelProvider.MapProperties(model, danhMucTotNghiep);
                danhMucTotNghiep.NgayCapNhat = DateTime.Now;
                danhMucTotNghiep.NguoiCapNhat = model.NguoiThucHien;

                var updateResult = await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).ReplaceOneAsync(filter, danhMucTotNghiep);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)EnumDanhMucTotNghiep.Fail;
                }
                return (int)EnumDanhMucTotNghiep.Success;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        
        }

        /// <summary>
        /// Xóa danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> Delete(string idDanhMucTotNghiep, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<DanhMucTotNghiepModel>.Filter.And(
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Id, idDanhMucTotNghiep)
                );

                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep)
                                            .Find(filter).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return (int)EnumDanhMucTotNghiep.NotFound;
                if (danhMucTotNghiep.Khoa == true)
                    return (int)EnumDanhMucTotNghiep.Locked;
                if (danhMucTotNghiep.TrangThai == TrangThaiDanhMucTotNghiepEnum.DaInBang)
                    return (int)EnumDanhMucTotNghiep.Printed;

                danhMucTotNghiep.Xoa = true;
                danhMucTotNghiep.NgayXoa = DateTime.Now;
                danhMucTotNghiep.NguoiXoa = nguoiThucHien;


                var updateResult = await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).ReplaceOneAsync(filter, danhMucTotNghiep);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)EnumDanhMucTotNghiep.Fail;
                }
                return (int)EnumDanhMucTotNghiep.Success;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái khóa mở danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> KhoaDanhMucTotNghiep(string idDanhMucTotNghiep, string nguoiThucHien, bool khoa)
        {
            try
            {
                var filter = Builders<DanhMucTotNghiepModel>.Filter.And(
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Id, idDanhMucTotNghiep)
                );


                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep)
                                            .Find(filter).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return (int)EnumDanhMucTotNghiep.NotFound;

                danhMucTotNghiep.Khoa = khoa;
                danhMucTotNghiep.NgayCapNhat = DateTime.Now;
                danhMucTotNghiep.NguoiCapNhat = nguoiThucHien;


                var updateResult = await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).ReplaceOneAsync(filter, danhMucTotNghiep);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)EnumDanhMucTotNghiep.Fail;
                }
                return (int)EnumDanhMucTotNghiep.Success;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái in bằng danh mục tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> CapNhatTrangThaiDaInBang(string idDanhMucTotNghiep)
        {
            try
            {
                var filter = Builders<DanhMucTotNghiepModel>.Filter.And(
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Id, idDanhMucTotNghiep)
                );

                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep)
                                            .Find(filter).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return (int)EnumDanhMucTotNghiep.NotFound;

                danhMucTotNghiep.TrangThai = TrangThaiDanhMucTotNghiepEnum.DaInBang;

                var updateResult = await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).ReplaceOneAsync(filter, danhMucTotNghiep);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)EnumDanhMucTotNghiep.Fail;
                }
                return (int)EnumDanhMucTotNghiep.Success;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }

        /// <summary>
        /// Cập nhật số lượng trường đẫ gửi
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<int> CapNhatSoLuongTruongDaGui(string idDanhMucTotNghiep, bool traLai = false)
        {
            try
            {
                var filter = Builders<DanhMucTotNghiepModel>.Filter.And(
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Id, idDanhMucTotNghiep)
                );

                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep)
                                            .Find(filter).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return (int)EnumDanhMucTotNghiep.NotFound;

                if (traLai == true && danhMucTotNghiep.TongSoTruongDaGui > 0)
                {
                    danhMucTotNghiep.TongSoTruongDaGui -= 1;
                }
                else
                {
                    danhMucTotNghiep.TongSoTruongDaGui += 1;
                }

                var updateResult = await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).ReplaceOneAsync(filter, danhMucTotNghiep);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)EnumDanhMucTotNghiep.Fail;
                }
                return (int)EnumDanhMucTotNghiep.Success;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }

        /// <summary>
        /// Cập nhật số lượng học sinh
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <param name="gui"></param>
        /// <param name="soLuongHocSinh"></param>
        /// <returns></returns>
        public async Task<int> CapNhatSoLuongHocSinh(string idDanhMucTotNghiep, int soLuongHocSinh)
        {
            try
            {
                var filter = Builders<DanhMucTotNghiepModel>.Filter.And(
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Id, idDanhMucTotNghiep)
                );

                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep)
                                            .Find(filter).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return (int)EnumDanhMucTotNghiep.NotFound;

                danhMucTotNghiep.SoLuongNguoiHoc += soLuongHocSinh;

                var updateResult = await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).ReplaceOneAsync(filter, danhMucTotNghiep);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)EnumDanhMucTotNghiep.Fail;
                }
                return (int)EnumDanhMucTotNghiep.Success;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }

        public List<DanhMucTotNghiepViewModel> GetAllByTruong(string idTruong)
        {
            var filterBuilder = Builders<DanhMucTotNghiepViewModel>.Filter;
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(collectionDanhMucTotNghiep);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName);
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName);
            var dmtnViaTruongCollection = _mongoDatabase.GetCollection<TruongHocViaDanhMucTotNghiepModel>(_collectionDMTNViaTruong);

            var dmtnViaTruongs = dmtnViaTruongCollection.Find(x => x.IdTruong == idTruong).ToList()
                                       .Join(
                                            danhMucTotNghiepCollection.AsQueryable(),
                                            dmtnVia => dmtnVia.IdDanhMucTotNghiep,
                                            dmtn => dmtn.Id,
                                            (dmtnVia, dmtn) =>
                                            {
                                                dmtnVia.DanhMucTotNghiep = dmtn;
                                                return dmtn;
                                            }
                                        )
                                         .Join(
                                                hinhThucDaoTaoCollection.AsQueryable(),
                                                dmtn => dmtn.IdHinhThucDaoTao,
                                                htdt => htdt.Id,
                                                (dmtn, htdt) =>
                                                {
                                                    dmtn.HinhThucDaoTao = htdt.Ten;
                                                    return dmtn;
                                                }
                                            )
                                             .Join(
                                                namThiTaoCollection.AsQueryable(),
                                                dmtn => dmtn.IdNamThi,
                                                namThi => namThi.Id,
                                                (dmtn, namThi) =>
                                                {
                                                    dmtn.NamThi = namThi.Ten;
                                                    return dmtn;
                                                }
                                            ).ToList(); 

            return dmtnViaTruongs;
        }


        /// <summary>
        /// Cập nhật số lượng trường đã duyệt
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public async Task<int> CapNhatSoLuongTruongDaDuyet(string idDanhMucTotNghiep)
        {
            try
            {
                var filter = Builders<DanhMucTotNghiepModel>.Filter.And(
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Xoa, false),
                  Builders<DanhMucTotNghiepModel>.Filter.Eq(dmtn => dmtn.Id, idDanhMucTotNghiep)
                );

                var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep)
                                            .Find(filter).FirstOrDefault();

                if (danhMucTotNghiep == null)
                    return (int)EnumDanhMucTotNghiep.NotFound;

                danhMucTotNghiep.TongSoTruongDaDuyet += 1;

                var updateResult = await _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).ReplaceOneAsync(filter, danhMucTotNghiep);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)EnumDanhMucTotNghiep.Fail;
                }
                return (int)EnumDanhMucTotNghiep.Success;
            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }


        public List<DanhMucTotNghiepViewModel> GetAllByHeDaoTao(string maHeDaoTao)
        {
            var filterBuilder = Builders<DanhMucTotNghiepViewModel>.Filter;
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViewModel>(collectionDanhMucTotNghiep);
            var hinhThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName);
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName);

            var filters = new List<FilterDefinition<DanhMucTotNghiepViewModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(maHeDaoTao)
                    ? filterBuilder.Eq("MaHeDaoTao", maHeDaoTao)
                    : null

            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var danhMucTotNghieps = danhMucTotNghiepCollection.Find(combinedFilter).ToList();

            // Join với bảng hình thức đào tạo và nam thi
            var danhMucTotNghiemVMs = danhMucTotNghieps
                .Join(
                    hinhThucDaoTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdHinhThucDaoTao,
                    htdt => htdt.Id,
                    (dmtn, htdt) =>
                    {
                        dmtn.HinhThucDaoTao = htdt.Ten;
                        return dmtn;
                    }
                )
                 .Join(
                    namThiTaoCollection.AsQueryable(),
                    dmtn => dmtn.IdNamThi,
                    namThi => namThi.Id,
                    (dmtn, namThi) =>
                    {
                        dmtn.NamThi = namThi.Ten;
                        return dmtn;
                    }
                ).ToList();

            return danhMucTotNghiemVMs;
        }

        #endregion

        #region Danh Muc Tot Nghiep Via Truong
        /// <summary>
        /// Cho phép trường gửi danh sách
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> CreateDanhMucTotNghiepViaTruong(DanhMucTotNghiepViaTruongInputModel model)
        {
            try
            {
                // các Collection
                var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
                var dmtnViaTruongCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViaTruongModel>(_collectionDMTNViaTruong);

                // Kiểm tra danh mục tốt nghiệp
                var dmtn = GetByID(model.IdDanhMucTotNghiep);
                if (dmtn == null)
                {
                    return (int)EnumDanhMucTotNghiep.NotFound;
                }

                string[] idTruongsArray = model.IdTruongs.Split(',');

                List<FilterDefinition<TruongModel>> filters = new List<FilterDefinition<TruongModel>>();
                foreach (var idTruong in idTruongsArray)
                {
                    filters.Add(Builders<TruongModel>.Filter.Eq(t => t.Id, idTruong));
                }
                // Kết hợp các FilterDefinition bằng phép OR
                FilterDefinition<TruongModel> combinedFilter = Builders<TruongModel>.Filter.Or(filters);

                // Kiểm tra sự tồn tại của tất cả idTruongs bằng một truy vấn
                var truongsExist = truongCollection.Find(combinedFilter).ToList();
                if (truongsExist.Count != idTruongsArray.Length)
                {
                    return (int)EnumDanhMucTotNghiep.NotFoundTruong;
                }

                List<DanhMucTotNghiepViaTruongModel> danhSachDanhMuc = new List<DanhMucTotNghiepViaTruongModel>();
                foreach (var idTruong in idTruongsArray)
                {
                    danhSachDanhMuc.Add(new DanhMucTotNghiepViaTruongModel
                    {
                        IdDanhMucTotNghiep = model.IdDanhMucTotNghiep,
                        IdTruong = idTruong,
                        NgayTao = DateTime.Now,
                        NguoiTao = model.NguoiThucHien
                    });
                }

                await dmtnViaTruongCollection.InsertManyAsync(danhSachDanhMuc);

                return (int)EnumDanhMucTotNghiep.Success;

            }
            catch
            {
                return (int)EnumDanhMucTotNghiep.Fail;
            }
        }

        public List<TruongHocViaDanhMucTotNghiepModel> GetTruong(string idDonVi, string idDanhMucTotNghiep)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var dmtnViaTruongCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepViaTruongModel>(_collectionDMTNViaTruong);
            string idDonViCha = idDonVi;
            var donVi = truongCollection.Find(x => x.Id == idDonVi).FirstOrDefault();
            if(donVi.LaPhong == false && string.IsNullOrEmpty(donVi.IdCha))
            {
                idDonViCha = donVi.IdCha;
            }

            var truongs = truongCollection.Find(x=>x.IdCha == idDonViCha).ToList();
            var dmtnViaTruong = dmtnViaTruongCollection.Find(x => x.IdDanhMucTotNghiep == idDanhMucTotNghiep).ToList()
                                                        .Select(x=>x.IdTruong).ToList();

            var truongsWithPermission = truongs.Select(truong =>
            {
                bool hasPermission = dmtnViaTruong.Contains(truong.Id);
                return new TruongHocViaDanhMucTotNghiepModel
                {
                    IdTruong = truong.Id,
                    TenTruong = truong.Ten,
                    MaTruong = truong.Ma,
                    HasPermision = hasPermission
                };
            }).ToList();

            return truongsWithPermission;

        }

        public List<TruongHocViaDanhMucTotNghiepModel> GetTruongHasPermision(string idDanhMucTotNghiep)
        {
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var dmtnViaTruongCollection = _mongoDatabase.GetCollection<TruongHocViaDanhMucTotNghiepModel>(_collectionDMTNViaTruong);

            var dmtnViaTruong = dmtnViaTruongCollection
                .Find(x => x.IdDanhMucTotNghiep == idDanhMucTotNghiep)
                .ToList();

           var truongs = truongCollection.AsQueryable();

            var result = dmtnViaTruong
                    .Join(
                        truongs,
                        dmtn => dmtn.IdTruong,
                        truong => truong.Id,
                        (dmtn, truong) =>
                        {
                            dmtn.IdTruong = truong.Id;
                            dmtn.TenTruong = truong.Ten;
                            dmtn.MaTruong = truong.Ma;
                            return dmtn;
                        }
                    )
                    .ToList();



            return result;
        }

        #endregion

        #region private

        private bool TrungTieuDe(string tieuDe, string idDanhMucTopNghiep)
        {
            var mt = idDanhMucTopNghiep == null
                ? _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).Find(n => true && n.Xoa == false && n.TieuDe == tieuDe).FirstOrDefault()
                : _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).Find(n => true && n.Xoa == false && n.TieuDe == tieuDe && n.Id != idDanhMucTopNghiep).FirstOrDefault();
            return mt != null ? true : false;
        }

        private string NamThi(string idNamThi)
        {
            var namThi = _mongoDatabase.GetCollection<NamThiModel>(collectionNamThiName)
                                           .Find(h => h.Xoa == false && h.Id == idNamThi).FirstOrDefault();
            return namThi == null ? null : namThi.Ten;
        }

        private bool KiemTraHTDT(string idHinhThucDaoTao)
        {
            var heDaoTao = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(collectionHinhThucDaoTaoName)
                                           .Find(h => h.Xoa == false && h.Id == idHinhThucDaoTao).FirstOrDefault();
            return heDaoTao != null ? true : false;
        }

        private bool TrungIdNamAndIdHinhThucDaoTao(string idNamThi, string idHinhThucDaoTao ,string idDanhMucTopNghiep, string maHeDaoTao)
        {
            var mt = idDanhMucTopNghiep == null
                ? _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).Find(n => true && n.Xoa == false && n.IdNamThi == idNamThi && n.IdHinhThucDaoTao == idHinhThucDaoTao && n.MaHeDaoTao == maHeDaoTao).FirstOrDefault()
                : _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionDanhMucTotNghiep).Find(n => true && n.Xoa == false && n.IdNamThi == idNamThi && n.IdHinhThucDaoTao == idHinhThucDaoTao && n.Id != idDanhMucTopNghiep && n.MaHeDaoTao == maHeDaoTao).FirstOrDefault();
            return mt != null ? true : false;
        }

        private bool KiemTraNgayCapBang(string nam, DateTime ngayCapBang)
        {
            if (nam == ngayCapBang.Year.ToString()) return true;
            return false;
        }


        #endregion
    }
}