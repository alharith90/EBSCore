using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBRequirementSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField RequirementCode = new TableField("RequirementCode", SqlDbType.Int);
        public TableField RequirementNo = new TableField("RequirementNo", SqlDbType.NVarChar);
        public TableField RequirementType = new TableField("RequirementType", SqlDbType.NVarChar);
        public TableField RequirementTitle = new TableField("RequirementTitle", SqlDbType.NVarChar);
        public TableField RequirementDescription = new TableField("RequirementDescription", SqlDbType.NVarChar);
        public TableField Subcategory = new TableField("Subcategory", SqlDbType.NVarChar);
        public TableField RequirementDetails = new TableField("RequirementDetails", SqlDbType.NVarChar);
        public TableField RequirementTags = new TableField("RequirementTags", SqlDbType.NVarChar);
        public TableField RequirementFrequency = new TableField("RequirementFrequency", SqlDbType.NVarChar);
        public TableField ExternalAudit = new TableField("ExternalAudit", SqlDbType.NVarChar);
        public TableField InternalAudit = new TableField("InternalAudit", SqlDbType.NVarChar);
        public TableField AuditReference = new TableField("AuditReference", SqlDbType.NVarChar);
        public TableField RiskCategory = new TableField("RiskCategory", SqlDbType.NVarChar);
        public TableField ControlOwner = new TableField("ControlOwner", SqlDbType.NVarChar);
        public TableField ControlOwnerFunction = new TableField("ControlOwnerFunction", SqlDbType.NVarChar);
        public TableField EvidenceRequired = new TableField("EvidenceRequired", SqlDbType.NVarChar);
        public TableField EvidenceDetails = new TableField("EvidenceDetails", SqlDbType.NVarChar);
        public TableField ControlID = new TableField("ControlID", SqlDbType.NVarChar);
        public TableField OrganizationUnitID = new TableField("OrganizationUnitID", SqlDbType.BigInt);
        public TableField EscalationProcess = new TableField("EscalationProcess", SqlDbType.NVarChar);
        public TableField EscalationThreshold = new TableField("EscalationThreshold", SqlDbType.NVarChar);
        public TableField BCMActivationType = new TableField("BCMActivationType", SqlDbType.NVarChar);
        public TableField BCMActivationDecision = new TableField("BCMActivationDecision", SqlDbType.NVarChar);
        public TableField EscalationContacts = new TableField("EscalationContacts", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField ModifiedBy = new TableField("ModifiedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBRequirementSP(IConfiguration configuration) : base(configuration)
        {
            base.SPName = "RequirementSP";
        }

        public new object QueryDatabase(SqlQueryType QueryType,
            string Operation = "", string UserID = "", string CompanyID = "", string RequirementCode = "",
            string RequirementNo = "", string RequirementType = "", string RequirementTitle = "", string RequirementDescription = "",
            string Subcategory = "", string RequirementDetails = "", string RequirementTags = "", string RequirementFrequency = "",
            string ExternalAudit = "", string InternalAudit = "", string AuditReference = "", string RiskCategory = "",
            string ControlOwner = "", string ControlOwnerFunction = "", string EvidenceRequired = "", string EvidenceDetails = "",
            string ControlID = "", string OrganizationUnitID = "", string EscalationProcess = "", string EscalationThreshold = "",
            string BCMActivationType = "", string BCMActivationDecision = "", string EscalationContacts = "", string Status = "",
            string CreatedBy = "", string ModifiedBy = "", string CreatedAt = "", string UpdatedAt = "")
        {
            FieldsArrayList = new ArrayList();
            this.Operation.SetValue(Operation, ref FieldsArrayList);
            this.UserID.SetValue(UserID, ref FieldsArrayList);
            this.CompanyID.SetValue(CompanyID, ref FieldsArrayList);
            this.RequirementCode.SetValue(RequirementCode, ref FieldsArrayList);
            this.RequirementNo.SetValue(RequirementNo, ref FieldsArrayList);
            this.RequirementType.SetValue(RequirementType, ref FieldsArrayList);
            this.RequirementTitle.SetValue(RequirementTitle, ref FieldsArrayList);
            this.RequirementDescription.SetValue(RequirementDescription, ref FieldsArrayList);
            this.Subcategory.SetValue(Subcategory, ref FieldsArrayList);
            this.RequirementDetails.SetValue(RequirementDetails, ref FieldsArrayList);
            this.RequirementTags.SetValue(RequirementTags, ref FieldsArrayList);
            this.RequirementFrequency.SetValue(RequirementFrequency, ref FieldsArrayList);
            this.ExternalAudit.SetValue(ExternalAudit, ref FieldsArrayList);
            this.InternalAudit.SetValue(InternalAudit, ref FieldsArrayList);
            this.AuditReference.SetValue(AuditReference, ref FieldsArrayList);
            this.RiskCategory.SetValue(RiskCategory, ref FieldsArrayList);
            this.ControlOwner.SetValue(ControlOwner, ref FieldsArrayList);
            this.ControlOwnerFunction.SetValue(ControlOwnerFunction, ref FieldsArrayList);
            this.EvidenceRequired.SetValue(EvidenceRequired, ref FieldsArrayList);
            this.EvidenceDetails.SetValue(EvidenceDetails, ref FieldsArrayList);
            this.ControlID.SetValue(ControlID, ref FieldsArrayList);
            this.OrganizationUnitID.SetValue(OrganizationUnitID, ref FieldsArrayList);
            this.EscalationProcess.SetValue(EscalationProcess, ref FieldsArrayList);
            this.EscalationThreshold.SetValue(EscalationThreshold, ref FieldsArrayList);
            this.BCMActivationType.SetValue(BCMActivationType, ref FieldsArrayList);
            this.BCMActivationDecision.SetValue(BCMActivationDecision, ref FieldsArrayList);
            this.EscalationContacts.SetValue(EscalationContacts, ref FieldsArrayList);
            this.Status.SetValue(Status, ref FieldsArrayList);
            this.CreatedBy.SetValue(CreatedBy, ref FieldsArrayList);
            this.ModifiedBy.SetValue(ModifiedBy, ref FieldsArrayList);
            this.CreatedAt.SetValue(CreatedAt, ref FieldsArrayList);
            this.UpdatedAt.SetValue(UpdatedAt, ref FieldsArrayList);

            return base.QueryDatabase(QueryType);
        }
    }
}
