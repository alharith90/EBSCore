using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.BCM;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace EBSCore.Web.Controllers.BCM
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SGovernanceController : ControllerBase
    {
        private readonly DBS7SGovernance_SP _governanceSp;
        private readonly Common _common;
        private readonly User? _currentUser;

        public S7SGovernanceController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _governanceSp = new DBS7SGovernance_SP(configuration);
            _common = new Common();
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
        }

        [HttpGet]
        [Authorize(Policy = "ManageCommittee")]
        public object Overview()
        {
            try
            {
                var snapshot = LoadSnapshot();
                return Ok(snapshot);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Unable to load governance snapshot");
            }
        }

        [HttpGet]
        [Authorize(Policy = "ManageRoles")]
        public object GapAnalysis()
        {
            try
            {
                var snapshot = LoadSnapshot();
                var results = snapshot.GetGapAnalysis().ToList();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Unable to calculate gap analysis");
            }
        }

        [HttpPost]
        [Authorize(Policy = "AssignMembers")]
        public object SaveRoleCompetency([FromBody] S7SCompetency competency)
        {
            try
            {
                _governanceSp.QueryDatabase(
                    DBParentStoredProcedureClass.SqlQueryType.NonQuery,
                    Operation: "upsertCompetency",
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    RoleID: competency.RoleID?.ToString(),
                    CompetencyID: competency.CompetencyID.ToString(),
                    SkillRequired: competency.SkillRequired,
                    RequiredLevel: competency.RequiredLevel,
                    AssessedLevel: competency.AssessedLevel,
                    Notes: competency.GapAnalysis
                );

                return Ok(new { Message = "Role competency saved" });
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error saving competency");
            }
        }

        private S7SGovernanceSnapshot LoadSnapshot()
        {
            var snapshot = new S7SGovernanceSnapshot();
            var dataSet = (DataSet)_governanceSp.QueryDatabase(
                DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                Operation: "rtvGovernance",
                UserID: _currentUser?.UserID,
                CompanyID: _currentUser?.CompanyID
            );

            if (_common.IsEmptyDataSet(dataSet))
            {
                return snapshot;
            }

            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    snapshot.Committees.Add(new S7SCommittee
                    {
                        CommitteeID = Convert.ToInt32(row["CommitteeID"]),
                        Name = row["Name"].ToString(),
                        Charter = row["Charter"].ToString(),
                        Chairperson = row["Chairperson"].ToString(),
                        Status = row["Status"].ToString()
                    });
                }
            }

            if (dataSet.Tables.Count > 1)
            {
                foreach (DataRow row in dataSet.Tables[1].Rows)
                {
                    var role = new S7SRole
                    {
                        RoleID = Convert.ToInt32(row["RoleID"]),
                        RoleName = row["RoleName"].ToString(),
                        Responsibilities = row["Responsibilities"].ToString(),
                        AssignedTo = row["AssignedTo"].ToString()
                    };
                    snapshot.Roles.Add(role);
                }
            }

            if (dataSet.Tables.Count > 2)
            {
                foreach (DataRow row in dataSet.Tables[2].Rows)
                {
                    var competency = new S7SCompetency
                    {
                        CompetencyID = Convert.ToInt32(row["CompetencyID"]),
                        RoleID = row["RoleID"] == DBNull.Value ? null : Convert.ToInt32(row["RoleID"]),
                        SkillRequired = row["SkillRequired"].ToString(),
                        RequiredLevel = row["RequiredLevel"].ToString(),
                        AssessedLevel = row["AssessedLevel"].ToString(),
                        GapAnalysis = S7SGapCalculator.DescribeGap(row["RequiredLevel"].ToString(), row["AssessedLevel"].ToString()),
                        TrainingRequired = row["TrainingRequired"] != DBNull.Value && Convert.ToBoolean(row["TrainingRequired"])
                    };

                    snapshot.Competencies.Add(competency);

                    var role = snapshot.Roles.FirstOrDefault(r => r.RoleID == competency.RoleID);
                    role?.Competencies.Add(competency);
                }
            }

            if (dataSet.Tables.Count > 3)
            {
                foreach (DataRow row in dataSet.Tables[3].Rows)
                {
                    var link = new S7SCommitteePlanLink
                    {
                        CommitteeID = Convert.ToInt32(row["CommitteeID"]),
                        PlanID = Convert.ToInt32(row["PlanID"]),
                        PlanName = row["PlanName"].ToString(),
                        LinkageType = row["LinkageType"].ToString()
                    };

                    var committee = snapshot.Committees.FirstOrDefault(c => c.CommitteeID == link.CommitteeID);
                    committee?.Plans.Add(link);
                }
            }

            if (dataSet.Tables.Count > 4)
            {
                foreach (DataRow row in dataSet.Tables[4].Rows)
                {
                    var training = new S7STrainingRequirement
                    {
                        TrainingRequirementID = Convert.ToInt32(row["TrainingRequirementID"]),
                        RoleID = row["RoleID"] == DBNull.Value ? null : Convert.ToInt32(row["RoleID"]),
                        CompetencyID = row["CompetencyID"] == DBNull.Value ? null : Convert.ToInt32(row["CompetencyID"]),
                        TrainingCourse = row["TrainingCourse"].ToString(),
                        TargetDate = row["TargetDate"] == DBNull.Value ? null : DateTime.Parse(row["TargetDate"].ToString()),
                        CompletionDate = row["CompletionDate"] == DBNull.Value ? null : DateTime.Parse(row["CompletionDate"].ToString()),
                        Notes = row["Notes"].ToString()
                    };

                    var role = snapshot.Roles.FirstOrDefault(r => r.RoleID == training.RoleID);
                    role?.TrainingRequirements.Add(training);
                }
            }

            return snapshot;
        }
    }
}
