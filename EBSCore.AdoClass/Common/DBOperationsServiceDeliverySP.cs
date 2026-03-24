using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBOperationsServiceDeliverySP : DBParentStoredProcedureClass
    {
        public DBOperationsServiceDeliverySP(IConfiguration configuration)
            : base("OperationsServiceDeliverySP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            int? OrganisationUnitID = null,
            bool? IncludeChildren = null,
            string StartDate = null,
            string EndDate = null,
            int? AssignmentGroupID = null,
            long? AssignedUserID = null,
            string PriorityCode = null,
            string StatusCode = null,
            string SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                OrganisationUnitID,
                IncludeChildren,
                StartDate,
                EndDate,
                AssignmentGroupID,
                AssignedUserID,
                PriorityCode,
                StatusCode,
                SerializedObject
            });
        }
    }
}
