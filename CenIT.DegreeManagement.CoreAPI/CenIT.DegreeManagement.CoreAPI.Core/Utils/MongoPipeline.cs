using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Utils
{
    public static class MongoPipeline
    {
        public static string GenerateSortPipeline(string order, string orderDir, string orderDefault)
        {
            string result;
            if (order.ToLower() == "cccd")
            {
                order = order.ToUpper();
            }
            string value = EString.CapitalizeFirstLetter(order);
            string valueDefault = EString.CapitalizeFirstLetter(orderDefault);

            switch (order)
            {
                case "0":
                case "1":
                    if (valueDefault.ToLower() == "hoten")
                    {
                        result = orderDir.ToUpper() == "ASC" ?
                             $@"{{
                                $addFields: {{
                                  TenCuoi: {{ $arrayElemAt: [{{ $split: ['$HoTen', ' '] }}, -1] }},
                                }},
                              }},
                              {{
                                $sort: {{
                                  TenCuoi: 1,
                                  HoTen: 1,
                                }},
                              }}," :
                                   $@"{{
                                $addFields: {{
                                  TenCuoi: {{ $arrayElemAt: [{{ $split: ['$HoTen', ' '] }}, -1] }},
                                }},
                              }},
                              {{
                                $sort: {{
                                  TenCuoi: -1,
                                  HoTen: -1,
                                }},
                              }},";
                    }
                    else
                    {
                        result = orderDir.ToUpper() == "ASC"
                        ? $@" {{
                                    $sort: {{
                                      {valueDefault}: 1, 
                                    }},
                                  }},"
                        : $@" {{
                                    $sort: {{
                                      {valueDefault}: -1, 
                                    }},
                              }},";
                    }
                    break;
                default:
                    if (value.ToLower() == "hoten")
                    {
                        result = orderDir.ToUpper() == "ASC" ?
                             $@"{{
                                $addFields: {{
                                  TenCuoi: {{ $arrayElemAt: [{{ $split: ['$HoTen', ' '] }}, -1] }},
                                }},
                              }},
                              {{
                                $sort: {{
                                  TenCuoi: 1,
                                  HoTen: 1,
                                }},
                              }}," :
                                   $@"{{
                                $addFields: {{
                                  TenCuoi: {{ $arrayElemAt: [{{ $split: ['$HoTen', ' '] }}, -1] }},
                                }},
                              }},
                              {{
                                $sort: {{
                                  TenCuoi: -1,
                                  HoTen: -1,
                                }},
                              }},";
                    }
                    else
                    {
                        result = orderDir.ToUpper() == "ASC"
                        ? $@" {{
                                    $sort: {{
                                      {value}: 1, 
                                    }},
                                  }},"
                        : $@" {{
                                    $sort: {{
                                      {value}: -1, 
                                    }},
                              }},";
                    }
                    break;
            }
            return result;
        }

        public static string GeneratePaginationPipeline(int pageSize, int startIndex, string nameArray)
        {
            if(pageSize < 0)
            {

                return "";
            }
            int skip = (startIndex - 1) * pageSize;

            var result = $@" {{
                                    $project: {{
                                        _id: 0,
                                        totalRow: 1,
                                        data: {{ $slice: ['{nameArray}', {skip + pageSize}, {pageSize}] }},
                                        }},
                                  }},";

            return result;
        }


        public static string GenerateFilterFromDateToDatePipeline(DateTime? fromDate, DateTime? toDate, string nameArray)
        {

            if (fromDate == null && toDate == null) return "";

            string fromDateString = fromDate == null ? "" : $@"$gte: ISODate('{fromDate?.ToString("yyyy-MM-dd")}'),";
            string toDateString = toDate == null ? "" : $@"$lte: ISODate('{toDate?.ToString("yyyy-MM-dd")}'),";

            string result = $@"'{nameArray}': {{
                                                 {fromDateString}
                                                 {toDateString}
                                              }},";
            return result;
        }

    }
}
