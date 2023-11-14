using Microsoft.AspNetCore.Mvc.Filters;

namespace CenIT.DegreeManagement.CoreAPI.Attributes
{
    /// <summary>
    /// AllowAnyPermission is an attribute used as a command flag to bypass permission checks.
    /// </summary>
    public class AllowAnyPermission : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            base.OnActionExecuting(context);
        }
    }
}
