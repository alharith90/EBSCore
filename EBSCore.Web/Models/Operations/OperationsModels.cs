using System;
using System.Collections.Generic;

namespace EBSCore.Web.Models.Operations
{
    public class ServiceRequest
    {
        public long? ServiceRequestID { get; set; }
        public string RequestNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long? RequesterUserID { get; set; }
        public string RequestChannelCode { get; set; }
        public int? ServiceCategoryID { get; set; }
        public int? ServiceTypeID { get; set; }
        public string PriorityCode { get; set; }
        public string ImpactCode { get; set; }
        public string UrgencyCode { get; set; }
        public string StatusCode { get; set; }
        public int? AssignmentGroupID { get; set; }
        public long? AssignedUserID { get; set; }
        public int? OrganisationUnitID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? FirstRespondedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string ResolutionCode { get; set; }
        public string ClosureReasonCode { get; set; }
        public bool IsReopened { get; set; }
        public string SLAStatus { get; set; }
    }

    public class OperationsIncident
    {
        public long? IncidentID { get; set; }
        public string IncidentNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SeverityCode { get; set; }
        public string ImpactCode { get; set; }
        public string UrgencyCode { get; set; }
        public string PriorityCode { get; set; }
        public string StatusCode { get; set; }
        public int? ServiceCategoryID { get; set; }
        public int? AssignmentGroupID { get; set; }
        public long? AssignedUserID { get; set; }
        public int? OrganisationUnitID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? FirstRespondedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? ResponseTargetAt { get; set; }
        public DateTime? ResolutionTargetAt { get; set; }
        public string RootCauseNotes { get; set; }
        public string SLAStatus { get; set; }
    }

    public class OperationsWorkOrder
    {
        public long? WorkOrderID { get; set; }
        public string WorkOrderNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StatusCode { get; set; }
        public int? AssignmentGroupID { get; set; }
        public long? AssignedUserID { get; set; }
        public int? OrganisationUnitID { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal? ProgressPercent { get; set; }
        public string EvidenceUrl { get; set; }
    }

    public class OperationsDashboardFilter
    {
        public int? OrganisationUnitID { get; set; }
        public bool IncludeChildren { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? AssignmentGroupID { get; set; }
        public long? AssignedUserID { get; set; }
        public string PriorityCode { get; set; }
        public string StatusCode { get; set; }
    }

    public class OperationsKpi
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public decimal Value { get; set; }
        public string Unit { get; set; }
        public decimal? PreviousValue { get; set; }
        public string Trend { get; set; }
    }

    public class QueueBacklogRow
    {
        public int AssignmentGroupID { get; set; }
        public string QueueName { get; set; }
        public int OpenItems { get; set; }
        public int OverdueItems { get; set; }
        public int BreachedItems { get; set; }
        public decimal AvgAgeHours { get; set; }
    }

    public class OperationsDashboardResponse
    {
        public List<OperationsKpi> Kpis { get; set; } = new();
        public List<QueueBacklogRow> QueueBacklog { get; set; } = new();
        public List<ServiceRequest> OldestOpenServiceRequests { get; set; } = new();
        public List<OperationsIncident> CriticalIncidents { get; set; } = new();
        public List<OperationsWorkOrder> OverdueWorkOrders { get; set; } = new();
    }
}
