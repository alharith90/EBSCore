using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBManagementReviewReportSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField ReportID = new TableField("ReportID", SqlDbType.Int);
        public TableField ReportTitle = new TableField("ReportTitle", SqlDbType.NVarChar);
        public TableField MeetingDate = new TableField("MeetingDate", SqlDbType.DateTime);
        public TableField Summary = new TableField("Summary", SqlDbType.NVarChar);
        public TableField Decisions = new TableField("Decisions", SqlDbType.NVarChar);
        public TableField FollowUpActions = new TableField("FollowUpActions", SqlDbType.NVarChar);
        public TableField NextReviewDate = new TableField("NextReviewDate", SqlDbType.DateTime);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBManagementReviewReportSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "ManagementReviewReportSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string UnitID = "", string ReportID = "",
            string ReportTitle = "", string MeetingDate = "", string Summary = "", string Decisions = "",
            string FollowUpActions = "", string NextReviewDate = "", string Status = "", string Notes = "",
            string CreatedBy = "", string ModifiedBy = "", string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.ReportID.SetValue(ReportID, ref FieldsArrayList);
            this.ReportTitle.SetValue(ReportTitle, ref FieldsArrayList);
            this.MeetingDate.SetValue(MeetingDate, ref FieldsArrayList);
            this.Summary.SetValue(Summary, ref FieldsArrayList);
            this.Decisions.SetValue(Decisions, ref FieldsArrayList);
            this.FollowUpActions.SetValue(FollowUpActions, ref FieldsArrayList);
            this.NextReviewDate.SetValue(NextReviewDate, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.Notes.SetValue(Notes, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
