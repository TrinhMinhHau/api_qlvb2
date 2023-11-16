using CenIT.DegreeManagement.CoreAPI.Bussiness.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.ThongKe;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness
{
    public class ThongKeBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        //private readonly string collectionNameHocSinh = "HocSinh";

        private IMongoDatabase _mongoDatabase;

        public ThongKeBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        public List<ThongKePhoiGocModel> ThongKePhoiGocDaIn(string idNamThi, string maDaoTao, string idPhoiGoc)
        {
            var matchMaHeDaoTao = string.IsNullOrEmpty(maDaoTao) ? "" : $"MaHeDaoTao: '{maDaoTao}',";
            var matchNamThi = string.IsNullOrEmpty(idNamThi) ? "" : $"'DanhMucTotNghiep.IdNamThi': '{idNamThi}',";
            var matchIdPhoiGoc = string.IsNullOrEmpty(idPhoiGoc) ? "" : $"'_id': ObjectId('{idPhoiGoc}'),";

            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionNamePhoiGoc}', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $match: {{
                                      Xoa: false,
                                      {matchMaHeDaoTao}
                                      {matchIdPhoiGoc}
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: '{_collectionHocSinhName}',
                                      let: {{ phoiGocId: '$_id' }},
                                      pipeline: [
                                        {{
                                          $match: {{
                                            $expr: {{ $ne: ['$IdPhoiGoc', ''] }},
                                          }},
                                        }},
                                        {{
                                          $addFields: {{
                                            IdPhoiGoc: {{ $toObjectId: '$IdPhoiGoc' }},
                                          }},
                                        }},
                                        {{
                                          $match: {{
                                            $expr: {{ $eq: ['$IdPhoiGoc', '$$phoiGocId'] }},
                                          }},
                                        }},
                                        {{
                                          $addFields: {{
                                            IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                          }},
                                        }},
                                        {{
                                          $lookup: {{
                                            from: '{_collectionNameDanhMucTotNghiep}',
                                            localField: 'IdDanhMucTotNghiep',
                                            foreignField: '_id',
                                            as: 'DanhMucTotNghieps',
                                          }},
                                        }},
                                        {{
                                          $addFields: {{
                                            DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                          }},
                                        }},
                                        {{
                                          $project: {{
                                            DanhMucTotNghieps: 0,
                                          }},
                                        }},
                                        {{
                                          $match: {{
                                            {matchNamThi}
                                          }},
                                        }},
                                      ],
                                      as: 'HocSinhs',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      TongSoHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$HocSinhs' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$HocSinhs' }},
                                        }},
                                      }},
                                    }},
                                  }},
                                  {{
                                    $group: {{
                                      _id: '$_id',
                                      TenPhoi: {{ $first: '$TenPhoi' }},
                                      DaInTrongNam: {{ $first: '$TongSoHocSinh' }},
                                      ChuaIn: {{ $first: '$SoLuongPhoi' }},
                                      TongDaIn: {{ $first: '$SoLuongPhoiDaSuDung' }},
                                    }},
                                  }},
                            ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);

            var resutl = ModelProvider.MapFromMongoDB<ThongKePhoiGocModel>(data);
            return resutl;
        }

        public List<ThongKeHocSinhTotNghiepTheoTruongModel> ThongKeHocSinhTotNghiepTheoTruong(out int total, string idNamThi, string idTruong)
        {
            var matchNamThi = string.IsNullOrEmpty(idNamThi) ? "" : $"'DanhMucTotNghiep.IdNamThi': '{idNamThi}',";
            var matchIdTruong= string.IsNullOrEmpty(idTruong) ? "" : $"'_id': ObjectId('{idTruong}'),";

            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionNameTruong}', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $match: {{
                                      Xoa: false,
                                      LaPhong: false,
                                      {matchIdTruong}
                                    }},
                                  }},
                                                                {{
                                $lookup: {{
                                  from: '{_collectionHocSinhName}',
                                  let: {{ truongId: '$_id' }},
                                  pipeline: [
                                    {{
                                      $match: {{
                                        $expr: {{ $ne: ['$IdTruong', ''] }},
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        IdTruong: {{ $toObjectId: '$IdTruong' }},
                                      }},
                                    }},
                                    {{
                                      $match: {{
                                        $expr: {{ $eq: ['$IdTruong', '$$truongId'] }},
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                      }},
                                    }},
                                    {{
                                      $lookup: {{
                                        from: '{_collectionNameDanhMucTotNghiep}',
                                        localField: 'IdDanhMucTotNghiep',
                                        foreignField: '_id',
                                        as: 'DanhMucTotNghieps',
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                      }},
                                    }},
                                    {{
                                      $project: {{
                                        DanhMucTotNghieps: 0,
                                      }},
                                    }},
                                    {{
                                      $match: {{
                                        {matchNamThi}
                                        TrangThai: {{ $gte: 2 }},
                                      }},
                                    }},
                                    {{
                                      $count: 'TotalStudents',
                                    }},
                                  ],
                                  as: 'HocSinhs',
                                }},
                              }},
                              {{
                                $project: {{
                                  Ma: 1,
                                  Ten: 1,
                                  TongSoHocSinhTotNghiep: {{
                                    $ifNull: [{{ $arrayElemAt: ['$HocSinhs.TotalStudents', 0] }}, 0],
                                  }},
                                }},
                              }},
                            ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
             total = TongSoHocSinhTotNghiep();
            var result = ModelProvider.MapFromMongoDB<ThongKeHocSinhTotNghiepTheoTruongModel>(data);

            return result;
        }

        public List<ThongKeHocSinhTotNghiepTheoDMTNModel> ThongKeHocSinhDoTotNghiepTheoDMTN(out int total,string idTruong ,string idNamThi, string idDanhMucTotNghiep)
        {
            var matchNamThi = string.IsNullOrEmpty(idNamThi) ? "" : $"'IdNamThi': '{idNamThi}',";
            var matchIdDanhMucTotNghiep = string.IsNullOrEmpty(idDanhMucTotNghiep) ? "" : $"'_id': ObjectId('{idDanhMucTotNghiep}'),";


            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionNameDanhMucTotNghiep}', 
                            'allowDiskUse': true,
                            'pipeline':[
                               {{
                                    $match: {{
            
                                      Xoa: false,
                                    {matchNamThi}
{matchIdDanhMucTotNghiep}
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: '{_collectionHocSinhName}',
                                      let: {{ danhMucTotNghiepId: '$_id' }},
                                      pipeline: [
                                        {{
                                          $match: {{
                                            $expr: {{ $ne: ['$IdDanhMucTotNghiep', ''] }},
                                          }},
                                        }},
                                        {{
                                          $addFields: {{
                                            IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                          }},
                                        }},
                                        {{
                                          $match: {{
                                            $expr: {{ $eq: ['$IdDanhMucTotNghiep', '$$danhMucTotNghiepId'] }},
                                          }},
                                        }},
                                        {{
                                          $match: {{
                                            IdTruong: '{idTruong}',
                                          }},
                                        }},
                                      ],
                                      as: 'HocSinhs',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      TongSoHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$HocSinhs' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$HocSinhs' }},
                                        }},
                                      }},
                                    }},
                                  }},
                                  {{
                                    $group: {{
                                      _id: '$_id',
                                      TenDanhMuc: {{ $first: '$TieuDe' }},
                                      SoHocSinhTotNghiep: {{ $first: '$TongSoHocSinh' }},
                                    }},
                                  }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            total = TongSoHocSinhTotNghiepTheoTruong(idTruong);
            var result = ModelProvider.MapFromMongoDB<ThongKeHocSinhTotNghiepTheoDMTNModel>(data);

            return result;
        }

        public List<ThongKePhatBangModel> ThongKePhatBang(string idNamThi, string idTruong)
        {
            var matchIdTruong = string.IsNullOrEmpty(idTruong) ? "" : $"'_id': ObjectId('{idTruong}'),";

            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionNameTruong}', 
                            'allowDiskUse': true,
                            'pipeline':[
                              {{
                                $match: {{
                                  Xoa: false,
                                  LaPhong: false,
                                {matchIdTruong}
                                }},
                              }},
                              {{
                                $lookup: {{
                                  from: '{_collectionHocSinhName}',
                                  let: {{ truongId: '$_id' }},
                                  pipeline: [
                                    {{
                                      $match: {{
                                        $expr: {{ $ne: ['$IdTruong', ''] }},
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        IdTruong: {{ $toObjectId: '$IdTruong' }},
                                      }},
                                    }},
                                    {{
                                      $match: {{
                                        $expr: {{ $eq: ['$IdTruong', '$$truongId'] }},
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                      }},
                                    }},
                                    {{
                                      $lookup: {{
                                        from: '{_collectionNameDanhMucTotNghiep}',
                                        localField: 'IdDanhMucTotNghiep',
                                        foreignField: '_id',
                                        as: 'DanhMucTotNghieps',
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                      }},
                                    }},
                                    {{
                                      $project: {{
                                        DanhMucTotNghieps: 0,
                                      }},
                                    }},
                                    {{
                                      $match: {{
                                        'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                      }},
                                    }},
                                  ],
                                  as: 'HocSinhs',
                                }},
                              }},
                               {{
                                $addFields: {{
                                  TongBangChuaPhat: {{
                                    $size: {{
                                      $filter: {{
                                        input: '$HocSinhs',
                                        as: 'hs',
                                        cond: {{ $eq: ['$$hs.TrangThai', 5] }}
                                      }}
                                    }}
                                  }},
                                  TongBangDaPhat: {{
                                    $size: {{
                                      $filter: {{
                                        input: '$HocSinhs',
                                        as: 'hs',
                                        cond: {{ $eq: ['$$hs.TrangThai', 6] }}
                                      }}
                                    }}
                                  }},
                                    TongSoLuongBang: {{
                                    $size: {{
                                      $filter: {{
                                        input: '$HocSinhs',
                                        as: 'hs',
                                        cond: {{ $gte: ['$$hs.TrangThai', 5] }}
                                      }}
                                    }}
                                  }}
                                }}
                              }},
                               {{
                                $group: {{
                                  _id: '$_id',
                                  Ten: {{ $first: '$Ten' }},
                                  TongBangChuaPhat: {{ $sum: '$TongBangChuaPhat' }},
                                  TongBangDaPhat: {{ $sum: '$TongBangDaPhat' }},
                                  TongSoLuongBang: {{ $sum: '$TongSoLuongBang' }},
                                }},
                              }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            var result = ModelProvider.MapFromMongoDB<ThongKePhatBangModel>(data);

            return result;
        }

        public ThongKeTongQuatPhatBangModel ThongKeTongQuatPhatBang(string idNamThi)
        {

            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionNameTruong}', 
                            'allowDiskUse': true,
                            'pipeline':[
                               {{
                                    $match: {{
                                      Xoa: false,
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: 'DanhMucTotNghiep',
                                      localField: 'IdDanhMucTotNghiep',
                                      foreignField: '_id',
                                      as: 'DanhMucTotNghieps',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      DanhMucTotNghieps: 0,
                                    }},
                                  }},
                                  {{
                                    $match: {{
                                      'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      TongBangChuaPhat: {{
                                        $size: {{
                                          $filter: {{
                                            input: {{
                                              $ifNull: ['$HocSinhs', []],
                                            }},
                                            as: 'hs',
                                            cond: {{ $eq: ['$$hs.TrangThai', 5] }},
                                          }},
                                        }},
                                      }},
                                      TongBangDaPhat: {{
                                        $size: {{
                                          $filter: {{
                                            input: {{
                                              $ifNull: ['$HocSinhs', []], 
                                            }},
                                            as: 'hs',
                                            cond: {{ $eq: ['$$hs.TrangThai', 6] }},
                                          }},
                                        }},
                                      }},
                                      TongSoLuongBang: {{
                                        $size: {{
                                          $filter: {{
                                            input: {{
                                              $ifNull: ['$HocSinhs', []], 
                                            }},
                                            as: 'hs',
                                            cond: {{ $gte: ['$$hs.TrangThai', 5] }},
                                          }},
                                        }},
                                      }},
                                    }},
                                  }},
                                  {{
                                    $group: {{
                                      _id: null,
                                      TongBangChuaPhat: {{ $sum: '$TongBangChuaPhat' }},
                                      TongBangDaPhat: {{ $sum: '$TongBangDaPhat' }},
                                      TongSoLuongBang: {{ $sum: '$TongSoLuongBang' }},
                                    }},
                                  }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            var result = ModelProvider.MapFromMongoDB<ThongKeTongQuatPhatBangModel>(data);
      

            return result.FirstOrDefault();
        }

        private int TongSoHocSinhTotNghiep()
        {
            var cmdRes = new BsonDocument
                {
                    { "aggregate", _collectionHocSinhName },
                    { "allowDiskUse", true },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                                {
                                    { "Xoa", false },
                                    { "TrangThai", new BsonDocument("$gte", 2) }
                                }),
                            new BsonDocument("$count", "Tong")
                        }
                    },
                    { "cursor", new BsonDocument() }
                };

            var result = _mongoDatabase.RunCommand<BsonDocument>(cmdRes);
            int count = 0;
            if (result!= 0)
            {
                count = result.GetValue("cursor", null)?.AsBsonDocument?.GetValue("firstBatch", null)?.AsBsonArray?[0]?.AsBsonDocument?.GetValue("Tong", 0).AsInt32 ?? 0;

            }

            return count;
        }

        private int TongSoHocSinhTotNghiepTheoTruong(string idTruong)
        {
            var cmdRes = new BsonDocument
                {
                    { "aggregate", _collectionHocSinhName },
                    { "allowDiskUse", true },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match", new BsonDocument
                                {
                                    { "Xoa", false },
                                    { "TrangThai", new BsonDocument("$gte", 2) },
                                    { "IdTruong", idTruong }

                                }),
                            new BsonDocument("$count", "Tong")
                        }
                    },
                    { "cursor", new BsonDocument() }
                };

            int count = 0;
            var result = _mongoDatabase.RunCommand<BsonDocument>(cmdRes);
            if (result != 0)
            {
                count = result.GetValue("cursor", null)?.AsBsonDocument?.GetValue("firstBatch", null)?.AsBsonArray?[0]?.AsBsonDocument?.GetValue("Tong", 0).AsInt32 ?? 0;
            }

            return count;
        }

        public List<HocSinhListModel> GetHocSinhDoTotNghiepByTruongAndNam(out int total, HSByTruongNamSearchModel modelSearch)
        {
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");
            int skip =  (modelSearch.StartIndex - 1) * modelSearch.PageSize;
            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionHocSinhName}', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                    $match: {{
                                      Xoa: false,
                                      IdTruong: '{modelSearch.IdTruong}',
                                      TrangThai: {{$gte: {(int)TrangThaiHocSinhEnum.ChoDuyet}}},
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: '{_collectionNameDanhMucTotNghiep}',
                                      localField: 'IdDanhMucTotNghiep',
                                      foreignField: '_id',
                                      as: 'DanhMucTotNghieps',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      DanhMucTotNghieps: 0,
                                    }},
                                  }},
                                  {{
                                    $match: {{
                                      'DanhMucTotNghiep.IdNamThi': '{modelSearch.IdNamThi}',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      IdTruong: {{ $toObjectId: '$IdTruong' }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: '{_collectionNameTruong}',
                                      localField: 'IdTruong',
                                      foreignField: '_id',
                                      as: 'Truongs',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      Truong: {{ $arrayElemAt: ['$Truongs', 0] }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      Truongs: 0,
                                    }},
                                  }},
                                    {order}
                                  {{
                                    $group: {{
                                      _id: null,
                                      totalRow: {{ $sum: 1 }},
                                      hocSinhs: {{
                                        $push: {{
                                          Id: '$_id',
                                          HoTen: '$HoTen',
                                          NoiSinh: '$NoiSinh',
                                          CCCD: '$CCCD',
                                          DanToc: '$DanToc',
                                          GioiTinh: '$GioiTinh',
                                          NgaySinh: '$NgaySinh',
                                          SoHieuVanBang: '$SoHieuVanBang',
                                          SoVaoSoCapBang: '$SoVaoSoCapBang',
                                          TenTruong: '$Truong.Ten',
                                        }},
                                      }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      _id: 0,
                                      totalRow: 1,
                                      data: {{ $slice: ['$hocSinhs', {skip + modelSearch.PageSize}, {modelSearch.PageSize}] }}
                                    }},
                                  }}
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            var hocSinhs = ModelProvider.ExtractDataFromMongoList<HocSinhListModel>(data, out total);
            return hocSinhs;
        }
        public List<ThongKePhoiGocModel> GetThongKeInPhoiBang(out int total, ThongKeInPhoiBangSearchModel modelSearch)
        {
            var matchMaHeDaoTao = string.IsNullOrEmpty(modelSearch.MaHeDaoTao) ? "" : $"'MaHeDaoTao':'{modelSearch.MaHeDaoTao}',";

            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "TenPhoi");
            int skip = (modelSearch.StartIndex - 1) * modelSearch.PageSize;
            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionNamePhoiGoc}', 
                            'allowDiskUse': true,
                            'pipeline':[
                                   {{
                                        $match: {{  
                                          Xoa: false,
                                            {matchMaHeDaoTao}
                                        }},
                                      }},
                                      {{
                                        $lookup: {{
                                          from: 'HocSinh',
                                          let: {{ phoiGocId: '$_id' }},
                                          pipeline: [
                                            {{
                                              $match: {{
                                                $expr: {{ $ne: ['$IdPhoiGoc', ''] }},
                                              }},
                                            }},
                                            {{
                                              $addFields: {{
                                                IdPhoiGoc: {{ $toObjectId: '$IdPhoiGoc' }},
                                              }},
                                            }},
                                            {{
                                              $match: {{
                                                $expr: {{ $eq: ['$IdPhoiGoc', '$$phoiGocId'] }},
                                              }},
                                            }},
                                            {{
                                              $addFields: {{
                                                IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                              }},
                                            }},
                                            {{
                                              $lookup: {{
                                                from: 'DanhMucTotNghiep',
                                                localField: 'IdDanhMucTotNghiep',
                                                foreignField: '_id',
                                                as: 'DanhMucTotNghieps',
                                              }},
                                            }},
                                            {{
                                              $addFields: {{
                                                DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                              }},
                                            }},
                                            {{
                                              $project: {{
                                                DanhMucTotNghieps: 0,
                                              }},
                                            }},
                                            {{
                                              $match: {{
                                                'DanhMucTotNghiep.IdNamThi': '{modelSearch.IdNamThi}',
                                              }},
                                            }},
                                          ],
                                          as: 'HocSinhs',
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          DaIn: {{ $size: '$HocSinhs' }},
                                        }},
                                      }},
                                      {order}
                                      {{
                                        $group: {{
                                          _id: null,
                                          totalRow: {{ $sum: 1 }},
                                          phois: {{
                                            $push: {{
                                              Id: '$_id',
                                              TenPhoi: '$TenPhoi',
                                              ChuaIn: '$SoLuongPhoi',
                                              DaIn: '$DaIn',
                                            }},
                                          }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          _id: 0,
                                          totalRow: 1,
                                          data: {{ $slice: ['$phois', {skip + modelSearch.PageSize}, {modelSearch.PageSize}] }},
                                        }},
                                      }},
                            ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);

            var resutl = ModelProvider.ExtractDataFromMongoList<ThongKePhoiGocModel>( data, out total);
            return resutl;
        }
        public List<ThongKePhatBangModel> GetThongKePhatBang(out int total,ThongKePhatBangSearchModel modelSearch, string idCha)
        {
            var matchIdTruong = string.IsNullOrEmpty(modelSearch.IdTruong) ? "" : $"'_id': ObjectId('{modelSearch.IdTruong}'),";
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "Ten");
            int skip = (modelSearch.StartIndex - 1) * modelSearch.PageSize;
            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionNameTruong}', 
                            'allowDiskUse': true,
                            'pipeline':[
                             {{
                                $match: {{
                                  Xoa: false,
                                  MaHeDaoTao: '{modelSearch.MaHeDaoTao}',
                                  IdCha: '{idCha}',
                                   {matchIdTruong}
                                }},
                              }},
                              {{
                                $lookup: {{
                                  from: 'HocSinh',
                                  let: {{ truongId: '$_id' }},
                                  pipeline: [
                                                {{
                                      $match: {{
                                        $expr: {{ $ne: ['$IdTruong', ''] }},
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        IdTruong: {{ $toObjectId: '$IdTruong' }},
                                      }},
                                    }},
                                    {{
                                      $match: {{
                                        $expr: {{ $eq: ['$IdTruong', '$$truongId'] }},
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                      }},
                                    }},
                                    {{
                                      $lookup: {{
                                        from: 'DanhMucTotNghiep',
                                        localField: 'IdDanhMucTotNghiep',
                                        foreignField: '_id',
                                        as: 'DanhMucTotNghieps',
                                      }},
                                    }},
                                    {{
                                      $addFields: {{
                                        DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                      }},
                                    }},
                                    {{
                                      $project: {{
                                        DanhMucTotNghieps: 0,
                                      }},
                                    }},
                                    {{
                                      $match: {{
                                        'DanhMucTotNghiep.IdNamThi': '{modelSearch.IdNamThi}',
                                      }},
                                    }},
                                  ],
                                  as: 'HocSinhs',
                                }},
                              }},
                              {{
                                $addFields: {{
                                  ChuaPhat: {{
                                    $size: {{
                                      $filter: {{
                                        input: {{
                                          $ifNull: ['$HocSinhs', []],
                                        }},
                                        as: 'hs',
                                        cond: {{ $eq: ['$$hs.TrangThai', 5] }},
                                      }},
                                    }},
                                  }},
                                  DaPhat: {{
                                    $size: {{
                                      $filter: {{
                                        input: {{
                                          $ifNull: ['$HocSinhs', []],
                                        }},
                                        as: 'hs',
                                        cond: {{ $eq: ['$$hs.TrangThai', 6] }},
                                      }},
                                    }},
                                  }},
                                }},
                              }},
                                {order}
                              {{
                                $group: {{
                                  _id: null,
                                  totalRow: {{ $sum: 1 }},
                                  truongs: {{
                                    $push: {{
                                      Id: '$_id',
                                      Ten: '$Ten',
                                      ChuaPhat: '$ChuaPhat',
                                      DaPhat: '$DaPhat',
                                    }},
                                  }},
                                }},
                              }},
                              {{
                                $project: {{
                                  _id: 0,
                                  totalRow: 1,
                                  data: {{ $slice: ['$truongs', {skip + modelSearch.PageSize}, {modelSearch.PageSize}] }},
                                }},
                              }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            var result = ModelProvider.ExtractDataFromMongoList<ThongKePhatBangModel>(data, out total);

            return result;
        }
        public List<HocSinhListModel> GetHocSinhDoTotNghiep(out int total, HocSinhTotNghiepSearchModel modelSearch)
        {
            var matchKhoaThi = string.IsNullOrEmpty(modelSearch.IdKhoaThi) ? "" : $"'IdKhoaThi': '{modelSearch.IdKhoaThi}',";
            var matchHDT = string.IsNullOrEmpty(modelSearch.MaHeDaoTao) ? "" : $"'Truong.MaHeDaoTao': '{modelSearch.MaHeDaoTao}',";
            var matchHTDT = string.IsNullOrEmpty(modelSearch.MaHinhThucDaoTao) ? "" : $"'Truong.MaHinhThucDaoTao': '{modelSearch.MaHinhThucDaoTao}',";

            var mathHDTAndHTDT = string.IsNullOrEmpty(matchHDT) && string.IsNullOrEmpty(matchHTDT) ? "" : $@"{{$match: {{{matchHDT} {matchHTDT}}},}},";

            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");
            int skip = (modelSearch.StartIndex - 1) * modelSearch.PageSize;
            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionHocSinhName}', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                        $match: {{
                                          Xoa: false,
                                          {matchKhoaThi}
                                          TrangThai : {{$gte: {(int)TrangThaiHocSinhEnum.ChoDuyet}}}
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                        }},
                                      }},
                                      {{
                                        $lookup: {{
                                          from: 'DanhMucTotNghiep',
                                          localField: 'IdDanhMucTotNghiep',
                                          foreignField: '_id',
                                          as: 'DanhMucTotNghieps',
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          DanhMucTotNghieps: 0,
                                        }},
                                      }},
                                      {{
                                        $match: {{
                                          'DanhMucTotNghiep.IdNamThi': '{modelSearch.IdNamThi}',
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          IdTruong: {{ $toObjectId: '$IdTruong' }},
                                        }},
                                      }},
                                      {{
                                        $lookup: {{
                                          from: 'Truong',
                                          localField: 'IdTruong',
                                          foreignField: '_id',
                                          as: 'Truongs',
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          Truong: {{ $arrayElemAt: ['$Truongs', 0] }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          Truongs: 0,
                                        }},
                                      }},
                                        {mathHDTAndHTDT}
                                      {{
                                        $addFields: {{
                                          TenCuoi: {{ $arrayElemAt: [{{ $split: ['$HoTen', ' '] }}, -1] }},
                                        }},
                                      }},
                                     {order}
                                      {{
                                        $group: {{
                                          _id: null,
                                          totalRow: {{ $sum: 1 }},
                                          hocSinhs: {{
                                            $push: {{
                                              Id: '$_id',
                                              HoTen: '$HoTen',
                                              NoiSinh: '$NoiSinh',
                                              GioiTinh: '$GioiTinh',
                                              NgaySinh: '$NgaySinh',
                                              SoHieuVanBang: '$SoHieuVanBang',
                                              SoVaoSoCapBang: '$SoVaoSoCapBang',
                                              TenTruong: '$Truong.Ten',
                                            }},
                                          }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          _id: 0,
                                          totalRow: 1,
                                          data: {{ $slice: ['$hocSinhs', {skip + modelSearch.PageSize}, {modelSearch.PageSize}] }},
                                        }},
                                      }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            var hocSinhs = ModelProvider.ExtractDataFromMongoList<HocSinhListModel>(data, out total);
            return hocSinhs;
        }

        public List<HocSinhListModel> GetHocSinhDTNByTruongAndNamOrDMTN(out int total, HSByTruongNamOrDMTNSearchModel modelSearch)
        {
            var matchIdDanhMucTotNghiep = string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep) ? "" : $"'IdDanhMucTotNghiep': '{modelSearch.IdDanhMucTotNghiep}',";
            var matchIdNamThi = string.IsNullOrEmpty(modelSearch.IdNamThi) ? "" : $"{{$match: {{'DanhMucTotNghiep.IdNamThi': '{modelSearch.IdNamThi}',}},}},";

            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");
            int skip = (modelSearch.StartIndex - 1) * modelSearch.PageSize;
            var cmdRes = $@"
                        {{
                            'aggregate': '{_collectionHocSinhName}', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                    $match: {{
                                      Xoa: false,
                                      IdTruong: '{modelSearch.IdTruong}',
                                      KetQua: 'x'
                                      TrangThai: {{$gte: {(int)TrangThaiHocSinhEnum.ChoDuyet}}},
                                        {matchIdDanhMucTotNghiep}
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: '{_collectionNameDanhMucTotNghiep}',
                                      localField: 'IdDanhMucTotNghiep',
                                      foreignField: '_id',
                                      as: 'DanhMucTotNghieps',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      DanhMucTotNghiep: {{ $arrayElemAt: ['$DanhMucTotNghieps', 0] }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      DanhMucTotNghieps: 0,
                                    }},
                                  }},
                                  {matchIdNamThi}
                                  {{
                                    $addFields: {{
                                      IdTruong: {{ $toObjectId: '$IdTruong' }},
                                    }},
                                  }},
                                  {{
                                    $lookup: {{
                                      from: '{_collectionNameTruong}',
                                      localField: 'IdTruong',
                                      foreignField: '_id',
                                      as: 'Truongs',
                                    }},
                                  }},
                                  {{
                                    $addFields: {{
                                      Truong: {{ $arrayElemAt: ['$Truongs', 0] }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      Truongs: 0,
                                    }},
                                  }},
                                    {order}
                                  {{
                                    $group: {{
                                      _id: null,
                                      totalRow: {{ $sum: 1 }},
                                      hocSinhs: {{
                                        $push: {{
                                          Id: '$_id',
                                          HoTen: '$HoTen',
                                          NoiSinh: '$NoiSinh',
                                          CCCD: '$CCCD',
                                          DanToc: '$DanToc',
                                          GioiTinh: '$GioiTinh',
                                          NgaySinh: '$NgaySinh',
                                          SoHieuVanBang: '$SoHieuVanBang',
                                          SoVaoSoCapBang: '$SoVaoSoCapBang',
                                          TenTruong: '$Truong.Ten',
                                        }},
                                      }},
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      _id: 0,
                                      totalRow: 1,
                                      data: {{ $slice: ['$hocSinhs', {skip + modelSearch.PageSize}, {modelSearch.PageSize}] }}
                                    }},
                                  }}
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            var hocSinhs = ModelProvider.ExtractDataFromMongoList<HocSinhListModel>(data, out total);
            return hocSinhs;
        }
    
        public List<HocSinhVM> GetThongKeHocSinhByNamOrDMTN(out int total, HSByTruongNamOrDMTNSearchModel modelSearch)
        {
            var collectionHocSinh = _mongoDatabase.GetCollection<HocSinhVM>(_collectionHocSinhName);
            var collectionDmtn= _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);

            var hocSinhs = collectionHocSinh.Find(x=>x.IdTruong == modelSearch.IdTruong).ToList()
                              .Join(
                              collectionDmtn.AsQueryable(),
                              hs => hs.IdDanhMucTotNghiep,
                              dmtn => dmtn.Id,
                              (hs, dmtn) =>
                              {
                                  hs.DanhMucTotNghiep = dmtn;
                                  return hs;
                              }
                          ).Where(x=>x.DanhMucTotNghiep.IdNamThi == modelSearch.IdNamThi).ToList();

            if (!string.IsNullOrEmpty(modelSearch.IdDanhMucTotNghiep))
            {
                hocSinhs = hocSinhs.Where(x => x.IdDanhMucTotNghiep == modelSearch.IdDanhMucTotNghiep).ToList();
            }

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
            if (modelSearch.PageSize > 0)
            {
                hocSinhs = hocSinhs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }
            return hocSinhs;


        }
    }
}

