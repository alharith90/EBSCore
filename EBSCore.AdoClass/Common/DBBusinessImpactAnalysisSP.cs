using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBBusinessImpactAnalysisSP : DBParentStoredProcedureClass
    {
        public DBBusinessImpactAnalysisSP(IConfiguration configuration)
            : base("BusinessImpactAnalysisSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? ProcessAssetID = null,
            string ProcessAssetName = null,
            string MAO = null,
            string RTO = null,
            string Criticality = null,
            string Impact = null,
            string CurrentControls = null,
            string ImprovementActions = null,
            long? CreatedBy = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                ProcessAssetID,
                ProcessAssetName,
                MAO,
                RTO,
                Criticality,
                Impact,
                CurrentControls,
                ImprovementActions,
                CreatedBy,
                ModifiedBy
            });
        }
    }
}
