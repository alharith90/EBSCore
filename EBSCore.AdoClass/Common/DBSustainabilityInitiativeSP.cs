using System;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBSustainabilityInitiativeSP : DBParentStoredProcedureClass
    {
        public DBSustainabilityInitiativeSP(IConfiguration configuration)
            : base("SustainabilityInitiativeSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? InitiativeID = null,
            string InitiativeName = null,
            string Description = null,
            string ESGCategory = null,
            DateTime? StartDate = null,
            DateTime? EndDate = null,
            string ResponsibleDepartment = null,
            string KeyMetrics = null,
            string BudgetAllocated = null,
            string Outcome = null,
            string Status = null,
            long? CreatedBy = null,
            DateTime? CreatedAt = null,
            long? UpdatedBy = null,
            DateTime? UpdatedAt = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                InitiativeID,
                InitiativeName,
                Description,
                ESGCategory,
                StartDate,
                EndDate,
                ResponsibleDepartment,
                KeyMetrics,
                BudgetAllocated,
                Outcome,
                Status,
                CreatedBy,
                CreatedAt,
                UpdatedBy,
                UpdatedAt
            });
        }
    }
}
