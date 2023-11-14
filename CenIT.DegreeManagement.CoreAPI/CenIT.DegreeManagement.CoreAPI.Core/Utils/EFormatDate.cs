using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Utils
{
    public static class EFormatDate
    {
        #region DateType enum

        public enum DateType
        {
            ddMMyyyy = 1,
            MMddyyyy = 2,
            yyyyMMdd = 3
        }

        #endregion

        /// <summary>
        ///     Convert2s the string.
        /// </summary>
        /// <returns></returns>
        public static string Convert2String()
        {
            var datestring = string.Empty;
            datestring += DateTime.Now.Year.ToString();
            datestring += DateTime.Now.Month.ToString();
            datestring += DateTime.Now.Day.ToString();
            datestring += DateTime.Now.Hour.ToString();
            datestring += DateTime.Now.Minute.ToString();
            datestring += DateTime.Now.Second.ToString();
            return datestring;
        }

        /// <summary>
        ///     Converts the hour string.
        /// </summary>
        /// <returns></returns>
        public static string ConvertHourString()
        {
            var datestring = string.Empty;

            datestring += DateTime.Now.Hour.ToString();
            datestring += DateTime.Now.Minute.ToString();
            datestring += DateTime.Now.Second.ToString();
            return datestring;
        }

        /// <summary>
        ///     Converts date from dd/MM/yyyy to yyyy/MM/dd.
        /// </summary>
        /// <returns>DateTime with format yyyy/MM/dd</returns>
        public static DateTime YYYYMMDD(string date, DateType type, char chSplit = '/')
        {
            var strdate = new string[3];
            DateTime dt;
            if (!string.IsNullOrEmpty(date))
                strdate = date.Split(chSplit);
            else
                return DateTime.Now;

            switch (type)
            {
                case DateType.ddMMyyyy:
                    dt =
                        Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[2]), Convert.ToInt32(strdate[1]),
                            Convert.ToInt32(strdate[0])));
                    break;
                case DateType.MMddyyyy:
                    dt =
                        Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[2]), Convert.ToInt32(strdate[0]),
                            Convert.ToInt32(strdate[1])));
                    break;
                case DateType.yyyyMMdd:
                    dt =
                        Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[0]), Convert.ToInt32(strdate[1]),
                            Convert.ToInt32(strdate[2])));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return dt;
        }

        public static Boolean isValidDate(string date, DateType type, char chSplit = '/')
        {
            try
            {
                var strdate = new string[3];
                DateTime dt;
                if (!string.IsNullOrEmpty(date))
                    strdate = date.Split(chSplit);
                else
                    return false;

                switch (type)
                {
                    case DateType.ddMMyyyy:
                        dt =
                            Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[2]), Convert.ToInt32(strdate[1]),
                                Convert.ToInt32(strdate[0])));
                        break;
                    case DateType.MMddyyyy:
                        dt =
                            Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[2]), Convert.ToInt32(strdate[0]),
                                Convert.ToInt32(strdate[1])));
                        break;
                    case DateType.yyyyMMdd:
                        dt =
                            Convert.ToDateTime(new DateTime(Convert.ToInt32(strdate[0]), Convert.ToInt32(strdate[1]),
                                Convert.ToInt32(strdate[2])));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Converts the date.
        /// </summary>
        /// <returns></returns>
        public static string ConvertDate()
        {
            var datestring = string.Empty;

            datestring += DateTime.Now.Day + "/";
            datestring += DateTime.Now.Month + "/";
            datestring += DateTime.Now.Year.ToString();

            return string.Format(datestring, "dd/MM/yyyy");
        }

        /// <summary>
        ///     Converts the date VN.
        /// </summary>
        /// <returns></returns>
        public static string ConvertDayToVn(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Monday)
                return "Thứ hai";
            if (dt.DayOfWeek == DayOfWeek.Tuesday)
                return "Thứ ba";
            if (dt.DayOfWeek == DayOfWeek.Wednesday)
                return "Thứ tư";
            if (dt.DayOfWeek == DayOfWeek.Thursday)
                return "Thứ năm";
            if (dt.DayOfWeek == DayOfWeek.Friday)
                return "Thứ sáu";
            if (dt.DayOfWeek == DayOfWeek.Saturday)
                return "Thứ bảy";
            if (dt.DayOfWeek == DayOfWeek.Sunday)
                return "Chủ nhật";

            return "";
        }

        /// <summary>
        ///     Get number week of year for culture vi-VN.
        /// </summary>
        /// <returns> Number week to int type</returns>
        public static int GetWeekByDate(DateTime date)
        {
            var cul = new CultureInfo("vi-VN");
            var dfi = cul.DateTimeFormat;
            var cal = dfi.Calendar;
            return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        /// <summary>
        ///     Get first day of week.
        /// </summary>
        /// <returns> First Day of week to DateTime</returns>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            var cul = new CultureInfo("vi-VN");
            var dfi = cul.DateTimeFormat;
            var cal = dfi.Calendar;
            var jan1 = new DateTime(year, 1, 1);

            var daysOffset = (int)dfi.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday;

            if (daysOffset < 0)
                firstMonday = jan1.AddDays(daysOffset + 7);
            else
                firstMonday = jan1.AddDays(daysOffset);

            var firstWeek = GetWeekByDate(jan1);

            if (daysOffset == 0)
                return firstMonday.AddDays((weekOfYear - 1) * 7);

            if (firstWeek == 0) return firstMonday.AddDays(weekOfYear * 7);
            return firstMonday.AddDays((weekOfYear - 2) * 7);
        }

        /// <summary>
        ///     Get last day of week
        /// </summary>
        /// <returns> DateTime - Last day of week</returns>
        public static DateTime LastDayOfWeek(int year, int weekOfyear)
        {
            return FirstDateOfWeek(year, weekOfyear).AddDays(6);
        }

        /// <summary>
        ///     Get day list in week
        /// </summary>
        /// <returns> List<DateTime> - day list in week</returns>
        public static List<DateTime> GetDayInWeek(int year, int weekOfYear)
        {
            var list = new List<DateTime>();
            var firstMonday = FirstDateOfWeek(year, weekOfYear);

            for (var i = 0; i < 7; i++)
            {
                DateTime date;
                date = firstMonday;
                date = date.AddDays(i);
                if (date.Year == year)
                    list.Add(date);
            }

            return list;
        }
    }
}
