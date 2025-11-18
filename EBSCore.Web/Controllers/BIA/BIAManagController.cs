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
        public async Task<object> Save(BIA BIA)
        {
            try
            {
                if (BIA.UnitID == null || BIA.UnitID == "")
                {
                    throw new Exception("Unit ID is required");
                }

                BIASP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveBIA",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    BIAID: BIA.BIAID,
                    BIACode: BIA.BIACode,
                    UnitID: BIA.UnitID,
                    ProcessID: BIA.ProcessID,
                    ProcessName: BIA.ProcessName,
                    ProcessDescription: BIA.ProcessDescription,
                    Frequency: BIA.Frequency,
                    Criticality: BIA.Criticality,
                    RTO: BIA.RTO,
                    RPO: BIA.RPO,
                    MAO: BIA.MAO,
                    MTPD: BIA.MTPD,
                    MTD: BIA.MTD,
                    MBCO: BIA.MBCO,
                    PrimaryStaff: BIA.PrimaryStaff,
                    BackupStaff: BIA.BackupStaff,
                    RTOJustification: BIA.RTOJustification,
                    MBCODetails: BIA.MBCODetails,
                    Priority: BIA.Priority,
                    RequiredCompetencies: BIA.RequiredCompetencies,
                    RevenueLossPerHour: BIA.RevenueLossPerHour,
                    CostOfDowntime: BIA.CostOfDowntime,
                    Remarks: BIA.Remarks,
                    LastComment: BIA.LastComment,
                    ReviewDate: BIA.ReviewDate,
                    WorkFlowStatus: BIA.WorkFlowStatus,
                    IsDeleted: BIA.IsDeleted,
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
