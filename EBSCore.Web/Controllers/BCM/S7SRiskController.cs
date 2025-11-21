using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using EBSCore.AdoClass;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.BCM;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SRiskController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBS7SRisk_SP SP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public S7SRiskController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            SP = new DBS7SRisk_SP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get(string? PageNumber = "1", string? PageSize = "10", string? SearchQuery = "")
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvRiskList",
                    CurrentUserID: CurrentUser?.UserID,
                    PageNumber: PageNumber,
                    PageSize: PageSize,
                    SearchQuery: SearchQuery
                );

                var Result = new
                {
                    Data = DSResult.Tables[0],
                    PageCount = DSResult.Tables.Count > 1 ? DSResult.Tables[1].Rows[0]["PageCount"] : 1
                };

                return Ok(Result);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving risks");
            }
        }

        [HttpGet("{RiskID}")]
        public async Task<object> Get(int RiskID)
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvRisk",
                    CurrentUserID: CurrentUser?.UserID,
                    RiskID: RiskID.ToString()
                );

                return Ok(DSResult.Tables[0]);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving risk");
            }
        }

        [HttpPost]
        public async Task<object> Save(S7SRisk model)
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "SaveRisk",
                    CurrentUserID: CurrentUser?.UserID,
                    RiskID: model.RiskID?.ToString(),
                    BCMRiskAssessmentId: model.BCMRiskAssessmentId?.ToString(),
                    ImpactAspectID: model.ImpactAspectID?.ToString(),
                    ImpactTimeFrameID: model.ImpactTimeFrameID?.ToString(),
                    ImpactID: model.ImpactID.ToString(),
                    LikelihoodID: model.LikelihoodID.ToString(),
                    RiskCategoryID: model.RiskCategoryID?.ToString(),
                    RiskTitle: model.RiskTitle,
                    RiskDescription: model.RiskDescription,
                    MitigationPlan: model.MitigationPlan
                );

                return Ok(DSResult.Tables[0]);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving risk");
            }
        }

        [HttpDelete("{RiskID}")]
        public async Task<object> Delete(int RiskID)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteRisk",
                    CurrentUserID: CurrentUser?.UserID,
                    RiskID: RiskID.ToString()
                );
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting risk");
            }
        }

        [HttpGet]
        public async Task<object> Categories()
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvCategories",
                    CurrentUserID: CurrentUser?.UserID
                );

                return Ok(DSResult.Tables[0]);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving categories");
            }
        }

        [HttpPost]
        public async Task<object> SaveCategory(S7SRiskCategory model)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveCategory",
                    CurrentUserID: CurrentUser?.UserID,
                    RiskCategoryID: model.RiskCategoryID?.ToString(),
                    CategoryName: model.CategoryName,
                    CategoryDescription: model.Description
                );
                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving category");
            }
        }

        [HttpGet]
        public async Task<object> Likelihoods()
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvLikelihoods",
                    CurrentUserID: CurrentUser?.UserID
                );

                return Ok(DSResult.Tables[0]);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving likelihoods");
            }
        }

        [HttpPost]
        public async Task<object> SaveLikelihood(S7SRiskLikelihood model)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveLikelihood",
                    CurrentUserID: CurrentUser?.UserID,
                    LikelihoodID: model.LikelihoodID?.ToString(),
                    LikelihoodName: model.LikelihoodName,
                    LikelihoodValue: model.LikelihoodValue.ToString(),
                    LikelihoodDescription: model.Description
                );
                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving likelihood");
            }
        }

        [HttpGet]
        public async Task<object> MatrixConfig()
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvMatrixConfig",
                    CurrentUserID: CurrentUser?.UserID
                );

                return Ok(DSResult.Tables[0]);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving matrix configuration");
            }
        }

        [HttpPost]
        public async Task<object> SaveMatrixConfiguration(S7SRiskMatrixConfiguration model)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveMatrixConfig",
                    CurrentUserID: CurrentUser?.UserID,
                    RiskMatrixConfigID: model.RiskMatrixConfigID?.ToString(),
                    MatrixName: model.MatrixName,
                    MatrixSize: model.MatrixSize.ToString(),
                    IsDynamic: model.IsDynamic ? "1" : "0",
                    ConfigJson: model.ConfigJson
                );
                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving matrix configuration");
            }
        }

        [HttpGet]
        public async Task<object> Tolerance()
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvTolerance",
                    CurrentUserID: CurrentUser?.UserID
                );

                return Ok(DSResult.Tables[0]);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving risk tolerance");
            }
        }

        [HttpPost]
        public async Task<object> SaveTolerance(S7SRiskLevel model)
        {
            try
            {
                var result = SP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveTolerance",
                    CurrentUserID: CurrentUser?.UserID,
                    RiskToleranceID: model.RiskToleranceID?.ToString(),
                    RiskMatrixConfigID: model.RiskMatrixConfigID?.ToString(),
                    HighThreshold: model.HighThreshold.ToString(),
                    MediumThreshold: model.MediumThreshold.ToString(),
                    LowLabel: model.LowLabel,
                    MediumLabel: model.MediumLabel,
                    HighLabel: model.HighLabel
                );
                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving risk tolerance");
            }
        }

        [HttpGet]
        public async Task<object> Heatmap()
        {
            try
            {
                DataSet DSResult = (DataSet)SP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvHeatmap",
                    CurrentUserID: CurrentUser?.UserID
                );

                return Ok(DSResult);
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving heatmap data");
            }
        }
    }
}
