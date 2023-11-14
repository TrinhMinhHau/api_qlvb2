using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo
{
    public class SoCapPhatBangBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private readonly string collectionNameSoCapPhatBang = "SoCapPhatBang";
        private readonly string collectionNameHocSinh = "HocSinh";


        private IMongoDatabase _mongoDatabase;

        public SoCapPhatBangBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }


        public string GetHocSinhTheoSoCapPhatBang(TruongViewModel truong, DanhMucTotNghiepViewModel dmtn , SoCapPhatBangSearchParam paramModel)
        {
            int skip = ((paramModel.StartIndex - 1) * paramModel.PageSize) + paramModel.PageSize;
            string pagination = paramModel.PageSize < 0 ? $@"hocSinhs: '$hocSinhs'" : $@"hocSinhs: {{ $slice: ['$hocSinhs', {skip}, {paramModel.PageSize}] }},";
            var cauHinh = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong).Find(x => x.Id == truong.IdCha).FirstOrDefault().CauHinh;
            var collectionNamThi = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);

            var khoaThi = collectionNamThi.Find(x => x.Id == dmtn.IdNamThi).FirstOrDefault().KhoaThis.Where(x => x.Id == paramModel.IdKhoaThi).FirstOrDefault();

            var cmdRes = $@"
                        {{
                            'aggregate': 'SoCapPhatBang', 
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
                                      let: {{ soCapPhatBangId: '$_id' }},
                                      pipeline: [
                                        {{
                                          $match: {{
                                            $expr: {{ $ne: ['$IdSoCapPhatBang', ''] }},
                                          }},
                                        }},
                                        {{
                                          $addFields: {{
                                            IdSoCapPhatBang: {{ $toObjectId: '$IdSoCapPhatBang' }},
                                          }},
                                        }},
                                        {{
                                          $match: {{
                                            $expr: {{ $eq: ['$IdSoCapPhatBang', '$$soCapPhatBangId'] }},
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
            string soCapBangJson = ModelProvider.ExtractJsonFromMongo(data);
            var truongJson = JsonConvert.SerializeObject(truong);
            var dmtnJson = JsonConvert.SerializeObject(dmtn);
            var cauHinhJson = JsonConvert.SerializeObject(cauHinh);
            var khoaThiJson = JsonConvert.SerializeObject(khoaThi);


            if (soCapBangJson == null)
            {
                return null;
            }

            string finalJson = $"{{\"Truong\": {truongJson}, \"DanhMucTotNghiep\": {dmtnJson},\"CauHinh\": {cauHinhJson},  \"KhoaThi\": {khoaThiJson} ,\"SoCapPhatBang\": {soCapBangJson}}}";


            return finalJson;
        }
    }
}
