using CenIT.DegreeManagement.CoreAPI.Core.Enums;
using CenIT.DegreeManagement.CoreAPI.Core.Enums.XepLoai;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using static CenIT.DegreeManagement.CoreAPI.Core.Helpers.DataTableValidatorHelper;

namespace CenIT.DegreeManagement.CoreAPI.Validation
{
    public static class HocSinhValidationRules
    {
        public static List<ValidationRule> GetRules(string[] monThis, string[] danTocs)
        {
            return new List<ValidationRule>
            {
                new ValidationRule
                {
                    ColumnName = "STT",
                    IsRequired = true,
                    IsUnique = true
                },
                new ValidationRule
                {
                    ColumnName = "Hoten",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "CCCD",
                    IsRequired = true,
                    IsUnique = true, // Kiểm tra trùng lặp CCCD
                },
                new ValidationRule
                {
                    ColumnName = "GioiTinh",
                    CustomValidator = value => string.IsNullOrEmpty(value) || value.ToLower() == "nam" || value.ToLower() == "nữ"
                },
                new ValidationRule
                {
                    ColumnName = "NoiSinh",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "NgaySinh",
                    IsRequired = true,
                    CustomValidator = value => DateTime.TryParse(value, out _)
                },
                  new ValidationRule
                {
                    ColumnName = "DanToc",
                    IsRequired = true,
                    CustomValidator = value => danTocs.Contains(value)
                },
                  new ValidationRule
                {
                    ColumnName = "Mon1",
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                    new ValidationRule
                {
                    ColumnName = "Mon2",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                      new ValidationRule
                {
                    ColumnName = "Mon3",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                        new ValidationRule
                {
                    ColumnName = "Mon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                          new ValidationRule
                {
                    ColumnName = "Mon5",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                  new ValidationRule
                {
                    ColumnName = "Mon6",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon2",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon3",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon5",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon6",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemXTN",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemKK",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemTB12",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                  new ValidationRule
                {
                    ColumnName = "XepLoai",
                    IsRequired = true,
                    CustomValidator = value =>  value.ToLower() == "khá" || value.ToLower() == "giỏi" || value.ToLower() == "trung bình" || value.ToLower() == "yếu"
                },
                 new ValidationRule
                {
                    ColumnName = "DiaChi",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "Lop",
                    IsRequired = true
                },
            };
        }

        public static List<ValidationRule> GetRuleHocSinhOld(string[] monThis, string[] danTocs, string[] arrayCccd, string[] arraySoHieu, string[] arraySoVaoSo)
        {
            return new List<ValidationRule>
            {
                new ValidationRule
                {
                    ColumnName = "STT",
                    IsRequired = true,
                    IsUnique = true
                },
                new ValidationRule
                {
                    ColumnName = "Hoten",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "CCCD",
                    IsRequired = true,
                    IsUnique = true,
                    ExistValidator = value => !arrayCccd.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "GioiTinh",
                    CustomValidator = value => string.IsNullOrEmpty(value) || value.ToLower() == "nam" || value.ToLower() == "nữ"
                },
                new ValidationRule
                {
                    ColumnName = "NoiSinh",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "NgaySinh",
                    IsRequired = true,
                    CustomValidator = value => DateTime.TryParse(value, out _)
                },
                  new ValidationRule
                {
                    ColumnName = "DanToc",
                    IsRequired = true,
                    CustomValidator = value => danTocs.Contains(value)
                },
                  new ValidationRule
                {
                    ColumnName = "Mon1",
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                    new ValidationRule
                {
                    ColumnName = "Mon2",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                      new ValidationRule
                {
                    ColumnName = "Mon3",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                        new ValidationRule
                {
                    ColumnName = "Mon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                          new ValidationRule
                {
                    ColumnName = "Mon5",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                            new ValidationRule
                {
                    ColumnName = "Mon6",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon2",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon3",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon5",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon6",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemXTN",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemKK",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemTB12",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                  new ValidationRule
                {
                    ColumnName = "XepLoai",
                    IsRequired = true,
                    CustomValidator = value =>  value.ToLower() == "khá" || value.ToLower() == "giỏi" || value.ToLower() == "trung bình" || value.ToLower() == "yếu"
                },
                new ValidationRule
                {
                    ColumnName = "DiaChi",
                    IsRequired = true
                },
                 new ValidationRule
                {
                    ColumnName = "SoHieuVanBang",
                    IsRequired = true,
                    IsUnique = true,
                    ExistValidator = value => !arraySoHieu.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "SoVaoSoCapBang",
                    IsRequired = true,
                    IsUnique = true,
                    ExistValidator = value => !arraySoVaoSo.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "Lop",
                    IsRequired = true
                },
            };
        }

        public static List<ValidationRule> GetRulesHocSinhNew(string[] monThis, string[] danTocs, string[] arrayCccd, bool isAutoGraded)
        {
            return new List<ValidationRule>
            {
                new ValidationRule
                {
                    ColumnName = "STT",
                    IsRequired = true,
                    IsUnique = true
                },
                new ValidationRule
                {
                    ColumnName = "Hoten",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "CCCD",
                    IsRequired = true,
                    IsUnique = true, // Kiểm tra trùng lặp CCCD
                    ExistValidator = value => !arrayCccd.Contains(value) // Kiểm tra cccd đã tồn tại chưa
                },
                new ValidationRule
                {
                    ColumnName = "GioiTinh",
                    IsRequired = true,
                    CustomValidator = value => string.IsNullOrEmpty(value) || value.ToLower() == GenderEnum.Male.ToStringValue().ToLower() 
                                                                           || value.ToLower() == GenderEnum.Female.ToStringValue().ToLower()
                },
                new ValidationRule
                {
                    ColumnName = "NoiSinh",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "NgaySinh",
                    IsRequired = true,
                    CustomValidator = value => DateTime.TryParse(value, out _)
                },
                  new ValidationRule
                {
                    ColumnName = "DanToc",
                    IsRequired = true,
                    CustomValidator = value => danTocs.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "DiemTB",
                    IsRequired = true,
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)

                },
                new ValidationRule
                {
                    ColumnName = "HanhKiem",
                    IsRequired = true,
                    CustomValidator = value => string.IsNullOrEmpty(value) || value.ToLower() == XepLoaiHanhKiem.Excellent.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiHanhKiem.Good.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiHanhKiem.Average.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiHanhKiem.Weak.ToStringValue().ToLower()
                },
                new ValidationRule
                {
                    ColumnName = "HocLuc",
                    IsRequired = !isAutoGraded,
                    CustomValidator = value => string.IsNullOrEmpty(value) || value.ToLower() == XepLoaiHocLucEnum.Excellent.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiHocLucEnum.Good.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiHocLucEnum.Average.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiHocLucEnum.Weak.ToStringValue().ToLower()
                },
                  new ValidationRule
                {
                    ColumnName = "MonNV",
                    IsRequired = true,
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                    new ValidationRule
                {
                    ColumnName = "MonTO",
                    IsRequired = true,
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                      new ValidationRule
                {
                    ColumnName = "Mon3",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                        new ValidationRule
                {
                    ColumnName = "Mon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                          new ValidationRule
                {
                    ColumnName = "Mon5",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                  new ValidationRule
                {
                    ColumnName = "Mon6",
                     CustomValidator = value => string.IsNullOrEmpty(value) || monThis.Contains(value)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMonNV",
                    IsRequired = true,
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMonTO",
                    IsRequired = true,
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon3",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon4",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                 new ValidationRule
                {
                    ColumnName = "DiemMon5",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemMon6",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemXTN",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemKK",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "DiemTB",
                    CustomValidator = value => string.IsNullOrEmpty(value) || Double.TryParse(value, out _)
                },
                new ValidationRule
                {
                    ColumnName = "XepLoai",
                    IsRequired = !isAutoGraded,
                    CustomValidator = value => string.IsNullOrEmpty(value) || value.ToLower() == XepLoaiTotNghieEnum.Excellent.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiTotNghieEnum.Good.ToStringValue().ToLower()
                                                                           || value.ToLower() == XepLoaiTotNghieEnum.Average.ToStringValue().ToLower()
                },
                 new ValidationRule
                {
                    ColumnName = "DiaChi",
                    IsRequired = true
                },
                new ValidationRule
                {
                    ColumnName = "Lop",
                    IsRequired = true
                },
            };
        }
    }
}
