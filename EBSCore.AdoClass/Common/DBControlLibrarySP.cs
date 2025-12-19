using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBControlLibrarySP : DBParentStoredProcedureClass
    {
        public DBControlLibrarySP(IConfiguration configuration)
            : base("ControlLibrarySP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? ControlID = null,
            string ControlName = null,
            string Description = null,
            string ControlType = null,
            string ControlCategory = null,
            string ControlOwner = null,
            string Frequency = null,
            bool? IsKeyControl = null,
            string RelatedRisks = null,
            string RelatedObligations = null,
            string ImplementationStatus = null,
            string LastTestDate = null,
            string LastTestResult = null,
            string DocumentationReference = null,
            long? CreatedBy = null,
            long? ModifiedBy = null,
            object SerializedObject = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                ControlID,
                ControlName,
                Description,
                ControlType,
                ControlCategory,
                ControlOwner,
                Frequency,
                IsKeyControl,
                RelatedRisks,
                RelatedObligations,
                ImplementationStatus,
                LastTestDate,
                LastTestResult,
                DocumentationReference,
                CreatedBy,
                ModifiedBy,
                SerializedObject
            });
        }
    }
}
