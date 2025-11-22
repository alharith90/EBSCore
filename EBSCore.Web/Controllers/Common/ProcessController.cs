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
    public class ProcessController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBProcessSP ProcessSP;
        private readonly Common Common;
        private readonly User CurrentUser;
        private readonly ILogger<ProcessController> _logger;

        public ProcessController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor ,  ILogger<ProcessController> logger)
        {
            Configuration = configuration;
            ProcessSP = new DBProcessSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
             _logger = logger;

        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)ProcessSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvProcesses",
                    CompanyID: CurrentUser.CompanyID,
                    UserID: CurrentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Processes");
                return BadRequest("Error retrieving Processes");
            }
        }

        [HttpPost]
        public async Task<object> Save(Process Process)
        {
            try
            {
                if (Process.UnitID == null || Process.UnitID == "") 
                {
                    throw new Exception("Unit ID is required");
                }
                ProcessSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveProcess",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: Process.UnitID,
                    ProcessID: Process.ProcessID,
                    ProcessName: Process.ProcessName,
                    ProcessCode: Process.ProcessCode,
                    ProcessDescription: Process.ProcessDescription,
                    ExpiryDate: Process.ExpiryDate,
                    Status: Process.Status,
                    Priority: Process.Priority,
                    Frequency: Process.Frequency,
                    Notes: Process.Notes,
                    CreatedBy: CurrentUser.UserID,
                    ModifiedBy: CurrentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Process");
                return BadRequest("Error saving Process");
            }
        }

        [HttpGet]
        public object GetOne(long ProcessID)
        {
            try
            {
                DataSet DSResult = (DataSet)ProcessSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvProcess",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    ProcessID: ProcessID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Process");
                return BadRequest("Error retrieving Process");
            }
        }

        [HttpGet]
        public object GetByUnit(long? UnitID = null)
        {
            try
            {
                DataSet DSResult = (DataSet)ProcessSP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvProcessesByUnit",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: UnitID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Process");
                return BadRequest("Error retrieving Process");
            }
        }

        [HttpDelete]
        public object Delete(Process Process)
        {
            try
            {
                ProcessSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteProcess",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    ProcessID: Process.ProcessID);

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Process");
                return BadRequest("Error deleting Process");
            }
        }
    }
}
