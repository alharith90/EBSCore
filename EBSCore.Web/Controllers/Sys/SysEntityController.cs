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
    public class SysEntityController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBSysEntitySP SysEntitySP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public SysEntityController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            SysEntitySP = new DBSysEntitySP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> GetCurrent()
        {
            try
            {
                DataSet DSResult = (DataSet)SysEntitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvSysEntity",
                    UserID: CurrentUser.UserID);
                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving entities");
            }
        }

        [HttpGet]
        public object GetOne(string EntityID)
        {
            try
            {
                DataSet DSResult = (DataSet)SysEntitySP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvSysEntity",
                    UserID: CurrentUser.UserID,
                    EntityID: EntityID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving entity");
            }
        }

        [HttpPost]
        public async Task<object> Save(SysEntity model)
        {
            try
            {
                SysEntitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveSysEntity",
                    UserID: CurrentUser.UserID,
                    EntityID: model.EntityID?.ToString(),
                    Name: model.Name,
                    Sector: model.Sector,
                    Description: model.Description,
                    Location: model.Location,
                    ContactPerson: model.ContactPerson,
                    Email: model.Email,
                    Phone: model.Phone,
                    CreatedBy: CurrentUser.UserID,
                    UpdatedBy: CurrentUser.UserID);

                return Ok("[]");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving entity");
            }
        }

        [HttpDelete]
        public object Delete(SysEntity model)
        {
            try
            {
                SysEntitySP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteSysEntity",
                    UserID: CurrentUser.UserID,
                    EntityID: model.EntityID?.ToString());

                return Ok("[]");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting entity");
            }
        }
    }
}
