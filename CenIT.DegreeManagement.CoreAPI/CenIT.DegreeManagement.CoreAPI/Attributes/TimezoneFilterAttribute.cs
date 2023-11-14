using CenIT.DegreeManagement.CoreAPI.Core.Attributes;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CenIT.DegreeManagement.CoreAPI.Attributes
{
    public class TimezoneFilterAttribute : ActionFilterAttribute
    {
        private readonly TimezoneService _timezoneService;

        public TimezoneFilterAttribute(TimezoneService timezoneService)
        {
            _timezoneService = timezoneService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var localOffset = _timezoneService.LocalOffset;

            foreach (var actionArgument in context.ActionArguments)
            {
                ConvertDateTimeProperties(actionArgument.Value, localOffset);
            }
        }

        private void ConvertDateTimeProperties(object obj, TimeSpan localOffset)
        {
            if (obj == null)
            {
                return;
            }

            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(DateTime))
                {
                    DateTime dateTime = (DateTime)property.GetValue(obj);
                    DateTime convertedDateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, TimeZoneInfo.Utc).Add(localOffset);
                    property.SetValue(obj, convertedDateTime);
                }
                else if (property.PropertyType.IsClass && !property.PropertyType.Namespace.StartsWith("System"))
                {
                    ConvertDateTimeProperties(property.GetValue(obj), localOffset);
                }
            }
        }
    }
}
