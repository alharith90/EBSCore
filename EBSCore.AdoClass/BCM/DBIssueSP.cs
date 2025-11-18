using EBSCore.AdoClass;
using Microsoft.Extensions.Configuration;
using static EBSCore.AdoClass.DBParentStoredProcedureClass;
using System.Collections;
using System.Data;

public class DBIssueSP : DBParentStoredProcedureClass
{
    public TableField Operation = new TableField("Operation", SqlDbType.NVarChar);
    public TableField CurrentUserID = new TableField("CurrentUserID", SqlDbType.Int);
    public TableField IssueID = new TableField("IssueID", SqlDbType.Int);

    public TableField Description = new TableField("Description", SqlDbType.NVarChar);
    public TableField Category = new TableField("Category", SqlDbType.NVarChar);
    public TableField Source = new TableField("Source", SqlDbType.NVarChar);
    public TableField Impact = new TableField("Impact", SqlDbType.NVarChar);
    public TableField DateIdentified = new TableField("DateIdentified", SqlDbType.Date);
    public TableField Owner = new TableField("Owner", SqlDbType.NVarChar);
    public TableField Status = new TableField("Status", SqlDbType.NVarChar);
    public TableField RootCause = new TableField("RootCause", SqlDbType.NVarChar);
    public TableField CorrectiveAction = new TableField("CorrectiveAction", SqlDbType.NVarChar);
    public TableField ActionOwner = new TableField("ActionOwner", SqlDbType.NVarChar);
    public TableField ActionDueDate = new TableField("ActionDueDate", SqlDbType.Date);
    public TableField ActionCompletionDate = new TableField("ActionCompletionDate", SqlDbType.Date);
    public TableField VerificationOfEffectiveness = new TableField("VerificationOfEffectiveness", SqlDbType.NVarChar);
    public TableField RelatedProcess = new TableField("RelatedProcess", SqlDbType.NVarChar);
    public TableField AuditReference = new TableField("AuditReference", SqlDbType.NVarChar);
    public TableField ReviewDate = new TableField("ReviewDate", SqlDbType.Date);
    public TableField RiskCategory = new TableField("RiskCategory", SqlDbType.NVarChar);
    public TableField Likelihood = new TableField("Likelihood", SqlDbType.Int);
    public TableField Consequence = new TableField("Consequence", SqlDbType.Int);
    public TableField DetectionMethod = new TableField("DetectionMethod", SqlDbType.NVarChar);
    public TableField IssueType = new TableField("IssueType", SqlDbType.NVarChar);
    public TableField LinkedBCP = new TableField("LinkedBCP", SqlDbType.NVarChar);
    public TableField LessonsLearned = new TableField("LessonsLearned", SqlDbType.NVarChar);
    public TableField IsRecurring = new TableField("IsRecurring", SqlDbType.Bit);
    public TableField MitigationActions = new TableField("MitigationActions", SqlDbType.NVarChar);
    public TableField EscalationLevel = new TableField("EscalationLevel", SqlDbType.NVarChar);
    public TableField ClosureApprovedBy = new TableField("ClosureApprovedBy", SqlDbType.NVarChar);
    public TableField ClosureComments = new TableField("ClosureComments", SqlDbType.NVarChar);

    public TableField PageSize = new TableField("PageSize", SqlDbType.Int);
    public TableField PageNumber = new TableField("PageNumber", SqlDbType.Int);
    public TableField SortColumn = new TableField("SortColumn", SqlDbType.NVarChar);
    public TableField SortDirection = new TableField("SortDirection", SqlDbType.NVarChar);
    public TableField SearchFields = new TableField("SearchFields", SqlDbType.NVarChar);
    public TableField SearchQuery = new TableField("SearchQuery", SqlDbType.NVarChar);

    public DBIssueSP(IConfiguration configuration) : base(configuration)
    {
        base.SPName = "IssueSP";
    }

    public new object QueryDatabase(
        SqlQueryType QueryType,
        string Operation = "", string CurrentUserID = "", string IssueID = "",
        string Description = "", string Category = "", string Source = "", string Impact = "", string DateIdentified = "",
        string Owner = "", string Status = "", string RootCause = "", string CorrectiveAction = "", string ActionOwner = "",
        string ActionDueDate = "", string ActionCompletionDate = "", string VerificationOfEffectiveness = "",
        string RelatedProcess = "", string AuditReference = "", string ReviewDate = "", string RiskCategory = "",
        string Likelihood = "", string Consequence = "", string DetectionMethod = "", string IssueType = "",
        string LinkedBCP = "", string LessonsLearned = "", string IsRecurring = "", string MitigationActions = "",
        string EscalationLevel = "", string ClosureApprovedBy = "", string ClosureComments = "",
        string PageSize = "", string PageNumber = "", string SortColumn = "", string SortDirection = "",
        string SearchFields = "", string SearchQuery = ""
    )
    {
        FieldsArrayList = new ArrayList();

        this.Operation.SetValue(Operation, ref FieldsArrayList);
        this.CurrentUserID.SetValue(CurrentUserID, ref FieldsArrayList);
        this.IssueID.SetValue(IssueID, ref FieldsArrayList);
        this.Description.SetValue(Description, ref FieldsArrayList);
        this.Category.SetValue(Category, ref FieldsArrayList);
        this.Source.SetValue(Source, ref FieldsArrayList);
        this.Impact.SetValue(Impact, ref FieldsArrayList);
        this.DateIdentified.SetValue(DateIdentified, ref FieldsArrayList);
        this.Owner.SetValue(Owner, ref FieldsArrayList);
        this.Status.SetValue(Status, ref FieldsArrayList);
        this.RootCause.SetValue(RootCause, ref FieldsArrayList);
        this.CorrectiveAction.SetValue(CorrectiveAction, ref FieldsArrayList);
        this.ActionOwner.SetValue(ActionOwner, ref FieldsArrayList);
        this.ActionDueDate.SetValue(ActionDueDate, ref FieldsArrayList);
        this.ActionCompletionDate.SetValue(ActionCompletionDate, ref FieldsArrayList);
        this.VerificationOfEffectiveness.SetValue(VerificationOfEffectiveness, ref FieldsArrayList);
        this.RelatedProcess.SetValue(RelatedProcess, ref FieldsArrayList);
        this.AuditReference.SetValue(AuditReference, ref FieldsArrayList);
        this.ReviewDate.SetValue(ReviewDate, ref FieldsArrayList);
        this.RiskCategory.SetValue(RiskCategory, ref FieldsArrayList);
        this.Likelihood.SetValue(Likelihood, ref FieldsArrayList);
        this.Consequence.SetValue(Consequence, ref FieldsArrayList);
        this.DetectionMethod.SetValue(DetectionMethod, ref FieldsArrayList);
        this.IssueType.SetValue(IssueType, ref FieldsArrayList);
        this.LinkedBCP.SetValue(LinkedBCP, ref FieldsArrayList);
        this.LessonsLearned.SetValue(LessonsLearned, ref FieldsArrayList);
        this.IsRecurring.SetValue(IsRecurring, ref FieldsArrayList);
        this.MitigationActions.SetValue(MitigationActions, ref FieldsArrayList);
        this.EscalationLevel.SetValue(EscalationLevel, ref FieldsArrayList);
        this.ClosureApprovedBy.SetValue(ClosureApprovedBy, ref FieldsArrayList);
        this.ClosureComments.SetValue(ClosureComments, ref FieldsArrayList);
        this.PageSize.SetValue(PageSize, ref FieldsArrayList);
        this.PageNumber.SetValue(PageNumber, ref FieldsArrayList);
        this.SortColumn.SetValue(SortColumn, ref FieldsArrayList);
        this.SortDirection.SetValue(SortDirection, ref FieldsArrayList);
        this.SearchFields.SetValue(SearchFields, ref FieldsArrayList);
        this.SearchQuery.SetValue(SearchQuery, ref FieldsArrayList);

        return base.QueryDatabase(QueryType);
    }
}
