using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBS7SDocControl_SP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField DocumentID = new TableField("DocumentID", SqlDbType.Int);
        public TableField Title = new TableField("Title", SqlDbType.NVarChar);
        public TableField ReferenceCode = new TableField("ReferenceCode", SqlDbType.NVarChar);
        public TableField Purpose = new TableField("Purpose", SqlDbType.NVarChar);
        public TableField Owner = new TableField("Owner", SqlDbType.NVarChar);
        public TableField VersionNumber = new TableField("VersionNumber", SqlDbType.NVarChar);
        public TableField IssueDate = new TableField("IssueDate", SqlDbType.Date);
        public TableField NextReviewDate = new TableField("NextReviewDate", SqlDbType.Date);
        public TableField ApprovedBy = new TableField("ApprovedBy", SqlDbType.NVarChar);
        public TableField Location = new TableField("Location", SqlDbType.NVarChar);
        public TableField AccessLevel = new TableField("AccessLevel", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField WorkflowStatusID = new TableField("WorkflowStatusID", SqlDbType.Int);
        public TableField ChangeSummary = new TableField("ChangeSummary", SqlDbType.NVarChar);
        public TableField ContentUri = new TableField("ContentUri", SqlDbType.NVarChar);
        public TableField ReviewComments = new TableField("ReviewComments", SqlDbType.NVarChar);
        public TableField ApprovalComments = new TableField("ApprovalComments", SqlDbType.NVarChar);
        public TableField CommPlanID = new TableField("CommPlanID", SqlDbType.Int);
        public TableField TemplateID = new TableField("TemplateID", SqlDbType.Int);
        public TableField Recipient = new TableField("Recipient", SqlDbType.NVarChar);

        public DBS7SDocControl_SP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "S7SDocControl_SP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string UserID = "",
            string CompanyID = "",
            string UnitID = "",
            string DocumentID = "",
            string Title = "",
            string ReferenceCode = "",
            string Purpose = "",
            string Owner = "",
            string VersionNumber = "",
            string IssueDate = "",
            string NextReviewDate = "",
            string ApprovedBy = "",
            string Location = "",
            string AccessLevel = "",
            string Status = "",
            string WorkflowStatusID = "",
            string ChangeSummary = "",
            string ContentUri = "",
            string ReviewComments = "",
            string ApprovalComments = "",
            string CommPlanID = "",
            string TemplateID = "",
            string Recipient = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.DocumentID.SetValue(DocumentID, ref FieldsArrayList);
            this.Title.SetValue(Title, ref FieldsArrayList);
            this.ReferenceCode.SetValue(ReferenceCode, ref FieldsArrayList);
            this.Purpose.SetValue(Purpose, ref FieldsArrayList);
            this.Owner.SetValue(Owner, ref FieldsArrayList);
            this.VersionNumber.SetValue(VersionNumber, ref FieldsArrayList);
            this.IssueDate.SetValue(IssueDate, ref FieldsArrayList);
            this.NextReviewDate.SetValue(NextReviewDate, ref FieldsArrayList);
            this.ApprovedBy.SetValue(ApprovedBy, ref FieldsArrayList);
            this.Location.SetValue(Location, ref FieldsArrayList);
            this.AccessLevel.SetValue(AccessLevel, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.WorkflowStatusID.SetValue(WorkflowStatusID, ref FieldsArrayList);
            this.ChangeSummary.SetValue(ChangeSummary, ref FieldsArrayList);
            this.ContentUri.SetValue(ContentUri, ref FieldsArrayList);
            this.ReviewComments.SetValue(ReviewComments, ref FieldsArrayList);
            this.ApprovalComments.SetValue(ApprovalComments, ref FieldsArrayList);
            this.CommPlanID.SetValue(CommPlanID, ref FieldsArrayList);
            this.TemplateID.SetValue(TemplateID, ref FieldsArrayList);
            this.Recipient.SetValue(Recipient, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
