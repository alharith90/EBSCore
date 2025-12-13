using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBEnvironmentalObjectiveSP : DBParentStoredProcedureClass
    {
        public DBEnvironmentalObjectiveSP(IConfiguration configuration)
            : base("EnvironmentalObjectiveSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? ObjectiveID = null,
            long? UnitID = null,
            string ObjectiveDescription = null,
            string TargetValue = null,
            string Unit = null,
            string BaselineValue = null,
            string CurrentValue = null,
            string TargetDate = null,
            string ResponsibleOwner = null,
            string Status = null,
            long? CreatedBy = null,
            string CreatedAt = null,
            long? UpdatedBy = null,
            string UpdatedAt = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                ObjectiveID,
                UnitID,
                ObjectiveDescription,
                TargetValue,
                Unit,
                BaselineValue,
                CurrentValue,
                TargetDate,
                ResponsibleOwner,
                Status,
                CreatedBy,
                CreatedAt,
                UpdatedBy,
                UpdatedAt
            });
        }
    }
}
