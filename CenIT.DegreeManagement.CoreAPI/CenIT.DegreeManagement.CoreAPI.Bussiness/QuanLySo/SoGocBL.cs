
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo
{
    public class SoGocBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private readonly string collectionNameSoGoc = "SoGoc";
        private readonly string collectionNameHocSinh = "HocSinh";
        private readonly string collectionNameDanhMucTotNghiep = "DanhMucTotNghiep";
        private readonly string collectionNameNamThi = "NamThi";

        private IMongoDatabase _mongoDatabase;

        public SoGocBL(IConfiguration configuration)
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
        public SoGocModel? GetbyIdDanhMucTotNghiep(string idDanhMucTotNghiep)
        {

            var collectionDMTN = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(collectionNameDanhMucTotNghiep);


            var dmtn = collectionDMTN.Find(d=>d.Xoa == false && d.Id == idDanhMucTotNghiep).FirstOrDefault();
            if(dmtn == null)
            {
                return null;
            }

            // tìm năm thi có khóa thi = idKhoaThi và năm thi = idNamThi
            var filter = Builders<SoGocModel>.Filter.And(
                    Builders<SoGocModel>.Filter.Eq(x => x.IdNamThi, dmtn.IdNamThi),
                    Builders<SoGocModel>.Filter.Eq(x => x.Xoa, false)
                );

            // tìm năm thi đó
            var soGoc = _mongoDatabase.GetCollection<SoGocModel>(collectionNameSoGoc).Find(filter).FirstOrDefault();
            return soGoc != null ? soGoc : null;
        }

        public string GetHocSinhTheoSoGoc(TruongViewModel truong, DanhMucTotNghiepViewModel dmtn, SoGocSearchParam paramModel)
        {

            int skip = ((paramModel.StartIndex - 1) * paramModel.PageSize) + paramModel.PageSize;
            string pagination = paramModel.PageSize < 0 ? $@"hocSinhs: '$hocSinhs'": $@"hocSinhs: {{ $slice: ['$hocSinhs', {skip}, {paramModel.PageSize}] }}," ;
            var cauHinh  = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(x => x.Id == truong.IdCha).FirstOrDefault().CauHinh;
            var collectionNamThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var khoaThi = collectionNamThi.Find(x=>x.Id == dmtn.IdNamThi).FirstOrDefault().KhoaThis.Where(x=>x.Id == paramModel.IdKhoaThi).FirstOrDefault();
            var cmdRes = $@"
                        {{
                            'aggregate': 'SoGoc', 
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
                                      let: {{ soGocId: '$_id' }},
                                      pipeline: [
                                        {{
                                          $match: {{
                                            $expr: {{ $ne: ['$IdSoGoc', ''] }},
                                          }},
                                        }},
                                        {{
                                          $addFields: {{
                                            IdSoGoc: {{ $toObjectId: '$IdSoGoc' }},
                                          }},
                                        }},
                                        {{
                                          $match: {{
                                            $expr: {{ $eq: ['$IdSoGoc', '$$soGocId'] }},
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
                                                    IdTruong: '{truong.Id}',
                                                    IdKhoaThi: '{paramModel.IdKhoaThi}'
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
                                                HoiDong: '$HoiDong',
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
            string soGocJson = ModelProvider.ExtractJsonFromMongo(data);
            var truongJson = JsonConvert.SerializeObject(truong);
            var cauHinhJson = JsonConvert.SerializeObject(cauHinh);
            var dmtnJson = JsonConvert.SerializeObject(dmtn);
            var khoaThiJson = JsonConvert.SerializeObject(khoaThi);

            if (soGocJson == null)
            {
                return null;
            }

            string finalJson = $"{{\"Truong\": {truongJson}, \"DanhMucTotNghiep\": {dmtnJson}, \"KhoaThi\": {khoaThiJson}, \"CauHinh\": {cauHinhJson} ,\"SoGoc\": {soGocJson}}}";
            return finalJson;
        }
    }
}
