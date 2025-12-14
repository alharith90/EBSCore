using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using EBSCore.AdoClass;
using EBSCore.Web.Models;
using System;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using EBSCore.Web.AppCode;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.Web.Controllers.BCM
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SupplierController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DBSupplierSP SupplierSP;
        private readonly User CurrentUser;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<SupplierController> logger)
        {
            Configuration = configuration;
            SupplierSP = new DBSupplierSP(Configuration);
            CurrentUser = httpContextAccessor.HttpContext.Session.GetObject<User>("User");
            _logger = logger;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                DataSet DSResult = (DataSet)SupplierSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvSuppliers",
                    CompanyID: CurrentUser.CompanyID,
                    UserID: CurrentUser.UserID
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Suppliers");
                return BadRequest("Error retrieving Suppliers");
            }
        }

        [HttpGet]
        public object GetOne(long SupplierID)
        {
            try
            {
                DataSet DSResult = (DataSet)SupplierSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvSupplier",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    SupplierID: SupplierID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Supplier");
                return BadRequest("Error retrieving Supplier");
            }
        }

        [HttpGet]
        public object GetByUnit(long? UnitID = null)
        {
            try
            {
                DataSet DSResult = (DataSet)SupplierSP.QueryDatabase(
                    SqlQueryType.FillDataset,
                    Operation: "rtvSuppliersByUnit",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: UnitID.ToString()
                );

                return Ok(JsonConvert.SerializeObject(DSResult.Tables[0]));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Supplier list");
                return BadRequest("Error retrieving Supplier list");
            }
        }

        [HttpPost]
        public async Task<object> Save(Supplier supplier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(supplier.UnitID))
                {
                    throw new Exception("Unit ID is required");
                }

                SupplierSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "SaveSupplier",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    UnitID: supplier.UnitID,
                    SupplierID: supplier.SupplierID,
                    SupplierType: supplier.SupplierType,
                    SupplierName: supplier.SupplierName,
                    Services: supplier.Services,
                    MainContactName: supplier.MainContactName,
                    MainContactEmail: supplier.MainContactEmail,
                    MainContactPhone: supplier.MainContactPhone,
                    SecondaryContactName: supplier.SecondaryContactName,
                    SecondaryContactEmail: supplier.SecondaryContactEmail,
                    SecondaryContactPhone: supplier.SecondaryContactPhone,
                    SLAInPlace: supplier.SLAInPlace?.ToString(),
                    RTOHours: supplier.RTOHours,
                    RPOHours: supplier.RPOHours,
                    Notes: supplier.Notes,
                    CreatedBy: CurrentUser.UserID,
                    ModifiedBy: CurrentUser.UserID
                );

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Supplier");
                return BadRequest("Error saving Supplier");
            }
        }

        [HttpDelete]
        public object Delete(Supplier supplier)
        {
            try
            {
                SupplierSP.QueryDatabase(
                    SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteSupplier",
                    UserID: CurrentUser.UserID,
                    CompanyID: CurrentUser.CompanyID,
                    SupplierID: supplier.SupplierID
                );

                return "[]";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Supplier");
                return BadRequest("Error deleting Supplier");
            }
        }
    }
}
