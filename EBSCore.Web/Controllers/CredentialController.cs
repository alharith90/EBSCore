using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models;
using EBSCore.Web.Models.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CredentialController : ControllerBase
    {
        private readonly DBWorkflowCredentialSP _credentialSP;
        private readonly Common _common;
        private readonly User? _currentUser;

        public CredentialController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _credentialSP = new DBWorkflowCredentialSP(configuration);
            _common = new Common();
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var ds = (DataSet)_credentialSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "rtvCredentials",
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID
                );

                var list = new List<WorkflowCredentialModel>();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new WorkflowCredentialModel
                        {
                            CredentialID = Convert.ToInt32(row["CredentialID"]),
                            CredentialName = row["Name"].ToString(),
                            CredentialType = row["CredentialType"].ToString(),
                            DataJson = row["DataJson"].ToString(),
                            Notes = row["Notes"].ToString()
                        });
                    }
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving credentials");
            }
        }

        [HttpPost]
        public async Task<object> Save([FromBody] WorkflowCredentialModel credential)
        {
            try
            {
                if (_currentUser == null || _currentUser.UserType != UserType.Manager)
                {
                    return Unauthorized();
                }

                var credentialId = _credentialSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar,
                    Operation: "SaveCredential",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    CredentialID: credential.CredentialID?.ToString(),
                    CredentialName: credential.CredentialName,
                    CredentialType: credential.CredentialType,
                    DataJson: credential.DataJson,
                    Notes: credential.Notes
                );

                return Ok(new { CredentialID = credentialId });
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error saving credential");
            }
        }

        [HttpDelete("{credentialId}")]
        public async Task<object> Delete(int credentialId)
        {
            try
            {
                if (_currentUser == null || _currentUser.UserType != UserType.Manager)
                {
                    return Unauthorized();
                }

                _credentialSP.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.ExecuteNonQuery,
                    Operation: "DeleteCredential",
                    UserID: _currentUser.UserID,
                    CompanyID: _currentUser.CompanyID,
                    CredentialID: credentialId.ToString()
                );

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error deleting credential");
            }
        }
    }
}
