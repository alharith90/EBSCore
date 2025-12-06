using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBThirdPartyProfileSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField ThirdPartyID = new TableField("ThirdPartyID", SqlDbType.Int);
        public TableField ThirdPartyName = new TableField("ThirdPartyName", SqlDbType.NVarChar);
        public TableField ServiceType = new TableField("ServiceType", SqlDbType.NVarChar);
        public TableField CriticalityTier = new TableField("CriticalityTier", SqlDbType.NVarChar);
        public TableField InherentRiskRating = new TableField("InherentRiskRating", SqlDbType.NVarChar);
        public TableField CountryRegion = new TableField("CountryRegion", SqlDbType.NVarChar);
        public TableField BusinessOwner = new TableField("BusinessOwner", SqlDbType.NVarChar);
        public TableField ContractValue = new TableField("ContractValue", SqlDbType.NVarChar);
        public TableField ContractExpiryDate = new TableField("ContractExpiryDate", SqlDbType.DateTime);
        public TableField KeySLAKPIRequirements = new TableField("KeySLAKPIRequirements", SqlDbType.NVarChar);
        public TableField ComplianceRequirements = new TableField("ComplianceRequirements", SqlDbType.NVarChar);
        public TableField PrivacyDataProcessing = new TableField("PrivacyDataProcessing", SqlDbType.Bit);
        public TableField RelatedAssetProcess = new TableField("RelatedAssetProcess", SqlDbType.NVarChar);
        public TableField ContingencyPlan = new TableField("ContingencyPlan", SqlDbType.NVarChar);
        public TableField LastAssessmentDate = new TableField("LastAssessmentDate", SqlDbType.DateTime);
        public TableField NextReviewDate = new TableField("NextReviewDate", SqlDbType.DateTime);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.Int);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.Int);

        public DBThirdPartyProfileSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "ThirdPartyProfileSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, ThirdPartyID, ThirdPartyName, ServiceType, CriticalityTier, InherentRiskRating, CountryRegion, BusinessOwner, ContractValue, ContractExpiryDate, KeySLAKPIRequirements, ComplianceRequirements, PrivacyDataProcessing, RelatedAssetProcess, ContingencyPlan, LastAssessmentDate, NextReviewDate, Status, CreatedBy, UpdatedBy };
        }
    }
}
