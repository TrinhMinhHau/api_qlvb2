using AutoMapper;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DanhMuc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Phoi;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.SoGoc;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.TinTuc;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Models.Sys.Truong;

namespace CenIT.DegreeManagement.CoreAPI.Processor
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<HocSinhImportViewModel, HocSinhModel>();
            CreateMap<CauHinhPhoiGocInputModel, CauHinhPhoiGocModel>();
            CreateMap<HocSinhImportVM, HocSinhTmpModel>();
            CreateMap<HocSinhTmpModel, HocSinhModel>();

            CreateMap<PhuLucSoGocModel, HocSinhDTO>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());
            //.ForMember(dest => dest.DanhMucTotNghiep.IdNamThi, opt => opt.MapFrom(src => src.IdNamThi))
            //.ForMember(dest => dest.DanhMucTotNghiep.NgayCapBang, opt => opt.MapFrom(src => src.NgayCap))
            //.ForMember(dest => dest.DanhMucTotNghiep.IdNamThi, opt => opt.MapFrom(src => src.IdNamThi))
            //.ForMember(dest => dest.MaHinhThucDaoTao, opt => opt.MapFrom(src => src.MaHTDT))
            //.ForMember(dest => dest.HoiDong, opt => opt.MapFrom(src => src.HoiDongThi));

            CreateMap<HocSinhVM, TraCuuHocSinhDTO>()
                .ForMember(dest => dest.TenTruong, opt => opt.MapFrom(src => src.Truong.Ten))
                .ForMember(dest => dest.KhoaThi, opt => opt.MapFrom(src => src.KhoaThi.Ngay))
                .ForMember(dest => dest.TenHinhThucDaoTao, opt => opt.MapFrom(src => src.HinhThucDaoTao.Ten))
                .ForMember(dest => dest.NamThi, opt => opt.MapFrom(src => src.NamThi.Ten)); 
                
            //CreateMap<SoGocModel, SoGocViewModel>();
            CreateMap<DanhMucTotNghiepModel, DanhMucTotNghiepViewModel>();
            CreateMap<SoGocModel, ThongTinHocSinhInVanBangViewModel>();
            CreateMap<SoGocModel, ThongTinDanhSachHocSinhInVanBangViewModel>();
            CreateMap<TinTucViewModel, TinTucListModel>();
            CreateMap<TinTucViewModel, TinTucListModel>();
            //truong
            CreateMap<TruongModel, TruongDTO>();
            CreateMap<HocSinhViewModel, HocSinhDTO>();
            CreateMap<HocSinhInBangModel, HocSinhInBangDTO>();



        }
    }
}
