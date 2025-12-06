using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBAuditUniverseSP : DBParentStoredProcedureClass
    {
        public DBAuditUniverseSP(IConfiguration configuration)
            : base("AuditUniverseSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? EntityProcessID = null,
            string EntityProcessName = null,
            string EntityOwner = null,
            string RiskRating = null,
            string LastAuditDate = null,
            string NextAuditDue = null,
            string AuditFrequency = null,
            string AuditPriority = null,
            long? CreatedBy = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                EntityProcessID,
                EntityProcessName,
                EntityOwner,
                RiskRating,
                LastAuditDate,
                NextAuditDue,
                AuditFrequency,
                AuditPriority,
                CreatedBy,
                ModifiedBy
            });
        }
    }
}
