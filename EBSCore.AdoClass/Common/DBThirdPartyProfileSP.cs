using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBThirdPartyProfileSP : DBParentStoredProcedureClass
    {
        public DBThirdPartyProfileSP(IConfiguration configuration)
            : base("ThirdPartyProfileSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            int? ThirdPartyID = null,
            string ThirdPartyName = null,
            string ServiceType = null,
            string CriticalityTier = null,
            string InherentRiskRating = null,
            string CountryRegion = null,
            string BusinessOwner = null,
            string ContractValue = null,
            string ContractExpiryDate = null,
            string KeySLAKPIRequirements = null,
            string ComplianceRequirements = null,
            bool? PrivacyDataProcessing = null,
            string RelatedAssetProcess = null,
            string ContingencyPlan = null,
            string LastAssessmentDate = null,
            string NextReviewDate = null,
            string Status = null,
            int? CreatedBy = null,
            int? UpdatedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                ThirdPartyID,
                ThirdPartyName,
                ServiceType,
                CriticalityTier,
                InherentRiskRating,
                CountryRegion,
                BusinessOwner,
                ContractValue,
                ContractExpiryDate,
                KeySLAKPIRequirements,
                ComplianceRequirements,
                PrivacyDataProcessing,
                RelatedAssetProcess,
                ContingencyPlan,
                LastAssessmentDate,
                NextReviewDate,
                Status,
                CreatedBy,
                UpdatedBy
            });
        }
    }
}
