using System.Collections;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace EBSCore.AdoClass
{
    public class DBEnvironmentalAspectSP : DBParentStoredProcedureClass
    {
        public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
        public TableField UserID = new TableField("UserID", SqlDbType.BigInt);
        public TableField CompanyID = new TableField("CompanyID", SqlDbType.Int);
        public TableField AspectID = new TableField("AspectID", SqlDbType.BigInt);
        public TableField UnitID = new TableField("UnitID", SqlDbType.BigInt);
        public TableField AspectDescription = new TableField("AspectDescription", SqlDbType.NVarChar);
        public TableField EnvironmentalImpact = new TableField("EnvironmentalImpact", SqlDbType.NVarChar);
        public TableField ImpactSeverity = new TableField("ImpactSeverity", SqlDbType.NVarChar);
        public TableField FrequencyLikelihood = new TableField("FrequencyLikelihood", SqlDbType.NVarChar);
        public TableField SignificanceRating = new TableField("SignificanceRating", SqlDbType.NVarChar);
        public TableField ControlsInPlace = new TableField("ControlsInPlace", SqlDbType.NVarChar);
        public TableField LegalRequirement = new TableField("LegalRequirement", SqlDbType.NVarChar);
        public TableField ImprovementActions = new TableField("ImprovementActions", SqlDbType.NVarChar);
        public TableField AspectOwner = new TableField("AspectOwner", SqlDbType.NVarChar);
        public TableField MonitoringMetric = new TableField("MonitoringMetric", SqlDbType.NVarChar);
        public TableField LastEvaluationDate = new TableField("LastEvaluationDate", SqlDbType.DateTime);
        public TableField Status = new TableField("Status", SqlDbType.NVarChar);
        public TableField CreatedBy = new TableField("CreatedBy", SqlDbType.BigInt);
        public TableField CreatedAt = new TableField("CreatedAt", SqlDbType.DateTime);
        public TableField UpdatedBy = new TableField("UpdatedBy", SqlDbType.BigInt);
        public TableField UpdatedAt = new TableField("UpdatedAt", SqlDbType.DateTime);

        public DBEnvironmentalAspectSP(IConfiguration configuration)
        {
            _config = configuration;
            Query = "EnvironmentalAspectSP";
        }

        public override ArrayList GetParameters(string operation)
        {
            return new ArrayList { Operation, UserID, CompanyID, AspectID, UnitID, AspectDescription, EnvironmentalImpact, ImpactSeverity, FrequencyLikelihood, SignificanceRating, ControlsInPlace, LegalRequirement, ImprovementActions, AspectOwner, MonitoringMetric, LastEvaluationDate, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt };
        }
    }
}
