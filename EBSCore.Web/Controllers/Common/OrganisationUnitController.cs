using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EBSCore.AdoClass;
using EBSCore.Web.Models;
using System;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using EBSCore.AdoClass.Common;
using EBSCore.Web.AppCode;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Data;
using Newtonsoft.Json;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrganisationUnitController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DBOrganisationUnitSP _organisationUnitSP;
        ErrorHandler objErrorHandler;
        Common objCommon;
        User currentUser;
        public OrganisationUnitController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _organisationUnitSP = new DBOrganisationUnitSP(_configuration);
            objErrorHandler = new ErrorHandler(configuration);
            currentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            objCommon = new Common();
        }
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)_organisationUnitSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvOrganisationUnits",
                                 CompanyID: currentUser.CompanyID,
                                 UserID: currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                objCommon.LogError(ex, Request);
                return BadRequest($"Error retrieving Organisation Units");
            }
        }
        [HttpGet]
        public async Task<object> GetTree()
        {
            try
            {
                DataSet DSResult = (DataSet)_organisationUnitSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvOrganisationUnitsTree",
                                 CompanyID: currentUser.CompanyID,
                                 UserID: currentUser.UserID);

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                objCommon.LogError(ex, Request);
                return BadRequest($"Error retrieving Organisation Units");
            }
        }
        [HttpPost]
        public async Task<object> Save(SysOrganisationUnit organisationUnit)
        {
            try
            {
                _organisationUnitSP.QueryDatabase(SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveOrganisationUnit",
                    UserID: currentUser.UserID,
                    CompanyID: currentUser.CompanyID,
                    UnitID: organisationUnit.UnitID,
                    UnitCode: organisationUnit.UnitCode,
                    UnitName: organisationUnit.UnitName,
                    UnitTypeID: organisationUnit.UnitTypeID,
                    ColorCode: organisationUnit.ColorCode,
                    ParentUnitID: organisationUnit.ParentUnitID,
                    Description: organisationUnit.Description,
                    Status: organisationUnit.Status,
                    ExternalID: organisationUnit.ExternalID,
                    Location: organisationUnit.Location,
                    HierarchyLevel: organisationUnit.HierarchyLevel,
                    IsActive: organisationUnit.IsActive,
                    CreatedBy: currentUser.UserID,
                    ModifiedBy: currentUser.UserID
                    );

                return "[]";
            }
            catch (Exception ex)
            {
                objCommon.LogError(ex, Request);
                return BadRequest($"Error saving Organisation Unit");
            }
        }
        [HttpGet("{UnitID}")]
        public object GetOne(long UnitID)
        {
            try
            {
                DataSet DSResult = (DataSet) _organisationUnitSP.QueryDatabase(SqlQueryType.FillDataset, Operation: "rtvOrganisationUnit", 
                    UserID:currentUser.UserID , 
                    CompanyID:currentUser.CompanyID,
                    UnitID: UnitID.ToString()
                    );
                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                objCommon.LogError(ex, Request);
                return BadRequest($"Error retrieving Organisation Unit");
            }
        }
        [HttpDelete("{UnitID}")]
        public object Delete(int UnitID)
        {
            try
            {
                _organisationUnitSP.QueryDatabase(SqlQueryType.ExecuteNonQuery, Operation: "DeleteOrganisationUnit", 
                    UserID :currentUser.UserID ,
                    CompanyID :currentUser.CompanyID,
                    UnitID: UnitID.ToString());
                return "[]";
            }
            catch (Exception ex)
            {
                objCommon.LogError(ex, Request);
                return BadRequest($"Error deleting Organisation Unit");
            }
        }
    }
}