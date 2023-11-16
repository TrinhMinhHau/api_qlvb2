using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.DuLieuHocSinh;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.QuanLySo;
using CenIT.DegreeManagement.CoreAPI.Models.DuLieuHocSinh;

namespace CenIT.DegreeManagement.CoreAPI.Processor.AutoMapperProfiles
{
    public class AutoMapperDanhMucTotNghiep : AutoMapperProfile
    {
        public AutoMapperDanhMucTotNghiep()
        {
            CreateMap<TruongHocViaDanhMucTotNghiepModel, TruongHocViaDanhMucTotNghiepDTO>();
            CreateMap<TruongHocViaDanhMucTotNghiepModel, TruongHocViaDMTNHasPermisionDTO>();
            //CreateMap<TruongHocViaDanhMucTotNghiepModel, DanhMucTotNghiepViewModel>()
            // .IncludeMembers(src => src.DanhMucTotNghiep)
            // .ForMember(dest => dest.TongSoTruong, opt => opt.Ignore())
            // .ForMember(dest => dest.NamThi, opt => opt.Ignore())
            // .ForMember(dest => dest.HinhThucDaoTao, opt => opt.Ignore());
        }
    }
}
