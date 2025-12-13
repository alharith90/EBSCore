using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBESGMetricSP : DBParentStoredProcedureClass
    {
        public DBESGMetricSP(IConfiguration configuration)
            : base("ESGMetricSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? MetricID = null,
            string MetricName = null,
            string Category = null,
            string Description = null,
            string UnitOfMeasure = null,
            string DataSource = null,
            string ReportingFrequency = null,
            string TargetValue = null,
            string LatestValue = null,
            string MeasurementDate = null,
            string Owner = null,
            string RelatedObjective = null,
            string RelatedRisk = null,
            string Trend = null,
            string Comments = null,
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
                MetricID,
                MetricName,
                Category,
                Description,
                UnitOfMeasure,
                DataSource,
                ReportingFrequency,
                TargetValue,
                LatestValue,
                MeasurementDate,
                Owner,
                RelatedObjective,
                RelatedRisk,
                Trend,
                Comments,
                CreatedBy,
                CreatedAt,
                UpdatedBy,
                UpdatedAt
            });
        }
    }
}
