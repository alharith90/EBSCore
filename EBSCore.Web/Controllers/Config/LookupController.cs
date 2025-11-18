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
    public class LookupController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBLookupSP LookupSP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public LookupController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            LookupSP = new DBLookupSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)LookupSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvLookups",
                    UserID: CurrentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving Lookups");
            }
        }

        [HttpPost]
        public async Task<object> Save(Lookup Lookup)
        {
            try
            {
                if (string.IsNullOrEmpty(Lookup.LookupType) || string.IsNullOrEmpty(Lookup.LookupDescriptionEn))
                {
                    throw new Exception("Lookup Type and Description (English) are required");
                }

                LookupSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveLookup",
                    UserID: CurrentUser.UserID,
                    LookupID: Lookup.LookupID,
                    LookupType: Lookup.LookupType,
                    LookupDescriptionAr: Lookup.LookupDescriptionAr,
                    LookupDescriptionEn: Lookup.LookupDescriptionEn,
                    ParentID: Lookup.ParentID,
                    Level: Lookup.Level,
                    Status: Lookup.Status.ToString(),
                    CreatedBy: CurrentUser.UserID,
                    UpdatedBy: CurrentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving Lookup");
            }
        }

        [HttpGet]
        public object GetOne(long LookupID)
        {
            try
            {
                DataSet DSResult = (DataSet)LookupSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvLookup",
                    UserID: CurrentUser.UserID,
                    LookupID: LookupID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving Lookup");
            }
        }

        [HttpDelete]
        public object Delete(Lookup Lookup)
        {
            try
            {
                LookupSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteLookup",
                    UserID: CurrentUser.UserID,
                    LookupID: Lookup.LookupID);

                return "[]";
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting Lookup");
            }
        }
    }
}
