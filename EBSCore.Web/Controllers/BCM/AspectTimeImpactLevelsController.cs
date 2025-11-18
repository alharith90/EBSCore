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
    public class AspectTimeImpactLevelsController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBAspectTimeImpactLevelSP SP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public AspectTimeImpactLevelsController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            SP = new DBAspectTimeImpactLevelSP(Configuration);
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
                    Operation: "rtvAspectTimeImpactLevels",
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
                return BadRequest("Error retrieving AspectTimeImpactLevels");
            }
        }

        [HttpGet("{AspectTimeImpactLevelID}")]
        public async Task<object> Get(int AspectTimeImpactLevelID)
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvAspectTimeImpactLevel",
                    CurrentUserID: CurrentUser?.UserID,
                    AspectTimeImpactLevelID: AspectTimeImpactLevelID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving AspectTimeImpactLevel");
            }
        }

        [HttpPost]
        public async Task<object> Save(AspectTimeImpactLevel m)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Save",
                    CurrentUserID: CurrentUser?.UserID,
                    AspectTimeImpactLevelID: m.AspectTimeImpactLevelID?.ToString(),
                    ImpactAspectID: m.ImpactAspectID?.ToString(),
                    ImpactTimeFrameID: m.ImpactTimeFrameID?.ToString(),
                    LevelID: m.LevelID?.ToString(),
                    ImpactLevel: m.ImpactLevel,
                    Justification: m.Justification,
                    ImpactColor: m.ImpactColor
                );
                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving AspectTimeImpactLevel");
            }
        }

        [HttpDelete("{AspectTimeImpactLevelID}")]
        public async Task<object> Delete(int AspectTimeImpactLevelID)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Delete",
                    CurrentUserID: CurrentUser?.UserID,
                    AspectTimeImpactLevelID: AspectTimeImpactLevelID.ToString()
                );
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting AspectTimeImpactLevel");
            }
        }
    }
}
