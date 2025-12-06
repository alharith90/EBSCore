using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBBCPPlanSP : DBParentStoredProcedureClass
    {
        public DBBCPPlanSP(IConfiguration configuration)
            : base("BCPPlanSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? BCPID = null,
            string PlanName = null,
            string Scope = null,
            string RecoveryTeamRoles = null,
            string ContactList = null,
            string InvocationCriteria = null,
            string RecoveryLocations = null,
            string RecoveryStrategyDetails = null,
            string KeySteps = null,
            string RequiredResources = null,
            string DependentSystems = null,
            string PlanRTO = null,
            string PlanRPO = null,
            string BackupSource = null,
            string AlternateSupplier = null,
            string LastTestDate = null,
            string TestResultSummary = null,
            string PlanOwner = null,
            string PlanStatusID = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                BCPID,
                PlanName,
                Scope,
                RecoveryTeamRoles,
                ContactList,
                InvocationCriteria,
                RecoveryLocations,
                RecoveryStrategyDetails,
                KeySteps,
                RequiredResources,
                DependentSystems,
                PlanRTO,
                PlanRPO,
                BackupSource,
                AlternateSupplier,
                LastTestDate,
                TestResultSummary,
                PlanOwner,
                PlanStatusID
            });
        }
    }
}
