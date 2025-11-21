using System;
using System.Collections.Generic;

namespace EBSCore.Web.Models.Document
{
    public class S7SDocument
    {
        public int? DocumentID { get; set; }
        public string Title { get; set; }
        public string ReferenceCode { get; set; }
        public string Purpose { get; set; }
        public string Owner { get; set; }
        public string VersionNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public string ApprovedBy { get; set; }
        public string Location { get; set; }
        public string AccessLevel { get; set; }
        public string Status { get; set; }
        public int? WorkflowStatusID { get; set; }
        public int? CurrentVersionID { get; set; }
        public List<S7SReview> Reviews { get; set; } = new List<S7SReview>();
        public List<S7SApproval> Approvals { get; set; } = new List<S7SApproval>();
        public List<S7SDocumentVersion> Versions { get; set; } = new List<S7SDocumentVersion>();
        public List<S7SDocumentChange> Changes { get; set; } = new List<S7SDocumentChange>();
    }

    public class S7SDocumentVersion
    {
        public int DocumentVersionID { get; set; }
        public string VersionNumber { get; set; }
        public int DocumentID { get; set; }
        public int? WorkflowStatusID { get; set; }
        public string WorkflowStatus { get; set; }
        public string ChangeSummary { get; set; }
        public string ContentUri { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public string ApprovedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class S7SDocumentChange
    {
        public int ChangeID { get; set; }
        public int DocumentID { get; set; }
        public string ChangedBy { get; set; }
        public DateTime? ChangeDate { get; set; }
        public string ChangeDescription { get; set; }
        public string VersionAfterChange { get; set; }
    }

    public class S7SReview
    {
        public int ReviewID { get; set; }
        public int DocumentID { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string ReviewComments { get; set; }
    }

    public class S7SApproval
    {
        public int ApprovalID { get; set; }
        public int DocumentID { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovalComments { get; set; }
    }
}
