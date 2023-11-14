using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.XepLoai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Utils
{
    public static class GraduationType
    {
        public class EvaluationResult
        {
            public string HocLuc { get; set; }
            public string KetQua { get; set; }
            public string XepLoai { get; set; }
        }

        public static EvaluationResult AutoEvaluateGraduation(string hocLuc, string ketQua, string xepLoai, string hanhKiem,double? dtb,
                                    double? diemNguVan, double? diemToan, string dienXT, string isLanDauXTN)
        {
            hocLuc = ClassifyGrade(dtb);
            ketQua = CheckPassOrFail(hanhKiem, hocLuc, dtb, diemNguVan, diemToan, dienXT, isLanDauXTN);
            xepLoai = CheckGraduationType(hanhKiem, hocLuc, isLanDauXTN, ketQua);

            return new EvaluationResult
            {
                HocLuc = hocLuc,
                KetQua = ketQua,
                XepLoai = xepLoai
            };
        }

        private static string CheckGraduationType(string hanhkiem, string hocluc, string isLanDauXTN, string ketQua)
        {
            if (!string.IsNullOrEmpty(isLanDauXTN) && !string.IsNullOrEmpty(ketQua))
            {
                if (HanhKiemTot(hanhkiem) && HocLucGioi(hocluc))
                {
                    return XepLoaiTotNghieEnum.Excellent.ToStringValue();
                }
                else if ((HanhKiemKha(hanhkiem) || HanhKiemTot(hanhkiem)) && (HocLucKha(hocluc)) || (HanhKiemKha(hanhkiem) && HocLucGioi(hocluc)))
                {
                    return XepLoaiTotNghieEnum.Good.ToStringValue();
                }
                else
                {
                    return XepLoaiTotNghieEnum.Average.ToStringValue();
                }
            } else if(!string.IsNullOrEmpty(ketQua))
            {
                return XepLoaiTotNghieEnum.Average.ToStringValue();
            } else
            {
                return null;
            }
        }

        private static string ClassifyGrade(double? dtb)
        {
            if (dtb >= 8 && dtb <= 10)
            {
                // LOẠI GIỎI
                return XepLoaiHocLucEnum.Excellent.ToStringValue();
            }
            else if (dtb >= 6.5 && dtb < 8)
            {
                // Loại khá
                return XepLoaiHocLucEnum.Good.ToStringValue();
            }
            else if (dtb >= 5 && dtb < 6.5)
            {
                //Loại TB
                return XepLoaiHocLucEnum.Average.ToStringValue();
            }
            else if (dtb >= 3.5 && dtb < 5)
            {
                //Loại YẾU
                return XepLoaiHocLucEnum.Weak.ToStringValue();
            }
            else
            {
                //Loại kém
                return XepLoaiHocLucEnum.Poor.ToStringValue();
            }
        }

        private static string CheckPassOrFail(string hanhKiem, string hocLuc, double? dtb, double? diemNguVan, double? diemToan, string dienXT, string isLanDauXTN)
        {
            List<string> hanhKiemListPass = new List<string> { XepLoaiHanhKiem.Excellent.ToStringValue(), XepLoaiHanhKiem.Good.ToStringValue(), XepLoaiHanhKiem.Average.ToStringValue() };
            List<string> hocLucListPass = new List<string> { XepLoaiHocLucEnum.Excellent.ToStringValue(), XepLoaiHocLucEnum.Good.ToStringValue(), XepLoaiHocLucEnum.Average.ToStringValue() };

            if (!string.IsNullOrEmpty(isLanDauXTN))
            {

                if ((hanhKiemListPass.Contains(hanhKiem) && hocLucListPass.Contains(hocLuc)))
                {
                    return "x";
                }

                if (hocLuc == XepLoaiHocLucEnum.Weak.ToStringValue() && dtb >= 3.5 && diemNguVan >= 5 && diemToan >= 5 && !string.IsNullOrEmpty(dienXT))
                {
                    return "x";
                }

                return null;
            } else
            {
                if ((hanhKiemListPass.Contains(hanhKiem) && hocLucListPass.Contains(hocLuc) && diemToan >= 5 && diemToan >= 5))
                {
                    return "x";
                }

                return null;
            }
        }

        private static bool HanhKiemTot(string hanhkiem)
        {
            return hanhkiem == XepLoaiHanhKiem.Excellent.ToStringValue();
        }

        private static bool HanhKiemKha(string hanhkiem)
        {
            return hanhkiem == XepLoaiHanhKiem.Good.ToStringValue();
        }

        private static bool HanhKiemTrungBinh(string hanhkiem)
        {
            return hanhkiem == XepLoaiHanhKiem.Average.ToStringValue();
        }


        private static bool HocLucGioi(string hocluc)
        {
            return hocluc == XepLoaiHocLucEnum.Excellent.ToStringValue();
        }

        private static bool HocLucKha(string hocluc)
        {
            return hocluc == XepLoaiHocLucEnum.Good.ToStringValue();
        }

        private static bool HocLucTrungBinh(string hocluc)
        {
            return hocluc == XepLoaiHocLucEnum.Average.ToStringValue();
        }

    }
}
