using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBThirdPartyIncidentSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField IssueIncidentID = new TableField("IssueIncidentID", SqlDbType.Int);
        public TableField ThirdPartyID = new TableField("ThirdPartyID", SqlDbType.Int);
        public TableField Date = new TableField("Date", SqlDbType.DateTime);
        public TableField IssueType = new TableField("IssueType", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField ImpactOnBusiness = new TableField("ImpactOnBusiness", SqlDbType.NVarChar);
        public TableField Severity = new TableField("Severity", SqlDbType.NVarChar);
        public TableField RelatedSLABreach = new TableField("RelatedSLABreach", SqlDbType.NVarChar);
        public TableField ActionsTakenByVendor = new TableField("ActionsTakenByVendor", SqlDbType.NVarChar);
        public TableField ActionsTakenInternally = new TableField("ActionsTakenInternally", SqlDbType.NVarChar);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField LinkedRiskEvent = new TableField("LinkedRiskEvent", SqlDbType.NVarChar);
        public TableField Notes = new TableField("Notes", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);

        public DBThirdPartyIncidentSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "ThirdPartyIncidentSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, IssueIncidentID, ThirdPartyID, Date, IssueType, Description, ImpactOnBusiness, Severity, RelatedSLABreach, ActionsTakenByVendor, ActionsTakenInternally, Status, LinkedRiskEvent, Notes, CreatedBy, UpdatedBy };
        }
    }
}
