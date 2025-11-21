using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EBSCore.AdoClass;
using EBSCore.Web.Models;
using EBSCore.Web.Models.BIA;
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
    public class BIAManagController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBBIASP BIASP;
        private readonly Common Common;
        private readonly User CurrentUser;

        public BIAManagController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            BIASP = new DBBIASP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            Common = new Common();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)BIASP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RetrieveAll",
                    CompanyID: CurrentUser.CompanyID,
                    UserID: CurrentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving BIAs");
            }
        }

        [HttpPost]
        public async Task<object> Save(BIA bia)
        {
            try
            {
                if (bia.UnitID == null || bia.UnitID == "")
                {
                    throw new Exception("Unit ID is required");
                }

                BIASP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveBIA",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
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
                    MAO: bia.MAO,
                    MTPD: bia.MTPD,
                    MTD: bia.MTD,
                    MBCO: bia.MBCO,
                    PrimaryStaff: bia.PrimaryStaff,
                    BackupStaff: bia.BackupStaff,
                    RTOJustification: bia.RTOJustification,
                    MBCODetails: bia.MBCODetails,
                    Priority: bia.Priority,
                    RequiredCompetencies: bia.RequiredCompetencies,
                    RevenueLossPerHour: bia.RevenueLossPerHour,
                    CostOfDowntime: bia.CostOfDowntime,
                    Remarks: bia.Remarks,
                    LastComment: bia.LastComment,
                    ReviewDate: bia.ReviewDate,
                    WorkFlowStatus: bia.WorkFlowStatus,
                    IsDeleted: bia.IsDeleted,
                    CreatedBy: CurrentUser.UserID,
                    UpdatedBy: CurrentUser.UserID);

                return "[]";
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error saving BIA");
            }
        }

        [HttpGet]
        public object GetOne(long BIAID)
        {
            try
            {
                DataSet DSResult = (DataSet)BIASP.QueryDatabase(SqlQueryType.FillDataset,
                    Operation: "RetrieveOne",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    BIAID: BIAID.ToString());

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error retrieving BIA");
            }
        }

        [HttpDelete]
        public object Delete(BIA BIA)
        {
            try
            {
                BIASP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteBIA",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    BIAID: BIA.BIAID);

                return "[]";
            }
            catch (Exception ex)
            {
                Common.LogError(ex, Request);
                return BadRequest("Error deleting BIA");
            }
        }
    }
}
