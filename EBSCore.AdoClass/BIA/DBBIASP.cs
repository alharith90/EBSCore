using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBBIASP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.Int);
        public TableField BIAID = new TableField("BIAID", SqlDbType.Int);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField BIACode = new TableField("BIACode", SqlDbType.NVarChar);
        public TableField UnitID = new TableField("UnitID", SqlDbType.Int);
        public TableField ProcessID = new TableField("ProcessID", SqlDbType.Int);
        public TableField ProcessName = new TableField("ProcessName", SqlDbType.NVarChar);
        public TableField ProcessDescription = new TableField("ProcessDescription", SqlDbType.NVarChar);
        public TableField Frequency = new TableField("Frequency", SqlDbType.Int);
        public TableField Criticality = new TableField("Criticality", SqlDbType.Int);
        public TableField RTO = new TableField("RTO", SqlDbType.Int);
        public TableField RPO = new TableField("RPO", SqlDbType.Int);
        public TableField MAO = new TableField("MAO", SqlDbType.Int);
        public TableField MTPD = new TableField("MTPD", SqlDbType.Int);
        public TableField MTD = new TableField("MTD", SqlDbType.Int);
        public TableField MBCO = new TableField("MBCO", SqlDbType.Int);
        public TableField PrimaryStaff = new TableField("PrimaryStaff", SqlDbType.Int);
        public TableField BackupStaff = new TableField("BackupStaff", SqlDbType.Int);
        public TableField RTOJustification = new TableField("RTOJustification", SqlDbType.NVarChar);
        public TableField MBCODetails = new TableField("MBCODetails", SqlDbType.NVarChar);
        public TableField Priority = new TableField("Priority", SqlDbType.Int);
        public TableField RequiredCompetencies = new TableField("RequiredCompetencies", SqlDbType.NVarChar);
        public TableField RevenueLossPerHour = new TableField("RevenueLossPerHour", SqlDbType.Decimal);
        public TableField CostOfDowntime = new TableField("CostOfDowntime", SqlDbType.Decimal);
        public TableField Remarks = new TableField("Remarks", SqlDbType.NVarChar);
        public TableField LastComment = new TableField("LastComment", SqlDbType.NVarChar);
        public TableField ReviewDate = new TableField("ReviewDate", SqlDbType.DateTime);
        public TableField WorkFlowStatus = new TableField("WorkFlowStatus", SqlDbType.Int);
        public TableField IsDeleted = new TableField("IsDeleted", SqlDbType.Bit);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBBIASP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "BIASP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", 
            string UserID = "", 
            string BIAID = "", 
            string CompanyID = "", 
            string BIACode = "",
            string UnitID = "", 
            string ProcessID = "", 
            string ProcessName = "", 
            string ProcessDescription = "",
            string Frequency = "", 
            string Criticality = "", 
            string RTO = "", 
            string RPO = "", 
            string MAO = "",
            string MTPD = "", 
            string MTD = "", 
            string MBCO = "",
            string PrimaryStaff = "",
            string BackupStaff = "",
            string RTOJustification = "",
            string MBCODetails = "", 
            string Priority = "", 
            string RequiredCompetencies = "",
            string RevenueLossPerHour = "", 
            string CostOfDowntime = "", 
            string Remarks = "",
            string LastComment = "", 
            string ReviewDate = "", 
            string WorkFlowStatus = "",
            string IsDeleted = "", 
            string CreatedBy = "", 
            string UpdatedBy = "", 
            string CreatedAt = "", 
            string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.BIAID.SetValue(BIAID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.BIACode.SetValue(BIACode, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.ProcessID.SetValue(ProcessID, ref FieldsArrayList);
            this.ProcessName.SetValue(ProcessName, ref FieldsArrayList);
            this.ProcessDescription.SetValue(ProcessDescription, ref FieldsArrayList);
            this.Frequency.SetValue(Frequency, ref FieldsArrayList);
            this.Criticality.SetValue(Criticality, ref FieldsArrayList);
            this.RTO.SetValue(RTO, ref FieldsArrayList);
            this.RPO.SetValue(RPO, ref FieldsArrayList);
            this.MAO.SetValue(MAO, ref FieldsArrayList);
            this.MTPD.SetValue(MTPD, ref FieldsArrayList);
            this.MTD.SetValue(MTD, ref FieldsArrayList);
            this.MBCO.SetValue(MBCO, ref FieldsArrayList);
            this.PrimaryStaff.SetValue(PrimaryStaff, ref FieldsArrayList);
            this.BackupStaff.SetValue(BackupStaff, ref FieldsArrayList);
            this.RTOJustification.SetValue(RTOJustification, ref FieldsArrayList);
            this.MBCODetails.SetValue(MBCODetails, ref FieldsArrayList);
            this.Priority.SetValue(Priority, ref FieldsArrayList);
            this.RequiredCompetencies.SetValue(RequiredCompetencies, ref FieldsArrayList);
            this.RevenueLossPerHour.SetValue(RevenueLossPerHour, ref FieldsArrayList);
            this.CostOfDowntime.SetValue(CostOfDowntime, ref FieldsArrayList);
            this.Remarks.SetValue(Remarks, ref FieldsArrayList);
            this.LastComment.SetValue(LastComment, ref FieldsArrayList);
            this.ReviewDate.SetValue(ReviewDate, ref FieldsArrayList);
            this.WorkFlowStatus.SetValue(WorkFlowStatus, ref FieldsArrayList);
            this.IsDeleted.SetValue(IsDeleted, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.UpdatedBy.SetValue(UpdatedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
