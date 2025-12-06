using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBKeyRiskIndicatorSP : DBParentStoredProcedureClass
    {
        public DBKeyRiskIndicatorSP(IConfiguration configuration)
            : base("KeyRiskIndicatorSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? IndicatorID = null,
            string IndicatorName = null,
            string RelatedRisk = null,
            string MeasurementFrequency = null,
            string DataSource = null,
            string ThresholdValue = null,
            string CurrentValue = null,
            string Status = null,
            string Owner = null,
            string LastUpdateDate = null,
            long? CreatedBy = null,
            long? ModifiedBy = null)
        {
            return base.QueryDatabase(queryType, new
            {
                Operation,
                UserID,
                CompanyID,
                IndicatorID,
                IndicatorName,
                RelatedRisk,
                MeasurementFrequency,
                DataSource,
                ThresholdValue,
                CurrentValue,
                Status,
                Owner,
                LastUpdateDate,
                CreatedBy,
                ModifiedBy
            });
        }
    }
}
