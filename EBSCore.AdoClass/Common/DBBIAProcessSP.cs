using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBBIAProcessSP : DBParentStoredProcedureClass
    {
        public DBBIAProcessSP(IConfiguration configuration)
            : base("BIAProcessSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? BIAID = null,
            string ProcessName = null,
            string Department = null,
            string ProcessOwner = null,
            string CriticalityLevel = null,
            string MAO = null,
            string Impact1Hour = null,
            string Impact1Day = null,
            string Impact1Week = null,
            string ImpactDimensions = null,
            string RTO = null,
            string RPO = null,
            string MinimumResources = null,
            string InternalDependencies = null,
            string ExternalDependencies = null,
            string RecoveryStrategies = null,
            string StrategyLibraryRef = null,
            string AllowableDataLoss = null,
            string BackupAvailability = null,
            bool? HasAlternateWorkaround = null,
            string BCPLink = null,
            string LastReviewDate = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                BIAID,
                ProcessName,
                Department,
                ProcessOwner,
                CriticalityLevel,
                MAO,
                Impact1Hour,
                Impact1Day,
                Impact1Week,
                ImpactDimensions,
                RTO,
                RPO,
                MinimumResources,
                InternalDependencies,
                ExternalDependencies,
                RecoveryStrategies,
                StrategyLibraryRef,
                AllowableDataLoss,
                BackupAvailability,
                HasAlternateWorkaround,
                BCPLink,
                LastReviewDate
            });
        }
    }
}
