using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBStrategicObjectiveSP : DBParentStoredProcedureClass
    {
        public DBStrategicObjectiveSP(IConfiguration configuration)
            : base("StrategicObjectiveSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? ObjectiveID = null,
            string ObjectiveCode = null,
            string ObjectiveNameEN = null,
            string ObjectiveNameAR = null,
            string DescriptionEN = null,
            string DescriptionAR = null,
            string Category = null,
            string Perspective = null,
            long? OwnerUserID = null,
            long? DepartmentID = null,
            string StartDate = null,
            string EndDate = null,
            string TargetValue = null,
            string UnitEN = null,
            string UnitAR = null,
            string RiskAppetiteThreshold = null,
            string StatusID = null,
            long? CreatedBy = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                ObjectiveID,
                ObjectiveCode,
                ObjectiveNameEN,
                ObjectiveNameAR,
                DescriptionEN,
                DescriptionAR,
                Category,
                Perspective,
                OwnerUserID,
                DepartmentID,
                StartDate,
                EndDate,
                TargetValue,
                UnitEN,
                UnitAR,
                RiskAppetiteThreshold,
                StatusID,
                CreatedBy,
                ModifiedBy
            });
        }
    }
}
