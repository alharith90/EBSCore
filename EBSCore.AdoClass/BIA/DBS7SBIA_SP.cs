using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;

namespace EBSCore.AdoClass
{
    public class DBS7SBIA_SP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField BIAID = new TableField("BIAID", SqlDbType.Int);
        public TableField BIACode = new TableField("BIACode", SqlDbType.NVarChar);
        public TableField UnitID = new TableField("UnitID", SqlDbType.NVarChar);
        public TableField ProcessID = new TableField("ProcessID", SqlDbType.NVarChar);
        public TableField ProcessName = new TableField("ProcessName", SqlDbType.NVarChar);
        public TableField ProcessDescription = new TableField("ProcessDescription", SqlDbType.NVarChar);
        public TableField Frequency = new TableField("Frequency", SqlDbType.NVarChar);
        public TableField Criticality = new TableField("Criticality", SqlDbType.NVarChar);
        public TableField RTO = new TableField("RTO", SqlDbType.NVarChar);
        public TableField RPO = new TableField("RPO", SqlDbType.NVarChar);
        public TableField MTPD = new TableField("MTPD", SqlDbType.NVarChar);
        public TableField MAO = new TableField("MAO", SqlDbType.NVarChar);
        public TableField MBCO = new TableField("MBCO", SqlDbType.NVarChar);
        public TableField Priority = new TableField("Priority", SqlDbType.NVarChar);
        public TableField RequiredCompetencies = new TableField("RequiredCompetencies", SqlDbType.NVarChar);
        public TableField AlternativeWorkLocation = new TableField("AlternativeWorkLocation", SqlDbType.NVarChar);
        public TableField RegulatoryRequirements = new TableField("RegulatoryRequirements", SqlDbType.NVarChar);
        public TableField PrimaryStaff = new TableField("PrimaryStaff", SqlDbType.NVarChar);
        public TableField BackupStaff = new TableField("BackupStaff", SqlDbType.NVarChar);
        public TableField RTOJustification = new TableField("RTOJustification", SqlDbType.NVarChar);
        public TableField MBCODetails = new TableField("MBCODetails", SqlDbType.NVarChar);
        public TableField RevenueLossPerHour = new TableField("RevenueLossPerHour", SqlDbType.NVarChar);
        public TableField CostOfDowntime = new TableField("CostOfDowntime", SqlDbType.NVarChar);
        public TableField Remarks = new TableField("Remarks", SqlDbType.NVarChar);
        public TableField LastComment = new TableField("LastComment", SqlDbType.NVarChar);
        public TableField ReviewDate = new TableField("ReviewDate", SqlDbType.NVarChar);
        public TableField WorkFlowStatus = new TableField("WorkFlowStatus", SqlDbType.NVarChar);
        public TableField IsDeleted = new TableField("IsDeleted", SqlDbType.Bit);

        public DBS7SBIA_SP(IConfiguration configuration) : base(configuration)
        {
            SPName = "S7SBIA_SP";
        }

        public new object QueryDatabase(
            SqlQueryType queryType,
            string Operation = "",
            string CompanyID = "",
            string UserID = "",
            string BIAID = "",
            string BIACode = "",
            string UnitID = "",
            string ProcessID = "",
            string ProcessName = "",
            string ProcessDescription = "",
            string Frequency = "",
            string Criticality = "",
            string RTO = "",
            string RPO = "",
            string MTPD = "",
            string MAO = "",
            string MBCO = "",
            string Priority = "",
            string RequiredCompetencies = "",
            string AlternativeWorkLocation = "",
            string RegulatoryRequirements = "",
            string PrimaryStaff = "",
            string BackupStaff = "",
            string RTOJustification = "",
            string MBCODetails = "",
            string RevenueLossPerHour = "",
            string CostOfDowntime = "",
            string Remarks = "",
            string LastComment = "",
            string ReviewDate = "",
            string WorkFlowStatus = "",
            string IsDeleted = ""
        )
        {
            FieldsArrayList = new ArrayList();

            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.BIAID.SetValue(BIAID, ref FieldsArrayList);
            this.BIACode.SetValue(BIACode, ref FieldsArrayList);
            this.UnitID.SetValue(UnitID, ref FieldsArrayList);
            this.ProcessID.SetValue(ProcessID, ref FieldsArrayList);
            this.ProcessName.SetValue(ProcessName, ref FieldsArrayList);
            this.ProcessDescription.SetValue(ProcessDescription, ref FieldsArrayList);
            this.Frequency.SetValue(Frequency, ref FieldsArrayList);
            this.Criticality.SetValue(Criticality, ref FieldsArrayList);
            this.RTO.SetValue(RTO, ref FieldsArrayList);
            this.RPO.SetValue(RPO, ref FieldsArrayList);
            this.MTPD.SetValue(MTPD, ref FieldsArrayList);
            this.MAO.SetValue(MAO, ref FieldsArrayList);
            this.MBCO.SetValue(MBCO, ref FieldsArrayList);
            this.Priority.SetValue(Priority, ref FieldsArrayList);
            this.RequiredCompetencies.SetValue(RequiredCompetencies, ref FieldsArrayList);
            this.AlternativeWorkLocation.SetValue(AlternativeWorkLocation, ref FieldsArrayList);
            this.RegulatoryRequirements.SetValue(RegulatoryRequirements, ref FieldsArrayList);
            this.PrimaryStaff.SetValue(PrimaryStaff, ref FieldsArrayList);
            this.BackupStaff.SetValue(BackupStaff, ref FieldsArrayList);
            this.RTOJustification.SetValue(RTOJustification, ref FieldsArrayList);
            this.MBCODetails.SetValue(MBCODetails, ref FieldsArrayList);
            this.RevenueLossPerHour.SetValue(RevenueLossPerHour, ref FieldsArrayList);
            this.CostOfDowntime.SetValue(CostOfDowntime, ref FieldsArrayList);
            this.Remarks.SetValue(Remarks, ref FieldsArrayList);
            this.LastComment.SetValue(LastComment, ref FieldsArrayList);
            this.ReviewDate.SetValue(ReviewDate, ref FieldsArrayList);
            this.WorkFlowStatus.SetValue(WorkFlowStatus, ref FieldsArrayList);
            this.IsDeleted.SetValue(IsDeleted, ref FieldsArrayList);

            return base.QueryDatabase(queryType);
        }
    }
}
