using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBESGMetricSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField MetricID = new TableField("MetricID", SqlDbType.BigInt);
        public TableField MetricName = new TableField("MetricName", SqlDbType.NVarChar);
        public TableField Category = new TableField("Category", SqlDbType.NVarChar);
        public TableField Description = new TableField("Description", SqlDbType.NVarChar);
        public TableField UnitOfMeasure = new TableField("UnitOfMeasure", SqlDbType.NVarChar);
        public TableField DataSource = new TableField("DataSource", SqlDbType.NVarChar);
        public TableField ReportingFrequency = new TableField("ReportingFrequency", SqlDbType.NVarChar);
        public TableField TargetValue = new TableField("TargetValue", SqlDbType.NVarChar);
        public TableField LatestValue = new TableField("LatestValue", SqlDbType.NVarChar);
        public TableField MeasurementDate = new TableField("MeasurementDate", SqlDbType.DateTime);
        public TableField Owner = new TableField("Owner", SqlDbType.NVarChar);
        public TableField RelatedObjective = new TableField("RelatedObjective", SqlDbType.NVarChar);
        public TableField RelatedRisk = new TableField("RelatedRisk", SqlDbType.NVarChar);
        public TableField Trend = new TableField("Trend", SqlDbType.NVarChar);
        public TableField Comments = new TableField("Comments", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.BigInt);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBESGMetricSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "ESGMetricSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, MetricID, MetricName, Category, Description, UnitOfMeasure, DataSource, ReportingFrequency, TargetValue, LatestValue, MeasurementDate, Owner, RelatedObjective, RelatedRisk, Trend, Comments, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt };
        }
    }
}
