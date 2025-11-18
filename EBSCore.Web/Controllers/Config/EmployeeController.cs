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
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBEmployeeSP EmployeeSP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public EmployeeController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            EmployeeSP = new DBEmployeeSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get(string? PageNumber = "1", string? PageSize = "10", string? SortColumn = "", string? SortDirection = "", string? SearchFields = "", string? SearchQuery = "")
        {
            try
            {
                DataSet DSResult = (DataSet)EmployeeSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvEmployees",
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
                return BadRequest("Error retrieving employees");
            }
        }

        [HttpGet("{EmployeeId}")]
        public async Task<object> Get(int EmployeeId)
        {
            try
            {
                DataSet DSResult = (DataSet)EmployeeSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvEmployee",
                    CurrentUserID: CurrentUser?.UserID,
                    EmployeeId: EmployeeId.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving employee");
            }
        }

        [HttpPost]
        public async Task<object> Save(Employee employee)
        {
            try
            {
                object Result = EmployeeSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Save",
                    CurrentUserID: CurrentUser?.UserID,
                    EmployeeId: employee.EmployeeId?.ToString(),
                    FullName: employee.FullName,
                    Position: employee.Position,
                    Email: employee.Email,
                    Phone: employee.Phone,
                    OrganizationUnitId: employee.OrganizationUnitId?.ToString(),
                    SourceId: employee.SourceId,
                    SourceSystem: employee.SourceSystem,
                    JobTitle: employee.JobTitle,
                    JobFamily: employee.JobFamily,
                    SupervisorId: employee.SupervisorId?.ToString(),
                    EmploymentType: employee.EmploymentType,
                    Location: employee.Location,
                    Department: employee.Department,
                    IsActive: employee.IsActive.ToString()
                );

                return Ok("Saved successfully");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving employee");
            }
        }

        [HttpDelete("{EmployeeId}")]
        public async Task<object> Delete(int EmployeeId)
        {
            try
            {
                object Result = EmployeeSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "Delete",
                    CurrentUserID: CurrentUser?.UserID,
                    EmployeeId: EmployeeId.ToString()
                );

                return Ok("Deleted Successfully!");
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting employee");
            }
        }
    }
}

