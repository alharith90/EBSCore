using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBThirdPartyIncidentSP : DBParentStoredProcedureClass
    {
        public DBThirdPartyIncidentSP(IConfiguration configuration)
            : base("ThirdPartyIncidentSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            int? IssueIncidentID = null,
            int? ThirdPartyID = null,
            string Date = null,
            string IssueType = null,
            string Description = null,
            string ImpactOnBusiness = null,
            string Severity = null,
            string RelatedSLABreach = null,
            string ActionsTakenByVendor = null,
            string ActionsTakenInternally = null,
            string Status = null,
            string LinkedRiskEvent = null,
            string Notes = null,
            int? CreatedBy = null,
            int? UpdatedBy = null,
            object SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                IssueIncidentID,
                ThirdPartyID,
                Date,
                IssueType,
                Description,
                ImpactOnBusiness,
                Severity,
                RelatedSLABreach,
                ActionsTakenByVendor,
                ActionsTakenInternally,
                Status,
                LinkedRiskEvent,
                Notes,
                CreatedBy,
                UpdatedBy,
                SerializedObject
            });
        }
    }
}
