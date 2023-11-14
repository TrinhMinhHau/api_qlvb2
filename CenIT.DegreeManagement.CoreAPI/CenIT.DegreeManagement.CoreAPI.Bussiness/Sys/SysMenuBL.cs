using CenIT.DegreeManagement.CoreAPI.Core.Business;
using CenIT.DegreeManagement.CoreAPI.Core.Processor;
using CenIT.DegreeManagement.CoreAPI.Core.Provider;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using Npgsql;
using System.Data.Common;

namespace CenIT.DegreeManagement.CoreAPI.Bussiness.Sys
{
    public class SysMenuBL : NpgsqlConnector
    {
        #region Name function or procedure
        private string _connectionString;
        private string _functionSysMenuGetByUserName = "fn_sys_menu_getbyusername";
        private string _menuSave = "p_sys_menu_save";
        private string _getMenuAll = "fn_get_menu_all";
        private string _menuDelete = "p_sys_menu_delete";
        private string _menu_getbyid = "fn_sys_menu_getbyid";
        #endregion

        #region Parameter
        private string _userName = "@p_username";
        private string _menuId = "@p_menuid";
        private string _name = "@p_name";
        private string _position = "@p_position";
        private string _parentId = "@p_parentid";
        private string _link = "@p_link";
        private string _icon = "@p_icon";
        private string _functionActionId = "@p_function_action_id";
        private string _isShow = "@p_is_show";
        #endregion

        public SysMenuBL(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Lấy danh sách menu theo username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<MenuModel> GetMenuByUserName(string userName)
        {

            DbParameter[] parameters = new DbParameter[]
                 {
                    new NpgsqlParameter(_userName, userName),
                 };
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_functionSysMenuGetByUserName, parameters);
            var list = ModelProvider.CreateListFromTable<MenuModel>(returnValue);

            return list;
        }


        /// <summary>
        /// Lấy danh sách tất cả menu
        /// </summary>
        /// <returns></returns>
        public List<MenuModel> GetAllMenu()
        {
            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_getMenuAll);
            var list = ModelProvider.CreateListFromTable<MenuModel>(returnValue);

            return list;
        }

        /// <summary>
        /// Lấy menu theo id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public MenuModel GetById(int? id)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(_menuId,id),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteStoreProcedureReturnQuery(_menu_getbyid, parameters);

            var list = ModelProvider.CreateListFromTable<MenuModel>(returnValue);
            MenuModel menu = list.FirstOrDefault()!;

            return menu;
        }

        /// <summary>
        /// Lưu menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int SaveMenu(MenuInputModel model)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(_menuId, model.MenuId),
                    new NpgsqlParameter(_name, model.NameMenu),
                    new NpgsqlParameter(_position, model.Position == null || model.Position == 0 ? DBNull.Value : model.Position),
                    new NpgsqlParameter(_parentId, model.ParentId == null || model.ParentId == 0 ? DBNull.Value : model.ParentId),
                    new NpgsqlParameter(_link,string.IsNullOrEmpty(model.Link) ? DBNull.Value : model.Link),
                    new NpgsqlParameter(_icon,string.IsNullOrEmpty(model.Icon) ? DBNull.Value : model.Icon),
                    new NpgsqlParameter(_functionActionId, model.FunctionActionId == null || model.FunctionActionId == 0 ? DBNull.Value : model.FunctionActionId),
                    new NpgsqlParameter(_isShow, model.IsShow),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_menuSave, parameters);

            return returnValue;
        }

        /// <summary>
        /// Xóa menu
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public int DeleteMenu(int menuId)
        {
            DbParameter[] parameters = new DbParameter[]
              {
                    new NpgsqlParameter(_menuId, menuId),
              };

            var returnValue = new ConnectionProcessor(_connectionString).ExcuteProcedureReturnValue(_menuDelete, parameters);

            return returnValue;
        }

    }
}
