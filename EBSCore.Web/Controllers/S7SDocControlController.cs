using EBSCore.AdoClass;
using EBSCore.Web.AppCode;
using EBSCore.Web.Models.Document;
using EBSCore.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace EBSCore.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class S7SDocControlController : ControllerBase
    {
        private readonly DBS7SDocControl_SP _docControl;
        private readonly Common _common;
        private readonly User? _currentUser;

        public S7SDocControlController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _docControl = new DBS7SDocControl_SP(configuration);
            _common = new Common();
            _currentUser = httpContextAccessor.HttpContext?.Session.GetObject<User>("User");
        }

        [HttpGet]
        public object GetLibrary()
        {
            try
            {
                var dataSet = (DataSet)_docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset, Operation: "GetLibrary", UserID: _currentUser?.UserID, CompanyID: _currentUser?.CompanyID);
                var list = new List<S7SDocument>();

                if (!_common.IsEmptyDataSet(dataSet))
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        list.Add(new S7SDocument
                        {
                            DocumentID = Convert.ToInt32(row[nameof(S7SDocument.DocumentID)]),
                            Title = row[nameof(S7SDocument.Title)].ToString(),
                            ReferenceCode = row[nameof(S7SDocument.ReferenceCode)].ToString(),
                            Owner = row[nameof(S7SDocument.Owner)].ToString(),
                            VersionNumber = row[nameof(S7SDocument.VersionNumber)].ToString(),
                            NextReviewDate = row[nameof(S7SDocument.NextReviewDate)] == DBNull.Value ? null : Convert.ToDateTime(row[nameof(S7SDocument.NextReviewDate)]),
                            Status = row[nameof(S7SDocument.Status)].ToString(),
                            WorkflowStatusID = row["WorkflowStatusID"] == DBNull.Value ? null : Convert.ToInt32(row["WorkflowStatusID"]),
                            CurrentVersionID = row["DocumentVersionID"] == DBNull.Value ? null : Convert.ToInt32(row["DocumentVersionID"])
                        });
                    }
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving documents");
            }
        }

        [HttpGet("{documentId}")]
        public object GetHistory(int documentId)
        {
            try
            {
                var dataSet = (DataSet)_docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset, Operation: "GetHistory", DocumentID: documentId.ToString(), UserID: _currentUser?.UserID, CompanyID: _currentUser?.CompanyID);
                var model = new S7SDocument { DocumentID = documentId };

                if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    var header = dataSet.Tables[0].Rows[0];
                    model.Title = header[nameof(S7SDocument.Title)].ToString();
                    model.ReferenceCode = header[nameof(S7SDocument.ReferenceCode)].ToString();
                    model.Owner = header[nameof(S7SDocument.Owner)].ToString();
                    model.VersionNumber = header[nameof(S7SDocument.VersionNumber)].ToString();
                    model.NextReviewDate = header[nameof(S7SDocument.NextReviewDate)] == DBNull.Value ? null : Convert.ToDateTime(header[nameof(S7SDocument.NextReviewDate)]);
                    model.Status = header[nameof(S7SDocument.Status)].ToString();
                }

                if (dataSet.Tables.Count > 1)
                {
                    foreach (DataRow row in dataSet.Tables[1].Rows)
                    {
                        model.Versions.Add(new S7SDocumentVersion
                        {
                            DocumentVersionID = Convert.ToInt32(row[nameof(S7SDocumentVersion.DocumentVersionID)]),
                            DocumentID = Convert.ToInt32(row[nameof(S7SDocumentVersion.DocumentID)]),
                            VersionNumber = row[nameof(S7SDocumentVersion.VersionNumber)].ToString(),
                            WorkflowStatusID = row["WorkflowStatusID"] == DBNull.Value ? null : Convert.ToInt32(row["WorkflowStatusID"]),
                            ChangeSummary = row[nameof(S7SDocumentVersion.ChangeSummary)].ToString(),
                            ContentUri = row[nameof(S7SDocumentVersion.ContentUri)].ToString(),
                            IssueDate = row[nameof(S7SDocumentVersion.IssueDate)] == DBNull.Value ? null : Convert.ToDateTime(row[nameof(S7SDocumentVersion.IssueDate)]),
                            NextReviewDate = row[nameof(S7SDocumentVersion.NextReviewDate)] == DBNull.Value ? null : Convert.ToDateTime(row[nameof(S7SDocumentVersion.NextReviewDate)]),
                            ApprovedBy = row[nameof(S7SDocumentVersion.ApprovedBy)].ToString(),
                            CreatedBy = row[nameof(S7SDocumentVersion.CreatedBy)].ToString(),
                            CreatedAt = row[nameof(S7SDocumentVersion.CreatedAt)] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[nameof(S7SDocumentVersion.CreatedAt)])
                        });
                    }
                }

                if (dataSet.Tables.Count > 2)
                {
                    foreach (DataRow row in dataSet.Tables[2].Rows)
                    {
                        model.Reviews.Add(new S7SReview
                        {
                            ReviewID = Convert.ToInt32(row[nameof(S7SReview.ReviewID)]),
                            DocumentID = Convert.ToInt32(row[nameof(S7SReview.DocumentID)]),
                            ReviewedBy = row[nameof(S7SReview.ReviewedBy)].ToString(),
                            ReviewDate = row[nameof(S7SReview.ReviewDate)] == DBNull.Value ? null : Convert.ToDateTime(row[nameof(S7SReview.ReviewDate)]),
                            ReviewComments = row[nameof(S7SReview.ReviewComments)].ToString()
                        });
                    }
                }

                if (dataSet.Tables.Count > 3)
                {
                    foreach (DataRow row in dataSet.Tables[3].Rows)
                    {
                        model.Approvals.Add(new S7SApproval
                        {
                            ApprovalID = Convert.ToInt32(row[nameof(S7SApproval.ApprovalID)]),
                            DocumentID = Convert.ToInt32(row[nameof(S7SApproval.DocumentID)]),
                            ApprovedBy = row[nameof(S7SApproval.ApprovedBy)].ToString(),
                            ApprovalDate = row[nameof(S7SApproval.ApprovalDate)] == DBNull.Value ? null : Convert.ToDateTime(row[nameof(S7SApproval.ApprovalDate)]),
                            ApprovalComments = row[nameof(S7SApproval.ApprovalComments)].ToString()
                        });
                    }
                }

                if (dataSet.Tables.Count > 4)
                {
                    foreach (DataRow row in dataSet.Tables[4].Rows)
                    {
                        model.Changes.Add(new S7SDocumentChange
                        {
                            ChangeID = Convert.ToInt32(row[nameof(S7SDocumentChange.ChangeID)]),
                            DocumentID = Convert.ToInt32(row[nameof(S7SDocumentChange.DocumentID)]),
                            ChangedBy = row[nameof(S7SDocumentChange.ChangedBy)].ToString(),
                            ChangeDate = row[nameof(S7SDocumentChange.ChangeDate)] == DBNull.Value ? null : Convert.ToDateTime(row[nameof(S7SDocumentChange.ChangeDate)]),
                            ChangeDescription = row[nameof(S7SDocumentChange.ChangeDescription)].ToString(),
                            VersionAfterChange = row[nameof(S7SDocumentChange.VersionAfterChange)].ToString()
                        });
                    }
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error retrieving document history");
            }
        }

        [HttpPost]
        public object Save([FromBody] S7SDocument document)
        {
            try
            {
                var docId = _docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.ExecuteScalar,
                    Operation: "SaveDocument",
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    DocumentID: document.DocumentID?.ToString(),
                    Title: document.Title,
                    ReferenceCode: document.ReferenceCode,
                    Purpose: document.Purpose,
                    Owner: document.Owner,
                    VersionNumber: document.VersionNumber,
                    IssueDate: document.IssueDate?.ToString("yyyy-MM-dd"),
                    NextReviewDate: document.NextReviewDate?.ToString("yyyy-MM-dd"),
                    ApprovedBy: document.ApprovedBy,
                    Location: document.Location,
                    AccessLevel: document.AccessLevel,
                    Status: document.Status,
                    WorkflowStatusID: document.WorkflowStatusID?.ToString());

                return Ok(new { DocumentID = Convert.ToInt32(docId) });
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error saving document");
            }
        }

        [HttpPost("{documentId}/review")]
        public object LogReview(int documentId, [FromBody] S7SReview review)
        {
            try
            {
                var result = _docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "LogReview",
                    DocumentID: documentId.ToString(),
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    Owner: review.ReviewedBy ?? _currentUser?.UserName,
                    ReviewComments: review.ReviewComments,
                    VersionNumber: $"REV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    NextReviewDate: review.ReviewDate?.ToString("yyyy-MM-dd"));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error logging review");
            }
        }

        [HttpPost("{documentId}/approve")]
        public object Approve(int documentId, [FromBody] S7SApproval approval)
        {
            try
            {
                var result = _docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "LogApproval",
                    DocumentID: documentId.ToString(),
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    ApprovedBy: approval.ApprovedBy ?? _currentUser?.UserName,
                    IssueDate: approval.ApprovalDate?.ToString("yyyy-MM-dd"),
                    ApprovalComments: approval.ApprovalComments,
                    VersionNumber: string.IsNullOrWhiteSpace(approval.ApprovalComments) ? "Approved" : approval.ApprovalComments);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error approving document");
            }
        }

        [HttpPost("{documentId}/publish")]
        public object Publish(int documentId, [FromBody] S7SDocument document)
        {
            try
            {
                var result = _docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "Publish",
                    DocumentID: documentId.ToString(),
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    ApprovedBy: document.ApprovedBy ?? _currentUser?.UserName,
                    IssueDate: document.IssueDate?.ToString("yyyy-MM-dd"),
                    NextReviewDate: document.NextReviewDate?.ToString("yyyy-MM-dd"),
                    ChangeSummary: document.Purpose,
                    VersionNumber: document.VersionNumber);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error publishing document");
            }
        }

        [HttpPost("notifications/review")]
        public object QueueReviewNotifications()
        {
            try
            {
                var result = _docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "QueueReviewNotifications",
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    Owner: _currentUser?.UserName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error queuing notifications");
            }
        }

        [HttpPost("communications/execute")]
        public object ExecuteCommunicationPlan([FromBody] int commPlanId)
        {
            try
            {
                var result = _docControl.QueryDatabase(DBParentStoredProcedureClass.SqlQueryType.FillDataset,
                    Operation: "ExecuteCommPlan",
                    CommPlanID: commPlanId.ToString(),
                    UserID: _currentUser?.UserID,
                    CompanyID: _currentUser?.CompanyID,
                    Owner: _currentUser?.UserName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _common.LogError(ex, Request);
                return BadRequest("Error executing communication plan");
            }
        }
    }
}
