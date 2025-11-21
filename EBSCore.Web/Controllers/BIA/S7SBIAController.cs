using EBSCore.AdoClass;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.BIA;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Threading.Tasks;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.BIA
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SBIAController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBS7SBIA_SP _sp;
        private readonly Common _common;
        private readonly User _currentUser;

        public S7SBIAController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _sp = new DBS7SBIA_SP(configuration);
            _common = new Common();
            _currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
        }

        [HttpGet]
        public async Task<object> GetList()
        {
            try
            {
                var ds = (DataSet)_sp.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvList",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving BIAs");
            }
        }

        [HttpGet]
        public async Task<object> GetOne(long biaId)
        {
            try
            {
                var ds = (DataSet)_sp.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "rtvItem",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    BIAID: biaId.ToString());

                return Ok(JsonConvert.SerializeObject(ds.Tables[0]));
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving BIA");
            }
        }

        [HttpPost]
        public async Task<object> Save(S7SBIA bia)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bia.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                _sp.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "Save",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    BIAID: bia.BIAID,
                    BIACode: bia.BIACode,
                    UnitID: bia.UnitID,
                    ProcessID: bia.ProcessID,
                    ProcessName: bia.ProcessName,
                    ProcessDescription: bia.ProcessDescription,
                    Frequency: bia.Frequency,
                    Criticality: bia.Criticality,
                    RTO: bia.RTO,
                    RPO: bia.RPO,
                    MTPD: bia.MTPD,
                    MAO: bia.MAO,
                    MBCO: bia.MBCO,
                    Priority: bia.Priority,
                    RequiredCompetencies: bia.RequiredCompetencies,
                    AlternativeWorkLocation: bia.AlternativeWorkLocation,
                    RegulatoryRequirements: bia.RegulatoryRequirements,
                    PrimaryStaff: bia.PrimaryStaff,
                    BackupStaff: bia.BackupStaff,
                    RTOJustification: bia.RTOJustification,
                    MBCODetails: bia.MBCODetails,
                    RevenueLossPerHour: bia.RevenueLossPerHour,
                    CostOfDowntime: bia.CostOfDowntime,
                    Remarks: bia.Remarks,
                    LastComment: bia.LastComment,
                    ReviewDate: bia.ReviewDate,
                    WorkFlowStatus: bia.WorkFlowStatus,
                    IsDeleted: bia.IsDeleted);

                _common.LogInfo($"BIA saved: {bia.BIACode}", Request?.Path.Value ?? "BIA save");
                return "[]";
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error saving BIA");
            }
        }

        [HttpDelete]
        public async Task<object> Delete(long biaId)
        {
            try
            {
                _sp.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "Delete",
                    CompanyID: _currentUser.CompanyID,
                    UserID: _currentUser.UserID,
                    BIAID: biaId.ToString());

                _common.LogInfo($"BIA deleted: {biaId}", Request?.Path.Value ?? "BIA delete");
                return "[]";
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error deleting BIA");
            }
        }
    }
}
