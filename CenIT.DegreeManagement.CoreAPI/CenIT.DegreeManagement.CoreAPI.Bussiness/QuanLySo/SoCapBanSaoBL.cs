using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo
{
    public class SoCapBanSaoBL : ConfigAppBussiness
    {

        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private readonly string collectionNameSoCapBanSao = "SoCapBanSao";
        private readonly string collectionNameHocSinh = "HocSinh";
        private readonly string collectionNameDanhMucTotNghiep = "DanhMucTotNghiep";
        private readonly string collectionNameSoGoc = "SoGoc";
        private readonly string collectionNameTruong = "Truong";
        private readonly string collectionNameNamThi = "NamThi";

        private IMongoDatabase _mongoDatabase;
        public SoCapBanSaoBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        /// <summary>
        /// Lấy thông tin sổ gốc theo danh muc tốt nghiệp
        /// </summary>
        /// <param name="idDanhMucTotNghiep"></param>
        /// <returns></returns>
        public string GetHocSinhTheoCapBanSao(TruongViewModel truong, DanhMucTotNghiepViewModel dmtn, SearchParamModel paramModel)
        {

            int skip = ((paramModel.StartIndex - 1) * paramModel.PageSize) + paramModel.PageSize;
            string pagination = paramModel.PageSize < 0 ? $@"hocSinhs: '$hocSinhs'" : $@"hocSinhs: {{ $slice: ['$hocSinhs', {skip}, {paramModel.PageSize}] }},
";
            var cmdRes = $@"
                        {{
                            'aggregate': 'SoCapBanSao', 
                            'allowDiskUse': true,
                            'pipeline':[
                                {{
                                    $match: {{
                                      Xoa: false,
                                      IdNamThi: ObjectId('{dmtn.IdNamThi}')
                                    }},
                                  }},
                                   {{
                                    $lookup: {{
                                      from: 'HocSinh',
                                      let: {{ soCapBanSaoId: '$_id' }},
                                      pipeline: [
                                        {{
                                          $match: {{
                                            $expr: {{ $ne: ['$IdSoCapBanSao', ''] }},
                                          }},
                                        }},
                                        {{
                                          $addFields: {{
                                            IdSoCapBanSao: {{ $toObjectId: '$IdSoCapBanSao' }},
                                          }},
                                        }},
                                        {{
                                          $match: {{
                                            $expr: {{ $eq: ['$IdSoCapBanSao', '$$soCapBanSaoId'] }},
                                          }},
                                        }},
                                        {{
                                            $addFields: {{
                                            TenCuoi: {{ $arrayElemAt: [{{ $split: ['$HoTen', ' '] }}, -1] }},
                                            }},
                                        }},
                                        {{
                                            $sort: {{
                                            TenCuoi: 1,
                                            HoTen: 1,
                                            }},
                                        }},
                                     {{
                                                $match: {{
                                                    IdDanhMucTotNghiep: '{dmtn.Id}',
                                                    IdTruong: '{truong.Id}'
                                                }}
                                            }},
                                        {{
                                            $project: {{
                                                _id:0,
                                                Id: '$_id',
                                                HoTen: '$HoTen',
                                                CCCD: '$CCCD',
                                                NgaySinh: '$NgaySinh',
                                                NoiSinh: '$NoiSinh',
                                                GioiTinh: '$GioiTinh',
                                                DanToc: '$DanToc',
                                                XepLoai: '$XepLoai',
                                                SoHieuVanBang: '$SoHieuVanBang',
                                                SoVaoSoCapBang: '$SoVaoSoCapBang'
                                            }}
                                        }}
                                      ],
                                      as: 'hocSinhs',
                                    }},
                                  }},
                                  {{
                                    $project:{{
                                        _id:0,
                                        Id: '$_id',
                                        IdNamThi: '$IdNamThi',
                                        NguoiKyBang: '$NguoiKyBang',
                                        CoQuanCapBang: '$CoQuanCapBang',
                                        DiaPhuongCapBang: '$DiaPhuongCapBang',
                                        UyBanNhanDan : '$UyBanNhanDan',
                                        totalRow: {{ $size: '$hocSinhs' }},
                                           {pagination}
                                    }}
                                  }}
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";


            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string soCapBanSao = ModelProvider.ExtractJsonFromMongo(data);
            var truongJson = JsonConvert.SerializeObject(truong);
            var dmtnJson = JsonConvert.SerializeObject(dmtn);

            if (soCapBanSao == null)
            {
                return null;
            }

            string finalJson = $"{{\"Truong\": {truongJson}, \"DanhMucTotNghiep\": {dmtnJson} ,\"SoCapBanSao\": {soCapBanSao}}}";

            return finalJson;
        }

        public List<HocSinhCapBanSaoViewModel> GetHocSinhCapBanSao(out int total, SoCapBanSaoSearchParamModel paramModel)
        {
            var collectionDonYeuCau = _mongoDatabase.GetCollection<HocSinhCapBanSaoViewModel>(_collectionDonYeuCauCapBanSaoName);
            var collectionHocSinh = _mongoDatabase.GetCollection<HocSinhViewModel>(_collectionHocSinhName);

            var donYeuCaus = collectionDonYeuCau.Find(x => x.IdTruong == paramModel.IdTruong && (int)x.TrangThai >= (int)TrangThaiDonYeuCauEnum.DaDuyet).ToList();

            donYeuCaus = donYeuCaus
                            .Join(
                                collectionHocSinh.AsQueryable(),
                              d => d.IdHocSinh,
                              hs => hs.Id,
                              (d, hs) =>
                              {
                                  d.HocSinh = hs;
                                  return d;
                              }
                          )
                          .Where(x=>x.HocSinh.IdKhoaThi == paramModel.IdKhoaThi && x.HocSinh.IdDanhMucTotNghiep == paramModel.IdDanhMucTotNghiep).ToList();

            total = donYeuCaus.Count;

            switch (paramModel.Order)
            {
                case "0":
                    donYeuCaus = paramModel.OrderDir.ToUpper() == "ASC"
                        ? donYeuCaus.OrderBy(x => x.HocSinh.HoTen.Split(' ').LastOrDefault()).ToList()
                        : donYeuCaus.OrderByDescending(x => x.HocSinh.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
                case "1":
                    donYeuCaus = paramModel.OrderDir.ToUpper() == "ASC"
                        ? donYeuCaus.OrderBy(x => x.HocSinh.HoTen.Split(' ').LastOrDefault()).ToList()
                        : donYeuCaus.OrderByDescending(x => x.HocSinh.HoTen.Split(' ').LastOrDefault()).ToList();
                    break;
            }
            if (paramModel.PageSize > 0)
            {
                donYeuCaus = donYeuCaus.Skip(paramModel.PageSize * paramModel.StartIndex).Take(paramModel.PageSize).ToList();
            }
            return donYeuCaus;

        }
    }
}
