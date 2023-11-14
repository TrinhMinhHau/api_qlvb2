using CenIT.DegreeManagement.CoreAPI.Bussiness.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Search;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness
{
    public class TrangChuBL : ConfigAppBussiness
    {
        private string _connectionString;
        private IConfiguration _configuration;
        private readonly string dbName = "nhatrangkha";
        private IMongoDatabase _mongoDatabase;

        public TrangChuBL(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionStrings:gddt"];

            //Dùng MongoClient để kết nối tới Server
            MongoClient client = new MongoClient(_connectionString);

            //Dùng lệnh GetDatabase để kết nối Cơ sở dữ liệu
            _mongoDatabase = client.GetDatabase(dbName);
        }

        #region Phòng

        /// <summary>
        /// Tra cứu học sinh tốt nghiệp theo cccd và họ tên (Trang chủ - phòng)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public string GetTraCuuHocSinhTotNghiep(string idDonVi, TraCuuHocHinhTotNghiepSearchModel modelSearch)
        {
            var matchHoTen = string.IsNullOrEmpty(modelSearch.HoTen) ? "" : $" HoTen: '{modelSearch.HoTen}',";
            var matchCCCD = string.IsNullOrEmpty(modelSearch.CCCD) ? "" : $" CCCD: '{modelSearch.CCCD}',";

            var matchIdDonVi = string.IsNullOrEmpty(idDonVi) ? "" : $" {{$match: {{'Truong.IdCha': '{idDonVi}', }},}},";
            string order = MongoPipeline.GenerateSortPipeline(modelSearch.Order, modelSearch.OrderDir, "HoTen");
            int skip = (modelSearch.StartIndex - 1) * modelSearch.PageSize;
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                    $match: {{
                                      Xoa: false,
                                      {matchHoTen},
                                      {matchCCCD}

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
                                 {matchIdDonVi}
                                 {order}
                                  {{
                                    $group: {{
                                      _id: null,
                                      totalRow: {{ $sum: 1 }},
                                      hocSinhs: {{
                                        $push: {{
                                          id: '$_id',
                                          hoTen: '$HoTen',
                                          cccd: '$CCCD',
                                          noiSinh: '$NoiSinh',
                                          gioiTinh: '$GioiTinh',
                                          ngaySinh: '$NgaySinh',
                                          soHieuVanBang: '$SoHieuVanBang',
                                          soVaoSoCapBang: '$SoVaoSoCapBang',
                                          trangThai: '$TrangThai',
                                          tenTruong: '$Truong.Ten',
                                          idDanhMucTotNghiep: '$IdDanhMucTotNghiep',
                                          maHeDaoTao : '$Truong.MaHeDaoTao',
                                          maHinhThucDaoTao : '$Truong.MaHinhThucDaoTao',
                                          idNamThi:'$DanhMucTotNghiep.IdNamThi',
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
            string json = ModelProvider.ExtractJsonFromMongo(data);
          
            return json;
        }


        /// <summary>
        /// Tra cứu học sinh tốt nghiệp theo cccd và họ tên (Trang chủ - phòng)
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<HocSinhVM> GetTraCuuHocSinhDaDuaVaoSo(out int total, string idDonVi, TraCuuHocHinhTotNghiepSearchModel modelSearch)
        {
            var filterBuilder = Builders<HocSinhVM>.Filter;
            var truongCollection = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong);
            var donYeuCauCollection = _mongoDatabase.GetCollection<DonYeuCauCapBanSaoModel>(_collectionDonYeuCauCapBanSaoName);
            var soGocCollection = _mongoDatabase.GetCollection<SoGocModel>(_collectionNameSoGoc);
            var namThiCollection = _mongoDatabase.GetCollection<NamThiModel>(_collectionNameNamThi);
            var danhMucTotNghiepCollection = _mongoDatabase.GetCollection<DanhMucTotNghiepModel>(_collectionNameDanhMucTotNghiep);
            var hinThucDaoTaoCollection = _mongoDatabase.GetCollection<HinhThucDaoTaoModel>(_collectionNamHinhThucDaoTao);

            CauHinhModel cauHinhDonViQL = truongCollection.Find(x=>x.Id == idDonVi).FirstOrDefault().CauHinh;
            var trangThais = new List<TrangThaiHocSinhEnum>
                            {
                                TrangThaiHocSinhEnum.DaDuaVaoSoGoc,
                                TrangThaiHocSinhEnum.DaCapBang,
                                TrangThaiHocSinhEnum.DaInBang,
                                TrangThaiHocSinhEnum.DaNhanBang,
                             };

            var filters = new List<FilterDefinition<HocSinhVM>>
            {
                 //filterBuilder.Eq("TrangThai", modelSearch.TrangThai),
                filterBuilder.Eq("Xoa", false),
                !string.IsNullOrEmpty(modelSearch.HoTen)
                    ? filterBuilder.Regex("HoTen", new BsonRegularExpression(modelSearch.HoTen, "i"))
                    : null,
                !string.IsNullOrEmpty(modelSearch.CCCD)
                    ? filterBuilder.Regex("CCCD", new BsonRegularExpression(modelSearch.CCCD, "i"))
                    : null,
            };
            filters.RemoveAll(filter => filter == null);

            var combinedFilter = filterBuilder.And(filters);


            var hocSinhs = _mongoDatabase.GetCollection<HocSinhVM>(_collectionHocSinhName)
                    .Find(combinedFilter)
                    .ToList()
                     .Join(
                          truongCollection.AsQueryable(),
                          hs => hs.IdTruong,
                          truong => truong.Id,
                          (hs, truong) =>
                          {
                              hs.Truong = truong;
                              hs.CauHinhDonViHienTai = cauHinhDonViQL;
                              hs.DonYeuCauCapBanSao = donYeuCauCollection.Find(x => x.IdHocSinh == hs.Id && x.TrangThai == TrangThaiDonYeuCauEnum.DaDuyet).FirstOrDefault();
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
                          namThiCollection.AsQueryable(),
                          hs => hs.DanhMucTotNghiep.IdNamThi,
                          namThi => namThi.Id,
                          (hs, namThi) =>
                          {
                              hs.NamThi = namThi;
                              hs.KhoaThi = namThi.KhoaThis.Where(x => x.Id == hs.IdKhoaThi).FirstOrDefault();
                              return hs;
                          }
                      )
                        .Join(
                          hinThucDaoTaoCollection.AsQueryable(),
                          hs => hs.DanhMucTotNghiep.IdHinhThucDaoTao,
                          htdt => htdt.Id,
                          (hs, htdt) =>
                          {
                              hs.HinhThucDaoTao = htdt;
                              return hs;
                          }
                      )
                        .GroupJoin(
                          soGocCollection.AsQueryable(),
                          hs => hs.IdSoGoc,
                          s => s.Id,
                          (hs, sGroup) =>
                          {
                              var soGoc = sGroup.FirstOrDefault(); 
                              if (soGoc == null)
                              {
                                  hs.SoGoc = new SoGocModel(); 
                              }
                              else
                              {
                                  hs.SoGoc = soGoc;
                              }
                              return hs;
                          }
                      )
                      .Where(x=>x.Truong.IdCha == idDonVi)
                      .ToList();

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

            if (modelSearch.PageSize >= 0)
            {
                hocSinhs = hocSinhs.Skip(modelSearch.PageSize * modelSearch.StartIndex).Take(modelSearch.PageSize).ToList();
            }

            return hocSinhs;
        }

        /// <summary>
        /// Lấy số lượng phôi đã in theo năm học và hedaotao (Trang chủ - phòng)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongPhoiDaIn(string idNamThi, string maHeDaoTao, string truongIdsString)
        {

            var cmdRes = $@"
                        {{
                            'aggregate': 'PhoiGoc', 
                            'allowDiskUse': true,
                            'pipeline':[
                                   {{
                                        $match: {{
                                          Xoa: false,
                                          MaHeDaoTao: '{maHeDaoTao}',
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
                                                'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                                'IdTruong' : {{$in : [{truongIdsString}]  }}
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
                                          _id: null,
                                          Tong: {{ $sum: '$TongSoHocSinh' }},
                                        }},
                                      }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        public string GetSoLuongDonViDaGui(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            var cmdRes = $@"
                        {{
                            'aggregate': 'Truong', 
                            'allowDiskUse': true,
                            'pipeline':[
                                    {{
                                        $match: {{
                                          Xoa: false,
                                          MaHeDaoTao: '{maHeDaoTao}',
                                          IdCha : '{idDonVi}'
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
                                                'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                                TrangThai: {{ $gte: 1 }}, 
                                              }},
                                            }},
                                          ],
                                          as: 'HocSinhs',
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          DaGui: {{
                                            $cond: {{
                                              if: {{ $eq: [{{ $size: '$HocSinhs' }}, 0] }},
                                              then: 0,
                                              else: 1,
                                            }},
                                          }},
                                        }},
                                      }},
                                      {{
                                        $project: {{
                                          DaGui: '$DaGui',
                                        }},
                                      }},
                                      {{
                                        $group: {{
                                        _id: null,
                                        Tong: {{$sum: 1}},
                                        DaGui: {{$sum: '$DaGui'}}
                                        }}
                                      }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        /// <summary>
        /// Lấy số lượng phôi đơn yêu cầu cấp bản sao năm học và hedaotao (Trang chủ - phòng)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongDonYeuCauCapBanSao(string idNamThi, string maHeDaoTao, string truongIdsString)
        {
          
            var cmdRes = $@"
                        {{
                            'aggregate': 'DonYeuCauCapBanSao', 
                            'allowDiskUse': true,
                            'pipeline':[
                                    {{
                                $match: {{
                                  Xoa: false,
                                  IdNamThi: '{idNamThi}',
                                  TrangThai: 0,
                                }},
                              }},
                              {{
                                $addFields: {{
                                  IdHocSinh: {{ $toObjectId: '$IdHocSinh' }},
                                }},
                              }},
                              {{
                                $lookup: {{
                                  from: 'HocSinh',
                                  localField: 'IdHocSinh',
                                  foreignField: '_id',
                                  as: 'HocSinh',
                                }},
                              }},
                              {{
                                $addFields: {{
                                  HocSinh: {{ $arrayElemAt: ['$HocSinh', 0] }},
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
                                $match: {{
                                  'Truong.MaHeDaoTao': '{maHeDaoTao}',
                                   'IdTruong' : {{$in : [{truongIdsString}]  }}
                                }},
                              }},
                                {{
                                  $group: {{
                                    _id: null,
                                    Tong: {{ $sum: 1 }},
                                  }},
                                }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        /// <summary>
        /// Lấy số lượng học sinh chưa duyệt theo năm học và hedaotao (Trang chủ - phòng)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhChuaDuyet(string idNamThi, string maHeDaoTao, string truongIdsString)
        {

            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                   {{
                                    $match: {{
                                      Xoa: false,
                                      TrangThai: 1,
                                      'IdTruong' : {{$in : [{truongIdsString}]  }}
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
                                    $match: {{
                                      'Truong.MaHeDaoTao': '{maHeDaoTao}',
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
                                    $match: {{
                                      'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                    }},
                                  }},
                                  {{
                                    $group: {{
                                      _id: 0,
                                      SoLuongChuaDuyet: {{ $sum: 1 }},
                                    }},
                                  }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        /// <summary>
        /// Lấy số lượng học sinh qua từng năm (Biểu đồ trang chủ)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhQuaTungNam(string maHeDaoTao, string idDonVi)
        {
            var truongs = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                         .Find(x => x.Xoa == false && x.IdCha == idDonVi)
                         .ToList()
                         .Select(x => x.Id)
                         .ToArray();
            var truongIdsString = string.Join(",", truongs.Select(x => $"'{x}'"));
            var cmdRes = $@"
                        {{
                            'aggregate': 'NamThi', 
                            'allowDiskUse': true,
                            'pipeline':[
                                    {{
                                        $match: {{
                                          Xoa: false,
                                        }},
                                      }},
                                      {{
                                        $lookup: {{
                                          from: 'DanhMucTotNghiep', 
                                          let: {{ namThiId: '$_id' }},
                                          pipeline: [
                                            {{
                                              $match: {{
                                                $expr: {{ $ne: ['$IdNamThi', ''] }},
                                              }},
                                            }},
                                            {{
                                              $addFields: {{
                                                IdNamThi: {{ $toObjectId: '$IdNamThi' }},
                                              }},
                                            }},
                                            {{
                                              $match: {{
                                                $expr: {{ $eq: ['$IdNamThi', '$$namThiId'] }},
                                              }},
                                            }},
                                            {{
                                              $lookup: {{
                                                from: 'HocSinh', 
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
                                                      $match: {{
                                                        'TrangThai': {{ $gte: 1 }},
                                                        'Truong.MaHeDaoTao': '{maHeDaoTao}',
                                                        'Truong.IdCha':'{idDonVi}'
                                                      }},
                                                    }},
                                                ],
                                                as: 'HocSinhs',
                                              }},
                                            }},
                                          ],
                                          as: 'DanhMucTotNghieps',
                                        }},
                                      }},
                                      {{
                                        $unwind: '$DanhMucTotNghieps'
                                      }},
                                      {{
                                        $addFields: {{
                                          TongHocSinh: {{
                                            $cond: {{
                                              if: {{ $isArray: '$DanhMucTotNghieps.HocSinhs' }},
                                              then: {{ $size: '$DanhMucTotNghieps.HocSinhs' }},
                                              else: 0,
                                            }},
                                          }},
                                        }},
                                      }},
                                      {{
                                        $group: {{
                                          _id: '$_id',
                                          NamThi: {{ $first: '$Ten' }},
                                          TongHocSinh: {{ $sum: '$TongHocSinh' }}
                                        }}
                                      }},
                                      {{
                                        $sort: {{
                                          NamThi: 1,
                                        }},
                                      }},
                                      {{
                                        $group: {{
                                          _id: null,
                                          totalRow: {{ $sum: '$TongHocSinh' }},
                                          thongKe: {{
                                            $push: {{
                                              NamThi: '$NamThi',
                                              TongHocSinh: '$TongHocSinh',
                                            }},
                                          }},
                                        }},
                                      }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        /// <summary>
        /// Lấy số lượng học sinh theo xếp loại (Biểu đồ trang chủ)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhTheoXepLoai(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            //var truongs = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
            //             .Find(x => x.Xoa == false && x.IdCha == idDonVi)
            //             .ToList()
            //             .Select(x => x.Id)
            //             .ToArray();
            //var truongIdsString = string.Join(",", truongs.Select(x => $"'{x}'"));

            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                        $match: {{
                                          Xoa: false,
                                          TrangThai: {{ $gte: 2 }},
                                        }},
                                      }},
                                      {{
                                        $addFields: {{
                                          IdTruong: {{ $toObjectId: '$IdTruong' }},
                                          IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
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
                                        $lookup: {{
                                          from: 'DanhMucTotNghiep',
                                          localField: 'IdDanhMucTotNghiep',
                                          foreignField: '_id',
                                          as: 'DanhMucTotNghieps',
                                        }},
                                      }},
                                      {{
                                        $unwind: '$Truongs',
                                      }},
                                      {{
                                        $unwind: '$DanhMucTotNghieps',
                                      }},
                                      {{
                                        $match: {{
                                          'Truongs.MaHeDaoTao': '{maHeDaoTao}',
                                          'Truongs.IdCha': '{idDonVi}',
                                          'DanhMucTotNghieps.IdNamThi': '{idNamThi}',
                                        }},
                                      }},
                                      {{
                                        $group: {{
                                          _id: null,
                                          XepLoaiGioi: {{
                                            $sum: {{
                                              $cond: [{{ $eq: ['$XepLoai', 'Giỏi'] }}, 1, 0],
                                            }},
                                          }},
                                          XepLoaiKha: {{
                                            $sum: {{
                                              $cond: [{{ $eq: ['$XepLoai', 'Khá'] }}, 1, 0],
                                            }},
                                          }},
                                          XepLoaiTrungBinh: {{
                                            $sum: {{
                                              $cond: [{{ $gte: ['$XepLoai', 'Trung Bình'] }}, 1, 0],
                                            }},
                                          }},
                                          TongHocSinh : {{$sum : 1}}
                                        }},
                                      }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        /// <summary>
        /// Lấy số lượng học sinh cấp phát bằng (Biểu đồ trang chủ)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="maHeDaoTao"></param>
        /// <returns></returns>
        public string GetSoLuongHocSinhCapPhatBang(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            //var truongs = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
            //             .Find(x => x.Xoa == false && x.IdCha == idDonVi)
            //             .ToList()
            //             .Select(x => x.Id)
            //             .ToArray();
            //var truongIdsString = string.Join(",", truongs.Select(x => $"'{x}'"));

            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                   {{
                                $match: {{
                                  Xoa: false,
                                }},
                              }},
                              {{
                                $addFields: {{
                                  IdTruong: {{ $toObjectId: '$IdTruong' }},
                                  IdDanhMucTotNghiep: {{ $toObjectId: '$IdDanhMucTotNghiep' }},
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
                                $lookup: {{
                                  from: 'DanhMucTotNghiep',
                                  localField: 'IdDanhMucTotNghiep',
                                  foreignField: '_id',
                                  as: 'DanhMucTotNghieps',
                                }},
                              }},
                              {{
                                $unwind: '$Truongs',
                              }},
                              {{
                                $unwind: '$DanhMucTotNghieps',
                              }},
                              {{
                                $match: {{
                                  'Truongs.MaHeDaoTao': '{maHeDaoTao}',
                                  'Truongs.IdCha': '{idDonVi}',
                                  'DanhMucTotNghieps.IdNamThi': '{idNamThi}',
                                }},
                              }},
                              {{
                                $group: {{
                                  _id: null,
                                  TongBangChuaPhat: {{
                                    $sum: {{
                                      $cond: [{{ $eq: ['$TrangThai', 5] }}, 1, 0],
                                    }},
                                  }},
                                  TongBangDaPhat: {{
                                    $sum: {{
                                      $cond: [{{ $eq: ['$TrangThai', 6] }}, 1, 0],
                                    }},
                                  }},
                                  TongSoLuongBang: {{
                                    $sum: {{
                                      $cond: [{{ $gte: ['$TrangThai', 5] }}, 1, 0],
                                    }},
                                  }},
                                }},
                              }},
                              ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);

            return json;
        }

        public string GetThongKeTongQuatByPhong(string idNamThi, string maHeDaoTao, string idDonVi)
        {
            var truongs = _mongoDatabase.GetCollection<TruongModel>(_collectionNameTruong)
                          .Find(x => x.Xoa == false && x.IdCha == idDonVi)
                          .ToList()
                          .Select(x => x.Id)
                          .ToArray();
            var truongIdsString = string.Join(",", truongs.Select(x => $"'{x}'"));

            string soLuongPhoiDaIn =  GetSoLuongPhoiDaIn(idNamThi, maHeDaoTao, truongIdsString);
            string soLuongDonViDaGui = GetSoLuongDonViDaGui(idNamThi, maHeDaoTao, idDonVi);
            string soLuongDonYeuCauCapBanSao = GetSoLuongDonYeuCauCapBanSao(idNamThi, maHeDaoTao, truongIdsString);
            string soLuongHocSinhChuaDuyet = GetSoLuongHocSinhChuaDuyet(idNamThi, maHeDaoTao, truongIdsString);

            string finalJson = $"{{\"SoLuongPhoiDaIn\": {(soLuongPhoiDaIn != null ? soLuongPhoiDaIn : "null")}" +
                $",\"SoLuongDonViDaGui\": {(soLuongDonViDaGui != null ? soLuongDonViDaGui : "null")} " +
                $",\"SoLuongDonYeuCauCapBanSao\": {(soLuongDonYeuCauCapBanSao != null ? soLuongDonYeuCauCapBanSao : "null")}" +
                $",\"SoLuongHocSinhChuaDuyet\": {(soLuongHocSinhChuaDuyet != null ? soLuongHocSinhChuaDuyet : "null")}}}";

            return finalJson;
        }

       

        #endregion

        #region Trường
        /// <summary>
        /// Lấy tổng số học sinh của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetTongSoHocSinhByTruong(string idTruong, string idNamThi)
        {
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                    $facet: {{
                                      hocSinhCount: [
                                        {{
                                          $match: {{
                                            Xoa: false,
                                            IdTruong: '{idTruong}',
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
                                          $match: {{
                                            'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                          }},
                                        }},
                                      ],
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      TongHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$hocSinhCount' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$hocSinhCount' }},
                                        }},
                                      }},
                                    }},
                                  }},
                               ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);
            return json;
        }


        /// <summary>
        /// Lấy số học sinh chờ duyệt của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSoHocSinhChoDuyetByTruong(string idTruong, string idNamThi)
        {
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                    $facet: {{
                                      hocSinhCount: [
                                        {{
                                          $match: {{
                                            Xoa: false,
                                            IdTruong: '{idTruong}',
                                            TrangThai: 1
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
                                          $match: {{
                                            'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                          }},
                                        }},
                                      ],
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      TongHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$hocSinhCount' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$hocSinhCount' }},
                                        }},
                                      }},
                                    }},
                                  }},
                               ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);
            return json;
        }


        /// <summary>
        /// Lấy số học sinh chờ duyệt của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSoHocSinhDaDuyetByTruong(string idTruong, string idNamThi)
        {
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                    $facet: {{
                                      hocSinhCount: [
                                        {{
                                          $match: {{
                                            Xoa: false,
                                            IdTruong: '{idTruong}',
                                            TrangThai: {{ $gte: 2 }}
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
                                          $match: {{
                                            'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                          }},
                                        }},
                                      ],
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      TongHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$hocSinhCount' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$hocSinhCount' }},
                                        }},
                                      }},
                                    }},
                                  }},
                               ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);
            return json;
        }

        /// <summary>
        /// Lấy số học sinh nhận bằng của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetSoHocSinhNhanBangByTruong(string idTruong, string idNamThi)
        {
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                  {{
                                    $facet: {{
                                      hocSinhCount: [
                                        {{
                                          $match: {{
                                            Xoa: false,
                                            IdTruong: '{idTruong}',
                                            TrangThai: {{ $gte: 5 }},
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
                                          $match: {{
                                            'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                          }},
                                        }},
                                      ],
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      TongHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$hocSinhCount' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$hocSinhCount' }},
                                        }},
                                      }},
                                    }},
                                  }},
                               ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);
            return json;
        }


        /// <summary>
        /// Lấy số học sinh đã nhận của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetHocSinhDaNhanBangByTruong(string idTruong, string idNamThi)
        {
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $facet: {{
                                      hocSinhCount: [
                                        {{
                                          $match: {{
                                            Xoa: false,
                                            IdTruong: '{idTruong}',
                                            TrangThai: 6,
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
                                          $match: {{
                                            'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                          }},
                                        }},
                                      ],
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      TongHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$hocSinhCount' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$hocSinhCount' }},
                                        }},
                                      }},
                                    }},
                                  }},
                               ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);
            return json;
        }

        /// <summary>
        /// Lấy số học sinh đã nhận của trường theo năm học (Trang chủ - trường)
        /// </summary>
        /// <param name="idNamThi"></param>
        /// <param name="idTruong"></param>
        /// <returns></returns>
        public string GetHocSinhChuaNhanBangByTruong(string idTruong, string idNamThi)
        {
            var cmdRes = $@"
                        {{
                            'aggregate': 'HocSinh', 
                            'allowDiskUse': true,
                            'pipeline':[
                                 {{
                                    $facet: {{
                                      hocSinhCount: [
                                        {{
                                          $match: {{
                                            Xoa: false,
                                            IdTruong: '{idTruong}',
                                            TrangThai: 5,
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
                                          $match: {{
                                            'DanhMucTotNghiep.IdNamThi': '{idNamThi}',
                                          }},
                                        }},
                                      ],
                                    }},
                                  }},
                                  {{
                                    $project: {{
                                      TongHocSinh: {{
                                        $cond: {{
                                          if: {{ $eq: [{{ $size: '$hocSinhCount' }}, 0] }},
                                          then: 0,
                                          else: {{ $size: '$hocSinhCount' }},
                                        }},
                                      }},
                                    }},
                                  }},
                               ],
                            'cursor': {{ 'batchSize': 25 }},
                        }}";

            var data = _mongoDatabase.RunCommand<object>(cmdRes);
            string json = ModelProvider.ExtractJsonFromMongo(data);
            return json;
        }


        public string GetThongKeTongQuatByTruong(string idTruong, string idNamThi)
        {
            string tongSoHocSinh = GetTongSoHocSinhByTruong(idTruong, idNamThi);
            string soHocSinhChoDuyet = GetSoHocSinhChoDuyetByTruong(idTruong, idNamThi);
            string soHocSinhDaDuyet = GetSoHocSinhDaDuyetByTruong(idTruong, idNamThi);
            string soHocSinhNhanBang = GetSoHocSinhNhanBangByTruong(idTruong, idNamThi);
            string soHocSinhDaNhanBang = GetHocSinhDaNhanBangByTruong(idTruong, idNamThi);
            string soHocSinhChuaNhanBang = GetHocSinhChuaNhanBangByTruong(idTruong, idNamThi);

            string finalJson = $"{{\"TongSoHocSinh\": {(tongSoHocSinh != null ? tongSoHocSinh : "null")}" +
                $",\"SoHocSinhChoDuyet\": {(soHocSinhChoDuyet != null ? soHocSinhChoDuyet : "null")} " +
                $",\"SoHocSinhDaDuyet\": {(soHocSinhDaDuyet != null ? soHocSinhDaDuyet : "null")}" +
                $",\"SoHocSinhNhanBang\": {(soHocSinhNhanBang != null ? soHocSinhNhanBang : "null")}" +
                $",\"SoHocSinhDaNhanBang\": {(soHocSinhDaNhanBang != null ? soHocSinhDaNhanBang : "null")}" +
                $",\"SoHocSinhChuaNhanBang\": {(soHocSinhChuaNhanBang != null ? soHocSinhChuaNhanBang : "null")}}}";

            return finalJson;
        }

        #endregion

    }
}
