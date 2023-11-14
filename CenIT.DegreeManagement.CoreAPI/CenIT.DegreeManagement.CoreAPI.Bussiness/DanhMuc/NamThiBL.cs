using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.DanhMuc
{
    public class NamThiBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";

        private IMongoDatabase _mongoDatabase;

        public NamThiBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        #region Năm thi

        /// <summary>
        /// Thêm năm thi
        /// </summary>
        /// <param name="model"></param>
        /// <param name="soGocModel"></param>
        /// <param name="soCapBanSaoModel"></param>
        /// <returns></returns>
        public async Task<int> Create(NamThiInputModel model, SoGocModel soGocModel, SoCapBanSaoModel soCapBanSaoModel, SoCapPhatBangModel soCapPhatBangModel)
        {
            if (TrungTen(model.Ten, model.Id)) return (int)NamThiEnum.ExistName;
            try
            {
                var namThi = new NamThiModel()
                {
                    Ten = model.Ten.Trim(),
                    NgayTao = DateTime.Now,
                    NguoiTao = model.NguoiThucHien
                };

                // insert
                await _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi).InsertOneAsync(namThi);
                if (namThi.Id != null)
                {
                    
                    soGocModel.NgayTao = DateTime.Now;
                    soGocModel.NguoiTao = model.NguoiThucHien;
                    soGocModel.IdNamThi = namThi.Id;

                    soCapBanSaoModel.NgayTao = DateTime.Now;
                    soCapBanSaoModel.NguoiTao = model.NguoiThucHien;
                    soCapBanSaoModel.IdNamThi = namThi.Id;

                    soCapPhatBangModel.NgayTao = DateTime.Now;
                    soCapPhatBangModel.NguoiTao = model.NguoiThucHien;
                    soCapPhatBangModel.IdNamThi = namThi.Id;

                    await _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc).InsertOneAsync(soGocModel);
                    await _mongoDatabase.GetCollection<SoCapBanSaoModel>(_collectionNameSoCaoBanSao).InsertOneAsync(soCapBanSaoModel);
                    await _mongoDatabase.GetCollection<SoCapPhatBangModel>(_collectionNameSoCapPhatBang).InsertOneAsync(soCapPhatBangModel);

                    return (int)NamThiEnum.Success;

                }
                return (int)NamThiEnum.Fail;
            }
            catch
            {
                return (int)NamThiEnum.Fail;
            }
        }

        /// <summary>
        /// Cập nhật năm thi
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> Modify(NamThiInputModel model)
        {
            if (TrungTen(model.Ten, model.Id)) return (int)NamThiEnum.ExistName;
            try
            {
                var filter = Builders<NamThiModel>.Filter.And(
                               Builders<NamThiModel>.Filter.Eq(hdt => hdt.Xoa, false),
                               Builders<NamThiModel>.Filter.Eq(hdt => hdt.Id, model.Id)
                             );

                var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                            .Find(filter).FirstOrDefault();

                if (namThi == null)
                    return (int)NamThiEnum.NotFound;

                ModelProvider.MapProperties(model, namThi);
                namThi.NguoiCapNhat = model.NguoiThucHien;
                namThi.NgayCapNhat = DateTime.Now;

                var updateResult = await _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                                        .ReplaceOneAsync(filter, namThi);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)NamThiEnum.Fail;
                }
                return (int)NamThiEnum.Success;
            }
            catch
            {
                return (int)NamThiEnum.Fail;
            }
        }

        /// <summary>
        /// Xóa năm thi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nguoiThucHien"></param>
        /// <returns></returns>
        public async Task<int> Delete(string id, string nguoiThucHien)
        {
            try
            {
                var filter = Builders<NamThiModel>.Filter.And(
                                Builders<NamThiModel>.Filter.Eq(hdt => hdt.Xoa, false),
                                Builders<NamThiModel>.Filter.Eq(hdt => hdt.Id, id)
                              );

                var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                            .Find(filter).FirstOrDefault();

                if (namThi == null)
                    return (int)NamThiEnum.NotFound;
                namThi.NguoiXoa = nguoiThucHien;
                namThi.NgayXoa = DateTime.Now;
                namThi.Xoa = true;

                var updateResult = await _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                                        .ReplaceOneAsync(filter, namThi);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)NamThiEnum.Fail;
                }
                return (int)NamThiEnum.Success;
            }
            catch
            {
                return (int)NamThiEnum.Fail;
            }
        }

        /// <summary>
        /// Lấy tất cả năm thí
        /// </summary>
        /// <returns></returns>
        public List<NamThiModel> GetAll()
        {
            var namThis = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                .Find(p => p.Xoa == false)
                .ToList();
            return namThis.OrderByDescending(x => x.Ten).ToList();
        }

        /// <summary>
        /// Lấy danh sách Năm Thi theo search param
        /// </summary>
        /// <returns></returns>
        public List<NamThiModel> GetSearch(out int total, SearchParamModel modelSearch)
        {
            var filterBuilder = Builders<NamThiModel>.Filter;
            var namThiTaoCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);

            var filters = new List<FilterDefinition<NamThiModel>>
            {
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.Search)
                    ? filterBuilder.Regex("Ten", new BsonRegularExpression(modelSearch.Search, "i"))
                    : null
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);
            var namThis = namThiTaoCollection.Find(combinedFilter).ToList();


            total = namThis.Count();

            //sắp xếp
            switch (modelSearch.Order)
            {
                case "0":
                    namThis = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? namThis.OrderBy(x => x.Ten).ToList()
                        : namThis.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "1":
                    namThis = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? namThis.OrderBy(x => x.Ten).ToList()
                        : namThis.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "ten":
                    namThis = modelSearch.OrderDir.ToUpper() == "ASC"
                        ? namThis.OrderBy(x => x.Ten).ToList()
                        : namThis.OrderByDescending(x => x.Ten).ToList();
                    break;
            }

            //phân trang
            if (modelSearch.PageSize > 0)
            {
                namThis = namThis.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return namThis;
        }

        /// <summary>
        /// Lấy năm thi theo ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NamThiModel GetById(string id)
        {
            var filter = Builders<NamThiModel>.Filter.And(
                             Builders<NamThiModel>.Filter.Eq(hdt => hdt.Xoa, false),
                             Builders<NamThiModel>.Filter.Eq(hdt => hdt.Id, id)
                           );

            var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                        .Find(filter).FirstOrDefault();
            return namThi;
        }

        public NamThiViewModel GetNamThiByDanhMucTotNghiepId(string idDanhMucTotNghiep)
        {
            // Tìm DanhMucTotNghiepModel theo idDanhMucTotNghiep
            var danhMucTotNghiep = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                                .Find(dmt => dmt.Id == idDanhMucTotNghiep)
                                                .FirstOrDefault();
            var namThiVM = new NamThiViewModel();

            if (danhMucTotNghiep != null)
            {
                // Sử dụng IdNamThi để lấy đối tượng NamThiModel tương ứng
                var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                           .Find(nt => nt.Id == danhMucTotNghiep.IdNamThi)
                                           .FirstOrDefault();

                ModelProvider.MapProperties(namThi, namThiVM);
                namThiVM.DanhMucTotNghiep = danhMucTotNghiep;

                return namThiVM;
            }

            return null; // Hoặc có thể xử lý khác tùy theo logic của bạn
        }

        public bool TrungTen(string Ten, string Id)
        {
            var mt = Id == null
                ? _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower()).FirstOrDefault()
                : _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi).Find(n => true && n.Xoa == false && n.Ten.ToLower() == Ten.ToLower() && n.Id != Id).FirstOrDefault();
            return mt != null ? true : false;
        }
        #endregion

        #region Khóa thi
        /// <summary>
        /// Thêm mới khoá thi
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="khoaThiModel"></param>
        /// <returns></returns>
        public async Task<int> CreateKhoaThi(string idNamThi, KhoaThiInputModel khoaThiModel)
        {
            if (TrungNgay(khoaThiModel.Ngay, idNamThi, khoaThiModel.Id)) return -2;
            try
            {
                var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                            .Find(n => n.Xoa == false && n.Id == idNamThi).FirstOrDefault();
                if (namThi == null)
                    return (int)KhoaThiEnum.NotExistNamThi;

                Guid guid = Guid.NewGuid();
                var newKhoaThi = new KhoaThiModel()
                {
                    Id = guid.ToString(),
                    Ten = khoaThiModel.Ten,
                    Ngay = khoaThiModel.Ngay,
                    NgayTao = DateTime.Now,
                    NguoiTao = khoaThiModel.NguoiThucHien
                };

                namThi.KhoaThis.Add(newKhoaThi);

                var updateResult = await _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                                         .ReplaceOneAsync((n=>n.Id == idNamThi), namThi);
                if (updateResult.ModifiedCount == 0)
                {
                    return (int)KhoaThiEnum.Fail;
                }
                return (int)KhoaThiEnum.Success;
            }
            catch
            {
                return (int)KhoaThiEnum.Fail;
            }
         
        }

        /// <summary>
        /// Cập nhật khóa thi
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="khoaThiModel"></param>
        /// <returns></returns>
        public int ModifyKhoaThi(string idNamThi, KhoaThiInputModel khoaThiModel)
        {
            if (TrungNgay(khoaThiModel.Ngay, idNamThi, khoaThiModel.Id)) return -2;
            try
            {
                var filter = Builders<NamThiModel>.Filter.Eq(p => p.Id, idNamThi);

                var update = Builders<NamThiModel>.Update
                    .Set("KhoaThis.$[f].Ten", khoaThiModel.Ten)
                    .Set("KhoaThis.$[f].Ngay", khoaThiModel.Ngay)
                    .Set("KhoaThis.$[f].NgayCapNhat", DateTime.Now)
                    .Set("KhoaThis.$[f].NguoiCapNhat", khoaThiModel.NguoiThucHien)
                    ;

                var arrayFilters = new[]
                        {
                            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                new BsonDocument("f._id",khoaThiModel.Id))
                        };

                UpdateResult u = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi).UpdateOne(filter, update, new UpdateOptions() { ArrayFilters = arrayFilters });
                if (u.ModifiedCount == 0)
                {
                    return -1; //Update thất bại
                }
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Delete khoa thi
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idKhoaThi"></param>
        /// <param name="UserAction"></param>
        /// <returns></returns>
        public int DeleteKhoaThi(string idNamThi, string idKhoaThi, string UserAction)
        {
            try
            {
                var filter = Builders<NamThiModel>.Filter.Eq(p => p.Id, idNamThi);

                var update = Builders<NamThiModel>.Update
                    .Set("KhoaThis.$[f].Xoa", true)
                    .Set("KhoaThis.$[f].NgayXoa", DateTime.Now)
                    .Set("KhoaThis.$[f].NguoiXoa", UserAction)
                    ;

                var arrayFilters = new[]
                        {
                            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                                new BsonDocument("f._id", idKhoaThi))
                        };

                UpdateResult u = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi).UpdateOne(filter, update, new UpdateOptions() { ArrayFilters = arrayFilters });
                if (u.ModifiedCount == 0)
                {
                    return -1; //Update thất bại
                }
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        /// <summary>
        /// Lấy ds Khóa thi theo năm 
        /// </summary>
        /// <param name="idNam"></param>
        /// <returns></returns>
        public List<KhoaThiModel> GetKhoaThiByNamThiId(string idNam)
        {
            var khoaThiModel = new List<KhoaThiModel>();
            khoaThiModel = GetById(idNam).KhoaThis
                .Where(k => k.Xoa == false)
                .OrderByDescending(k => k.Ngay)
                .ToList();
            return khoaThiModel;
        }

        /// <summary>
        /// Lấy ds Khóa thi idDanhMucTotNghiep
        /// </summary>
        /// <param name="idNam"></param>
        /// <returns></returns>
        public List<KhoaThiModel> GetKhoaThiByIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {
            var dmtn = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep)
                                    .Find(d=>d.Id == idDanhMucTotNghiep).FirstOrDefault();

            if (dmtn == null)
                return new List<KhoaThiModel>();

            var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi)
                                    .Find(d => d.Id == dmtn.IdNamThi).FirstOrDefault();

            var khoaThiModel = new List<KhoaThiModel>();
            khoaThiModel = namThi.KhoaThis
                .Where(k => k.Xoa == false)
                .OrderByDescending(k => k.Ngay)
                .ToList();
            return khoaThiModel;
        }

        /// <summary>
        /// Lấy ds Khóa thi theo năm 
        /// </summary>
        /// <param name="idNam"></param>
        /// <returns></returns>
        public List<KhoaThiModel> GetSearchKhoaThiByNamThiId(out int total, SearchParamModel modelSearch, string idNam)
        {
            ////Get dữ liệu từ nội dung tìm kiếm
            var clt = GetById(idNam).KhoaThis
                .Where(k => k.Xoa == false && k.Ten.Contains(modelSearch.Search ?? ""))
                .ToList();
            total = clt.Count();

            // order & OrderDir
            switch (modelSearch.Order)
            {
                case "0":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ten).ToList() : clt.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "1":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ngay).ToList() : clt.OrderByDescending(x => x.Ngay).ToList();
                    break;
                case "ten":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ten).ToList() : clt.OrderByDescending(x => x.Ten).ToList();
                    break;
                case "ngay":
                    clt = modelSearch.OrderDir.ToUpper() == "ASC" ? clt.OrderBy(x => x.Ngay).ToList() : clt.OrderByDescending(x => x.Ngay).ToList();
                    break;
            }

            clt = clt.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            return clt;
        }


        /// <summary>
        /// Lấy tất cả khóa thi
        /// </summary>
        /// <returns></returns>
        public List<KhoaThiModel> GetAllKhoaThi()
        {
            var allNamThi = new List<NamThiModel>();
            allNamThi = GetAll();

            var allKhoaThi = new List<KhoaThiModel>();
            foreach (var nam in allNamThi)
            {
                allKhoaThi.AddRange(GetKhoaThiByNamThiId(nam.Id));
            }
            return allKhoaThi;
        }

        /// <summary>
        /// Get Khoa thi by Id
        /// </summary>
        /// <param name="idKhoaThi"></param>
        /// <returns></returns>
        public KhoaThiModel? GetKhoaThiById(string idNamThi, string idKhoaThi)
        {
            // tìm năm thi có khóa thi = idKhoaThi và năm thi = idNamThi
            var filter = Builders<NamThiModel>.Filter.And(
                    Builders<NamThiModel>.Filter.Eq(x => x.Id, idNamThi),
                    Builders<NamThiModel>.Filter.ElemMatch(x => x.KhoaThis,
                        Builders<KhoaThiModel>.Filter.And(
                                Builders<KhoaThiModel>.Filter.Eq(x => x.Id, idKhoaThi),
                                Builders<KhoaThiModel>.Filter.Eq(x => x.Xoa, false)
                        )
                    )
                );

            // tìm năm thi đó
            var namThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi).Find(filter).FirstOrDefault();
            return namThi != null ? namThi.KhoaThis.Find(x => x.Id == idKhoaThi) : null;
        }

        public bool TrungNgay(DateTime Ngay, string IdNamThi, string IdKhoaThi)
        {
            // Filter khóa thi
            var filterKhoaThiID = IdKhoaThi == null
                ? Builders<KhoaThiModel>.Filter.And(
                            Builders<KhoaThiModel>.Filter.Eq(x => x.Ngay, Ngay.ToUniversalTime()),
                            Builders<KhoaThiModel>.Filter.Eq(x => x.Xoa, false))
                : Builders<KhoaThiModel>.Filter.And(
                            Builders<KhoaThiModel>.Filter.Eq(x => x.Ngay, Ngay.ToUniversalTime()),
                            Builders<KhoaThiModel>.Filter.Eq(x => x.Xoa, false),
                            Builders<KhoaThiModel>.Filter.Ne(x => x.Id, IdKhoaThi));

            // Filter khóa thi trong năm
            var filter = Builders<NamThiModel>.Filter.And(
                Builders<NamThiModel>.Filter.Eq(x => x.Id, IdNamThi),
                Builders<NamThiModel>.Filter.Eq(x => x.Xoa, false),
                Builders<NamThiModel>.Filter.ElemMatch(x => x.KhoaThis,
                    filterKhoaThiID
                )
            );

            // Tìm
            var khoaThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi).Find(filter).FirstOrDefault();

            return khoaThi != null ? true : false;
        }
        #endregion
    }
}