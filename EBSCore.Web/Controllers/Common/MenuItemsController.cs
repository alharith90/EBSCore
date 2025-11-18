using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EBSCore.AdoClass;
using EBSCore.Web.Models;
using System;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MenuItemsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBMenuItemsSP MenuItemSP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public MenuItemsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            MenuItemSP = new DBMenuItemsSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)MenuItemSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvMenuItems",
                    UserID: CurrentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving MenuItemes");
            }
        }

        [HttpGet]
        public async Task<object> GetTree()
        {
            try
            {
                DataSet DSResult = (DataSet)MenuItemSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvMenuItemsTree",
                    UserID: CurrentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving MenuItemes");
            }
        }

        [HttpGet]
        public object GetOne(long MenuItemID)
        {
            try
            {
                DataSet DSResult = (DataSet)MenuItemSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvMenuItem",
                    UserID: CurrentUser.UserID,
                    MenuItemID: MenuItemID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving MenuItem");
            }
        }

        [HttpPost]
        public async Task<object> Save(MenuItem MenuItem)
        {
            try
            {

                MenuItemSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "Save",
                    MenuItemID: MenuItem.MenuItemID,
                    ParentID: MenuItem.ParentID,
                    LabelAR: MenuItem.LabelAR,
                    LabelEN: MenuItem.LabelEN,
                    DescriptionAR: MenuItem.DescriptionAR,
                    DescriptionEn: MenuItem.DescriptionEn,
                    Url: MenuItem.Url,
                    Icon: MenuItem.Icon,
                    Order: MenuItem.Order,
                    IsActive: MenuItem.IsActive,
                    Permission: MenuItem.Permission,
                    Type: MenuItem.Type,
                    CreatedBy: CurrentUser.UserID,
                    UpdatedBy: CurrentUser.UserID,
                    CreatedAt: MenuItem.CreatedAt,
                    UpdatedAt: MenuItem.UpdatedAt);


                return "[]";
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving MenuItem");
            }
        }



        //[HttpGet]
        //public object GetByUnit(long? UnitID = null)
        //{
        //    try
        //    {
        //        DataSet DSResult = (DataSet)MenuItemSP.QueryDatabase(SqlQueryType.FillDataset,
        //            Operation: "rtvMenuItemesByUnit",
        //            UserID: CurrentUser.UserID,
        //            UnitID: UnitID.ToString());

        //        return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.LogError(ex, Request);
        //        return BadRequest("Error retrieving MenuItem");
        //    }
        //}

        [HttpDelete]
        public object Delete(MenuItem MenuItem)
        {
            try
            {
                MenuItemSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteMenuItem",
                    UserID: CurrentUser.UserID,
                    MenuItemID: MenuItem.MenuItemID);

                return "[]";
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting MenuItem");
            }
        }
    }
}
