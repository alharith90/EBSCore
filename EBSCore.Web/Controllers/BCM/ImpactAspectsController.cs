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
    public class ImpactAspectsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBImpactAspectSP SP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public ImpactAspectsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            SP = new DBImpactAspectSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get(string? PageNumber = "1", string? PageSize = "10", string? SortColumn = "", string? SortDirection = "", string? SearchFields = "", string? SearchQuery = "")
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvImpactAspects",
                    CurrentUserID: CurrentUser?.UserID,
                    PageNumber: PageNumber,
                    PageSize: PageSize,
                    SearchFields: SearchFields,
                    SearchQuery: SearchQuery,
                    SortColumn: SortColumn,
                    SortDirection: SortDirection
                );

                var Result = new
                {
                    Data = DSResult.Tables[0],
                    PageCount = DSResult.Tables.Count > 1 ? DSResult.Tables[1].Rows[0]["PageCount"] : 1
                };

                return Ok(JsonConvert.SerializeObject(Result));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving ImpactAspects");
            }
        }

        [HttpGet("{ImpactAspectID}")]
        public async Task<object> Get(int ImpactAspectID)
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvImpactAspect",
                    CurrentUserID: CurrentUser?.UserID,
                    ImpactAspectID: ImpactAspectID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving ImpactAspect");
            }
        }

        [HttpPost]
        public async Task<object> Save(ImpactAspect m)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Save",
                    CurrentUserID: CurrentUser?.UserID,
                    ImpactAspectID: m.ImpactAspectID?.ToString(),
                    AspectName: m.AspectName,
                    Description: m.Description
                );
                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving ImpactAspect");
            }
        }

        [HttpDelete("{ImpactAspectID}")]
        public async Task<object> Delete(int ImpactAspectID)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Delete",
                    CurrentUserID: CurrentUser?.UserID,
                    ImpactAspectID: ImpactAspectID.ToString()
                );
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting ImpactAspect");
            }
        }
    }
}
