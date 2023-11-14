using CenIT.DegreeManagement.CoreAPI.Core.Enums.TraCuu;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo
{
    public class PhuLucSoGocModel : BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string IdHocSinh { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public DateTime NgaySinh { get; set; }
        public string NoiSinh { get; set; }
        public bool GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string IdNamThi { get; set; }
        public string XepLoai { get; set; }
        public string MaHTDT { get; set; }
        public string? HoiDongThi { get; set; }
        public string IdKhoaThi { get; set; }
        public DateTime NgayCap { get; set; }
        public string? SoHieuVanBangCapLai { get; set; }
        public string SoHieuVanBangCu { get; set; }
        public string? SoVaoSoCapBangCapLai { get; set; }
        public string SoVaoSoCapBangCu { get; set; }
        public string PathFileVanBan { get; set; }
        public string NoiDungChinhSua { get; set; }
        public string LyDo { get; set; }
        public LoaiHanhDongEnum LoaiHanhDong { get; set; }
        public string? IdPhoiGoc { get; set; }

    }

    public class PhuLucSoGocViewModel : PhuLucSoGocModel
    {
        public HocSinhModel HocSinh { get; set; } = new HocSinhModel();


    }

    public class PhuLucSoGocSearchModel : SearchParamModel
    {
        public string IdDanhMucTotNghiep { get; set; }
        public string IdTruong { get; set; }
        //public string NguoiThucHien { get; set; }
    }

    public class LichSuChinhSuaModel : PhuLucSoGocModel
    {
        public DateTime KhoaThi { get; set; }
        public string TenHinhThucDaoTao { get; set; }
        public string NamThi { get; set; }
    }


    public class PhuLucViewModel :  BaseModel
    {
        public string IdHocSinh { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public DateTime NgaySinh { get; set; }
        public string NoiSinh { get; set; }
        public bool GioiTinh { get; set; }
        public string DanToc { get; set; }
        public string XepLoai { get; set; }
        public string IdKhoaThi { get; set; }
        public DateTime NgayCap { get; set; }
        public string IdNamThi { get; set; }
        public string MaHTDT { get; set; }
        public string? SoHieuVanBangCapLai { get; set; }
        public string SoHieuVanBangCu { get; set; }
        public string? SoVaoSoCapBangCapLai { get; set; }
        public string SoVaoSoCapBangCu { get; set; }
        public string? HoiDongThi { get; set; }

    }


}
