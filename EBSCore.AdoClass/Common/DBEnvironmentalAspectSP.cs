using System;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;

namespace EBSCore.AdoClass
{
    public class DBEnvironmentalAspectSP : DBParentStoredProcedureClass
    {
        public DBEnvironmentalAspectSP(IConfiguration configuration)
            : base("EnvironmentalAspectSP", configuration)
        {
        }

        public object QueryDatabase(
            SqlQueryType queryType,
            string Operation = null,
            long? UserID = null,
            int? CompanyID = null,
            long? AspectID = null,
            long? UnitID = null,
            string AspectDescription = null,
            string EnvironmentalImpact = null,
            string ImpactSeverity = null,
            string FrequencyLikelihood = null,
            string SignificanceRating = null,
            string ControlsInPlace = null,
            string LegalRequirement = null,
            string ImprovementActions = null,
            string AspectOwner = null,
            string MonitoringMetric = null,
            DateTime? LastEvaluationDate = null,
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
                AspectID,
                UnitID,
                AspectDescription,
                EnvironmentalImpact,
                ImpactSeverity,
                FrequencyLikelihood,
                SignificanceRating,
                ControlsInPlace,
                LegalRequirement,
                ImprovementActions,
                AspectOwner,
                MonitoringMetric,
                LastEvaluationDate,
                Status,
                CreatedBy,
                CreatedAt,
                UpdatedBy,
                UpdatedAt
            });
        }
    }
}
