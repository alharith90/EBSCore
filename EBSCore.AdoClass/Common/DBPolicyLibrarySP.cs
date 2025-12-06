using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBPolicyLibrarySP : DBParentStoredProcedureClass
    {
        public DBPolicyLibrarySP(IConfiguration configuration)
            : base("PolicyLibrarySP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? PolicyID = null,
            string PolicyCode = null,
            string PolicyNameEN = null,
            string PolicyNameAR = null,
            string PolicyType = null,
            string CategoryEN = null,
            string CategoryAR = null,
            string DescriptionEN = null,
            string DescriptionAR = null,
            string EffectiveDate = null,
            string ReviewDate = null,
            long? OwnerUserID = null,
            long? ApproverUserID = null,
            string StatusID = null,
            string RelatedRegulationIDs = null,
            string RelatedControlIDs = null,
            string VersionNumber = null,
            string DocumentPath = null,
            bool? IsMandatory = null,
            string AppliesToRoles = null,
            long? CreatedBy = null,
            string CreatedAt = null,
            long? UpdatedBy = null,
            string UpdatedAt = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                PolicyID,
                PolicyCode,
                PolicyNameEN,
                PolicyNameAR,
                PolicyType,
                CategoryEN,
                CategoryAR,
                DescriptionEN,
                DescriptionAR,
                EffectiveDate,
                ReviewDate,
                OwnerUserID,
                ApproverUserID,
                StatusID,
                RelatedRegulationIDs,
                RelatedControlIDs,
                VersionNumber,
                DocumentPath,
                IsMandatory,
                AppliesToRoles,
                CreatedBy,
                CreatedAt,
                UpdatedBy,
                UpdatedAt,
                ModifiedBy
            });
        }
    }
}
