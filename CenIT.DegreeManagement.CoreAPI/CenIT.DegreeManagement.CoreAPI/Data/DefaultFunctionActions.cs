using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;

namespace CenIT.DegreeManagement.CoreAPI.Data
{
    public static class DefaultFunctionActions
    {
        public static List<FunctionActionModel> GetDefaultActions()
        {
            return new List<FunctionActionModel>
        {
            new FunctionActionModel { FunctionActionId = 0, Action = "Login" },
            new FunctionActionModel { FunctionActionId = 0, Action = "Logout" }
        };
        }
    }
}
