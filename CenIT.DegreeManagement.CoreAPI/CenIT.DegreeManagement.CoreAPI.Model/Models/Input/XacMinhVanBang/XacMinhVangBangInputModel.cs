using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Model.Models.Input.XacMinhVanBang
{
    public class XacMinhVangBangInputBaseModel
    {
        public string DonViYeuCauXacMinh { get; set; } = null!;
        public string CongVanSo { get; set; } = null!;
        public DateTime NgayTrenCongVan { get; set; }
        public string? PathFileYeuCau { get; set; }
        public IFormFile? FileYeuCau { get; set; }
        public string NguoiThucHien { get; set; } = null!;
    }

    public class XacMinhVangBangInputModel : XacMinhVangBangInputBaseModel
    {
        public string IdHocSinh { get; set; } = null!;
    }
    public class XacMinhVangBangListInputModel : XacMinhVangBangInputBaseModel
    {
        public List<string> IdHocSinhs { get; set; } = null!;
    }

}
